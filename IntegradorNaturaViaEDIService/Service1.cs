using System.Collections.Generic;
using System.Configuration;
using System.ServiceProcess;

namespace IntegradorNaturaViaEDIService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Program.Start();
        }

        protected override void OnStop()
        {
        }
    }
}
