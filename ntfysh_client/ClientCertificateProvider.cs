using System;
using System.Security.Cryptography.X509Certificates;

namespace ntfysh_client
{
    /// <summary>
    /// 解析 mTLS 所需的客户端证书。
    /// 仅从 Windows 系统证书存储（当前用户 / 本地计算机“个人”存储）按指纹（Thumbprint）读取。
    /// 指纹为空时表示不发送客户端证书（兼容未开启 mTLS 的服务器）。
    /// </summary>
    public static class ClientCertificateProvider
    {
        /// <summary>
        /// 按设置中的指纹解析客户端证书；若未填写或解析失败则返回 null（不发送客户端证书）。
        /// </summary>
        public static X509Certificate2? Resolve(SettingsModel settings)
        {
            if (settings is null) return null;
            return LoadFromStore(settings.ClientCertificateThumbprint);
        }

        private static X509Certificate2? LoadFromStore(string thumbprint)
        {
            if (string.IsNullOrWhiteSpace(thumbprint)) return null;

            string wanted = NormalizeThumbprint(thumbprint);
            if (wanted.Length == 0) return null;

            // 优先当前用户“个人”存储，其次本地计算机
            StoreLocation[] locations = { StoreLocation.CurrentUser, StoreLocation.LocalMachine };
            foreach (StoreLocation location in locations)
            {
                using var store = new X509Store(StoreName.My, location);
                try
                {
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                }
                catch
                {
                    continue;
                }

                foreach (X509Certificate2 cert in store.Certificates)
                {
                    if (string.Equals(NormalizeThumbprint(cert.Thumbprint), wanted, StringComparison.Ordinal))
                    {
                        return cert;
                    }
                }
            }

            return null;
        }

        private static string NormalizeThumbprint(string? thumbprint)
        {
            if (thumbprint is null) return string.Empty;
            var sb = new System.Text.StringBuilder(thumbprint.Length);
            foreach (char c in thumbprint)
            {
                if (c != ' ' && c != '-') sb.Append(char.ToUpperInvariant(c));
            }
            return sb.ToString();
        }
    }
}
