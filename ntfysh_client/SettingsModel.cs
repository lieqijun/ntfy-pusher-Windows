namespace ntfysh_client
{
    public class SettingsModel
    {
        public uint Revision { get; set; }
        public decimal Timeout { get; set; }
        public decimal ReconnectAttemptDelay { get; set; }
        public bool NativeNotificationsAutoCopyToClipboard { get; set; }
        public bool AutoStartEnabled { get; set; }
        public bool AutoStartSilent { get; set; }

        public string LastUsedServer { get; set; } = "wss://ntfy.sh";
        public string LastUsedBearerToken { get; set; } = "";
    }
}
