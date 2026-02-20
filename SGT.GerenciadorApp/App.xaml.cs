using Microsoft.Shell;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;

namespace SGT.GerenciadorApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        public static System.Windows.Forms.NotifyIcon NotificationIcon = null;
        public static System.Threading.Timer ThreadExecucaoEBS;
        public static System.Threading.Timer ThreadEnvioEmailEncerramentoMDFe;
        public static System.Threading.Thread ThreadEnvioEmailJanelaCarregamento;
        public static System.Threading.Timer ThreadConsultaDocumentosDestinados;
        public static System.Threading.Timer ThreadConsultaNFSeDestinadasSaoPauloSP;
        public static System.Threading.Timer ThreadAverbacaoATM;
        public static System.Threading.Timer ThreadIntegracaoDocumentosDestinados;
        public static System.Threading.Timer ThreadConsultaXMLEmail;
        public static System.Threading.Timer ThreadConsultaCTeDestinados;
        public static System.Threading.Timer ThreadConsultaMDFeDestinados;
        public static System.Threading.Timer ThreadImportacaoCTF;
        public static System.Threading.Timer ThreadConsultaSituacaoMDFe;

        #region Monitoramento

        public static System.Threading.Timer ThreadIntegracaoEventosViagem;
        public static System.Threading.Timer ThreadIntegracaoControleEntrega;

        #endregion

        private const string UniqueNameApp = "MultisoftwareGerenciadorApp";
        private static string _stringConexaoAdmin;

        private static string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            return "Data Source=" + configuracao.DBServidor + ";Initial Catalog=" + configuracao.DBBase + ";User Id=" + configuracao.DBUsuario + ";Password=" + configuracao.DBSenha + ";";
        }

        public static string StringConexao(int codigoCliente, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);

            string stringConexao = "";

            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoCliente);

            if (cliente != null)
                stringConexao = GerarStringConexao(cliente.ClienteConfiguracao);
            else
                throw new Exception("O cliente informado não existe ou não possui uma configuração.");

            return stringConexao;
        }


        public static string StringConexaoHomologacao(int codigoCliente, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);

            string stringConexao = "";

            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoCliente);

            if (cliente != null)
                stringConexao = GerarStringConexao(cliente.ClienteConfiguracaoHomologacao);
            else
                throw new Exception("O cliente informado não existe ou não possui uma configuração.");

            return stringConexao;
        }



        public static string StringConexaoAdmin
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_stringConexaoAdmin))
                    _stringConexaoAdmin = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");

                return _stringConexaoAdmin;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
            log4net.Config.XmlConfigurator.Configure();
#else
            CriptografarAppConfig("connectionStrings");
#endif

            //MigrateDatabase(StringConexao);
            //RegisterInStartup();
            AppDomain.CurrentDomain.UnhandledException += App_UnhandledException;
            Application.Current.DispatcherUnhandledException += App_DispatcherUnhandledException;

            AppContext.SetSwitch("Switch.System.Security.Cryptography.Xml.UseInsecureHashAlgorithms", true);
            AppContext.SetSwitch("Switch.System.Security.Cryptography.Pkcs.UseInsecureHashAlgorithms", true);
        }

        public void MigrateDatabase(string stringConexao)
        {
            try
            {
                Migrations.Migrator.MigrateToLatest(stringConexao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void CriptografarAppConfig(string section, string provider = "RSAProtectedConfigurationProvider")
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSection confStrSect = config.GetSection(section);

            if (confStrSect != null && !confStrSect.SectionInformation.IsProtected)
            {
                confStrSect.SectionInformation.ProtectSection(provider);
                config.Save();
            }
        }

        private void RegisterInStartup()
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                registryKey.SetValue("Gerenciador MultiTMS", System.Reflection.A‌​ssembly.GetExecutingA‌​ssembly().Location, RegistryValueKind.String);
        }

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(UniqueNameApp))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Servicos.Log.TratarErro("Exceção não tratada: " + ((System.Exception)e.ExceptionObject).ToString());
            Servicos.Log.TratarErro("Reiniciando a aplicação.");

            string CaminhoReiniciarApp = System.AppDomain.CurrentDomain.BaseDirectory;
            string CaminhoPowerShell = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";

            if (System.IO.Directory.Exists(CaminhoReiniciarApp) && System.IO.Directory.Exists(CaminhoPowerShell) && Utilidades.IO.FileStorageService.Storage.Exists(CaminhoReiniciarApp + "reiniciar-aplicacao.ps1"))
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = CaminhoPowerShell;
                p.StartInfo.Arguments = "-windowstyle hidden -NoLogo -NonInteractive -ExecutionPolicy Bypass " + CaminhoReiniciarApp + "reiniciar-aplicacao.ps1"; // SQL-INJECTION-SAFE
                p.Start();
            }
            else
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);

            Application.Current.Shutdown();
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Servicos.Log.TratarErro("Exceção não tratada: " + e.Exception.ToString());
            Servicos.Log.TratarErro("Reiniciando a aplicação.");

            e.Handled = true;

            string CaminhoReiniciarApp = System.AppDomain.CurrentDomain.BaseDirectory;
            string CaminhoPowerShell = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";

            if (System.IO.Directory.Exists(CaminhoReiniciarApp) && System.IO.Directory.Exists(CaminhoPowerShell) && Utilidades.IO.FileStorageService.Storage.Exists(CaminhoReiniciarApp + "reiniciar-aplicacao.ps1"))
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = CaminhoPowerShell;
                p.StartInfo.Arguments = "-windowstyle hidden -NoLogo -NonInteractive -ExecutionPolicy Bypass " + CaminhoReiniciarApp + "reiniciar-aplicacao.ps1"; // SQL-INJECTION-SAFE
                p.Start();
            }
            else
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);

            Application.Current.Shutdown();
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // Bring window to foreground
            if (MainWindow.WindowState == WindowState.Minimized)
            {
                MainWindow.WindowState = WindowState.Normal;
            }

            this.MainWindow.Activate();

            return true;
        }

        #endregion
    }
}

