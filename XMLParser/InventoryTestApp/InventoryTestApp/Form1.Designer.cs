namespace InventoryTestApp
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
            this.BeginParse = new System.Windows.Forms.Button();
            this.ParseVerizonTDS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BeginParse
            // 
            this.BeginParse.Location = new System.Drawing.Point(108, 139);
            this.BeginParse.Name = "BeginParse";
            this.BeginParse.Size = new System.Drawing.Size(75, 23);
            this.BeginParse.TabIndex = 0;
            this.BeginParse.Text = "BeginParse";
            this.BeginParse.UseVisualStyleBackColor = true;
            this.BeginParse.Click += new System.EventHandler(this.BeginParse_Click);
            // 
            // ParseVerizonTDS
            // 
            this.ParseVerizonTDS.Location = new System.Drawing.Point(89, 179);
            this.ParseVerizonTDS.Name = "ParseVerizonTDS";
            this.ParseVerizonTDS.Size = new System.Drawing.Size(123, 23);
            this.ParseVerizonTDS.TabIndex = 1;
            this.ParseVerizonTDS.Text = "ParseVerizonTDS";
            this.ParseVerizonTDS.UseVisualStyleBackColor = true;
            this.ParseVerizonTDS.Click += new System.EventHandler(this.ParseVerizonTDS_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.ParseVerizonTDS);
            this.Controls.Add(this.BeginParse);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BeginParse;
        private System.Windows.Forms.Button ParseVerizonTDS;
    }
}

