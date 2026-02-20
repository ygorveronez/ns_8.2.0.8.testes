using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.BI;
using Infrastructure.Services.Cache;
using System.Web.Caching;

namespace SGT.WebAdmin.ProcessarEmissaoDocumentos
{
    public sealed class WindowsBackgroundService : BackgroundServiceBase
    {
        private Dictionary<int, Thread> _threadsExecutando = new Dictionary<int, Thread>();
        private System.Threading.Thread _threadTransmitirDocumento;
        private static string _keyCacheEmissaoDocumentos = "TransmitirDocumentoPorThread";
        private static bool _transmitirDocumentoPorThread = false;
        private static bool _emissaoDocumentosNFe = false;
        private static bool _emissaoDocumentosEtapaFrete = false;
        private string _webServiceConsultaCTe = string.Empty;

        public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, IConfiguration configuration) : base(logger, configuration)
        {
            _webServiceConsultaCTe = WebServiceConsultaCTe;
            _transmitirDocumentoPorThread = _configuration.GetValue<bool>("TransmitirDocumentoPorThread");
            _emissaoDocumentosNFe = _configuration.GetValue<bool>("EmissaoDocumentosNFe");
            _emissaoDocumentosEtapaFrete = _configuration.GetValue<bool>("EmissaoDocumentosEtapaFrete");
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
            
            DateTime dtAtualizouRegrasImpostos = DateTime.Now.AddMinutes(-11);

            while (!cancellationToken.IsCancellationRequested)
            {
                GC.Collect();
                try
                {                   
                    #region Criar parâmetro em cache e iniciar thread processar para transmitir os documentos

                    if (_transmitirDocumentoPorThread)
                    {
                        bool objCacheEmissaoDocumentos = CacheProvider.Instance.Get<bool>(_keyCacheEmissaoDocumentos);
                        if (!objCacheEmissaoDocumentos)
                            CacheProvider.Instance.Add(_keyCacheEmissaoDocumentos, true, TimeSpan.FromHours(12));

                        if (_threadTransmitirDocumento == null)
                        {
                            _threadTransmitirDocumento = new System.Threading.Thread(() => ProcessarTransmitirDocumentos(cancellationToken));
                            _threadTransmitirDocumento.Start();
                        }
                    }

                    #endregion Criar parâmetro em cache e iniciar thread processar a transmissão dos documentos

                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        if (dtAtualizouRegrasImpostos.AddMinutes(10) < DateTime.Now)
                        {
                            dtAtualizouRegrasImpostos = DateTime.Now;
                            Servicos.Embarcador.ISS.RegrasCalculoISS.GetInstance(unitOfWork).AtualizarRegrasISS(unitOfWork);
                            Servicos.Embarcador.ICMS.RegrasCalculoImpostos.GetInstance(unitOfWork).AtualizarRegrasICMS(unitOfWork);
                            Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.ConfigureApplicationEmissorDocumento(unitOfWork);
                        }

                        #region Validar threads ativas na aplicação

                        Repositorio.ControleThread repControleThread = new Repositorio.ControleThread(unitOfWork);
                        if (_emissaoDocumentosNFe)
                        {
                            Dominio.Entidades.ControleThread controleThreadNFe = repControleThread.BuscarPorThread("EmissaoDocumentosNFe");
                            Dominio.Entidades.ControleThread controleThreadNFeIntegracao = repControleThread.BuscarPorThread("EmissaoDocumentosNFeIntegracao");

                            if ((controleThreadNFe?.Ativo ?? false) || (controleThreadNFeIntegracao?.Ativo ?? false))
                            {
                                Servicos.Log.GravarAdvertencia("Processo abortado! Não é possível utilizar o serviço caso a thread EmissaoDocumentosNFe ou EmissaoDocumentosNFeIntegracao esteja ativa, favor verificar.");
                                await Task.Delay(10000, cancellationToken);
                                continue;
                            }
                        }

                        if (_emissaoDocumentosEtapaFrete)
                        {
                            Dominio.Entidades.ControleThread controleThreadEtapaFrete = repControleThread.BuscarPorThread("EmissaoDocumentosEtapaFrete");
                            Dominio.Entidades.ControleThread controleThreadEtapaFreteIntegracao = repControleThread.BuscarPorThread("EmissaoDocumentosEtapaFreteIntegracao");

                            if ((controleThreadEtapaFrete?.Ativo ?? false) || (controleThreadEtapaFreteIntegracao?.Ativo ?? false))
                            {
                                Servicos.Log.GravarAdvertencia("Processo abortado! Não é possível utilizar o serviço caso a thread EmissaoDocumentosEtapaFrete ou EmissaoDocumentosEtapaFreteIntegracao esteja ativa, favor verificar.");
                                await Task.Delay(10000, cancellationToken);
                                continue;
                            }
                        }

                        #endregion Validar threads ativas na aplicação

                        int quantidadeThreadsAtivas = _threadsExecutando.Count;

                        for (int i = quantidadeThreadsAtivas - 1; i >= 0; i--)
                        {
                            if (_threadsExecutando.ElementAt(i).Value.IsAlive)
                                continue;

                            _threadsExecutando.Remove(_threadsExecutando.ElementAt(i).Key);
                        }

                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();

                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasEmissaoDocumentos = ObterCargaEmissaoDocumentos(configuracao, unitOfWork);

                        if (cargasEmissaoDocumentos.Count <= 0)
                        {
                            await Task.Delay(10000, cancellationToken);
                            continue;
                        }

                        //Agrupar Registros por Empresa
                        var queryGroup = cargasEmissaoDocumentos.GroupBy(x => new { x.Empresa.Codigo }).Select(y => new { CodigoEmpresa = y.Key.Codigo, listaCargas = y });

                        foreach (var item in queryGroup) 
                        {
                            System.Threading.Thread thread = new System.Threading.Thread(() => ProcessarCargaEmissaoDocumentosNaEtapaNFe(configuracao, item.listaCargas.ToList(), cancellationToken));

                            thread.Start();

                            _threadsExecutando.Add(item.CodigoEmpresa, thread);
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

        private List<Dominio.Entidades.Embarcador.Cargas.Carga> ObterCargaEmissaoDocumentos(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            List<int> codigosEmpresaExecutando = _threadsExecutando.Select(o => o.Key).ToList();
            int segundos = -configuracao.TempoSegundosParaInicioEmissaoDocumentos;
            int limiteRegistros = 5;
            int inicioContadorPedidos = 0;
            int fimContadorPedidos = 0;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> retorno = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            if (_emissaoDocumentosNFe)
                retorno.AddRange(repositorioCarga.BuscarCargasAutorizadasEmissaoNaEtapaNFePadraoEIntegracao(codigosEmpresaExecutando, DateTime.Now.AddSeconds(segundos), limiteRegistros, configuracao.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos, configuracao.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, "DataInicioGeracaoCTes", "asc"));

            if (_emissaoDocumentosEtapaFrete && retorno.Count() < limiteRegistros)
                retorno.AddRange(repositorioCarga.BuscaCodigosCargasAutorizadasEmissaoNaEtapaDeFretePadraoEIntegracao(codigosEmpresaExecutando, DateTime.Now.AddSeconds(segundos), limiteRegistros, configuracao.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos, configuracao.ExigirCargaRoteirizada, 0, 0, inicioContadorPedidos, fimContadorPedidos));

            return retorno;
        }

        private void ProcessarCargaEmissaoDocumentosNaEtapaNFe(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas, CancellationToken cancellationToken)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                    int tipoEnvio = configuracao.CodigoTipoEnvioEmissaoCTe;

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in listaCargas)
                    {
                        Servicos.Log.GravarInfo("Iniciando emissao documentos " + carga.Codigo.ToString(), "SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe");

                        serCarga.ValidarEmissaoDocumentosCarga(carga.Codigo, unitOfWork, TipoServicoMultisoftware, _webServiceConsultaCTe, tipoEnvio, true);
                        serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                        Servicos.Log.GravarInfo("Finalizada emissao documentos " + carga.Codigo.ToString(), "SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe");
                        unitOfWork.FlushAndClear();
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private async void ProcessarTransmitirDocumentos(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                        int limiteRegistros = 40;
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCte = repCargaCTe.BuscarCargasCTePendentesTransmissao(limiteRegistros, "Codigo", "asc");

                        if (listaCargaCte.Count <= 0)
                        {
                            await Task.Delay(10000, cancellationToken);
                            continue;
                        }

                        servicoCte.ProcessarTransmisaoDocumentos(listaCargaCte, unitOfWork);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                if (cancellationToken.IsCancellationRequested)
                    return;

                await Task.Delay(5000, cancellationToken);
            }
        }
    }
}