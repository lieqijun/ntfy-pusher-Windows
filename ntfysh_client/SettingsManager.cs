using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ntfysh_client
{
    public static class SettingsManager
    {
        private static string GetSettingsFilePath()
        {
            string binaryDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                ?? throw new InvalidOperationException("无法确定应用程序目录");
            return Path.Combine(binaryDirectory, "settings.json");
        }

        private static SettingsModel GetDefaultSettings() => new()
        {
            Revision = 5,
            Timeout = 5,
            ReconnectAttemptDelay = 3,
            NativeNotificationsAutoCopyToClipboard = false,
            AutoStartEnabled = false,
            AutoStartSilent = false
        };

        private static void MergeSettingsRevisions(SettingsModel older, SettingsModel newer)
        {
            if (older.Revision < 3)
            {
                older.NativeNotificationsAutoCopyToClipboard = newer.NativeNotificationsAutoCopyToClipboard;
                older.AutoStartEnabled = newer.AutoStartEnabled;
            }
            if (older.Revision < 4)
            {
                older.AutoStartSilent = newer.AutoStartSilent;
            }
            older.Revision = newer.Revision;
        }

        public static SettingsModel LoadSettings()
        {
            string settingsFilePath = GetSettingsFilePath();
            SettingsModel defaultSettings = GetDefaultSettings();

            if (!File.Exists(settingsFilePath))
            {
                SaveSettings(defaultSettings);
                return defaultSettings;
            }

            string settingsSerialised = File.ReadAllText(settingsFilePath);
            if (string.IsNullOrWhiteSpace(settingsSerialised))
            {
                SaveSettings(defaultSettings);
                return defaultSettings;
            }

            SettingsModel? settings = JsonConvert.DeserializeObject<SettingsModel>(settingsSerialised);
            if (settings is null)
            {
                SaveSettings(defaultSettings);
                return defaultSettings;
            }

            if (settings.Revision < defaultSettings.Revision)
            {
                MergeSettingsRevisions(settings, defaultSettings);
                SaveSettings(settings);
            }

            return settings;
        }

        public static void SaveSettings(SettingsModel settings)
        {
            string settingsFilePath = GetSettingsFilePath();
            string settingsSerialised = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(settingsFilePath, settingsSerialised);
        }
    }
}
