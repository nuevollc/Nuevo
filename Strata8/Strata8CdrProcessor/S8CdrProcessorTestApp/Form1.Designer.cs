namespace S8CdrProcessorTestApp
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
            this.S8ProcessorStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // S8ProcessorStart
            // 
            this.S8ProcessorStart.Location = new System.Drawing.Point(78, 166);
            this.S8ProcessorStart.Name = "S8ProcessorStart";
            this.S8ProcessorStart.Size = new System.Drawing.Size(141, 32);
            this.S8ProcessorStart.TabIndex = 0;
            this.S8ProcessorStart.Text = "S8ProcessorStart";
            this.S8ProcessorStart.UseVisualStyleBackColor = true;
            this.S8ProcessorStart.Click += new System.EventHandler(this.S8ProcessorStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.S8ProcessorStart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button S8ProcessorStart;
    }
}

