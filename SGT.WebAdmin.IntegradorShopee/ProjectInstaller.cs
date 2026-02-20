using System.Collections;
using System.ComponentModel;

namespace SGT.WebAdmin.IntegradorShopee
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
                this.serviceInstaller1.ServiceName = serviceName;
                this.serviceInstaller1.DisplayName = serviceName;
            }
        }
    }
}
