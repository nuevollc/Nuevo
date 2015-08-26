namespace LTESftpMgrSvc
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
            this.LTESftpProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.LTESftpSvcInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            // LTESftpProcessInstaller1
            // 
            this.LTESftpProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.LTESftpProcessInstaller1.Password = null;
            this.LTESftpProcessInstaller1.Username = null;
            // 
            // LTESftpSvcInstaller1
            // 
            this.LTESftpSvcInstaller1.ServiceName = "Service1";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.LTESftpProcessInstaller1,
            this.LTESftpSvcInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller LTESftpProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller LTESftpSvcInstaller1;
    }
}