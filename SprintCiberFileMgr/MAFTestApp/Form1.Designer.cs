namespace TruMobility.Network.Services
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartProcessor = new System.Windows.Forms.Button();
            this.StopProcessor = new System.Windows.Forms.Button();
            this.GetCallDirection = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StartProcessor
            // 
            this.StartProcessor.Location = new System.Drawing.Point(97, 69);
            this.StartProcessor.Name = "StartProcessor";
            this.StartProcessor.Size = new System.Drawing.Size(135, 47);
            this.StartProcessor.TabIndex = 0;
            this.StartProcessor.Text = "StartProcessor";
            this.StartProcessor.UseVisualStyleBackColor = true;
            this.StartProcessor.Click += new System.EventHandler(this.StartProcessor_Click);
            // 
            // StopProcessor
            // 
            this.StopProcessor.Location = new System.Drawing.Point(97, 137);
            this.StopProcessor.Name = "StopProcessor";
            this.StopProcessor.Size = new System.Drawing.Size(135, 45);
            this.StopProcessor.TabIndex = 1;
            this.StopProcessor.Text = "StopProcessor";
            this.StopProcessor.UseVisualStyleBackColor = true;
            this.StopProcessor.Click += new System.EventHandler(this.StopProcessor_Click);
            // 
            // GetCallDirection
            // 
            this.GetCallDirection.Location = new System.Drawing.Point(107, 213);
            this.GetCallDirection.Name = "GetCallDirection";
            this.GetCallDirection.Size = new System.Drawing.Size(113, 23);
            this.GetCallDirection.TabIndex = 2;
            this.GetCallDirection.Text = "GetCallDirection";
            this.GetCallDirection.UseVisualStyleBackColor = true;
            this.GetCallDirection.Click += new System.EventHandler(this.GetCallDirection_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.GetCallDirection);
            this.Controls.Add(this.StopProcessor);
            this.Controls.Add(this.StartProcessor);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button StartProcessor;
        private System.Windows.Forms.Button StopProcessor;
        private System.Windows.Forms.Button GetCallDirection;
    }
}

