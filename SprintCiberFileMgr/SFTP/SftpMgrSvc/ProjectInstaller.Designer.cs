namespace WindowsService1
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
            this.MAFSftpSvcProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.MAFSftpSvcInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // MAFSftpSvcProcessInstaller1
            // 
            this.MAFSftpSvcProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.MAFSftpSvcProcessInstaller1.Password = null;
            this.MAFSftpSvcProcessInstaller1.Username = null;
            // 
            // MAFSftpSvcInstaller
            // 
            this.MAFSftpSvcInstaller.ServiceName = "MAFSftpMgrSvc";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MAFSftpSvcProcessInstaller1,
            this.MAFSftpSvcInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller MAFSftpSvcProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller MAFSftpSvcInstaller;
    }
}