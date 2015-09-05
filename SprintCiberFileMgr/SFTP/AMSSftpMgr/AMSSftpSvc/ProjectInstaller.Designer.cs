namespace AMSSftpSvc
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
            this.AMSSftpProjectInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.AMSSftpSvcInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // AMSSftpProjectInstaller
            // 
            this.AMSSftpProjectInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.AMSSftpProjectInstaller.Password = null;
            this.AMSSftpProjectInstaller.Username = null;
            // 
            // AMSSftpSvcInstaller
            // 
            this.AMSSftpSvcInstaller.ServiceName = "AMSSftpSvc";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.AMSSftpProjectInstaller,
            this.AMSSftpSvcInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller AMSSftpProjectInstaller;
        private System.ServiceProcess.ServiceInstaller AMSSftpSvcInstaller;
    }
}