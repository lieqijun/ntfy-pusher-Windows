namespace ntfysh_client
{
    partial class SubscribeDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.topicId = new System.Windows.Forms.TextBox();
            this.connectionType = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.labelTopicId = new System.Windows.Forms.Label();
            this.labelConnectionType = new System.Windows.Forms.Label();
            this.chkUseCustomServer = new System.Windows.Forms.CheckBox();
            this.panelCustomServer = new System.Windows.Forms.Panel();
            this.labelBearerToken = new System.Windows.Forms.Label();
            this.txtBearerToken = new System.Windows.Forms.TextBox();
            this.labelServerUrl = new System.Windows.Forms.Label();
            this.serverUrl = new System.Windows.Forms.TextBox();
            this.panelCustomServer.SuspendLayout();
            this.SuspendLayout();
            
            // topicId
            this.topicId.Location = new System.Drawing.Point(16, 36);
            this.topicId.Name = "topicId";
            this.topicId.Size = new System.Drawing.Size(420, 21);
            this.topicId.TabIndex = 0;
            this.topicId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.topicId_KeyDown);
            
            // labelTopicId
            this.labelTopicId.AutoSize = true;
            this.labelTopicId.Location = new System.Drawing.Point(13, 20);
            this.labelTopicId.Name = "labelTopicId";
            this.labelTopicId.Size = new System.Drawing.Size(77, 12);
            this.labelTopicId.TabIndex = 1;
            this.labelTopicId.Text = "Topic 主题：";
            
            // chkUseCustomServer
            this.chkUseCustomServer.AutoSize = true;
            this.chkUseCustomServer.Location = new System.Drawing.Point(16, 64);
            this.chkUseCustomServer.Name = "chkUseCustomServer";
            this.chkUseCustomServer.Size = new System.Drawing.Size(120, 16);
            this.chkUseCustomServer.TabIndex = 2;
            this.chkUseCustomServer.Text = "使用自定义服务器";
            this.chkUseCustomServer.UseVisualStyleBackColor = true;
            this.chkUseCustomServer.CheckedChanged += new System.EventHandler(this.chkUseCustomServer_CheckedChanged);
            
            // panelCustomServer
            this.panelCustomServer.Controls.Add(this.labelBearerToken);
            this.panelCustomServer.Controls.Add(this.txtBearerToken);
            this.panelCustomServer.Controls.Add(this.labelServerUrl);
            this.panelCustomServer.Controls.Add(this.serverUrl);
            this.panelCustomServer.Location = new System.Drawing.Point(16, 86);
            this.panelCustomServer.Name = "panelCustomServer";
            this.panelCustomServer.Size = new System.Drawing.Size(420, 110);
            this.panelCustomServer.TabIndex = 3;
            this.panelCustomServer.Visible = false;
            
            // labelServerUrl
            this.labelServerUrl.AutoSize = true;
            this.labelServerUrl.Location = new System.Drawing.Point(4, 8);
            this.labelServerUrl.Name = "labelServerUrl";
            this.labelServerUrl.Size = new System.Drawing.Size(89, 12);
            this.labelServerUrl.TabIndex = 0;
            this.labelServerUrl.Text = "服务器地址：";
            
            // serverUrl
            this.serverUrl.Location = new System.Drawing.Point(7, 24);
            this.serverUrl.Name = "serverUrl";
            this.serverUrl.Size = new System.Drawing.Size(402, 21);
            this.serverUrl.TabIndex = 1;
            this.serverUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.serverUrl_KeyDown);
            
            // labelBearerToken
            this.labelBearerToken.AutoSize = true;
            this.labelBearerToken.Location = new System.Drawing.Point(4, 52);
            this.labelBearerToken.Name = "labelBearerToken";
            this.labelBearerToken.Size = new System.Drawing.Size(83, 12);
            this.labelBearerToken.TabIndex = 2;
            this.labelBearerToken.Text = "Bearer Token：";
            
            // txtBearerToken
            this.txtBearerToken.Location = new System.Drawing.Point(7, 68);
            this.txtBearerToken.Name = "txtBearerToken";
            this.txtBearerToken.PasswordChar = '*';
            this.txtBearerToken.Size = new System.Drawing.Size(402, 21);
            this.txtBearerToken.TabIndex = 3;
            this.txtBearerToken.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBearerToken_KeyDown);
            
            // labelConnectionType
            this.labelConnectionType.AutoSize = true;
            this.labelConnectionType.Location = new System.Drawing.Point(13, 204);
            this.labelConnectionType.Name = "labelConnectionType";
            this.labelConnectionType.Size = new System.Drawing.Size(89, 12);
            this.labelConnectionType.TabIndex = 4;
            this.labelConnectionType.Text = "连接方式：";
            
            // connectionType
            this.connectionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.connectionType.FormattingEnabled = true;
            this.connectionType.Items.AddRange(new object[] {
            "Websockets (Recommended)",
            "Long HTTP JSON (Robust)"});
            this.connectionType.Location = new System.Drawing.Point(16, 220);
            this.connectionType.Name = "connectionType";
            this.connectionType.Size = new System.Drawing.Size(420, 20);
            this.connectionType.TabIndex = 5;
            this.connectionType.SelectedIndexChanged += new System.EventHandler(this.connectionType_SelectedIndexChanged);
            
            // button1
            this.button1.Location = new System.Drawing.Point(260, 256);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 28);
            this.button1.TabIndex = 6;
            this.button1.Text = "订阅";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            
            // button2
            this.button2.Location = new System.Drawing.Point(351, 256);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(85, 28);
            this.button2.TabIndex = 7;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            
            // SubscribeDialog
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 296);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.connectionType);
            this.Controls.Add(this.labelConnectionType);
            this.Controls.Add(this.panelCustomServer);
            this.Controls.Add(this.chkUseCustomServer);
            this.Controls.Add(this.labelTopicId);
            this.Controls.Add(this.topicId);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubscribeDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "添加订阅";
            this.Load += new System.EventHandler(this.SubscribeDialog_Load);
            this.panelCustomServer.ResumeLayout(false);
            this.panelCustomServer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.TextBox topicId;
        private System.Windows.Forms.Label labelTopicId;
        private System.Windows.Forms.CheckBox chkUseCustomServer;
        private System.Windows.Forms.Panel panelCustomServer;
        private System.Windows.Forms.Label labelServerUrl;
        private System.Windows.Forms.TextBox serverUrl;
        private System.Windows.Forms.Label labelBearerToken;
        private System.Windows.Forms.TextBox txtBearerToken;
        private System.Windows.Forms.Label labelConnectionType;
        private System.Windows.Forms.ComboBox connectionType;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}
