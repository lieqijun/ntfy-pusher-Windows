using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static ntfysh_client.SettingsModel;

namespace ntfysh_client
{
    public partial class SettingsDialog : Form
    {
        public decimal Timeout
        {
            get => timeout.Value;
            set => timeout.Value = value;
        }

        public decimal ReconnectAttempts
        {
            get => reconnectAttempts.Value;
            set => reconnectAttempts.Value = value;
        }

        public decimal ReconnectAttemptDelay
        {
            get => reconnectAttemptDelay.Value;
            set => reconnectAttemptDelay.Value = value;
        }

        #region: Native notifications option
        public bool UseNativeWindowsNotifications
        {
            get => useNativeWindowsNotifications.Checked;
            set => useNativeWindowsNotifications.Checked = value;
        }

        public bool AutoStartEnabled
        {
            get => checkBoxAutoStart.Checked;
            set => checkBoxAutoStart.Checked = value;
        }

        public bool AutoStartSilent
        {
            get => checkBoxSilentStart.Checked;
            set => checkBoxSilentStart.Checked = value;
        }
        #endregion

        #region: Native notification options
        public bool NativeNotificationsAutoCopyToClipboard
        {
            get => nativeNotificationAutoCopy.Checked;
            set => nativeNotificationAutoCopy.Checked = value;
        }
        #endregion

        public SettingsDialog()
        {
            InitializeComponent();
            SetNotificationsUiElements();
            checkBoxSilentStart.Enabled = checkBoxAutoStart.Checked;
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void SetNotificationsUiElements()
        {
            timeoutLabel.Text = _windowsNotificationsTimeout;
        }

        private const string _windowsNotificationsTimeout = "通知提示超时（秒，可能因系统辅助功能设置而被忽略）:";

        private void SettingsDialog_Load(object sender, EventArgs e)
        {

        }

        private void timeoutLabel_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxSilentStart.Enabled = checkBoxAutoStart.Checked;
        }

    }
}
