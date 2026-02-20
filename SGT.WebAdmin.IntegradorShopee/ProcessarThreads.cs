using SGT.WebAdmin.IntegradorShopee.Thread;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;

namespace SGT.WebAdmin.IntegradorShopee
{
    public partial class ProcessarThreads : ServiceBase
    {
        private List<int> _cargasEmProcessamento;
        private List<System.Threading.Thread> _threadsGeracaoCargas;
        private System.Threading.Thread _threadPasso1;
        private System.Threading.Thread _threadPasso2;
        private System.Threading.Thread _threadPasso1WebHook;
        private System.Threading.Thread _threadPasso2WebHook;


        private static string _arquivoLog = "Logs";
        private List<ConfiguracaoThread> _configuracoes;

        public ProcessarThreads()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Servicos.Log.TratarErro("Serviço IntegradorShopee iniciado.");
            try
            {
                string defaultCulture = Program.appSettings["AppSettings:DefaultCulture"];

                if (string.IsNullOrWhiteSpace(defaultCulture))
                    defaultCulture = "pt-BR";

                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(defaultCulture);

                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

                ConfigurarThreads();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw ex;
            }
        }

        public void ConfigurarThreads()
        {
            _configuracoes = new List<ConfiguracaoThread>();

            #region ConfiguraTreadCargasPendentes

            _cargasEmProcessamento = new List<int>();
            _threadsGeracaoCargas = new List<System.Threading.Thread>();

            try
            { // INICIALIZA _threadConsultaCargasPendentes

                int Id = Int16.Parse(Program.appSettings["ThreadCargasPendentes:Id"]);
                string Nome = Program.appSettings["ThreadCargasPendentes:Nome"];
                bool Ativa = bool.Parse(Program.appSettings["ThreadCargasPendentes:Ativa"]);
                int threadsEmParalelo = int.Parse(Program.appSettings["ThreadCargasPendentes:threadsEmParalelo"]);
                bool LockDeletes = bool.Parse(Program.appSettings["ThreadCargasPendentes:LockDeletes"]);
                int SleepFilaVazia = int.Parse(Program.appSettings["ThreadCargasPendentes:SleepFilaVazia"]);
                int SleepPasso2 = int.Parse(Program.appSettings["ThreadCargasPendentes:SleepPasso2"]);

                _configuracoes.Add(new ConfiguracaoThread(Id, Nome, Ativa, threadsEmParalelo, LockDeletes, SleepFilaVazia, SleepPasso2));

                if (_threadPasso1 == null)
                {
                    _threadPasso1 = new System.Threading.Thread(StartPasso1ParallelThreads);
                    _threadPasso1.Start();
                }

                if (_threadPasso2 == null)
                {
                    _threadPasso2 = new System.Threading.Thread(StartPasso2ParallelThreads);
                    _threadPasso2.Start();
                }

                /*
                if (_threadPasso1WebHook == null)
                {
                    _threadPasso1WebHook = new System.Threading.Thread(StartPasso1WebHookParallelThreads);
                    _threadPasso1WebHook.Start();
                }

                if (_threadPasso2WebHook == null)
                {
                    _threadPasso2WebHook = new System.Threading.Thread(StartPasso2WebHookParallelThreads);
                    _threadPasso2WebHook.Start();
                }
                */
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, _arquivoLog);
                throw e;
            }

            #endregion
        }

        private void StartPasso1ParallelThreads()
        {
            var configuracao = ObterConfiguracaoThread("ThreadIntegracaoShopee");
            while (true)
            {
                if (!(configuracao?.Ativa ?? false))
                {
                    Servicos.Log.TratarErro("Serviço IntegradorShopee desabilitado, aguardando 10 minutos para nova execução.", _arquivoLog);
                    System.Threading.Thread.Sleep(600000);
                    continue;
                }

                try
                {
                    string arquivoPasso1 = $"Passo1_{DateTime.Now.ToString("yyyyMMdd")}";

                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao))
                    {
                        //Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
                        Servicos.Embarcador.Integracao.Shopee.IntegracaoShopee integracaoShopee = new Servicos.Embarcador.Integracao.Shopee.IntegracaoShopee(unitOfWork, Program.TipoServicoMultisoftware, Program.StringConexao, configuracao.LockDeletes, configuracao.SleepPasso2);

                        integracaoShopee.IntegrartPasso1ComPassa1WebHook(arquivoPasso1, Program.StringConexao);
                        //integracaoShopee.StartPasso1(repPacote.BuscarPacotesPendentesIntegracaoPasso1(), configuracao.NucleosEmParalelo, arquivoPasso1);
                        integracaoShopee.StartPasso1DownloadDocumentos(configuracao.NucleosEmParalelo, arquivoPasso1);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, _arquivoLog);
                    System.Threading.Thread.Sleep(5000);
                }

                GC.Collect();
                System.Threading.Thread.Sleep(5000);
            }
        }

        private void StartPasso2ParallelThreads()
        {
            var configuracao = ObterConfiguracaoThread("ThreadIntegracaoShopee");
            while (true)
            {
                if (!(configuracao?.Ativa ?? false))
                {
                    Servicos.Log.TratarErro("Serviço IntegradorShopee desabilitado, aguardando 10 minutos para nova execução.", _arquivoLog);
                    System.Threading.Thread.Sleep(600000);
                    continue;
                }

                try
                {
                    using (NHibernate.ISession session = Repositorio.SessionHelper.OpenSession(Program.StringConexao))
                    {
                        using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(session))
                        {
                            //altera pacotes com erro 
                            string arquivoPAsso2 = $"Passo2_{DateTime.Now.ToString("yyyyMMdd")}";

                            //Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
                            Servicos.Embarcador.Integracao.Shopee.IntegracaoShopee integracaoShopee = new Servicos.Embarcador.Integracao.Shopee.IntegracaoShopee(unitOfWork, Program.TipoServicoMultisoftware, Program.StringConexao, configuracao.LockDeletes, configuracao.SleepPasso2);
                            
                            //integracaoShopee.StartPasso2(repPacote.BuscarPacotesPendentesIntegracaoPasso2(), configuracao.NucleosEmParalelo, Program.StringConexao, arquivoPAsso2);
                            integracaoShopee.StartPasso2VincularDocumentosCarga(configuracao.NucleosEmParalelo, Program.StringConexao);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, _arquivoLog);
                    System.Threading.Thread.Sleep(5000);
                }

                GC.Collect();
                System.Threading.Thread.Sleep(5000);
            }
        }

        private void StartPasso1WebHookParallelThreads()
        {
            var configuracao = ObterConfiguracaoThread("ThreadIntegracaoShopee");
            while (true)
            {
                if (!(configuracao?.Ativa ?? false))
                {
                    Servicos.Log.TratarErro("Serviço IntegradorWebHookShopee desabilitado, aguardando 10 minutos para nova execução.", _arquivoLog);
                    System.Threading.Thread.Sleep(600000);
                    continue;
                }
                try
                {
                    string arquivoPasso1 = $"Passo1WebHook_{DateTime.Now.ToString("yyyyMMdd")}";
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao))
                    {
                        Repositorio.Embarcador.Cargas.PacoteWebHook repPacoteWebHook = new Repositorio.Embarcador.Cargas.PacoteWebHook(unitOfWork);
                        new Servicos.Embarcador.Integracao.Shopee.IntegracaoShopee(unitOfWork, Program.TipoServicoMultisoftware, Program.StringConexao, configuracao.LockDeletes, configuracao.SleepPasso2).StartPasso1WebHook(repPacoteWebHook.BuscarPacotesPendentesIntegracaoPasso1WebHook(), configuracao.NucleosEmParalelo, arquivoPasso1);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, _arquivoLog);
                    System.Threading.Thread.Sleep(5000);
                }
                GC.Collect();
                System.Threading.Thread.Sleep(5000);
            }
        }

        private void StartPasso2WebHookParallelThreads()
        {
            var configuracao = ObterConfiguracaoThread("ThreadIntegracaoShopee");
            while (true)
            {
                if (!(configuracao?.Ativa ?? false))
                {
                    Servicos.Log.TratarErro("Serviço IntegradorWebHookShopee desabilitado, aguardando 10 minutos para nova execução.", _arquivoLog);
                    System.Threading.Thread.Sleep(600000);
                    continue;
                }
                try
                {
                    using (NHibernate.ISession session = Repositorio.SessionHelper.OpenSession(Program.StringConexao))
                    {
                        using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(session))
                        {
                            //altera pacotes com erro 
                            string arquivoPAsso2 = $"Passo2WebHook_{DateTime.Now.ToString("yyyyMMdd")}";

                            Repositorio.Embarcador.Cargas.PacoteWebHook repPacoteWebHook = new Repositorio.Embarcador.Cargas.PacoteWebHook(unitOfWork);
                            new Servicos.Embarcador.Integracao.Shopee.IntegracaoShopee(unitOfWork, Program.TipoServicoMultisoftware, Program.StringConexao, configuracao.LockDeletes, configuracao.SleepPasso2).StartPasso2WebHook(repPacoteWebHook.BuscarPacotesPendentesIntegracaoPasso2WebHook(), configuracao.NucleosEmParalelo, Program.StringConexao, arquivoPAsso2);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, _arquivoLog);
                    System.Threading.Thread.Sleep(5000);
                }
                GC.Collect();
            }
        }

        private ConfiguracaoThread ObterConfiguracaoThread(string nome)
        {
            return _configuracoes.Where(x => x.Nome == nome).FirstOrDefault();
        }

        protected override void OnStop()
        {
            if (_threadPasso1 != null)
            {
                _threadPasso1.Abort();
                _threadPasso1 = null;
            }

            if (_threadPasso2 != null)
            {
                _threadPasso2.Abort();
                _threadPasso2 = null;
            }

            GC.Collect();
            Servicos.Log.TratarErro("Serviço parado.");
        }
    }
}
