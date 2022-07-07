using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace DirectoryMonitoringService
{
    [RunInstaller(true)]
    public partial class MonitoringServiceInstaller : Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public MonitoringServiceInstaller()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "DirectoryMonitoringService";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
