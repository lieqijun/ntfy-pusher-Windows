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

        /// <summary>
        /// mTLS 客户端证书指纹（Thumbprint）。留空表示不发送客户端证书（兼容未开启 mTLS 的服务器）。
        /// 开启 mTLS 时填写，客户端会从 Windows 系统证书存储（个人/My）按该指纹查找含私钥的证书。
        /// </summary>
        public string ClientCertificateThumbprint { get; set; } = "";
    }
}
