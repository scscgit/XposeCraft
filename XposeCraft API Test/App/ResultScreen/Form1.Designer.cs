namespace XposeCraft_UI_API_Prototype_Test
{
	partial class Form1
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

		/// <summary>
		/// Logs an entry into a read-only textbox.
		/// </summary>
		/// <param name="context">where the information originated</param>
		/// <param name="text">logged information</param>
		public void Log(object context, string text)
		{
			var beginning = context == null ? "" : (context + ": ");
			log.AppendText(beginning + text + "\n");
			if (autoScrollCheckBox.Checked)
			{
				log.ScrollToCaret();
			}
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.log = new System.Windows.Forms.RichTextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.autoScrollCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(605, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "XposeCraft UI API Prototype Test Results";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.Text = "notifyIcon1";
			this.notifyIcon1.Visible = true;
			// 
			// log
			// 
			this.log.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.log.BackColor = System.Drawing.SystemColors.HighlightText;
			this.log.Location = new System.Drawing.Point(0, 99);
			this.log.MaximumSize = new System.Drawing.Size(600, 1000);
			this.log.MinimumSize = new System.Drawing.Size(600, 50);
			this.log.Name = "log";
			this.log.ReadOnly = true;
			this.log.Size = new System.Drawing.Size(600, 361);
			this.log.TabIndex = 1;
			this.log.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(216, 47);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(175, 36);
			this.button1.TabIndex = 2;
			this.button1.Text = "Run Test";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// autoScrollCheckBox
			// 
			this.autoScrollCheckBox.AutoSize = true;
			this.autoScrollCheckBox.Location = new System.Drawing.Point(411, 58);
			this.autoScrollCheckBox.Name = "autoScrollCheckBox";
			this.autoScrollCheckBox.Size = new System.Drawing.Size(74, 17);
			this.autoScrollCheckBox.TabIndex = 3;
			this.autoScrollCheckBox.Text = "AutoScroll";
			this.autoScrollCheckBox.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(604, 461);
			this.Controls.Add(this.autoScrollCheckBox);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.log);
			this.Controls.Add(this.label1);
			this.MaximumSize = new System.Drawing.Size(620, 900);
			this.MinimumSize = new System.Drawing.Size(620, 200);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.RichTextBox log;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.CheckBox autoScrollCheckBox;
	}
}

