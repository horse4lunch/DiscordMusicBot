namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            urlTextBox = new TextBox();
            btnDownload = new Button();
            progressBar = new ProgressBar();
            txtOutput = new TextBox();
            btnBrowse = new Button();
            downloadDirTextBox = new TextBox();
            discordLog = new TextBox();
            btnPython = new Button();
            PyDirTextBox = new TextBox();
            SuspendLayout();
            // 
            // urlTextBox
            // 
            urlTextBox.BackColor = SystemColors.ActiveBorder;
            urlTextBox.Location = new Point(3, 41);
            urlTextBox.Name = "urlTextBox";
            urlTextBox.Size = new Size(334, 23);
            urlTextBox.TabIndex = 0;
            urlTextBox.Text = "Youtube Url or search query";
            // 
            // btnDownload
            // 
            btnDownload.Location = new Point(3, 12);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(145, 23);
            btnDownload.TabIndex = 1;
            btnDownload.Text = "Download";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(420, 12);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(368, 23);
            progressBar.TabIndex = 3;
            // 
            // txtOutput
            // 
            txtOutput.BackColor = SystemColors.ActiveBorder;
            txtOutput.Location = new Point(420, 41);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.ScrollBars = ScrollBars.Both;
            txtOutput.Size = new Size(368, 397);
            txtOutput.TabIndex = 4;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(3, 70);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(145, 23);
            btnBrowse.TabIndex = 5;
            btnBrowse.Text = "Download location";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // downloadDirTextBox
            // 
            downloadDirTextBox.BackColor = SystemColors.ActiveBorder;
            downloadDirTextBox.Location = new Point(3, 99);
            downloadDirTextBox.Name = "downloadDirTextBox";
            downloadDirTextBox.Size = new Size(334, 23);
            downloadDirTextBox.TabIndex = 6;
            // 
            // discordLog
            // 
            discordLog.BackColor = SystemColors.ActiveBorder;
            discordLog.Location = new Point(3, 185);
            discordLog.Multiline = true;
            discordLog.Name = "discordLog";
            discordLog.ScrollBars = ScrollBars.Both;
            discordLog.Size = new Size(411, 253);
            discordLog.TabIndex = 7;
            // 
            // btnPython
            // 
            btnPython.Location = new Point(3, 128);
            btnPython.Name = "btnPython";
            btnPython.Size = new Size(145, 23);
            btnPython.TabIndex = 8;
            btnPython.Text = "Python Directory";
            btnPython.UseVisualStyleBackColor = true;
            btnPython.Click += btnPython_Click;
            // 
            // PyDirTextBox
            // 
            PyDirTextBox.BackColor = SystemColors.ActiveBorder;
            PyDirTextBox.Location = new Point(3, 157);
            PyDirTextBox.Name = "PyDirTextBox";
            PyDirTextBox.Size = new Size(334, 23);
            PyDirTextBox.TabIndex = 9;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(800, 450);
            Controls.Add(PyDirTextBox);
            Controls.Add(btnPython);
            Controls.Add(discordLog);
            Controls.Add(downloadDirTextBox);
            Controls.Add(btnBrowse);
            Controls.Add(txtOutput);
            Controls.Add(progressBar);
            Controls.Add(btnDownload);
            Controls.Add(urlTextBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Music Bot";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox urlTextBox;
        private Button btnDownload;
        private ProgressBar progressBar;
        private TextBox txtOutput;
        private Button btnBrowse;
        private TextBox downloadDirTextBox;
        private TextBox discordLog;
        private Button btnPython;
        private TextBox PyDirTextBox;
    }
}
