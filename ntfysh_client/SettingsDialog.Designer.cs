
using System;

namespace ntfysh_client
{
    partial class SettingsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            buttonPanel = new System.Windows.Forms.Panel();
            cancelButton = new System.Windows.Forms.Button();
            saveButton = new System.Windows.Forms.Button();
            timeoutLabel = new System.Windows.Forms.Label();
            timeout = new System.Windows.Forms.NumericUpDown();
            reconnectAttemptDelay = new System.Windows.Forms.NumericUpDown();
            reconnectAttemptDelayLabel = new System.Windows.Forms.Label();
            nativeNotificationAutoCopy = new System.Windows.Forms.CheckBox();
            checkBoxAutoStart = new System.Windows.Forms.CheckBox();
            checkBoxSilentStart = new System.Windows.Forms.CheckBox();
            buttonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)timeout).BeginInit();
            ((System.ComponentModel.ISupportInitialize)reconnectAttemptDelay).BeginInit();
            SuspendLayout();
            // 
            // buttonPanel
            // 
            resources.ApplyResources(buttonPanel, "buttonPanel");
            buttonPanel.BackColor = System.Drawing.SystemColors.Control;
            buttonPanel.Controls.Add(cancelButton);
            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Name = "buttonPanel";
            // 
            // cancelButton
            // 
            resources.ApplyResources(cancelButton, "cancelButton");
            cancelButton.Name = "cancelButton";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // saveButton
            // 
            resources.ApplyResources(saveButton, "saveButton");
            saveButton.Name = "saveButton";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // timeoutLabel
            // 
            resources.ApplyResources(timeoutLabel, "timeoutLabel");
            timeoutLabel.Name = "timeoutLabel";
            timeoutLabel.Click += timeoutLabel_Click;
            // 
            // timeout
            // 
            resources.ApplyResources(timeout, "timeout");
            timeout.Maximum = new decimal(new int[] { -1981284353, -1966660860, 0, 0 });
            timeout.Name = "timeout";
            // 
            // reconnectAttemptDelay
            // 
            resources.ApplyResources(reconnectAttemptDelay, "reconnectAttemptDelay");
            reconnectAttemptDelay.Maximum = new decimal(new int[] { -1981284353, -1966660860, 0, 0 });
            reconnectAttemptDelay.Name = "reconnectAttemptDelay";
            // 
            // reconnectAttemptDelayLabel
            // 
            resources.ApplyResources(reconnectAttemptDelayLabel, "reconnectAttemptDelayLabel");
            reconnectAttemptDelayLabel.Name = "reconnectAttemptDelayLabel";
            // 
            // nativeNotificationAutoCopy
            // 
            resources.ApplyResources(nativeNotificationAutoCopy, "nativeNotificationAutoCopy");
            nativeNotificationAutoCopy.Name = "nativeNotificationAutoCopy";
            nativeNotificationAutoCopy.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoStart
            // 
            resources.ApplyResources(checkBoxAutoStart, "checkBoxAutoStart");
            checkBoxAutoStart.Name = "checkBoxAutoStart";
            checkBoxAutoStart.UseVisualStyleBackColor = true;
            checkBoxAutoStart.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // checkBoxSilentStart
            // 
            resources.ApplyResources(checkBoxSilentStart, "checkBoxSilentStart");
            checkBoxSilentStart.Name = "checkBoxSilentStart";
            checkBoxSilentStart.UseVisualStyleBackColor = true;
            // 
            // 
            // SettingsDialog
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            Controls.Add(nativeNotificationAutoCopy);
            Controls.Add(checkBoxAutoStart);
            Controls.Add(checkBoxSilentStart);
            Controls.Add(reconnectAttemptDelay);
            Controls.Add(reconnectAttemptDelayLabel);
            Controls.Add(timeout);
            Controls.Add(timeoutLabel);
            Controls.Add(buttonPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            Load += SettingsDialog_Load;
            buttonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)timeout).EndInit();
            ((System.ComponentModel.ISupportInitialize)reconnectAttemptDelay).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }




        #endregion

        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Label timeoutLabel;
        private System.Windows.Forms.NumericUpDown timeout;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.NumericUpDown reconnectAttemptDelay;
        private System.Windows.Forms.Label reconnectAttemptDelayLabel;
        private System.Windows.Forms.CheckBox nativeNotificationAutoCopy;
        private System.Windows.Forms.CheckBox checkBoxAutoStart;
        private System.Windows.Forms.CheckBox checkBoxSilentStart;
    }
}
