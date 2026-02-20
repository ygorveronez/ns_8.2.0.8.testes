using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Servicos.Http;
using System;
using System.ServiceProcess;

namespace LeitorCanhotoOCR
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            try
            {
                MigrarAppSettings(AppDomain.CurrentDomain.BaseDirectory);

                var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                                  .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                  .AddJsonFile("appsettings.json");

                HttpClientRegistration.RegisterClients();
                appSettings = builder.Build();

                Servicos.IO.FileStorage.ConfigureApplicationFileStorage(StringConexaoAdmin, Host);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                throw;
            }

#if !DEBUG
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
#else
            while (true)
            {
                Start();
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            }
#endif
        }

        public static void Start()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            try
            {
#if !DEBUG
                new Servicos.Embarcador.Configuracoes.Arquivo().AjustarConfiguracoes(unitOfWork);

#else
                Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
#endif
                Servicos.Embarcador.Canhotos.LeitorOCR serLeitorOCR = new Servicos.Embarcador.Canhotos.LeitorOCR(unitOfWork);

                string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;
                string caminhoRaizFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaizFTP;
                string caminhoCanhotos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoCanhotos;

                string tipoArmazenamento = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().TipoArmazenamentoLeitorOCR;
                string enderecoFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnderecoFTP;
                string usuarioFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UsuarioFTP;
                string senhaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().SenhaFTP;
                bool ftpPassivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FTPPassivo.HasValue && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FTPPassivo.Value;
                string portaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PortaFTP;
                bool utilizaSFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaSFTP.HasValue && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaSFTP.Value;
                string apiLink = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRLink;
                string apiKey = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRKey;

                string caminhoBatReiniciar = appSettings["AppSettings:CaminhoBatReiniciar"];

                serLeitorOCR.Iniciar(caminho, caminhoCanhotos, tipoArmazenamento, enderecoFTP, usuarioFTP, senhaFTP, caminhoRaizFTP, ftpPassivo, portaFTP, utilizaSFTP, StringConexaoAdmin, caminhoBatReiniciar, apiLink, apiKey);

            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private static IConfigurationRoot appSettings;

        private static string _host;
        public static string Host
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_host))
                {
                    _host = appSettings["AppSettings:Host"];
                    if (string.IsNullOrWhiteSpace(_host))
                        Servicos.Log.TratarErro($"Appsettings sem configuração para Host. Adicione no appsettings.json parametro com nome host");
                }
                return _host;
            }
        }

        private static string _stringConexao;
        public static string StringConexao
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_stringConexao))
                {
                    using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(StringConexaoAdmin))
                    {
                        AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                        AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Host);

                        if (clienteURLAcesso == null)
                            throw new Exception($"O Host {Host} não possui uma configuração");


                        if (clienteURLAcesso.URLHomologacao && clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao != null)
                            _stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao);
                        else
                            _stringConexao = GerarStringConexao(clienteURLAcesso.Cliente.ClienteConfiguracao);

                        Servicos.Log.SetStringConexao(_stringConexao);
                        Servicos.Log.SetCliente(clienteURLAcesso.Cliente.NomeFantasia + (clienteURLAcesso.URLHomologacao ? " Homo" : " Produção"));
                    }
                }

                return _stringConexao;
            }
        }

        private static string _stringConexaoAdmin;
        public static string StringConexaoAdmin
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_stringConexaoAdmin))
                    _stringConexaoAdmin = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");

                return _stringConexaoAdmin;
            }
        }

        private static string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            return "Data Source=" + configuracao.DBServidor + ";Initial Catalog=" + configuracao.DBBase + ";User Id=" + configuracao.DBUsuario + ";Password=" + configuracao.DBSenha + ";";
        }

        private static void MigrarAppSettings(string diretorio)
        {
            string jsonFilePath = Utilidades.IO.FileStorageService.LocalStorage.Combine(diretorio, "appsettings.json");

            if (!Utilidades.IO.FileStorageService.LocalStorage.Exists(jsonFilePath))
            {
                var appSettings = System.Configuration.ConfigurationManager.AppSettings;

                var jsonObject = new JObject();
                var appSettingsObject = new JObject();

                var allowedKeys = new[] { "Host", "CaminhoBatReiniciar" };
                foreach (var key in appSettings.AllKeys)
                {
                    if (Array.Exists(allowedKeys, allowedKey => allowedKey.Equals(key, StringComparison.OrdinalIgnoreCase)))
                    {
                        appSettingsObject[key] = appSettings[key];
                    }
                }

                jsonObject["AppSettings"] = appSettingsObject;

                Utilidades.IO.FileStorageService.LocalStorage.WriteAllText(jsonFilePath, jsonObject.ToString());
            }
        }
    }
}
