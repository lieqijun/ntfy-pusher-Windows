using System;
using System.Windows.Forms;
namespace ntfysh_client
{
    public partial class SubscribeDialog : Form
    {
        private readonly ListBox _notificationTopics;
        private readonly string _preFillServer;
        private readonly string _preFillToken;
        // 关键修复：根据复选框状态返回服务器地址
        public string TopicId => topicId.Text.Trim();
        public string ServerUrl
        {
            get
            {
                if (UseCustomServer)
                {
                    return serverUrl.Text.Trim();
                }
                // 不使用自定义服务器 → 直接返回传入的记忆服务器
                return _preFillServer;
            }
        }

        // ============ 修改这里：移除if判断，永远读取输入框真实值 ============
        public string BearerToken => txtBearerToken.Text.Trim();

        public bool UseCustomServer => chkUseCustomServer.Checked;
        public string Unique => $"{TopicId}@{ServerUrl}";
        public bool UseWebsockets
        {
            get
            {
                switch (connectionType.Text)
                {
                    case "Websockets (Recommended)":
                        return true;
                    case "Long HTTP JSON (Robust)":
                        return false;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
        public SubscribeDialog(ListBox notificationTopics, string preFillServer, string preFillToken)
        {
            _notificationTopics = notificationTopics;
            _preFillServer = preFillServer;
            _preFillToken = preFillToken;
            InitializeComponent();
        }
        private void SubscribeDialog_Load(object sender, EventArgs e)
        {
            connectionType.SelectedIndex = 0;
            // ========== 修改：默认不勾选【使用自定义服务器】 ==========
            chkUseCustomServer.Checked = false;
            // 面板里面的输入框依然回填记忆值，用户勾选后直接可见
            serverUrl.Text = _preFillServer;
            txtBearerToken.Text = _preFillToken;
        }
        private void chkUseCustomServer_CheckedChanged(object sender, EventArgs e)
        {
            panelCustomServer.Visible = chkUseCustomServer.Checked;
        }
        private bool ReparseAddress()
        {
            string[] parts = serverUrl.Text.Split("://", 2);
            if (parts.Length != 2) return false;
            string schema = parts[0].ToLower();
            switch (schema)
            {
                case "http":
                case "https":
                case "ws":
                case "wss":
                    break;
                default:
                    return false;
            }
            if (UseWebsockets)
            {
                switch (schema)
                {
                    case "http": schema = "ws"; break;
                    case "https": schema = "wss"; break;
                }
            }
            else
            {
                switch (schema)
                {
                    case "ws": schema = "http"; break;
                    case "wss": schema = "https"; break;
                }
            }
            string finalAddress = schema + "://" + parts[1];
            if (!Uri.IsWellFormedUriString(finalAddress, UriKind.Absolute))
                return false;
            serverUrl.Text = finalAddress;
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TopicId))
            {
                MessageBox.Show("You must specify a topic name.", "Topic name not specified", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                topicId.Focus();
                return;
            }
            // ========== 修改：只有勾选自定义服务器才校验输入框服务器 ==========
            if (UseCustomServer)
            {
                if (string.IsNullOrWhiteSpace(serverUrl.Text))
                {
                    MessageBox.Show("You must specify a server URL. The default is wss://ntfy.sh", "Server URL not specified", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    serverUrl.Focus();
                    return;
                }
            }
            if (_notificationTopics.Items.Contains(Unique))
            {
                MessageBox.Show($"The specified topic '{TopicId}' on the server '{ServerUrl}' is already subscribed", "Topic already subscribed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                topicId.Focus();
                return;
            }
            try
            {
                // 仅勾选自定义服务器的时候才解析地址
                if (UseCustomServer && !ReparseAddress())
                {
                    MessageBox.Show($"The specified server URL is invalid. Accepted schemas are: http:// https:// ws:// wss://", "Invalid Server URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    connectionType.Focus();
                    return;
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show($"The selected Connection Type '{connectionType.Text}' is invalid.", "Invalid Connection Type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                connectionType.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        private void topicId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button1.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
        private void serverUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button1.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
        private void txtBearerToken_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button1.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
        private void connectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UseCustomServer) ReparseAddress();
        }
    }
}
