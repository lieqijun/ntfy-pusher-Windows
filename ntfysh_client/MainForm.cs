using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Newtonsoft.Json;
using ntfysh_client.Notifications;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using System.Globalization;

namespace ntfysh_client
{
    public partial class MainForm : Form
    {
        private readonly NotificationListener _notificationListener;
        private bool _startInTray;
        private bool _trueExit;
        private readonly Dictionary<string, string> _toastMessages = new Dictionary<string, string>();

        public MainForm(NotificationListener notificationListener, bool startInTray = false)
        {
            _notificationListener = notificationListener;
            _startInTray = startInTray;
            _notificationListener.OnNotificationReceive += OnNotificationReceive;
            _notificationListener.OnConnectionMultiAttemptFailure += OnConnectionMultiAttemptFailure;
            _notificationListener.OnConnectionCredentialsFailure += OnConnectionCredentialsFailure;
            ToastNotificationManagerCompat.OnActivated += OnToastActivated;
            InitializeComponent();
        }

        private void OnToastActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            this.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                var args = ToastArguments.Parse(e.Argument);
                if (args.TryGetValue("action", out string action) && action == "copy")
                {
                    if (args.TryGetValue("tag", out string tag) && _toastMessages.TryGetValue(tag, out string message))
                    {
                        try
                        {
                            Clipboard.SetText(message);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"复制失败: {ex.Message}");
                        }
                        finally
                        {
                            _toastMessages.Remove(tag);
                        }
                    }
                }
            });
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon.Dispose();
            ToastNotificationManagerCompat.OnActivated -= OnToastActivated;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadTopics();
        }

        protected override void SetVisibleCore(bool value)
        {
            if (_startInTray)
            {
                _startInTray = false;
                Opacity = 0;
                base.SetVisibleCore(true);
                base.SetVisibleCore(false);
                Opacity = 1;
                return;
            }
            base.SetVisibleCore(value);
        }

        private void CopyToClipboard(string text)
        {
            try
            {
                Thread clipboardThread = new Thread(() =>
                {
                    try
                    {
                        Clipboard.SetText(text);
                        Debug.WriteLine($"复制到剪贴板: {text}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"复制失败: {ex.Message}");
                    }
                });
                clipboardThread.SetApartmentState(ApartmentState.STA);
                clipboardThread.IsBackground = true;
                clipboardThread.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"创建复制线程失败: {ex.Message}");
            }
        }

        private void OnNotificationReceive(object sender, NotificationReceiveEventArgs e)
        {
            ToolTipIcon priorityIcon = e.Priority switch
            {
                NotificationPriority.Max => ToolTipIcon.Error,
                NotificationPriority.High => ToolTipIcon.Warning,
                NotificationPriority.Default => ToolTipIcon.Info,
                NotificationPriority.Low => ToolTipIcon.Info,
                NotificationPriority.Min => ToolTipIcon.None,
                _ => throw new ArgumentOutOfRangeException("Unknown priority received")
            };

            string finalTitle = string.IsNullOrWhiteSpace(e.Title) ? $"{e.Sender.TopicId}@{e.Sender.ServerUrl}" : e.Title;
            string tag = Guid.NewGuid().ToString();
            _toastMessages[tag] = e.Message;
            if (Program.Settings.NativeNotificationsAutoCopyToClipboard)
            {
                CopyToClipboard(e.Message);
            }
            string buttonText = Program.Settings.NativeNotificationsAutoCopyToClipboard
                ? "已自动复制到剪切板"
                : "复制内容";

            new ToastContentBuilder()
                .AddText(finalTitle)
                .AddText(e.Message)
                .AddButton(new ToastButton()
                    .SetContent(buttonText)
                    .AddArgument("action", "copy")
                    .AddArgument("tag", tag)
                    .SetBackgroundActivation())
                .SetToastDuration(ToastDuration.Short)
                .SetToastScenario(priorityIcon == ToolTipIcon.Error ? ToastScenario.Alarm : ToastScenario.Default)
                .Show(toast =>
                {
                    double timeoutSeconds = (double)Convert.ToDecimal(Program.Settings.Timeout);
                    toast.ExpirationTime = DateTime.Now.AddSeconds(timeoutSeconds);
                });
        }

        private void OnConnectionMultiAttemptFailure(NotificationListener sender, SubscribedTopic topic)
        {
        }

        private void OnConnectionCredentialsFailure(NotificationListener sender, SubscribedTopic topic)
        {
        }

        private void subscribeNewTopic_Click(object sender, EventArgs e)
        {
            using SubscribeDialog dialog = new SubscribeDialog(无订阅的主题, Program.Settings.LastUsedServer, Program.Settings.LastUsedBearerToken);
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;

            // 服务器地址仅勾选自定义服务器时才更新记忆
            if (dialog.UseCustomServer)
            {
                Program.Settings.LastUsedServer = dialog.ServerUrl;
            }
            // Token 不管有没有勾选自定义服务器，都记忆保存
            Program.Settings.LastUsedBearerToken = dialog.BearerToken;
            SettingsManager.SaveSettings(Program.Settings);


            int reconnectAttempts = 0; // 0 = 无限重连，断开后一直尝试重连
            int reconnectAttemptDelay = Convert.ToInt32(Math.Ceiling(Program.Settings.ReconnectAttemptDelay));

            string uniqueId = dialog.Unique;
            string topicId = dialog.TopicId;
            string serverUrl = dialog.ServerUrl;
            string? bearerToken = string.IsNullOrWhiteSpace(dialog.BearerToken) ? null : dialog.BearerToken;

            if (dialog.UseWebsockets)
            {
                _notificationListener.SubscribeToTopicUsingWebsocket(uniqueId, topicId, serverUrl, bearerToken, reconnectAttempts, reconnectAttemptDelay);
            }
            else
            {
                _notificationListener.SubscribeToTopicUsingLongHttpJson(uniqueId, topicId, serverUrl, bearerToken, reconnectAttempts, reconnectAttemptDelay);
            }

            无订阅的主题.Items.Add(uniqueId);
            SaveTopicsToFile();
        }

        private async void removeSelectedTopics_Click(object sender, EventArgs e)
        {
            while (无订阅的主题.SelectedIndex > -1)
            {
                string topicUniqueString = (string)无订阅的主题.Items[无订阅的主题.SelectedIndex];
                await _notificationListener.UnsubscribeFromTopicAsync(topicUniqueString);
                无订阅的主题.Items.Remove(topicUniqueString);
            }
            SaveTopicsToFile();
        }

        private void UpdateAutoStart()
        {
            const string runKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            string appName = "ntfysh_client";
            string execPath = Application.ExecutablePath;
            using RegistryKey? rk = Registry.CurrentUser.OpenSubKey(runKeyPath, writable: true);
            if (rk == null) return;

            if (Program.Settings.AutoStartEnabled)
            {
                string launchCommand = $"\"{execPath}\"";
                if (Program.Settings.AutoStartSilent)
                {
                    launchCommand += " --start-in-tray";
                }
                rk.SetValue(appName, launchCommand);
            }
            else
            {
                try
                {
                    rk.DeleteValue(appName, throwOnMissingValue: false);
                }
                catch
                {
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using SettingsDialog dialog = new();
            dialog.ReconnectAttemptDelay = Program.Settings.ReconnectAttemptDelay;
            dialog.Timeout = Program.Settings.Timeout;
            dialog.NativeNotificationsAutoCopyToClipboard = Program.Settings.NativeNotificationsAutoCopyToClipboard;
            dialog.AutoStartEnabled = Program.Settings.AutoStartEnabled;
            dialog.AutoStartSilent = Program.Settings.AutoStartSilent;

            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;

            Program.Settings.Timeout = dialog.Timeout;
            Program.Settings.ReconnectAttemptDelay = dialog.ReconnectAttemptDelay;
            Program.Settings.NativeNotificationsAutoCopyToClipboard = dialog.NativeNotificationsAutoCopyToClipboard;
            Program.Settings.AutoStartEnabled = dialog.AutoStartEnabled;
            Program.Settings.AutoStartSilent = dialog.AutoStartSilent;

            UpdateAutoStart();
            SettingsManager.SaveSettings(Program.Settings);
        }

        private void notificationTopics_SelectedValueChanged(object sender, EventArgs e)
        {
            removeSelectedTopics.Enabled = 无订阅的主题.SelectedIndices.Count > 0;
        }

        private void notificationTopics_Click(object sender, EventArgs e)
        {
            MouseEventArgs ev = (MouseEventArgs)e;
            int clickedItemIndex = 无订阅的主题.IndexFromPoint(new Point(ev.X, ev.Y));
            if (clickedItemIndex == -1) 无订阅的主题.ClearSelected();
        }

        private void notificationTopics_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            int clickedItemIndex = 无订阅的主题.IndexFromPoint(e.Location);
            if (clickedItemIndex == -1)
            {
                无订阅的主题.ClearSelected();
                return;
            }
            无订阅的主题.SelectedIndex = clickedItemIndex;
        }

        private void topicsContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = 无订阅的主题.SelectedIndex == -1;
        }

        private void copyTopicNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (无订阅的主题.SelectedItem is not string topicText) return;
            int atIndex = topicText.IndexOf('@');
            string topicName = atIndex < 0 ? topicText : topicText.Substring(0, atIndex);
            CopyToClipboard(topicName);
        }

        private void copyTopicFullAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (无订阅的主题.SelectedItem is string topicText)
            {
                CopyToClipboard(topicText);
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouseEv = (MouseEventArgs)e;
            if (mouseEv.Button != MouseButtons.Left) return;
            Visible = !Visible;
            BringToFront();
        }

        private void showControlWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible = true;
            BringToFront();
        }

        private string GetTopicsFilePath()
        {
            string binaryDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException("Unable to determine path for application");
            return Path.Combine(binaryDirectory ?? throw new InvalidOperationException("Unable to determine path for topics file"), "topics.json");
        }

        private string GetLegacyTopicsFilePath()
        {
            string binaryDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException("Unable to determine path for application");
            return Path.Combine(binaryDirectory ?? throw new InvalidOperationException("Unable to determine path for legacy topics file"), "topics.txt");
        }

        private void SaveTopicsToFile()
        {
            string topicsSerialised = JsonConvert.SerializeObject(_notificationListener.SubscribedTopicsByUnique.Select(st => st.Value).ToList(), Formatting.Indented);
            File.WriteAllText(GetTopicsFilePath(), topicsSerialised);
        }

        private void LoadTopics()
        {
            string legacyTopicsPath = GetLegacyTopicsFilePath();
            string topicsFilePath = GetTopicsFilePath();

            if (File.Exists(legacyTopicsPath))
            {
                List<string> legacyTopics = new List<string>();
                using (StreamReader reader = new StreamReader(legacyTopicsPath))
                {
                    while (!reader.EndOfStream)
                    {
                        string? legacyTopic = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(legacyTopic)) legacyTopics.Add(legacyTopic);
                    }
                }
                // 修复：只传两个参数，适配新版SubscribedTopic构造
                List<SubscribedTopic> newTopics = legacyTopics.Select(lt => new SubscribedTopic(lt, "https://ntfy.sh")).ToList();
                string newFormatSerialised = JsonConvert.SerializeObject(newTopics, Formatting.Indented);
                File.WriteAllText(topicsFilePath, newFormatSerialised);
                File.Delete(legacyTopicsPath);
            }

            if (!File.Exists(topicsFilePath)) return;

            string topicsSerialised = File.ReadAllText(topicsFilePath);
            if (string.IsNullOrWhiteSpace(topicsSerialised))
            {
                File.Delete(topicsFilePath);
                return;
            }

            List<SubscribedTopic>? topics = JsonConvert.DeserializeObject<List<SubscribedTopic>>(topicsSerialised);
            if (topics is null)
            {
                return;
            }

            int reconnectAttempts = 0; // 0 = 无限重连，断开后一直尝试重连
            int reconnectAttemptDelay = Convert.ToInt32(Math.Ceiling(Program.Settings.ReconnectAttemptDelay));

            foreach (SubscribedTopic topic in topics)
            {
                string[] parts = topic.ServerUrl.Split("://", 2);
                switch (parts[0].ToLower())
                {
                    case "ws":
                    case "wss":
                        _notificationListener.SubscribeToTopicUsingWebsocket(
                            $"{topic.TopicId}@{topic.ServerUrl}",
                            topic.TopicId,
                            topic.ServerUrl,
                            topic.BearerToken,
                            reconnectAttempts,
                            reconnectAttemptDelay);
                        break;
                    case "http":
                    case "https":
                        _notificationListener.SubscribeToTopicUsingLongHttpJson(
                            $"{topic.TopicId}@{topic.ServerUrl}",
                            topic.TopicId,
                            topic.ServerUrl,
                            topic.BearerToken,
                            reconnectAttempts,
                            reconnectAttemptDelay);
                        break;
                    default:
                        continue;
                }
                无订阅的主题.Items.Add($"{topic.TopicId}@{topic.ServerUrl}");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_trueExit) return;
            if (e.CloseReason != CloseReason.UserClosing) return;
            Visible = false;
            e.Cancel = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _trueExit = true;
            Close();
        }

        private void notificationTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }

    public static class LocalizationHelper
    {
        public static void ApplyLanguage(string cultureName)
        {
            try
            {
                var culture = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
            catch (CultureNotFoundException)
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            }
        }

        private static ResourceManager _globalResMgr =
            new ResourceManager("ntfysh_client.Properties.Resources", Assembly.GetExecutingAssembly());

        public static string GetGlobalString(string key)
        {
            try
            {
                return _globalResMgr.GetString(key);
            }
            catch
            {
                return $"[{key}]";
            }
        }
    }
}
