using System.Threading;

namespace SGT.MercadoLivre
{
    public sealed class WindowsBackgroundService : BackgroundServiceBase
    {
        private Dictionary<int, Thread> _threadsExecutando = new Dictionary<int, Thread>();
        private System.Threading.Thread _threadNFesComplementares;
        private System.Threading.Thread _threadInicioAutomaticoRotaEFacility;
        private static int _quantidadeThreadsExecutar = 0;
        private static int _quantidadeThreadsExecutarRotaEFacility = 10;
        private static string _caminhoArquivos = string.Empty;
        private static string _caminhoArquivosIntegracao = string.Empty;
        private static string _prefixoMSMQ = string.Empty;

        public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            _quantidadeThreadsExecutar = _configuration.GetValue<int>("QuantidadeThreadsExecutar");
            _caminhoArquivos = _configuration.GetValue<string>("CaminhoArquivos");
            _caminhoArquivosIntegracao = _configuration.GetValue<string>("CaminhoArquivosIntegracao");
            _prefixoMSMQ = _configuration.GetValue<string>("PrefixoMSMQ");
            _quantidadeThreadsExecutarRotaEFacility = _configuration.GetValue<int>("QuantidadeThreadsExecutarRotaEFacility");
            MaxPoolSize = _configuration.GetValue<int>("MaxPoolSize");
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
            Servicos.IO.FileStorage.ConfigureApplicationFileStorage(StringConexaoAdmin, Host);
            Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.ConfigureApplicationEmissorDocumento(StringConexao);
            Servicos.Log.GravarInfo($"Iniciando o serviço...");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    
                    if (_threadNFesComplementares == null)
                    {
                        _threadNFesComplementares = new System.Threading.Thread(() => ProcessarNFesComplementaresAsync(cancellationToken));
                        _threadNFesComplementares.Start();
                    }

                    if (_threadInicioAutomaticoRotaEFacility == null)
                    {
                        _threadInicioAutomaticoRotaEFacility = new System.Threading.Thread(() => ProcessarInicioAutomaticoRotaEFacilityAsync(cancellationToken));
                        _threadInicioAutomaticoRotaEFacility.Start();
                    }

                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        int quantidadeThreadsDisponiveis = 0;
                        int quantidadeThreadsAtivas = _threadsExecutando.Count;
                        object lockObterPedido = new object();

                        for (int i = quantidadeThreadsAtivas - 1; i >= 0; i--)
                        {
                            if (_threadsExecutando.ElementAt(i).Value.IsAlive)
                                continue;

                            _threadsExecutando.Remove(_threadsExecutando.ElementAt(i).Key);
                        }

                        quantidadeThreadsDisponiveis = _quantidadeThreadsExecutar - _threadsExecutando.Count;

                        if (quantidadeThreadsDisponiveis <= 0)
                        {
                            await Task.Delay(5000, cancellationToken);
                            continue;
                        }

                        List<int> cargaIntegracaoMercadoLivrePendentes = ObterCargaIntegracaoMercadoLivrePendentesConsulta(quantidadeThreadsDisponiveis, unitOfWork);

                        if (cargaIntegracaoMercadoLivrePendentes.Count <= 0)
                        {
                            await Task.Delay(10000, cancellationToken);
                            continue;
                        }

                        for (int i = 0; i < quantidadeThreadsDisponiveis && i < cargaIntegracaoMercadoLivrePendentes.Count; i++)
                        {
                            int codigoCargaIntegracaoMercadoLivrePendenteConsulta = cargaIntegracaoMercadoLivrePendentes[i];

                            System.Threading.Thread thread = new System.Threading.Thread(() => ConsultarCargaIntegracaoMercadoLivre(codigoCargaIntegracaoMercadoLivrePendenteConsulta, lockObterPedido, cancellationToken));

                            thread.Start();

                            _threadsExecutando.Add(codigoCargaIntegracaoMercadoLivrePendenteConsulta, thread);
                        }

                        await Task.Delay(5000, cancellationToken);

                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);

                    await Task.Delay(5000, cancellationToken);
                }
            }
        }

        private List<int> ObterCargaIntegracaoMercadoLivrePendentesConsulta(int quantidade, Repositorio.UnitOfWork unitOfWork)
        {
            int[] codigosExecutando = _threadsExecutando.Select(o => o.Key).ToArray();

            Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);

            return repCargaIntegracaoMercadoLivre.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitSituacao.AgConsulta, quantidade, codigosExecutando);
        }

        private void ConsultarCargaIntegracaoMercadoLivre(int codigoCargaIntegracaoMercadoLivre, object lockObterPedido, CancellationToken cancellationToken)
        {
            try
            {
                Servicos.Log.GravarInfo($"Consultando CargaIntegracaoMercadoLivre: " + codigoCargaIntegracaoMercadoLivre);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcMercadoLivre = new Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre(Cliente, _caminhoArquivos, _caminhoArquivosIntegracao, _prefixoMSMQ, _quantidadeThreadsExecutarRotaEFacility);

                    svcMercadoLivre.NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes = true;

                    svcMercadoLivre.ConsultarCargaIntegracaoMercadoLivrePendente(codigoCargaIntegracaoMercadoLivre, TipoServicoMultisoftware, lockObterPedido, Auditado, unitOfWork, cancellationToken);
                }

                Servicos.Log.GravarInfo($"Finalizou Consulta CargaIntegracaoMercadoLivre: " + codigoCargaIntegracaoMercadoLivre);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private async void ProcessarNFesComplementaresAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        List<Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre> listaCargaNotasPendentesIntegracaoMercadoLivre = new List<Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre>();
                        listaCargaNotasPendentesIntegracaoMercadoLivre = await ConsultarNFesComplementaresPendetesAsync(unitOfWork, cancellationToken);

                        if (listaCargaNotasPendentesIntegracaoMercadoLivre.Count <= 0)
                        {
                            await Task.Delay(300000, cancellationToken);
                            continue;
                        }

                        foreach (Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre cargaNotasPendentesIntegracaoMercadoLivre in listaCargaNotasPendentesIntegracaoMercadoLivre)
                        {
                            string mensagemErro = string.Empty;

                            Servicos.Log.GravarInfo($"Fazendo Download das Notas da Carga: " + cargaNotasPendentesIntegracaoMercadoLivre.Carga.Codigo);

                            Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcMercadoLivre = new Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre(Cliente, _caminhoArquivos, _caminhoArquivosIntegracao, _prefixoMSMQ, _quantidadeThreadsExecutarRotaEFacility);
                            svcMercadoLivre.EfetuarDownloadNFeComplementaresPorCarga(out mensagemErro, cargaNotasPendentesIntegracaoMercadoLivre.Carga.Codigo, TipoServicoMultisoftware, unitOfWork);
                            AlterarSituacaoCargaNotasPendentesIntegracaoMercadoLivre(mensagemErro, cargaNotasPendentesIntegracaoMercadoLivre, unitOfWork, cancellationToken);

                            Servicos.Log.GravarInfo($"Finalizou Dowload das Notas da Carga: " + cargaNotasPendentesIntegracaoMercadoLivre.Carga.Codigo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                await Task.Delay(300000, cancellationToken);
            }
        }

        private async void ProcessarInicioAutomaticoRotaEFacilityAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
                        List<int> listaCargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarCodigosPorSituacaoAgConfirmacao(10);

                        if (listaCargaIntegracaoMercadoLivre.Count <= 0)
                        {
                            await Task.Delay(300000, cancellationToken);
                            continue;
                        }

                        Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcMercadoLivre = new Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre(Cliente, _caminhoArquivos, _caminhoArquivosIntegracao, _prefixoMSMQ, _quantidadeThreadsExecutarRotaEFacility);

                        for (int i = 0; i < listaCargaIntegracaoMercadoLivre.Count; i++)
                        {
                            int codigoCargaIntegracaoMercadoLivre = listaCargaIntegracaoMercadoLivre[i];

                            svcMercadoLivre.IniciarAutomaticamenteRotaEFacility(codigoCargaIntegracaoMercadoLivre, Auditado, unitOfWork);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                await Task.Delay(300000, cancellationToken);
            }
        }

        private async Task<List<Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre>> ConsultarNFesComplementaresPendetesAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.PainelNFeTransporte.CargaNotasPendentesIntegracaoMercadoLivre repCargaNotasPendentesIntegracaoMercadoLivre = new Repositorio.Embarcador.PainelNFeTransporte.CargaNotasPendentesIntegracaoMercadoLivre(unitOfWork, cancellationToken);

            return await repCargaNotasPendentesIntegracaoMercadoLivre.BuscarPorSituacaoPendenteDownloadAsync();
        }

        private void AlterarSituacaoCargaNotasPendentesIntegracaoMercadoLivre(string mensagemErro, Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre cargaNotasPendentesIntegracaoMercadoLivre, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.PainelNFeTransporte.CargaNotasPendentesIntegracaoMercadoLivre repCargaNotasPendentesIntegracaoMercadoLivre = new Repositorio.Embarcador.PainelNFeTransporte.CargaNotasPendentesIntegracaoMercadoLivre(unitOfWork, cancellationToken);
 
            if (string.IsNullOrEmpty(mensagemErro))
            {
                cargaNotasPendentesIntegracaoMercadoLivre.SituacaoDownloadNotas = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Concluido;
                cargaNotasPendentesIntegracaoMercadoLivre.MensagemRetorno = "Downloado das notas Concluído com sucesso.";
            }
            else
            {
                cargaNotasPendentesIntegracaoMercadoLivre.SituacaoDownloadNotas = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Falha;
                cargaNotasPendentesIntegracaoMercadoLivre.MensagemRetorno = mensagemErro;
            }

            repCargaNotasPendentesIntegracaoMercadoLivre.Atualizar(cargaNotasPendentesIntegracaoMercadoLivre);
        }

        
    }
}