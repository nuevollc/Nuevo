namespace LDProcessorTestApp
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
            this.GetLDMins = new System.Windows.Forms.Button();
            this.GetWebResponse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GetLDMins
            // 
            this.GetLDMins.Location = new System.Drawing.Point(123, 99);
            this.GetLDMins.Name = "GetLDMins";
            this.GetLDMins.Size = new System.Drawing.Size(75, 23);
            this.GetLDMins.TabIndex = 0;
            this.GetLDMins.Text = "GetLDMins";
            this.GetLDMins.UseVisualStyleBackColor = true;
            this.GetLDMins.Click += new System.EventHandler(this.GetLDMins_Click);
            // 
            // GetWebResponse
            // 
            this.GetWebResponse.Location = new System.Drawing.Point(77, 170);
            this.GetWebResponse.Name = "GetWebResponse";
            this.GetWebResponse.Size = new System.Drawing.Size(121, 23);
            this.GetWebResponse.TabIndex = 1;
            this.GetWebResponse.Text = "GetWebResponse";
            this.GetWebResponse.UseVisualStyleBackColor = true;
            this.GetWebResponse.Click += new System.EventHandler(this.GetWebResponse_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.GetWebResponse);
            this.Controls.Add(this.GetLDMins);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button GetLDMins;
        private System.Windows.Forms.Button GetWebResponse;
    }
}

