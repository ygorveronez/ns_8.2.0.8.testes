namespace SGT.Shopee
{
    public sealed class WindowsBackgroundService : BackgroundServiceBase
    {
        private System.Threading.Thread _threadPasso1;
        private System.Threading.Thread _threadPasso2;
        private static int _quantidadeThreadsExecutar = 0;
        private static string _caminhoArquivos = string.Empty;
        private static string _caminhoArquivosIntegracao = string.Empty;
        private static string _prefixoMSMQ = string.Empty;
        private static int _sleepFilaVazia = 1000;
        private static int _sleepPasso2 = 200;
        private static bool _lockDeletes = false;

        public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            _quantidadeThreadsExecutar = _configuration.GetValue<int>("QuantidadeThreadsExecutar");
            _caminhoArquivos = _configuration.GetValue<string>("CaminhoArquivos");
            _caminhoArquivosIntegracao = _configuration.GetValue<string>("CaminhoArquivosIntegracao");
            _prefixoMSMQ = _configuration.GetValue<string>("PrefixoMSMQ");
            _lockDeletes = _configuration.GetValue<bool>("LockDeletes");
            MaxPoolSize = _configuration.GetValue<int>("MaxPoolSize");
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro($"Iniciando o serviço...");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro($"Parando o serviço...");

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                Servicos.IO.FileStorage.ConfigureApplicationFileStorage(StringConexaoAdmin, Host);

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_threadPasso1 == null)
                    {
                        _threadPasso1 = new System.Threading.Thread(() => StartPasso1ParallelThreads(cancellationToken));
                        _threadPasso1.Start();
                    }

                    if (_threadPasso2 == null)
                    {
                        _threadPasso2 = new System.Threading.Thread(() => StartPasso2ParallelThreads(cancellationToken));
                        _threadPasso2.Start();
                    }

                    await Task.Delay(300000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void StartPasso1ParallelThreads(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    string arquivoPasso1 = $"Passo1_{DateTime.Now.ToString("yyyyMMdd")}";

                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        Servicos.Embarcador.Integracao.Shopee.IntegracaoShopee integracaoShopee = new Servicos.Embarcador.Integracao.Shopee.IntegracaoShopee(unitOfWork, TipoServicoMultisoftware, this.StringConexao, _lockDeletes, _sleepPasso2);

                        //integracaoShopee.IntegrartPasso1ComPassa1WebHook(arquivoPasso1, this.StringConexao);
                        integracaoShopee.StartPasso1DownloadDocumentos(_quantidadeThreadsExecutar, arquivoPasso1, unitOfWork, token);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                if (token.IsCancellationRequested)
                    return;

                GC.Collect();
                System.Threading.Thread.Sleep(5000);
            }
        }

        private void StartPasso2ParallelThreads(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        //altera pacotes com erro 
                        string arquivoPasso2 = $"Passo2_{DateTime.Now.ToString("yyyyMMdd")}";

                        Servicos.Embarcador.Integracao.Shopee.IntegracaoShopee integracaoShopee = new Servicos.Embarcador.Integracao.Shopee.IntegracaoShopee(unitOfWork, this.TipoServicoMultisoftware, this.StringConexao, _lockDeletes, _sleepPasso2);
                        integracaoShopee.StartPasso2VincularDocumentosCarga(_quantidadeThreadsExecutar, unitOfWork, token);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                if (token.IsCancellationRequested)
                    return;

                GC.Collect();
                System.Threading.Thread.Sleep(5000);
            }
        }
    }
}