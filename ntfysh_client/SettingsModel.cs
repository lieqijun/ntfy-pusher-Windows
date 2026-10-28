namespace ntfysh_client
{
    public class SettingsModel
    {
        public uint Revision { get; set; }
        public decimal Timeout { get; set; }
        public decimal ReconnectAttempts { get; set; }
        public decimal ReconnectAttemptDelay { get; set; }
        public bool NativeNotificationsAutoCopyToClipboard { get; set; }
        public bool AutoStartEnabled { get; set; }
        public bool AutoStartSilent { get; set; }

        // 记忆上次订阅服务器与BearerToken
        public string LastUsedServer { get; set; } = "https://ntfy.sh";
        public string LastUsedBearerToken { get; set; } = "";
    }
}
