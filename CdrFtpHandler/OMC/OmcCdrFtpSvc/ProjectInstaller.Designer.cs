namespace OmcCdrFtpSvc
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
            this.OmcCdrFtpSvcInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.OmcCdrFtpSvc = new System.ServiceProcess.ServiceInstaller();
            // 
            // OmcCdrFtpSvcInstaller
            // 
            this.OmcCdrFtpSvcInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.OmcCdrFtpSvcInstaller.Password = null;
            this.OmcCdrFtpSvcInstaller.Username = null;
            // 
            // OmcCdrFtpSvc
            // 
            this.OmcCdrFtpSvc.Description = "Cdr FTP Svc, OMC to Logi";
            this.OmcCdrFtpSvc.ServiceName = "OmcCdrFtpsvc";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.OmcCdrFtpSvcInstaller,
            this.OmcCdrFtpSvc});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller OmcCdrFtpSvcInstaller;
        private System.ServiceProcess.ServiceInstaller OmcCdrFtpSvc;
    }
}