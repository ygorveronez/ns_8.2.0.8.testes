namespace SGT.WebAdmin.ProcessarCalculoDeFrete
{
    public sealed class WindowsBackgroundService : BackgroundServiceBase
    {
        private List<int> _cargasEmProcessamento;
        private List<System.Threading.Thread> _threadsGeracaoCargas;
        private System.Threading.Thread _threadCargasPendentes;
        private static string _arquivoLog = "Logs";
        private List<Dominio.ObjetosDeValor.Embarcador.ProcessarCalculoDeFreteService.ConfiguracaoThread> _configuracoes;
        private int _idConfigurarThread = 10;
        private string _nomeConfigurarThread = "ThreadCargasPendentes";
        private bool _ativaConfigurarThread = true;
        private int _numeroTheadsParalelas = 20;
        private string _defaultCulture = string.Empty;

        public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            _idConfigurarThread = _configuration.GetValue<Int32>("ThreadCargasPendentes:Id");
            _nomeConfigurarThread = _configuration.GetValue<string>("ThreadCargasPendentes:Nome");
            _ativaConfigurarThread = _configuration.GetValue<bool>("ThreadCargasPendentes:Ativa");
            _numeroTheadsParalelas = _configuration.GetValue<Int32>("ThreadCargasPendentes:NumeroTheadsParalelas");
            _defaultCulture = _configuration.GetValue<string>("AppSettings:defaultCulture");
            MaxPoolSize = _configuration.GetValue<int?>("AppSettings:MaxPoolSize") ?? 0;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Servicos.Log.GravarInfo($"Parando o serviço...");

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_defaultCulture))
                _defaultCulture = "pt-BR";

            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(_defaultCulture);

            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            Servicos.IO.FileStorage.ConfigureApplicationFileStorage(StringConexaoAdmin, Host);
            Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.ConfigureApplicationEmissorDocumento(StringConexao);

            Servicos.Log.GravarInfo("Serviço CalcularFreteCargasPendentes iniciado.");

            #region ConfiguraTreadCargasPendentes
            _configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.ProcessarCalculoDeFreteService.ConfiguracaoThread>();
            _cargasEmProcessamento = new List<int>();
            _threadsGeracaoCargas = new List<System.Threading.Thread>();
            _configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.ProcessarCalculoDeFreteService.ConfiguracaoThread(_idConfigurarThread, _nomeConfigurarThread, _ativaConfigurarThread, _numeroTheadsParalelas));
            #endregion

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ConfigurarThreads();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    throw;
                }

                await Task.Delay(300000, cancellationToken);
            }
        }

        public void ConfigurarThreads()
        {
            try
            {
                if (_threadCargasPendentes == null)
                {
                    _threadCargasPendentes = new System.Threading.Thread(ThreadsCargasPendentesStart);
                    _threadCargasPendentes.Start();
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, _arquivoLog);
                throw;
            }
        }

        private void ThreadsCargasPendentesStart()
        {
            DateTime dtAtualizouRegrasImpostos = DateTime.Now.AddMinutes(-11);

            while (true)
            {
                GC.Collect();
                try
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        if (dtAtualizouRegrasImpostos.AddMinutes(10) < DateTime.Now)
                        {
                            dtAtualizouRegrasImpostos = DateTime.Now;
                            Servicos.Embarcador.ISS.RegrasCalculoISS.GetInstance(unitOfWork).AtualizarRegrasISS(unitOfWork);
                            Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork).AtualizarRegrasICMS(unitOfWork);
                        }

                        Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
                        var configuracao = ObterConfiguracaoThread("ThreadCargasPendentes");
                        if (!(configuracao?.Ativa ?? false))
                        {
                            Servicos.Log.GravarInfo("Serviço ThreadCalcularFreteCargasPendentes desabilitado, aguardando 10 minutos para nova execução.", _arquivoLog);
                            System.Threading.Thread.Sleep(600000);
                            continue;
                        }

                        int quantidadeThreadsDisponiveis = 0;
                        int quantidadeThreadsAtivas = _threadsGeracaoCargas.Count;

                        List<int> threadsRemover = new List<int>();
                        for (int i = quantidadeThreadsAtivas - 1; i >= 0; i--)
                        {
                            if (_threadsGeracaoCargas[i].IsAlive)
                                continue;
                            _threadsGeracaoCargas.RemoveAt(i);
                        }
                        quantidadeThreadsDisponiveis = configuracao.NumeroTheadsParalelas - _threadsGeracaoCargas.Count;
                        if (quantidadeThreadsDisponiveis <= 0)
                        {
                            Servicos.Log.GravarInfo($"Já existem {_threadsGeracaoCargas.Count} threads ativas. Aguardando 10 segundos para nova verificação.", _arquivoLog);
                            System.Threading.Thread.Sleep(1000);
                            continue;
                        }

                        List<int> cargasPendentesGeracao = ObterCargasPendentes(unitOfWork);

                        if (cargasPendentesGeracao.Count <= 0)
                        {
                            Servicos.Log.GravarInfo($"Não existem cargas pendentes para CalcularFreteCargasPendentes. Aguardando 10 segundos para nova verificação. {_threadsGeracaoCargas.Count} de {configuracao.NumeroTheadsParalelas} threads em execução.", _arquivoLog);
                            System.Threading.Thread.Sleep(1000);
                            continue;
                        }

                        for (int i = 0; i < quantidadeThreadsDisponiveis && i < cargasPendentesGeracao.Count; i++)
                        {
                            int codigoCargaPendenteGeracao = cargasPendentesGeracao[i];
                            System.Threading.Thread thread = new System.Threading.Thread(() => GerarCarga(codigoCargaPendenteGeracao));
                            thread.Start();
                            _threadsGeracaoCargas.Add(thread);
                        }
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, _arquivoLog);
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        private void GerarCarga(int codigoCargaPendenteGeracao)
        {
            try
            {
                lock (_cargasEmProcessamento)
                {
                    if (!_cargasEmProcessamento.Any(o => o == codigoCargaPendenteGeracao))
                        _cargasEmProcessamento.Add(codigoCargaPendenteGeracao);
                    else
                        return;
                    Servicos.Log.GravarInfo($"Iniciou thread para geração de CalcularFreteCargasPendentes {codigoCargaPendenteGeracao}.", _arquivoLog);
                }

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    List<int> cargas = new List<int>();
                    cargas.Add(codigoCargaPendenteGeracao);
                    Servicos.Embarcador.Carga.Frete.CalcularFreteCargasPendentes(Dominio.Enumeradores.LoteCalculoFrete.Padrao, TipoServicoMultisoftware, unitOfWork, StringConexao, Cliente, cargas);
                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, _arquivoLog);
            }
            finally
            {
                lock (_cargasEmProcessamento)
                {
                    if (_cargasEmProcessamento.Any(o => o == codigoCargaPendenteGeracao))
                        _cargasEmProcessamento.Remove(codigoCargaPendenteGeracao);

                    Servicos.Log.GravarInfo($"Finalizou thread para geração de CalcularFreteCargasPendentes {codigoCargaPendenteGeracao}.", _arquivoLog);
                }
            }
        }

        private List<int> ObterCargasPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<int> codigosCargaPendentes = new List<int>();
            codigosCargaPendentes.AddRange(repositorioCarga.BuscarCargasAguardandoCalculoFrete(Dominio.Enumeradores.LoteCalculoFrete.Integracao, configuracao.ExigirCargaRoteirizada, 15, TipoServicoMultisoftware, configuracao.CalcularFreteInicioCarga));
            codigosCargaPendentes.AddRange(repositorioCarga.BuscarCargasAguardandoCalculoFrete(Dominio.Enumeradores.LoteCalculoFrete.Padrao, configuracao.ExigirCargaRoteirizada, 15, TipoServicoMultisoftware, configuracao.CalcularFreteInicioCarga));
            codigosCargaPendentes = codigosCargaPendentes.Distinct().ToList();
            List<int> codigosSemThread = new List<int>();
            lock (_cargasEmProcessamento)
            {
                foreach (var carga in codigosCargaPendentes)
                    if (!_cargasEmProcessamento.Any(o => o == carga))
                        codigosSemThread.Add(carga);
            }
            return codigosSemThread;
        }

        private Dominio.ObjetosDeValor.Embarcador.ProcessarCalculoDeFreteService.ConfiguracaoThread ObterConfiguracaoThread(string nome)
        {
            return _configuracoes.Where(x => x.Nome == nome).FirstOrDefault();
        }
    }
}