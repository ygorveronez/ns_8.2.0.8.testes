using System.Configuration;
using System.ServiceProcess;

namespace LeitorCanhotoOCR
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
