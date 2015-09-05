namespace DailyUserCallReportSvc
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
            this.userDailyCallReportInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.DailyUserCallReportInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // userDailyCallReportInstaller
            // 
            this.userDailyCallReportInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.userDailyCallReportInstaller.Password = null;
            this.userDailyCallReportInstaller.Username = null;
            // 
            // DailyUserCallReportInstaller
            // 
            this.DailyUserCallReportInstaller.ServiceName = "DailyUserCallReport";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.userDailyCallReportInstaller,
            this.DailyUserCallReportInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller userDailyCallReportInstaller;
        private System.ServiceProcess.ServiceInstaller DailyUserCallReportInstaller;
    }
}