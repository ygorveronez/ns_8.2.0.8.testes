using System.Collections;
using System.ComponentModel;

namespace SGT.Monitoramento
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            RetrieveServiceName();

            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            RetrieveServiceName();

            base.Uninstall(savedState);
        }

        private void RetrieveServiceName()
        {
            string serviceName = Context.Parameters["ServiceName"];
            if (!string.IsNullOrEmpty(serviceName))
            {
                this.ServicoMonitoramento.ServiceName = serviceName;
                this.ServicoMonitoramento.DisplayName = serviceName;
            }
        }
    }
}
