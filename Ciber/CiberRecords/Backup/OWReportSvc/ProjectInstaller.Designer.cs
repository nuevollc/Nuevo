namespace OWReportSvc
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ServiceProcess.ServiceInstaller serviceInstaller1;
            this.OWReportProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceInstaller1
            // 
            serviceInstaller1.ServiceName = "OWReportSvc";
            serviceInstaller1.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller1_AfterInstall);
            // 
            // OWReportProcessInstaller1
            // 
            this.OWReportProcessInstaller1.Password = null;
            this.OWReportProcessInstaller1.Username = null;
            this.OWReportProcessInstaller1.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.OWReportProcessInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.OWReportProcessInstaller1,
            serviceInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller OWReportProcessInstaller1;
    }
}