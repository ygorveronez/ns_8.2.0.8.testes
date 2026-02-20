using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace SGT.WebAdmin.ReportWindowsService
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
                this.ServicoGeracaoRelatorios.ServiceName = serviceName;
                this.ServicoGeracaoRelatorios.DisplayName = serviceName;
            }
        }
    }
}
