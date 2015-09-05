namespace DailyCallReportSvc
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
            this.DailyCallReportProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.DailyCallReportService = new System.ServiceProcess.ServiceInstaller();
            this.serviceInstaller2 = new System.ServiceProcess.ServiceInstaller();
            // 
            // DailyCallReportProcessInstaller
            // 
            this.DailyCallReportProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.DailyCallReportProcessInstaller.Password = null;
            this.DailyCallReportProcessInstaller.Username = null;
            this.DailyCallReportProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller1_AfterInstall);
            // 
            // DailyCallReportService
            // 
            this.DailyCallReportService.ServiceName = "DailyCallReportingSvc";
            this.DailyCallReportService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // serviceInstaller2
            // 
            this.serviceInstaller2.ServiceName = "DailyCallReportSvc";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.DailyCallReportProcessInstaller,
            this.DailyCallReportService,
            this.serviceInstaller2});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller DailyCallReportProcessInstaller;
        private System.ServiceProcess.ServiceInstaller DailyCallReportService;
        private System.ServiceProcess.ServiceInstaller serviceInstaller2;
    }
}