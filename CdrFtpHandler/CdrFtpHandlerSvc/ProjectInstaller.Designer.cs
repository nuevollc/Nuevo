namespace CdrFtpHandlerSvc
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
            this.BWCdrFtpSvcProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.BWCdrFtpSvc = new System.ServiceProcess.ServiceInstaller();
            // 
            // BWCdrFtpSvcProcessInstaller
            // 
            this.BWCdrFtpSvcProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.BWCdrFtpSvcProcessInstaller.Password = null;
            this.BWCdrFtpSvcProcessInstaller.Username = null;
            this.BWCdrFtpSvcProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller1_AfterInstall);
            // 
            // BWCdrFtpSvc
            // 
            this.BWCdrFtpSvc.Description = "Cdr FTP Svc, BW to Logi";
            this.BWCdrFtpSvc.ServiceName = "BWCdrFtpSvc";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.BWCdrFtpSvcProcessInstaller,
            this.BWCdrFtpSvc});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller BWCdrFtpSvcProcessInstaller;
        private System.ServiceProcess.ServiceInstaller BWCdrFtpSvc;
    }
}