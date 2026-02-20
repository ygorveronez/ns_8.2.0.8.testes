using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.Cache;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/Integracao")]
    public class IntegracaoController : BaseController
    {
        #region Construtores

        public IntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion  Construtores

        #region Atributos Privados Somente Leitura

        private List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> _tipoIntegracaoExistentes;
        private const string ConfiguracaoIntegracaoKey = "ConfiguracaoIntegracao";

        #endregion Atributos Privados Somente Leitura

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDados(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoAvon repConfiguracaoIntegracaoAvon = new Repositorio.Embarcador.Configuracoes.IntegracaoAvon(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoSAD repositorioConfiguracaoIntegracaoSAD = new Repositorio.Embarcador.Configuracoes.IntegracaoSAD(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon> configuracoesIntegracaoAvon = await repConfiguracaoIntegracaoAvon.BuscarTodosAsync();
                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD> configuracoesIntegracaoSAD = await repositorioConfiguracaoIntegracaoSAD.BuscarTodosAsync();
                _tipoIntegracaoExistentes = repTipoIntegracao.BuscarTipos();

                var retorno = new
                {
                    TiposExistentes = _tipoIntegracaoExistentes,

                    IntegracaoNatura = ObterIntegracaoNatura(configuracaoIntegracao),
                    IntegracaoOpentech = ObterIntegracaoOpentech(configuracaoIntegracao, unidadeDeTrabalho),
                    IntegracaoDTe = ObterIntegracaoDTe(configuracaoIntegracao),
                    IntegracaoMundialRisk = ObterIntegracaoMundialRisk(configuracaoIntegracao),
                    IntegracaoLogiun = ObterIntegracaoLogiun(configuracaoIntegracao),
                    IntegracaoBuonny = ObterIntegracaoBuonny(configuracaoIntegracao),
                    IntegracaoAvior = ObterIntegracaoAvior(configuracaoIntegracao),
                    IntegracaoNOX = ObterIntegracaoNOX(configuracaoIntegracao),
                    IntegracaoCarrefour = ObterIntegracaoCarrefur(configuracaoIntegracao),
                    IntegracaoGoldenService = ObterIntegracaoGoldenService(configuracaoIntegracao),
                    IntegracaoGPA = ObterIntegracaoGPA(configuracaoIntegracao),
                    IntegracaoOrtec = ObterIntegracaoOrtec(configuracaoIntegracao),
                    IntegracaoAPIGoogle = ObterIntegracaoAPIGoogle(configuracaoIntegracao),
                    IntegracaoPamCard = ObterIntegracaoPamCard(configuracaoIntegracao),
                    IntegracaoPiracanjuba = ObterIntegracaoPiracanjuba(unidadeDeTrabalho),
                    IntegracaoRaster = ObterIntegracaoRaster(configuracaoIntegracao),
                    IntegracaoUnileverFourKites = ObterIntegracaoUnileverFourKites(configuracaoIntegracao),
                    IntegracaoDigibee = ObterIntegracaoDigibee(configuracaoIntegracao),
                    IntegracaoTelerisco = ObterIntegracaoTelerisco(configuracaoIntegracao),
                    IntegracaoCargoX = ObterIntegracaoCargoX(configuracaoIntegracao),
                    IntegracaoRiachuelo = ObterIntegracaoRiachuelo(configuracaoIntegracao),
                    IntegracaoKrona = ObterIntegracaoKrona(configuracaoIntegracao),
                    IntegracaoInfolog = ObterIntegracaoInfolog(configuracaoIntegracao),
                    IntegracaoPH = ObterIntegracaoPH(configuracaoIntegracao),
                    IntegracaoBoticario = ObterIntegracaoBoticario(configuracaoIntegracao, unidadeDeTrabalho),
                    IntegracaoToledo = ObterIntegracaoToledo(configuracaoIntegracao),
                    IntegracaoQbit = ObterIntegracaoQbit(configuracaoIntegracao),
                    ListaConfiguracoesIntegracaoAvon = configuracoesIntegracaoAvon.Select(o => new
                    {
                        o.Codigo,
                        Empresa = new { o.Empresa.Codigo, o.Empresa.Descricao },
                        o.EnterpriseID,
                        o.TokenHomologacao,
                        o.TokenProducao
                    }).ToList(),
                    ListaConfiguracoesIntegracaoSAD = configuracoesIntegracaoSAD.Select(o => new
                    {
                        o.Codigo,
                        CentroDescarregamento = new
                        {
                            Codigo = o.CentroDescarregamento?.Codigo ?? 0,
                            Descricao = o.CentroDescarregamento?.Descricao ?? ""
                        },
                        o.Token,
                        o.URLIntegracaoSADBuscarSenha,
                        o.URLIntegracaoSADFinalizarAgenda,
                        o.URLIntegracaoSADCancelarAgenda
                    }).ToList(),
                    IntegracaoAdagio = ObterIntegracaoAdagio(configuracaoIntegracao),
                    IntegracaoTrizzy = ObterIntegracaoTrizzy(configuracaoIntegracao),
                    IntegracacaoAX = ObterIntegracaoAX(configuracaoIntegracao),
                    PossuiIntegracaoSAD = configuracaoIntegracao?.PossuiIntegracaoSAD ?? false,
                    IntegracaoCobasi = ObterIntegracaoCobasi(configuracaoIntegracao),
                    //QuantidadeNotificacaoEmillenium = configuracaoIntegracao?.QuantidadeNotificacaoEmillenium ?? 0,
                    //EmailsNotificacaoEmillenium = configuracaoIntegracao?.EmailsNotificacaoEmillenium ?? string.Empty,
                    PossuiIntegracaoMichelin = configuracaoIntegracao?.PossuiIntegracaoMichelin ?? false,
                    PossuiIntegracaoGadle = configuracaoIntegracao?.PossuiIntegracaoGadle ?? false,
                    IntegracaoMichelin = ObterIntegracaoMichelin(configuracaoIntegracao),
                    IntegracaoTelhaNorte = ObterIntegracaoTelhaNorte(configuracaoIntegracao, unidadeDeTrabalho),
                    IntegracaoCadastrosMulti = ObterIntegracaoCadastrosMulti(configuracaoIntegracao, unidadeDeTrabalho),
                    IntegracaoTotvs = ObterIntegracaoTotvs(configuracaoIntegracao),
                    IntegracaoAngelLira = ObterIntegracaoAngelLira(unidadeDeTrabalho),
                    IntegracaoA52 = ObterIntegracaoA52(unidadeDeTrabalho),
                    IntegracaoBBC = ObterIntegracaoBBC(unidadeDeTrabalho),
                    IntegracaoMercadoLivre = ObterIntegracaoMercadoLivre(unidadeDeTrabalho),
                    IntegracaoKuehneNagel = ObterIntegracaoKuehneNagel(unidadeDeTrabalho),
                    IntegracaoDansales = ObterIntegracaoDansales(unidadeDeTrabalho),
                    IntegracaoTarget = ObterIntegracaoTarget(unidadeDeTrabalho),
                    IntegracaoExtratta = ObterIntegracaoExtratta(unidadeDeTrabalho),
                    IntegracaoMicDta = ObterIntegracaoMicDta(unidadeDeTrabalho),
                    IntegracaoGadle = ObterIntegracaoGadle(unidadeDeTrabalho),
                    IntegracaoOnetrust = ObterIntegracaoOnetrust(configuracaoIntegracao),
                    IntegracaoInforDoc = ObterIntegracaoInforDoc(unidadeDeTrabalho),
                    IntegracaoSintegra = ObterIntegracaoSintegra(configuracaoIntegracao),
                    IntegracaoUltragaz = ObterIntegracaoUltragaz(configuracaoIntegracao),
                    IntegracaoIsis = ObterIntegracaoIsis(unidadeDeTrabalho),
                    IntegracaoMagalu = ObterIntegracaoMagalu(unidadeDeTrabalho),
                    IntegracaoGSW = ObterIntegracaoGSW(unidadeDeTrabalho),
                    IntegracaoArquivei = ObterIntegracaoArquivei(unidadeDeTrabalho),
                    IntegracaoSaintGobain = ObterIntegracaoSaintGobain(unidadeDeTrabalho, configuracaoIntegracao),
                    IntegracaoDPA = ObterIntegracaoDPA(unidadeDeTrabalho),
                    IntegracaoRavex = ObterIntegracaoRavex(unidadeDeTrabalho),
                    IntegracaoCTASmart = ObterIntegracaoCTASmart(unidadeDeTrabalho, configuracaoIntegracao),
                    IntegracaoHavan = ObterIntegracaoHavan(unidadeDeTrabalho),
                    IntegracaoFrota162 = ObterIntegracaoFrota162(unidadeDeTrabalho),
                    IntegracaoDexco = ObterIntegracaoDexco(unidadeDeTrabalho),
                    IntegracaoIntercab = ObterIntegracaoIntercab(unidadeDeTrabalho),
                    IntegracaoProtheus = ObterIntegracaoProtheus(unidadeDeTrabalho),
                    IntegracaoUnilever = ObterIntegracaoUnilever(unidadeDeTrabalho),
                    IntegracaoSimonetti = ObterIntegracaoSimonetti(unidadeDeTrabalho),
                    IntegracaoBrasilRisk = ObterIntegracaoBrasilRisk(configuracaoIntegracao, unidadeDeTrabalho),
                    IntegracaoNstech = ObterIntegracaoNstech(unidadeDeTrabalho),
                    IntegracaoMarisa = ObterIntegracaoMarisa(unidadeDeTrabalho),
                    IntegracaoDeca = ObterIntegracaoDeca(unidadeDeTrabalho),
                    IntegracaoVLI = ObterIntegracaoVLI(unidadeDeTrabalho),
                    IntegracaoMarilan = ObterIntegracaoMarilan(unidadeDeTrabalho),
                    IntegracaoCorreios = ObterIntegracaoCorreios(configuracaoIntegracao, unidadeDeTrabalho),
                    IntegracaoArcelorMittal = ObterIntegracaoArcelorMittal(unidadeDeTrabalho),
                    IntegracaoEMP = ObterIntegracaoEMP(unidadeDeTrabalho),
                    IntegracaoTicketLog = ObterIntegracaoTicketLog(unidadeDeTrabalho),
                    IntegracaoEmillenium = ObterIntegracaoEmillenium(unidadeDeTrabalho, configuracaoIntegracao),
                    IntegracaoGNRE = ObterIntegracaoGNRE(unidadeDeTrabalho),
                    IntegracaoDigitalCom = ObterIntegracaoDigitalCom(unidadeDeTrabalho),
                    IntegracaoLBC = ObterIntegracaoLBC(unidadeDeTrabalho),
                    IntegracaoTecnorisk = ObterIntegracaoTecnorisk(unidadeDeTrabalho),
                    IntegracaoDestinadosSAP = ObterIntegracaoDestinadosSAP(unidadeDeTrabalho),
                    IntegracaoNeokohm = ObterIntegracaoNeokohm(unidadeDeTrabalho),
                    IntegracaoMoniloc = ObterIntegracaoMoniloc(unidadeDeTrabalho),
                    IntegracaoApisulLog = await ObterIntegracaoApisulLogAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoFroggr = ObterIntegracaoFroggr(unidadeDeTrabalho),
                    IntegracaoSAP = ObterIntegracaoSAP(unidadeDeTrabalho),
                    IntegracaoYPE = ObterIntegracaoYPE(unidadeDeTrabalho),
                    IntegracaoOTM = ObterIntegracaoOTM(unidadeDeTrabalho),
                    IntegracaoSIC = ObterIntegracaoSIC(unidadeDeTrabalho),
                    IntegracaoFrimesa = ObterIntegracaoFrimesa(unidadeDeTrabalho),
                    IntegracaoLoggi = await ObterIntegracaoLoggiAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoCTePagamentoLoggi = await ObterIntegracaoCTePagamentoLoggiAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoValoresCTeLoggi = await ObterIntegracaoValoresCTeLoggiAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoJJ = await ObterIntegracaoJJAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoKlios = await ObterIntegracaoKliosAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoSAPV9 = await ObterIntegracaoSAPV9Async(unidadeDeTrabalho, cancellationToken),
                    IntegracaoSAPST = await ObterIntegracaoSAPSTAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoSAP_API4 = await ObterIntegracaoSAP_API4Async(unidadeDeTrabalho, cancellationToken),
                    IntegracaoLogRisk = await ObterIntegracaoLogRiskAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoBrado = await ObterIntegracaoBradoAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoEShip = await ObterIntegracaoEShipAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoDiageo = await ObterIntegracaoDiageoAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoP44 = await ObterIntegracaoP44Async(unidadeDeTrabalho, cancellationToken),
                    IntegracaoYandeh = await ObterIntegracaoYandehAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoEFrete = await ObterIntegracaoEFreteAsync(unidadeDeTrabalho, configuracaoIntegracao, cancellationToken),
                    IntegracaoOpenTech = await ObterIntegracaoOpenTechAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoBalancaKIKI = await ObterIntegracaoBalancaKIKIAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoComprovei = await ObterIntegracaoComproveiAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoComproveiRota = await ObterIntegracaoComproveiRotaAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoKMM = await ObterIntegracaoKMMAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoLogvett = await ObterIntegracaoLogvettAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoAtlas = await ObterIntegracaoAtlasAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoFlora = await ObterIntegracaoFloraAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoCalisto = await ObterIntegracaoCalistoAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoTrizy = await ObterIntegracaoTrizyAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoObramax = await ObterIntegracaoObramaxAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoObramaxCTE = await ObterIntegracaoObramaxCTEAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoObramaxNFE = await ObterIntegracaoObramaxNFEAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoObramaxProvisao = await ObterIntegracaoObramaxProvisaoAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoShopee = await ObterIntegracaoShopeeAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoItalac = await ObterIntegracaoItalacAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoItalacFatura = await ObterIntegracaoItalacFaturaAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoPager = await ObterIntegracaoPagerAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoElectrolux = await ObterIntegracaoElectroluxAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoWhatsApp = await ObterIntegracaoWhatsAppAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoLoggiFaturas = await ObterIntegracaoLoggiFaturasAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoRuntec = await ObterIntegracaoRuntecAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoATSLog = await ObterIntegracaoATSLogAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoCTeAnterioresLoggi = await ObterIntegracaoCTeAnterioresLoggiAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoCamil = await ObterIntegracaoCamilAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoBuntech = await ObterIntegracaoBuntechAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoRouteasy = await ObterIntegracaoRouteasyAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoConfirmaFacil = await ObterIntegracaoConfirmaFacilAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoBind = await ObterIntegracaoBindAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoCebrace = await ObterIntegracaoCebraceAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoMondelez = await ObterIntegracaoMondelezAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoTrizyEventos = await ObterIntegracaoTrizyEventosAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoVector = await ObterIntegracaoVectorAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoGrupoSC = await ObterIntegracaoGrupoSCAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoFusion = await ObterIntegracaoFusionAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoSalesforce = await ObterIntegracaoSalesforceAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoConecttec = await ObterIntegracaoConecttecAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoMars = await ObterIntegracaoMarsAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoGlobus = await ObterIntegracaoGlobusAsync(unidadeDeTrabalho, cancellationToken),
                    ConfiguracaoAcessoViaToken = await ObterConfiguracaoAcessoViaTokenAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoFS = await ObterIntegracaoFSAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoVedacit = await ObterIntegracaoVedacitAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoTransSat = await ObterIntegracaoTransSatAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoJDEFaturas = await ObterIntegracaoJDEFaturasAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoOlfar = await ObterIntegracaoOlfarAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoMigrate = await ObterIntegracaoMigrateAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoEfesus = await ObterIntegracaoEfesusAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoCassol = await ObterIntegracaoCassolAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoWeberChile = await ObterIntegracaoWeberChileAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoLactalis = await ObterIntegracaoLactalisAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoPortalCabotagem = await ObterIntegracaoPortalCabotagem(unidadeDeTrabalho, cancellationToken),
                    IntegracaoSistemaTransben = await ObterIntegracaoSistemaTransbenAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoATSSmartWeb = await ObterIntegracaoATSSmartWebAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoVSTrack = await ObterIntegracaoVSTrackAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoTrafegus = await ObterIntegracaoTrafegusAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoYMS = await ObterIntegracaoYMSAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoHUB = await ObterIntegracaoHUBAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoOnisys = await ObterIntegracaoOnisysAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoSkymark = await ObterIntegracaoSkymarkAsync(unidadeDeTrabalho, cancellationToken),
                    IntegracaoSenior = await ObterIntegracaoSeniorAsync(unidadeDeTrabalho, cancellationToken),
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar as configurações de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterIntegracoesConfiguradas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
                Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoTransSat repIntegracaoTransSat = new Repositorio.Embarcador.Configuracoes.IntegracaoTransSat(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT> operadorasCIOTExistentes = repConfiguracaoCIOT.BuscarOperadorasDisponiveis();

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTransSat integracaoTransSat = repIntegracaoTransSat.Buscar();

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposExistentes = repTipoIntegracao.BuscarTipos();

                var retorno = new
                {
                    OperadorasCIOTExistentes = operadorasCIOTExistentes,
                    TiposExistentes = tiposExistentes,
                    PossuiIntegracaoBrasilRisk = configuracaoIntegracao?.PossuiIntegracaoBrasilRisk ?? false,
                    PossuiIntegracaoMundialRisk = configuracaoIntegracao?.PossuiIntegracaoMundialRisk ?? false,
                    PossuiIntegracaoLogiun = configuracaoIntegracao?.PossuiIntegracaoLogiun ?? false,
                    PossuiIntegracaoGoldenService = configuracaoIntegracao?.PossuiIntegracaoGoldenService ?? false,
                    PossuiIntegracaoNOX = configuracaoIntegracao?.PossuiIntegracaoNOX ?? false,
                    PossuiIntegracaoRaster = configuracaoIntegracao?.PossuiIntegracaoRaster ?? false,
                    PossuiIntegracaoPH = configuracaoIntegracao?.PossuiIntegracaoPH ?? false,
                    PossuiIntegracaoTrizy = configuracaoIntegracao?.PossuiIntegracaoTrizy ?? false,
                    PossuiIntegracaoAX = configuracaoIntegracao?.PossuiIntegracaoAX ?? false,
                    PossuiIntegracaoMichelin = configuracaoIntegracao?.PossuiIntegracaoMichelin ?? false,
                    PossuiIntegracaoMarfrig = configuracaoIntegracao?.PossuiIntegracaoMarfrig ?? false,
                    PossuiIntegracaoDeCadastrosMulti = configuracaoIntegracao?.PossuiIntegracaoDeCadastrosMulti ?? false,
                    PossuiIntegracaoDeTotvs = configuracaoIntegracao?.PossuiIntegracaoDeTotvs ?? false,
                    PossuiIntegracaoTransSat = integracaoTransSat?.PossuiIntegracao ?? false
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar as configurações de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Salvar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string codigoMatrizNatura = Request.Params("CodigoMatrizNatura");
                string usuarioNatura = Request.Params("UsuarioNatura");
                string senhaNatura = Request.Params("SenhaNatura");
                string dominioOpenTech = Request.Params("DominioOpenTech");
                string senhaOpenTech = Request.Params("SenhaOpenTech");
                string usuarioOpenTech = Request.Params("UsuarioOpenTech");
                string urlOpenTech = Request.Params("URLOpenTech");
                string codigoIntegradorEFrete = Request.Params("CodigoIntegradorEFrete");
                string URLRecepcaoDTe = Request.Params("URLRecepcaoDTe");
                string usuarioEFrete = Request.Params("UsuarioEFrete");
                string senhaEFrete = Request.Params("SenhaEFrete");
                string usuarioBrasilRisk = Request.Params("UsuarioBrasilRisk");
                string senhaBrasilRisk = Request.Params("SenhaBrasilRisk");
                string urlHomologacaoBrasilRisk = Request.Params("URLHomologacaoBrasilRisk");
                string urlProducaoBrasilRisk = Request.Params("URLProducaoBrasilRisk");
                string urlBrasilRiskGestao = Request.Params("URLBrasilRiskGestao");
                string urlBrasilRiskVeiculoMotorista = Request.Params("URLBrasilRiskVeiculoMotorista");
                string cnpjEmbarcadorBrasilRisk = Request.Params("CNPJEmbarcadorBrasilRisk");
                string usuarioMundialRisk = Request.Params("UsuarioMundialRisk");
                string senhaMundialRisk = Request.Params("SenhaMundialRisk");
                string urlHomologacaoMundialRisk = Request.Params("URLHomologacaoMundialRisk");
                string urlProducaoMundialRisk = Request.Params("URLProducaoMundialRisk");
                string usuarioLogiun = Request.Params("UsuarioLogiun");
                string senhaLogiun = Request.Params("SenhaLogiun");
                string urlHomologacaoLogiun = Request.Params("URLHomologacaoLogiun");
                string urlProducaoLogiun = Request.Params("URLProducaoLogiun");
                string cnpjClienteBuonny = Request.Params("CNPJClienteBuonny");
                string tokenBuonny = Request.Params("TokenBuonny");
                string urlHomologacaoBuonny = Request.Params("URLHomologacaoBuonny");
                string urlProducaoBuonny = Request.Params("URLProducaoBuonny");
                string urlRestHomologacaoBuonny = Request.Params("URLRestHomologacaoBuonny");
                string urlRestProducaoBuonny = Request.Params("URLRestProducaoBuonny");
                string urlAvior = Request.Params("URLAvior");
                string usuarioAvior = Request.Params("UsuarioAvior");
                string senhaAvior = Request.Params("SenhaAvior");
                string senhaNOX = Request.Params("SenhaNOX");
                string tokenNOX = Request.Params("TokenNOX");
                string urlHomologacaoNOX = Request.Params("URLHomologacaoNOX");
                string urlProducaoNOX = Request.Params("URLProducaoNOX");
                string usuarioNOX = Request.Params("UsuarioNOX");
                string cnpjMatrizNOX = Request.Params("CNPJMatrizNOX");
                string codigoGoldenService = Request.Params("CodigoGoldenService");
                string idGoldenService = Request.Params("IdGoldenService");
                string senhaGoldenService = Request.Params("SenhaGoldenService");
                string urlHomologacaoGoldenService = Request.Params("URLHomologacaoGoldenService");
                string urlProducaoGoldenService = Request.Params("URLProducaoGoldenService");
                string urlHomologacaoGPA = Request.Params("URLHomologacaoGPA");
                string urlProducaoGPA = Request.Params("URLProducaoGPA");
                string urlOrtec = Request.GetStringParam("URLOrtec");
                string usuarioOrtec = Request.GetStringParam("UsuarioOrtec");
                string senhaOrtec = Request.GetStringParam("SenhaOrtec");
                string APIKeyGoogle = Request.GetStringParam("APIKeyGoogle");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding geoServiceGeocoding = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding>("GeoServiceGeocoding", Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Google);
                string servidorRouteOSM = Request.GetStringParam("ServidorRouteOSM");
                string servidorRouteGoogleOrTools = Request.GetStringParam("ServidorRouteGoogleOrTools");
                string servidorNominatim = Request.GetStringParam("ServidorNominatim");

                bool possuiIntegracaoNOX = Request.GetBoolParam("PossuiIntegracaoNOX");
                bool possuiIntegracaoGoldenService = Request.GetBoolParam("PossuiIntegracaoGoldenService");
                bool.TryParse(Request.Params("PossuiIntegracaoBrasilRisk"), out bool possuiIntegracaoBrasilRisk);
                bool.TryParse(Request.Params("PossuiIntegracaoMundialRisk"), out bool possuiIntegracaoMundialRisk);
                bool.TryParse(Request.Params("PossuiIntegracaoLogiun"), out bool possuiIntegracaoLogiun);
                bool.TryParse(Request.Params("EnviarOcorrenciaNaturaAutomaticamente"), out bool enviarOcorrenciaNaturaAutomaticamente);
                bool.TryParse(Request.Params("UtilizarValorFreteTMSNatura"), out bool utilizarValorFreteTMSNatura);
                bool.TryParse(Request.Params("PossuiIntegracaoGPA"), out bool possuiIntegracaoGPA);
                bool.TryParse(Request.Params("IntegrarVeiculoMotorista"), out bool integrarVeiculoMotorista);
                bool.TryParse(Request.Params("IntegrarColetaOpentech"), out bool integrarColetaOpentech);
                bool.TryParse(Request.Params("EnviarCodigoEmbarcadorProdutoOpentech"), out bool enviarCodigoEmbarcadorProdutoOpentech);
                bool.TryParse(Request.Params("AtualizarVeiculoMotoristaOpentech"), out bool atualizarVeiculoMotoristaOpentech);
                bool.TryParse(Request.Params("CadastrarMotoristaAntesConsultarBuonny"), out bool cadastrarMotoristaAntesConsultarBuonny);

                int.TryParse(Request.Params("CodigoClienteOpenTech"), out int codigoClienteOpenTech);
                int.TryParse(Request.Params("CodigoPASOpenTech"), out int codigoPASOpenTech);
                int.TryParse(Request.Params("MatrizEFrete"), out int codigoMatrizEFrete);
                int.TryParse(Request.Params("TempoHorasConsultasBuonny"), out int tempoHorasConsultasBuonny);
                int.TryParse(Request.Params("CodigoProdutoColetaOpentech"), out int codigoProdutoColetaOpentech);
                int.TryParse(Request.Params("CodigoProdutoColetaEmbarcadorOpentech"), out int codigoProdutoColetaEmbarcadorOpentech);
                int.TryParse(Request.Params("CodigoProdutoColetaTransportadorOpentech"), out int codigoProdutoColetaTransportadorOpentech);
                int.TryParse(Request.Params("CodigoProdutoPadraoOpentech"), out int codigoProdutoPadraoOpentech);
                int.TryParse(Request.Params("ComponenteFreteValorNFTPEMP"), out int codigoComponenteFreteValorNFTPEMP);
                int.TryParse(Request.Params("ComponenteImpostosNFTPEMP"), out int codigoComponenteImpostosNFTPEMP);
                int.TryParse(Request.Params("ComponenteValorTotalPrestacaoNFTPEMP"), out int codigoComponenteValorTotalPrestacaoNFTPEMP);

                int codigoEmpresaFixaPamCard = Request.GetIntParam("EmpresaFixaPamCard");
                int codigoEmpresaFixaTelerisco = Request.GetIntParam("EmpresaFixaTelerisco");

                decimal.TryParse(Request.Params("ValorBaseOpenTech"), out decimal valorBaseOpenTech);
                decimal.TryParse(Request.Params("ValorBaseBrasilRisk"), out decimal valorBaseBrasilRisk);

                Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho, cancellationToken);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
                _tipoIntegracaoExistentes = await repTipoIntegracao.BuscarTiposAsync();

                await unidadeDeTrabalho.StartAsync();

                if (configuracaoIntegracao == null)
                    configuracaoIntegracao = new Dominio.Entidades.Embarcador.Configuracoes.Integracao();
                else
                    configuracaoIntegracao.Initialize();

                configuracaoIntegracao.APIKeyGoogle = APIKeyGoogle;
                configuracaoIntegracao.GeoServiceGeocoding = geoServiceGeocoding;
                configuracaoIntegracao.ServidorRouteOSM = servidorRouteOSM;
                configuracaoIntegracao.ServidorRouteGoogleOrTools = servidorRouteGoogleOrTools;
                configuracaoIntegracao.ServidorNominatim = servidorNominatim;

                configuracaoIntegracao.CodigoMatrizNatura = codigoMatrizNatura;
                configuracaoIntegracao.UsuarioNatura = usuarioNatura;
                configuracaoIntegracao.SenhaNatura = senhaNatura;
                configuracaoIntegracao.UtilizarValorFreteTMSNatura = utilizarValorFreteTMSNatura;
                configuracaoIntegracao.EnviarOcorrenciaNaturaAutomaticamente = enviarOcorrenciaNaturaAutomaticamente;

                configuracaoIntegracao.CodigoClienteOpenTech = codigoClienteOpenTech;
                configuracaoIntegracao.CodigoPASOpenTech = codigoPASOpenTech;
                configuracaoIntegracao.CodigoProdutoColetaOpentech = codigoProdutoColetaOpentech;
                configuracaoIntegracao.CodigoProdutoColetaEmbarcadorOpentech = codigoProdutoColetaEmbarcadorOpentech;
                configuracaoIntegracao.CodigoProdutoColetaTransportadorOpentech = codigoProdutoColetaTransportadorOpentech;
                configuracaoIntegracao.CodigoProdutoPadraoOpentech = codigoProdutoPadraoOpentech;
                configuracaoIntegracao.DominioOpenTech = dominioOpenTech;
                configuracaoIntegracao.SenhaOpenTech = senhaOpenTech;
                configuracaoIntegracao.URLOpenTech = urlOpenTech;
                configuracaoIntegracao.UsuarioOpenTech = usuarioOpenTech;
                configuracaoIntegracao.ValorBaseOpenTech = valorBaseOpenTech;
                configuracaoIntegracao.IntegrarVeiculoMotorista = integrarVeiculoMotorista;
                configuracaoIntegracao.IntegrarColetaOpentech = integrarColetaOpentech;
                configuracaoIntegracao.EnviarCodigoEmbarcadorProdutoOpentech = enviarCodigoEmbarcadorProdutoOpentech;
                configuracaoIntegracao.AtualizarVeiculoMotoristaOpentech = atualizarVeiculoMotoristaOpentech;
                configuracaoIntegracao.IntegrarRotaCargaOpentech = Request.GetBoolParam("IntegrarRotaCargaOpentech");
                configuracaoIntegracao.CodigoProdutoVeiculoComLocalizadorOpenTech = Request.GetIntParam("CodigoProdutoVeiculoComLocalizadorOpenTech");
                configuracaoIntegracao.NotificarFalhaIntegracaoOpentech = Request.GetBoolParam("NotificarFalhaIntegracaoOpentech");
                configuracaoIntegracao.EmailsNotificacaoFalhaIntegracaoOpentech = Request.GetStringParam("EmailsNotificacaoFalhaIntegracaoOpentech");
                configuracaoIntegracao.CodigoProdutoParaValidarSomenteVeiculoMotoristaSemUsoRastreador = Request.GetIntParam("CodigoProdutoParaValidarSomenteVeiculoMotoristaSemUsoRastreador");
                configuracaoIntegracao.PermitirTransportadorReenviarIntegracoesComProblemasOpenTech = Request.GetBoolParam("PermitirTransportadorReenviarIntegracoesComProblemasOpenTech");
                configuracaoIntegracao.EnviarDataPrevisaoEntregaDataCarregamentoOpentech = Request.GetBoolParam("EnviarDataPrevisaoEntregaDataCarregamentoOpentech");
                configuracaoIntegracao.EnviarDataAtualNaDataPrevisaoOpentech = Request.GetBoolParam("EnviarDataAtualNaDataPrevisaoOpentech");
                configuracaoIntegracao.IntegrarCargaOpenTechV10 = Request.GetBoolParam("IntegrarCargaOpenTechV10");
                configuracaoIntegracao.CalcularPrevisaoEntregaComBaseDistanciaOpentech = Request.GetBoolParam("CalcularPrevisaoEntregaComBaseDistanciaOpentech");

                configuracaoIntegracao.EnviarDataPrevisaoSaidaPedidoOpentech = Request.GetBoolParam("EnviarDataPrevisaoSaidaPedidoOpentech");
                configuracaoIntegracao.EnviarInformacoesRastreadorCavaloOpentech = Request.GetBoolParam("EnviarInformacoesRastreadorCavaloOpentech");
                configuracaoIntegracao.EnviarCodigoIntegracaoRotaCargaOpenTech = Request.GetBoolParam("EnviarCodigoIntegracaoRotaCargaOpenTech");
                configuracaoIntegracao.EnviarNrfonecelBrancoOpenTech = Request.GetBoolParam("EnviarNrfonecelBrancoOpenTech");
                configuracaoIntegracao.EnviarPlacaVeiculoSeNaoExistirNumeroFrotaOpenTech = Request.GetBoolParam("EnviarPlacaVeiculoSeNaoExistirNumeroFrotaOpenTech");
                configuracaoIntegracao.EnviarValorNotasValorDocOpenTech = Request.GetBoolParam("EnviarValorNotasValorDocOpenTech");
                configuracaoIntegracao.EnviarCodigoIntegracaoCentroCustoCargaOpenTech = Request.GetBoolParam("EnviarCodigoIntegracaoCentroCustoCargaOpenTech");
                configuracaoIntegracao.EnviarValorDasNotasNoCampoValorDoc = Request.GetBoolParam("EnviarValorDasNotasNoCampoValorDoc");
                configuracaoIntegracao.CadastrarRotaCargaOpentech = Request.GetBoolParam("CadastrarRotaCargaOpentech");

                configuracaoIntegracao.EncerrarTodosCIOTAutomaticamente = Request.GetBoolParam("EncerrarTodosCIOTAutomaticamente");

                configuracaoIntegracao.CodigoIntegradorEFrete = codigoIntegradorEFrete;
                configuracaoIntegracao.URLRecepcaoDTe = URLRecepcaoDTe;

                configuracaoIntegracao.MatrizEFrete = repEmpresa.BuscarPorCodigo(codigoMatrizEFrete);
                configuracaoIntegracao.SenhaEFrete = senhaEFrete;
                configuracaoIntegracao.UsuarioEFrete = usuarioEFrete;

                configuracaoIntegracao.PossuiIntegracaoBrasilRisk = possuiIntegracaoBrasilRisk;
                configuracaoIntegracao.UsuarioBrasilRisk = usuarioBrasilRisk;
                configuracaoIntegracao.SenhaBrasilRisk = senhaBrasilRisk;
                configuracaoIntegracao.URLHomologacaoBrasilRisk = urlHomologacaoBrasilRisk;
                configuracaoIntegracao.URLProducaoBrasilRisk = urlProducaoBrasilRisk;
                configuracaoIntegracao.URLBrasilRiskGestao = urlBrasilRiskGestao;
                configuracaoIntegracao.URLBrasilRiskVeiculoMotorista = urlBrasilRiskVeiculoMotorista;
                configuracaoIntegracao.CNPJEmbarcadorBrasilRisk = cnpjEmbarcadorBrasilRisk;
                configuracaoIntegracao.ValorBaseBrasilRisk = valorBaseBrasilRisk;
                configuracaoIntegracao.EnviarTodosDestinosBrasilRisk = Request.GetBoolParam("EnviarTodosDestinosBrasilRisk");
                configuracaoIntegracao.InicioViagemFixoHoraAtualMaisMinutos = Request.GetBoolParam("InicioViagemFixoHoraAtualMaisMinutos");
                configuracaoIntegracao.EnviarDadosTransportadoraSubContratadaNasObservacoes = Request.GetBoolParam("EnviarDadosTransportadoraSubContratadaNasObservacoes");
                configuracaoIntegracao.MinutosAMaisInicioViagem = Request.GetIntParam("MinutosAMaisInicioViagem");
                configuracaoIntegracao.IntegrarRotaBrasilRisk = Request.GetBoolParam("IntegrarRotaBrasilRisk");
                Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoTipoIntegracao configuracaoTipoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoTipoIntegracao()
                {
                    TipoIntegracao = TipoIntegracao.BrasilRisk,
                    IntegrarCargaTransbordo = Request.GetBoolParam("BrasilRiskGerarParaCargasDeTransbordo")
                };
                SalvarConfiguracaoNoTipoIntegracao(configuracaoTipoIntegracao, unidadeDeTrabalho);

                configuracaoIntegracao.PossuiIntegracaoMundialRisk = possuiIntegracaoMundialRisk;
                configuracaoIntegracao.UsuarioMundialRisk = usuarioMundialRisk;
                configuracaoIntegracao.SenhaMundialRisk = senhaMundialRisk;
                configuracaoIntegracao.URLHomologacaoMundialRisk = urlHomologacaoMundialRisk;
                configuracaoIntegracao.URLProducaoMundialRisk = urlProducaoMundialRisk;

                configuracaoIntegracao.PossuiIntegracaoLogiun = possuiIntegracaoLogiun;
                configuracaoIntegracao.UsuarioLogiun = usuarioLogiun;
                configuracaoIntegracao.SenhaLogiun = senhaLogiun;
                configuracaoIntegracao.URLHomologacaoLogiun = urlHomologacaoLogiun;
                configuracaoIntegracao.URLProducaoLogiun = urlProducaoLogiun;

                configuracaoIntegracao.CNPJClienteBuonny = cnpjClienteBuonny;
                configuracaoIntegracao.CNPJGerenciadoraDeRiscoBuonny = Request.GetStringParam("CNPJGerenciadoraDeRiscoBuonny");
                configuracaoIntegracao.TokenBuonny = tokenBuonny;
                configuracaoIntegracao.URLHomologacaoBuonny = urlHomologacaoBuonny;
                configuracaoIntegracao.URLProducaoBuonny = urlProducaoBuonny;
                configuracaoIntegracao.URLRestHomologacaoBuonny = urlRestHomologacaoBuonny;
                configuracaoIntegracao.URLRestProducaoBuonny = urlRestProducaoBuonny;
                configuracaoIntegracao.TempoHorasConsultasBuonny = tempoHorasConsultasBuonny;
                configuracaoIntegracao.CadastrarMotoristaAntesConsultarBuonny = cadastrarMotoristaAntesConsultarBuonny;

                configuracaoIntegracao.URLAvior = urlAvior;
                configuracaoIntegracao.UsuarioAvior = usuarioAvior;
                configuracaoIntegracao.SenhaAvior = senhaAvior;
                configuracaoIntegracao.CNPJAvior = Request.GetStringParam("CNPJAvior");

                configuracaoIntegracao.CNPJMatrizNOX = cnpjMatrizNOX;
                configuracaoIntegracao.PossuiIntegracaoNOX = possuiIntegracaoNOX;
                configuracaoIntegracao.SenhaNOX = senhaNOX;
                configuracaoIntegracao.TokenNOX = tokenNOX;
                configuracaoIntegracao.URLHomologacaoNOX = urlHomologacaoNOX;
                configuracaoIntegracao.URLProducaoNOX = urlProducaoNOX;
                configuracaoIntegracao.UsuarioNOX = usuarioNOX;

                configuracaoIntegracao.URLCarrefourCancelamentoCarga = Request.GetStringParam("URLCarrefourCancelamentoCarga");
                configuracaoIntegracao.URLCarrefourCarga = Request.GetStringParam("URLCarrefourCarga");
                configuracaoIntegracao.URLCarrefourIndicadorIntegracaoCTe = Request.GetStringParam("URLCarrefourIndicadorIntegracaoCTe");
                configuracaoIntegracao.URLCarrefourOcorrencia = Request.GetStringParam("URLCarrefourOcorrencia");
                configuracaoIntegracao.URLCarrefourProvisao = Request.GetStringParam("URLCarrefourProvisao");
                configuracaoIntegracao.URLCarrefourValidarCancelamentoCarga = Request.GetStringParam("URLCarrefourValidarCancelamentoCarga");
                configuracaoIntegracao.TokenCarrefour = Request.GetStringParam("TokenCarrefour");
                configuracaoIntegracao.TokenCarrefourProvisao = Request.GetStringParam("TokenCarrefourProvisao");
                configuracaoIntegracao.TokenCarrefourIndicadorIntegracaoCTe = Request.GetStringParam("TokenCarrefourIndicadorIntegracaoCTe");

                configuracaoIntegracao.PossuiIntegracaoGoldenService = possuiIntegracaoGoldenService;
                configuracaoIntegracao.CodigoGoldenService = codigoGoldenService;
                configuracaoIntegracao.IdGoldenService = idGoldenService;
                configuracaoIntegracao.SenhaGoldenService = senhaGoldenService;
                configuracaoIntegracao.URLHomologacaoGoldenService = urlHomologacaoGoldenService;
                configuracaoIntegracao.URLProducaoGoldenService = urlProducaoGoldenService;

                configuracaoIntegracao.PossuiIntegracaoGPA = possuiIntegracaoGPA;
                configuracaoIntegracao.URLProducaoGPA = urlProducaoGPA;
                configuracaoIntegracao.URLHomologacaoGPA = urlHomologacaoGPA;
                configuracaoIntegracao.APIKeyGPA = Request.GetStringParam("APIKeyGPA");

                configuracaoIntegracao.URLOrtec = urlOrtec;
                configuracaoIntegracao.UsuarioOrtec = usuarioOrtec;
                configuracaoIntegracao.SenhaOrtec = senhaOrtec;
                configuracaoIntegracao.IntegrarEntregaOrtec = Request.GetBoolParam("IntegrarEntregaOrtec");

                configuracaoIntegracao.EmpresaFixaPamCard = codigoEmpresaFixaPamCard > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresaFixaPamCard) : null;
                configuracaoIntegracao.URLPamcardCorporativo = Request.GetStringParam("URLPamcardCorporativo");
                configuracaoIntegracao.URLPamcardCorporativoAutenticacao = Request.GetStringParam("URLPamcardCorporativoAutenticacao");

                configuracaoIntegracao.PossuiIntegracaoAX = Request.GetBoolParam("PossuiIntegracaoAX");
                configuracaoIntegracao.URLAX = Request.GetStringParam("URLAX");
                configuracaoIntegracao.URLAXContratoFrete = Request.GetStringParam("URLAXContratoFrete");
                configuracaoIntegracao.URLAXOrdemVenda = Request.GetStringParam("URLAXOrdemVenda");
                configuracaoIntegracao.URLAXCompansacao = Request.GetStringParam("URLAXCompansacao");
                configuracaoIntegracao.URLAXPedido = Request.GetStringParam("URLAXPedido");
                configuracaoIntegracao.URLAXComplemento = Request.GetStringParam("URLAXComplemento");
                configuracaoIntegracao.URLAXCancelamento = Request.GetStringParam("URLAXCancelamento");
                configuracaoIntegracao.UsuarioAX = Request.GetStringParam("UsuarioAX");
                configuracaoIntegracao.SenhaAX = Request.GetStringParam("SenhaAX");
                configuracaoIntegracao.CNPJAX = Request.GetStringParam("CNPJAX");

                configuracaoIntegracao.PossuiIntegracaoTrizy = Request.GetBoolParam("PossuiIntegracaoTrizy");
                configuracaoIntegracao.TokenTrizy = Request.GetStringParam("TokenTrizy");
                configuracaoIntegracao.URLTrizy = Request.GetStringParam("URLTrizy");
                configuracaoIntegracao.AgenciaTrizy = Request.GetStringParam("AgenciaTrizy");
                configuracaoIntegracao.NaoRealizarIntegracaoPedido = Request.GetBoolParam("NaoRealizarIntegracaoPedido");
                configuracaoIntegracao.QuantidadeEixosPadrao = Request.GetIntParam("QuantidadeEixosPadrao");
                configuracaoIntegracao.CNPJCompanyTrizy = Request.GetStringParam("CNPJCompanyTrizy");

                configuracaoIntegracao.PossuiIntegracaoRaster = Request.GetBoolParam("PossuiIntegracaoRaster");
                configuracaoIntegracao.UsuarioRaster = Request.GetStringParam("UsuarioRaster");
                configuracaoIntegracao.SenhaRaster = Request.GetStringParam("SenhaRaster");
                configuracaoIntegracao.URLRaster = Request.GetStringParam("URLRaster");
                configuracaoIntegracao.NotificarFalhaIntegracaoRaster = Request.GetBoolParam("NotificarFalhaIntegracaoRaster");
                configuracaoIntegracao.GerarIntegracaoPreSmEtapaCargaDadosTransporteRaster = Request.GetBoolParam("GerarIntegracaoPreSmEtapaCargaDadosTransporteRaster");
                configuracaoIntegracao.GerarIntegracaoEfetivaPreSmEtapaCargaFreteRaster = Request.GetBoolParam("GerarIntegracaoEfetivaPreSmEtapaCargaFreteRaster");
                configuracaoIntegracao.GerarIntegracaoSetRevisaoPreSMnaEtapaIntegracao = Request.GetBoolParam("GerarIntegracaoSetRevisaoPreSMnaEtapaIntegracao");
                configuracaoIntegracao.ReenviarIntegracaoDadosTransporteAoAlterarDadosTransporteRaster = Request.GetBoolParam("ReenviarIntegracaoDadosTransporteAoAlterarDadosTransporteRaster");

                configuracaoIntegracao.URLProducaoUnileverFourKites = Request.GetStringParam("URLProducaoUnileverFourKites");
                configuracaoIntegracao.URLHomologacaoUnileverFourKites = Request.GetStringParam("URLHomologacaoUnileverFourKites");
                configuracaoIntegracao.UsuarioUnileverFourKites = Request.GetStringParam("UsuarioUnileverFourKites");
                configuracaoIntegracao.SenhaUnileverFourKites = Request.GetStringParam("SenhaUnileverFourKites");

                configuracaoIntegracao.URLIntegracaoDigibee = Request.GetStringParam("URLIntegracaoDigibee");
                configuracaoIntegracao.URLIntegracaoCancelamentoDigibee = Request.GetStringParam("URLIntegracaoCancelamentoDigibee");
                configuracaoIntegracao.URLIntegracaoDadosCargaDigibee = Request.GetStringParam("URLIntegracaoDadosCargaDigibee");
                configuracaoIntegracao.URLIntegracaoDadosContabeisCTeDigibee = Request.GetStringParam("URLIntegracaoDadosContabeisCTeDigibee");
                configuracaoIntegracao.URLIntegracaoEscrituracaoCTeDigibee = Request.GetStringParam("URLIntegracaoEscrituracaoCTeDigibee");
                configuracaoIntegracao.URLAutenticacaoDigibee = Request.GetStringParam("URLAutenticacaoDigibee");
                configuracaoIntegracao.UsuarioAutenticacaoDigibee = Request.GetStringParam("UsuarioAutenticacaoDigibee");
                configuracaoIntegracao.SenhaAutenticacaoDigibee = Request.GetStringParam("SenhaAutenticacaoDigibee");
                configuracaoIntegracao.APIKeyDigibee = Request.GetStringParam("APIKeyDigibee");
                configuracaoIntegracao.IntegracaoDigibeePadraoConsinco = Request.GetBoolParam("IntegracaoDigibeePadraoConsinco");
                configuracaoIntegracao.AjustarDataParaCorresponderQuinzenaDigibee = Request.GetBoolParam("AjustarDataParaCorresponderQuinzenaDigibee");
                configuracaoIntegracao.APIKeyDigibeeGeral = Request.GetStringParam("APIKeyDigibeeGeral");

                configuracaoIntegracao.PossuiIntegracaoPH = Request.GetBoolParam("PossuiIntegracaoPH");
                configuracaoIntegracao.UsuarioPH = Request.GetStringParam("UsuarioPH");
                configuracaoIntegracao.SenhaPH = Request.GetStringParam("SenhaPH");
                configuracaoIntegracao.URLHomologacaoPH = Request.GetStringParam("URLHomologacaoPH");
                configuracaoIntegracao.URLProducaoPH = Request.GetStringParam("URLProducaoPH");
                configuracaoIntegracao.CNPJContadorPH = Request.GetStringParam("CNPJContadorPH");
                configuracaoIntegracao.SoftwarePH = Request.GetStringParam("SoftwarePH");
                configuracaoIntegracao.PortaPH = Request.GetStringParam("PortaPH");
                configuracaoIntegracao.IPSocketPH = Request.GetStringParam("IPSocketPH");
                configuracaoIntegracao.PortaSocketPH = Request.GetStringParam("PortaSocketPH");

                configuracaoIntegracao.URLIntegracaoTelerisco = Request.GetStringParam("URLIntegracaoTelerisco");
                configuracaoIntegracao.CNPJEmbarcadorTelerisco = Request.GetStringParam("CNPJEmbarcadorTelerisco");
                configuracaoIntegracao.CaminhoCertificadoTelerisco = Request.GetStringParam("CaminhoCertificadoTelerisco");
                configuracaoIntegracao.SenhaCertificadoTelerisco = Request.GetStringParam("SenhaCertificadoTelerisco");
                configuracaoIntegracao.CodigosAceitosRetornoTelerisco = Request.GetStringParam("CodigosAceitosRetornoTelerisco");
                configuracaoIntegracao.IntegracaoViaPOSTTelerisco = Request.GetBoolParam("IntegracaoViaPOSTTelerisco");
                configuracaoIntegracao.EmpresaFixaTelerisco = codigoEmpresaFixaTelerisco > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresaFixaTelerisco) : null;
                configuracaoIntegracao.NaoEnviarDataEmbarqueGrMotoristaTelerisco = Request.GetBoolParam("NaoEnviarDataEmbarqueGrMotoristaTelerisco");
                configuracaoIntegracao.EnviarEmpresaFixaComoCNPJEmbarcadorNaIntegracaoTelerisco = Request.GetBoolParam("EnviarEmpresaFixaComoCNPJEmbarcadorNaIntegracaoTelerisco");

                configuracaoIntegracao.URLIntegracaoCargoX = Request.GetStringParam("URLIntegracaoCargoX");
                configuracaoIntegracao.TokenCargoX = Request.GetStringParam("TokenCargoX");

                configuracaoIntegracao.URLIntegracaoRiachuelo = Request.GetStringParam("URLIntegracaoRiachuelo");
                configuracaoIntegracao.URLIntegracaoEntregaNFeRiachuelo = Request.GetStringParam("URLIntegracaoEntregaNFeRiachuelo");
                configuracaoIntegracao.HabilitarDataSaidaCDLoja = Request.GetBoolParam("HabilitarDataSaidaCDLoja");

                configuracaoIntegracao.PossuiIntegracaoKrona = Request.GetBoolParam("PossuiIntegracaoKrona");
                configuracaoIntegracao.URLIntegracaoKrona = Request.GetStringParam("URLIntegracaoKrona");

                configuracaoIntegracao.PossuiIntegracaoBoticario = Request.GetBoolParam("PossuiIntegracaoBoticario");
                configuracaoIntegracao.URLIntegracaoBoticario = Request.GetStringParam("URLIntegracaoBoticario");
                configuracaoIntegracao.IntegracaoBoticarioClientId = Request.GetStringParam("IntegracaoBoticarioClientId");
                configuracaoIntegracao.IntegracaoBoticarioClientSecret = Request.GetStringParam("IntegracaoBoticarioClientSecret");
                configuracaoIntegracao.URLGerarTokenBoticario = Request.GetStringParam("URLGerarTokenBoticario");
                configuracaoIntegracao.URLEnvioSequenciaBoticario = Request.GetStringParam("URLEnvioSequenciaBoticario");

                configuracaoIntegracao.PossuiIntegracaoInfolog = Request.GetBoolParam("PossuiIntegracaoInfolog");
                configuracaoIntegracao.URLIntegracaoInfolog = Request.GetStringParam("URLIntegracaoInfolog");
                configuracaoIntegracao.UsuarioInfolog = Request.GetStringParam("UsuarioInfolog");
                configuracaoIntegracao.SenhaInfolog = Request.GetStringParam("SenhaInfolog");
                configuracaoIntegracao.CodigoOperacaoInfolog = Request.GetStringParam("CodigoOperacaoInfolog");

                configuracaoIntegracao.URLIntegracaoSaintGobain = Request.GetStringParam("URLIntegracaoSaintGobain");
                configuracaoIntegracao.UserNameSaintGobain = Request.GetStringParam("UserNameSaintGobain");
                configuracaoIntegracao.PasswordSaintGobain = Request.GetStringParam("PasswordSaintGobain");
                configuracaoIntegracao.PossuiIntegracaoSaintGobain = Request.GetBoolParam("PossuiIntegracaoSaintGobain");

                configuracaoIntegracao.PossuiIntegracaoUltragaz = Request.GetBoolParam("PossuiIntegracaoUltragaz");
                configuracaoIntegracao.URLAutenticacaoUltragaz = Request.GetStringParam("URLAutenticacaoUltragaz");
                configuracaoIntegracao.URLIntegracaoUltragaz = Request.GetStringParam("URLIntegracaoUltragaz");
                configuracaoIntegracao.ClientSecretUltragaz = Request.GetStringParam("ClientSecretUltragaz");
                configuracaoIntegracao.ClientIdUltragaz = Request.GetStringParam("ClientIdUltragaz");
                configuracaoIntegracao.URLContabilizacaoUltragaz = Request.GetStringParam("URLContabilizacaoUltragaz");
                configuracaoIntegracao.URLIntegracaoVeiculoUltragaz = Request.GetStringParam("URLIntegracaoVeiculoUltragaz");
                configuracaoIntegracao.NaoPermitirReenviarIntegracaoPagamentoAgRetorno = Request.GetBoolParam("NaoPermitirReenviarIntegracaoPagamentoAgRetorno");

                configuracaoIntegracao.URLToledo = Request.GetStringParam("URLToledo");
                configuracaoIntegracao.URLQbit = Request.GetStringParam("URLQbit");

                configuracaoIntegracao.URLAdagio = Request.GetStringParam("URLAdagio");
                configuracaoIntegracao.EmailAdagio = Request.GetStringParam("EmailAdagio");
                configuracaoIntegracao.SenhaAdagio = Request.GetStringParam("SenhaAdagio");

                configuracaoIntegracao.PossuiIntegracaoSAD = Request.GetBoolParam("PossuiIntegracaoSAD");

                configuracaoIntegracao.URLCorreios = Request.GetStringParam("URLCorreios");
                configuracaoIntegracao.UsuarioCorreios = Request.GetStringParam("UsuarioCorreios");
                configuracaoIntegracao.SenhaCorreios = Request.GetStringParam("SenhaCorreios");

                configuracaoIntegracao.URLCobasi = Request.GetStringParam("URLCobasi");
                configuracaoIntegracao.APIKeyCobasi = Request.GetStringParam("APIKeyCobasi");

                configuracaoIntegracao.URLEmillenium = Request.GetStringParam("URLEmillenium");
                configuracaoIntegracao.URLEmilleniumConfirmarEntrega = Request.GetStringParam("URLEmilleniumConfirmarEntrega");
                configuracaoIntegracao.SenhaFrontDoor = Request.GetStringParam("SenhaFrontDoor");
                configuracaoIntegracao.UsuarioEmillenium = Request.GetStringParam("UsuarioEmillenium");
                configuracaoIntegracao.SenhaEmillenium = Request.GetStringParam("SenhaEmillenium");
                configuracaoIntegracao.TransIdAtualEmillenium = Request.GetIntParam("TransIdAtualEmillenium");
                configuracaoIntegracao.QuantidadeNotificacaoEmillenium = Request.GetIntParam("QuantidadeNotificacaoEmillenium");
                configuracaoIntegracao.EmailsNotificacaoEmillenium = Request.GetStringParam("EmailsNotificacaoEmillenium");

                configuracaoIntegracao.PossuiIntegracaoMichelin = Request.GetBoolParam("PossuiIntegracaoMichelin");
                configuracaoIntegracao.UsuarioMichelin = Request.GetStringParam("UsuarioMichelin");
                configuracaoIntegracao.SenhaMichelin = Request.GetStringParam("SenhaMichelin");
                configuracaoIntegracao.URLHomologacaoMichelin = Request.GetStringParam("URLHomologacaoMichelin");
                configuracaoIntegracao.URLProducaoMichelin = Request.GetStringParam("URLProducaoMichelin");
                configuracaoIntegracao.CodigoTransportadoraMichelin = Request.GetStringParam("CodigoTransportadoraMichelin");
                configuracaoIntegracao.CnpjTransportadoraMichelin = Request.GetStringParam("CnpjTransportadoraMichelin");

                configuracaoIntegracao.PossuiIntegracaoDeCadastrosMulti = Request.GetBoolParam("PossuiIntegracaoDeCadastrosMulti");
                configuracaoIntegracao.URLIntegracaoCadastrosMulti = Request.GetStringParam("URLIntegracaoCadastrosMulti");
                configuracaoIntegracao.TokenIntegracaoCadastrosMulti = Request.GetStringParam("TokenIntegracaoCadastrosMulti");
                configuracaoIntegracao.URLIntegracaoCadastrosMultiSecundario = Request.GetStringParam("URLIntegracaoCadastrosMultiSecundario");
                configuracaoIntegracao.TokenIntegracaoCadastrosMultiSecundario = Request.GetStringParam("TokenIntegracaoCadastrosMultiSecundario");
                configuracaoIntegracao.RealizarIntegracaoDePessoaParaPessoa = Request.GetBoolParam("RealizarIntegracaoDePessoaParaPessoa");
                configuracaoIntegracao.RealizarIntegracaoDeTransportadorParaEmpresa = Request.GetBoolParam("RealizarIntegracaoDeTransportadorParaEmpresa");
                configuracaoIntegracao.RealizarIntegracaoDeContainer = Request.GetBoolParam("RealizarIntegracaoDeContainer");
                configuracaoIntegracao.RealizarIntegracaoDeNavio = Request.GetBoolParam("RealizarIntegracaoDeNavio");
                configuracaoIntegracao.RealizarIntegracaoDeViagem = Request.GetBoolParam("RealizarIntegracaoDeViagem");
                configuracaoIntegracao.RealizarIntegracaoDeCTeAnterior = Request.GetBoolParam("RealizarIntegracaoDeCTeAnterior");
                configuracaoIntegracao.RealizarIntegracaoDeCTeParaComplementoOSMae = Request.GetBoolParam("RealizarIntegracaoDeCTeParaComplementoOSMae");
                configuracaoIntegracao.RealizarIntegracaoDePorto = Request.GetBoolParam("RealizarIntegracaoDePorto");
                configuracaoIntegracao.RealizarIntegracaoDeTipoDeContainer = Request.GetBoolParam("RealizarIntegracaoDeTipoDeContainer");
                configuracaoIntegracao.RealizarIntegracaoDeTerminalPortuario = Request.GetBoolParam("RealizarIntegracaoDeTerminalPortuario");
                configuracaoIntegracao.RealizarIntegracaoDeProdutoEmbarcador = Request.GetBoolParam("RealizarIntegracaoDeProdutoEmbarcador");

                configuracaoIntegracao.PossuiIntegracaoDeTotvs = Request.GetBoolParam("PossuiIntegracaoDeTotvs");
                configuracaoIntegracao.URLIntegracaoTotvs = Request.GetStringParam("URLIntegracaoTotvs");
                configuracaoIntegracao.URLIntegracaoTotvsProcess = Request.GetStringParam("URLIntegracaoTotvsProcess");
                configuracaoIntegracao.UsuarioTotvs = Request.GetStringParam("UsuarioTotvs");
                configuracaoIntegracao.SenhaTotvs = Request.GetStringParam("SenhaTotvs");
                configuracaoIntegracao.ContextoTotvs = Request.GetStringParam("ContextoTotvs");

                configuracaoIntegracao.URLOnetrust = Request.GetStringParam("URLOnetrust");
                configuracaoIntegracao.URLObterTokenOnetrust = Request.GetStringParam("URLObterTokenOnetrust");
                configuracaoIntegracao.UrlRegularizacaoOneTrust = Request.GetStringParam("UrlRegularizacaoOneTrust");
                configuracaoIntegracao.PurposeIdOneTrust = Request.GetStringParam("PurposeIdOneTrust");
                configuracaoIntegracao.ClientIdOneTrust = Request.GetStringParam("ClientIdOneTrust");
                configuracaoIntegracao.ClientSecretOneTrust = Request.GetStringParam("ClientSecretOneTrust");

                configuracaoIntegracao.URLSintegra = Request.GetStringParam("URLSintegra");
                configuracaoIntegracao.TokenSintegra = Request.GetStringParam("TokenSintegra");
                configuracaoIntegracao.IntervaloConsultaSintegra = Request.GetIntParam("IntervaloConsultaSintegra");

                configuracaoIntegracao.PossuiIntegracaoCTASmart = Request.GetBoolParam("PossuiIntegracaoCTASmart");

                if (configuracaoIntegracao.Codigo > 0)
                    repConfiguracaoIntegracao.Atualizar(configuracaoIntegracao, Auditado);
                else
                    repConfiguracaoIntegracao.Inserir(configuracaoIntegracao, Auditado);

                SalvarConfiguracoesIntegracaoAngelLira(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoAvon(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoSAD(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoMercadoLivre(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoA52(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoBBC(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoKuehneNagel(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoDansales(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoTarget(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoExtratta(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoMicDta(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoRavex(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoGadle(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoTelhaNorte(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoInforDoc(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoIsis(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoMagalu(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoCadastroMulti(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoGSW(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoArquivei(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoDPA(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoCTASmart(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoSaintGobain(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoHavan(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoFrota162(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoDexco(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoIntercab(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoProtheus(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoUnilever(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoNstech(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoSimonetti(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoMarisa(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoDeca(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoVLI(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoMarilan(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoCorreios(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoArcelorMittal(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoEMP(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoTicketLog(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracaoIntegracaoEmillenium(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoGNRE(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoDigitalCom(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoLBC(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesTecnorisk(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesDestinadosSAP(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesNeokohm(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesMoniloc(configuracaoIntegracao, unidadeDeTrabalho);
                await SalvarConfiguracoesApisulLogAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                SalvarConfiguracoesFroggr(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesSAP(configuracaoIntegracao, unidadeDeTrabalho);
                await SalvarConfiguracoesSAP_API4Async(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                SalvarConfiguracoesYPE(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesOTM(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesSIC(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoFrimesa(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesLoggi(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesCTePagamentoLoggi(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesValoresCTeLoggi(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesJJ(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoKlios(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoSAPV9(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoSAPST(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoLogRisk(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoBrado(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoEShip(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesDiageo(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesP44(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoYandeh(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoEFrete(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoBalancaKIKI(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoComprovei(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoComproveiRota(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoComprovei(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoKMM(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoLogvett(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoOpenTech(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoAtlas(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoFlora(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoCalisto(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoTrizy(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoObramax(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoObramaxCTE(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoObramaxNFE(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoObramaxProvisao(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoShopee(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoItalac(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoItalacFatura(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoPager(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoElectrolux(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoWhatsApp(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoBoticarioFreeFlow(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesLoggiFaturas(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesRuntec(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesPiracanjuba(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesCTeAnterioresLoggi(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesATSLog(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesCamil(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoBuntech(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoRouteasy(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesConfirmaFacil(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesBind(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesCebrace(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoMondelez(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoGrupoSC(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoFusion(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesIntegracaoMars(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesAcessoViaToken(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesTrizyEventos(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesVector(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesSalesforce(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesConecttec(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesGlobus(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracaoFS(configuracaoIntegracao, unidadeDeTrabalho);
                SalvarConfiguracoesMigrate(configuracaoIntegracao, unidadeDeTrabalho);
                await SalvarConfiguracoesVedacitAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesJDEFaturasAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesTransSatAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoOlfarAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesEfesusAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesCassolAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoWeberChileAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoLactalisAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoPortalCabotagemAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoSistemaTransbenAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoATSSmartWebAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoVSTrackAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoTrafegusAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoYMSAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoHUBAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoSeniorAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);
                await SalvarConfiguracoesIntegracaoSkymarkAsync(configuracaoIntegracao, unidadeDeTrabalho, cancellationToken);

                await unidadeDeTrabalho.CommitChangesAsync();

                CacheProvider.Instance.Remove(ConfiguracaoIntegracaoKey);

                return new JsonpResult(false);
            }
            catch (Exception ex)
            {
                await unidadeDeTrabalho.RollbackAsync();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao salvar as configurações de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        private void SalvarConfiguracoesIntegracaoAtlas(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoAtlas repConfiguracaoIntegracaoAtlas = new Repositorio.Embarcador.Configuracoes.IntegracaoAtlas(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAtlas configuracaoIntegracaoAtlas = repConfiguracaoIntegracaoAtlas.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoAtlas == null)
                configuracaoIntegracaoAtlas = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAtlas();
            else
                configuracaoIntegracaoAtlas.Initialize();

            configuracaoIntegracaoAtlas.Ativa = Request.GetBoolParam("AtivaAtlas");
            configuracaoIntegracaoAtlas.Usuario = Request.GetStringParam("UsuarioAtlas");
            configuracaoIntegracaoAtlas.Senha = Request.GetStringParam("SenhaAtlas");
            configuracaoIntegracaoAtlas.URLAcesso = Request.GetStringParam("URLAcessoAtlas");
            configuracaoIntegracaoAtlas.CodigoCliente = Request.GetStringParam("CodigoClienteAtlas");

            if (configuracaoIntegracaoAtlas.Codigo > 0)
            {
                repConfiguracaoIntegracaoAtlas.Atualizar(configuracaoIntegracaoAtlas);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoAtlas.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Atlas.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoAtlas.Inserir(configuracaoIntegracaoAtlas);
        }

        public async Task<IActionResult> BuscarCidadesOpenTech()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade repConfiguracaoIntegracaoLocalidade = new Repositorio.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                // Busca Integracao
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.OpenTech.IntegracaoCidade> cidades = new Servicos.Embarcador.Integracao.OpenTech.IntegracaoCargaOpenTech(unitOfWork).ObterCidadesParaIntegracoes(out string erro);

                if (cidades == null && !string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(true, false, erro);

                // Itera retorno
                for (int i = 0, s = cidades.Count; i < s; i++)
                {
                    unitOfWork.FlushAndClear();

                    Dominio.ObjetosDeValor.Embarcador.Integracao.OpenTech.IntegracaoCidade cidade = cidades[i];
                    Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade integracao = null;

                    string descricaoCidade = cidade != null && !string.IsNullOrWhiteSpace(cidade.Cidade) ? cidade.Cidade.Substring(0, cidade.Cidade.Length - 5) : string.Empty;
                    //Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(cidade.Estado);
                    if (cidade.Pais != "BRASIL") //Considera Exterior
                        integracao = repConfiguracaoIntegracaoLocalidade.BuscarPorCidadeExterior(descricaoCidade, cidade.Estado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech);
                    else
                        integracao = repConfiguracaoIntegracaoLocalidade.BuscarPorIBGE(cidade.IBGE, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech);

                    Dominio.Entidades.Localidade localidade = null;

                    if (integracao != null)
                    {
                        integracao.CodigoIntegracao = cidade.Integracao;
                        repConfiguracaoIntegracaoLocalidade.Atualizar(integracao);
                    }
                    else if (cidade.Pais != "BRASIL")  //Exterior
                    {
                        localidade = repLocalidade.BuscarPorDescricaoEPais(descricaoCidade, cidade.Estado);
                        if (localidade != null)
                        {
                            integracao = new Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade()
                            {
                                Localidade = localidade,
                                TipoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech,
                                CodigoIntegracao = cidade.Integracao
                            };
                            repConfiguracaoIntegracaoLocalidade.Inserir(integracao);
                        }
                        else
                        {
                            localidade = repLocalidade.BuscarPorCodigoIBGE(cidade.IBGE);
                            if (localidade != null)
                            {
                                integracao = new Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade()
                                {
                                    Localidade = localidade,
                                    TipoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech,
                                    CodigoIntegracao = cidade.Integracao
                                };
                                repConfiguracaoIntegracaoLocalidade.Inserir(integracao);
                            }
                        }
                    }
                    else
                    {
                        localidade = repLocalidade.BuscarPorCodigoIBGE(cidade.IBGE);
                        if (localidade != null)
                        {
                            integracao = new Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoLocalidade()
                            {
                                Localidade = localidade,
                                TipoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech,
                                CodigoIntegracao = cidade.Integracao
                            };
                            repConfiguracaoIntegracaoLocalidade.Inserir(integracao);
                        }
                    }
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar as configurações de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarTokenConecttec()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string token = Request.GetStringParam("TokenConecttec");
                Repositorio.WebService.Integradora repIntegradora = new Repositorio.WebService.Integradora(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoConecttec repConfiguracaoIntegracaoConecttec = new Repositorio.Embarcador.Configuracoes.IntegracaoConecttec(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConecttec configuracaoIntegracaoConecttec = repConfiguracaoIntegracaoConecttec.BuscarPrimeiroRegistro();

                Dominio.Entidades.WebService.Integradora integradora = null;

                if (!string.IsNullOrEmpty(token))
                    integradora = repIntegradora.BuscarPorToken(token);

                if (integradora == null)
                    integradora = new Dominio.Entidades.WebService.Integradora();

                integradora.TodosWebServicesLiberados = true;
                integradora.Ativo = true;
                integradora.Descricao = "Conecttec";
                integradora.TipoAutenticacao = TipoAutenticacao.Token;
                integradora.Token = Guid.NewGuid().ToString().Replace("-", "");

                if (integradora.Codigo > 0)
                    repIntegradora.Atualizar(integradora, Auditado);
                else
                    repIntegradora.Inserir(integradora, Auditado);

                configuracaoIntegracaoConecttec.TokenURLRecebimento = integradora.Token;
                repConfiguracaoIntegracaoConecttec.Atualizar(configuracaoIntegracaoConecttec);

                return new JsonpResult(new { Token = integradora.Token }, true, "Token Gerado com sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter token.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarURLRecebimentoConecttec()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoConecttec repConfiguracaoIntegracaoConecttec = new Repositorio.Embarcador.Configuracoes.IntegracaoConecttec(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConecttec configuracaoIntegracaoConecttec = repConfiguracaoIntegracaoConecttec.BuscarPrimeiroRegistro();

                string providerID = Request.GetStringParam("ProviderID");
                string url = Request.GetStringParam("URL");

                string mensagem = "";

                if (!new Servicos.Embarcador.Integracao.Conecttec.IntegracaoConecttec(unitOfWork).AtualizarURLRecebimentoCallback(providerID, url, ref mensagem))
                    return new JsonpResult(false, mensagem);

                configuracaoIntegracaoConecttec.URLRecebimentoCallback = url;
                repConfiguracaoIntegracaoConecttec.Atualizar(configuracaoIntegracaoConecttec);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar URL de recebimento com a Conecttec.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoEMP repositorioConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repositorioConfiguracaoIntegracaoEMP.Buscar();
                Repositorio.Embarcador.Integracao.ArquivoIntegracao repositorioArquivoIntegracao = new Repositorio.Embarcador.Integracao.ArquivoIntegracao(unitOfWork);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                for (int i = 0; i < files.Count; i++)
                {
                    Servicos.DTO.CustomFile file = files[i];
                    string certificado = file.Key;

                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao, "EMP", certificado);

                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, file.FileName);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                    Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, ConvertStreamToByteArray(file.InputStream));

                    Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = new Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao()
                    {
                        NomeArquivo = caminho
                    };

                    repositorioArquivoIntegracao.Inserir(arquivoIntegracao);

                    if (certificado == "Certificado P12 Schema Registry Retina")
                        configuracaoIntegracaoEMP.CertificadoShemaRegistryRetina = arquivoIntegracao;
                    else
                        configuracaoIntegracaoEMP.CertificadoCRTServerRetina = arquivoIntegracao;

                    repositorioConfiguracaoIntegracaoEMP.Atualizar(configuracaoIntegracaoEMP);
                }

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao salvar certificados.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados - Obter Objeto Integração

        private dynamic ObterIntegracaoNatura(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                CodigoMatrizNatura = configuracaoIntegracao?.CodigoMatrizNatura ?? string.Empty,
                UsuarioNatura = configuracaoIntegracao?.UsuarioNatura ?? string.Empty,
                SenhaNatura = configuracaoIntegracao?.SenhaNatura ?? string.Empty,
                EnviarOcorrenciaNaturaAutomaticamente = configuracaoIntegracao?.EnviarOcorrenciaNaturaAutomaticamente ?? false,
                UtilizarValorFreteTMSNatura = configuracaoIntegracao?.UtilizarValorFreteTMSNatura ?? false
            };
        }

        private dynamic ObterIntegracaoOpentech(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralOpenTech configuracaoIntegracaoOpenTech = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralOpenTech(unidadeDeTrabalho).Buscar();
            bool _EnviarDataNFeNaDataPrevistaOpentech = configuracaoIntegracaoOpenTech?.EnviarDataNFeNaDataPrevistaOpentech ?? false;
            bool _ConsiderarLocalidadeProdutoIntegracaoEntrega = configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega ?? false;

            return new
            {
                CodigoClienteOpenTech = configuracaoIntegracao?.CodigoClienteOpenTech ?? 0,
                CodigoPASOpenTech = configuracaoIntegracao?.CodigoPASOpenTech ?? 0,
                CodigoProdutoColetaOpentech = configuracaoIntegracao?.CodigoProdutoColetaOpentech ?? 0,
                CodigoProdutoColetaEmbarcadorOpentech = configuracaoIntegracao?.CodigoProdutoColetaEmbarcadorOpentech ?? 0,
                CodigoProdutoColetaTransportadorOpentech = configuracaoIntegracao?.CodigoProdutoColetaTransportadorOpentech ?? 0,
                CodigoProdutoPadraoOpentech = configuracaoIntegracao?.CodigoProdutoPadraoOpentech ?? 0,
                DominioOpenTech = configuracaoIntegracao?.DominioOpenTech ?? string.Empty,
                SenhaOpenTech = configuracaoIntegracao?.SenhaOpenTech ?? string.Empty,
                UsuarioOpenTech = configuracaoIntegracao?.UsuarioOpenTech ?? string.Empty,
                URLOpenTech = configuracaoIntegracao?.URLOpenTech ?? string.Empty,
                ValorBaseOpenTech = configuracaoIntegracao?.ValorBaseOpenTech.ToString("n2") ?? string.Empty,
                CodigoProdutoVeiculoComLocalizadorOpenTech = configuracaoIntegracao?.CodigoProdutoVeiculoComLocalizadorOpenTech,
                IntegrarVeiculoMotorista = configuracaoIntegracao?.IntegrarVeiculoMotorista ?? false,
                IntegrarColetaOpentech = configuracaoIntegracao?.IntegrarColetaOpentech ?? false,
                AtualizarVeiculoMotoristaOpentech = configuracaoIntegracao?.AtualizarVeiculoMotoristaOpentech ?? false,
                EnviarCodigoEmbarcadorProdutoOpentech = configuracaoIntegracao?.EnviarCodigoEmbarcadorProdutoOpentech ?? false,
                IntegrarRotaCargaOpentech = configuracaoIntegracao?.IntegrarRotaCargaOpentech ?? false,
                NotificarFalhaIntegracaoOpentech = configuracaoIntegracao?.NotificarFalhaIntegracaoOpentech ?? false,
                EmailsNotificacaoFalhaIntegracaoOpentech = configuracaoIntegracao?.EmailsNotificacaoFalhaIntegracaoOpentech ?? string.Empty,
                CodigoProdutoParaValidarSomenteVeiculoMotoristaSemUsoRastreador = configuracaoIntegracao?.CodigoProdutoParaValidarSomenteVeiculoMotoristaSemUsoRastreador ?? 0,
                PermitirTransportadorReenviarIntegracoesComProblemasOpenTech = configuracaoIntegracao?.PermitirTransportadorReenviarIntegracoesComProblemasOpenTech ?? false,
                EnviarDataPrevisaoEntregaDataCarregamentoOpentech = configuracaoIntegracao?.EnviarDataPrevisaoEntregaDataCarregamentoOpentech ?? false,
                EnviarDataAtualNaDataPrevisaoOpentech = configuracaoIntegracao?.EnviarDataAtualNaDataPrevisaoOpentech ?? false,
                CalcularPrevisaoEntregaComBaseDistanciaOpentech = configuracaoIntegracao?.CalcularPrevisaoEntregaComBaseDistanciaOpentech ?? false,
                EnviarDataPrevisaoSaidaPedidoOpentech = configuracaoIntegracao?.EnviarDataPrevisaoSaidaPedidoOpentech ?? false,
                EnviarInformacoesRastreadorCavaloOpentech = configuracaoIntegracao?.EnviarInformacoesRastreadorCavaloOpentech ?? false,
                EnviarCodigoIntegracaoRotaCargaOpenTech = configuracaoIntegracao?.EnviarCodigoIntegracaoRotaCargaOpenTech ?? false,
                EnviarNrfonecelBrancoOpenTech = configuracaoIntegracao?.EnviarNrfonecelBrancoOpenTech ?? false,
                EnviarPlacaVeiculoSeNaoExistirNumeroFrotaOpenTech = configuracaoIntegracao?.EnviarPlacaVeiculoSeNaoExistirNumeroFrotaOpenTech ?? false,
                EnviarValorNotasValorDocOpenTech = configuracaoIntegracao?.EnviarValorNotasValorDocOpenTech ?? false,
                EnviarCodigoIntegracaoCentroCustoCargaOpenTech = configuracaoIntegracao?.EnviarCodigoIntegracaoCentroCustoCargaOpenTech ?? false,
                EnviarValorDasNotasNoCampoValorDoc = configuracaoIntegracao?.EnviarValorDasNotasNoCampoValorDoc ?? false,
                CadastrarRotaCargaOpentech = configuracaoIntegracao?.CadastrarRotaCargaOpentech ?? false,
                ConsiderarLocalidadeProdutoIntegracaoEntrega = _ConsiderarLocalidadeProdutoIntegracaoEntrega,
                EnviarDataNFeNaDataPrevistaOpentech = _EnviarDataNFeNaDataPrevistaOpentech,
                IntegrarCargaOpenTechV10 = configuracaoIntegracao?.IntegrarCargaOpenTechV10 ?? false,
            };
        }

        private dynamic ObterIntegracaoDTe(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLRecepcaoDTe = configuracaoIntegracao?.URLRecepcaoDTe ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoMundialRisk(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoMundialRisk = configuracaoIntegracao?.PossuiIntegracaoMundialRisk ?? false,
                SenhaMundialRisk = configuracaoIntegracao?.SenhaMundialRisk ?? string.Empty,
                UsuarioMundialRisk = configuracaoIntegracao?.UsuarioMundialRisk ?? string.Empty,
                URLHomologacaoMundialRisk = configuracaoIntegracao?.URLHomologacaoMundialRisk ?? string.Empty,
                URLProducaoMundialRisk = configuracaoIntegracao?.URLProducaoMundialRisk ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoLogiun(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoLogiun = configuracaoIntegracao?.PossuiIntegracaoLogiun ?? false,
                SenhaLogiun = configuracaoIntegracao?.SenhaLogiun ?? string.Empty,
                UsuarioLogiun = configuracaoIntegracao?.UsuarioLogiun ?? string.Empty,
                URLHomologacaoLogiun = configuracaoIntegracao?.URLHomologacaoLogiun ?? string.Empty,
                URLProducaoLogiun = configuracaoIntegracao?.URLProducaoLogiun ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoBuonny(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                CNPJClienteBuonny = configuracaoIntegracao?.CNPJClienteBuonny ?? string.Empty,
                CNPJGerenciadoraDeRiscoBuonny = configuracaoIntegracao?.CNPJGerenciadoraDeRiscoBuonny ?? string.Empty,
                TokenBuonny = configuracaoIntegracao?.TokenBuonny ?? string.Empty,
                URLHomologacaoBuonny = configuracaoIntegracao?.URLHomologacaoBuonny ?? string.Empty,
                URLProducaoBuonny = configuracaoIntegracao?.URLProducaoBuonny ?? string.Empty,
                URLRestHomologacaoBuonny = configuracaoIntegracao?.URLRestHomologacaoBuonny ?? string.Empty,
                URLRestProducaoBuonny = configuracaoIntegracao?.URLRestProducaoBuonny ?? string.Empty,
                TempoHorasConsultasBuonny = configuracaoIntegracao?.TempoHorasConsultasBuonny ?? 0,
                CadastrarMotoristaAntesConsultarBuonny = configuracaoIntegracao?.CadastrarMotoristaAntesConsultarBuonny ?? false,
            };
        }

        private dynamic ObterIntegracaoAvior(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLAvior = configuracaoIntegracao?.URLAvior ?? string.Empty,
                UsuarioAvior = configuracaoIntegracao?.UsuarioAvior ?? string.Empty,
                SenhaAvior = configuracaoIntegracao?.SenhaAvior ?? string.Empty,
                CNPJAvior = configuracaoIntegracao?.CNPJAvior ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoNOX(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoNOX = configuracaoIntegracao?.PossuiIntegracaoNOX ?? false,
                SenhaNOX = configuracaoIntegracao?.SenhaNOX ?? string.Empty,
                TokenNOX = configuracaoIntegracao?.TokenNOX ?? string.Empty,
                URLHomologacaoNOX = configuracaoIntegracao?.URLHomologacaoNOX ?? string.Empty,
                URLProducaoNOX = configuracaoIntegracao?.URLProducaoNOX ?? string.Empty,
                UsuarioNOX = configuracaoIntegracao?.UsuarioNOX ?? string.Empty,
                CNPJMatrizNOX = configuracaoIntegracao?.CNPJMatrizNOX ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoCarrefur(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLCarrefourCancelamentoCarga = configuracaoIntegracao?.URLCarrefourCancelamentoCarga ?? string.Empty,
                URLCarrefourCarga = configuracaoIntegracao?.URLCarrefourCarga ?? string.Empty,
                URLCarrefourIndicadorIntegracaoCTe = configuracaoIntegracao?.URLCarrefourIndicadorIntegracaoCTe ?? string.Empty,
                URLCarrefourOcorrencia = configuracaoIntegracao?.URLCarrefourOcorrencia ?? string.Empty,
                URLCarrefourProvisao = configuracaoIntegracao?.URLCarrefourProvisao ?? string.Empty,
                URLCarrefourValidarCancelamentoCarga = configuracaoIntegracao?.URLCarrefourValidarCancelamentoCarga ?? string.Empty,
                TokenCarrefour = configuracaoIntegracao?.TokenCarrefour ?? string.Empty,
                TokenCarrefourIndicadorIntegracaoCTe = configuracaoIntegracao?.TokenCarrefourIndicadorIntegracaoCTe ?? string.Empty,
                TokenCarrefourProvisao = configuracaoIntegracao?.TokenCarrefourProvisao ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoGoldenService(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                CodigoGoldenService = configuracaoIntegracao?.CodigoGoldenService ?? string.Empty,
                IdGoldenService = configuracaoIntegracao?.IdGoldenService ?? string.Empty,
                PossuiIntegracaoGoldenService = configuracaoIntegracao?.PossuiIntegracaoGoldenService ?? false,
                SenhaGoldenService = configuracaoIntegracao?.SenhaGoldenService ?? string.Empty,
                URLHomologacaoGoldenService = configuracaoIntegracao?.URLHomologacaoGoldenService ?? string.Empty,
                URLProducaoGoldenService = configuracaoIntegracao?.URLProducaoGoldenService ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoGPA(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoGPA = configuracaoIntegracao?.PossuiIntegracaoGPA ?? false,
                URLHomologacaoGPA = configuracaoIntegracao?.URLHomologacaoGPA ?? string.Empty,
                URLProducaoGPA = configuracaoIntegracao?.URLProducaoGPA ?? string.Empty,
                APIKeyGPA = configuracaoIntegracao?.APIKeyGPA ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoOrtec(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLOrtec = configuracaoIntegracao?.URLOrtec ?? string.Empty,
                UsuarioOrtec = configuracaoIntegracao?.UsuarioOrtec ?? string.Empty,
                SenhaOrtec = configuracaoIntegracao?.SenhaOrtec ?? string.Empty,
                IntegrarEntregaOrtec = configuracaoIntegracao?.IntegrarEntregaOrtec ?? false,
            };
        }

        private dynamic ObterIntegracaoAPIGoogle(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                APIKeyGoogle = configuracaoIntegracao?.APIKeyGoogle ?? string.Empty,
                GeoServiceGeocoding = configuracaoIntegracao?.GeoServiceGeocoding ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Google,
                ServidorRouteOSM = configuracaoIntegracao?.ServidorRouteOSM ?? string.Empty,
                ServidorRouteGoogleOrTools = configuracaoIntegracao?.ServidorRouteGoogleOrTools ?? string.Empty,
                ServidorNominatim = configuracaoIntegracao?.ServidorNominatim ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoPamCard(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                EmpresaFixaPamCard = new
                {
                    Codigo = configuracaoIntegracao?.EmpresaFixaPamCard?.Codigo ?? 0,
                    Descricao = configuracaoIntegracao?.EmpresaFixaPamCard?.Descricao ?? string.Empty
                },
                URLPamcardCorporativo = configuracaoIntegracao?.URLPamcardCorporativo ?? string.Empty,
                URLPamcardCorporativoAutenticacao = configuracaoIntegracao?.URLPamcardCorporativoAutenticacao ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoPiracanjuba(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoPiracanjuba repConfiguracaoIntegracaoPiracanjuba = new Repositorio.Embarcador.Configuracoes.IntegracaoPiracanjuba(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba configuracaoIntegracaoPiracanjuba = repConfiguracaoIntegracaoPiracanjuba.Buscar();

            return new
            {
                URLIntegracaoCanhotoPiracanjuba = configuracaoIntegracaoPiracanjuba?.URLIntegracaoCanhotoPiracanjuba ?? string.Empty,
                URLIntegracaoCanhotoPiracanjubaContingencia = configuracaoIntegracaoPiracanjuba?.URLIntegracaoCanhotoPiracanjubaContingencia ?? string.Empty,
                configuracaoIntegracaoPiracanjuba?.DataFaturamentoNota,
                URLIntegracaoCargaPiracanjuba = configuracaoIntegracaoPiracanjuba?.URLIntegracaoCargaPiracanjuba ?? string.Empty,
                StringAmbientePiracanjuba = configuracaoIntegracaoPiracanjuba?.StringAmbientePiracanjuba ?? "",
                AmbienteProducaoPiracanjuba = configuracaoIntegracaoPiracanjuba?.AmbienteProducaoPiracanjuba ?? false,
                URLAutenticacaoPiracanjuba = configuracaoIntegracaoPiracanjuba?.URLAutenticacao ?? string.Empty,
                ClientIDPiracanjuba = configuracaoIntegracaoPiracanjuba?.ClientID ?? string.Empty,
                ClientSecretPiracanjuba = configuracaoIntegracaoPiracanjuba?.ClientSecret ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoRaster(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoRaster = configuracaoIntegracao?.PossuiIntegracaoRaster ?? false,
                UsuarioRaster = configuracaoIntegracao?.UsuarioRaster ?? string.Empty,
                SenhaRaster = configuracaoIntegracao?.SenhaRaster ?? string.Empty,
                URLRaster = configuracaoIntegracao?.URLRaster ?? string.Empty,
                NotificarFalhaIntegracaoRaster = configuracaoIntegracao?.NotificarFalhaIntegracaoRaster ?? false,
                GerarIntegracaoPreSmEtapaCargaDadosTransporteRaster = configuracaoIntegracao?.GerarIntegracaoPreSmEtapaCargaDadosTransporteRaster ?? false,
                GerarIntegracaoEfetivaPreSmEtapaCargaFreteRaster = configuracaoIntegracao?.GerarIntegracaoEfetivaPreSmEtapaCargaFreteRaster ?? false,
                GerarIntegracaoSetRevisaoPreSMnaEtapaIntegracao = configuracaoIntegracao?.GerarIntegracaoSetRevisaoPreSMnaEtapaIntegracao ?? false,
                ReenviarIntegracaoDadosTransporteAoAlterarDadosTransporteRaster = configuracaoIntegracao?.ReenviarIntegracaoDadosTransporteAoAlterarDadosTransporteRaster ?? false,
            };
        }

        private dynamic ObterIntegracaoUnileverFourKites(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLHomologacaoUnileverFourKites = configuracaoIntegracao?.URLHomologacaoUnileverFourKites ?? string.Empty,
                URLProducaoUnileverFourKites = configuracaoIntegracao?.URLProducaoUnileverFourKites ?? string.Empty,
                UsuarioUnileverFourKites = configuracaoIntegracao?.UsuarioUnileverFourKites ?? string.Empty,
                SenhaUnileverFourKites = configuracaoIntegracao?.SenhaUnileverFourKites ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoDigibee(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLIntegracaoDigibee = configuracaoIntegracao?.URLIntegracaoDigibee ?? string.Empty,
                URLIntegracaoDadosCargaDigibee = configuracaoIntegracao?.URLIntegracaoDadosCargaDigibee ?? string.Empty,
                URLIntegracaoCancelamentoDigibee = configuracaoIntegracao?.URLIntegracaoCancelamentoDigibee ?? string.Empty,
                URLAutenticacaoDigibee = configuracaoIntegracao?.URLAutenticacaoDigibee ?? string.Empty,
                UsuarioAutenticacaoDigibee = configuracaoIntegracao?.UsuarioAutenticacaoDigibee ?? string.Empty,
                SenhaAutenticacaoDigibee = configuracaoIntegracao?.SenhaAutenticacaoDigibee ?? string.Empty,
                APIKeyDigibee = configuracaoIntegracao?.APIKeyDigibee ?? string.Empty,
                IntegracaoDigibeePadraoConsinco = configuracaoIntegracao?.IntegracaoDigibeePadraoConsinco ?? false,
                AjustarDataParaCorresponderQuinzenaDigibee = configuracaoIntegracao?.AjustarDataParaCorresponderQuinzenaDigibee ?? false,
                URLIntegracaoDadosContabeisCTeDigibee = configuracaoIntegracao?.URLIntegracaoDadosContabeisCTeDigibee ?? string.Empty,
                URLIntegracaoEscrituracaoCTeDigibee = configuracaoIntegracao?.URLIntegracaoEscrituracaoCTeDigibee ?? string.Empty,
                APIKeyDigibeeGeral = configuracaoIntegracao?.APIKeyDigibeeGeral ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoTelerisco(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLIntegracaoTelerisco = configuracaoIntegracao?.URLIntegracaoTelerisco ?? string.Empty,
                CNPJEmbarcadorTelerisco = configuracaoIntegracao?.CNPJEmbarcadorTelerisco ?? string.Empty,
                CaminhoCertificadoTelerisco = configuracaoIntegracao?.CaminhoCertificadoTelerisco ?? string.Empty,
                NaoEnviarDataEmbarqueGrMotoristaTelerisco = configuracaoIntegracao?.NaoEnviarDataEmbarqueGrMotoristaTelerisco ?? false,
                EnviarEmpresaFixaComoCNPJEmbarcadorNaIntegracaoTelerisco = configuracaoIntegracao?.EnviarEmpresaFixaComoCNPJEmbarcadorNaIntegracaoTelerisco ?? false,
                SenhaCertificadoTelerisco = configuracaoIntegracao?.SenhaCertificadoTelerisco ?? string.Empty,
                CodigosAceitosRetornoTelerisco = configuracaoIntegracao?.CodigosAceitosRetornoTelerisco ?? string.Empty,
                IntegracaoViaPOSTTelerisco = configuracaoIntegracao?.IntegracaoViaPOSTTelerisco ?? false,
                EmpresaFixaTelerisco = new
                {
                    Codigo = configuracaoIntegracao?.EmpresaFixaTelerisco?.Codigo ?? 0,
                    Descricao = configuracaoIntegracao?.EmpresaFixaTelerisco?.Descricao ?? string.Empty
                },
            };
        }

        private dynamic ObterIntegracaoCargoX(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLIntegracaoCargoX = configuracaoIntegracao?.URLIntegracaoCargoX ?? string.Empty,
                TokenCargoX = configuracaoIntegracao?.TokenCargoX ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoRiachuelo(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLIntegracaoRiachuelo = configuracaoIntegracao?.URLIntegracaoRiachuelo ?? string.Empty,
                URLIntegracaoEntregaNFeRiachuelo = configuracaoIntegracao?.URLIntegracaoEntregaNFeRiachuelo ?? string.Empty,
                HabilitarDataSaidaCDLoja = configuracaoIntegracao?.HabilitarDataSaidaCDLoja ?? false,
            };
        }

        private dynamic ObterIntegracaoKrona(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoKrona = configuracaoIntegracao?.PossuiIntegracaoKrona ?? false,
                URLIntegracaoKrona = configuracaoIntegracao?.URLIntegracaoKrona ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoInfolog(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoInfolog = configuracaoIntegracao?.PossuiIntegracaoInfolog ?? false,
                URLIntegracaoInfolog = configuracaoIntegracao?.URLIntegracaoInfolog ?? string.Empty,
                UsuarioInfolog = configuracaoIntegracao?.UsuarioInfolog ?? string.Empty,
                SenhaInfolog = configuracaoIntegracao?.SenhaInfolog ?? string.Empty,
                CodigoOperacaoInfolog = configuracaoIntegracao?.CodigoOperacaoInfolog ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoPH(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoPH = configuracaoIntegracao?.PossuiIntegracaoPH ?? false,
                UsuarioPH = configuracaoIntegracao?.UsuarioPH ?? "",
                SenhaPH = configuracaoIntegracao?.SenhaPH ?? "",
                URLHomologacaoPH = configuracaoIntegracao?.URLHomologacaoPH ?? "",
                URLProducaoPH = configuracaoIntegracao?.URLProducaoPH ?? "",
                CNPJContadorPH = configuracaoIntegracao?.CNPJContadorPH ?? "",
                SoftwarePH = configuracaoIntegracao?.SoftwarePH ?? "",
                PortaPH = configuracaoIntegracao?.PortaPH ?? "",
                IPSocketPH = configuracaoIntegracao?.IPSocketPH ?? "",
                PortaSocketPH = configuracaoIntegracao?.PortaSocketPH ?? "",
            };
        }

        private dynamic ObterIntegracaoBoticario(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow repConfiguracaoIntegracaoBoticarioFreeFlow = new Repositorio.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow configIntegracaoBoticarioFreeFlow = repConfiguracaoIntegracaoBoticarioFreeFlow.Buscar();

            return new
            {
                PossuiIntegracaoBoticario = configuracaoIntegracao?.PossuiIntegracaoBoticario ?? false,
                URLIntegracaoBoticario = configuracaoIntegracao?.URLIntegracaoBoticario ?? string.Empty,
                IntegracaoBoticarioClientId = configuracaoIntegracao?.IntegracaoBoticarioClientId ?? string.Empty,
                IntegracaoBoticarioClientSecret = configuracaoIntegracao?.IntegracaoBoticarioClientSecret ?? string.Empty,
                URLGerarTokenBoticario = configuracaoIntegracao?.URLGerarTokenBoticario ?? string.Empty,
                URLEnvioSequenciaBoticario = configuracaoIntegracao?.URLEnvioSequenciaBoticario ?? string.Empty,

                PossuiIntegracaoBoticarioFreeFlow = configIntegracaoBoticarioFreeFlow?.PossuiIntegracao ?? false,
                URLIntegracaoBoticarioFreeFlow = configIntegracaoBoticarioFreeFlow?.URLIntegracao ?? string.Empty,
                URLAutenticacaoBoticarioFreeFlow = configIntegracaoBoticarioFreeFlow?.URLAutenticacao ?? string.Empty,
                ClientSecretBoticarioFreeFlow = configIntegracaoBoticarioFreeFlow?.ClientSecret ?? string.Empty,
                ClientIdBoticarioFreeFlow = configIntegracaoBoticarioFreeFlow?.ClientId ?? string.Empty,
                URLConsultaAVIPED = configIntegracaoBoticarioFreeFlow?.URLConsultaAVIPED ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoToledo(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLToledo = configuracaoIntegracao?.URLToledo ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoQbit(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLQbit = configuracaoIntegracao?.URLQbit ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoAdagio(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLAdagio = configuracaoIntegracao?.URLAdagio ?? string.Empty,
                EmailAdagio = configuracaoIntegracao?.EmailAdagio ?? string.Empty,
                SenhaAdagio = configuracaoIntegracao?.SenhaAdagio ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoTrizzy(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoTrizy = configuracaoIntegracao?.PossuiIntegracaoTrizy ?? false,
                TokenTrizy = configuracaoIntegracao?.TokenTrizy ?? string.Empty,
                URLTrizy = configuracaoIntegracao?.URLTrizy ?? string.Empty,
                AgenciaTrizy = configuracaoIntegracao?.AgenciaTrizy ?? string.Empty,
                NaoRealizarIntegracaoPedido = configuracaoIntegracao?.NaoRealizarIntegracaoPedido ?? false,
                CNPJCompanyTrizy = configuracaoIntegracao?.CNPJCompanyTrizy ?? string.Empty,
                QuantidadeEixosPadrao = configuracaoIntegracao?.QuantidadeEixosPadrao ?? 0,
            };
        }

        private dynamic ObterIntegracaoAX(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoAX = configuracaoIntegracao?.PossuiIntegracaoAX ?? false,
                URLAX = configuracaoIntegracao?.URLAX ?? string.Empty,
                URLAXContratoFrete = configuracaoIntegracao?.URLAXContratoFrete ?? string.Empty,
                URLAXOrdemVenda = configuracaoIntegracao?.URLAXOrdemVenda ?? string.Empty,
                URLAXCompansacao = configuracaoIntegracao?.URLAXCompansacao ?? string.Empty,
                URLAXPedido = configuracaoIntegracao?.URLAXPedido ?? string.Empty,
                URLAXComplemento = configuracaoIntegracao?.URLAXComplemento ?? string.Empty,
                URLAXCancelamento = configuracaoIntegracao?.URLAXCancelamento ?? string.Empty,
                UsuarioAX = configuracaoIntegracao?.UsuarioAX ?? string.Empty,
                SenhaAX = configuracaoIntegracao?.SenhaAX ?? string.Empty,
                CNPJAX = configuracaoIntegracao?.CNPJAX ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoCobasi(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLCobasi = configuracaoIntegracao?.URLCobasi ?? string.Empty,
                APIKeyCobasi = configuracaoIntegracao?.APIKeyCobasi ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoCadastrosMulti(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoCadastroMulti repositorioIntegracaoCadastroMulti = new Repositorio.Embarcador.Configuracoes.IntegracaoCadastroMulti(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCadastroMulti integracaoCadastroMulti = repositorioIntegracaoCadastroMulti.Buscar();

            return new
            {
                PossuiIntegracaoDeCadastrosMulti = configuracaoIntegracao?.PossuiIntegracaoDeCadastrosMulti ?? false,
                URLIntegracaoCadastrosMulti = configuracaoIntegracao?.URLIntegracaoCadastrosMulti ?? string.Empty,
                TokenIntegracaoCadastrosMulti = configuracaoIntegracao?.TokenIntegracaoCadastrosMulti ?? string.Empty,
                URLIntegracaoCadastrosMultiSecundario = configuracaoIntegracao?.URLIntegracaoCadastrosMultiSecundario ?? string.Empty,
                TokenIntegracaoCadastrosMultiSecundario = configuracaoIntegracao?.TokenIntegracaoCadastrosMultiSecundario ?? string.Empty,
                RealizarIntegracaoDePessoaParaPessoa = configuracaoIntegracao?.RealizarIntegracaoDePessoaParaPessoa ?? false,
                RealizarIntegracaoDeTransportadorParaEmpresa = configuracaoIntegracao?.RealizarIntegracaoDeTransportadorParaEmpresa ?? false,
                RealizarIntegracaoDeContainer = configuracaoIntegracao?.RealizarIntegracaoDeContainer ?? false,
                RealizarIntegracaoDeNavio = configuracaoIntegracao?.RealizarIntegracaoDeNavio ?? false,
                RealizarIntegracaoDeViagem = configuracaoIntegracao?.RealizarIntegracaoDeViagem ?? false,
                RealizarIntegracaoDeCTeAnterior = configuracaoIntegracao?.RealizarIntegracaoDeCTeAnterior ?? false,
                RealizarIntegracaoDePorto = configuracaoIntegracao?.RealizarIntegracaoDePorto ?? false,
                RealizarIntegracaoDeTipoDeContainer = configuracaoIntegracao?.RealizarIntegracaoDeTipoDeContainer ?? false,
                RealizarIntegracaoDeTerminalPortuario = configuracaoIntegracao?.RealizarIntegracaoDeTerminalPortuario ?? false,
                RealizarIntegracaoDeProdutoEmbarcador = configuracaoIntegracao?.RealizarIntegracaoDeProdutoEmbarcador ?? false,
                EnviarDocumentacaoCTeAverbacaoInstancia = integracaoCadastroMulti?.EnviarDocumentacaoCTeAverbacaoInstancia ?? false,
                RealizarIntegracaoDeCTeParaComplementoOSMae = configuracaoIntegracao?.RealizarIntegracaoDeCTeParaComplementoOSMae ?? false,
            };
        }

        private dynamic ObterIntegracaoTotvs(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoDeTotvs = configuracaoIntegracao?.PossuiIntegracaoDeTotvs ?? false,
                URLIntegracaoTotvs = configuracaoIntegracao?.URLIntegracaoTotvs ?? string.Empty,
                UsuarioTotvs = configuracaoIntegracao?.UsuarioTotvs ?? string.Empty,
                SenhaTotvs = configuracaoIntegracao?.SenhaTotvs ?? string.Empty,
                ContextoTotvs = configuracaoIntegracao?.ContextoTotvs ?? string.Empty,
                URLIntegracaoTotvsProcess = configuracaoIntegracao?.URLIntegracaoTotvsProcess ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoAngelLira(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repConfiguracaoIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira configuracaoIntegracaoAngelLira = repConfiguracaoIntegracaoAngelLira.Buscar();

            return new
            {
                HomologacaoAngelLira = configuracaoIntegracaoAngelLira?.Homologacao ?? true,
                UsuarioAngelLira = configuracaoIntegracaoAngelLira?.Usuario ?? string.Empty,
                SenhaAngelLira = configuracaoIntegracaoAngelLira?.Senha ?? string.Empty,
                ObterRotasAutomaticamenteAngelLira = configuracaoIntegracaoAngelLira?.ObterRotasAutomaticamente ?? false,
                IntegracaoTemperaturaAngelLira = configuracaoIntegracaoAngelLira?.IntegracaoTemperatura ?? false,
                URLAngelLira = configuracaoIntegracaoAngelLira?.URLAcesso ?? string.Empty,
                UtilizarDataAgendamentoPedidoAngelLira = configuracaoIntegracaoAngelLira?.UtilizarDataAgendamentoPedido ?? false,
                EnviarDadosFormatadosAngelLira = configuracaoIntegracaoAngelLira?.EnviarDadosFormatados ?? false,
                NaoEnviarRotaViagemAngelLira = configuracaoIntegracaoAngelLira?.NaoEnviarRotaViagem ?? false,
                GerarViagensPorPedidoAngelLira = configuracaoIntegracaoAngelLira?.GerarViagensPorPedido ?? false,
                AplicarRegraLocalPalletizacaoAngelLira = configuracaoIntegracaoAngelLira?.AplicarRegraLocalPalletizacao ?? false,
                ConsultarPosicaoAbastecimentoAngelLira = configuracaoIntegracaoAngelLira?.ConsultarPosicaoAbastecimento ?? false,
                URLAcessoPedido = configuracaoIntegracaoAngelLira?.URLAcessoPedido ?? string.Empty,
                UsuarioAcessoPedido = configuracaoIntegracaoAngelLira?.UsuarioAcessoPedido ?? string.Empty,
                SenhaAcessoPedido = configuracaoIntegracaoAngelLira?.SenhaAcessoPedido ?? string.Empty,
                UtilizarDataAtualETempoRotaParaInicioEFimViagemAngelLira = configuracaoIntegracaoAngelLira?.UtilizarDataAtualETempoRotaParaInicioEFimViagem ?? false,
                RegraCodigoIdentificacaoViagem = configuracaoIntegracaoAngelLira?.RegraCodigoIdentificacaoViagem ?? AngelLiraRegraCodigoIdentificacaoViagem.NumeroPedidoEmbarcadorMaisPlaca,
                IgnorarValidacaoCargaAgrupadaRegraCodigoViagemAngelLira = configuracaoIntegracaoAngelLira?.IgnorarValidacaoCargaAgrupadaRegraCodigoViagem ?? false
            };
        }

        private dynamic ObterIntegracaoA52(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoA52 repConfiguracaoIntegracaoA52 = new Repositorio.Embarcador.Configuracoes.IntegracaoA52(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracaoA52 = repConfiguracaoIntegracaoA52.BuscarPrimeiroRegistro();

            return new
            {
                URLA52 = configuracaoIntegracaoA52?.URL ?? string.Empty,
                CPFCNPJA52 = configuracaoIntegracaoA52?.CPFCNPJ ?? string.Empty,
                SenhaA52 = configuracaoIntegracaoA52?.Senha ?? string.Empty,
                UtilizarDataAgendamentoPedidoA52 = configuracaoIntegracaoA52?.UtilizarDataAgendamentoPedido ?? false,
                IntegrarMacrosDadosTransporteCargaA52 = configuracaoIntegracaoA52?.IntegrarMacrosDadosTransporteCarga ?? false,
                IntegrarSituacaoMotoristaA52 = configuracaoIntegracaoA52?.IntegrarSituacaoMotorista ?? false,
                AplicarRegraLocalPalletizacaoA52 = configuracaoIntegracaoA52?.AplicarRegraLocalPalletizacao ?? false,
                URLNovaA52 = configuracaoIntegracaoA52?.URLNova ?? string.Empty,
                VersaoIntegracaoA52 = configuracaoIntegracaoA52?.VersaoIntegracao ?? VersaoA52Enum.Versao10
            };
        }

        private dynamic ObterIntegracaoBBC(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoBBC repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoBBC(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracao = repConfiguracaoIntegracao.BuscarPrimeiroRegistro();

            return new
            {
                URLBBC = configuracaoIntegracao?.URL ?? string.Empty,
                PossuiIntegracaoViagemBBC = configuracaoIntegracao?.PossuiIntegracaoViagem ?? false,
                URLViagemBBC = configuracaoIntegracao?.URLViagem ?? string.Empty,
                CnpjEmpresaViagemBBC = configuracaoIntegracao?.CnpjEmpresaViagem ?? string.Empty,
                SenhaViagemBBC = configuracaoIntegracao?.SenhaViagem ?? string.Empty,
                ClientSecretBBC = configuracaoIntegracao?.ClientSecret ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoMercadoLivre(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre repConfiguracaoIntegracaoMercadoLivre = new Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre configuracaoMercadoLivre = repConfiguracaoIntegracaoMercadoLivre.BuscarPrimeiroRegistro();

            return new
            {
                IDMercadoLivre = configuracaoMercadoLivre?.ID ?? string.Empty,
                SecretKeyMercadoLivre = configuracaoMercadoLivre?.SecretKey ?? string.Empty,
                URLMercadoLivre = configuracaoMercadoLivre?.URL ?? string.Empty,
                LimparComposicaoCargaRetiradaRotaFacilityMercadoLivre = configuracaoMercadoLivre?.LimparComposicaoCargaRetiradaRotaFacility ?? false,
                NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes = configuracaoMercadoLivre?.NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes ?? false
            };
        }

        private dynamic ObterIntegracaoKuehneNagel(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoKuehneNagel repConfiguracaoIntegracaoKuehneNagel = new Repositorio.Embarcador.Configuracoes.IntegracaoKuehneNagel(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKuehneNagel configuracaoIntegracaoKuehneNagel = repConfiguracaoIntegracaoKuehneNagel.Buscar();

            return new
            {
                PossuiIntegracaoKuehneNagel = configuracaoIntegracaoKuehneNagel?.PossuiIntegracao ?? false,
                EnderecoFTPKuehneNagel = configuracaoIntegracaoKuehneNagel?.EnderecoFTP ?? string.Empty,
                UsuarioKuehneNagel = configuracaoIntegracaoKuehneNagel?.Usuario ?? string.Empty,
                SenhaKuehneNagel = configuracaoIntegracaoKuehneNagel?.Senha ?? string.Empty,
                DiretorioKuehneNagel = configuracaoIntegracaoKuehneNagel?.Diretorio ?? string.Empty,
                PortaKuehneNagel = configuracaoIntegracaoKuehneNagel?.Porta ?? string.Empty,
                PassivoKuehneNagel = configuracaoIntegracaoKuehneNagel?.Passivo ?? false,
                UtilizarSFTPKuehneNagel = configuracaoIntegracaoKuehneNagel?.UtilizarSFTP ?? false,
                SSLKuehneNagel = configuracaoIntegracaoKuehneNagel?.SSL ?? false
            };
        }

        private dynamic ObterIntegracaoDansales(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDansales repConfiguracaoIntegracaoDansales = new Repositorio.Embarcador.Configuracoes.IntegracaoDansales(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales configuracaoIntegracaoDansales = repConfiguracaoIntegracaoDansales.Buscar();

            return new
            {
                PossuiIntegracaoDansales = configuracaoIntegracaoDansales?.PossuiIntegracao ?? false,
                UsuarioDansales = configuracaoIntegracaoDansales?.Usuario ?? string.Empty,
                SenhaDansales = configuracaoIntegracaoDansales?.Senha ?? string.Empty,
                URLIntegracaoDansales = configuracaoIntegracaoDansales?.URLIntegracao ?? string.Empty,
                URLIntegracaoDansalesChat = configuracaoIntegracaoDansales?.URLIntegracaoChat ?? string.Empty,
                URLIntegracaoDansalesToken = configuracaoIntegracaoDansales?.URLToken ?? string.Empty,
                UsuarioDansalesToken = configuracaoIntegracaoDansales?.UsuarioToken ?? string.Empty,
                SenhaDansalesToken = configuracaoIntegracaoDansales?.SenhaToken ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoTarget(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTargetGeral repConfiguracaoIntegracaoTargetGeral = new Repositorio.Embarcador.Configuracoes.IntegracaoTargetGeral(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral configuracaoIntegracaoTargetGeral = repConfiguracaoIntegracaoTargetGeral.Buscar();

            return new
            {
                PossuiIntegracaoTargetEmpresa = configuracaoIntegracaoTargetGeral?.PossuiIntegracaoEmpresa ?? false,
                URLTargetEmpresa = configuracaoIntegracaoTargetGeral?.URLEmpresa ?? string.Empty,
                UsuarioTargetEmpresa = configuracaoIntegracaoTargetGeral?.UsuarioEmpresa ?? string.Empty,
                SenhaTargetEmpresa = configuracaoIntegracaoTargetGeral?.SenhaEmpresa ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoExtratta(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoExtratta repIntegracaoExtratta = new Repositorio.Embarcador.Configuracoes.IntegracaoExtratta(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta integracaoExtratta = repIntegracaoExtratta.Buscar();

            return new
            {
                PossuiIntegracaoExtratta = integracaoExtratta?.PossuiIntegracao ?? false,
                URLExtratta = integracaoExtratta?.URL ?? string.Empty,
                TokenExtratta = integracaoExtratta?.Token ?? string.Empty,
                CNPJAplicacaoExtratta = integracaoExtratta?.CNPJAplicacao ?? string.Empty,
                CNPJEmpresaExtratta = integracaoExtratta?.CNPJEmpresa ?? string.Empty,
                DocumentoUsuarioExtratta = integracaoExtratta?.DocumentoUsuario ?? string.Empty,
                UsuarioExtratta = integracaoExtratta?.Usuario ?? string.Empty,
                IntegrarAbastecimentoComTicketLog = integracaoExtratta?.IntegrarAbastecimentoComTicketLog ?? false,
                CodigoClienteTicketLog = integracaoExtratta?.CodigoClienteTicketLog ?? string.Empty,
                CodigoProdutoTicketLog = integracaoExtratta?.CodigoProdutoTicketLog ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoRavex(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoRavex repConfiguracaoIntegracaoRavex = new Repositorio.Embarcador.Configuracoes.IntegracaoRavex(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRavex configuracaoIntegracaoRavex = repConfiguracaoIntegracaoRavex.Buscar();

            return new
            {
                PossuiIntegracaoRavex = configuracaoIntegracaoRavex?.PossuiIntegracao ?? false,
                URLRavex = configuracaoIntegracaoRavex?.UrlIntegracao ?? string.Empty,
                UsuarioRavex = configuracaoIntegracaoRavex?.Usuario ?? string.Empty,
                SenhaRavex = configuracaoIntegracaoRavex?.Senha ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoMicDta(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMicDta repConfiguracaoIntegracaoMicDtal = new Repositorio.Embarcador.Configuracoes.IntegracaoMicDta(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMicDta configuracaoIntegracaoMicDta = repConfiguracaoIntegracaoMicDtal.Buscar();

            return new
            {
                PossuiIntegracaoMicDta = configuracaoIntegracaoMicDta?.PossuiIntegracao ?? false,
                URLMicDta = configuracaoIntegracaoMicDta?.URL ?? string.Empty,
                MetodoManifestacaoEmbarcaMicDta = configuracaoIntegracaoMicDta?.MetodoManifestacaoEmbarca ?? string.Empty,
                GerarIntegracaNaEtapaDoFrete = configuracaoIntegracaoMicDta?.GerarIntegracaNaEtapaDoFrete ?? false,
                LicencaTNTI = configuracaoIntegracaoMicDta?.LicencaTNTI ?? string.Empty,
                VencimentoLicencaTNTI = configuracaoIntegracaoMicDta != null && configuracaoIntegracaoMicDta.VencimentoLicencaTNTI.HasValue ? configuracaoIntegracaoMicDta.VencimentoLicencaTNTI.Value.ToString("dd/MM/yyyy") : ""
            };
        }

        private dynamic ObterIntegracaoGadle(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoGadle repConfiguracaoIntegracaoGadle = new Repositorio.Embarcador.Configuracoes.IntegracaoGadle(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle configuracaoIntegracaoGadle = repConfiguracaoIntegracaoGadle.Buscar();

            return new
            {
                URLIntegracaoGadle = configuracaoIntegracaoGadle?.URLIntegracaoGadle ?? string.Empty,
                TokenIntegracaoGadle = configuracaoIntegracaoGadle?.TokenIntegracaoGadle ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoOnetrust(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLOnetrust = configuracaoIntegracao?.URLOnetrust ?? string.Empty,
                URLObterTokenOnetrust = configuracaoIntegracao?.URLObterTokenOnetrust ?? string.Empty,
                UrlRegularizacaoOneTrust = configuracaoIntegracao?.UrlRegularizacaoOneTrust ?? string.Empty,
                PurposeIdOneTrust = configuracaoIntegracao?.PurposeIdOneTrust ?? string.Empty,
                ClientIdOneTrust = configuracaoIntegracao?.ClientIdOneTrust ?? string.Empty,
                ClientSecretOneTrust = configuracaoIntegracao?.ClientSecretOneTrust ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoSintegra(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                URLSintegra = configuracaoIntegracao?.URLSintegra ?? string.Empty,
                TokenSintegra = configuracaoIntegracao?.TokenSintegra ?? string.Empty,
                IntervaloConsultaSintegra = configuracaoIntegracao?.IntervaloConsultaSintegra ?? 0,
            };
        }

        private dynamic ObterIntegracaoTelhaNorte(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro repositorioTelhaNorteParametro = new Repositorio.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro> parametros = repositorioTelhaNorteParametro.Buscar();

            return new
            {
                URLTelhaNorte = configuracaoIntegracao?.URLTelhaNorte ?? string.Empty,
                URLPedidoTelhaNorte = configuracaoIntegracao?.URLPedidoTelhaNorte ?? string.Empty,
                URLObterToken = configuracaoIntegracao?.URLObterTokenTelhaNorte ?? string.Empty,
                Chaves = (from o in parametros
                          select new
                          {
                              o.Codigo,
                              o.Chave,
                              o.Valor
                          }).ToList()
            };
        }

        private dynamic ObterIntegracaoMichelin(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                SenhaMichelin = configuracaoIntegracao?.SenhaMichelin ?? string.Empty,
                UsuarioMichelin = configuracaoIntegracao?.UsuarioMichelin ?? string.Empty,
                URLHomologacaoMichelin = configuracaoIntegracao?.URLHomologacaoMichelin ?? string.Empty,
                URLProducaoMichelin = configuracaoIntegracao?.URLProducaoMichelin ?? string.Empty,
                CodigoTransportadoraMichelin = configuracaoIntegracao?.CodigoTransportadoraMichelin ?? string.Empty,
                CnpjTransportadoraMichelin = configuracaoIntegracao?.CnpjTransportadoraMichelin ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoUltragaz(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new
            {
                PossuiIntegracaoUltragaz = configuracaoIntegracao?.PossuiIntegracaoUltragaz ?? false,
                URLIntegracaoUltragaz = configuracaoIntegracao?.URLIntegracaoUltragaz ?? string.Empty,
                URLAutenticacaoUltragaz = configuracaoIntegracao?.URLAutenticacaoUltragaz ?? string.Empty,
                ClientSecretUltragaz = configuracaoIntegracao?.ClientSecretUltragaz ?? string.Empty,
                ClientIdUltragaz = configuracaoIntegracao?.ClientIdUltragaz ?? string.Empty,
                URLContabilizacaoUltragaz = configuracaoIntegracao?.URLContabilizacaoUltragaz ?? string.Empty,
                URLIntegracaoVeiculoUltragaz = configuracaoIntegracao?.URLIntegracaoVeiculoUltragaz ?? string.Empty,
                NaoPermitirReenviarIntegracaoPagamentoAgRetorno = configuracaoIntegracao?.NaoPermitirReenviarIntegracaoPagamentoAgRetorno ?? false
            };
        }

        private dynamic ObterIntegracaoInforDoc(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoInforDoc repConfiguracaoIntegracaoInforDoc = new Repositorio.Embarcador.Configuracoes.IntegracaoInforDoc(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoInforDoc configuracaoIntegracaoInforDoc = repConfiguracaoIntegracaoInforDoc.Buscar();

            return new
            {
                URLInforDoc = configuracaoIntegracaoInforDoc?.URL ?? string.Empty,
                APIKeyInforDoc = configuracaoIntegracaoInforDoc?.APIKey ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoIsis(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoIsis repConfiguracaoIntegracaoIsis = new Repositorio.Embarcador.Configuracoes.IntegracaoIsis(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIsis configuracaoIntegracaoIsis = repConfiguracaoIntegracaoIsis.Buscar();

            return new
            {
                PossuiIntegracaoFTPIsis = configuracaoIntegracaoIsis?.PossuiIntegracaoFTP ?? false,
                EnderecoFTPIsis = configuracaoIntegracaoIsis?.EnderecoFTP ?? string.Empty,
                UsuarioIsis = configuracaoIntegracaoIsis?.Usuario ?? string.Empty,
                SenhaIsis = configuracaoIntegracaoIsis?.Senha ?? string.Empty,
                DiretorioIsis = configuracaoIntegracaoIsis?.Diretorio ?? string.Empty,
                PortaIsis = configuracaoIntegracaoIsis?.Porta ?? string.Empty,
                PassivoIsis = configuracaoIntegracaoIsis?.Passivo ?? false,
                UtilizarSFTPIsis = configuracaoIntegracaoIsis?.UtilizarSFTP ?? false,
                SSLIsis = configuracaoIntegracaoIsis?.SSL ?? false,
                NomenclaturaArquivoIsis = configuracaoIntegracaoIsis?.NomenclaturaArquivo ?? string.Empty,
                NomenclaturaArquivoCarregamentoIsis = configuracaoIntegracaoIsis?.NomenclaturaArquivoCarregamento ?? string.Empty,
                DiretorioCarregamentoIsis = configuracaoIntegracaoIsis?.DiretorioCarregamento ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoMagalu(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMagalu repConfiguracaoIntegracaoMagalu = new Repositorio.Embarcador.Configuracoes.IntegracaoMagalu(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMagalu configuracaoIntegracaoMagalu = repConfiguracaoIntegracaoMagalu.Buscar();

            return new
            {
                URLMagalu = configuracaoIntegracaoMagalu?.URL ?? string.Empty,
                TokenMagalu = configuracaoIntegracaoMagalu?.Token ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoGSW(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoGSW repConfiguracaoIntegracaoGSW = new Repositorio.Embarcador.Configuracoes.IntegracaoGSW(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGSW configuracaoIntegracaoGSW = repConfiguracaoIntegracaoGSW.Buscar();

            return new
            {
                URLGSW = configuracaoIntegracaoGSW?.URL ?? string.Empty,
                UsuarioGSW = configuracaoIntegracaoGSW?.Usuario ?? string.Empty,
                SenhaGSW = configuracaoIntegracaoGSW?.Senha ?? string.Empty,
                CodigoInicialConsultaXMLCTeGSW = configuracaoIntegracaoGSW?.CodigoInicialConsultaXMLCTe.ToString() ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoArquivei(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoArquivei repConfiguracaoIntegracaoArquivei = new Repositorio.Embarcador.Configuracoes.IntegracaoArquivei(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArquivei configuracaoIntegracaoArquivei = repConfiguracaoIntegracaoArquivei.Buscar();

            return new
            {
                URLArquivei = configuracaoIntegracaoArquivei?.URLArquivei ?? string.Empty,
                IDArquivei = configuracaoIntegracaoArquivei?.IDArquivei ?? string.Empty,
                KeyArquivei = configuracaoIntegracaoArquivei?.KeyArquivei ?? string.Empty,
                CodigoInicialConsultaXMLCTeArquivei = configuracaoIntegracaoArquivei?.CodigoInicialConsultaXMLCTeArquivei.ToString() ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoCTASmart(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoCTASmart repConfiguracaoIntegracaoCTASmart = new Repositorio.Embarcador.Configuracoes.IntegracaoCTASmart(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart> configuracaoIntegracaoCTASmart = repConfiguracaoIntegracaoCTASmart.BuscarTodos();
            return new
            {
                PossuiIntegracaoCTASmart = configuracaoIntegracao?.PossuiIntegracaoCTASmart ?? false,
                ListConfiguracoesIntegracaoCTASmart = configuracaoIntegracaoCTASmart.Select(o => new
                {
                    o.Codigo,
                    URLCTASmart = o.URL,
                    TokenCTASmart = o.Token,
                    DataInicioCTASmart = o.DataInicio?.ToString("dd/MM/yyyy") ?? "",
                    CodigoEmpresaCTASmart = o.CodigoEmpresa
                }).ToList(),
            };
        }

        private dynamic ObterIntegracaoDPA(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDPA repIntegracaoDPA = new Repositorio.Embarcador.Configuracoes.IntegracaoDPA(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA configuracaoIntegracaoDPA = repIntegracaoDPA.Buscar();

            return new
            {
                URLIntegracaoDPA = configuracaoIntegracaoDPA?.URLIntegracaoDPA ?? string.Empty,
                URLAutenticacaoDPA = configuracaoIntegracaoDPA?.URLAutenticacaoDPA ?? string.Empty,
                UsuarioAutenticacaoDPA = configuracaoIntegracaoDPA?.UsuarioAutenticacaoDPA ?? string.Empty,
                SenhaAutenticacaoDPA = configuracaoIntegracaoDPA?.SenhaAutenticacaoDPA ?? string.Empty,
                URLIntegracaoDPACiot = configuracaoIntegracaoDPA?.URLIntegracaoDPACiot ?? string.Empty,
                URLAutenticacaoDPACiot = configuracaoIntegracaoDPA?.URLAutenticacaoDPACiot ?? string.Empty,
                UsuarioAutenticacaoDPACiot = configuracaoIntegracaoDPA?.UsuarioAutenticacaoDPACiot ?? string.Empty,
                SenhaAutenticacaoDPACiot = configuracaoIntegracaoDPA?.SenhaAutenticacaoDPACiot ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoSaintGobain(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSaintGobain repIntegracaoSaintGobain = new Repositorio.Embarcador.Configuracoes.IntegracaoSaintGobain(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain configuracaoIntegracaoSaintGobain = repIntegracaoSaintGobain.Buscar();

            return new
            {
                //T_CONFIGURACAO_INTEGRACAO
                URLIntegracaoSaintGobain = configuracaoIntegracao?.URLIntegracaoSaintGobain ?? string.Empty,
                UserNameSaintGobain = configuracaoIntegracao?.UserNameSaintGobain ?? string.Empty,
                PasswordSaintGobain = configuracaoIntegracao?.PasswordSaintGobain ?? string.Empty,
                PossuiIntegracaoSaintGobain = configuracaoIntegracao?.PossuiIntegracaoSaintGobain ?? false,
                //T_CONFIGURACAO_INTEGRACAO_SAINTGOBAIN
                URLIntegracaoPedidoSaintGobain = configuracaoIntegracaoSaintGobain?.UrlConsultaPedido ?? string.Empty,
                URLIntegracaoUsuarioSaintGobain = configuracaoIntegracaoSaintGobain?.UrlConsultaUsuario ?? string.Empty,
                URLValidaTokenSaintGobain = configuracaoIntegracaoSaintGobain?.UrlValidaToken ?? string.Empty,
                ApikeySaintGobain = configuracaoIntegracaoSaintGobain?.APIKey ?? string.Empty,
                ClientIDSaintGobain = configuracaoIntegracaoSaintGobain?.ClientID ?? string.Empty,
                ClientSecretSaintGobain = configuracaoIntegracaoSaintGobain?.ClientSecret ?? string.Empty,
                UrlIntegracaoCargaSnowFlake = configuracaoIntegracaoSaintGobain?.UrlIntegracaoCargaSnowFlake ?? string.Empty,
                UrlIntegracaoAgendamentoSnowFlake = configuracaoIntegracaoSaintGobain?.UrlIntegracaoAgendamentoSnowFlake ?? string.Empty,
                UrlIntegracaoFreteSnowFlake = configuracaoIntegracaoSaintGobain?.UrlIntegracaoFreteSnowFlake ?? string.Empty,
                UsuariosSnowFlake = configuracaoIntegracaoSaintGobain?.UsuariosSnowFlake ?? string.Empty,
                SenhaSnowFlake = configuracaoIntegracaoSaintGobain?.SenhaSnowFlake ?? string.Empty,
                ApiKeySnowFlake = configuracaoIntegracaoSaintGobain?.ApiKeySnowFlake ?? string.Empty,
                UtilizarEndPointPIPO = configuracaoIntegracaoSaintGobain?.UtilizarEndPointPIPO ?? false,
            };
        }

        private dynamic ObterIntegracaoHavan(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoHavan repConfiguracaoIntegracaoHavan = new Repositorio.Embarcador.Configuracoes.IntegracaoHavan(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHavan configuracaoIntegracaoHavan = repConfiguracaoIntegracaoHavan.Buscar();

            return new
            {
                URLAutenticacaoHavan = configuracaoIntegracaoHavan?.URLAutenticacao ?? string.Empty,
                URLEnvioOcorrenciaHavan = configuracaoIntegracaoHavan?.URLEnvioOcorrencia ?? string.Empty,
                UsuarioHavan = configuracaoIntegracaoHavan?.Usuario ?? string.Empty,
                SenhaHavan = configuracaoIntegracaoHavan?.Senha ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoFrota162(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoFrota162 repositorioConfiguracaoIntegracaoFrota162 = new Repositorio.Embarcador.Configuracoes.IntegracaoFrota162(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162 configuracaoIntegracaoFrota162 = repositorioConfiguracaoIntegracaoFrota162.Buscar();

            return new
            {
                PossuiIntegracaoFrota162 = configuracaoIntegracaoFrota162?.PossuiIntegracaoFrota162 ?? false,
                UsuarioFrota162 = configuracaoIntegracaoFrota162?.Usuario ?? string.Empty,
                SenhaFrota162 = configuracaoIntegracaoFrota162?.Senha ?? string.Empty,
                URLFrota162 = configuracaoIntegracaoFrota162?.URL ?? string.Empty,
                TokenFrota162 = configuracaoIntegracaoFrota162?.Token ?? string.Empty,
                SecretKeyFrota162 = configuracaoIntegracaoFrota162?.SecretKey ?? string.Empty,
                CompanyIdFrota162 = configuracaoIntegracaoFrota162?.CompanyId ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoDexco(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDexco repositorioConfiguracaoIntegracaoDexco = new Repositorio.Embarcador.Configuracoes.IntegracaoDexco(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDexco configuracaoIntegracaoDexco = repositorioConfiguracaoIntegracaoDexco.BuscarIntegracao();

            return new
            {
                AccessKeyDexco = configuracaoIntegracaoDexco?.AccessKeyDexco ?? string.Empty,
                FoType = configuracaoIntegracaoDexco?.FoType ?? string.Empty,
                UrlDexco = configuracaoIntegracaoDexco?.UrlDexco ?? string.Empty,
                UsuarioDexco = configuracaoIntegracaoDexco?.Usuario ?? string.Empty,
                SenhaDexco = configuracaoIntegracaoDexco?.Senha ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoIntercab(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioConfiguracaoIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab configuracaoIntegracaoIntercab = repositorioConfiguracaoIntegracaoIntercab.BuscarIntegracao();

            return new
            {
                PossuiIntegracaoIntercab = configuracaoIntegracaoIntercab?.PossuiIntegracaoIntercab ?? false,
                AtivarIntegracaoCargas = configuracaoIntegracaoIntercab?.AtivarIntegracaoCargas ?? false,
                AtivarNovoHomeDash = configuracaoIntegracaoIntercab?.AtivarNovoHomeDash ?? false,
                IntegracaoDocumentacaoCarga = configuracaoIntegracaoIntercab?.IntegracaoDocumentacaoCarga ?? false,
                AtivarIntegracaoCancelamentoCarga = configuracaoIntegracaoIntercab?.AtivarIntegracaoCancelamentoCarga ?? false,
                AtivarIntegracaoCartaCorrecao = configuracaoIntegracaoIntercab?.AtivarIntegracaoCartaCorrecao ?? false,
                CodigoTipoOperacao = configuracaoIntegracaoIntercab?.CodigoTipoOperacao ?? string.Empty,
                URLIntercab = configuracaoIntegracaoIntercab?.URLIntercab ?? string.Empty,
                TokenIntercab = configuracaoIntegracaoIntercab?.TokenIntercab ?? string.Empty,
                AtivarIntegracaoMercante = configuracaoIntegracaoIntercab?.AtivarIntegracaoMercante ?? false,
                AtivarIntegracaoCteManual = configuracaoIntegracaoIntercab?.AtivarIntegracaoCteManual ?? false,
                AtivarIntegracaoMDFeAquaviario = configuracaoIntegracaoIntercab?.AtivarIntegracaoMDFeAquaviario ?? false,
                AtivarIntegracaoCargaAtualParaNovo = configuracaoIntegracaoIntercab?.AtivarIntegracaoCargaAtualParaNovo ?? false,
                AtivarIntegracaoOcorrencias = configuracaoIntegracaoIntercab?.AtivarIntegracaoOcorrencias ?? false,
                DefinirModalPeloTipoCarga = configuracaoIntegracaoIntercab?.DefinirModalPeloTipoCarga ?? false,
                AtivarIntegracaoFatura = configuracaoIntegracaoIntercab?.AtivarIntegracaoFatura ?? false,
                BuscarTipoServicoModeloDocumentoVinculadoCarga = configuracaoIntegracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false,
                ModificarTimelineDeAcordoComTipoServicoDocumento = configuracaoIntegracaoIntercab?.ModificarTimelineDeAcordoComTipoServicoDocumento ?? false,
                HabilitarTimelineIntegracaoFaturaCarga = configuracaoIntegracaoIntercab?.HabilitarTimelineIntegracaoFaturaCarga ?? false,
                HabilitarTimelineFaturamentoCarga = configuracaoIntegracaoIntercab?.HabilitarTimelineFaturamentoCarga ?? false,
                HabilitarTimelineMercanteCarga = configuracaoIntegracaoIntercab?.HabilitarTimelineMercanteCarga ?? false,
                HabilitarTimelineMDFeAquaviario = configuracaoIntegracaoIntercab?.HabilitarTimelineMDFeAquaviario ?? false,
                AtivarNovosFiltrosConsultaCarga = configuracaoIntegracaoIntercab?.AtivarNovosFiltrosConsultaCarga ?? false,
                AjustarLayoutFiltrosTelaCarga = configuracaoIntegracaoIntercab?.AjustarLayoutFiltrosTelaCarga ?? false,
                ModificarTimelineIntegracaoCarga = configuracaoIntegracaoIntercab?.ModificarTimelineIntegracaoCarga ?? false,
                AtivarPreFiltrosTelaCargaIntercab = configuracaoIntegracaoIntercab?.AtivarPreFiltrosTelaCarga ?? false,
                QuantidadeDiasParaDataInicialIntercab = configuracaoIntegracaoIntercab?.QuantidadeDiasParaDataInicial ?? 0,
                SituacoesCargaIntercab = configuracaoIntegracaoIntercab?.SituacoesCarga.ToList() ?? null,
                HabilitarTimelineCargaPortoPorto = configuracaoIntegracaoIntercab?.HabilitarTimelineCargaPortoPorto ?? false,
                HabilitarTimelineCargaPorta = configuracaoIntegracaoIntercab?.HabilitarTimelineCargaPorta ?? false,
                HabilitarTimelineCargaSVMProprio = configuracaoIntegracaoIntercab?.HabilitarTimelineCargaSVMProprio ?? false,
                HabilitarTimelineCargaFeeder = configuracaoIntegracaoIntercab?.HabilitarTimelineCargaFeeder ?? false,
                RemoverObrigacaoCodigoEmbarcacaoCadastroNavioIntercab = configuracaoIntegracaoIntercab?.RemoverObrigacaoCodigoEmbarcacaoCadastroNavio ?? false,
                AtivarControleDashRegiaoOperador = configuracaoIntegracaoIntercab?.AtivarControleDashRegiaoOperador ?? false,
                SelecionarTipoOperacaoIntercab = configuracaoIntegracaoIntercab?.SelecionarTipoOperacao ?? false,
                AtivarGeracaoCCePelaRolagemWS = configuracaoIntegracaoIntercab?.AtivarGeracaoCCePelaRolagemWS ?? false,
                TipoOperacaoIntercab = new
                {
                    Codigo = configuracaoIntegracaoIntercab?.TipoOperacao?.Codigo ?? 0,
                    Descricao = configuracaoIntegracaoIntercab?.TipoOperacao?.Descricao ?? string.Empty
                },
                TipoCargaPadraoIntercab = new
                {
                    Codigo = configuracaoIntegracaoIntercab?.TipoDeCargaPadrao?.Codigo ?? 0,
                    Descricao = configuracaoIntegracaoIntercab?.TipoDeCargaPadrao?.Descricao ?? string.Empty
                },
            };
        }

        private dynamic ObterIntegracaoProtheus(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoProtheus repositorioConfiguracaoIntegracaoProtheus = new Repositorio.Embarcador.Configuracoes.IntegracaoProtheus(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoProtheus configuracaoIntegracaoProtheus = repositorioConfiguracaoIntegracaoProtheus.Buscar();

            return new
            {
                PossuiIntegracaoProtheus = configuracaoIntegracaoProtheus?.PossuiIntegracaoProtheus ?? false,
                URLAutenticacaoProtheus = configuracaoIntegracaoProtheus?.URLAutenticacao ?? string.Empty,
                UsuarioProtheus = configuracaoIntegracaoProtheus?.Usuario ?? string.Empty,
                SenhaProtheus = configuracaoIntegracaoProtheus?.Senha ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoSimonetti(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSimonetti repositorioConfiguracaoIntegracaoSimonetti = new Repositorio.Embarcador.Configuracoes.IntegracaoSimonetti(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSimonetti configuracaoIntegracaoSimonetti = repositorioConfiguracaoIntegracaoSimonetti.BuscarDadosIntegracao();

            return new
            {
                PossuiIntegracaoSimonetti = configuracaoIntegracaoSimonetti?.PossuiIntegracaoSimonetti ?? false,
                URLEnviaOcorrenciaSimonetti = configuracaoIntegracaoSimonetti?.URLEnviaOcorrenciaSimonetti ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoUnilever(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = repositorioConfiguracaoIntegracaoUnilever.Buscar();

            return new
            {
                PossuiIntegracaoUnilever = configuracaoIntegracaoUnilever?.PossuiIntegracaoUnilever ?? false,
                IntegrarValorPreCalculo = configuracaoIntegracaoUnilever?.IntegrarValorPreCalculo ?? false,
                IntegrarDadosValePedagio = configuracaoIntegracaoUnilever?.IntegrarDadosValePedagio ?? false,
                IntegrarAvancoParaEmissao = configuracaoIntegracaoUnilever?.IntegrarAvancoParaEmissao ?? false,
                IntegrarLeilaoManual = configuracaoIntegracaoUnilever?.IntegrarLeilaoManual ?? false,
                IntegrarLotePagamento = configuracaoIntegracaoUnilever?.IntegrarLotePagamento ?? false,
                IntegrarCanhoto = configuracaoIntegracaoUnilever?.IntegrarCanhoto ?? false,
                IntegrarCancelamentoPagamento = configuracaoIntegracaoUnilever?.IntegrarCancelamentoPagamento ?? false,
                URLIntegracaoRetornoUnilever = configuracaoIntegracaoUnilever?.URLIntegracaoRetornoUnilever ?? string.Empty,
                ClientIDIntegracaoUnilever = configuracaoIntegracaoUnilever?.ClientIDIntegracaoUnilever ?? string.Empty,
                ClientSecretIntegracaoUnilever = configuracaoIntegracaoUnilever?.ClientSecretIntegracaoUnilever ?? string.Empty,
                URLIntegracaoAvancoParaEmissao = configuracaoIntegracaoUnilever?.URLIntegracaoAvancoParaEmissao ?? string.Empty,
                URLIntegracaoTravamentoDTUnilever = configuracaoIntegracaoUnilever?.URLIntegracaoTravamentoDTUnilever ?? string.Empty,
                URLIntegracaoProvisaoUnilever = configuracaoIntegracaoUnilever?.URLIntegracaoProvisaoUnilever ?? string.Empty,
                URLIntegracaoValorPreCalculoUnilever = configuracaoIntegracaoUnilever?.URLIntegracaoValorPreCalculoUnilever ?? string.Empty,
                URLIntegracaoCancelamento = configuracaoIntegracaoUnilever?.URLIntegracaoCancelamento ?? string.Empty,
                URLIntegracaoLeilaoManual = configuracaoIntegracaoUnilever?.URLIntegracaoLeilaoManual ?? string.Empty,
                URLIntegracaoEscrituracaoRetorno = configuracaoIntegracaoUnilever?.URLIntegracaoEscrituracaoRetorno ?? string.Empty,
                URLIntegracaoLotePagamento = configuracaoIntegracaoUnilever?.URLIntegracaoLotePagamento ?? string.Empty,
                URLIntegracaoCancelamentoProvisao = configuracaoIntegracaoUnilever?.URLIntegracaoCancelamentoProvisao ?? string.Empty,
                URLIntegracaoCanhoto = configuracaoIntegracaoUnilever?.URLIntegracaoCanhoto ?? string.Empty,
                URLIntegracaoCancelamentoPagamento = configuracaoIntegracaoUnilever?.URLIntegracaoCancelamentoPagamento ?? string.Empty,
                URLIntegracaoOcorrencia = configuracaoIntegracaoUnilever?.URLIntegracaoOcorrencia ?? string.Empty,

            };
        }

        private dynamic ObterIntegracaoBrasilRisk(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoTipoIntegracao configuracaoTipoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoTipoIntegracao()
            {
                TipoIntegracao = TipoIntegracao.BrasilRisk
            };

            configuracaoTipoIntegracao = ObterConfiguracaoDoTipoIntegracao(configuracaoTipoIntegracao, unitOfWork);

            return new
            {
                PossuiIntegracaoBrasilRisk = configuracaoIntegracao?.PossuiIntegracaoBrasilRisk ?? false,
                SenhaBrasilRisk = configuracaoIntegracao?.SenhaBrasilRisk ?? string.Empty,
                UsuarioBrasilRisk = configuracaoIntegracao?.UsuarioBrasilRisk ?? string.Empty,
                URLHomologacaoBrasilRisk = configuracaoIntegracao?.URLHomologacaoBrasilRisk ?? string.Empty,
                URLProducaoBrasilRisk = configuracaoIntegracao?.URLProducaoBrasilRisk ?? string.Empty,
                URLBrasilRiskGestao = configuracaoIntegracao?.URLBrasilRiskGestao ?? string.Empty,
                URLBrasilRiskVeiculoMotorista = configuracaoIntegracao?.URLBrasilRiskVeiculoMotorista ?? string.Empty,
                CNPJEmbarcadorBrasilRisk = configuracaoIntegracao?.CNPJEmbarcadorBrasilRisk ?? string.Empty,
                ValorBaseBrasilRisk = configuracaoIntegracao?.ValorBaseBrasilRisk ?? 0,
                EnviarTodosDestinosBrasilRisk = configuracaoIntegracao?.EnviarTodosDestinosBrasilRisk ?? false,
                InicioViagemFixoHoraAtualMaisMinutos = configuracaoIntegracao?.InicioViagemFixoHoraAtualMaisMinutos ?? false,
                EnviarDadosTransportadoraSubContratadaNasObservacoes = configuracaoIntegracao?.EnviarDadosTransportadoraSubContratadaNasObservacoes ?? false,
                MinutosAMaisInicioViagem = configuracaoIntegracao?.MinutosAMaisInicioViagem ?? 0,
                BrasilRiskGerarParaCargasDeTransbordo = configuracaoTipoIntegracao?.IntegrarCargaTransbordo ?? false,
                IntegrarRotaBrasilRisk = configuracaoIntegracao?.IntegrarRotaBrasilRisk ?? false,
            };
        }

        private dynamic ObterIntegracaoMarisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMarisa repositorioConfiguracaoIntegracaMarisa = new Repositorio.Embarcador.Configuracoes.IntegracaoMarisa(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarisa configuracaoIntegracaoMarisa = repositorioConfiguracaoIntegracaMarisa.BuscarDadosIntegracao();

            return new
            {
                PossuiIntegracaoMarisa = configuracaoIntegracaoMarisa?.PossuiIntegracaoMarisa ?? false,
                UsuarioMarisa = configuracaoIntegracaoMarisa?.Usuario ?? string.Empty,
                SenhaMarisa = configuracaoIntegracaoMarisa?.Senha ?? string.Empty,
                UrlMarisa = configuracaoIntegracaoMarisa?.Url ?? string.Empty,
                EnderecoIntegracaoTabelaMarisa = configuracaoIntegracaoMarisa?.EnderecoIntegracaoTabelaMarisa ?? string.Empty,
                UsuarioIntegracaoTabelaMarisa = configuracaoIntegracaoMarisa?.UsuarioIntegracaoTabelaMarisa ?? string.Empty,
                SenhaIntegracaoTabelaMarisa = configuracaoIntegracaoMarisa?.SenhaIntegracaoTabelaMarisa ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoNstech(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoNSTech repositorioConfiguracaoIntegracaoNstech = new Repositorio.Embarcador.Configuracoes.IntegracaoNSTech(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech configuracaoIntegracaoNstech = repositorioConfiguracaoIntegracaoNstech.Buscar();

            return new
            {
                UrlAutenticacaoNstech = configuracaoIntegracaoNstech?.UrlAutenticacao ?? string.Empty,
                SenhaAutenticacaoNstech = configuracaoIntegracaoNstech?.SenhaAutenticacao ?? string.Empty,
                IDAutenticacaoNstech = configuracaoIntegracaoNstech?.IDAutenticacao ?? string.Empty,
                UrlIntegracaoSMNstech = configuracaoIntegracaoNstech?.UrlIntegracaoSM ?? string.Empty,
                UrlIntegracaoSolicitacaoCadastral = configuracaoIntegracaoNstech?.UrlIntegracaoSolicitacaoCadastral ?? string.Empty,
                UrlIntegracaoVerificacaoCadastral = configuracaoIntegracaoNstech?.UrlIntegracaoVerificacaoCadastral ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoDeca(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDeca repConfiguracaoIntegracaoDeca = new Repositorio.Embarcador.Configuracoes.IntegracaoDeca(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca configuracaoIntegracaoDeca = repConfiguracaoIntegracaoDeca.Buscar();

            return new
            {
                PossuiIntegracaoDeca = configuracaoIntegracaoDeca?.PossuiIntegracaoDeca ?? false,
                URLAutenticacaoDeca = configuracaoIntegracaoDeca?.URLAutenticacaoDeca ?? string.Empty,
                SenhaDeca = configuracaoIntegracaoDeca?.SenhaDeca ?? string.Empty,
                UsuarioDeca = configuracaoIntegracaoDeca?.UsuarioDeca ?? string.Empty,
                PossuiIntegracaoBalancaDeca = configuracaoIntegracaoDeca?.PossuiIntegracaoBalanca ?? false,
                URLBalancaDeca = configuracaoIntegracaoDeca?.URLBalanca ?? string.Empty,
                TokenBalancaDeca = configuracaoIntegracaoDeca?.TokenBalanca ?? string.Empty,
                URLInicioViagemDeca = configuracaoIntegracaoDeca?.URLInicioViagemDeca ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoMarilan(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMarilan repositorioIntegracaoMarilan = new Repositorio.Embarcador.Configuracoes.IntegracaoMarilan(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarilan integracaoMarilan = repositorioIntegracaoMarilan.BuscarPrimeiroRegistro();

            return new
            {
                PossuiIntegracaoMarilan = integracaoMarilan?.PossuiIntegracaoMarilan ?? false,
                URLMarilan = integracaoMarilan?.URLMarilan ?? string.Empty,
                URLMarilanChamadoOcorrencia = integracaoMarilan?.URLMarilanChamadoOcorrencia ?? string.Empty,
                SenhaMarilan = integracaoMarilan?.SenhaMarilan ?? string.Empty,
                UsuarioMarilan = integracaoMarilan?.UsuarioMarilan ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoVLI(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoVLI repConfiguracaoIntegracaoVLI = new Repositorio.Embarcador.Configuracoes.IntegracaoVLI(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI configuracaoIntegracaoVLI = repConfiguracaoIntegracaoVLI.Buscar();

            return new
            {
                PossuiIntegracaoVLI = configuracaoIntegracaoVLI?.PossuiIntegracaoVLI ?? false,
                SenhaAutenticacaoVLI = configuracaoIntegracaoVLI?.SenhaAutenticacao ?? string.Empty,
                IDAutenticacaoVLI = configuracaoIntegracaoVLI?.IDAutenticacao ?? string.Empty,
                UrlAutenticacaoVLI = configuracaoIntegracaoVLI?.UrlAutenticacao ?? string.Empty,
                UrlIntegracaoRastreamentoVLI = configuracaoIntegracaoVLI?.UrlIntegracaoRastreamento ?? string.Empty,
                UrlIntegracaoCarregamento = configuracaoIntegracaoVLI?.UrlIntegracaoCarregamento ?? string.Empty,
                UrlIntegracaoDescarregamentoPortosValeVLI = configuracaoIntegracaoVLI?.UrlIntegracaoDescarregamentoPortosValeVLI ?? string.Empty,
                UrlIntegracaoDescarregamentoVLI = configuracaoIntegracaoVLI?.UrlIntegracaoDescarregamento ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoCorreios(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoCorreios repositorioConfiguracaoIntegracaoCorreios = new Repositorio.Embarcador.Configuracoes.IntegracaoCorreios(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoIntegracaoCorreios = repositorioConfiguracaoIntegracaoCorreios.Buscar();

            return new
            {
                URLCorreios = configuracaoIntegracao?.URLCorreios ?? string.Empty,
                UsuarioCorreios = configuracaoIntegracao?.UsuarioCorreios ?? string.Empty,
                SenhaCorreios = configuracaoIntegracao?.SenhaCorreios ?? string.Empty,
                URLTokenCorreios = configuracaoIntegracaoCorreios?.URLToken ?? string.Empty,
                URLEventosCorreios = configuracaoIntegracaoCorreios?.URLEventos ?? string.Empty,
                CartaoPostagemCorreios = configuracaoIntegracaoCorreios?.CartaoPostagem ?? string.Empty,
                URLPLPCorreios = configuracaoIntegracaoCorreios?.URLPLP ?? string.Empty,
                UsuarioSIGEP = configuracaoIntegracaoCorreios?.UsuarioSIGEP ?? string.Empty,
                SenhaSIGEP = configuracaoIntegracaoCorreios?.SenhaSIGEP ?? string.Empty,
                NumeroContratoCorreios = configuracaoIntegracaoCorreios?.NumeroContrato ?? string.Empty,
                NumeroDiretoriaCorreios = configuracaoIntegracaoCorreios?.NumeroDiretoria ?? string.Empty,
                CodigoAdministrativoCorreios = configuracaoIntegracaoCorreios?.CodigoAdministrativo ?? string.Empty,
                CodigoServicoAdicionalCorreios = configuracaoIntegracaoCorreios?.CodigoServicoAdicional ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoArcelorMittal(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal repositorioConfiguracaoIntegracaoArcelorMittal = new Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal configuracaoIntegracaoArcelorMittal = repositorioConfiguracaoIntegracaoArcelorMittal.Buscar();

            return new
            {
                PossuiIntegracaoArcelorMittal = configuracaoIntegracaoArcelorMittal?.PossuiIntegracao ?? false,
                URLOcorrenciaArcelorMittal = configuracaoIntegracaoArcelorMittal?.URLOcorrencia ?? string.Empty,
                URLConfirmarAvancoTransporteArcelorMittal = configuracaoIntegracaoArcelorMittal?.URLConfirmarAvancoTransporte ?? string.Empty,
                UsuarioArcelorMittal = configuracaoIntegracaoArcelorMittal?.Usuario ?? string.Empty,
                SenhaArcelorMittal = configuracaoIntegracaoArcelorMittal?.Senha ?? string.Empty,
                URLDadosTransporteSAP = configuracaoIntegracaoArcelorMittal?.URLDadosTransporteSAP ?? string.Empty,
                URLAtualizarNFeAprovada = configuracaoIntegracaoArcelorMittal?.URLAtualizarNFeAprovada ?? string.Empty,
                URLRetornoAdicionarPedidoEmLote = configuracaoIntegracaoArcelorMittal?.URLRetornoAdicionarPedidoEmLote ?? string.Empty
            };
        }

        private dynamic ObterIntegracaoEMP(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();

            return new
            {
                PossuiIntegracaoEMP = configuracaoIntegracaoEMP?.PossuiIntegracaoEMP ?? false,
                BoostrapServersEMP = configuracaoIntegracaoEMP?.BoostrapServersEMP ?? string.Empty,
                GroupIdEMP = configuracaoIntegracaoEMP?.GroupIdEMP ?? string.Empty,
                UsuarioEMP = configuracaoIntegracaoEMP?.UsuarioEMP ?? string.Empty,
                SenhaEMP = configuracaoIntegracaoEMP?.SenhaEMP ?? string.Empty,

                AtivarEnvioCTesAnterioresEMP = configuracaoIntegracaoEMP?.AtivarEnvioCTesAnterioresEMP ?? false,
                TopicCTesAnterioresEMP = configuracaoIntegracaoEMP?.TopicCTesAnterioresEMP ?? string.Empty,
                EnviarTopicCTesAnterioresEMP = configuracaoIntegracaoEMP?.StatusTopicCTesAnterioresEMP == "A" ? true : false,
                TopicBuscarCTesEMP = configuracaoIntegracaoEMP?.TopicBuscarCTesEMP ?? string.Empty,
                EnviarTopicBuscarCTesEMP = configuracaoIntegracaoEMP?.StatusTopicBuscarCTesEMP == "A" ? true : false,
                TopicBuscarFaturaCTeEMP = configuracaoIntegracaoEMP?.TopicBuscarFaturaCTeEMP ?? string.Empty,
                EnviarTopicBuscarFaturaCTeEMP = configuracaoIntegracaoEMP?.StatusTopicBuscarFaturaCTeEMP == "A" ? true : false,

                TopicBuscarCargaEMP = configuracaoIntegracaoEMP?.TopicBuscarCargaEMP ?? string.Empty,
                EnviarTopicBuscarCargaEMP = configuracaoIntegracaoEMP?.StatusTopicBuscarCargaEMP == "A" ? true : false,
                AtivarIntegracaoCargaEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoCargaEMP ?? false,
                AtivarEnvioSerializaçãoEMP = configuracaoIntegracaoEMP?.AtivarEnvioSerializaçãoEMP ?? false,
                TopicEnvioIntegracaoCargaEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCargaEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoCargaEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCargaEMP == "A" ? true : false,
                TopicEnvioIntegracaoDadosCargaEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoDadosCargaEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoDadosCargaEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoDadosCargaEMP == "A" ? true : false,
                AtivarIntegracaoCancelamentoCargaEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoCancelamentoCargaEMP ?? false,
                TopicEnvioIntegracaoCancelamentoCargaEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCancelamentoCargaEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoCancelamentoCargaEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCancelamentoCargaEMP == "A" ? true : false,

                AtivarIntegracaoOcorrenciaEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoOcorrenciaEMP ?? false,
                TopicEnvioIntegracaoOcorrenciaEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoOcorrenciaEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoOcorrenciaEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoOcorrenciaEMP == "A" ? true : false,
                AtivarIntegracaoCancelamentoOcorrenciaEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoCancelamentoOcorrenciaEMP ?? false,
                TopicEnvioIntegracaoCancelamentoOcorrenciaEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCancelamentoOcorrenciaEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoCancelamentoOcorrenciaEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCancelamentoOcorrenciaEMP == "A" ? true : false,

                AtivarIntegracaoCTeManualEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoCTeManualEMP ?? false,
                TopicEnvioIntegracaoCTeManualEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCTeManualEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoCTeManualEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCTeManualEMP == "A" ? true : false,
                TopicEnvioIntegracaoCancelamentoCTeManualEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCancelamentoCTeManualEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoCancelamentoCTeManualEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCancelamentoCTeManualEMP == "A" ? true : false,

                AtivarIntegracaoFaturaEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoFaturaEMP ?? false,
                TopicEnvioIntegracaoFaturaEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoFaturaEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoFaturaEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoFaturaEMP == "A" ? true : false,
                AtivarIntegracaoCancelamentoFaturaEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoCancelamentoFaturaEMP ?? false,
                TopicEnvioIntegracaoCancelamentoFaturaEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCancelamentoFaturaEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoCancelamentoFaturaEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCancelamentoFaturaEMP == "A" ? true : false,

                AtivarIntegracaoCartaCorrecaoEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoCartaCorrecaoEMP ?? false,
                TopicEnvioIntegracaoCartaCorrecaoEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCartaCorrecaoEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoCartaCorrecaoEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCartaCorrecaoEMP == "A" ? true : false,

                AtivarIntegracaoComObjetoUnitoParaTodosTopics = configuracaoIntegracaoEMP?.AtivarIntegracaoComObjetoUnitoParaTodosTopics ?? false,

                AtivarIntegracaoBooking = configuracaoIntegracaoEMP?.AtivarIntegracaoBooking ?? false,
                AtivarLeituraHeaderBookingEMP = configuracaoIntegracaoEMP?.AtivarLeituraHeaderBooking ?? false,
                TopicBooking = configuracaoIntegracaoEMP?.TopicBooking ?? string.Empty,
                EnviarTopicRecebimentoIntegracaoBooking = configuracaoIntegracaoEMP?.StatusTopicRecebimentoIntegracaoBooking == "A" ? true : false,
                ConsumerGroupBookingEMP = configuracaoIntegracaoEMP?.ConsumerGroupBooking ?? string.Empty,

                AtivarIntegracaoContainerEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoContainerEMP ?? false,
                TopicEnvioIntegracaoContainerEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoContainerEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoContainerEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoContainerEMP == "A" ? true : false,

                AtivarIntegracaoNFTPEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoNFTPEMP ?? false,
                TopicEnvioIntegracaoNFTPEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoNFTPEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoNFTPEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoNFTPEMP == "A" ? true : false,
                ComponenteFreteValorNFTPEMP = new
                {
                    Codigo = configuracaoIntegracaoEMP?.ComponenteFreteNFTPEMP?.Codigo,
                    Descricao = configuracaoIntegracaoEMP?.ComponenteFreteNFTPEMP?.Descricao ?? string.Empty

                },
                ComponenteImpostosNFTPEMP = new
                {
                    Codigo = configuracaoIntegracaoEMP?.ComponenteImpostoNFTPEMP?.Codigo,
                    Descricao = configuracaoIntegracaoEMP?.ComponenteImpostoNFTPEMP?.Descricao ?? string.Empty
                },
                ComponenteValorTotalPrestacaoNFTPEMP = new
                {
                    Codigo = configuracaoIntegracaoEMP?.ComponenteValorTotalPrestacaoNFTPEMP?.Codigo,
                    Descricao = configuracaoIntegracaoEMP?.ComponenteValorTotalPrestacaoNFTPEMP?.Descricao ?? string.Empty
                },
                AtivarIntegracaoRecebimentoNavioEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoNavioEMP ?? false,
                AtivarLeituraHeaderVesselEMP = configuracaoIntegracaoEMP?.AtivarLeituraHeaderVessel ?? false,
                TopicRecebimentoIntegracaoVesselEMP = configuracaoIntegracaoEMP?.TopicRecebimentoIntegracaoVesselEMP ?? string.Empty,
                EnviarTopicRecebimentoIntegracaoVesselEMP = configuracaoIntegracaoEMP?.StatusTopicRecebimentoIntegracaoVesselEMP == "A" ? true : false,
                ConsumerGroupVesselEMP = configuracaoIntegracaoEMP?.ConsumerGroupVessel ?? string.Empty,

                AtivarIntegracaoRecebimentoPessoaEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoPessoaEMP ?? false,
                AtivarLeituraHeaderCustomerEMP = configuracaoIntegracaoEMP?.AtivarLeituraHeaderCustomer ?? false,
                TopicRecebimentoIntegracaoCustomerEMP = configuracaoIntegracaoEMP?.TopicRecebimentoIntegracaoCustomerEMP ?? string.Empty,
                EnviarTopicRecebimentoIntegracaoCustomerEMP = configuracaoIntegracaoEMP?.StatusTopicRecebimentoIntegracaoCustomerEMP == "A" ? true : false,
                ConsumerGroupCustomerEMP = configuracaoIntegracaoEMP?.ConsumerGroupCustomer ?? string.Empty,

                AtivarIntegracaoRecebimentoScheduleEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoScheduleEMP ?? false,
                AtivarLeituraHeaderScheduleEMP = configuracaoIntegracaoEMP?.AtivarLeituraHeaderSchedule ?? false,
                TopicRecebimentoIntegracaoScheduleEMP = configuracaoIntegracaoEMP?.TopicRecebimentoIntegracaoScheduleEMP ?? string.Empty,
                EnviarTopicRecebimentoIntegracaoScheduleEMP = configuracaoIntegracaoEMP?.StatusTopicRecebimentoIntegracaoScheduleEMP == "A" ? true : false,
                ConsumerGroupScheduleEMP = configuracaoIntegracaoEMP?.ConsumerGroupSchedule ?? string.Empty,

                AtivarIntegracaoRecebimentoRolagemEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoRolagemEMP ?? false,
                AtivarLeituraHeaderRolagemEMP = configuracaoIntegracaoEMP?.AtivarLeituraHeaderRolagem ?? false,
                TopicRecebimentoIntegracaoRolagemEMP = configuracaoIntegracaoEMP?.TopicRecebimentoIntegracaoRolagemEMP ?? string.Empty,
                EnviarTopicRecebimentoIntegracaoRolagemEMP = configuracaoIntegracaoEMP?.StatusTopicRecebimentoIntegracaoRolagemEMP == "A" ? true : false,
                ConsumerGroupRolagemEMP = configuracaoIntegracaoEMP?.ConsumerGroupRolagem ?? string.Empty,

                AtivarIntegracaoBoletoEMP = configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoBoletoEMP ?? false,
                TopicEnvioIntegracaoBoletoEMP = configuracaoIntegracaoEMP?.TopicIntegracaoBoletoEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoBoletoEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoBoletoEMP == "A" ? true : false,

                UrlSchemaRegistry = configuracaoIntegracaoEMP?.UrlSchemaRegistry ?? string.Empty,
                UsuarioSchemaRegistry = configuracaoIntegracaoEMP?.UsuarioSchemaRegistry ?? string.Empty,
                SenhaSchemaRegistry = configuracaoIntegracaoEMP?.SenhaSchemaRegistry ?? string.Empty,

                ModificarConexaoParaRetina = configuracaoIntegracaoEMP?.ModificarConexaoParaRetina ?? false,
                ModificarConexaoParaEnvioRetina = configuracaoIntegracaoEMP?.ModificarConexaoParaEnvioRetina ?? false,
                GroupIDRetina = configuracaoIntegracaoEMP?.GroupIDRetina ?? string.Empty,
                BootstrapServerRetina = configuracaoIntegracaoEMP?.BootstrapServerRetina ?? string.Empty,
                URLSchemaRegistryRetina = configuracaoIntegracaoEMP?.URLSchemaRegistryRetina ?? string.Empty,
                UsuarioServerRetina = configuracaoIntegracaoEMP?.UsuarioServerRetina ?? string.Empty,
                UsuarioSchemaRegistryRetina = configuracaoIntegracaoEMP?.UsuarioSchemaRegistryRetina ?? string.Empty,
                SenhaServerRetina = configuracaoIntegracaoEMP?.SenhaServerRetina ?? string.Empty,
                SenhaSchemaRegistryRetina = configuracaoIntegracaoEMP?.SenhaSchemaRegistryRetina ?? string.Empty,

                AtivarIntegracaoParaSILEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoParaSILEMP ?? false,
                TopicEnvioIntegracaoParaSILEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoParaSILEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoParaSILEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoParaSILEMP == "A" ? true : false,

                AtivarIntegracaoCEMercanteEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoCEMercanteEMP ?? false,
                TopicEnvioIntegracaoCEMercanteEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCEMercanteEMP ?? string.Empty,
                EnviarTopicEnvioIntegracaoCEMercanteEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicEnvioIntegracaoCEMercanteEMP == "A" ? true : false,

                AtivarLeituraHeadersConsumoKeyEMP = configuracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoKeyEMP ?? string.Empty,
                AtivarLeituraHeadersConsumoValueEMP = configuracaoIntegracaoEMP?.AtivarLeituraHeadersConsumoValueEMP ?? string.Empty,

                AtivarEnvioIntegracaoCTEDaCargaEMP = configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoCTEDaCargaEMP ?? false,
                TopicIntegracaoCTEDaCargaEMP = configuracaoIntegracaoEMP?.TopicIntegracaoCTEDaCargaEMP ?? string.Empty,
                EnviarTopicIntegracaoCTEDaCargaEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoCTEDaCargaEMP == "A" ? true : false,
                AtivarIntegracaoCancelamentoDaCargaEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoCancelamentoDaCargaEMP ?? false,
                TopicIntegracaoCancelamentoDaCargaEMP = configuracaoIntegracaoEMP?.TopicIntegracaoCancelamentoDaCargaEMP ?? string.Empty,
                EnviarTopicIntegracaoCancelamentoDaCargaEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoCancelamentoDaCargaEMP == "A" ? true : false,
                AtivarEnvioIntegracaoCTEManualEMP = configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoCTEManualEMP ?? false,
                TopicIntegracaoCTEManualEMP = configuracaoIntegracaoEMP?.TopicIntegracaoCTEManualEMP ?? string.Empty,
                EnviarTopicIntegracaoCTEManualEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoCTEManualEMP == "A" ? true : false,
                AtivarEnvioIntegracaoCancelamentoCTEManualEMP = configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoCancelamentoCTEManualEMP ?? false,
                TopicIntegracaoCancelamentoCTEManualEMP = configuracaoIntegracaoEMP?.TopicIntegracaoCancelamentoCTEManualEMP ?? string.Empty,
                EnviarTopicIntegracaoCancelamentoCTEManualEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoCancelamentoCTEManualEMP == "A" ? true : false,
                AtivarEnvioIntegracaoOcorrenciaEMP = configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoOcorrenciaEMP ?? false,
                TopicIntegracaoOcorrenciaEMP = configuracaoIntegracaoEMP?.TopicIntegracaoOcorrenciaEMP ?? string.Empty,
                EnviarTopicIntegracaoOcorrenciaEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoOcorrenciaEMP == "A" ? true : false,
                AtivarEnvioIntegracaoCancelamentoOcorrenciaEMP = configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoCancelamentoOcorrenciaEMP ?? false,
                TopicIntegracaoCancelamentoOcorrenciaEMP = configuracaoIntegracaoEMP?.TopicIntegracaoCancelamentoOcorrenciaEMP ?? string.Empty,
                EnviarTopicIntegracaoCancelamentoOcorrenciaEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoCancelamentoOcorrenciaEMP == "A" ? true : false,
                AtivarEnvioIntegracaoFaturaEMP = configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoFaturaEMP ?? false,
                TopicIntegracaoFaturaEMP = configuracaoIntegracaoEMP?.TopicIntegracaoFaturaEMP ?? string.Empty,
                EnviarTopicIntegracaoFaturaEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoFaturaEMP == "A" ? true : false,
                AtivarEnvioIntegracaoCancelamentoFaturaEMP = configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoCancelamentoFaturaEMP ?? false,
                TopicIntegracaoCancelamentoFaturaEMP = configuracaoIntegracaoEMP?.TopicIntegracaoCancelamentoFaturaEMP ?? string.Empty,
                EnviarTopicIntegracaoCancelamentoFaturaEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoCancelamentoFaturaEMP == "A" ? true : false,
                AtivarEnvioIntegracaoCartaCorrecaoEMP = configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoCartaCorrecaoEMP ?? false,
                TopicIntegracaoCartaCorrecaoEMP = configuracaoIntegracaoEMP?.TopicIntegracaoCartaCorrecaoEMP ?? string.Empty,
                EnviarTopicIntegracaoCartaCorrecaoEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoCartaCorrecaoEMP == "A" ? true : false,
                AtivarEnvioIntegracaoBoletoEMP = configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoBoletoEMP ?? false,
                TopicIntegracaoBoletoEMP = configuracaoIntegracaoEMP?.TopicIntegracaoBoletoEMP ?? string.Empty,
                EnviarTopicIntegracaoBoletoEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoBoletoEMP == "A" ? true : false,
                AtivarEnvioIntegracaoCancelamentoBoletoEMP = configuracaoIntegracaoEMP?.AtivarEnvioIntegracaoCancelamentoBoletoEMP ?? false,
                TopicIntegracaoCancelamentoBoletoEMP = configuracaoIntegracaoEMP?.TopicIntegracaoCancelamentoBoletoEMP ?? string.Empty,
                EnviarTopicIntegracaoCancelamentoBoletoEMP = configuracaoIntegracaoEMP?.StatusEnviarTopicIntegracaoCancelamentoBoletoEMP == "A" ? true : false,
                AtivarIntegracaoCancelamentoBoletoEMP = configuracaoIntegracaoEMP?.AtivarIntegracaoCancelamentoBoletoEMP ?? false,
                TopicEnvioIntegracaoCancelamentoBoletoEMP = configuracaoIntegracaoEMP?.TopicEnvioIntegracaoCancelamentoBoletoEMP ?? string.Empty,
                EnviarTopicEnvioDoCancelamentoBoletoEMP = configuracaoIntegracaoEMP?.StatusTopicEnvioIntegracaoCancelamentoBoletoEMP == "A" ? true : false,


                EnviarNoLayoutAvroDoPortalEMP = configuracaoIntegracaoEMP?.EnviarNoLayoutAvroDoPortalEMP ?? false,

                TipoAVRO = configuracaoIntegracaoEMP?.TipoAVRO ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAVRO.V1,

                TipoCargaPadraoEMP = new
                {
                    Codigo = configuracaoIntegracaoEMP?.TipoDeCarga?.Codigo,
                    Descricao = configuracaoIntegracaoEMP?.TipoDeCarga?.Descricao ?? string.Empty
                },
            };
        }

        private dynamic ObterIntegracaoTicketLog(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTicketLog repConfiguracaoIntegracaoTicketLog = new Repositorio.Embarcador.Configuracoes.IntegracaoTicketLog(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTicketLog configuracaoIntegracaoTicketLog = repConfiguracaoIntegracaoTicketLog.Buscar();

            return new
            {
                PossuiIntegracaoTicketLog = configuracaoIntegracaoTicketLog?.PossuiIntegracaoTicketLog ?? false,
                URLTicketLog = configuracaoIntegracaoTicketLog?.URLTicketLog ?? string.Empty,
                UsuarioTicketLog = configuracaoIntegracaoTicketLog?.UsuarioTicketLog ?? string.Empty,
                SenhaTicketLog = configuracaoIntegracaoTicketLog?.SenhaTicketLog ?? string.Empty,
                CodigoClienteTicketLog = configuracaoIntegracaoTicketLog?.CodigoClienteTicketLog ?? string.Empty,
                ChaveAutorizacaoTicketLog = configuracaoIntegracaoTicketLog?.ChaveAutorizacaoTicketLog ?? string.Empty,
                HorasConsultaTicketLog = configuracaoIntegracaoTicketLog?.HorasConsultaTicketLog ?? string.Empty,
                ConfiguracaoAbastecimentoTicketLog = new
                {
                    Codigo = configuracaoIntegracaoTicketLog?.ConfiguracaoAbastecimentoTicketLog?.Codigo ?? 0,
                    Descricao = configuracaoIntegracaoTicketLog?.ConfiguracaoAbastecimentoTicketLog?.Descricao ?? string.Empty
                },
            };
        }

        private dynamic ObterIntegracaoEmillenium(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoEmilenium repositorioConfiguracaoIntegracaoEmilenium = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEmilenium(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmilenium configuracaoIntegracaoEmillenium = repositorioConfiguracaoIntegracaoEmilenium.BuscarConfiguracaoPadrao();

            return new
            {
                // T_CONFIGURACAO_INTEGRACAO
                URLEmillenium = configuracaoIntegracao?.URLEmillenium ?? string.Empty,
                URLEmilleniumConfirmarEntrega = configuracaoIntegracao?.URLEmilleniumConfirmarEntrega ?? string.Empty,
                SenhaFrontDoor = configuracaoIntegracao?.SenhaFrontDoor ?? string.Empty,
                UsuarioEmillenium = configuracaoIntegracao?.UsuarioEmillenium ?? string.Empty,
                SenhaEmillenium = configuracaoIntegracao?.SenhaEmillenium ?? string.Empty,
                TransIdAtualEmillenium = configuracaoIntegracao?.TransIdAtualEmillenium ?? 0,
                // T_CONFIGURACAO_EMILENIUM
                TransIdInicioBuscaMassiva = configuracaoIntegracaoEmillenium?.TransIdInicioBuscaMassiva ?? 0,
                TransIdFimBuscaMassiva = configuracaoIntegracaoEmillenium?.TransIdFimBuscaMassiva ?? 0,
                DataFinalizacaoBuscaTransIdMassivo = configuracaoIntegracaoEmillenium != null && configuracaoIntegracaoEmillenium.DataFinalizacaoBuscaMassiva.HasValue ? configuracaoIntegracaoEmillenium.DataFinalizacaoBuscaMassiva.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
            };
        }

        private dynamic ObterIntegracaoGNRE(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoGNRE repoConfigIntGNRE = new Repositorio.Embarcador.Configuracoes.IntegracaoGNRE(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGNRE configIntGNRE = repoConfigIntGNRE.Buscar();

            return new
            {
                PossuiIntegracaoGNRE = configIntGNRE?.PossuiIntegracaoGNRE ?? false,
                UsuarioIntegracaoGNRE = configIntGNRE?.UsuarioIntegracaoGNRE ?? "",
                SenhaIntegracaoGNRE = configIntGNRE?.SenhaIntegracaoGNRE ?? "",
                URLIntegracaoGNRE = configIntGNRE?.URLIntegracaoGNRE ?? ""
            };
        }

        private dynamic ObterIntegracaoDigitalCom(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom repositorioIntegracaoDigitalCom = new Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom integracaoDigitalCom = repositorioIntegracaoDigitalCom.Buscar();

            return new
            {
                ValidacaoTAGDigitalCom = integracaoDigitalCom?.ValidacaoTAGDigitalCom ?? false,
                EndpointDigitalCom = integracaoDigitalCom?.EndpointDigitalCom ?? string.Empty,
                TokenDigitalCom = integracaoDigitalCom?.TokenDigitalCom ?? string.Empty,
                CNPJLogin = integracaoDigitalCom?.CNPJLogin ?? string.Empty,
                UsuarioDigitalCom = integracaoDigitalCom?.UsuarioDigitalCom ?? string.Empty,
                SenhaDigitalCom = integracaoDigitalCom?.SenhaDigitalCom ?? string.Empty,
                UrlAutenticacaoDigitalCom = integracaoDigitalCom?.UrlAutenticacaoDigitalCom ?? string.Empty,
                TipoObtencaoCNPJTransportadora = integracaoDigitalCom?.TipoObtencaoCNPJTransportadora ?? 0
            };
        }

        private dynamic ObterIntegracaoLBC(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoLBC repositorioIntegracaoLBC = new Repositorio.Embarcador.Configuracoes.IntegracaoLBC(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC IntegracaoLBC = repositorioIntegracaoLBC.Buscar();

            return new
            {
                PossuiIntegracaoLBC = IntegracaoLBC?.PossuiIntegracaoLBC ?? false,
                URLIntegracaoLBC = IntegracaoLBC?.URLIntegracaoLBC ?? string.Empty,
                URLIntegracaoLBCAnexo = IntegracaoLBC?.URLIntegracaoLBCAnexo ?? string.Empty,
                URLIntegracaoLBCCustoFixo = IntegracaoLBC?.URLIntegracaoLBCCustoFixo ?? string.Empty,
                URLIntegracaoLBCTabelaFreteCliente = IntegracaoLBC?.URLIntegracaoLBCTabelaFreteCliente ?? string.Empty,
                UsuarioLBC = IntegracaoLBC?.UsuarioLBC ?? string.Empty,
                SenhaLBC = IntegracaoLBC?.SenhaLBC ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoTecnorisk(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTecnorisk repositorioIntegracaoTecnorisk = new Repositorio.Embarcador.Configuracoes.IntegracaoTecnorisk(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTecnorisk integracaoTecnorisk = repositorioIntegracaoTecnorisk.Buscar();

            return new
            {
                PossuiIntegracaoTecnorisk = integracaoTecnorisk?.PossuiIntegracaoTecnorisk ?? false,
                URLIntegracaoTecnorisk = integracaoTecnorisk?.URLIntegracaoTecnorisk ?? string.Empty,
                IDPGR = integracaoTecnorisk?.IDPGR ?? 0,
                IDPropriedadeMonitoramento = integracaoTecnorisk?.IDPropriedadeMonitoramento ?? 0,
                UsuarioTecnorisk = integracaoTecnorisk?.UsuarioTecnorisk ?? string.Empty,
                SenhaTecnorisk = integracaoTecnorisk?.SenhaTecnorisk ?? string.Empty,
                CargaMercadoria = integracaoTecnorisk?.CargaMercadoria ?? 0,
            };
        }

        private dynamic ObterIntegracaoDestinadosSAP(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDestinadosSAP repositorioIntegracaoDestinadosSAP = new Repositorio.Embarcador.Configuracoes.IntegracaoDestinadosSAP(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP integracaoDestinadosSAP = repositorioIntegracaoDestinadosSAP.Buscar();

            return new
            {
                PossuiIntegracaoDestinadosSAP = integracaoDestinadosSAP?.PossuiIntegracao ?? false,
                ClientIDIntegracaoDestinadosSAP = integracaoDestinadosSAP?.ClientIDIntegracao ?? string.Empty,
                ClientSecretIntegracaoDestinadosSAP = integracaoDestinadosSAP?.ClientSecretIntegracao ?? string.Empty,
                URLIntegracaoXMLDestinadosSAP = integracaoDestinadosSAP?.URLIntegracaoXML ?? string.Empty,
                URLIntegracaoStatusDestinadosSAP = integracaoDestinadosSAP?.URLIntegracaoStatus ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoNeokohm(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoNeokohm repositorioIntegracaoNeokohm = new Repositorio.Embarcador.Configuracoes.IntegracaoNeokohm(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm integracaoNeokohm = repositorioIntegracaoNeokohm.Buscar();

            return new
            {
                PossuiIntegracaoNeokohm = integracaoNeokohm?.PossuiIntegracaoNeokohm ?? false,
                URLIntegracaoNeokohm = integracaoNeokohm?.URLIntegracaoNeokohm ?? string.Empty,
                TokenNeokohm = integracaoNeokohm?.TokenNeokohm ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoMoniloc(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMoniloc repositorioIntegracaoMoniloc = new Repositorio.Embarcador.Configuracoes.IntegracaoMoniloc(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMoniloc integracaoMoniloc = repositorioIntegracaoMoniloc.Buscar();

            return new
            {
                PossuiIntegracaoMoniloc = integracaoMoniloc?.PossuiIntegracaoMoniloc ?? false,
                UsuarioFTPMoniloc = integracaoMoniloc?.UsuarioFTP ?? string.Empty,
                SenhaFTPMoniloc = integracaoMoniloc?.SenhaFTP ?? string.Empty,
                PortaFTPMoniloc = integracaoMoniloc?.PortaFTP ?? string.Empty,
                DiretorioConsumoCargasDiariasMoniloc = integracaoMoniloc?.DiretorioConsumoCargasDiarias ?? string.Empty,
                DiretorioConsumoMoniloc = integracaoMoniloc?.DiretorioConsumo ?? string.Empty,
                DiretorioEnvioCVAMoniloc = integracaoMoniloc?.DiretorioEnvioCVA ?? string.Empty,
                DiretorioRetornoCVAMoniloc = integracaoMoniloc?.DiretorioRetornoCVA ?? string.Empty,
                FTPPassivoMoniloc = integracaoMoniloc?.FTPPassivo ?? false,
                SFPTMoniloc = integracaoMoniloc?.SFTP ?? false,
                SSLMoniloc = integracaoMoniloc?.SSL ?? false,
                HostFTPMoniloc = integracaoMoniloc?.HostFTP ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoApisulLogAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ApisulLog))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoApisulLog repositorioIntegracaoApisulLog = new Repositorio.Embarcador.Configuracoes.IntegracaoApisulLog(unitOfWork, cancellationToken);
            IntegracaoApisulLog integracaoApisul = await repositorioIntegracaoApisulLog.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoApisulLog = integracaoApisul?.PossuiIntegracaoApisulLog ?? false,
                URLIntegracaoApisulLog = integracaoApisul?.URLIntegracaoApisulLog ?? string.Empty,
                URLIntegracaoApisulLogEventosApisulLog = integracaoApisul?.URLIntegracaoApisulLogEventos ?? string.Empty,
                TokenApisulLog = integracaoApisul?.Token ?? string.Empty,
                CNPJEmbarcadorApisulLog = integracaoApisul?.CNPJEmbarcador ?? string.Empty,
                NaoUtilizarRastreadoresApisulLog = integracaoApisul?.NaoUtilizarRastreadores ?? false,
                EtapaCarga = integracaoApisul?.EtapaCarga,
                EnviarCodigoIntegracaoAbaCodigosIntegracaoTipoDeCargaApisulLog = integracaoApisul?.EnviarCodigoIntegracaoAbaCodigosIntegracaoTipoDeCarga ?? false,
                ConcatenarCodigoIntegracaoDoClienteCidadeEstadoApisulLog = integracaoApisul?.ConcatenarCodigoIntegracaoDoClienteCidadeEstado ?? false,
                ConcatenarCodigoIntegracaoTransporteOridemEDestinoApisulLog = integracaoApisul?.ConcatenarCodigoIntegracaoTransporteOridemEDestino ?? false,
                ValorCargaOrigemApisulLog = integracaoApisul?.ValorCargaOrigem ?? 0,
                OrigemDataInicioViagem = integracaoApisul?.OrigemDataInicioViagem,
                TipoCargaApisulLog = integracaoApisul?.TipoCarga,
                IdentificadorUnicoViagemApisulLog = integracaoApisul?.IdentificadorUnicoViagem ?? null
            };
        }

        private dynamic ObterIntegracaoFroggr(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoFroggr repIntegracaoFroggr = new Repositorio.Embarcador.Configuracoes.IntegracaoFroggr(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFroggr IntegracaoFroggr = repIntegracaoFroggr.BuscarPrimeiroRegistro();

            return new
            {
                Codigo = IntegracaoFroggr?.Codigo ?? 0,
                PossuiIntegracaoFroggr = IntegracaoFroggr?.PossuiIntegracaoFroggr ?? false,
                URLIntegracaoFroggr = IntegracaoFroggr?.URLIntegracaoFroggr ?? string.Empty,
                UsuarioIntegracaoFroggr = IntegracaoFroggr?.UsuarioIntegracaoFroggr ?? string.Empty,
                SenhaIntegracaoFroggr = IntegracaoFroggr?.SenhaIntegracaoFroggr ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoSAP(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSAP repositorioIntegracaoSAP = new Repositorio.Embarcador.Configuracoes.IntegracaoSAP(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP integracaoSAP = repositorioIntegracaoSAP.Buscar();

            return new
            {
                PossuiIntegracaoSAP = integracaoSAP?.PossuiIntegracao ?? false,
                URLSAP = integracaoSAP?.URL ?? string.Empty,
                URLEnviaVendaFrete = integracaoSAP?.URLEnviaVendaFrete ?? string.Empty,
                URLDescontoAvaria = integracaoSAP?.URLDescontoAvaria ?? string.Empty,
                URLSolicitacaoCancelamento = integracaoSAP?.URLSolicitacaoCancelamento ?? string.Empty,
                URLSolicitacaoCancelamentoCTe = integracaoSAP?.URLSolicitacaoCancelamentoCTe ?? string.Empty,
                UsuarioSAP = integracaoSAP?.Usuario ?? string.Empty,
                SenhaSAP = integracaoSAP?.Senha ?? string.Empty,
                RealizarIntegracaoComDadosFatura = integracaoSAP?.RealizarIntegracaoComDadosFatura ?? false,
                URLIntegracaoFatura = integracaoSAP?.URLIntegracaoFatura ?? string.Empty,
                URLCriarSaldoFrete = integracaoSAP?.URLCriarSaldoFrete ?? string.Empty,
                URLConsultaDocumentos = integracaoSAP?.URLConsultaDocumentos ?? string.Empty,
                URLConsultaFatura = integracaoSAP?.URLConsultaFatura ?? string.Empty,
                URLIntegracaoEstornoFatura = integracaoSAP?.URLIntegracaoEstornoFatura ?? string.Empty,
                URLConsultaEstornoFatura = integracaoSAP?.URLConsultaEstornoFatura ?? string.Empty,
                URLEnviaVendaServicoNFSe = integracaoSAP?.URLEnviaVendaServicoNFSe ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoYPE(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoYPE repositorioIntegracaoYPE = new Repositorio.Embarcador.Configuracoes.IntegracaoYPE(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYPE integracaoYPE = repositorioIntegracaoYPE.BuscarPrimeiroRegistro();

            return new
            {
                PossuiIntegracaoYPE = integracaoYPE?.PossuiIntegracao ?? false,
                UrlintegracaoYpe = integracaoYPE?.URLintegracao ?? string.Empty,
                UsuarioYPE = integracaoYPE?.Usuario ?? string.Empty,
                SenhaYPE = integracaoYPE?.Senha ?? string.Empty,
                URLIntegracaoOcorrencia = integracaoYPE?.URLIntegracaoOcorrencia ?? string.Empty,
                URLintegracaoRecebeDadosLaudo = integracaoYPE?.URLintegracaoRecebeDadosLaudo ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoOTM(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoOTM repositorioIntegracaoOTM = new Repositorio.Embarcador.Configuracoes.IntegracaoOTM(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOTM integracaoOTM = repositorioIntegracaoOTM.BuscarPrimeiroRegistro();

            return new
            {
                PossuiIntegracaoOTM = integracaoOTM?.PossuiIntegracaoOTM ?? false,
                ClientIDOTM = integracaoOTM?.ClientIDOTM ?? string.Empty,
                ClientSecretOTM = integracaoOTM?.ClientSecretOTM ?? string.Empty,
                URLIntegracaoLeilaoOTM = integracaoOTM?.URLIntegracaoLeilaoOTM ?? string.Empty,
            };
        }

        private dynamic ObterIntegracaoSIC(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSIC repositorioIntegracaoSIC = new Repositorio.Embarcador.Configuracoes.IntegracaoSIC(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC integracaoSIC = repositorioIntegracaoSIC.BuscarPrimeiroRegistro();

            return new
            {
                PossuiIntegracaoSIC = integracaoSIC?.PossuiIntegracaoSIC ?? false,
                RealizarIntegracaoNovosCadastrosPessoaSIC = integracaoSIC?.RealizarIntegracaoNovosCadastrosPessoaSIC ?? false,
                URLIntegracaoSIC = integracaoSIC?.URLIntegracaoSIC ?? string.Empty,
                LoginSIC = integracaoSIC?.LoginSIC ?? string.Empty,
                SenhaSIC = integracaoSIC?.SenhaSIC ?? string.Empty,
                TipoCadastroVeiculoSIC = integracaoSIC?.TipoCadastroVeiculoSIC ?? string.Empty,
                TipoCadastroMotoristaSIC = integracaoSIC?.TipoCadastroMotoristaSIC ?? string.Empty,
                TipoCadastroClientesSIC = integracaoSIC?.TipoCadastroClientesSIC ?? string.Empty,
                TipoCadastroTransportadoresTerceirosSIC = integracaoSIC?.TipoCadastroTransportadoresTerceirosSIC ?? string.Empty,
                EmpresaSIC = integracaoSIC?.EmpresaSIC ?? string.Empty,
                TipoCadastroClientesTerceirosSIC = integracaoSIC?.TipoCadastroClientesTerceirosSIC ?? false,
            };
        }

        private dynamic ObterIntegracaoFrimesa(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa repConfiguracaoIntegracaoFrimesa = new Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa configuracaoIntegracaoFrimesa = repConfiguracaoIntegracaoFrimesa.Buscar();

            return new
            {
                URLContabilizacaoFrimesa = configuracaoIntegracaoFrimesa?.URLContabilizacao ?? string.Empty,
                UsuarioFrimesa = configuracaoIntegracaoFrimesa?.Usuario ?? string.Empty,
                SenhaFrimesa = configuracaoIntegracaoFrimesa?.Senha ?? string.Empty,
                ClientID = configuracaoIntegracaoFrimesa?.ClientID ?? string.Empty,
                ClientSecret = configuracaoIntegracaoFrimesa?.ClientSecret ?? string.Empty,
                AccessToken = configuracaoIntegracaoFrimesa?.AccessToken ?? string.Empty,
                Scope = configuracaoIntegracaoFrimesa?.Scope ?? string.Empty,
                Situacao = configuracaoIntegracaoFrimesa?.Situacao ?? false,
                TipoIntegracaoOAuth = configuracaoIntegracaoFrimesa?.TipoIntegracaoOAuth ?? TipoIntegracaoOAuth.OAuth1_0
            };
        }

        private async Task<dynamic> ObterIntegracaoLoggiAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Loggi))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoLoggi repConfiguracaoIntegracaoLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoLoggi(unidadeDeTrabalho, cancellationToken);
            IntegracaoLoggi configuracaoIntegracaoLoggi = await repConfiguracaoIntegracaoLoggi.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoLoggi = configuracaoIntegracaoLoggi?.PossuiIntegracao ?? false,
                URLIntegracaoLoggi = configuracaoIntegracaoLoggi?.URLIntegracao ?? string.Empty,
                UrlIntegracaoCTeLoggi = configuracaoIntegracaoLoggi?.UrlIntegracaoCTe ?? string.Empty,
                ClientIDLoggi = configuracaoIntegracaoLoggi?.ClientID ?? string.Empty,
                ClientSecretLoggi = configuracaoIntegracaoLoggi?.ClientSecret ?? string.Empty,
                TokenLoggi = configuracaoIntegracaoLoggi?.Token ?? string.Empty,
                ScopeLoggi = configuracaoIntegracaoLoggi?.Scope ?? string.Empty,
                URLConsultaPacotes = configuracaoIntegracaoLoggi?.URLConsultaPacotes ?? string.Empty,
                URLAutenticacaoEventoEntrega = configuracaoIntegracaoLoggi?.URLAutenticacaoEventoEntrega ?? string.Empty,
                URLIntegracaoEventoEntrega = configuracaoIntegracaoLoggi?.URLIntegracaoEventoEntrega ?? string.Empty,
            };
        }

        private async Task<dynamic> ObterIntegracaoCTePagamentoLoggiAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.CTePagamentoLoggi))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi repConfiguracaoIntegracaoCTePagamentoLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi(unidadeDeTrabalho, cancellationToken);
            IntegracaoCTePagamentoLoggi configuracaoIntegracaoCTePagamentoLoggi = await repConfiguracaoIntegracaoCTePagamentoLoggi.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoCTePagamentoLoggi = configuracaoIntegracaoCTePagamentoLoggi?.PossuiIntegracao ?? false,
                URLAutenticacaoCTePagamentoLoggi = configuracaoIntegracaoCTePagamentoLoggi?.URLAutenticacao ?? string.Empty,
                ClientIDCTePagamentoLoggi = configuracaoIntegracaoCTePagamentoLoggi?.ClientID ?? string.Empty,
                ClientSecretCTePagamentoLoggi = configuracaoIntegracaoCTePagamentoLoggi?.ClientSecret ?? string.Empty,
                TokenCTePagamentoLoggi = configuracaoIntegracaoCTePagamentoLoggi?.Token ?? string.Empty,
                ScopeCTePagamentoLoggi = configuracaoIntegracaoCTePagamentoLoggi?.Scope ?? string.Empty,
                URLEnvioDocumentosCTePagamentoLoggi = configuracaoIntegracaoCTePagamentoLoggi?.URLEnvioDocumentos ?? string.Empty,
                IntegrarCTeSubstitutoCTePagamentoLoggi = configuracaoIntegracaoCTePagamentoLoggi?.IntegrarCTeSubstituto ?? false,
            };
        }

        private async Task<dynamic> ObterIntegracaoValoresCTeLoggiAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ValoresCTeLoggi))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi repIntegracaoValoresCTeLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi(unidadeDeTrabalho, cancellationToken);
            IntegracaoValoresCTeLoggi configuracaoIntegracaoValoresCTeLoggi = await repIntegracaoValoresCTeLoggi.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoValoresCTeLoggi = configuracaoIntegracaoValoresCTeLoggi?.PossuiIntegracao ?? false,
                URLAutenticacaoValoresCTeLoggi = configuracaoIntegracaoValoresCTeLoggi?.URLAutenticacao ?? string.Empty,
                ClientIDValoresCTeLoggi = configuracaoIntegracaoValoresCTeLoggi?.ClientID ?? string.Empty,
                ClientSecretValoresCTeLoggi = configuracaoIntegracaoValoresCTeLoggi?.ClientSecret ?? string.Empty,
                TokenValoresCTeLoggi = configuracaoIntegracaoValoresCTeLoggi?.Token ?? string.Empty,
                ScopeValoresCTeLoggi = configuracaoIntegracaoValoresCTeLoggi?.Scope ?? string.Empty,
                URLEnvioDocumentosValoresCTeLoggi = configuracaoIntegracaoValoresCTeLoggi?.URLEnvioDocumentos ?? string.Empty,
            };

        }

        private async Task<object> ObterIntegracaoCTeAnterioresLoggiAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.CTeAnterioresLoggi))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi repositorioIntegracaoCTeAnterioresLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi(unidadeDeTrabalho, cancellationToken);
            IntegracaoCTeAnterioresLoggi configuracaoIntegracaoCTeAnterioresLoggi = await repositorioIntegracaoCTeAnterioresLoggi.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoCTeAnterioresLoggi = configuracaoIntegracaoCTeAnterioresLoggi?.PossuiIntegracao ?? false,
                URLAutenticacaoCTeAnterioresLoggi = configuracaoIntegracaoCTeAnterioresLoggi?.URLAutenticacao ?? string.Empty,
                ClientIDCTeAnterioresLoggi = configuracaoIntegracaoCTeAnterioresLoggi?.ClientID ?? string.Empty,
                ClientSecretCTeAnterioresLoggi = configuracaoIntegracaoCTeAnterioresLoggi?.ClientSecret ?? string.Empty,
                ScopeCTeAnterioresLoggi = configuracaoIntegracaoCTeAnterioresLoggi?.Scope ?? string.Empty,
                URLEnvioDocumentosCTeAnterioresLoggi = configuracaoIntegracaoCTeAnterioresLoggi?.URLEnvioDocumentos ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoCamilAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Camil))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoCamil repositorioIntegracaoCamil = new Repositorio.Embarcador.Configuracoes.IntegracaoCamil(unidadeDeTrabalho, cancellationToken);
            IntegracaoCamil configuracaoIntegracaoCamil = await repositorioIntegracaoCamil.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoCamil = configuracaoIntegracaoCamil?.PossuiIntegracao ?? false,
                URLIntegracaoCamil = configuracaoIntegracaoCamil?.URL ?? string.Empty,
                ApiKeyCamil = configuracaoIntegracaoCamil?.ApiKey ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoCebraceAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Cebrace))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoCebrace repositorioIntegracaoCebrace = new Repositorio.Embarcador.Configuracoes.IntegracaoCebrace(unidadeDeTrabalho, cancellationToken);
            IntegracaoCebrace configuracaoIntegracaoCebrace = await repositorioIntegracaoCebrace.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoCebrace = configuracaoIntegracaoCebrace?.PossuiIntegracao ?? false,
                URLIntegracaoCebrace = configuracaoIntegracaoCebrace?.URLIntegracao ?? string.Empty,
                URLAutenticacaoCebrace = configuracaoIntegracaoCebrace?.URLAutenticacao ?? string.Empty,
                APIKeyIntegracaoCebrace = configuracaoIntegracaoCebrace?.ApiKey ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoJJAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.JJ))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoJJ repConfiguracaoIntegracaoJJ = new Repositorio.Embarcador.Configuracoes.IntegracaoJJ(unidadeDeTrabalho, cancellationToken);
            IntegracaoJJ configuracaoIntegracaoJJ = await repConfiguracaoIntegracaoJJ.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoJJ = configuracaoIntegracaoJJ?.PossuiIntegracao ?? false,
                URLIntegracaoAtendimentoJJ = configuracaoIntegracaoJJ?.URLIntegracaoAtendimento ?? string.Empty,
                UsuarioJJ = configuracaoIntegracaoJJ?.Usuario ?? string.Empty,
                SenhaJJ = configuracaoIntegracaoJJ?.Senha ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoKliosAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Klios))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoKlios repConfiguracaoIntegracaoKlios = new Repositorio.Embarcador.Configuracoes.IntegracaoKlios(unidadeDeTrabalho, cancellationToken);
            IntegracaoKlios configuracaoIntegracaoKlios = await repConfiguracaoIntegracaoKlios.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoKlios = configuracaoIntegracaoKlios?.PossuiIntegracao ?? false,
                URLAutenticacaoKlios = configuracaoIntegracaoKlios?.URLAutenticacao ?? string.Empty,
                UsuarioKlios = configuracaoIntegracaoKlios?.Usuario ?? string.Empty,
                SenhaKlios = configuracaoIntegracaoKlios?.Senha ?? string.Empty,
                URLConsultaAnaliseConjuntoKlios = configuracaoIntegracaoKlios?.URLConsultaAnaliseConjunto ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoSAPV9Async(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SAP_V9))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9 repConfiguracaoIntegracaoSAPV9 = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9(unidadeDeTrabalho, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPV9 configuracaoIntegracaoSAPV9 = await repConfiguracaoIntegracaoSAPV9.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoSAPV9 = configuracaoIntegracaoSAPV9?.PossuiIntegracaoSAPV9 ?? false,
                URLReciboFrete = configuracaoIntegracaoSAPV9?.URLReciboFrete ?? string.Empty,
                Usuario = configuracaoIntegracaoSAPV9?.Usuario ?? string.Empty,
                Senha = configuracaoIntegracaoSAPV9?.Senha ?? string.Empty,
                URLCancelamento = configuracaoIntegracaoSAPV9?.URLCancelamento ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoSAPSTAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SAP_ST))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoSAPST repConfiguracaoIntegracaoSAPST = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPST(unidadeDeTrabalho, cancellationToken);
            IntegracaoSAPST configuracaoIntegracaoSAPST = await repConfiguracaoIntegracaoSAPST.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoSAPST = configuracaoIntegracaoSAPST?.PossuiIntegracaoSAPST ?? false,
                URLCriarAtendimento = configuracaoIntegracaoSAPST?.URLCriarAtendimento ?? string.Empty,
                Usuario = configuracaoIntegracaoSAPST?.Usuario ?? string.Empty,
                Senha = configuracaoIntegracaoSAPST?.Senha ?? string.Empty,
                URLCancelamentoST = configuracaoIntegracaoSAPST?.URLCancelamentoST ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoSAP_API4Async(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SAP_API4))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoSapAPI4 repConfiguracaoIntegracaoSAP_API4 = new Repositorio.Embarcador.Configuracoes.IntegracaoSapAPI4(unidadeDeTrabalho, cancellationToken);
            IntegracaoSapAPI4 configuracaoIntegracaoSAP_API4 = await repConfiguracaoIntegracaoSAP_API4.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoSAP_API4 = configuracaoIntegracaoSAP_API4?.PossuiIntegracaoSAP_API4 ?? false,
                UsuarioSAP_API4 = configuracaoIntegracaoSAP_API4?.UsuarioSAP_API4 ?? string.Empty,
                SenhaSAP_API4 = configuracaoIntegracaoSAP_API4?.SenhaSAP_API4 ?? string.Empty,
                URLSAP_API4 = configuracaoIntegracaoSAP_API4?.URLSAP_API4 ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoBradoAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Brado))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoBrado repConfiguracaoIntegracaoBrado = new Repositorio.Embarcador.Configuracoes.IntegracaoBrado(unidadeDeTrabalho, cancellationToken);
            IntegracaoBrado configuracaoIntegracaoBrado = await repConfiguracaoIntegracaoBrado.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoBrado = configuracaoIntegracaoBrado?.PossuiIntegracao ?? false,
                URLAutenticacaoBrado = configuracaoIntegracaoBrado?.URLAutenticacao ?? string.Empty,
                UsuarioBrado = configuracaoIntegracaoBrado?.Usuario ?? string.Empty,
                SenhaBrado = configuracaoIntegracaoBrado?.Senha ?? string.Empty,
                CodigoGestaoBrado = configuracaoIntegracaoBrado?.CodigoGestao ?? string.Empty,
                URLEnvioDadosTransporteBrado = configuracaoIntegracaoBrado?.URLEnvioDadosTransporte ?? string.Empty,
                URLEnvioDocumentosEmitidosBrado = configuracaoIntegracaoBrado?.URLEnvioDocumentosEmitidos ?? string.Empty,
                URLCancelamentoBrado = configuracaoIntegracaoBrado?.URLCancelamentoBrado ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoEFreteAsync(UnitOfWork unidadeDeTrabalho, Integracao configuracaoIntegracao, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete repConfiguracaoIntegracaoEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(unidadeDeTrabalho, cancellationToken);
            IntegracaoGeralEFrete configuracaoIntegracaoEFrete = await repConfiguracaoIntegracaoEFrete.BuscarPrimeiroRegistroAsync();

            return new
            {
                // T_CONFIGURACAO_INTEGRACAO
                CodigoIntegradorEFrete = configuracaoIntegracao?.CodigoIntegradorEFrete ?? string.Empty,
                UsuarioEFrete = configuracaoIntegracao?.UsuarioEFrete ?? string.Empty,
                SenhaEFrete = configuracaoIntegracao?.SenhaEFrete ?? string.Empty,
                MatrizEFrete = new
                {
                    Codigo = configuracaoIntegracao?.MatrizEFrete?.Codigo ?? 0,
                    Descricao = configuracaoIntegracao?.MatrizEFrete?.RazaoSocial ?? string.Empty
                },
                EncerrarTodosCIOTAutomaticamente = configuracaoIntegracao?.EncerrarTodosCIOTAutomaticamente ?? false,
                // T_CONFIGURACAO_INTEGRACAO_GERAL_EFRETE
                PossuiIntegracaoRecebivelEFrete = configuracaoIntegracaoEFrete?.PossuiIntegracaoRecebivel,
                DeduzirImpostosValorTotalFrete = configuracaoIntegracaoEFrete?.DeduzirImpostosValorTotalFrete,
                VersaoEFrete = configuracaoIntegracaoEFrete?.VersaoEFrete,
                URLRecebivelEFrete = configuracaoIntegracaoEFrete?.URLRecebivel ?? "",
                URLAutenticacaoEFrete = configuracaoIntegracaoEFrete?.URLAutenticacao ?? "",
                URLCancelamentoRecebivel = configuracaoIntegracaoEFrete?.URLCancelamentoRecebivel ?? "",
                URLPagamentoRecebivel = configuracaoIntegracaoEFrete?.URLPagamentoRecebivel ?? "",
                APIKeyEFrete = configuracaoIntegracaoEFrete?.APIKey ?? "",
                CodigoIntegracaoRecebivelEFrete = configuracaoIntegracaoEFrete?.CodigoIntegracaoRecebivel ?? "",
                UsuarioRecebivelEFrete = configuracaoIntegracaoEFrete?.UsuarioRecebivel ?? "",
                SenhaRecebivelEFrete = configuracaoIntegracaoEFrete?.SenhaRecebivel ?? "",
                EnviarImpostosNaIntegracaoDoCIOT = configuracaoIntegracaoEFrete?.EnviarImpostosNaIntegracaoDoCIOT ?? false,
                EnviarDadosRegulatorioANTT = configuracaoIntegracaoEFrete?.EnviarDadosRegulatorioANTT ?? false,
                ConsultarTagAoIncluirVeiculoNaCarga = configuracaoIntegracaoEFrete?.ConsultarTagAoIncluirVeiculoNaCarga ?? false,
            };
        }

        private async Task<object> ObterIntegracaoOpenTechAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.OpenTech))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoGeralOpenTech repositorioOpenTech = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralOpenTech(unidadeDeTrabalho, cancellationToken);

            IntegracaoGeralOpenTech configuracaoIntegracaoOpenTech = await repositorioOpenTech.BuscarPrimeiroRegistroAsync();

            return new
            {
                EnviarDataNFeNaDataPrevistaOpentech = configuracaoIntegracaoOpenTech?.EnviarDataNFeNaDataPrevistaOpentech,
                ConsiderarLocalidadeProdutoIntegracaoEntrega = configuracaoIntegracaoOpenTech?.ConsiderarLocalidadeProdutoIntegracaoEntrega
            };
        }

        private async Task<object> ObterIntegracaoEShipAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Eship))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoEShip repConfiguracaoIntegracaoEShip = new Repositorio.Embarcador.Configuracoes.IntegracaoEShip(unidadeDeTrabalho, cancellationToken);
            IntegracaoEship configuracaoIntegracaoEShip = await repConfiguracaoIntegracaoEShip.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoEship = configuracaoIntegracaoEShip?.PossuiIntegracao ?? false,
                URLComunicacaoEship = configuracaoIntegracaoEShip?.URLComunicacao ?? string.Empty,
                ApiKeyEship = configuracaoIntegracaoEShip?.ApiToken ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoYandehAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Yandeh))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoYandeh repConfiguracaoIntegracaoYandeh = new Repositorio.Embarcador.Configuracoes.IntegracaoYandeh(unidadeDeTrabalho, cancellationToken);
            IntegracaoYandeh configuracaoIntegracaoYandeh = await repConfiguracaoIntegracaoYandeh.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoYandeh = configuracaoIntegracaoYandeh?.PossuiIntegracao ?? false,
                URLComunicacaoYandeh = configuracaoIntegracaoYandeh?.URLComunicacao ?? string.Empty,
                UsuarioYandeh = configuracaoIntegracaoYandeh?.Usuario ?? string.Empty,
                SenhaYandeh = configuracaoIntegracaoYandeh?.Senha ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoLogRiskAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.LogRisk))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoLogRisk repoConfigIntLogRisk = new Repositorio.Embarcador.Configuracoes.IntegracaoLogRisk(unidadeDeTrabalho, cancellationToken);
            IntegracaoLogRisk configIntLogRisk = await repoConfigIntLogRisk.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoLogRisk = configIntLogRisk?.PossuiIntegracao ?? false,
                UsuarioLogRisk = configIntLogRisk?.Usuario ?? "",
                SenhaLogRisk = configIntLogRisk?.Senha ?? "",
                DominioLogRisk = configIntLogRisk?.Dominio ?? "",
                CNPJClienteLogRisk = configIntLogRisk?.CNPJCliente ?? ""
            };
        }

        private async Task<object> ObterIntegracaoDiageoAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Diageo))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoDiageo repConfiguracaoIntegracaoDiageo = new Repositorio.Embarcador.Configuracoes.IntegracaoDiageo(unidadeDeTrabalho, cancellationToken);
            IntegracaoDiageo configuracaoIntegracaoDiageo = await repConfiguracaoIntegracaoDiageo.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoDiageo = configuracaoIntegracaoDiageo?.PossuiIntegracao ?? false,
                EnderecoDiageo = configuracaoIntegracaoDiageo?.Endereco ?? string.Empty,
                PortaDiageo = configuracaoIntegracaoDiageo?.Porta ?? string.Empty,
                UsuarioDiageo = configuracaoIntegracaoDiageo?.Usuario ?? string.Empty,
                SenhaDiageo = configuracaoIntegracaoDiageo?.Senha ?? string.Empty,
                OutboundDiageo = configuracaoIntegracaoDiageo?.Outbound ?? string.Empty,
                DiretorioInboundDiageo = configuracaoIntegracaoDiageo?.DiretorioInbound ?? string.Empty,
                PassivoDiageo = configuracaoIntegracaoDiageo?.Passivo ?? false,
                UtilizarSFTPDiageo = configuracaoIntegracaoDiageo?.UtilizarSFTP ?? false,
                SSLDiageo = configuracaoIntegracaoDiageo?.SSL ?? false,


            };
        }

        private async Task<object> ObterIntegracaoP44Async(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.P44))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoP44 repConfiguracaoIntegracaoP44 = new Repositorio.Embarcador.Configuracoes.IntegracaoP44(unidadeDeTrabalho, cancellationToken);
            IntegracaoP44 configuracaoIntegracaoP44 = await repConfiguracaoIntegracaoP44.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoP44 = configuracaoIntegracaoP44?.PossuiIntegracao ?? false,
                UsuarioP44 = configuracaoIntegracaoP44?.Usuario ?? string.Empty,
                SenhaP44 = configuracaoIntegracaoP44?.Senha ?? string.Empty,
                ClientIdP44 = configuracaoIntegracaoP44?.ClientId ?? string.Empty,
                ClientSecretP44 = configuracaoIntegracaoP44?.ClientSecret ?? string.Empty,
                URLAutenticacaoP44 = configuracaoIntegracaoP44?.URLAutenticacao ?? string.Empty,
                URLAplicacaoP44 = configuracaoIntegracaoP44?.URLAplicacao ?? string.Empty,
                URLIntegracaoPatioP44 = configuracaoIntegracaoP44?.URLIntegracaoPatio ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoBalancaKIKIAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.BalancaKIKI))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoBalancaKIKI repConfiguracaoIntegracaoBalancaKIKI = new Repositorio.Embarcador.Configuracoes.IntegracaoBalancaKIKI(unidadeDeTrabalho, cancellationToken);
            IntegracaoBalancaKIKI configuracaoIntegracaoBalancaKIKI = await repConfiguracaoIntegracaoBalancaKIKI.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoBalancaKIKI = configuracaoIntegracaoBalancaKIKI?.PossuiIntegracao ?? false,
                URLBalancaKIKI = configuracaoIntegracaoBalancaKIKI?.URL ?? string.Empty
            };
        }

        private async Task<object> ObterIntegracaoComproveiAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Comprovei))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoComprovei repConfiguracaoIntegracaoComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(unidadeDeTrabalho, cancellationToken);
            IntegracaoComprovei configuracaoIntegracaoComprovei = await repConfiguracaoIntegracaoComprovei.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoComprovei = configuracaoIntegracaoComprovei?.PossuiIntegracao ?? false,
                URLComprovei = configuracaoIntegracaoComprovei?.URL ?? string.Empty,
                UsuarioComprovei = configuracaoIntegracaoComprovei?.Usuario ?? string.Empty,
                SenhaComprovei = configuracaoIntegracaoComprovei?.Senha ?? string.Empty,
                URLBaseRestComprovei = configuracaoIntegracaoComprovei?.URLBaseRest ?? string.Empty,
                URLComproveiIACanhoto = configuracaoIntegracaoComprovei?.URLIACanhoto ?? string.Empty,
                UsuarioComproveiIACanhoto = configuracaoIntegracaoComprovei?.UsuarioIACanhoto ?? string.Empty,
                SenhaComproveiIACanhoto = configuracaoIntegracaoComprovei?.SenhaIACanhoto ?? string.Empty,
                PossuiIntegracaoIACanhoto = configuracaoIntegracaoComprovei?.PossuiIntegracaoIACanhoto ?? false,
                URLIntegracaoRetornoGerarCarregamentoComprovei = configuracaoIntegracaoComprovei?.URLIntegracaoRetornoGerarCarregamento ?? string.Empty,
                URLIntegracaoRetornoConfirmacaoPedidosComprovei = configuracaoIntegracaoComprovei?.URLIntegracaoRetornoConfirmacaoPedidos ?? string.Empty,
                URLIntegracaoRetornoEnviarDigitalizacaoCanhotosComprovei = configuracaoIntegracaoComprovei?.URLIntegracaoRetornoEnviarDigitalizacaoCanhoto ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoComproveiRotaAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ComproveiRota))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoComproveiRota repConfiguracaoIntegracaoComproveiRota = new Repositorio.Embarcador.Configuracoes.IntegracaoComproveiRota(unidadeDeTrabalho, cancellationToken);
            IntegracaoComproveiRota configuracaoIntegracaoComproveiRota = await repConfiguracaoIntegracaoComproveiRota.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoComproveiRota = configuracaoIntegracaoComproveiRota?.PossuiIntegracao ?? false,
                URLComproveiRota = configuracaoIntegracaoComproveiRota?.URL ?? string.Empty,
                UsuarioComproveiRota = configuracaoIntegracaoComproveiRota?.Usuario ?? string.Empty,
                SenhaComproveiRota = configuracaoIntegracaoComproveiRota?.Senha ?? string.Empty,
                URLBaseRestComproveiRota = configuracaoIntegracaoComproveiRota?.URLBaseRest ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoKMMAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.KMM))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repConfiguracaoIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unidadeDeTrabalho, cancellationToken);
            IntegracaoKMM configuracaoIntegracaoKMM = await repConfiguracaoIntegracaoKMM.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoKMM = configuracaoIntegracaoKMM?.PossuiIntegracao ?? false,
                URLKMM = configuracaoIntegracaoKMM?.URL ?? string.Empty,
                UsuarioKMM = configuracaoIntegracaoKMM?.Usuario ?? string.Empty,
                SenhaKMM = configuracaoIntegracaoKMM?.Senha ?? string.Empty,
                CodGestaoKMM = configuracaoIntegracaoKMM?.CodGestao ?? null,
                TokenTimeHoursKMM = configuracaoIntegracaoKMM?.TokenTimeHours ?? null,
            };
        }

        private async Task<object> ObterIntegracaoLogvettAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Logvett))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoLogvett repConfiguracaoIntegracaoLogvett = new Repositorio.Embarcador.Configuracoes.IntegracaoLogvett(unidadeDeTrabalho, cancellationToken);
            IntegracaoLogvett configuracaoIntegracaoLogvett = await repConfiguracaoIntegracaoLogvett.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoLogvett = configuracaoIntegracaoLogvett?.PossuiIntegracao ?? false,
                UsuarioLogvett = configuracaoIntegracaoLogvett?.Usuario ?? string.Empty,
                SenhaLogvett = configuracaoIntegracaoLogvett?.Senha ?? string.Empty,
                URLTituloPagarLogvett = configuracaoIntegracaoLogvett?.URLTituloPagar ?? string.Empty,
                URLBaixarTituloLogvett = configuracaoIntegracaoLogvett?.URLBaixarTitulo ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoAtlasAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Atlas))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoAtlas repConfiguracaoIntegracaoAtlas = new Repositorio.Embarcador.Configuracoes.IntegracaoAtlas(unidadeDeTrabalho, cancellationToken);
            IntegracaoAtlas configuracaoIntegracaoAtlas = await repConfiguracaoIntegracaoAtlas.BuscarPrimeiroRegistroAsync();

            return new
            {
                AtivaAtlas = configuracaoIntegracaoAtlas?.Ativa ?? false,
                UsuarioAtlas = configuracaoIntegracaoAtlas?.Usuario ?? string.Empty,
                SenhaAtlas = configuracaoIntegracaoAtlas?.Senha ?? string.Empty,
                URLAcessoAtlas = configuracaoIntegracaoAtlas?.URLAcesso ?? string.Empty,
                CodigoClienteAtlas = configuracaoIntegracaoAtlas?.CodigoCliente ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoFloraAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Flora))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoFlora repConfiguracaoIntegracaoFlora = new Repositorio.Embarcador.Configuracoes.IntegracaoFlora(unidadeDeTrabalho, cancellationToken);
            IntegracaoFlora configuracaoIntegracaoFlora = await repConfiguracaoIntegracaoFlora.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoFlora = configuracaoIntegracaoFlora?.PossuiIntegracao ?? false,
                EnvioCargaFlora = configuracaoIntegracaoFlora?.EnvioCarga ?? string.Empty,
                UsuarioFlora = configuracaoIntegracaoFlora?.Usuario ?? string.Empty,
                SenhaFlora = configuracaoIntegracaoFlora?.Senha ?? string.Empty,
                URLFlora = configuracaoIntegracaoFlora?.URL ?? string.Empty,
                CodigoFretePrevistoFlora = configuracaoIntegracaoFlora?.CodigoFretePrevisto ?? string.Empty,
                CodigoFreteConfirmadoFlora = configuracaoIntegracaoFlora?.CodigoFreteConfirmado ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoCalistoAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Calisto))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoCalisto repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoCalisto(unitOfWork, cancellationToken);
            IntegracaoCalisto configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                URLCalisto = configuracaoIntegracao?.URL ?? string.Empty,
                PossuiIntegracaoCalisto = configuracaoIntegracao?.PossuiIntegracao ?? false,
                UsuarioCalisto = configuracaoIntegracao?.Usuario ?? string.Empty,
                SenhaCalisto = configuracaoIntegracao?.Senha ?? string.Empty,
                URLContabilizacao = configuracaoIntegracao?.URLContabilizacao ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoTrizyAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Trizy))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork, cancellationToken);
            IntegracaoTrizy configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                URLEnvioCarga = configuracaoIntegracao?.URLEnvioCarga ?? string.Empty,
                URLEnvioCancelamentoCarga = configuracaoIntegracao?.URLEnvioCancelamentoCarga ?? string.Empty,
                URLEnvioEventosPatio = configuracaoIntegracao?.URLEnvioEventosPatio ?? string.Empty,
                TokenEnvioMS = configuracaoIntegracao?.TokenEnvioMS ?? string.Empty,
                ValidarIntegracaoTrizyPorOperacao = configuracaoIntegracao?.ValidarIntegracaoPorOperacao ?? false,
                TrizyIntegrarApenasCargasComControleDeEntrega = configuracaoIntegracao?.IntegrarApenasCargasComControleDeEntrega ?? false,
                TrizyPermitirIntegrarMultiplasCargasParaOMesmoMotorista = configuracaoIntegracao?.PermitirIntegrarMultiplasCargasParaOMesmoMotorista ?? false,
                TrizyTipoDocumentoPais = configuracaoIntegracao?.TipoDocumentoPais ?? TipoDocumentoPaisTrizy.Brasil,
                TrizyEnviarPDFDocumentosFiscais = configuracaoIntegracao?.EnviarPDFDocumentosFiscais ?? false,
                TrizyDocumentosFiscaisEnvioPDF = configuracaoIntegracao?.DocumentosFiscaisEnvioPDF != null ? configuracaoIntegracao.DocumentosFiscaisEnvioPDF.Select(e => (int)e).ToArray() : new int[0],
                DiasIntervaloTracking = configuracaoIntegracao?.DiasIntervaloTracking ?? 0,
                EnviarPatchAtualizacoesEntrega = configuracaoIntegracao?.EnviarPatchAtualizacoesEntrega ?? false,
                EnviarNomeFantasiaQuandoPossuir = configuracaoIntegracao?.EnviarNomeFantasiaQuandoPossuir ?? false,
                VersaoIntegracaoTrizy = configuracaoIntegracao?.VersaoIntegracao ?? VersaoIntegracaoTrizy.Versao1,
                IntegrarOfertasCargas = configuracaoIntegracao?.IntegrarOfertasCargas ?? false,
                URLIntegracaoOfertas = configuracaoIntegracao?.URLIntegracaoOfertas ?? configuracaoIntegracao?.URLIntegracaoOfertas ?? string.Empty,
                URLIntegracaoGrupoMotoristas = configuracaoIntegracao?.URLIntegracaoGrupoMotoristas ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoObramaxAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Obramax))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoObramax repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoObramax(unitOfWork, cancellationToken);
            IntegracaoObramax configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoObramax = configuracaoIntegracao?.PossuiIntegracao ?? false,
                EndpointObramax = configuracaoIntegracao?.Endpoint ?? string.Empty,
                TokenObramax = configuracaoIntegracao?.Token ?? string.Empty,
                EndpointPedidoOcorrenciaObramax = configuracaoIntegracao?.EndpointPedidoOcorrencia ?? string.Empty,
                CodigoEventoCanhotoObramax = configuracaoIntegracao?.CodigoEventoCanhoto ?? 0,
            };
        }

        private async Task<object> ObterIntegracaoObramaxCTEAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ObramaxCTE))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoObramaxCTE repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoObramaxCTE(unitOfWork, cancellationToken);
            IntegracaoObramaxCTE configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoObramaxCTE = configuracaoIntegracao?.PossuiIntegracaoObramaxCTE ?? false,
                EndpointObramaxCTE = configuracaoIntegracao?.EndpointObramaxCTE ?? string.Empty,
                TokenObramaxCTE = configuracaoIntegracao?.TokenObramaxCTE ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoObramaxNFEAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ObramaxNFE))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoObramaxNFE repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoObramaxNFE(unitOfWork, cancellationToken);
            IntegracaoObramaxNFE configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoObramaxNFE = configuracaoIntegracao?.PossuiIntegracaoObramaxNFE ?? false,
                EndpointObramaxNFE = configuracaoIntegracao?.EndpointObramaxNFE ?? string.Empty,
                TokenObramaxNFE = configuracaoIntegracao?.TokenObramaxNFE ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoObramaxProvisaoAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ObramaxProvisao))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoObramaxProvisao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoObramaxProvisao(unitOfWork, cancellationToken);
            IntegracaoObramaxProvisao configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoObramaxProvisao = configuracaoIntegracao?.PossuiIntegracaoObramaxProvisao ?? false,
                EndpointObramaxProvisao = configuracaoIntegracao?.EndpointObramaxProvisao ?? string.Empty,
                TokenObramaxProvisao = configuracaoIntegracao?.TokenObramaxProvisao ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoShopeeAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Shopee))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoShopee repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoShopee(unitOfWork, cancellationToken);
            IntegracaoShopee configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoPacoteShopee = configuracaoIntegracao?.PossuiIntegracaoPacote ?? false,
                EndpointPacoteShopee = configuracaoIntegracao?.EndpointPacote ?? string.Empty,
                UsuarioShopee = configuracaoIntegracao?.Usuario ?? string.Empty,
                SenhaShopee = configuracaoIntegracao?.Senha ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoItalacAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Italac))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoItalac repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoItalac(unitOfWork, cancellationToken);
            IntegracaoItalac configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoItalac = configuracaoIntegracao?.PossuiIntegracaoItalac ?? false,
                URLItalac = configuracaoIntegracao?.URLItalac ?? string.Empty,
                UsuarioItalac = configuracaoIntegracao?.UsuarioItalac ?? string.Empty,
                SenhaItalac = configuracaoIntegracao?.SenhaItalac ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoItalacFaturaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ItalacFaturas))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoItalacFatura repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoItalacFatura(unitOfWork, cancellationToken);
            IntegracaoItalacFatura configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoItalacFatura = configuracaoIntegracao?.PossuiIntegracaoItalacFatura ?? false,
                URLItalacFatura = configuracaoIntegracao?.URLItalacFatura ?? string.Empty,
                UsuarioItalacFatura = configuracaoIntegracao?.UsuarioItalacFatura ?? string.Empty,
                SenhaItalacFatura = configuracaoIntegracao?.SenhaItalacFatura ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoPagerAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Pager))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoPager repositorioIntegracaoPager = new Repositorio.Embarcador.Configuracoes.IntegracaoPager(unitOfWork, cancellationToken);
            IntegracaoPager integracaoPager = await repositorioIntegracaoPager.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoPager = integracaoPager?.PossuiIntegracao ?? false,
                URLIntegracaoPager = integracaoPager?.URLIntegracao ?? string.Empty,
                UsuarioPager = integracaoPager?.Usuario ?? string.Empty,
                SenhaPager = integracaoPager?.Senha ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoElectroluxAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Electrolux))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux repConfiguracaoIntegracaoElectrolux = new Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux(unidadeDeTrabalho, cancellationToken);
            IntegracaoElectrolux configuracaoIntegracaoElectrolux = await repConfiguracaoIntegracaoElectrolux.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoElectrolux = configuracaoIntegracaoElectrolux?.PossuiIntegracao ?? false,
                UsuarioElectrolux = configuracaoIntegracaoElectrolux?.Usuario ?? string.Empty,
                SenhaElectrolux = configuracaoIntegracaoElectrolux?.Senha ?? string.Empty,
                URLConembElectrolux = configuracaoIntegracaoElectrolux?.URLCONEMB ?? string.Empty,
                LayoutEDIConembElectrolux = "",
                URLOcorrenElectrolux = configuracaoIntegracaoElectrolux?.URLOCORREN ?? string.Empty,
                LayoutEDIOcorrenElectrolux = "",
                URLNotfisListaElectrolux = configuracaoIntegracaoElectrolux?.UrlNotfisLista ?? string.Empty,
                URLNotfisDetalhadaElectrolux = configuracaoIntegracaoElectrolux?.UrlNotfisDetalhada ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoWhatsAppAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.WhatsApp))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoWhatsApp repConfiguracaoIntegracaoWhatsApp = new Repositorio.Embarcador.Configuracoes.IntegracaoWhatsApp(unitOfWork, cancellationToken);
            IntegracaoWhatsApp configuracaoIntegracaoWhatsApp = await repConfiguracaoIntegracaoWhatsApp.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoWhatsApp = configuracaoIntegracaoWhatsApp?.PossuiIntegracao ?? false,
                TokenWhatsApp = configuracaoIntegracaoWhatsApp?.Token ?? string.Empty,
                IdContaWhatsApp = configuracaoIntegracaoWhatsApp?.IdContaWhatsAppBusiness ?? string.Empty,
                IdNumeroTelefoneWhatsApp = configuracaoIntegracaoWhatsApp?.IdNumeroTelefone ?? string.Empty,
                IdAplicativoWhatsApp = configuracaoIntegracaoWhatsApp?.IdAplicativo ?? string.Empty,

            };
        }

        private async Task<object> ObterIntegracaoLoggiFaturasAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.LoggiFaturas))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoLoggiFaturas repConfiguracaoIntegracaoLoggiFaturas = new Repositorio.Embarcador.Configuracoes.IntegracaoLoggiFaturas(unidadeDeTrabalho, cancellationToken);
            IntegracaoLoggiFaturas configuracaoIntegracaoLoggiFaturas = await repConfiguracaoIntegracaoLoggiFaturas.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoLoggiFaturas = configuracaoIntegracaoLoggiFaturas?.PossuiIntegracao ?? false,
                URLLoggiFaturas = configuracaoIntegracaoLoggiFaturas?.URL ?? string.Empty,
                UsuarioLoggiFaturas = configuracaoIntegracaoLoggiFaturas?.Usuario ?? string.Empty,
                SenhaLoggiFaturas = configuracaoIntegracaoLoggiFaturas?.Senha ?? string.Empty,
                NumeroMaterialLoggiFaturas = configuracaoIntegracaoLoggiFaturas?.NumeroMaterial ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoRuntecAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Runtec))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoRuntec repConfiguracaoIntegracaoRuntec = new Repositorio.Embarcador.Configuracoes.IntegracaoRuntec(unidadeDeTrabalho, cancellationToken);
            IntegracaoRuntec configuracaoIntegracaoRuntec = await repConfiguracaoIntegracaoRuntec.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoRuntec = configuracaoIntegracaoRuntec?.PossuiIntegracao ?? false,
                URLRuntec = configuracaoIntegracaoRuntec?.URL ?? string.Empty,
                UsuarioRuntec = configuracaoIntegracaoRuntec?.Usuario ?? string.Empty,
                SenhaRuntec = configuracaoIntegracaoRuntec?.Senha ?? string.Empty
            };
        }

        private async Task<object> ObterIntegracaoATSLogAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ATSLog))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoATSLog repositorioConfiguracaoIntegracaoATSLog = new Repositorio.Embarcador.Configuracoes.IntegracaoATSLog(unidadeDeTrabalho, cancellationToken);
            IntegracaoATSLog configuracaoIntegracaoATSLog = await repositorioConfiguracaoIntegracaoATSLog.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoATSLog = configuracaoIntegracaoATSLog?.PossuiIntegracao ?? false,
                URLIntegracaoATSLog = configuracaoIntegracaoATSLog?.URL ?? string.Empty,
                UsuarioIntegracaoATSLog = configuracaoIntegracaoATSLog?.Usuario ?? string.Empty,
                SenhaIntegracaoATSLog = configuracaoIntegracaoATSLog?.Senha ?? string.Empty,
                SecretKeyIntegracaoATSLog = configuracaoIntegracaoATSLog?.SecretKey ?? string.Empty,
                CNPJCompanyIntegracaoATSLog = configuracaoIntegracaoATSLog?.CNPJCompany ?? string.Empty,
                NomeCompanyIntegracaoATSLog = configuracaoIntegracaoATSLog?.NomeCompany ?? string.Empty,
                LocalidadeIntegracaoATSLog = new
                {
                    Codigo = configuracaoIntegracaoATSLog?.Localidade?.Codigo ?? 0,
                    Descricao = configuracaoIntegracaoATSLog?.Localidade?.Descricao ?? string.Empty
                },
            };
        }

        private async Task<object> ObterIntegracaoBuntechAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Buntech))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoBuntech repositorioConfiguracaoIntegracaoBuntech = new Repositorio.Embarcador.Configuracoes.IntegracaoBuntech(unidadeDeTrabalho, cancellationToken);
            IntegracaoBuntech configuracaoIntegracaoBuntech = await repositorioConfiguracaoIntegracaoBuntech.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoBuntech = configuracaoIntegracaoBuntech?.PossuiIntegracao ?? false,
                URLAutenticacaoBuntech = configuracaoIntegracaoBuntech?.URLAutenticacao ?? string.Empty,
                URLProvisao = configuracaoIntegracaoBuntech?.URLProvisao ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoRouteasyAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Routeasy))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy repConfiguracaoIntegracaoRouteasy = new Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy(unitOfWork, cancellationToken);
            IntegracaoRouteasy configuracaoIntegracaoRouteasy = await repConfiguracaoIntegracaoRouteasy.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoRouteasy = configuracaoIntegracaoRouteasy?.PossuiIntegracao ?? false,
                URLIntegracaoRouteasy = configuracaoIntegracaoRouteasy?.URL ?? string.Empty,
                APIKeyIntegracaoRouteasy = configuracaoIntegracaoRouteasy?.APIKey ?? string.Empty,
                ConfiguracaoLoads = configuracaoIntegracaoRouteasy?.ConfiguracaoLoads ?? string.Empty,
                EnviarDataCriacaoVendaPedidoAbaAdicionaisIntegracao = configuracaoIntegracaoRouteasy?.EnviarDataCriacaoVendaPedidoAbaAdicionaisIntegracao ?? false
            };
        }

        private async Task<object> ObterIntegracaoConfirmaFacilAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ConfirmaFacil))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoConfirmaFacil repConfiguracaoIntegracaoConfirmaFacil = new Repositorio.Embarcador.Configuracoes.IntegracaoConfirmaFacil(unidadeDeTrabalho, cancellationToken);
            IntegracaoConfirmaFacil configuracaoIntegracaoConfirmaFacil = await repConfiguracaoIntegracaoConfirmaFacil.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoConfirmaFacil = configuracaoIntegracaoConfirmaFacil?.PossuiIntegracao ?? false,
                URLConfirmaFacil = configuracaoIntegracaoConfirmaFacil?.URL ?? string.Empty,
                EmailConfirmaFacil = configuracaoIntegracaoConfirmaFacil?.Email ?? string.Empty,
                SenhaConfirmaFacil = configuracaoIntegracaoConfirmaFacil?.Senha ?? string.Empty,
                IDClienteConfirmaFacil = configuracaoIntegracaoConfirmaFacil?.IDCliente ?? string.Empty,
                IDProdutoConfirmaFacil = configuracaoIntegracaoConfirmaFacil?.IDProduto ?? string.Empty
            };
        }

        private async Task<object> ObterIntegracaoBindAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Bind))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoBind repositorioIntegracaoBind = new Repositorio.Embarcador.Configuracoes.IntegracaoBind(unidadeDeTrabalho, cancellationToken);
            IntegracaoBind configuracaoIntegracaoBind = await repositorioIntegracaoBind.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoBind = configuracaoIntegracaoBind?.PossuiIntegracao ?? false,
                URLIntegracaoBind = configuracaoIntegracaoBind?.URLIntegracao ?? string.Empty,
                APIKeyIntegracaoBind = configuracaoIntegracaoBind?.APIKeyIntegracao ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoVectorAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Vector))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoVector repositorioIntegracaoVector = new Repositorio.Embarcador.Configuracoes.IntegracaoVector(unidadeDeTrabalho, cancellationToken);
            IntegracaoVector configuracaoIntegracaoVector = await repositorioIntegracaoVector.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoVector = configuracaoIntegracaoVector?.PossuiIntegracao ?? false,
                URLIntegracaoVector = configuracaoIntegracaoVector?.URLIntegracao ?? string.Empty,
                ClientIdIntegracaoVector = configuracaoIntegracaoVector?.ClientID ?? string.Empty,
                ClientSecretIntegracaoVector = configuracaoIntegracaoVector?.ClientSecret ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoTrizyEventosAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.TrizyEventos))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrizyEventos repositorioIntegracaoTrizyEventos = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizyEventos(unidadeDeTrabalho, cancellationToken);
            IntegracaoTrizyEventos configuracaoIntegracaoTrizyEventos = await repositorioIntegracaoTrizyEventos.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoTrizyEventos = configuracaoIntegracaoTrizyEventos?.PossuiIntegracao ?? false,
                URLIntegracaoTrizyEventos = configuracaoIntegracaoTrizyEventos?.URLIntegracao ?? string.Empty,
                TokenIntegracaoTrizyEventos = configuracaoIntegracaoTrizyEventos?.Token ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoMondelezAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Mondelez))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoMondelez repositorioIntegracaoMondelez = new Repositorio.Embarcador.Configuracoes.IntegracaoMondelez(unidadeDeTrabalho, cancellationToken);
            IntegracaoMondelez configuracaoIntegracaoMondelez = await repositorioIntegracaoMondelez.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoMondelez = configuracaoIntegracaoMondelez?.PossuiIntegracao ?? false,
                URLDrivinMondelez = configuracaoIntegracaoMondelez?.URLDrivin ?? string.Empty,
                ApiKeyDrivinMondelez = configuracaoIntegracaoMondelez?.ApiKeyDrivin ?? string.Empty,
                ApiKeyDrivinLegadoMondelez = configuracaoIntegracaoMondelez?.ApiKeyDrivinLegado ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoGrupoSCAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.GrupoSC))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoGrupoSC repositorioIntegracaoGrupoSC = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoSC(unidadeDeTrabalho, cancellationToken);
            IntegracaoGrupoSC configuracaoIntegracaoGrupoSC = await repositorioIntegracaoGrupoSC.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoGrupoSC = configuracaoIntegracaoGrupoSC?.PossuiIntegracao ?? false,
                URLIntegracaoGrupoSC = configuracaoIntegracaoGrupoSC?.URLIntegracao ?? string.Empty,
                ApiKeyGrupoSC = configuracaoIntegracaoGrupoSC?.ApiKey ?? string.Empty
            };
        }

        private async Task<object> ObterIntegracaoFusionAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Fusion))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoFusion repositorioIntegracaoFusion = new Repositorio.Embarcador.Configuracoes.IntegracaoFusion(unidadeDeTrabalho, cancellationToken);
            IntegracaoFusion configuracaoIntegracaoFusion = await repositorioIntegracaoFusion.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoFusion = configuracaoIntegracaoFusion?.PossuiIntegracao ?? false,
                URLIntegracaoPedidoFusion = configuracaoIntegracaoFusion?.URLIntegracaoPedido ?? string.Empty,
                TokenFusion = configuracaoIntegracaoFusion?.Token ?? string.Empty,
                URLIntegracaoCargaFusion = configuracaoIntegracaoFusion?.URLIntegracaoCarga ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoTrafegusAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Trafegus))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(unidadeDeTrabalho, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = await repConfiguracaoIntegracaoTrafegus.BuscarAsync();

            return new
            {
                PossuiIntegracaoTrafegus = configuracaoIntegracaoTrafegus?.PossuiIntegracao ?? false,
                PGRTrafegus = configuracaoIntegracaoTrafegus?.PGR ?? 0,
                URLIntegracaoCargaTrafegus = configuracaoIntegracaoTrafegus?.Url ?? string.Empty,
                UsuarioTrafegus = configuracaoIntegracaoTrafegus?.Usuario ?? string.Empty,
                SenhaTrafegus = configuracaoIntegracaoTrafegus?.Senha ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoSalesforceAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Salesforce))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoSalesforce repositorioIntegracaoSalesforce = new Repositorio.Embarcador.Configuracoes.IntegracaoSalesforce(unidadeDeTrabalho, cancellationToken);
            IntegracaoSalesforce configuracaoIntegracaoSalesforce = await repositorioIntegracaoSalesforce.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoSalesforce = configuracaoIntegracaoSalesforce?.PossuiIntegracao ?? false,
                URLBaseSalesforce = configuracaoIntegracaoSalesforce?.URLBase ?? string.Empty,
                URITokenSalesforce = configuracaoIntegracaoSalesforce?.URIToken ?? string.Empty,
                URICasoDevolucaoSalesforce = configuracaoIntegracaoSalesforce?.URICasoDevolucao ?? string.Empty,
                ClientIDSalesforce = configuracaoIntegracaoSalesforce?.ClientID ?? string.Empty,
                ClientSecretSalesforce = configuracaoIntegracaoSalesforce?.ClientSecret ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoConecttecAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Conecttec))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoConecttec repConfiguracaoIntegracaoConecttec = new Repositorio.Embarcador.Configuracoes.IntegracaoConecttec(unidadeDeTrabalho, cancellationToken);
            IntegracaoConecttec configuracaoIntegracaoConecttec = await repConfiguracaoIntegracaoConecttec.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoConecttec = configuracaoIntegracaoConecttec?.PossuiIntegracao ?? false,
                URLConecttec = configuracaoIntegracaoConecttec?.URL ?? string.Empty,
                ProviderIDConecttec = configuracaoIntegracaoConecttec?.ProviderID ?? string.Empty,
                StationIDConecttec = configuracaoIntegracaoConecttec?.StationID ?? string.Empty,
                PortaBrokerConecttec = configuracaoIntegracaoConecttec?.BrokerPort ?? 0,
                SecretKEYConecttec = configuracaoIntegracaoConecttec?.SecretKEY ?? string.Empty,
                URLRecebimentoCallbackConecttec = configuracaoIntegracaoConecttec?.URLRecebimentoCallback ?? string.Empty
            };
        }
        private async Task<object> ObterIntegracaoMigrateAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Migrate))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoMigrate repConfiguracaoIntegracaoMigrate = new Repositorio.Embarcador.Configuracoes.IntegracaoMigrate(unidadeDeTrabalho, cancellationToken);
            IntegracaoMigrate configuracaoIntegracaoMigrate = await repConfiguracaoIntegracaoMigrate.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracao = true,
                URLMigrate = configuracaoIntegracaoMigrate?.URL ?? string.Empty,
            };
        }
        private async Task<object> ObterIntegracaoMarsAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Mars))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoMars repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoMars(unidadeDeTrabalho, cancellationToken);
            IntegracaoMars configuracaoIntegracao = await repositorioIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoMars = configuracaoIntegracao?.PossuiIntegracao ?? false,
                URLIntegracaoCargaCTeMars = configuracaoIntegracao?.URLIntegracaoCargaCTe ?? string.Empty,
                URLAutenticacaoMars = configuracaoIntegracao?.URLAutenticacao ?? string.Empty,
                URLIntegracaoCanhotoMars = configuracaoIntegracao?.URLIntegracaoCanhoto ?? string.Empty,
                ClientIDMars = configuracaoIntegracao?.ClientID ?? string.Empty,
                ClientSecretMars = configuracaoIntegracao?.ClientSecret ?? string.Empty,
                URLIntegracaoCancelamentosCargas = configuracaoIntegracao?.URLIntegracaoCancelamentosCargas ?? string.Empty,
                ClientIDCancelamentosCargas = configuracaoIntegracao?.ClientIDCancelamentosCargas ?? string.Empty,
                ClientSecretCancelamentosCargas = configuracaoIntegracao?.ClientSecretCancelamentosCargas ?? string.Empty,
                URLAutenticacaoCancelamentosCargas = configuracaoIntegracao?.URLAutenticacaoCancelamentosCargas ?? string.Empty,
            };
        }

        private async Task<object> ObterConfiguracaoAcessoViaTokenAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken repositorioConfiguracaoAcessoViaToken = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken(unidadeDeTrabalho, cancellationToken);
            ConfiguracaoAcessoViaToken configuracaoAcessoViaToken = await repositorioConfiguracaoAcessoViaToken.BuscarPrimeiroRegistroAsync();

            return new
            {
                GerarUrlAcessoPortalMultiCliforAcessoViaToken = configuracaoAcessoViaToken?.GerarUrlAcessoPortalMultiClifor ?? false,
                AudienciaAcessoViaToken = configuracaoAcessoViaToken?.Audiencia ?? string.Empty,
                ChaveSecretaAcessoViaToken = configuracaoAcessoViaToken?.ChaveSecreta ?? string.Empty,
                EmissorAcessoViaToken = configuracaoAcessoViaToken?.Emissor ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoGlobusAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Globus))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoGlobus repIntegracaoGlobus = new Repositorio.Embarcador.Configuracoes.IntegracaoGlobus(unidadeDeTrabalho, cancellationToken);
            IntegracaoGlobus integracaoGlobus = await repIntegracaoGlobus.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoGlobus = integracaoGlobus?.PossuiIntegracao ?? false,
                IntegrarComContabilidadeGlobus = integracaoGlobus?.IntegrarComContabilidade ?? false,
                IntegrarComEscritaFiscalGlobus = integracaoGlobus?.IntegrarComEscritaFiscal ?? false,
                IntegrarComContasPagarGlobus = integracaoGlobus?.IntegrarComContasPagar ?? false,
                IntegrarComContasReceberGlobus = integracaoGlobus?.IntegrarComContasReceber ?? false,
                ShortCodeEscrituracaoISSGlobus = integracaoGlobus?.ShortCodeEscrituracaoISS ?? null,
                ShortCodeNFSeGlobus = integracaoGlobus?.ShortCodeNFSe ?? null,
                ShortCodeFinanceiroGlobus = integracaoGlobus?.ShortCodeFinanceiro ?? null,
                ShortCodeXMLGlobus = integracaoGlobus?.ShortCodeXML ?? null,
                ShortCodeParticipanteGlobus = integracaoGlobus?.ShortCodeParticipante ?? null,
                CodigoIntegrarComContabilidadeGlobus = integracaoGlobus?.CodigoIntegrarComContabilidade ?? null,
                CodigoIntegrarComEscritaFiscalGlobus = integracaoGlobus?.CodigoIntegrarComEscritaFiscal ?? null,
                CodigoIntegrarComContasPagarGlobus = integracaoGlobus?.CodigoIntegrarComContasPagar ?? null,
                CodigoIntegrarComContasReceberGlobus = integracaoGlobus?.CodigoIntegrarComContasReceber ?? null,
                SistemaIntegrarComContabilidadeGlobus = integracaoGlobus?.SistemaIntegrarComContabilidade ?? null,
                SistemaIntegrarComEscritaFiscalGlobus = integracaoGlobus?.SistemaIntegrarComEscritaFiscal ?? null,
                SistemaIntegrarComContasPagarGlobus = integracaoGlobus?.SistemaIntegrarComContasPagar ?? null,
                SistemaIntegrarComContasReceberGlobus = integracaoGlobus?.SistemaIntegrarComContasReceber ?? null,
                URLWebServiceEscrituracaoISSGlobus = integracaoGlobus?.URLWebServiceEscrituracaoISS ?? null,
                URLWebServiceNFSeGlobus = integracaoGlobus?.URLWebServiceNFSe ?? null,
                URLWebServiceFinanceiroGlobus = integracaoGlobus?.URLWebServiceFinanceiro ?? null,
                URLWebServiceXMLGlobus = integracaoGlobus?.URLWebServiceXML ?? null,
                URLWebServiceParticipanteGlobus = integracaoGlobus?.URLWebServiceParticipante ?? null,
                TokenEscrituracaoISSGlobus = integracaoGlobus?.TokenEscrituracaoISS ?? null,
                TokenNFSeGlobus = integracaoGlobus?.TokenNFSe ?? null,
                TokenFinanceiroGlobus = integracaoGlobus?.TokenFinanceiro ?? null,
                TokenXMLGlobus = integracaoGlobus?.TokenXML ?? null,
                TokenParticipanteGlobus = integracaoGlobus?.TokenParticipante ?? null,
                SistemaGlobus = integracaoGlobus?.Sistema ?? null,
                UsuarioGlobus = integracaoGlobus?.Usuario ?? null,
                GrupoGlobus = integracaoGlobus?.Grupo ?? null,
            };
        }

        private async Task<object> ObterIntegracaoFSAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.FS))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoFS repositorioIntegracaoFS = new Repositorio.Embarcador.Configuracoes.IntegracaoFS(unidadeDeTrabalho, cancellationToken);
            IntegracaoFS integracaoFS = await repositorioIntegracaoFS.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoFS = integracaoFS?.PossuiIntegracao ?? false,
                URLIntegracaoFS = integracaoFS?.URLIntegracao ?? string.Empty,
                URLAutenticacaoFS = integracaoFS?.URLAutenticacao ?? string.Empty,
                ClientIDFS = integracaoFS?.ClientID ?? string.Empty,
                ClientSecretFS = integracaoFS?.ClientSecret ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoVedacitAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Vedacit))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoVedacit repositorioIntegracaoVedacit = new Repositorio.Embarcador.Configuracoes.IntegracaoVedacit(unidadeDeTrabalho, cancellationToken);
            IntegracaoVedacit configuracaoIntegracaoVedacit = await repositorioIntegracaoVedacit.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoVedacit = configuracaoIntegracaoVedacit?.PossuiIntegracao ?? false,
                URLIntegracaoVedacit = configuracaoIntegracaoVedacit?.URLIntegracao ?? string.Empty,
                UsuarioIntegracaoVedacit = configuracaoIntegracaoVedacit?.Usuario ?? string.Empty,
                SenhaIntegracaoVedacit = configuracaoIntegracaoVedacit?.Senha ?? string.Empty,
                URLIntegracaoCargaVedacit = configuracaoIntegracaoVedacit?.URLIntegracaoCarga ?? string.Empty,
                UsuarioIntegracaoCargaVedacit = configuracaoIntegracaoVedacit?.UsuarioIntegracaoCarga ?? string.Empty,
                SenhaIntegracaoCargaVedacit = configuracaoIntegracaoVedacit?.SenhaIntegracaoCarga ?? string.Empty
            };
        }

        private async Task<object> ObterIntegracaoTransSatAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.TransSat))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoTransSat repIntegracaoTransSat = new Repositorio.Embarcador.Configuracoes.IntegracaoTransSat(unidadeDeTrabalho, cancellationToken);
            IntegracaoTransSat integracaoTransSat = await repIntegracaoTransSat.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoTransSat = integracaoTransSat?.PossuiIntegracao ?? false,
                URLWebServiceTransSat = integracaoTransSat?.URLWebServiceIntegracaoTransSat ?? null,
                TokenTransSat = integracaoTransSat?.TokenIntegracaoTransSat ?? null,
                EmailParaReceberRetornoDaGRTransSat = integracaoTransSat?.EmailParaReceberRetornoDaGR ?? null,
            };
        }

        private async Task<dynamic> ObterIntegracaoJDEFaturasAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.JDEFaturas))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoJDEFaturas repositorioIntegracaoJDEFaturas = new Repositorio.Embarcador.Configuracoes.IntegracaoJDEFaturas(unidadeDeTrabalho, cancellationToken);
            IntegracaoJDEFaturas configuracaoIntegracaoJDEFaturas = await repositorioIntegracaoJDEFaturas.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoJDEFaturas = configuracaoIntegracaoJDEFaturas?.PossuiIntegracao ?? false,
                URLIntegracaoJDEFaturas = configuracaoIntegracaoJDEFaturas?.URLIntegracao ?? string.Empty,
                UsuarioIntegracaoJDEFaturas = configuracaoIntegracaoJDEFaturas?.Usuario ?? string.Empty,
                SenhaIntegracaoJDEFaturas = configuracaoIntegracaoJDEFaturas?.Senha ?? string.Empty,
            };
        }

        private async Task<dynamic> ObterIntegracaoEfesusAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Efesus))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoEfesus repositorioIntegracaoEfesus = new Repositorio.Embarcador.Configuracoes.IntegracaoEfesus(unidadeDeTrabalho, cancellationToken);
            IntegracaoEfesus configuracaoIntegracaoEfesus = await repositorioIntegracaoEfesus.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoEfesus = configuracaoIntegracaoEfesus?.PossuiIntegracao ?? false,
                URLAutenticacaoEfesus = configuracaoIntegracaoEfesus?.URLIntegracao ?? string.Empty,
                UsuarioEfesus = configuracaoIntegracaoEfesus?.Usuario ?? string.Empty,
                SenhaEfesus = configuracaoIntegracaoEfesus?.Senha ?? string.Empty
            };
        }

        private async Task<dynamic> ObterIntegracaoOlfarAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Olfar))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoOlfar repositorioIntegracaoOlfar = new Repositorio.Embarcador.Configuracoes.IntegracaoOlfar(unidadeDeTrabalho, cancellationToken);
            IntegracaoOlfar configuracaoIntegracaoOlfar = await repositorioIntegracaoOlfar.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoOlfar = configuracaoIntegracaoOlfar?.PossuiIntegracao ?? false,
                URLIntegracaoOlfar = configuracaoIntegracaoOlfar?.URLIntegracao ?? string.Empty,
            };
        }

        private async Task<object> ObterIntegracaoCassolAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.CassolEventosEntrega) && !_tipoIntegracaoExistentes.Contains(TipoIntegracao.Cassol))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoCassol repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoCassol(unitOfWork, cancellationToken);
            IntegracaoCassol configuracaoIntegracao = await repConfiguracaoIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoCassol = configuracaoIntegracao?.PossuiIntegracao ?? false,
                URLCassol = configuracaoIntegracao?.URLIntegracao ?? string.Empty,
                TokenCassol = configuracaoIntegracao?.Token ?? string.Empty
            };
        }

        private async Task<object> ObterIntegracaoWeberChileAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.WeberChile))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoWeberChile repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoWeberChile(unidadeDeTrabalho, cancellationToken);
            IntegracaoWeberChile configuracaoIntegracao = await repositorioIntegracao.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoWeberChile = configuracaoIntegracao?.PossuiIntegracao ?? false,
                URLAutenticacaoWeberChile = configuracaoIntegracao?.URLAutenticacao ?? string.Empty,
                URLIntegracaoWeberChile = configuracaoIntegracao?.URLIntegracao ?? string.Empty,
                ClientIDWeberChile = configuracaoIntegracao?.ClientID ?? string.Empty,
                ClientSecretWeberChile = configuracaoIntegracao?.ClientSecret ?? string.Empty,
                APIKeyWeberChile = configuracaoIntegracao?.ApiKey ?? string.Empty
            };
        }

        private async Task<dynamic> ObterIntegracaoLactalisAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Lactalis))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoLactalis repositorioIntegracaoLactalis = new Repositorio.Embarcador.Configuracoes.IntegracaoLactalis(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLactalis configuracaoIntegracaoLactalis = await repositorioIntegracaoLactalis.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoLactalis = configuracaoIntegracaoLactalis?.PossuiIntegracao ?? false,
                URLIntegracaoLactalis = configuracaoIntegracaoLactalis?.URLIntegracao ?? string.Empty,
                URLAutenticacaoLactalis = configuracaoIntegracaoLactalis?.URLAutenticacao ?? string.Empty,
                UsuarioLactalis = configuracaoIntegracaoLactalis?.Usuario ?? string.Empty,
                SenhaLactalis = configuracaoIntegracaoLactalis?.Senha ?? string.Empty,
            };

        }

        private async Task<object> ObterIntegracaoPortalCabotagem(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.PortalCabotagem))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem repositorioIntegracaoPortalCabotagem = new Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem(unidadeDeTrabalho, cancellationToken);
            IntegracaoPortalCabotagem configuracaoIntegracaoPortalCabotagem = await repositorioIntegracaoPortalCabotagem.BuscarPrimeiroRegistroAsync();

            return new
            {
                AtivarIntegracaoPortalAzureStoragePortalCabotagem = configuracaoIntegracaoPortalCabotagem?.AtivarIntegracaoPortalAzureStorage ?? false,
                ContainerPortalCabotagem = configuracaoIntegracaoPortalCabotagem?.Container ?? string.Empty,
                StorageAccountPortalCabotagem = configuracaoIntegracaoPortalCabotagem?.StorageAccount ?? string.Empty,
                URLPortalCabotagem = configuracaoIntegracaoPortalCabotagem?.URL ?? string.Empty,
                ClienteIDPortalCabotagem = configuracaoIntegracaoPortalCabotagem?.ClienteID ?? string.Empty,
                SecretPortaCabotagem = configuracaoIntegracaoPortalCabotagem?.Secret ?? string.Empty,
                AtivarEnvioPDFFaturaPortalCabotagem = configuracaoIntegracaoPortalCabotagem?.AtivarEnvioPDFFatura ?? false,
                AtivarEnvioPDFCTEPortalCabotagem = configuracaoIntegracaoPortalCabotagem?.AtivarEnvioPDFCTE ?? false,
                AtivarEnvioPDFBOLETOPortalCabotagem = configuracaoIntegracaoPortalCabotagem?.AtivarEnvioPDFBoleto ?? false,
                AtivarEnvioXMLCTEPortalCabotagem = configuracaoIntegracaoPortalCabotagem?.AtivarEnvioXMLCTE ?? false
            };
        }

        private async Task<object> ObterIntegracaoSistemaTransbenAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SistemaTransben))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoSistemaTransben repIntegracaoSistemaTransben = new Repositorio.Embarcador.Configuracoes.IntegracaoSistemaTransben(unidadeDeTrabalho, cancellationToken);
            IntegracaoSistemaTransben integracaoSistemaTransben = await repIntegracaoSistemaTransben.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoSistemaTransben = integracaoSistemaTransben?.PossuiIntegracao ?? false,
                EnviarDadosCargaParaSistemaTransben = integracaoSistemaTransben?.EnviarDadosCargaParaSistemaTransben ?? false,
                URLIntegracaoSistemaTransben = integracaoSistemaTransben?.URLSistemaTransben ?? null,
                UsuarioIntegracaoSistemaTransben = integracaoSistemaTransben?.Usuario ?? null,
                SenhaIntegracaoSistemaTransben = integracaoSistemaTransben?.Senha ?? null
            };
        }
        private async Task<object> ObterIntegracaoATSSmartWebAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ATSSmartWeb))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoATSSmartWeb repIntegracaoATSSmartWeb = new Repositorio.Embarcador.Configuracoes.IntegracaoATSSmartWeb(unidadeDeTrabalho, cancellationToken);
            IntegracaoATSSmartWeb integracaoATSSmartWeb = await repIntegracaoATSSmartWeb.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoATSSmartWeb = integracaoATSSmartWeb?.PossuiIntegracao ?? false,
                SecretKeyATSSmartWeb = integracaoATSSmartWeb?.SecretKEY ?? null,
                URLIntegracaoATSSmartWeb = integracaoATSSmartWeb?.URL ?? null,
                UsuarioIntegracaoATSSmartWeb = integracaoATSSmartWeb?.Usuario ?? null,
                SenhaIntegracaoATSSmartWeb = integracaoATSSmartWeb?.Senha ?? null,
                CNPJCompanyIntegracaoATSSmartWeb = integracaoATSSmartWeb?.CNPJCompany ?? string.Empty,
                NomeCompanyIntegracaoATSSmartWeb = integracaoATSSmartWeb?.NomeCompany ?? string.Empty,
                LocalidadeIntegracaoATSSmartWeb = new
                {
                    Codigo = integracaoATSSmartWeb?.Localidade?.Codigo ?? 0,
                    Descricao = integracaoATSSmartWeb?.Localidade?.Descricao ?? string.Empty
                },
            };
        }
        private async Task<object> ObterIntegracaoVSTrackAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.VSTrack))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoVSTrack repIntegracaoVSTrack = new Repositorio.Embarcador.Configuracoes.IntegracaoVSTrack(unidadeDeTrabalho, cancellationToken);
            IntegracaoVSTrack integracaoVSTrack = await repIntegracaoVSTrack.BuscarPrimeiroRegistroAsync();

            return new
            {
                IntegracaoEtapa1CargaVSTrack = integracaoVSTrack?.IntegracaoEtapa1Carga ?? false,
                IntegracaoEtapa6CargaVSTrack = integracaoVSTrack?.IntegracaoEtapa6Carga ?? false,
                GrantTypeVSTrack = integracaoVSTrack?.GrantType ?? null,
                URLProducaoVSTrack = integracaoVSTrack?.URLProducao ?? null,
                URLHomologacaoVSTrack = integracaoVSTrack?.URLHomologacao ?? null,
                UsernameVSTrack = integracaoVSTrack?.Username ?? null,
                PasswordVSTrack = integracaoVSTrack?.Password ?? null,

            };
        }

        private async Task<object> ObterIntegracaoYMSAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.YMS))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoYMS repIntegracaoYMS
                = new Repositorio.Embarcador.Configuracoes.IntegracaoYMS(unidadeDeTrabalho, cancellationToken);
            IntegracaoYMS integracaoYMS = await repIntegracaoYMS.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoYMS = integracaoYMS?.PossuiIntegracao,
                URLIntegracaoAutenticacaoYMS = integracaoYMS?.URLAutenticacao ?? null,
                URLIntegracaoCriacaoYMS = integracaoYMS?.URLIntegracao ?? null,
                URLCancelamentoYMS = integracaoYMS?.URLCancelamento ?? null,
                UsuarioYMS = integracaoYMS?.Usuario ?? null,
                SenhaYMS = integracaoYMS?.Senha ?? null,
                TipoAutenticacaoYMS = integracaoYMS?.TipoAutenticacaoYMS ?? null,
                ParametrosAdicionaisYMS = integracaoYMS?.ParametrosAdicionais ?? null,
                URLIntegracaoAtualizacaoYMS = integracaoYMS?.URLIntegracaoAtualizacao,

            };
        }

        private async Task<object> ObterIntegracaoOnisysAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Onisys))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoOnisys repositorioIntegracaoOnisys = new Repositorio.Embarcador.Configuracoes.IntegracaoOnisys(unidadeDeTrabalho, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOnisys integracaoOnisys = await repositorioIntegracaoOnisys.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoOnisys = integracaoOnisys?.PossuiIntegracao ?? false,
                UrlAutenticacaoOnisys = integracaoOnisys?.URLAutenticacao ?? null,
                UsuarioOnisys = integracaoOnisys?.Usuario ?? null,
                SenhaOnisys = integracaoOnisys?.Senha ?? null,
                URLIntegracaoOnisys = integracaoOnisys?.URLIntegracao ?? null,
            };
        }

        private async Task<object> ObterIntegracaoSeniorAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Senior))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoSenior repositorioIntegracaoSenior
                = new Repositorio.Embarcador.Configuracoes.IntegracaoSenior(unidadeDeTrabalho, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSenior integracaoSenior = await repositorioIntegracaoSenior.BuscarPrimeiroRegistroAsync();

            return new
            {
                PossuiIntegracaoSenior = integracaoSenior?.PossuiIntegracao ?? false,
                URLAutenticacaoSenior = integracaoSenior?.URLAutenticacao ?? null,
                URLIntegracaoSenior = integracaoSenior?.URLIntegracao ?? null,
                UsuarioSenior = integracaoSenior?.Usuario ?? null,
                SenhaSenior = integracaoSenior?.Senha ?? null,
            };
        }

        private async Task<object> ObterIntegracaoHUBAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.HUB))
                return null;

            Repositorio.Embarcador.Configuracoes.IntegracaoHUB repositorioIntegracaoHUB = new Repositorio.Embarcador.Configuracoes.IntegracaoHUB(unidadeDeTrabalho, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHUB integracaoHUB = await repositorioIntegracaoHUB.BuscarPrimeiroRegistroAsync();

            return new
            {
                UrlAutenticacaoTokenHUB = integracaoHUB?.UrlAutenticacaoToken,
                UrlIntegracaoHUB = integracaoHUB?.UrlIntegracao,
                IdOrganizacaoHUB = integracaoHUB?.IdOrganizacao,
                ConexaoServiceBUSHUB = integracaoHUB?.ConexaoServiceBUS,
                ChaveSecretaHUB = integracaoHUB?.ChaveSecreta,

            };
        }

        private async Task<object> ObterIntegracaoSkymarkAsync(UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Skymark))
                return null;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoSkymark repositorioConfiguracaoIntegracaoSkymark
                = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoSkymark(unidadeDeTrabalho, cancellationToken);
            ConfiguracaoIntegracaoSkymark configuracaoIntegracaoSkymark = await repositorioConfiguracaoIntegracaoSkymark.BuscarPrimeiroRegistroAsync();

            return new
            {
                HabilitarIntegracaoSkymark = configuracaoIntegracaoSkymark?.HabilitarIntegracao,
                UrlSkymark = configuracaoIntegracaoSkymark?.Url ?? null,
                CampoIntegracaoSkymark = configuracaoIntegracaoSkymark?.Integracao ?? null,
                ContratoSkymark = configuracaoIntegracaoSkymark?.Contrato ?? null,
                ChaveUmSkymark = configuracaoIntegracaoSkymark?.ChaveUm ?? null,
                ChaveDoisSkymark = configuracaoIntegracaoSkymark?.ChaveDois ?? null,
            };
        }

        #endregion Métodos Privados - Obter Objeto Integração

        #region Métodos Privados - Salvar Abas Integrações

        private void SalvarConfiguracoesIntegracaoAngelLira(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.AngelLira))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repConfiguracaoIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira configuracaoIntegracaoAngelLira = repConfiguracaoIntegracaoAngelLira.Buscar();

            if (configuracaoIntegracaoAngelLira == null)
                configuracaoIntegracaoAngelLira = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira();
            else
                configuracaoIntegracaoAngelLira.Initialize();

            configuracaoIntegracaoAngelLira.EnviarDadosFormatados = Request.GetBoolParam("EnviarDadosFormatadosAngelLira");
            configuracaoIntegracaoAngelLira.Homologacao = Request.GetBoolParam("HomologacaoAngelLira");
            configuracaoIntegracaoAngelLira.Usuario = Request.GetStringParam("UsuarioAngelLira");
            configuracaoIntegracaoAngelLira.Senha = Request.GetStringParam("SenhaAngelLira");
            configuracaoIntegracaoAngelLira.URLAcesso = Request.GetStringParam("URLAngelLira");
            configuracaoIntegracaoAngelLira.IntegracaoTemperatura = Request.GetBoolParam("IntegracaoTemperaturaAngelLira");
            configuracaoIntegracaoAngelLira.ObterRotasAutomaticamente = Request.GetBoolParam("ObterRotasAutomaticamenteAngelLira");
            configuracaoIntegracaoAngelLira.UtilizarDataAgendamentoPedido = Request.GetBoolParam("UtilizarDataAgendamentoPedidoAngelLira");
            configuracaoIntegracaoAngelLira.NaoEnviarRotaViagem = Request.GetBoolParam("NaoEnviarRotaViagemAngelLira");
            configuracaoIntegracaoAngelLira.GerarViagensPorPedido = Request.GetBoolParam("GerarViagensPorPedidoAngelLira");
            configuracaoIntegracaoAngelLira.AplicarRegraLocalPalletizacao = Request.GetBoolParam("AplicarRegraLocalPalletizacaoAngelLira");
            configuracaoIntegracaoAngelLira.ConsultarPosicaoAbastecimento = Request.GetBoolParam("ConsultarPosicaoAbastecimentoAngelLira");
            configuracaoIntegracaoAngelLira.URLAcessoPedido = Request.GetStringParam("URLAcessoPedido");
            configuracaoIntegracaoAngelLira.UsuarioAcessoPedido = Request.GetStringParam("UsuarioAcessoPedido");
            configuracaoIntegracaoAngelLira.SenhaAcessoPedido = Request.GetStringParam("SenhaAcessoPedido");
            configuracaoIntegracaoAngelLira.UtilizarDataAtualETempoRotaParaInicioEFimViagem = Request.GetBoolParam("UtilizarDataAtualETempoRotaParaInicioEFimViagemAngelLira");
            configuracaoIntegracaoAngelLira.RegraCodigoIdentificacaoViagem = (AngelLiraRegraCodigoIdentificacaoViagem)Request.GetIntParam("RegraCodigoIdentificacaoViagem");
            configuracaoIntegracaoAngelLira.IgnorarValidacaoCargaAgrupadaRegraCodigoViagem = Request.GetBoolParam("IgnorarValidacaoCargaAgrupadaRegraCodigoViagemAngelLira");

            if (configuracaoIntegracaoAngelLira.Codigo > 0)
            {
                repConfiguracaoIntegracaoAngelLira.Atualizar(configuracaoIntegracaoAngelLira);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoAngelLira.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a AngelLira.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoAngelLira.Inserir(configuracaoIntegracaoAngelLira);

        }

        private void SalvarConfiguracoesIntegracaoAvon(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Avon))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoAvon repIntegracaoAvon = new Repositorio.Embarcador.Configuracoes.IntegracaoAvon(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            dynamic integracoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracoesIntegracaoAvon"));

            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon> integracoesExistentes = repIntegracaoAvon.BuscarTodos();

            if (integracoesExistentes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic integracao in integracoes)
                {
                    int codigo = 0;
                    if (integracao.Codigo != null && int.TryParse((string)integracao.Codigo, out codigo))
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon> integracoesDeletar = integracoesExistentes.Where(obj => !codigos.Contains(obj.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon integracaoDeletar in integracoesDeletar)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, null, $"Removeu a integração da Avon da empresa {integracaoDeletar.Empresa.Descricao}.", unidadeDeTrabalho);
                    repIntegracaoAvon.Deletar(integracaoDeletar);
                }
            }

            foreach (dynamic integracao in integracoes)
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon integracaoAvon = null;

                int codigo = 0;

                if (integracao.Codigo != null && int.TryParse((string)integracao.Codigo, out codigo))
                    integracaoAvon = repIntegracaoAvon.BuscarPorCodigo(codigo, false);

                if (integracaoAvon == null)
                    integracaoAvon = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon();
                else
                    integracaoAvon.Initialize();

                integracaoAvon.Empresa = repEmpresa.BuscarPorCodigo((int)integracao.Empresa.Codigo);
                integracaoAvon.EnterpriseID = (string)integracao.EnterpriseID;
                integracaoAvon.TokenHomologacao = (string)integracao.TokenHomologacao;
                integracaoAvon.TokenProducao = (string)integracao.TokenProducao;

                if (integracaoAvon.Codigo > 0)
                {
                    repIntegracaoAvon.Atualizar(integracaoAvon);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = integracaoAvon.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração da Avon da empresa {integracaoAvon.Empresa.Descricao}.", unidadeDeTrabalho);
                }
                else
                    repIntegracaoAvon.Inserir(integracaoAvon);


            }
        }

        private void SalvarConfiguracoesIntegracaoSAD(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SAD))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSAD repositorioSAD = new Repositorio.Embarcador.Configuracoes.IntegracaoSAD(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unidadeDeTrabalho);

            dynamic integracoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracoesIntegracaoSAD"));

            int? codigo;
            List<int> codigosManter = new List<int>();

            foreach (dynamic integracao in integracoes)
            {
                codigo = ((string)integracao.Codigo).ToNullableInt();
                if (codigo.HasValue)
                    codigosManter.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD> integracoesSAD = repositorioSAD.BuscarTodos();
            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD> integracoesSADDeletar = (from obj in integracoesSAD where !codigosManter.Contains(obj.Codigo) select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD integracaoSADDeletar in integracoesSADDeletar)
            {
                string mensagem = integracaoSADDeletar.CentroDescarregamento != null ? $" do CD {integracaoSADDeletar.CentroDescarregamento.Descricao}" : "";
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, null, $"Removeu a integração SAD{mensagem}.", unidadeDeTrabalho);
                repositorioSAD.Deletar(integracaoSADDeletar);
            }

            foreach (dynamic integracao in integracoes)
            {
                codigo = ((string)integracao.Codigo).ToNullableInt();

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD integracaoSADAdicionar = null;

                if (codigo.HasValue)
                    integracaoSADAdicionar = (from obj in integracoesSAD where obj.Codigo == codigo.Value select obj).FirstOrDefault();

                if (integracaoSADAdicionar == null)
                    integracaoSADAdicionar = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD();

                int? codigoCentroDescarregamento = ((string)integracao.CentroDescarregamento.Codigo).ToNullableInt();

                integracaoSADAdicionar.CentroDescarregamento = codigoCentroDescarregamento.HasValue ? repositorioCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento.Value) : null;
                integracaoSADAdicionar.Token = ((string)integracao.Token).ToString();
                integracaoSADAdicionar.URLIntegracaoSADBuscarSenha = ((string)integracao.URLIntegracaoSADBuscarSenha).ToString();
                integracaoSADAdicionar.URLIntegracaoSADFinalizarAgenda = ((string)integracao.URLIntegracaoSADFinalizarAgenda).ToString();
                integracaoSADAdicionar.URLIntegracaoSADCancelarAgenda = ((string)integracao.URLIntegracaoSADCancelarAgenda)?.ToString();

                if (integracaoSADAdicionar.Codigo > 0)
                {
                    repositorioSAD.Atualizar(integracaoSADAdicionar);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, null, $"Atualizou a integração SAD {integracaoSADAdicionar.Codigo}.", unidadeDeTrabalho);
                }
                else
                    repositorioSAD.Inserir(integracaoSADAdicionar);

            }
        }

        private void SalvarConfiguracoesIntegracaoDansales(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Dansales))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoDansales repositorioConfiguracaoIntegracaoDansales = new Repositorio.Embarcador.Configuracoes.IntegracaoDansales(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales configuracaoIntegracaoDansales = repositorioConfiguracaoIntegracaoDansales.Buscar();

            if (configuracaoIntegracaoDansales == null)
                configuracaoIntegracaoDansales = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales();
            else
                configuracaoIntegracaoDansales.Initialize();

            configuracaoIntegracaoDansales.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoDansales");
            configuracaoIntegracaoDansales.URLIntegracao = Request.GetStringParam("URLIntegracaoDansales");
            configuracaoIntegracaoDansales.URLIntegracaoChat = Request.GetStringParam("URLIntegracaoDansalesChat");
            configuracaoIntegracaoDansales.Usuario = Request.GetStringParam("UsuarioDansales");
            configuracaoIntegracaoDansales.Senha = Request.GetStringParam("SenhaDansales");

            configuracaoIntegracaoDansales.URLToken = Request.GetStringParam("URLIntegracaoDansalesToken");
            configuracaoIntegracaoDansales.UsuarioToken = Request.GetStringParam("UsuarioDansalesToken");
            configuracaoIntegracaoDansales.SenhaToken = Request.GetStringParam("SenhaDansalesToken");

            if (configuracaoIntegracaoDansales.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoDansales.Atualizar(configuracaoIntegracaoDansales);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoDansales.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a Dansales.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoDansales.Inserir(configuracaoIntegracaoDansales);

        }

        private void SalvarConfiguracoesIntegracaoKuehneNagel(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.KuehneNagel))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoKuehneNagel repositorioConfiguracaoIntegracaoKuehneNagel = new Repositorio.Embarcador.Configuracoes.IntegracaoKuehneNagel(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKuehneNagel configuracaoIntegracaoKuehneNagel = repositorioConfiguracaoIntegracaoKuehneNagel.Buscar();

            if (configuracaoIntegracaoKuehneNagel == null)
                configuracaoIntegracaoKuehneNagel = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKuehneNagel();
            else
                configuracaoIntegracaoKuehneNagel.Initialize();

            configuracaoIntegracaoKuehneNagel.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoKuehneNagel");
            configuracaoIntegracaoKuehneNagel.EnderecoFTP = Request.GetStringParam("EnderecoFTPKuehneNagel");
            configuracaoIntegracaoKuehneNagel.Usuario = Request.GetStringParam("UsuarioKuehneNagel");
            configuracaoIntegracaoKuehneNagel.Senha = Request.GetStringParam("SenhaKuehneNagel");
            configuracaoIntegracaoKuehneNagel.Porta = Request.GetStringParam("PortaKuehneNagel");
            configuracaoIntegracaoKuehneNagel.Diretorio = Request.GetStringParam("DiretorioKuehneNagel");
            configuracaoIntegracaoKuehneNagel.Passivo = Request.GetBoolParam("PassivoKuehneNagel");
            configuracaoIntegracaoKuehneNagel.UtilizarSFTP = Request.GetBoolParam("UtilizarSFTPKuehneNagel");
            configuracaoIntegracaoKuehneNagel.SSL = Request.GetBoolParam("SSLKuehneNagel");

            if (configuracaoIntegracaoKuehneNagel.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoKuehneNagel.Atualizar(configuracaoIntegracaoKuehneNagel);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoKuehneNagel.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a Kuehne+Nagel.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoKuehneNagel.Inserir(configuracaoIntegracaoKuehneNagel);

        }

        private void SalvarConfiguracoesIntegracaoMercadoLivre(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.MercadoLivre))
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            if (!repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MercadoLivre))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre repConfiguracaoIntegracaoMercadoLivre = new Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre configuracaoIntegracaoMercadoLivre = repConfiguracaoIntegracaoMercadoLivre.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoMercadoLivre == null)
                configuracaoIntegracaoMercadoLivre = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre();
            else
                configuracaoIntegracaoMercadoLivre.Initialize();

            configuracaoIntegracaoMercadoLivre.URL = Request.GetStringParam("URLMercadoLivre");
            configuracaoIntegracaoMercadoLivre.SecretKey = Request.GetStringParam("SecretKeyMercadoLivre");
            configuracaoIntegracaoMercadoLivre.ID = Request.GetStringParam("IDMercadoLivre");
            configuracaoIntegracaoMercadoLivre.LimparComposicaoCargaRetiradaRotaFacility = Request.GetBoolParam("LimparComposicaoCargaRetiradaRotaFacilityMercadoLivre");
            configuracaoIntegracaoMercadoLivre.NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes = Request.GetBoolParam("NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes");

            if (configuracaoIntegracaoMercadoLivre.Codigo <= 0)
            {
                repConfiguracaoIntegracaoMercadoLivre.Inserir(configuracaoIntegracaoMercadoLivre);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, null, $"Adicionou a configuração de integração com o Mercado Livre.", unitOfWork);
            }
            else
            {
                repConfiguracaoIntegracaoMercadoLivre.Atualizar(configuracaoIntegracaoMercadoLivre);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoMercadoLivre.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com o Mercado Livre.", unitOfWork);
            }
        }

        private void SalvarConfiguracoesIntegracaoA52(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.A52))
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            if (!repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoA52 repConfiguracaoIntegracaoA52 = new Repositorio.Embarcador.Configuracoes.IntegracaoA52(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracaoA52 = repConfiguracaoIntegracaoA52.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoA52 == null)
                configuracaoIntegracaoA52 = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52();
            else
                configuracaoIntegracaoA52.Initialize();

            configuracaoIntegracaoA52.URL = Request.GetStringParam("URLA52");
            configuracaoIntegracaoA52.CPFCNPJ = Request.GetStringParam("CPFCNPJA52");
            configuracaoIntegracaoA52.Senha = Request.GetStringParam("SenhaA52");
            configuracaoIntegracaoA52.UtilizarDataAgendamentoPedido = Request.GetBoolParam("UtilizarDataAgendamentoPedidoA52");
            configuracaoIntegracaoA52.IntegrarMacrosDadosTransporteCarga = Request.GetBoolParam("IntegrarMacrosDadosTransporteCargaA52");
            configuracaoIntegracaoA52.IntegrarSituacaoMotorista = Request.GetBoolParam("IntegrarSituacaoMotoristaA52");
            configuracaoIntegracaoA52.AplicarRegraLocalPalletizacao = Request.GetBoolParam("AplicarRegraLocalPalletizacaoA52");
            configuracaoIntegracaoA52.URLNova = Request.GetStringParam("URLNovaA52");
            configuracaoIntegracaoA52.VersaoIntegracao = Request.GetEnumParam("VersaoIntegracaoA52", VersaoA52Enum.Versao10);

            if (configuracaoIntegracaoA52.Codigo > 0)
            {
                repConfiguracaoIntegracaoA52.Atualizar(configuracaoIntegracaoA52);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoA52.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a A52.", unitOfWork);
            }
            else
                repConfiguracaoIntegracaoA52.Inserir(configuracaoIntegracaoA52);
        }

        private void SalvarConfiguracoesIntegracaoBBC(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.BBC))
                return;
            Repositorio.Embarcador.Configuracoes.IntegracaoBBC repConfiguracaoIntegracaoBBC = new Repositorio.Embarcador.Configuracoes.IntegracaoBBC(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC configuracaoIntegracaoBBC = repConfiguracaoIntegracaoBBC.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoBBC == null)
                configuracaoIntegracaoBBC = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBBC();
            else
                configuracaoIntegracaoBBC.Initialize();

            configuracaoIntegracaoBBC.URL = Request.GetStringParam("URLBBC");
            configuracaoIntegracaoBBC.PossuiIntegracaoViagem = Request.GetBoolParam("PossuiIntegracaoViagemBBC");
            configuracaoIntegracaoBBC.URLViagem = Request.GetStringParam("URLViagemBBC");
            configuracaoIntegracaoBBC.CnpjEmpresaViagem = Request.GetStringParam("CnpjEmpresaViagemBBC");
            configuracaoIntegracaoBBC.SenhaViagem = Request.GetStringParam("SenhaViagemBBC");
            configuracaoIntegracaoBBC.ClientSecret = Request.GetStringParam("ClientSecretBBC");

            if (configuracaoIntegracaoBBC.Codigo > 0)
            {
                repConfiguracaoIntegracaoBBC.Atualizar(configuracaoIntegracaoBBC);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoBBC.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a BBC.", unitOfWork);
            }
            else
                repConfiguracaoIntegracaoBBC.Inserir(configuracaoIntegracaoBBC);
        }

        private void SalvarConfiguracoesIntegracaoExtratta(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Extratta))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoExtratta repIntegracaoExtratta = new Repositorio.Embarcador.Configuracoes.IntegracaoExtratta(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta integracaoExtratta = repIntegracaoExtratta.Buscar();

            if (integracaoExtratta == null)
                integracaoExtratta = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta();
            else
                integracaoExtratta.Initialize();

            integracaoExtratta.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoExtratta");
            integracaoExtratta.URL = Request.GetStringParam("URLExtratta");
            integracaoExtratta.Token = Request.GetStringParam("TokenExtratta");
            integracaoExtratta.CNPJAplicacao = Request.GetStringParam("CNPJAplicacaoExtratta");
            integracaoExtratta.CNPJEmpresa = Request.GetStringParam("CNPJEmpresaExtratta");
            integracaoExtratta.DocumentoUsuario = Request.GetStringParam("DocumentoUsuarioExtratta");
            integracaoExtratta.Usuario = Request.GetStringParam("UsuarioExtratta");

            integracaoExtratta.IntegrarAbastecimentoComTicketLog = Request.GetBoolParam("IntegrarAbastecimentoComTicketLog");
            integracaoExtratta.CodigoClienteTicketLog = Request.GetStringParam("CodigoClienteTicketLog");
            integracaoExtratta.CodigoProdutoTicketLog = Request.GetStringParam("CodigoProdutoTicketLog");

            if (integracaoExtratta.Codigo > 0)
            {
                repIntegracaoExtratta.Atualizar(integracaoExtratta);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = integracaoExtratta.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a Extratta.", unidadeDeTrabalho);
            }
            else
                repIntegracaoExtratta.Inserir(integracaoExtratta);
        }

        private void SalvarConfiguracoesIntegracaoTarget(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Target))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoTargetGeral repositorioConfiguracaoIntegracaoTargetGeral = new Repositorio.Embarcador.Configuracoes.IntegracaoTargetGeral(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral configuracaoIntegracaoTargetGeral = repositorioConfiguracaoIntegracaoTargetGeral.Buscar();

            if (configuracaoIntegracaoTargetGeral == null)
                configuracaoIntegracaoTargetGeral = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral();
            else
                configuracaoIntegracaoTargetGeral.Initialize();

            configuracaoIntegracaoTargetGeral.PossuiIntegracaoEmpresa = Request.GetBoolParam("PossuiIntegracaoTargetEmpresa");
            configuracaoIntegracaoTargetGeral.URLEmpresa = Request.GetStringParam("URLTargetEmpresa");
            configuracaoIntegracaoTargetGeral.UsuarioEmpresa = Request.GetStringParam("UsuarioTargetEmpresa");
            configuracaoIntegracaoTargetGeral.SenhaEmpresa = Request.GetStringParam("SenhaTargetEmpresa");

            if (configuracaoIntegracaoTargetGeral.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoTargetGeral.Atualizar(configuracaoIntegracaoTargetGeral);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoTargetGeral.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a Target.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoTargetGeral.Inserir(configuracaoIntegracaoTargetGeral);
        }

        private void SalvarConfiguracoesIntegracaoRavex(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Ravex))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoRavex repositorioConfiguracaoIntegracaoRavex = new Repositorio.Embarcador.Configuracoes.IntegracaoRavex(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRavex configuracaoIntegracaoRavex = repositorioConfiguracaoIntegracaoRavex.Buscar();

            if (configuracaoIntegracaoRavex == null)
                configuracaoIntegracaoRavex = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRavex();
            else
                configuracaoIntegracaoRavex.Initialize();

            configuracaoIntegracaoRavex.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoRavex");
            configuracaoIntegracaoRavex.UrlIntegracao = Request.GetStringParam("URLRavex");
            configuracaoIntegracaoRavex.Senha = Request.GetStringParam("SenhaRavex");
            configuracaoIntegracaoRavex.Usuario = Request.GetStringParam("UsuarioRavex");

            if (configuracaoIntegracaoRavex.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoRavex.Atualizar(configuracaoIntegracaoRavex);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoRavex.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a Ravex.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoRavex.Inserir(configuracaoIntegracaoRavex);
        }

        private void SalvarConfiguracoesIntegracaoMicDta(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.MicDta))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoMicDta repositorioConfiguracaoIntegracaoMicDta = new Repositorio.Embarcador.Configuracoes.IntegracaoMicDta(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMicDta configuracaoIntegracaoMicDta = repositorioConfiguracaoIntegracaoMicDta.Buscar();

            if (configuracaoIntegracaoMicDta == null)
                configuracaoIntegracaoMicDta = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMicDta();
            else
                configuracaoIntegracaoMicDta.Initialize();

            configuracaoIntegracaoMicDta.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoMicDta");
            configuracaoIntegracaoMicDta.URL = Request.GetStringParam("URLMicDta");
            configuracaoIntegracaoMicDta.MetodoManifestacaoEmbarca = Request.GetStringParam("MetodoManifestacaoEmbarcaMicDta");
            configuracaoIntegracaoMicDta.GerarIntegracaNaEtapaDoFrete = Request.GetBoolParam("GerarIntegracaNaEtapaDoFrete");
            configuracaoIntegracaoMicDta.LicencaTNTI = Request.GetStringParam("LicencaTNTI");
            configuracaoIntegracaoMicDta.VencimentoLicencaTNTI = Request.GetNullableDateTimeParam("VencimentoLicencaTNTI");

            if (configuracaoIntegracaoMicDta.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoMicDta.Atualizar(configuracaoIntegracaoMicDta);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoMicDta.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a MIC/DTA.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoMicDta.Inserir(configuracaoIntegracaoMicDta);
        }

        private void SalvarConfiguracoesIntegracaoGadle(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Gadle))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoGadle repositorioConfiguracaoIntegracaoGadle = new Repositorio.Embarcador.Configuracoes.IntegracaoGadle(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle configuracaoIntegracaoGadle = repositorioConfiguracaoIntegracaoGadle.Buscar();

            if (configuracaoIntegracaoGadle == null)
                configuracaoIntegracaoGadle = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle();
            else
                configuracaoIntegracaoGadle.Initialize();

            configuracaoIntegracaoGadle.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoGadle");
            configuracaoIntegracaoGadle.URLIntegracaoGadle = Request.GetStringParam("URLIntegracaoGadle");
            configuracaoIntegracaoGadle.TokenIntegracaoGadle = Request.GetStringParam("TokenIntegracaoGadle");

            configuracaoIntegracao.PossuiIntegracaoGadle = Request.GetBoolParam("PossuiIntegracaoGadle");

            if (configuracaoIntegracaoGadle.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoGadle.Atualizar(configuracaoIntegracaoGadle);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoGadle.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a Gadle.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoGadle.Inserir(configuracaoIntegracaoGadle);
        }

        private void SalvarConfiguracoesIntegracaoTelhaNorte(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.TelhaNorte))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro repositorioTelhaNorteParametro = new Repositorio.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro> parametros = repositorioTelhaNorteParametro.Buscar();

            foreach (Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro parametroDeletar in parametros)
                repositorioTelhaNorteParametro.Deletar(parametroDeletar);

            dynamic integracaoTelhaNorte = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracoesIntegracaoTelhaNorte"));

            configuracaoIntegracao.URLTelhaNorte = ((string)integracaoTelhaNorte.DadosIntegracaoTelhaNorte.URLTelhaNorte).ToString();
            configuracaoIntegracao.URLPedidoTelhaNorte = ((string)integracaoTelhaNorte.DadosIntegracaoTelhaNorte.URLPedidoTelhaNorte).ToString();
            configuracaoIntegracao.URLObterTokenTelhaNorte = ((string)integracaoTelhaNorte.DadosIntegracaoTelhaNorte.URLObterToken).ToString();

            foreach (dynamic parametro in integracaoTelhaNorte.ParametrosIntegracaoTelhaNorte)
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro parametroAdicionar = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro()
                {
                    Chave = ((string)parametro.Chave).ToString(),
                    Valor = ((string)parametro.Valor).ToString()
                };

                repositorioTelhaNorteParametro.Inserir(parametroAdicionar);
            }
        }

        private void SalvarConfiguracoesIntegracaoInforDoc(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.InforDoc))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoInforDoc repositorioConfiguracaoIntegracaoInforDoc = new Repositorio.Embarcador.Configuracoes.IntegracaoInforDoc(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoInforDoc configuracaoIntegracaoInforDoc = repositorioConfiguracaoIntegracaoInforDoc.Buscar();

            if (configuracaoIntegracaoInforDoc == null)
                configuracaoIntegracaoInforDoc = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoInforDoc();
            else
                configuracaoIntegracaoInforDoc.Initialize();

            configuracaoIntegracaoInforDoc.URL = Request.GetStringParam("URLInforDoc");
            configuracaoIntegracaoInforDoc.APIKey = Request.GetStringParam("APIKeyInforDoc");

            if (configuracaoIntegracaoInforDoc.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoInforDoc.Atualizar(configuracaoIntegracaoInforDoc);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoInforDoc.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a InforDoc.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoInforDoc.Inserir(configuracaoIntegracaoInforDoc);
        }

        private void SalvarConfiguracoesIntegracaoIsis(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Isis))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoIsis repositorioConfiguracaoIntegracaoIsis = new Repositorio.Embarcador.Configuracoes.IntegracaoIsis(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIsis configuracaoIntegracaoIsis = repositorioConfiguracaoIntegracaoIsis.Buscar();

            if (configuracaoIntegracaoIsis == null)
                configuracaoIntegracaoIsis = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIsis();
            else
                configuracaoIntegracaoIsis.Initialize();

            configuracaoIntegracaoIsis.PossuiIntegracaoFTP = Request.GetBoolParam("PossuiIntegracaoFTPIsis");
            configuracaoIntegracaoIsis.EnderecoFTP = Request.GetStringParam("EnderecoFTPIsis");
            configuracaoIntegracaoIsis.Usuario = Request.GetStringParam("UsuarioIsis");
            configuracaoIntegracaoIsis.Senha = Request.GetStringParam("SenhaIsis");
            configuracaoIntegracaoIsis.Porta = Request.GetStringParam("PortaIsis");
            configuracaoIntegracaoIsis.Diretorio = Request.GetStringParam("DiretorioIsis");
            configuracaoIntegracaoIsis.Passivo = Request.GetBoolParam("PassivoIsis");
            configuracaoIntegracaoIsis.UtilizarSFTP = Request.GetBoolParam("UtilizarSFTPIsis");
            configuracaoIntegracaoIsis.SSL = Request.GetBoolParam("SSLIsis");
            configuracaoIntegracaoIsis.NomenclaturaArquivo = Request.GetStringParam("NomenclaturaArquivoIsis");
            configuracaoIntegracaoIsis.NomenclaturaArquivoCarregamento = Request.GetStringParam("NomenclaturaArquivoCarregamentoIsis");
            configuracaoIntegracaoIsis.DiretorioCarregamento = Request.GetStringParam("DiretorioCarregamentoIsis");

            if (configuracaoIntegracaoIsis.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoIsis.Atualizar(configuracaoIntegracaoIsis);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoIsis.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a Isis.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoIsis.Inserir(configuracaoIntegracaoIsis);

        }

        private void SalvarConfiguracoesIntegracaoMagalu(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Magalu))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoMagalu repositorioConfiguracaoIntegracaoMagalu = new Repositorio.Embarcador.Configuracoes.IntegracaoMagalu(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMagalu configuracaoIntegracaoMagalu = repositorioConfiguracaoIntegracaoMagalu.Buscar();

            if (configuracaoIntegracaoMagalu == null)
                configuracaoIntegracaoMagalu = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMagalu();
            else
                configuracaoIntegracaoMagalu.Initialize();

            configuracaoIntegracaoMagalu.URL = Request.GetStringParam("URLMagalu");
            configuracaoIntegracaoMagalu.Token = Request.GetStringParam("TokenMagalu");

            if (configuracaoIntegracaoMagalu.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoMagalu.Atualizar(configuracaoIntegracaoMagalu);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoMagalu.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Magalu.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoMagalu.Inserir(configuracaoIntegracaoMagalu);
        }

        private void SalvarConfiguracoesIntegracaoCadastroMulti(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.CadastrosMulti))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCadastroMulti repositorioIntegracaoCadastroMulti = new Repositorio.Embarcador.Configuracoes.IntegracaoCadastroMulti(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCadastroMulti integracaoCadastroMulti = repositorioIntegracaoCadastroMulti.Buscar();

            if (integracaoCadastroMulti == null)
                integracaoCadastroMulti = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCadastroMulti();
            else
                integracaoCadastroMulti.Initialize();

            integracaoCadastroMulti.EnviarDocumentacaoCTeAverbacaoInstancia = Request.GetBoolParam("EnviarDocumentacaoCTeAverbacaoInstancia");

            if (integracaoCadastroMulti.Codigo > 0)
            {
                repositorioIntegracaoCadastroMulti.Atualizar(integracaoCadastroMulti);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = integracaoCadastroMulti.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração Cadastros Multi.", unitOfWork);
            }
            else
                repositorioIntegracaoCadastroMulti.Inserir(integracaoCadastroMulti);
        }

        private void SalvarConfiguracoesIntegracaoGSW(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.GSW))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoGSW repositorioConfiguracaoIntegracaoGSW = new Repositorio.Embarcador.Configuracoes.IntegracaoGSW(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGSW configuracaoIntegracaoGSW = repositorioConfiguracaoIntegracaoGSW.Buscar();

            if (configuracaoIntegracaoGSW == null)
                configuracaoIntegracaoGSW = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGSW();
            else
                configuracaoIntegracaoGSW.Initialize();

            configuracaoIntegracaoGSW.URL = Request.GetStringParam("URLGSW");
            configuracaoIntegracaoGSW.Usuario = Request.GetStringParam("UsuarioGSW");
            configuracaoIntegracaoGSW.Senha = Request.GetStringParam("SenhaGSW");
            configuracaoIntegracaoGSW.CodigoInicialConsultaXMLCTe = Request.GetLongParam("CodigoInicialConsultaXMLCTeGSW");

            if (configuracaoIntegracaoGSW.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoGSW.Atualizar(configuracaoIntegracaoGSW);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoGSW.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a GSW.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoGSW.Inserir(configuracaoIntegracaoGSW);
        }

        private void SalvarConfiguracoesIntegracaoArquivei(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Arquivei))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoArquivei repositorioConfiguracaIntegracaoArquivei = new Repositorio.Embarcador.Configuracoes.IntegracaoArquivei(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArquivei configuracaoIntegracaoArquivei = repositorioConfiguracaIntegracaoArquivei.Buscar();

            if (configuracaoIntegracaoArquivei == null)
                configuracaoIntegracaoArquivei = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArquivei();
            else
                configuracaoIntegracaoArquivei.Initialize();

            configuracaoIntegracaoArquivei.URLArquivei = Request.GetStringParam("URLArquivei");
            configuracaoIntegracaoArquivei.IDArquivei = Request.GetStringParam("IDArquivei");
            configuracaoIntegracaoArquivei.KeyArquivei = Request.GetStringParam("KeyArquivei");
            configuracaoIntegracaoArquivei.CodigoInicialConsultaXMLCTeArquivei = Request.GetLongParam("CodigoInicialConsultaXMLCTeArquivei");

            if (configuracaoIntegracaoArquivei.Codigo > 0)
            {
                repositorioConfiguracaIntegracaoArquivei.Atualizar(configuracaoIntegracaoArquivei);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoArquivei.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Arquivei.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaIntegracaoArquivei.Inserir(configuracaoIntegracaoArquivei);
        }

        private void SalvarConfiguracoesIntegracaoCTASmart(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.CTASmart))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCTASmart repositorioConfiguracaoIntegracaoCTASmart = new Repositorio.Embarcador.Configuracoes.IntegracaoCTASmart(unidadeDeTrabalho);


            dynamic integracoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracoesIntegracaoCTASmart"));

            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart> integracoesExistentes = repositorioConfiguracaoIntegracaoCTASmart.BuscarTodos();

            if (integracoesExistentes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic integracao in integracoes)
                {
                    int codigo = 0;
                    if (integracao.Codigo != null && int.TryParse((string)integracao.Codigo, out codigo))
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart> integracoesDeletar = integracoesExistentes.Where(obj => !codigos.Contains(obj.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart integracaoDeletar in integracoesDeletar)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, null, $"Removeu a integração da CTASmart do Token: {integracaoDeletar.Token}.", unidadeDeTrabalho);
                    repositorioConfiguracaoIntegracaoCTASmart.Deletar(integracaoDeletar);
                }
            }

            foreach (dynamic integracao in integracoes)
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart integracaoCTASmart = null;

                int codigo = 0;

                if (integracao.Codigo != null && int.TryParse((string)integracao.Codigo, out codigo))
                    integracaoCTASmart = repositorioConfiguracaoIntegracaoCTASmart.BuscarPorCodigo(codigo, false);

                if (integracaoCTASmart == null)
                    integracaoCTASmart = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart();
                else
                    integracaoCTASmart.Initialize();

                integracaoCTASmart.URL = (string)integracao.URLCTASmart;
                integracaoCTASmart.Token = (string)integracao.TokenCTASmart;
                integracaoCTASmart.DataInicio = ((string)integracao.DataInicioCTASmart).ToDateTime();
                integracaoCTASmart.CodigoEmpresa = (string)integracao.CodigoEmpresaCTASmart;

                if (integracaoCTASmart.Codigo > 0)
                {
                    repositorioConfiguracaoIntegracaoCTASmart.Atualizar(integracaoCTASmart);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = integracaoCTASmart.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração da CTASmart do Token: {integracaoCTASmart.Token}.", unidadeDeTrabalho);
                }
                else
                    repositorioConfiguracaoIntegracaoCTASmart.Inserir(integracaoCTASmart);


            }


        }

        private void SalvarConfiguracoesIntegracaoDPA(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.DPA))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoDPA repositorioConfiguracaoIntegracaoDPA = new Repositorio.Embarcador.Configuracoes.IntegracaoDPA(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA configuracaoIntegracaoDPA = repositorioConfiguracaoIntegracaoDPA.Buscar();

            if (configuracaoIntegracaoDPA == null)
                configuracaoIntegracaoDPA = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA();
            else
                configuracaoIntegracaoDPA.Initialize();

            configuracaoIntegracaoDPA.URLIntegracaoDPA = Request.GetStringParam("URLIntegracaoDPA");
            configuracaoIntegracaoDPA.URLAutenticacaoDPA = Request.GetStringParam("URLAutenticacaoDPA");
            configuracaoIntegracaoDPA.UsuarioAutenticacaoDPA = Request.GetStringParam("UsuarioAutenticacaoDPA");
            configuracaoIntegracaoDPA.SenhaAutenticacaoDPA = Request.GetStringParam("SenhaAutenticacaoDPA");
            configuracaoIntegracaoDPA.URLIntegracaoDPACiot = Request.GetStringParam("URLIntegracaoDPACiot");
            configuracaoIntegracaoDPA.URLAutenticacaoDPACiot = Request.GetStringParam("URLAutenticacaoDPACiot");
            configuracaoIntegracaoDPA.UsuarioAutenticacaoDPACiot = Request.GetStringParam("UsuarioAutenticacaoDPACiot");
            configuracaoIntegracaoDPA.SenhaAutenticacaoDPACiot = Request.GetStringParam("SenhaAutenticacaoDPACiot");

            if (configuracaoIntegracaoDPA.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoDPA.Atualizar(configuracaoIntegracaoDPA);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoDPA.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a dpa.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoDPA.Inserir(configuracaoIntegracaoDPA);
        }

        private void SalvarConfiguracoesIntegracaoSaintGobain(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SaintGobain))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSaintGobain repositorioConfiguracaoIntegracaoSaintGobain = new Repositorio.Embarcador.Configuracoes.IntegracaoSaintGobain(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain configuracaoIntegracaoSaintGobain = repositorioConfiguracaoIntegracaoSaintGobain.Buscar();

            if (configuracaoIntegracaoSaintGobain == null)
                configuracaoIntegracaoSaintGobain = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain();
            else
                configuracaoIntegracaoSaintGobain.Initialize();

            configuracaoIntegracaoSaintGobain.UrlConsultaPedido = Request.GetStringParam("URLIntegracaoPedidoSaintGobain");
            configuracaoIntegracaoSaintGobain.UrlConsultaUsuario = Request.GetStringParam("URLIntegracaoUsuarioSaintGobain");
            configuracaoIntegracaoSaintGobain.UrlValidaToken = Request.GetStringParam("URLValidaTokenSaintGobain");
            configuracaoIntegracaoSaintGobain.APIKey = Request.GetStringParam("ApikeySaintGobain");
            configuracaoIntegracaoSaintGobain.ClientID = Request.GetStringParam("ClientIDSaintGobain");
            configuracaoIntegracaoSaintGobain.ClientSecret = Request.GetStringParam("ClientSecretSaintGobain");
            configuracaoIntegracaoSaintGobain.UrlIntegracaoFreteSnowFlake = Request.GetStringParam("UrlIntegracaoFreteSnowFlake");
            configuracaoIntegracaoSaintGobain.UrlIntegracaoCargaSnowFlake = Request.GetStringParam("UrlIntegracaoCargaSnowFlake");
            configuracaoIntegracaoSaintGobain.UrlIntegracaoAgendamentoSnowFlake = Request.GetStringParam("UrlIntegracaoAgendamentoSnowFlake");
            configuracaoIntegracaoSaintGobain.UsuariosSnowFlake = Request.GetStringParam("UsuariosSnowFlake");
            configuracaoIntegracaoSaintGobain.SenhaSnowFlake = Request.GetStringParam("SenhaSnowFlake");
            configuracaoIntegracaoSaintGobain.ApiKeySnowFlake = Request.GetStringParam("ApiKeySnowFlake");
            configuracaoIntegracaoSaintGobain.UtilizarEndPointPIPO = Request.GetBoolParam("UtilizarEndPointPIPO");

            if (configuracaoIntegracaoSaintGobain.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoSaintGobain.Atualizar(configuracaoIntegracaoSaintGobain);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoSaintGobain.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Saint Gobain.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoSaintGobain.Inserir(configuracaoIntegracaoSaintGobain);
        }

        private void SalvarConfiguracoesIntegracaoHavan(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Havan))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoHavan repositorioConfiguracaoIntegracaoHavan = new Repositorio.Embarcador.Configuracoes.IntegracaoHavan(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHavan configuracaoIntegracaoHavan = repositorioConfiguracaoIntegracaoHavan.Buscar();

            if (configuracaoIntegracaoHavan == null)
                configuracaoIntegracaoHavan = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHavan();
            else
                configuracaoIntegracaoHavan.Initialize();

            configuracaoIntegracaoHavan.URLAutenticacao = Request.GetStringParam("URLAutenticacaoHavan");
            configuracaoIntegracaoHavan.URLEnvioOcorrencia = Request.GetStringParam("URLEnvioOcorrenciaHavan");
            configuracaoIntegracaoHavan.Usuario = Request.GetStringParam("UsuarioHavan");
            configuracaoIntegracaoHavan.Senha = Request.GetStringParam("SenhaHavan");

            if (configuracaoIntegracaoHavan.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoHavan.Atualizar(configuracaoIntegracaoHavan);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoHavan.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Havan.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoHavan.Inserir(configuracaoIntegracaoHavan);
        }

        private void SalvarConfiguracoesIntegracaoFrota162(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Frota162))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoFrota162 repositorioConfiguracaoIntegracaoFrota162 = new Repositorio.Embarcador.Configuracoes.IntegracaoFrota162(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162 configuracaoIntegracaoFrota162 = repositorioConfiguracaoIntegracaoFrota162.Buscar();

            if (configuracaoIntegracaoFrota162 == null)
                configuracaoIntegracaoFrota162 = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162();
            else
                configuracaoIntegracaoFrota162.Initialize();

            configuracaoIntegracaoFrota162.PossuiIntegracaoFrota162 = Request.GetBoolParam("PossuiIntegracaoFrota162");
            configuracaoIntegracaoFrota162.Usuario = Request.GetStringParam("UsuarioFrota162");
            configuracaoIntegracaoFrota162.Senha = Request.GetStringParam("SenhaFrota162");
            configuracaoIntegracaoFrota162.URL = Request.GetStringParam("URLFrota162");
            configuracaoIntegracaoFrota162.Token = Request.GetStringParam("TokenFrota162");
            configuracaoIntegracaoFrota162.SecretKey = Request.GetStringParam("SecretKeyFrota162");
            configuracaoIntegracaoFrota162.CompanyId = Request.GetStringParam("CompanyIdFrota162");

            if (configuracaoIntegracaoFrota162.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoFrota162.Atualizar(configuracaoIntegracaoFrota162);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoFrota162.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração da Frota 162.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoFrota162.Inserir(configuracaoIntegracaoFrota162);
        }

        private void SalvarConfiguracoesIntegracaoDexco(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Dexco))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoDexco repositorioConfiguracaoIntegracaoDexco = new Repositorio.Embarcador.Configuracoes.IntegracaoDexco(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDexco configuracaoIntegracaoDexco = repositorioConfiguracaoIntegracaoDexco.BuscarIntegracao();

            if (configuracaoIntegracaoDexco == null)
                configuracaoIntegracaoDexco = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDexco();
            else
                configuracaoIntegracaoDexco.Initialize();

            configuracaoIntegracaoDexco.AccessKeyDexco = Request.GetStringParam("AccessKeyDexco");
            configuracaoIntegracaoDexco.UrlDexco = Request.GetStringParam("UrlDexco");
            configuracaoIntegracaoDexco.FoType = Request.GetStringParam("FoType");
            configuracaoIntegracaoDexco.Usuario = Request.GetStringParam("UsuarioDexco");
            configuracaoIntegracaoDexco.Senha = Request.GetStringParam("SenhaDexco");


            if (configuracaoIntegracaoDexco.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoDexco.Atualizar(configuracaoIntegracaoDexco);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoDexco.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração da Dexco.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoDexco.Inserir(configuracaoIntegracaoDexco);
        }

        private void SalvarConfiguracoesIntegracaoIntercab(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Intercab))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioConfiguracaoIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab configuracaoIntegracaoIntercab = repositorioConfiguracaoIntegracaoIntercab.BuscarIntegracao();

            if (configuracaoIntegracaoIntercab == null)
                configuracaoIntegracaoIntercab = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab();
            else
                configuracaoIntegracaoIntercab.Initialize();

            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            int codigoTipoOperacaoIntercab = Request.GetIntParam("TipoOperacaoIntercab");

            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            int codigoTipoCargaPadraoIntercab = Request.GetIntParam("TipoCargaPadraoIntercab");

            configuracaoIntegracaoIntercab.PossuiIntegracaoIntercab = Request.GetBoolParam("PossuiIntegracaoIntercab");
            configuracaoIntegracaoIntercab.AtivarIntegracaoCargas = Request.GetBoolParam("AtivarIntegracaoCargas");
            configuracaoIntegracaoIntercab.AtivarNovoHomeDash = Request.GetBoolParam("AtivarNovoHomeDash");
            configuracaoIntegracaoIntercab.IntegracaoDocumentacaoCarga = Request.GetBoolParam("IntegracaoDocumentacaoCarga");
            configuracaoIntegracaoIntercab.AtivarIntegracaoCancelamentoCarga = Request.GetBoolParam("AtivarIntegracaoCancelamentoCarga");
            configuracaoIntegracaoIntercab.AtivarIntegracaoCartaCorrecao = Request.GetBoolParam("AtivarIntegracaoCartaCorrecao");
            configuracaoIntegracaoIntercab.CodigoTipoOperacao = Request.GetStringParam("CodigoTipoOperacao");
            configuracaoIntegracaoIntercab.URLIntercab = Request.GetStringParam("URLIntercab");
            configuracaoIntegracaoIntercab.TokenIntercab = Request.GetStringParam("TokenIntercab");
            configuracaoIntegracaoIntercab.AtivarIntegracaoMercante = Request.GetBoolParam("AtivarIntegracaoMercante");
            configuracaoIntegracaoIntercab.AtivarIntegracaoCteManual = Request.GetBoolParam("AtivarIntegracaoCteManual");
            configuracaoIntegracaoIntercab.AtivarIntegracaoOcorrencias = Request.GetBoolParam("AtivarIntegracaoOcorrencias");
            configuracaoIntegracaoIntercab.DefinirModalPeloTipoCarga = Request.GetBoolParam("DefinirModalPeloTipoCarga");
            configuracaoIntegracaoIntercab.BuscarTipoServicoModeloDocumentoVinculadoCarga = Request.GetBoolParam("BuscarTipoServicoModeloDocumentoVinculadoCarga");
            configuracaoIntegracaoIntercab.ModificarTimelineDeAcordoComTipoServicoDocumento = Request.GetBoolParam("ModificarTimelineDeAcordoComTipoServicoDocumento");
            configuracaoIntegracaoIntercab.AtivarIntegracaoMDFeAquaviario = Request.GetBoolParam("AtivarIntegracaoMDFeAquaviario");
            configuracaoIntegracaoIntercab.AtivarIntegracaoCargaAtualParaNovo = Request.GetBoolParam("AtivarIntegracaoCargaAtualParaNovo");
            configuracaoIntegracaoIntercab.AtivarIntegracaoFatura = Request.GetBoolParam("AtivarIntegracaoFatura");
            configuracaoIntegracaoIntercab.HabilitarTimelineIntegracaoFaturaCarga = Request.GetBoolParam("HabilitarTimelineIntegracaoFaturaCarga");
            configuracaoIntegracaoIntercab.HabilitarTimelineFaturamentoCarga = Request.GetBoolParam("HabilitarTimelineFaturamentoCarga");
            configuracaoIntegracaoIntercab.HabilitarTimelineMercanteCarga = Request.GetBoolParam("HabilitarTimelineMercanteCarga");
            configuracaoIntegracaoIntercab.HabilitarTimelineMDFeAquaviario = Request.GetBoolParam("HabilitarTimelineMDFeAquaviario");
            configuracaoIntegracaoIntercab.AtivarNovosFiltrosConsultaCarga = Request.GetBoolParam("AtivarNovosFiltrosConsultaCarga");
            configuracaoIntegracaoIntercab.AjustarLayoutFiltrosTelaCarga = Request.GetBoolParam("AjustarLayoutFiltrosTelaCarga");
            configuracaoIntegracaoIntercab.ModificarTimelineIntegracaoCarga = Request.GetBoolParam("ModificarTimelineIntegracaoCarga");
            configuracaoIntegracaoIntercab.HabilitarTimelineCargaPortoPorto = Request.GetBoolParam("HabilitarTimelineCargaPortoPorto");
            configuracaoIntegracaoIntercab.AtivarPreFiltrosTelaCarga = Request.GetBoolParam("AtivarPreFiltrosTelaCargaIntercab");
            configuracaoIntegracaoIntercab.QuantidadeDiasParaDataInicial = Request.GetIntParam("QuantidadeDiasParaDataInicialIntercab");
            configuracaoIntegracaoIntercab.HabilitarTimelineCargaPorta = Request.GetBoolParam("HabilitarTimelineCargaPorta");
            configuracaoIntegracaoIntercab.HabilitarTimelineCargaSVMProprio = Request.GetBoolParam("HabilitarTimelineCargaSVMProprio");
            configuracaoIntegracaoIntercab.HabilitarTimelineCargaFeeder = Request.GetBoolParam("HabilitarTimelineCargaFeeder");
            configuracaoIntegracaoIntercab.RemoverObrigacaoCodigoEmbarcacaoCadastroNavio = Request.GetBoolParam("RemoverObrigacaoCodigoEmbarcacaoCadastroNavioIntercab");
            configuracaoIntegracaoIntercab.AtivarControleDashRegiaoOperador = Request.GetBoolParam("AtivarControleDashRegiaoOperador");
            configuracaoIntegracaoIntercab.SelecionarTipoOperacao = Request.GetBoolParam("SelecionarTipoOperacaoIntercab");
            configuracaoIntegracaoIntercab.AtivarGeracaoCCePelaRolagemWS = Request.GetBoolParam("AtivarGeracaoCCePelaRolagemWS");
            configuracaoIntegracaoIntercab.TipoOperacao = (codigoTipoOperacaoIntercab > 0 && configuracaoIntegracaoIntercab.SelecionarTipoOperacao) ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacaoIntercab) : null;
            configuracaoIntegracaoIntercab.TipoDeCargaPadrao = codigoTipoCargaPadraoIntercab > 0 ? repTipoDeCarga.BuscarPorCodigo(codigoTipoCargaPadraoIntercab) : null;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaMercante> situacoesCarga = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaMercante>("SituacoesCargaIntercab");
            if (configuracaoIntegracaoIntercab.SituacoesCarga == null)
                configuracaoIntegracaoIntercab.SituacoesCarga = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaMercante>();
            else
                configuracaoIntegracaoIntercab.SituacoesCarga.Clear();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaMercante situacaoCargaMercante in situacoesCarga)
                configuracaoIntegracaoIntercab.SituacoesCarga.Add(situacaoCargaMercante);

            if (configuracaoIntegracaoIntercab.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoIntercab.Atualizar(configuracaoIntegracaoIntercab);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoIntercab.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração da Intercab.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoIntercab.Inserir(configuracaoIntegracaoIntercab);
        }

        private void SalvarConfiguracoesIntegracaoProtheus(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Protheus))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoProtheus repositorioConfiguracaoIntegracaoProtheus = new Repositorio.Embarcador.Configuracoes.IntegracaoProtheus(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoProtheus configuracaoIntegracaoProtheus = repositorioConfiguracaoIntegracaoProtheus.Buscar();

            if (configuracaoIntegracaoProtheus == null)
                configuracaoIntegracaoProtheus = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoProtheus();
            else
                configuracaoIntegracaoProtheus.Initialize();

            configuracaoIntegracaoProtheus.PossuiIntegracaoProtheus = Request.GetBoolParam("PossuiIntegracaoProtheus");
            configuracaoIntegracaoProtheus.URLAutenticacao = Request.GetStringParam("URLAutenticacaoProtheus");
            configuracaoIntegracaoProtheus.Usuario = Request.GetStringParam("UsuarioProtheus");
            configuracaoIntegracaoProtheus.Senha = Request.GetStringParam("SenhaProtheus");

            if (configuracaoIntegracaoProtheus.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoProtheus.Atualizar(configuracaoIntegracaoProtheus);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoProtheus.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o Protheus.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoProtheus.Inserir(configuracaoIntegracaoProtheus);
        }

        private void SalvarConfiguracoesIntegracaoSimonetti(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Simonetti))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSimonetti repositorioConfiguracaoIntegracaoSimonetti = new Repositorio.Embarcador.Configuracoes.IntegracaoSimonetti(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSimonetti configuracaoIntegracaoSimonetti = repositorioConfiguracaoIntegracaoSimonetti.BuscarDadosIntegracao();

            if (configuracaoIntegracaoSimonetti == null)
                configuracaoIntegracaoSimonetti = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSimonetti();

            configuracaoIntegracaoSimonetti.PossuiIntegracaoSimonetti = Request.GetBoolParam("PossuiIntegracaoSimonetti");
            configuracaoIntegracaoSimonetti.URLEnviaOcorrenciaSimonetti = Request.GetStringParam("URLEnviaOcorrenciaSimonetti");

            if (configuracaoIntegracaoSimonetti.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoSimonetti.Atualizar(configuracaoIntegracaoSimonetti);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoSimonetti.GetChanges();

                if (alteracoes != null && alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o Simonetti.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoSimonetti.Inserir(configuracaoIntegracaoSimonetti);
        }

        private void SalvarConfiguracoesIntegracaoUnilever(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Unilever))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = repositorioConfiguracaoIntegracaoUnilever.Buscar();

            if (configuracaoIntegracaoUnilever == null)
                configuracaoIntegracaoUnilever = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever();
            else
                configuracaoIntegracaoUnilever.Initialize();

            configuracaoIntegracaoUnilever.PossuiIntegracaoUnilever = Request.GetBoolParam("PossuiIntegracaoUnilever");
            configuracaoIntegracaoUnilever.IntegrarValorPreCalculo = Request.GetBoolParam("IntegrarValorPreCalculo");
            configuracaoIntegracaoUnilever.IntegrarDadosValePedagio = Request.GetBoolParam("IntegrarDadosValePedagio");
            configuracaoIntegracaoUnilever.IntegrarAvancoParaEmissao = Request.GetBoolParam("IntegrarAvancoParaEmissao");
            configuracaoIntegracaoUnilever.IntegrarLeilaoManual = Request.GetBoolParam("IntegrarLeilaoManual");
            configuracaoIntegracaoUnilever.IntegrarLotePagamento = Request.GetBoolParam("IntegrarLotePagamento");
            configuracaoIntegracaoUnilever.IntegrarCanhoto = Request.GetBoolParam("IntegrarCanhoto");
            configuracaoIntegracaoUnilever.IntegrarCancelamentoPagamento = Request.GetBoolParam("IntegrarCancelamentoPagamento");
            configuracaoIntegracaoUnilever.URLIntegracaoRetornoUnilever = Request.GetStringParam("URLIntegracaoRetornoUnilever");
            configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever = Request.GetStringParam("ClientIDIntegracaoUnilever");
            configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever = Request.GetStringParam("ClientSecretIntegracaoUnilever");
            configuracaoIntegracaoUnilever.URLIntegracaoAvancoParaEmissao = Request.GetStringParam("URLIntegracaoAvancoParaEmissao");
            configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever = Request.GetStringParam("URLIntegracaoTravamentoDTUnilever");
            configuracaoIntegracaoUnilever.URLIntegracaoProvisaoUnilever = Request.GetStringParam("URLIntegracaoProvisaoUnilever");
            configuracaoIntegracaoUnilever.URLIntegracaoValorPreCalculoUnilever = Request.GetStringParam("URLIntegracaoValorPreCalculoUnilever");
            configuracaoIntegracaoUnilever.URLIntegracaoCancelamento = Request.GetStringParam("URLIntegracaoCancelamento");
            configuracaoIntegracaoUnilever.URLIntegracaoLeilaoManual = Request.GetStringParam("URLIntegracaoLeilaoManual");
            configuracaoIntegracaoUnilever.URLIntegracaoEscrituracaoRetorno = Request.GetStringParam("URLIntegracaoEscrituracaoRetorno");
            configuracaoIntegracaoUnilever.URLIntegracaoLotePagamento = Request.GetStringParam("URLIntegracaoLotePagamento");
            configuracaoIntegracaoUnilever.URLIntegracaoCancelamentoProvisao = Request.GetStringParam("URLIntegracaoCancelamentoProvisao");
            configuracaoIntegracaoUnilever.URLIntegracaoCanhoto = Request.GetStringParam("URLIntegracaoCanhoto");
            configuracaoIntegracaoUnilever.URLIntegracaoCancelamentoPagamento = Request.GetStringParam("URLIntegracaoCancelamentoPagamento");
            configuracaoIntegracaoUnilever.URLIntegracaoOcorrencia = Request.GetStringParam("URLIntegracaoOcorrencia");

            if (configuracaoIntegracaoUnilever.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoUnilever.Atualizar(configuracaoIntegracaoUnilever);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoUnilever.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o Unilever.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoUnilever.Inserir(configuracaoIntegracaoUnilever);
        }

        private void SalvarConfiguracoesIntegracaoMarisa(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Marisa))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoMarisa repositorioConfiguracaoIntegracaoMarisa = new Repositorio.Embarcador.Configuracoes.IntegracaoMarisa(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarisa existeIntegracao = repositorioConfiguracaoIntegracaoMarisa.BuscarDadosIntegracao();

            if (existeIntegracao == null)
                existeIntegracao = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarisa();
            else
                existeIntegracao.Initialize();

            existeIntegracao.PossuiIntegracaoMarisa = Request.GetBoolParam("PossuiIntegracaoMarisa");
            existeIntegracao.Usuario = Request.GetStringParam("UsuarioMarisa");
            existeIntegracao.Senha = Request.GetStringParam("SenhaMarisa");
            existeIntegracao.Url = Request.GetStringParam("UrlMarisa");
            existeIntegracao.EnderecoIntegracaoTabelaMarisa = Request.GetStringParam("EnderecoIntegracaoTabelaMarisa");
            existeIntegracao.UsuarioIntegracaoTabelaMarisa = Request.GetStringParam("UsuarioIntegracaoTabelaMarisa");
            existeIntegracao.SenhaIntegracaoTabelaMarisa = Request.GetStringParam("SenhaIntegracaoTabelaMarisa");

            if (existeIntegracao.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoMarisa.Atualizar(existeIntegracao);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = existeIntegracao.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o Marisa.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoMarisa.Inserir(existeIntegracao);
        }

        private void SalvarConfiguracoesIntegracaoNstech(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.NSTech))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoNSTech repositorioConfiguracaoIntegracaoNstech = new Repositorio.Embarcador.Configuracoes.IntegracaoNSTech(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech configuracaoIntegracaoNstech = repositorioConfiguracaoIntegracaoNstech.Buscar();

            if (configuracaoIntegracaoNstech == null)
                configuracaoIntegracaoNstech = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNSTech();
            else
                configuracaoIntegracaoNstech.Initialize();

            configuracaoIntegracaoNstech.UrlAutenticacao = Request.GetStringParam("UrlAutenticacaoNstech");
            configuracaoIntegracaoNstech.SenhaAutenticacao = Request.GetStringParam("SenhaAutenticacaoNstech");
            configuracaoIntegracaoNstech.IDAutenticacao = Request.GetStringParam("IDAutenticacaoNstech");
            configuracaoIntegracaoNstech.UrlIntegracaoSM = Request.GetStringParam("UrlIntegracaoSMNstech");
            configuracaoIntegracaoNstech.UrlIntegracaoSolicitacaoCadastral = Request.GetStringParam("UrlIntegracaoSolicitacaoCadastral");
            configuracaoIntegracaoNstech.UrlIntegracaoVerificacaoCadastral = Request.GetStringParam("UrlIntegracaoVerificacaoCadastral");


            if (configuracaoIntegracaoNstech.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoNstech.Atualizar(configuracaoIntegracaoNstech);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoNstech.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a NSTech.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoNstech.Inserir(configuracaoIntegracaoNstech);
        }

        private void SalvarConfiguracoesIntegracaoDeca(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Deca))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoDeca repositorioConfiguracaoIntegracaoDeca = new Repositorio.Embarcador.Configuracoes.IntegracaoDeca(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca configuracaoIntegracaoDeca = repositorioConfiguracaoIntegracaoDeca.Buscar();

            if (configuracaoIntegracaoDeca == null)
                configuracaoIntegracaoDeca = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca();
            else
                configuracaoIntegracaoDeca.Initialize();

            configuracaoIntegracaoDeca.PossuiIntegracaoDeca = Request.GetBoolParam("PossuiIntegracaoDeca");
            configuracaoIntegracaoDeca.URLAutenticacaoDeca = Request.GetStringParam("URLAutenticacaoDeca");
            configuracaoIntegracaoDeca.UsuarioDeca = Request.GetStringParam("UsuarioDeca");
            configuracaoIntegracaoDeca.SenhaDeca = Request.GetStringParam("SenhaDeca");

            configuracaoIntegracaoDeca.PossuiIntegracaoBalanca = Request.GetBoolParam("PossuiIntegracaoBalancaDeca");
            configuracaoIntegracaoDeca.URLBalanca = Request.GetStringParam("URLBalancaDeca");
            configuracaoIntegracaoDeca.TokenBalanca = Request.GetStringParam("TokenBalancaDeca");
            configuracaoIntegracaoDeca.URLInicioViagemDeca = Request.GetStringParam("URLInicioViagemDeca");

            if (configuracaoIntegracaoDeca.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoDeca.Atualizar(configuracaoIntegracaoDeca);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoDeca.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Deca.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoDeca.Inserir(configuracaoIntegracaoDeca);
        }

        private void SalvarConfiguracoesIntegracaoVLI(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.VLI))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoVLI repositorioConfiguracaoIntegracaoVLI = new Repositorio.Embarcador.Configuracoes.IntegracaoVLI(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI configuracaoIntegracaovli = repositorioConfiguracaoIntegracaoVLI.Buscar();

            if (configuracaoIntegracaovli == null)
                configuracaoIntegracaovli = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI();
            else
                configuracaoIntegracaovli.Initialize();

            configuracaoIntegracaovli.PossuiIntegracaoVLI = Request.GetBoolParam("PossuiIntegracaoVLI");
            configuracaoIntegracaovli.SenhaAutenticacao = Request.GetStringParam("SenhaAutenticacaoVLI");
            configuracaoIntegracaovli.IDAutenticacao = Request.GetStringParam("IDAutenticacaoVLI");
            configuracaoIntegracaovli.UrlAutenticacao = Request.GetStringParam("UrlAutenticacaoVLI");
            configuracaoIntegracaovli.UrlIntegracaoRastreamento = Request.GetStringParam("UrlIntegracaoRastreamentoVLI");
            configuracaoIntegracaovli.UrlIntegracaoCarregamento = Request.GetStringParam("UrlIntegracaoCarregamento");
            configuracaoIntegracaovli.UrlIntegracaoDescarregamentoPortosValeVLI = Request.GetStringParam("UrlIntegracaoDescarregamentoPortosValeVLI");
            configuracaoIntegracaovli.UrlIntegracaoDescarregamento = Request.GetStringParam("UrlIntegracaoDescarregamentoVLI");

            if (configuracaoIntegracaovli.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoVLI.Atualizar(configuracaoIntegracaovli);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaovli.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração VLI.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoVLI.Inserir(configuracaoIntegracaovli);
        }

        private void SalvarConfiguracoesIntegracaoMarilan(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Marilan))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoMarilan repositorioIntegracaoMarilan = new Repositorio.Embarcador.Configuracoes.IntegracaoMarilan(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarilan integracaoMarilan = repositorioIntegracaoMarilan.BuscarPrimeiroRegistro();

            if (integracaoMarilan == null)
                integracaoMarilan = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarilan();
            else
                integracaoMarilan.Initialize();

            integracaoMarilan.PossuiIntegracaoMarilan = Request.GetBoolParam("PossuiIntegracaoMarilan");
            integracaoMarilan.URLMarilan = Request.GetStringParam("URLMarilan");
            integracaoMarilan.URLMarilanChamadoOcorrencia = Request.GetStringParam("URLMarilanChamadoOcorrencia");
            integracaoMarilan.UsuarioMarilan = Request.GetStringParam("UsuarioMarilan");
            integracaoMarilan.SenhaMarilan = Request.GetStringParam("SenhaMarilan");

            if (integracaoMarilan.Codigo > 0)
            {
                repositorioIntegracaoMarilan.Atualizar(integracaoMarilan);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = integracaoMarilan.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Marilan.", unitOfWork);
            }
            else
                repositorioIntegracaoMarilan.Inserir(integracaoMarilan);
        }

        private void SalvarConfiguracoesIntegracaoCorreios(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Correios))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCorreios repositorioConfiguracaoIntegracaoCorreios = new Repositorio.Embarcador.Configuracoes.IntegracaoCorreios(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoIntegracaoCorreios = repositorioConfiguracaoIntegracaoCorreios.Buscar();

            if (configuracaoIntegracaoCorreios == null)
                configuracaoIntegracaoCorreios = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios();
            else
                configuracaoIntegracaoCorreios.Initialize();

            configuracaoIntegracaoCorreios.URLToken = Request.GetStringParam("URLTokenCorreios");
            configuracaoIntegracaoCorreios.URLEventos = Request.GetStringParam("URLEventosCorreios");
            configuracaoIntegracaoCorreios.CartaoPostagem = Request.GetStringParam("CartaoPostagemCorreios");
            configuracaoIntegracaoCorreios.URLPLP = Request.GetStringParam("URLPLPCorreios");
            configuracaoIntegracaoCorreios.UsuarioSIGEP = Request.GetStringParam("UsuarioSIGEP");
            configuracaoIntegracaoCorreios.SenhaSIGEP = Request.GetStringParam("SenhaSIGEP");
            configuracaoIntegracaoCorreios.NumeroContrato = Request.GetStringParam("NumeroContratoCorreios");
            configuracaoIntegracaoCorreios.NumeroDiretoria = Request.GetStringParam("NumeroDiretoriaCorreios");
            configuracaoIntegracaoCorreios.CodigoAdministrativo = Request.GetStringParam("CodigoAdministrativoCorreios");
            configuracaoIntegracaoCorreios.CodigoServicoAdicional = Request.GetStringParam("CodigoServicoAdicionalCorreios");

            if (configuracaoIntegracaoCorreios.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoCorreios.Atualizar(configuracaoIntegracaoCorreios);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoCorreios.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o Correios.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoCorreios.Inserir(configuracaoIntegracaoCorreios);
        }

        private void SalvarConfiguracoesIntegracaoArcelorMittal(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ArcelorMittal))
                return;


            Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal repositorioConfiguracaoIntegracaoArcelorMittal = new Repositorio.Embarcador.Configuracoes.IntegracaoArcelorMittal(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal configuracaoIntegracaoIntegracaoArcelorMittal = repositorioConfiguracaoIntegracaoArcelorMittal.Buscar();

            if (configuracaoIntegracaoIntegracaoArcelorMittal == null)
                configuracaoIntegracaoIntegracaoArcelorMittal = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal();
            else
                configuracaoIntegracaoIntegracaoArcelorMittal.Initialize();

            configuracaoIntegracaoIntegracaoArcelorMittal.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoArcelorMittal");
            configuracaoIntegracaoIntegracaoArcelorMittal.URLOcorrencia = Request.GetStringParam("URLOcorrenciaArcelorMittal");
            configuracaoIntegracaoIntegracaoArcelorMittal.URLConfirmarAvancoTransporte = Request.GetStringParam("URLConfirmarAvancoTransporteArcelorMittal");
            configuracaoIntegracaoIntegracaoArcelorMittal.URLAtualizarNFeAprovada = Request.GetStringParam("URLAtualizarNFeAprovada");
            configuracaoIntegracaoIntegracaoArcelorMittal.URLRetornoAdicionarPedidoEmLote = Request.GetStringParam("URLRetornoAdicionarPedidoEmLote");
            configuracaoIntegracaoIntegracaoArcelorMittal.Usuario = Request.GetStringParam("UsuarioArcelorMittal");
            configuracaoIntegracaoIntegracaoArcelorMittal.Senha = Request.GetStringParam("SenhaArcelorMittal");

            configuracaoIntegracaoIntegracaoArcelorMittal.URLDadosTransporteSAP = Request.GetStringParam("URLDadosTransporteSAP");


            if (configuracaoIntegracaoIntegracaoArcelorMittal.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoArcelorMittal.Atualizar(configuracaoIntegracaoIntegracaoArcelorMittal);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoIntegracaoArcelorMittal.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a ArcelorMittal.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoArcelorMittal.Inserir(configuracaoIntegracaoIntegracaoArcelorMittal);
        }

        private void SalvarConfiguracoesIntegracaoEMP(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.EMP))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho);

            bool enviarStatusTopicCTesAnterioresEMP = Request.GetBoolParam("EnviarTopicCTesAnterioresEMP");
            bool enviarStatusTopicBuscarCTesEMP = Request.GetBoolParam("EnviarTopicBuscarCTesEMP");
            bool enviarStatusTopicBuscarFaturaCTeEMP = Request.GetBoolParam("EnviarTopicBuscarFaturaCTeEMP");
            bool enviarStatusTopicBuscarCargaEMP = Request.GetBoolParam("EnviarTopicBuscarCargaEMP");
            bool enviarStatusTopicEnvioIntegracaoCargaEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoCargaEMP");
            bool enviarStatusTopicEnvioIntegracaoDadosCargaEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoDadosCargaEMP");
            bool enviarStatusTopicEnvioIntegracaoCancelamentoCargaEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoCancelamentoCargaEMP");
            bool enviarStatusTopicEnvioIntegracaoOcorrenciaEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoOcorrenciaEMP");
            bool enviarStatusTopicEnvioIntegracaoCancelamentoOcorrenciaEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoCancelamentoOcorrenciaEMP");
            bool enviarStatusTopicEnvioIntegracaoCTeManualEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoCTeManualEMP");
            bool enviarStatusTopicEnvioIntegracaoCancelamentoCTeManualEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoCancelamentoCTeManualEMP");
            bool enviarStatusTopicEnvioIntegracaoFaturaEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoFaturaEMP");
            bool enviarStatusTopicEnvioIntegracaoCancelamentoFaturaEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoCancelamentoFaturaEMP");
            bool enviarStatusTopicEnvioIntegracaoCartaCorrecaoEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoCartaCorrecaoEMP");
            bool enviarStatusTopicEnvioIntegracaoContainerEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoContainerEMP");
            bool enviarStatusTopicEnvioIntegracaoNFTPEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoNFTPEMP");
            bool enviarStatusTopicEnvioIntegracaoParaSILEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoParaSILEMP");
            bool enviarStatusTopicEnvioIntegracaoCEMercanteEMP = Request.GetBoolParam("EnviarTopicEnvioIntegracaoCEMercanteEMP");
            bool enviarStatusTopicRecebimentoIntegracaoVesselEMP = Request.GetBoolParam("EnviarTopicRecebimentoIntegracaoVesselEMP");
            bool enviarStatusTopicRecebimentoIntegracaoCustomerEMP = Request.GetBoolParam("EnviarTopicRecebimentoIntegracaoCustomerEMP");
            bool enviarStatusTopicRecebimentoIntegracaoBooking = Request.GetBoolParam("EnviarTopicRecebimentoIntegracaoBooking");
            bool enviarStatusTopicRecebimentoIntegracaoScheduleEMP = Request.GetBoolParam("EnviarTopicRecebimentoIntegracaoScheduleEMP");
            bool enviarStatusTopicRecebimentoIntegracaoRolagemEMP = Request.GetBoolParam("EnviarTopicRecebimentoIntegracaoRolagemEMP");
            bool enviarStatusTopicEnvioIntegracaoCTEDaCargaEMP = Request.GetBoolParam("EnviarTopicIntegracaoCTEDaCargaEMP");
            bool enviarStatusTopicEnvioIntegracaoCancelamentoDaCargaEMP = Request.GetBoolParam("EnviarTopicIntegracaoCancelamentoDaCargaEMP");
            bool enviarStatusTopicIntegracaoCTEManualEMP = Request.GetBoolParam("EnviarTopicIntegracaoCTEManualEMP");
            bool enviarStatusTopicEnvioIntegracaoCancelamentoCTEManuelEMP = Request.GetBoolParam("EnviarTopicIntegracaoCancelamentoCTEManualEMP");
            bool enviarStatusTopicIntegracaoOcorrenciaEMP = Request.GetBoolParam("EnviarTopicIntegracaoOcorrenciaEMP");
            bool enviarStatusTopicIntegracaoCancelamentoOcorrenciaEMP = Request.GetBoolParam("EnviarTopicIntegracaoCancelamentoOcorrenciaEMP");
            bool enviarStatusTopicIntegracaoFaturaEMP = Request.GetBoolParam("EnviarTopicIntegracaoFaturaEMP");
            bool enviarStatusTopicIntegracaoCancelamentoFaturaEMP = Request.GetBoolParam("EnviarTopicIntegracaoCancelamentoFaturaEMP");
            bool enviarStatusTopicIntegracaoCartaCorrecaoEMP = Request.GetBoolParam("EnviarTopicIntegracaoCartaCorrecaoEMP");
            bool enviarStatusTopicIntegracaoBoletoEMP = Request.GetBoolParam("EnviarTopicIntegracaoBoletoEMP");
            bool enviarStatusTopicIntegracaoCancelamentoBoletoEMP = Request.GetBoolParam("EnviarTopicIntegracaoCancelamentoBoletoEMP");
            bool enviarStatusTopicIntegracaoCancelamentoDoBoletoEMP = Request.GetBoolParam("EnviarTopicEnvioDoCancelamentoBoletoEMP");

            int.TryParse(Request.Params("TipoCargaPadraoEMP"), out int codigoTipoDeCarga);

            if (configuracaoIntegracaoIntegracaoEMP == null)
                configuracaoIntegracaoIntegracaoEMP = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP();
            else
                configuracaoIntegracaoIntegracaoEMP.Initialize();

            configuracaoIntegracaoIntegracaoEMP.PossuiIntegracaoEMP = Request.GetBoolParam("PossuiIntegracaoEMP");
            configuracaoIntegracaoIntegracaoEMP.BoostrapServersEMP = Request.GetStringParam("BoostrapServersEMP");
            //Auth
            configuracaoIntegracaoIntegracaoEMP.GroupIdEMP = Request.GetStringParam("GroupIdEMP");
            configuracaoIntegracaoIntegracaoEMP.UsuarioEMP = Request.GetStringParam("UsuarioEMP");
            configuracaoIntegracaoIntegracaoEMP.SenhaEMP = Request.GetStringParam("SenhaEMP");
            //Misc
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioCTesAnterioresEMP = Request.GetBoolParam("AtivarEnvioCTesAnterioresEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicCTesAnterioresEMP = Request.GetStringParam("TopicCTesAnterioresEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicCTesAnterioresEMP = enviarStatusTopicCTesAnterioresEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.TopicBuscarCTesEMP = Request.GetStringParam("TopicBuscarCTesEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicBuscarCTesEMP = enviarStatusTopicBuscarCTesEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.TopicBuscarFaturaCTeEMP = Request.GetStringParam("TopicBuscarFaturaCTeEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicBuscarFaturaCTeEMP = enviarStatusTopicBuscarFaturaCTeEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.TopicBuscarCargaEMP = Request.GetStringParam("TopicBuscarCargaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicBuscarCargaEMP = enviarStatusTopicBuscarCargaEMP ? "A" : "I";
            //Carga
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoCargaEMP = Request.GetBoolParam("AtivarIntegracaoCargaEMP");
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioSerializaçãoEMP = Request.GetBoolParam("AtivarEnvioSerializaçãoEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoCargaEMP = Request.GetStringParam("TopicEnvioIntegracaoCargaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoCargaEMP = enviarStatusTopicEnvioIntegracaoCargaEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoDadosCargaEMP = Request.GetStringParam("TopicEnvioIntegracaoDadosCargaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoDadosCargaEMP = enviarStatusTopicEnvioIntegracaoDadosCargaEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioIntegracaoCTEDaCargaEMP = Request.GetBoolParam("AtivarEnvioIntegracaoCTEDaCargaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicIntegracaoCTEDaCargaEMP = Request.GetStringParam("TopicIntegracaoCTEDaCargaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicIntegracaoCTEDaCargaEMP = enviarStatusTopicEnvioIntegracaoCTEDaCargaEMP ? "A" : "I";
            //Carga cancelamento
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoCancelamentoCargaEMP = Request.GetBoolParam("AtivarIntegracaoCancelamentoCargaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoCancelamentoCargaEMP = Request.GetStringParam("TopicEnvioIntegracaoCancelamentoCargaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoCancelamentoCargaEMP = enviarStatusTopicEnvioIntegracaoCancelamentoCargaEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoCancelamentoDaCargaEMP = Request.GetBoolParam("AtivarIntegracaoCancelamentoDaCargaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicIntegracaoCancelamentoDaCargaEMP = Request.GetStringParam("TopicIntegracaoCancelamentoDaCargaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicIntegracaoCancelamentoDaCargaEMP = enviarStatusTopicEnvioIntegracaoCancelamentoDaCargaEMP ? "A" : "I";
            //Ocorrencias
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoOcorrenciaEMP = Request.GetBoolParam("AtivarIntegracaoOcorrenciaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoOcorrenciaEMP = Request.GetStringParam("TopicEnvioIntegracaoOcorrenciaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoOcorrenciaEMP = enviarStatusTopicEnvioIntegracaoOcorrenciaEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioIntegracaoOcorrenciaEMP = Request.GetBoolParam("AtivarEnvioIntegracaoOcorrenciaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicIntegracaoOcorrenciaEMP = Request.GetStringParam("TopicIntegracaoOcorrenciaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicIntegracaoOcorrenciaEMP = enviarStatusTopicIntegracaoOcorrenciaEMP ? "A" : "I";
            //Ocorrencias cancelamento
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoCancelamentoOcorrenciaEMP = Request.GetBoolParam("AtivarIntegracaoCancelamentoOcorrenciaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoCancelamentoOcorrenciaEMP = Request.GetStringParam("TopicEnvioIntegracaoCancelamentoOcorrenciaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoCancelamentoOcorrenciaEMP = enviarStatusTopicEnvioIntegracaoCancelamentoOcorrenciaEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioIntegracaoCancelamentoOcorrenciaEMP = Request.GetBoolParam("AtivarEnvioIntegracaoCancelamentoOcorrenciaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicIntegracaoCancelamentoOcorrenciaEMP = Request.GetStringParam("TopicIntegracaoCancelamentoOcorrenciaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicIntegracaoCancelamentoOcorrenciaEMP = enviarStatusTopicIntegracaoCancelamentoOcorrenciaEMP ? "A" : "I";
            //CTe Manual
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoCTeManualEMP = Request.GetBoolParam("AtivarIntegracaoCTeManualEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoCTeManualEMP = Request.GetStringParam("TopicEnvioIntegracaoCTeManualEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoCTeManualEMP = enviarStatusTopicEnvioIntegracaoCTeManualEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoCancelamentoCTeManualEMP = Request.GetStringParam("TopicEnvioIntegracaoCancelamentoCTeManualEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoCancelamentoCTeManualEMP = enviarStatusTopicEnvioIntegracaoCancelamentoCTeManualEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioIntegracaoCTEManualEMP = Request.GetBoolParam("AtivarEnvioIntegracaoCTEManualEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicIntegracaoCTEManualEMP = Request.GetStringParam("TopicIntegracaoCTEManualEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicIntegracaoCTEManualEMP = enviarStatusTopicIntegracaoCTEManualEMP ? "A" : "I";
            //CTe Manual Cancelamento
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioIntegracaoCancelamentoCTEManualEMP = Request.GetBoolParam("AtivarEnvioIntegracaoCancelamentoCTEManualEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicIntegracaoCancelamentoCTEManualEMP = Request.GetStringParam("TopicIntegracaoCancelamentoCTEManualEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicIntegracaoCancelamentoCTEManualEMP = enviarStatusTopicEnvioIntegracaoCancelamentoCTEManuelEMP ? "A" : "I";
            //Fatura
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoFaturaEMP = Request.GetBoolParam("AtivarIntegracaoFaturaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoFaturaEMP = Request.GetStringParam("TopicEnvioIntegracaoFaturaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoFaturaEMP = enviarStatusTopicEnvioIntegracaoFaturaEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioIntegracaoFaturaEMP = Request.GetBoolParam("AtivarEnvioIntegracaoFaturaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicIntegracaoFaturaEMP = Request.GetStringParam("TopicIntegracaoFaturaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicIntegracaoFaturaEMP = enviarStatusTopicIntegracaoFaturaEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.EnviarNoLayoutAvroDoPortalEMP = Request.GetBoolParam("EnviarNoLayoutAvroDoPortalEMP");
            //Cancelamento fatura
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoCancelamentoFaturaEMP = Request.GetBoolParam("AtivarIntegracaoCancelamentoFaturaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoCancelamentoFaturaEMP = Request.GetStringParam("TopicEnvioIntegracaoCancelamentoFaturaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoCancelamentoFaturaEMP = enviarStatusTopicEnvioIntegracaoCancelamentoFaturaEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioIntegracaoCancelamentoFaturaEMP = Request.GetBoolParam("AtivarEnvioIntegracaoCancelamentoFaturaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicIntegracaoCancelamentoFaturaEMP = Request.GetStringParam("TopicIntegracaoCancelamentoFaturaEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicIntegracaoCancelamentoFaturaEMP = enviarStatusTopicIntegracaoCancelamentoFaturaEMP ? "A" : "I";
            //Carta Correcao
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoCartaCorrecaoEMP = Request.GetBoolParam("AtivarIntegracaoCartaCorrecaoEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoCartaCorrecaoEMP = Request.GetStringParam("TopicEnvioIntegracaoCartaCorrecaoEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoCartaCorrecaoEMP = enviarStatusTopicEnvioIntegracaoCartaCorrecaoEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioIntegracaoCartaCorrecaoEMP = Request.GetBoolParam("AtivarEnvioIntegracaoCartaCorrecaoEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicIntegracaoCartaCorrecaoEMP = Request.GetStringParam("TopicIntegracaoCartaCorrecaoEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicIntegracaoCartaCorrecaoEMP = enviarStatusTopicIntegracaoCartaCorrecaoEMP ? "A" : "I";
            //Boleto
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioIntegracaoBoletoEMP = Request.GetBoolParam("AtivarEnvioIntegracaoBoletoEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicIntegracaoBoletoEMP = Request.GetStringParam("TopicIntegracaoBoletoEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicIntegracaoBoletoEMP = enviarStatusTopicIntegracaoBoletoEMP ? "A" : "I";
            //Cancelamento Boleto
            configuracaoIntegracaoIntegracaoEMP.AtivarEnvioIntegracaoCancelamentoBoletoEMP = Request.GetBoolParam("AtivarEnvioIntegracaoCancelamentoBoletoEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicIntegracaoCancelamentoBoletoEMP = Request.GetStringParam("TopicIntegracaoCancelamentoBoletoEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicIntegracaoCancelamentoBoletoEMP = enviarStatusTopicIntegracaoCancelamentoBoletoEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoCancelamentoBoletoEMP = Request.GetBoolParam("AtivarIntegracaoCancelamentoBoletoEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoCancelamentoBoletoEMP = Request.GetStringParam("TopicEnvioIntegracaoCancelamentoBoletoEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoCancelamentoBoletoEMP = enviarStatusTopicIntegracaoCancelamentoDoBoletoEMP ? "A" : "I";
            //Container
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoContainerEMP = Request.GetBoolParam("AtivarIntegracaoContainerEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoContainerEMP = Request.GetStringParam("TopicEnvioIntegracaoContainerEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoContainerEMP = enviarStatusTopicEnvioIntegracaoContainerEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoComObjetoUnitoParaTodosTopics = Request.GetBoolParam("AtivarIntegracaoComObjetoUnitoParaTodosTopics");
            //Booking
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoBooking = Request.GetBoolParam("AtivarIntegracaoBooking");
            configuracaoIntegracaoIntegracaoEMP.TopicBooking = Request.GetStringParam("TopicBooking");
            configuracaoIntegracaoIntegracaoEMP.AtivarLeituraHeaderBooking = Request.GetBoolParam("AtivarLeituraHeaderBookingEMP");
            configuracaoIntegracaoIntegracaoEMP.ConsumerGroupBooking = Request.GetStringParam("ConsumerGroupBookingEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoBooking = enviarStatusTopicRecebimentoIntegracaoBooking ? "A" : "I";
            //Vessel-Navio
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoRecebimentoNavioEMP = Request.GetBoolParam("AtivarIntegracaoRecebimentoNavioEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicRecebimentoIntegracaoVesselEMP = Request.GetStringParam("TopicRecebimentoIntegracaoVesselEMP");
            configuracaoIntegracaoIntegracaoEMP.AtivarLeituraHeaderVessel = Request.GetBoolParam("AtivarLeituraHeaderVesselEMP");
            configuracaoIntegracaoIntegracaoEMP.ConsumerGroupVessel = Request.GetStringParam("ConsumerGroupVesselEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoVesselEMP = enviarStatusTopicRecebimentoIntegracaoVesselEMP ? "A" : "I";
            //Pessoa-Customer
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoRecebimentoPessoaEMP = Request.GetBoolParam("AtivarIntegracaoRecebimentoPessoaEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicRecebimentoIntegracaoCustomerEMP = Request.GetStringParam("TopicRecebimentoIntegracaoCustomerEMP");
            configuracaoIntegracaoIntegracaoEMP.AtivarLeituraHeaderCustomer = Request.GetBoolParam("AtivarLeituraHeaderCustomerEMP");
            configuracaoIntegracaoIntegracaoEMP.ConsumerGroupCustomer = Request.GetStringParam("ConsumerGroupCustomerEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoCustomerEMP = enviarStatusTopicRecebimentoIntegracaoCustomerEMP ? "A" : "I";
            //Schedule
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoRecebimentoScheduleEMP = Request.GetBoolParam("AtivarIntegracaoRecebimentoScheduleEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicRecebimentoIntegracaoScheduleEMP = Request.GetStringParam("TopicRecebimentoIntegracaoScheduleEMP");
            configuracaoIntegracaoIntegracaoEMP.AtivarLeituraHeaderSchedule = Request.GetBoolParam("AtivarLeituraHeaderScheduleEMP");
            configuracaoIntegracaoIntegracaoEMP.ConsumerGroupSchedule = Request.GetStringParam("ConsumerGroupScheduleEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoScheduleEMP = enviarStatusTopicRecebimentoIntegracaoScheduleEMP ? "A" : "I";
            //Rolagem
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoRecebimentoRolagemEMP = Request.GetBoolParam("AtivarIntegracaoRecebimentoRolagemEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicRecebimentoIntegracaoRolagemEMP = Request.GetStringParam("TopicRecebimentoIntegracaoRolagemEMP");
            configuracaoIntegracaoIntegracaoEMP.AtivarLeituraHeaderRolagem = Request.GetBoolParam("AtivarLeituraHeaderRolagemEMP");
            configuracaoIntegracaoIntegracaoEMP.ConsumerGroupRolagem = Request.GetStringParam("ConsumerGroupRolagemEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicRecebimentoIntegracaoRolagemEMP = enviarStatusTopicRecebimentoIntegracaoRolagemEMP ? "A" : "I";
            //SIL
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoParaSILEMP = Request.GetBoolParam("AtivarIntegracaoParaSILEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoParaSILEMP = Request.GetStringParam("TopicEnvioIntegracaoParaSILEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoParaSILEMP = enviarStatusTopicEnvioIntegracaoParaSILEMP ? "A" : "I";
            //Mercante
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoCEMercanteEMP = Request.GetBoolParam("AtivarIntegracaoCEMercanteEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoCEMercanteEMP = Request.GetStringParam("TopicEnvioIntegracaoCEMercanteEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusEnviarTopicEnvioIntegracaoCEMercanteEMP = enviarStatusTopicEnvioIntegracaoCEMercanteEMP ? "A" : "I";
            //SchemaRegistry
            configuracaoIntegracaoIntegracaoEMP.UrlSchemaRegistry = Request.GetStringParam("UrlSchemaRegistry");
            configuracaoIntegracaoIntegracaoEMP.UsuarioSchemaRegistry = Request.GetStringParam("UsuarioSchemaRegistry");
            configuracaoIntegracaoIntegracaoEMP.SenhaSchemaRegistry = Request.GetStringParam("SenhaSchemaRegistry");
            //Retina
            configuracaoIntegracaoIntegracaoEMP.ModificarConexaoParaRetina = Request.GetBoolParam("ModificarConexaoParaRetina");
            configuracaoIntegracaoIntegracaoEMP.ModificarConexaoParaEnvioRetina = Request.GetBoolParam("ModificarConexaoParaEnvioRetina");
            configuracaoIntegracaoIntegracaoEMP.GroupIDRetina = Request.GetStringParam("GroupIDRetina");
            configuracaoIntegracaoIntegracaoEMP.BootstrapServerRetina = Request.GetStringParam("BootstrapServerRetina");
            configuracaoIntegracaoIntegracaoEMP.URLSchemaRegistryRetina = Request.GetStringParam("URLSchemaRegistryRetina");
            configuracaoIntegracaoIntegracaoEMP.UsuarioServerRetina = Request.GetStringParam("UsuarioServerRetina");
            configuracaoIntegracaoIntegracaoEMP.UsuarioSchemaRegistryRetina = Request.GetStringParam("UsuarioSchemaRegistryRetina");
            configuracaoIntegracaoIntegracaoEMP.SenhaServerRetina = Request.GetStringParam("SenhaServerRetina");
            configuracaoIntegracaoIntegracaoEMP.SenhaSchemaRegistryRetina = Request.GetStringParam("SenhaSchemaRegistryRetina");
            //Avro
            configuracaoIntegracaoIntegracaoEMP.TipoAVRO = Request.GetEnumParam<TipoAVRO>("TipoAVRO");
            //Consumo
            configuracaoIntegracaoIntegracaoEMP.AtivarLeituraHeadersConsumoKeyEMP = Request.GetStringParam("AtivarLeituraHeadersConsumoKeyEMP");
            configuracaoIntegracaoIntegracaoEMP.AtivarLeituraHeadersConsumoValueEMP = Request.GetStringParam("AtivarLeituraHeadersConsumoValueEMP");

            //NFTP
            int.TryParse(Request.Params("ComponenteFreteValorNFTPEMP"), out int codigoComponenteFreteValorNFTPEMP);
            int.TryParse(Request.Params("ComponenteImpostosNFTPEMP"), out int codigoComponenteImpostosNFTPEMP);
            int.TryParse(Request.Params("ComponenteValorTotalPrestacaoNFTPEMP"), out int componenteValorTotalPrestacaoNFTPEMP);
            configuracaoIntegracaoIntegracaoEMP.AtivarIntegracaoNFTPEMP = Request.GetBoolParam("AtivarIntegracaoNFTPEMP");
            configuracaoIntegracaoIntegracaoEMP.TopicEnvioIntegracaoNFTPEMP = Request.GetStringParam("TopicEnvioIntegracaoNFTPEMP");
            configuracaoIntegracaoIntegracaoEMP.StatusTopicEnvioIntegracaoNFTPEMP = enviarStatusTopicEnvioIntegracaoNFTPEMP ? "A" : "I";
            configuracaoIntegracaoIntegracaoEMP.ComponenteFreteNFTPEMP = repComponenteFrete.BuscarPorCodigo(codigoComponenteFreteValorNFTPEMP);
            configuracaoIntegracaoIntegracaoEMP.ComponenteImpostoNFTPEMP = repComponenteFrete.BuscarPorCodigo(codigoComponenteImpostosNFTPEMP);
            configuracaoIntegracaoIntegracaoEMP.ComponenteValorTotalPrestacaoNFTPEMP = repComponenteFrete.BuscarPorCodigo(componenteValorTotalPrestacaoNFTPEMP);

            configuracaoIntegracaoIntegracaoEMP.TipoDeCarga = repTipoCarga.BuscarPorCodigo(codigoTipoDeCarga);

            if (configuracaoIntegracaoIntegracaoEMP.Codigo > 0)
            {
                repConfiguracaoIntegracaoEMP.Atualizar(configuracaoIntegracaoIntegracaoEMP);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoIntegracaoEMP.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a EMP.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoEMP.Inserir(configuracaoIntegracaoIntegracaoEMP);
        }

        private void SalvarConfiguracoesIntegracaoTicketLog(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.TicketLog))
                return;

            Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento repConfigAbastecimento = new Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoTicketLog repConfiguracaoIntegracaoTicketLog = new Repositorio.Embarcador.Configuracoes.IntegracaoTicketLog(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTicketLog configuracaoIntegracaoTicketLog = repConfiguracaoIntegracaoTicketLog.Buscar();

            if (configuracaoIntegracaoTicketLog == null)
                configuracaoIntegracaoTicketLog = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTicketLog();
            else
                configuracaoIntegracaoTicketLog.Initialize();

            int codigoConfigAbastecimentoTicketLog = Request.GetIntParam("ConfiguracaoAbastecimentoTicketLog");

            configuracaoIntegracaoTicketLog.PossuiIntegracaoTicketLog = Request.GetBoolParam("PossuiIntegracaoTicketLog");
            configuracaoIntegracaoTicketLog.URLTicketLog = Request.GetStringParam("URLTicketLog");
            configuracaoIntegracaoTicketLog.UsuarioTicketLog = Request.GetStringParam("UsuarioTicketLog");
            configuracaoIntegracaoTicketLog.SenhaTicketLog = Request.GetStringParam("SenhaTicketLog");
            configuracaoIntegracaoTicketLog.CodigoClienteTicketLog = Request.GetStringParam("CodigoClienteTicketLog");
            configuracaoIntegracaoTicketLog.ChaveAutorizacaoTicketLog = Request.GetStringParam("ChaveAutorizacaoTicketLog");
            configuracaoIntegracaoTicketLog.HorasConsultaTicketLog = Request.GetStringParam("HorasConsultaTicketLog");
            configuracaoIntegracaoTicketLog.ConfiguracaoAbastecimentoTicketLog = codigoConfigAbastecimentoTicketLog > 0 ? repConfigAbastecimento.BuscarPorCodigo(codigoConfigAbastecimentoTicketLog) : null;

            if (configuracaoIntegracaoTicketLog.Codigo > 0)
            {
                repConfiguracaoIntegracaoTicketLog.Atualizar(configuracaoIntegracaoTicketLog);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoTicketLog.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a TicketLog.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoTicketLog.Inserir(configuracaoIntegracaoTicketLog);
        }

        private void SalvarConfiguracaoIntegracaoEmillenium(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Emillenium))
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoEmilenium repositorioConfiguracaoIntegracaoEmile = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEmilenium(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmilenium configuracaoIntegracaoEmilenum = repositorioConfiguracaoIntegracaoEmile.BuscarConfiguracaoPadrao();

            if (configuracaoIntegracaoEmilenum == null)
                configuracaoIntegracaoEmilenum = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmilenium();
            else
                configuracaoIntegracaoEmilenum.Initialize();

            configuracaoIntegracaoEmilenum.TransIdInicioBuscaMassiva = Request.GetIntParam("TransIdInicioBuscaMassiva");
            configuracaoIntegracaoEmilenum.TransIdFimBuscaMassiva = Request.GetIntParam("TransIdFimBuscaMassiva");
            if (configuracaoIntegracaoEmilenum.TransIdInicioBuscaMassiva > 0)
                configuracaoIntegracaoEmilenum.DataFinalizacaoBuscaMassiva = null;

            if (configuracaoIntegracaoEmilenum.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoEmile.Atualizar(configuracaoIntegracaoEmilenum);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoEmilenum.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração Emillenium busca massiva.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoEmile.Inserir(configuracaoIntegracaoEmilenum);
        }

        private void SalvarConfiguracoesIntegracaoGNRE(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.GNRE))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoGNRE repositorioConfiguracaoIntegracaoGNRE = new Repositorio.Embarcador.Configuracoes.IntegracaoGNRE(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGNRE configuracaoIntegracaoGNRE = repositorioConfiguracaoIntegracaoGNRE.Buscar();

            if (configuracaoIntegracaoGNRE == null)
                configuracaoIntegracaoGNRE = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGNRE();
            else
                configuracaoIntegracaoGNRE.Initialize();

            configuracaoIntegracaoGNRE.PossuiIntegracaoGNRE = Request.GetBoolParam("PossuiIntegracaoGNRE");
            configuracaoIntegracaoGNRE.URLIntegracaoGNRE = Request.GetStringParam("URLIntegracaoGNRE");
            configuracaoIntegracaoGNRE.UsuarioIntegracaoGNRE = Request.GetStringParam("UsuarioIntegracaoGNRE");
            configuracaoIntegracaoGNRE.SenhaIntegracaoGNRE = Request.GetStringParam("SenhaIntegracaoGNRE");


            if (configuracaoIntegracaoGNRE.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoGNRE.Atualizar(configuracaoIntegracaoGNRE);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoGNRE.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o GNRE.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoGNRE.Inserir(configuracaoIntegracaoGNRE);
        }

        private void SalvarConfiguracoesIntegracaoLogRisk(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.LogRisk))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoLogRisk repositorioConfiguracaoIntegracaoLogRisk = new Repositorio.Embarcador.Configuracoes.IntegracaoLogRisk(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogRisk configuracaoIntegracaoLogRisk = repositorioConfiguracaoIntegracaoLogRisk.Buscar();

            if (configuracaoIntegracaoLogRisk == null)
                configuracaoIntegracaoLogRisk = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogRisk();
            else
                configuracaoIntegracaoLogRisk.Initialize();

            configuracaoIntegracaoLogRisk.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoLogRisk");
            configuracaoIntegracaoLogRisk.Usuario = Request.GetStringParam("UsuarioLogRisk");
            configuracaoIntegracaoLogRisk.Senha = Request.GetStringParam("SenhaLogRisk");
            configuracaoIntegracaoLogRisk.Dominio = Request.GetStringParam("DominioLogRisk");
            configuracaoIntegracaoLogRisk.CNPJCliente = Request.GetStringParam("CNPJClienteLogRisk");


            if (configuracaoIntegracaoLogRisk.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoLogRisk.Atualizar(configuracaoIntegracaoLogRisk);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoLogRisk.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o LogRisk.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoLogRisk.Inserir(configuracaoIntegracaoLogRisk);
        }

        private void SalvarConfiguracoesIntegracaoDigitalCom(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.DigitalCom))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom repConfiguracaoIntegracaoDigitalCom = new Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom configuracaoIntegracaoIntegracaoDigitalCom = repConfiguracaoIntegracaoDigitalCom.Buscar();

            if (configuracaoIntegracaoIntegracaoDigitalCom == null)
                configuracaoIntegracaoIntegracaoDigitalCom = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom();
            else
                configuracaoIntegracaoIntegracaoDigitalCom.Initialize();

            configuracaoIntegracaoIntegracaoDigitalCom.ValidacaoTAGDigitalCom = Request.GetBoolParam("ValidacaoTAGDigitalCom");
            configuracaoIntegracaoIntegracaoDigitalCom.EndpointDigitalCom = Request.GetStringParam("EndpointDigitalCom");
            configuracaoIntegracaoIntegracaoDigitalCom.TokenDigitalCom = Request.GetStringParam("TokenDigitalCom");
            configuracaoIntegracaoIntegracaoDigitalCom.CNPJLogin = Request.GetStringParam("CNPJLogin").ObterSomenteNumeros();
            configuracaoIntegracaoIntegracaoDigitalCom.UsuarioDigitalCom = Request.GetStringParam("UsuarioDigitalCom");
            configuracaoIntegracaoIntegracaoDigitalCom.SenhaDigitalCom = Request.GetStringParam("SenhaDigitalCom");
            configuracaoIntegracaoIntegracaoDigitalCom.UrlAutenticacaoDigitalCom = Request.GetStringParam("UrlAutenticacaoDigitalCom");
            configuracaoIntegracaoIntegracaoDigitalCom.TipoObtencaoCNPJTransportadora = Request.GetEnumParam<TipoObtencaoCNPJTransportadora>("TipoObtencaoCNPJTransportadora");


            if (configuracaoIntegracaoIntegracaoDigitalCom.Codigo > 0)
            {
                repConfiguracaoIntegracaoDigitalCom.Atualizar(configuracaoIntegracaoIntegracaoDigitalCom);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoIntegracaoDigitalCom.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a DigitalCom.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoDigitalCom.Inserir(configuracaoIntegracaoIntegracaoDigitalCom);
        }

        private void SalvarConfiguracoesIntegracaoLBC(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.LBC))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoLBC repConfiguracaoIntegracaoLBC = new Repositorio.Embarcador.Configuracoes.IntegracaoLBC(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC configuracaoIntegracaoIntegracaoLBC = repConfiguracaoIntegracaoLBC.Buscar();

            if (configuracaoIntegracaoIntegracaoLBC == null)
                configuracaoIntegracaoIntegracaoLBC = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC();
            else
                configuracaoIntegracaoIntegracaoLBC.Initialize();

            configuracaoIntegracaoIntegracaoLBC.PossuiIntegracaoLBC = Request.GetBoolParam("PossuiIntegracaoLBC");
            configuracaoIntegracaoIntegracaoLBC.URLIntegracaoLBCAnexo = Request.GetStringParam("URLIntegracaoLBCAnexo");
            configuracaoIntegracaoIntegracaoLBC.URLIntegracaoLBCCustoFixo = Request.GetStringParam("URLIntegracaoLBCCustoFixo");
            configuracaoIntegracaoIntegracaoLBC.URLIntegracaoLBCTabelaFreteCliente = Request.GetStringParam("URLIntegracaoLBCTabelaFreteCliente");
            configuracaoIntegracaoIntegracaoLBC.URLIntegracaoLBC = Request.GetStringParam("URLIntegracaoLBC");
            configuracaoIntegracaoIntegracaoLBC.UsuarioLBC = Request.GetStringParam("UsuarioLBC");
            configuracaoIntegracaoIntegracaoLBC.SenhaLBC = Request.GetStringParam("SenhaLBC");


            if (configuracaoIntegracaoIntegracaoLBC.Codigo > 0)
            {
                repConfiguracaoIntegracaoLBC.Atualizar(configuracaoIntegracaoIntegracaoLBC);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoIntegracaoLBC.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a LBC.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoLBC.Inserir(configuracaoIntegracaoIntegracaoLBC);
        }

        private void SalvarConfiguracoesTecnorisk(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Tecnorisk))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoTecnorisk repConfiguracaoIntegracaoTecnorisk = new Repositorio.Embarcador.Configuracoes.IntegracaoTecnorisk(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTecnorisk configuracaoIntegracaoTecnorisk = repConfiguracaoIntegracaoTecnorisk.Buscar();

            if (configuracaoIntegracaoTecnorisk == null)
                configuracaoIntegracaoTecnorisk = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTecnorisk();
            else
                configuracaoIntegracaoTecnorisk.Initialize();

            configuracaoIntegracaoTecnorisk.PossuiIntegracaoTecnorisk = Request.GetBoolParam("PossuiIntegracaoTecnorisk");
            configuracaoIntegracaoTecnorisk.URLIntegracaoTecnorisk = Request.GetStringParam("URLIntegracaoTecnorisk");
            configuracaoIntegracaoTecnorisk.IDPGR = Request.GetIntParam("IdPGR");
            configuracaoIntegracaoTecnorisk.IDPropriedadeMonitoramento = Request.GetIntParam("IdPropriedadeMonitoramento");
            configuracaoIntegracaoTecnorisk.UsuarioTecnorisk = Request.GetStringParam("UsuarioTecnorisk");
            configuracaoIntegracaoTecnorisk.SenhaTecnorisk = Request.GetStringParam("SenhaTecnorisk");
            configuracaoIntegracaoTecnorisk.CargaMercadoria = Request.GetIntParam("CargaMercadoria");


            if (configuracaoIntegracaoTecnorisk.Codigo > 0)
            {
                repConfiguracaoIntegracaoTecnorisk.Atualizar(configuracaoIntegracaoTecnorisk);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoTecnorisk.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Tecnorisk.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoTecnorisk.Inserir(configuracaoIntegracaoTecnorisk);
        }

        private void SalvarConfiguracoesDestinadosSAP(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.DestinadosSAP))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoDestinadosSAP repConfiguracaoIntegracaoDestinadosSAP = new Repositorio.Embarcador.Configuracoes.IntegracaoDestinadosSAP(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP configuracaoIntegracaoDestinadosSAP = repConfiguracaoIntegracaoDestinadosSAP.Buscar();

            if (configuracaoIntegracaoDestinadosSAP == null)
                configuracaoIntegracaoDestinadosSAP = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP();
            else
                configuracaoIntegracaoDestinadosSAP.Initialize();

            configuracaoIntegracaoDestinadosSAP.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoDestinadosSAP");
            configuracaoIntegracaoDestinadosSAP.ClientIDIntegracao = Request.GetStringParam("ClientIDIntegracaoDestinadosSAP");
            configuracaoIntegracaoDestinadosSAP.ClientSecretIntegracao = Request.GetStringParam("ClientSecretIntegracaoDestinadosSAP");
            configuracaoIntegracaoDestinadosSAP.URLIntegracaoXML = Request.GetStringParam("URLIntegracaoXMLDestinadosSAP");
            configuracaoIntegracaoDestinadosSAP.URLIntegracaoStatus = Request.GetStringParam("URLIntegracaoStatusDestinadosSAP");

            if (configuracaoIntegracaoDestinadosSAP.Codigo > 0)
            {
                repConfiguracaoIntegracaoDestinadosSAP.Atualizar(configuracaoIntegracaoDestinadosSAP);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoDestinadosSAP.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração de envio de documentos destinados.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoDestinadosSAP.Inserir(configuracaoIntegracaoDestinadosSAP);
        }

        private void SalvarConfiguracoesNeokohm(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Neokohm))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoNeokohm repConfiguracaoIntegracaoNeokohm = new Repositorio.Embarcador.Configuracoes.IntegracaoNeokohm(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm configuracaoIntegracaoNeokohm = repConfiguracaoIntegracaoNeokohm.Buscar();

            if (configuracaoIntegracaoNeokohm == null)
                configuracaoIntegracaoNeokohm = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm();
            else
                configuracaoIntegracaoNeokohm.Initialize();

            configuracaoIntegracaoNeokohm.PossuiIntegracaoNeokohm = Request.GetBoolParam("PossuiIntegracaoNeokohm");
            configuracaoIntegracaoNeokohm.URLIntegracaoNeokohm = Request.GetStringParam("URLIntegracaoNeokohm");
            configuracaoIntegracaoNeokohm.TokenNeokohm = Request.GetStringParam("TokenNeokohm");


            if (configuracaoIntegracaoNeokohm.Codigo > 0)
            {
                repConfiguracaoIntegracaoNeokohm.Atualizar(configuracaoIntegracaoNeokohm);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoNeokohm.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Neokohm.", unitOfWork);
            }
            else
                repConfiguracaoIntegracaoNeokohm.Inserir(configuracaoIntegracaoNeokohm);
        }

        private void SalvarConfiguracoesMoniloc(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Moniloc))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoMoniloc repConfiguracaoIntegracaoMoniloc = new Repositorio.Embarcador.Configuracoes.IntegracaoMoniloc(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMoniloc configuracaoIntegracaoMoniloc = repConfiguracaoIntegracaoMoniloc.Buscar();

            if (configuracaoIntegracaoMoniloc == null)
                configuracaoIntegracaoMoniloc = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMoniloc();
            else
                configuracaoIntegracaoMoniloc.Initialize();

            configuracaoIntegracaoMoniloc.PossuiIntegracaoMoniloc = Request.GetBoolParam("PossuiIntegracaoMoniloc");
            configuracaoIntegracaoMoniloc.UsuarioFTP = Request.GetStringParam("UsuarioFTPMoniloc");
            configuracaoIntegracaoMoniloc.SenhaFTP = Request.GetStringParam("SenhaFTPMoniloc");
            configuracaoIntegracaoMoniloc.PortaFTP = Request.GetStringParam("PortaFTPMoniloc");
            configuracaoIntegracaoMoniloc.DiretorioConsumoCargasDiarias = Request.GetStringParam("DiretorioConsumoCargasDiariasMoniloc");
            configuracaoIntegracaoMoniloc.DiretorioConsumo = Request.GetStringParam("DiretorioConsumoMoniloc");
            configuracaoIntegracaoMoniloc.DiretorioEnvioCVA = Request.GetStringParam("DiretorioEnvioCVAMoniloc");
            configuracaoIntegracaoMoniloc.DiretorioRetornoCVA = Request.GetStringParam("DiretorioRetornoCVAMoniloc");

            configuracaoIntegracaoMoniloc.FTPPassivo = Request.GetBoolParam("FTPPassivoMoniloc");
            configuracaoIntegracaoMoniloc.SFTP = Request.GetBoolParam("SFPTMoniloc");
            configuracaoIntegracaoMoniloc.SSL = Request.GetBoolParam("SSLMoniloc");
            configuracaoIntegracaoMoniloc.HostFTP = Request.GetStringParam("HostFTPMoniloc");


            if (configuracaoIntegracaoMoniloc.Codigo > 0)
            {
                repConfiguracaoIntegracaoMoniloc.Atualizar(configuracaoIntegracaoMoniloc);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoMoniloc.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Moniloc.", unitOfWork);
            }
            else
                repConfiguracaoIntegracaoMoniloc.Inserir(configuracaoIntegracaoMoniloc);
        }

        private async Task SalvarConfiguracoesApisulLogAsync(Integracao configuracaoIntegracao, UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ApisulLog))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoApisulLog repositorioConfiguracaoIntegracaoApisulLog = new Repositorio.Embarcador.Configuracoes.IntegracaoApisulLog(unitOfWork, cancellationToken);
            IntegracaoApisulLog configuracaoIntegracaoApisulLog = await repositorioConfiguracaoIntegracaoApisulLog.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoApisulLog == null)
                configuracaoIntegracaoApisulLog = new IntegracaoApisulLog();

            configuracaoIntegracaoApisulLog.PossuiIntegracaoApisulLog = Request.GetBoolParam("PossuiIntegracaoApisulLog");
            configuracaoIntegracaoApisulLog.URLIntegracaoApisulLog = Request.GetStringParam("URLIntegracaoApisulLog");
            configuracaoIntegracaoApisulLog.URLIntegracaoApisulLogEventos = Request.GetStringParam("URLIntegracaoApisulLogEventosApisulLog");
            configuracaoIntegracaoApisulLog.Token = Request.GetStringParam("TokenApisulLog");
            configuracaoIntegracaoApisulLog.CNPJEmbarcador = Request.GetStringParam("CNPJEmbarcadorApisulLog");
            configuracaoIntegracaoApisulLog.NaoUtilizarRastreadores = Request.GetBoolParam("NaoUtilizarRastreadoresApisulLog");
            configuracaoIntegracaoApisulLog.EtapaCarga = Request.GetEnumParam<SituacaoCarga>("EtapaCarga");
            configuracaoIntegracaoApisulLog.EnviarCodigoIntegracaoAbaCodigosIntegracaoTipoDeCarga = Request.GetBoolParam("EnviarCodigoIntegracaoAbaCodigosIntegracaoTipoDeCargaApisulLog");
            configuracaoIntegracaoApisulLog.ConcatenarCodigoIntegracaoDoClienteCidadeEstado = Request.GetBoolParam("ConcatenarCodigoIntegracaoDoClienteCidadeEstadoApisulLog");
            configuracaoIntegracaoApisulLog.ConcatenarCodigoIntegracaoTransporteOridemEDestino = Request.GetBoolParam("ConcatenarCodigoIntegracaoTransporteOridemEDestinoApisulLog");
            configuracaoIntegracaoApisulLog.OrigemDataInicioViagem = Request.GetEnumParam<OrigemDataInicioViagem>("OrigemDataInicioViagem");
            configuracaoIntegracaoApisulLog.ValorCargaOrigem = Request.GetDecimalParam("ValorCargaOrigemApisulLog");
            configuracaoIntegracaoApisulLog.TipoCarga = Request.GetStringParam("TipoCargaApisulLog");
            configuracaoIntegracaoApisulLog.IdentificadorUnicoViagem = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Integracao.ApiSulLog.IdentificadorUnicoViagem>("IdentificadorUnicoViagemApisulLog");

            if (configuracaoIntegracaoApisulLog.Codigo > 0)
            {
                await repositorioConfiguracaoIntegracaoApisulLog.AtualizarAsync(configuracaoIntegracaoApisulLog);

                await Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadasAsync(Auditado, configuracaoIntegracao, configuracaoIntegracaoApisulLog.GetChanges(), "Alterou a configuração de integração com a ApisulLog.", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update, cancellationToken);

                return;
            }

            await repositorioConfiguracaoIntegracaoApisulLog.InserirAsync(configuracaoIntegracaoApisulLog);
        }

        private void SalvarConfiguracoesFroggr(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Froggr))
                return;
            Repositorio.Embarcador.Configuracoes.IntegracaoFroggr repIntegracaoFroggr = new Repositorio.Embarcador.Configuracoes.IntegracaoFroggr(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFroggr IntegracaoFroggr = repIntegracaoFroggr.BuscarPrimeiroRegistro();


            if (IntegracaoFroggr == null)
                IntegracaoFroggr = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFroggr();

            IntegracaoFroggr.PossuiIntegracaoFroggr = Request.GetBoolParam("PossuiIntegracaoFroggr");
            IntegracaoFroggr.URLIntegracaoFroggr = Request.GetStringParam("URLIntegracaoFroggr");
            IntegracaoFroggr.UsuarioIntegracaoFroggr = Request.GetStringParam("UsuarioIntegracaoFroggr");
            IntegracaoFroggr.SenhaIntegracaoFroggr = Request.GetStringParam("SenhaIntegracaoFroggr");

            if (IntegracaoFroggr.Codigo > 0)
            {
                repIntegracaoFroggr.Atualizar(IntegracaoFroggr);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = IntegracaoFroggr?.GetChanges();

                if (alteracoes != null && alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Froggr.", unitOfWork);
            }
            else
                repIntegracaoFroggr.Inserir(IntegracaoFroggr);
        }
        private void SalvarConfiguracoesSAP(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SAP))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSAP repConfiguracaoIntegracaoSAP = new Repositorio.Embarcador.Configuracoes.IntegracaoSAP(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP configuracaoIntegracaoSAP = repConfiguracaoIntegracaoSAP.Buscar();

            if (configuracaoIntegracaoSAP == null)
                configuracaoIntegracaoSAP = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP();
            else
                configuracaoIntegracaoSAP.Initialize();

            configuracaoIntegracaoSAP.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoSAP");
            configuracaoIntegracaoSAP.URL = Request.GetStringParam("URLSAP");
            configuracaoIntegracaoSAP.URLEnviaVendaFrete = Request.GetStringParam("URLEnviaVendaFrete");
            configuracaoIntegracaoSAP.URLDescontoAvaria = Request.GetStringParam("URLDescontoAvaria");
            configuracaoIntegracaoSAP.URLSolicitacaoCancelamento = Request.GetStringParam("URLSolicitacaoCancelamento");
            configuracaoIntegracaoSAP.URLSolicitacaoCancelamentoCTe = Request.GetStringParam("URLSolicitacaoCancelamentoCTe");
            configuracaoIntegracaoSAP.Usuario = Request.GetStringParam("UsuarioSAP");
            configuracaoIntegracaoSAP.Senha = Request.GetStringParam("SenhaSAP");
            configuracaoIntegracaoSAP.RealizarIntegracaoComDadosFatura = Request.GetBoolParam("RealizarIntegracaoComDadosFatura");
            configuracaoIntegracaoSAP.URLIntegracaoFatura = Request.GetStringParam("URLIntegracaoFatura");
            configuracaoIntegracaoSAP.URLCriarSaldoFrete = Request.GetStringParam("URLCriarSaldoFrete");
            configuracaoIntegracaoSAP.URLConsultaDocumentos = Request.GetStringParam("URLConsultaDocumentos");
            configuracaoIntegracaoSAP.URLConsultaFatura = Request.GetStringParam("URLConsultaFatura");
            configuracaoIntegracaoSAP.URLIntegracaoEstornoFatura = Request.GetStringParam("URLIntegracaoEstornoFatura");
            configuracaoIntegracaoSAP.URLConsultaEstornoFatura = Request.GetStringParam("URLConsultaEstornoFatura");
            configuracaoIntegracaoSAP.URLEnviaVendaServicoNFSe = Request.GetStringParam("URLEnviaVendaServicoNFSe");

            if (configuracaoIntegracaoSAP.Codigo > 0)
            {
                repConfiguracaoIntegracaoSAP.Atualizar(configuracaoIntegracaoSAP);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoSAP.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o SAP.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoSAP.Inserir(configuracaoIntegracaoSAP);
        }

        private async Task SalvarConfiguracoesSAP_API4Async(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SAP_API4))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSapAPI4 repConfiguracaoIntegracaoSAP = new Repositorio.Embarcador.Configuracoes.IntegracaoSapAPI4(unidadeDeTrabalho, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSapAPI4 configuracaoIntegracaoSAP = await repConfiguracaoIntegracaoSAP.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoSAP == null)
                configuracaoIntegracaoSAP = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSapAPI4();
            else
                configuracaoIntegracaoSAP.Initialize();

            configuracaoIntegracaoSAP.PossuiIntegracaoSAP_API4 = Request.GetBoolParam("PossuiIntegracaoSAP_API4");
            configuracaoIntegracaoSAP.URLSAP_API4 = Request.GetStringParam("URLSAP_API4");
            configuracaoIntegracaoSAP.UsuarioSAP_API4 = Request.GetStringParam("UsuarioSAP_API4");
            configuracaoIntegracaoSAP.SenhaSAP_API4 = Request.GetStringParam("SenhaSAP_API4");

            if (configuracaoIntegracaoSAP.Codigo > 0)
            {
                await repConfiguracaoIntegracaoSAP.AtualizarAsync(configuracaoIntegracaoSAP);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoSAP.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o SAP API4.", unidadeDeTrabalho);
            }
            else
                await repConfiguracaoIntegracaoSAP.InserirAsync(configuracaoIntegracaoSAP);
        }

        private void SalvarConfiguracoesYPE(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.YPE))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoYPE repConfiguracaoIntegracaoYPE = new Repositorio.Embarcador.Configuracoes.IntegracaoYPE(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYPE configuracaoIntegracaoYPE = repConfiguracaoIntegracaoYPE.BuscarPrimeiroRegistro();
            if (configuracaoIntegracaoYPE == null)
                configuracaoIntegracaoYPE = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYPE();
            else
                configuracaoIntegracaoYPE.Initialize();

            configuracaoIntegracaoYPE.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoYPE");
            configuracaoIntegracaoYPE.URLintegracao = Request.GetStringParam("UrlintegracaoYpe");
            configuracaoIntegracaoYPE.URLIntegracaoOcorrencia = Request.GetStringParam("URLIntegracaoOcorrencia");
            configuracaoIntegracaoYPE.Usuario = Request.GetStringParam("UsuarioYPE");
            configuracaoIntegracaoYPE.Senha = Request.GetStringParam("SenhaYPE");
            configuracaoIntegracaoYPE.URLintegracaoRecebeDadosLaudo = Request.GetStringParam("URLintegracaoRecebeDadosLaudo");

            if (configuracaoIntegracaoYPE.Codigo > 0)
            {
                repConfiguracaoIntegracaoYPE.Atualizar(configuracaoIntegracaoYPE);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoYPE.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o SAP.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoYPE.Inserir(configuracaoIntegracaoYPE);
        }

        private void SalvarConfiguracoesOTM(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.OTM))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoOTM repositorioIntegracaoOTM = new Repositorio.Embarcador.Configuracoes.IntegracaoOTM(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOTM integracaoOTM = repositorioIntegracaoOTM.BuscarPrimeiroRegistro();

            if (integracaoOTM == null)
                integracaoOTM = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOTM();
            else
                integracaoOTM.Initialize();

            integracaoOTM.PossuiIntegracaoOTM = Request.GetBoolParam("PossuiIntegracaoOTM");
            integracaoOTM.ClientIDOTM = Request.GetStringParam("ClientIDOTM");
            integracaoOTM.ClientSecretOTM = Request.GetStringParam("ClientSecretOTM");
            integracaoOTM.URLIntegracaoLeilaoOTM = Request.GetStringParam("URLIntegracaoLeilaoOTM");

            if (integracaoOTM.Codigo > 0)
            {
                repositorioIntegracaoOTM.Atualizar(integracaoOTM);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = integracaoOTM.GetChanges();

                if ((alteracoes?.Count ?? 0) > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração da OTM.", unitOfWork);
            }
            else
                repositorioIntegracaoOTM.Inserir(integracaoOTM);
        }

        private void SalvarConfiguracoesSIC(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SIC))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSIC repConfiguracaoIntegracaoSIC = new Repositorio.Embarcador.Configuracoes.IntegracaoSIC(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC = repConfiguracaoIntegracaoSIC.Buscar();

            if (configuracaoIntegracaoSIC == null)
                configuracaoIntegracaoSIC = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC();
            else
                configuracaoIntegracaoSIC.Initialize();

            configuracaoIntegracaoSIC.PossuiIntegracaoSIC = Request.GetBoolParam("PossuiIntegracaoSIC");
            configuracaoIntegracaoSIC.RealizarIntegracaoNovosCadastrosPessoaSIC = Request.GetBoolParam("RealizarIntegracaoNovosCadastrosPessoaSIC");
            configuracaoIntegracaoSIC.URLIntegracaoSIC = Request.GetStringParam("URLIntegracaoSIC");
            configuracaoIntegracaoSIC.LoginSIC = Request.GetStringParam("LoginSIC");
            configuracaoIntegracaoSIC.SenhaSIC = Request.GetStringParam("SenhaSIC");
            configuracaoIntegracaoSIC.TipoCadastroVeiculoSIC = Request.GetStringParam("TipoCadastroVeiculoSIC");
            configuracaoIntegracaoSIC.TipoCadastroMotoristaSIC = Request.GetStringParam("TipoCadastroMotoristaSIC");
            configuracaoIntegracaoSIC.TipoCadastroClientesSIC = Request.GetStringParam("TipoCadastroClientesSIC");
            configuracaoIntegracaoSIC.TipoCadastroClientesTerceirosSIC = Request.GetBoolParam("TipoCadastroClientesTerceirosSIC");
            configuracaoIntegracaoSIC.TipoCadastroTransportadoresTerceirosSIC = Request.GetStringParam("TipoCadastroTransportadoresTerceirosSIC");
            configuracaoIntegracaoSIC.EmpresaSIC = Request.GetStringParam("EmpresaSIC");

            if (configuracaoIntegracaoSIC.Codigo > 0)
            {
                repConfiguracaoIntegracaoSIC.Atualizar(configuracaoIntegracaoSIC);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoSIC.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a SIC.", unitOfWork);
            }
            else
                repConfiguracaoIntegracaoSIC.Inserir(configuracaoIntegracaoSIC);
        }

        private void SalvarConfiguracoesIntegracaoFrimesa(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Frimesa))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa repositorioConfiguracaoIntegracaoFrimesa = new Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa configuracaoIntegracaoFrimesa = repositorioConfiguracaoIntegracaoFrimesa.Buscar();

            if (configuracaoIntegracaoFrimesa == null)
                configuracaoIntegracaoFrimesa = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa();
            else
                configuracaoIntegracaoFrimesa.Initialize();

            configuracaoIntegracaoFrimesa.URLContabilizacao = Request.GetStringParam("URLContabilizacaoFrimesa");
            configuracaoIntegracaoFrimesa.Usuario = Request.GetStringParam("UsuarioFrimesa");
            configuracaoIntegracaoFrimesa.Senha = Request.GetStringParam("SenhaFrimesa");
            configuracaoIntegracaoFrimesa.ClientID = Request.GetStringParam("ClientID");
            configuracaoIntegracaoFrimesa.ClientSecret = Request.GetStringParam("ClientSecret");
            configuracaoIntegracaoFrimesa.AccessToken = Request.GetStringParam("AccessToken");
            configuracaoIntegracaoFrimesa.Scope = Request.GetStringParam("Scope");
            configuracaoIntegracaoFrimesa.Situacao = Request.GetBoolParam("Situacao");
            configuracaoIntegracaoFrimesa.TipoIntegracaoOAuth = Request.GetEnumParam<TipoIntegracaoOAuth>("TipoIntegracaoOAuth");

            if (configuracaoIntegracaoFrimesa.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoFrimesa.Atualizar(configuracaoIntegracaoFrimesa);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoFrimesa.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Frimesa.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoFrimesa.Inserir(configuracaoIntegracaoFrimesa);
        }

        private void SalvarConfiguracoesLoggi(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Loggi))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoLoggi repositorioConfiguracaoIntegracaoLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoLoggi(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi configuracaoIntegracaoLoggi = repositorioConfiguracaoIntegracaoLoggi.Buscar();

            if (configuracaoIntegracaoLoggi == null)
                configuracaoIntegracaoLoggi = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi();
            else
                configuracaoIntegracaoLoggi.Initialize();

            configuracaoIntegracaoLoggi.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoLoggi");
            configuracaoIntegracaoLoggi.URLIntegracao = Request.GetStringParam("URLIntegracaoLoggi");
            configuracaoIntegracaoLoggi.UrlIntegracaoCTe = Request.GetStringParam("UrlIntegracaoCTeLoggi");
            configuracaoIntegracaoLoggi.ClientID = Request.GetStringParam("ClientIDLoggi");
            configuracaoIntegracaoLoggi.ClientSecret = Request.GetStringParam("ClientSecretLoggi");
            configuracaoIntegracaoLoggi.Token = Request.GetStringParam("TokenLoggi");
            configuracaoIntegracaoLoggi.URLConsultaPacotes = Request.GetStringParam("URLConsultaPacotes");
            configuracaoIntegracaoLoggi.Scope = Request.GetStringParam("ScopeLoggi");
            configuracaoIntegracaoLoggi.URLIntegracaoEventoEntrega = Request.GetStringParam("URLIntegracaoEventoEntrega");
            configuracaoIntegracaoLoggi.URLAutenticacaoEventoEntrega = Request.GetStringParam("URLAutenticacaoEventoEntrega");

            if (configuracaoIntegracaoLoggi.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoLoggi.Atualizar(configuracaoIntegracaoLoggi);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoLoggi.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Loggi.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoLoggi.Inserir(configuracaoIntegracaoLoggi);
        }

        private void SalvarConfiguracoesCTePagamentoLoggi(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.CTePagamentoLoggi))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi repositorioConfiguracaoIntegracaoCTePagamentoLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi configuracaoIntegracaoCTePagamentoLoggi = repositorioConfiguracaoIntegracaoCTePagamentoLoggi.Buscar();

            if (configuracaoIntegracaoCTePagamentoLoggi == null)
                configuracaoIntegracaoCTePagamentoLoggi = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi();
            else
                configuracaoIntegracaoCTePagamentoLoggi.Initialize();

            configuracaoIntegracaoCTePagamentoLoggi.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoCTePagamentoLoggi");
            configuracaoIntegracaoCTePagamentoLoggi.URLAutenticacao = Request.GetStringParam("URLAutenticacaoCTePagamentoLoggi");
            configuracaoIntegracaoCTePagamentoLoggi.ClientID = Request.GetStringParam("ClientIDCTePagamentoLoggi");
            configuracaoIntegracaoCTePagamentoLoggi.ClientSecret = Request.GetStringParam("ClientSecretCTePagamentoLoggi");
            configuracaoIntegracaoCTePagamentoLoggi.Token = Request.GetStringParam("TokenCTePagamentoLoggi");
            configuracaoIntegracaoCTePagamentoLoggi.URLEnvioDocumentos = Request.GetStringParam("URLEnvioDocumentosCTePagamentoLoggi");
            configuracaoIntegracaoCTePagamentoLoggi.Scope = Request.GetStringParam("ScopeCTePagamentoLoggi");
            configuracaoIntegracaoCTePagamentoLoggi.IntegrarCTeSubstituto = Request.GetBoolParam("IntegrarCTeSubstitutoCTePagamentoLoggi");

            if (configuracaoIntegracaoCTePagamentoLoggi.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoCTePagamentoLoggi.Atualizar(configuracaoIntegracaoCTePagamentoLoggi);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoCTePagamentoLoggi.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração de CT-e Pagamentos com a Loggi.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoCTePagamentoLoggi.Inserir(configuracaoIntegracaoCTePagamentoLoggi);
        }


        private void SalvarConfiguracoesValoresCTeLoggi(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ValoresCTeLoggi))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi repIntegracaoValoresCTeLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi configuracaoIntegracaoValoresCTeLoggi = repIntegracaoValoresCTeLoggi.Buscar();

            if (configuracaoIntegracaoValoresCTeLoggi == null)
                configuracaoIntegracaoValoresCTeLoggi = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoValoresCTeLoggi();
            else
                configuracaoIntegracaoValoresCTeLoggi.Initialize();

            configuracaoIntegracaoValoresCTeLoggi.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoValoresCTeLoggi");
            configuracaoIntegracaoValoresCTeLoggi.URLAutenticacao = Request.GetStringParam("URLAutenticacaoValoresCTeLoggi");
            configuracaoIntegracaoValoresCTeLoggi.ClientID = Request.GetStringParam("ClientIDValoresCTeLoggi");
            configuracaoIntegracaoValoresCTeLoggi.ClientSecret = Request.GetStringParam("ClientSecretValoresCTeLoggi");
            configuracaoIntegracaoValoresCTeLoggi.Token = Request.GetStringParam("TokenValoresCTeLoggi");
            configuracaoIntegracaoValoresCTeLoggi.URLEnvioDocumentos = Request.GetStringParam("URLEnvioDocumentosValoresCTeLoggi");
            configuracaoIntegracaoValoresCTeLoggi.Scope = Request.GetStringParam("ScopeValoresCTeLoggi");

            if (configuracaoIntegracaoValoresCTeLoggi.Codigo > 0)
            {
                repIntegracaoValoresCTeLoggi.Atualizar(configuracaoIntegracaoValoresCTeLoggi);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoValoresCTeLoggi.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração de Valores CT-e Loggi.", unidadeDeTrabalho);
            }
            else
                repIntegracaoValoresCTeLoggi.Inserir(configuracaoIntegracaoValoresCTeLoggi);
        }

        private void SalvarConfiguracoesCTeAnterioresLoggi(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.CTeAnterioresLoggi))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi repositorioIntegracaoCTeAnterioresLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi configuracaoIntegracaoValoresCTeLoggi = repositorioIntegracaoCTeAnterioresLoggi.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoValoresCTeLoggi == null)
                configuracaoIntegracaoValoresCTeLoggi = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi();
            else
                configuracaoIntegracaoValoresCTeLoggi.Initialize();

            configuracaoIntegracaoValoresCTeLoggi.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoCTeAnterioresLoggi");
            configuracaoIntegracaoValoresCTeLoggi.URLAutenticacao = Request.GetStringParam("URLAutenticacaoCTeAnterioresLoggi");
            configuracaoIntegracaoValoresCTeLoggi.ClientID = Request.GetStringParam("ClientIDCTeAnterioresLoggi");
            configuracaoIntegracaoValoresCTeLoggi.ClientSecret = Request.GetStringParam("ClientSecretCTeAnterioresLoggi");
            configuracaoIntegracaoValoresCTeLoggi.Scope = Request.GetStringParam("ScopeCTeAnterioresLoggi");
            configuracaoIntegracaoValoresCTeLoggi.URLEnvioDocumentos = Request.GetStringParam("URLEnvioDocumentosCTeAnterioresLoggi");

            if (configuracaoIntegracaoValoresCTeLoggi.Codigo > 0)
            {
                repositorioIntegracaoCTeAnterioresLoggi.Atualizar(configuracaoIntegracaoValoresCTeLoggi);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoValoresCTeLoggi.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração de CT-e Anteriores Loggi.", unidadeDeTrabalho);
            }
            else
                repositorioIntegracaoCTeAnterioresLoggi.Inserir(configuracaoIntegracaoValoresCTeLoggi);
        }

        private void SalvarConfiguracoesJJ(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.JJ))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoJJ repositorioConfiguracaoIntegracaoJJ = new Repositorio.Embarcador.Configuracoes.IntegracaoJJ(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoJJ configuracaoIntegracaoJJ = repositorioConfiguracaoIntegracaoJJ.Buscar();

            if (configuracaoIntegracaoJJ == null)
                configuracaoIntegracaoJJ = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoJJ();
            else
                configuracaoIntegracaoJJ.Initialize();

            configuracaoIntegracaoJJ.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoJJ");
            configuracaoIntegracaoJJ.URLIntegracaoAtendimento = Request.GetStringParam("URLIntegracaoAtendimentoJJ");
            configuracaoIntegracaoJJ.Usuario = Request.GetStringParam("UsuarioJJ");
            configuracaoIntegracaoJJ.Senha = Request.GetStringParam("SenhaJJ");

            if (configuracaoIntegracaoJJ.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoJJ.Atualizar(configuracaoIntegracaoJJ);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoJJ.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a JJ.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoJJ.Inserir(configuracaoIntegracaoJJ);
        }

        private void SalvarConfiguracoesIntegracaoKlios(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Klios))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoKlios repositorioConfiguracaoIntegracaoKlios = new Repositorio.Embarcador.Configuracoes.IntegracaoKlios(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKlios configuracaoIntegracaoKlios = repositorioConfiguracaoIntegracaoKlios.Buscar();

            if (configuracaoIntegracaoKlios == null)
                configuracaoIntegracaoKlios = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKlios();
            else
                configuracaoIntegracaoKlios.Initialize();

            configuracaoIntegracaoKlios.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoKlios");
            configuracaoIntegracaoKlios.URLAutenticacao = Request.GetStringParam("URLAutenticacaoKlios");
            configuracaoIntegracaoKlios.Usuario = Request.GetStringParam("UsuarioKlios");
            configuracaoIntegracaoKlios.Senha = Request.GetStringParam("SenhaKlios");
            configuracaoIntegracaoKlios.URLConsultaAnaliseConjunto = Request.GetStringParam("URLConsultaAnaliseConjuntoKlios");

            if (configuracaoIntegracaoKlios.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoKlios.Atualizar(configuracaoIntegracaoKlios);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoKlios.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Klios.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoKlios.Inserir(configuracaoIntegracaoKlios);
        }

        private void SalvarConfiguracoesIntegracaoSAPV9(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SAP_V9))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9 repositorioConfiguracaoIntegracaoSAPV9 = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPV9 configuracaoIntegracaoSAPV9 = repositorioConfiguracaoIntegracaoSAPV9.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoSAPV9 == null)
                configuracaoIntegracaoSAPV9 = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPV9();
            else
                configuracaoIntegracaoSAPV9.Initialize();

            configuracaoIntegracaoSAPV9.PossuiIntegracaoSAPV9 = Request.GetBoolParam("PossuiIntegracaoSAPV9");
            configuracaoIntegracaoSAPV9.URLReciboFrete = Request.GetStringParam("URLReciboFrete");
            configuracaoIntegracaoSAPV9.Usuario = Request.GetStringParam("Usuario");
            configuracaoIntegracaoSAPV9.Senha = Request.GetStringParam("Senha");
            configuracaoIntegracaoSAPV9.URLCancelamento = Request.GetStringParam("URLCancelamento");

            if (configuracaoIntegracaoSAPV9.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoSAPV9.Atualizar(configuracaoIntegracaoSAPV9);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoSAPV9.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o SAP V9.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoSAPV9.Inserir(configuracaoIntegracaoSAPV9);
        }

        private void SalvarConfiguracoesIntegracaoSAPST(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SAP_ST))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSAPST repositorioConfiguracaoIntegracaoSAPST = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPST(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPST configuracaoIntegracaoSAPST = repositorioConfiguracaoIntegracaoSAPST.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoSAPST == null)
                configuracaoIntegracaoSAPST = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPST();
            else
                configuracaoIntegracaoSAPST.Initialize();

            configuracaoIntegracaoSAPST.PossuiIntegracaoSAPST = Request.GetBoolParam("PossuiIntegracaoSAPST");
            configuracaoIntegracaoSAPST.URLCriarAtendimento = Request.GetStringParam("URLCriarAtendimento");
            configuracaoIntegracaoSAPST.Usuario = Request.GetStringParam("Usuario");
            configuracaoIntegracaoSAPST.Senha = Request.GetStringParam("Senha");
            configuracaoIntegracaoSAPST.URLCancelamentoST = Request.GetStringParam("URLCancelamentoST");


            if (configuracaoIntegracaoSAPST.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoSAPST.Atualizar(configuracaoIntegracaoSAPST);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoSAPST.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o SAP ST.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoSAPST.Inserir(configuracaoIntegracaoSAPST);
        }

        private void SalvarConfiguracoesIntegracaoBrado(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Brado))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoBrado repositorioConfiguracaoIntegracaoBrado = new Repositorio.Embarcador.Configuracoes.IntegracaoBrado(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado configuracaoIntegracaoBrado = repositorioConfiguracaoIntegracaoBrado.Buscar();

            if (configuracaoIntegracaoBrado == null)
                configuracaoIntegracaoBrado = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBrado();
            else
                configuracaoIntegracaoBrado.Initialize();

            configuracaoIntegracaoBrado.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoBrado");
            configuracaoIntegracaoBrado.URLAutenticacao = Request.GetStringParam("URLAutenticacaoBrado");
            configuracaoIntegracaoBrado.Usuario = Request.GetStringParam("UsuarioBrado");
            configuracaoIntegracaoBrado.Senha = Request.GetStringParam("SenhaBrado");
            configuracaoIntegracaoBrado.CodigoGestao = Request.GetStringParam("CodigoGestaoBrado");
            configuracaoIntegracaoBrado.URLEnvioDadosTransporte = Request.GetStringParam("URLEnvioDadosTransporteBrado");
            configuracaoIntegracaoBrado.URLEnvioDocumentosEmitidos = Request.GetStringParam("URLEnvioDocumentosEmitidosBrado");
            configuracaoIntegracaoBrado.URLCancelamentoBrado = Request.GetStringParam("URLCancelamentoBrado");

            if (configuracaoIntegracaoBrado.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoBrado.Atualizar(configuracaoIntegracaoBrado);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoBrado.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Brado.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoBrado.Inserir(configuracaoIntegracaoBrado);
        }

        private void SalvarConfiguracoesIntegracaoEFrete(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.EFrete))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete repIntegracaoEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoEFrete = repIntegracaoEFrete.Buscar();

            if (configuracaoIntegracaoEFrete == null)
                configuracaoIntegracaoEFrete = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete();
            else
                configuracaoIntegracaoEFrete.Initialize();

            configuracaoIntegracaoEFrete.CodigoIntegracaoRecebivel = Request.GetStringParam("CodigoIntegracaoRecebivelEFrete");
            configuracaoIntegracaoEFrete.UsuarioRecebivel = Request.GetStringParam("UsuarioRecebivelEFrete");
            configuracaoIntegracaoEFrete.URLCancelamentoRecebivel = Request.GetStringParam("URLCancelamentoRecebivel");
            configuracaoIntegracaoEFrete.URLPagamentoRecebivel = Request.GetStringParam("URLPagamentoRecebivel");
            configuracaoIntegracaoEFrete.SenhaRecebivel = Request.GetStringParam("SenhaRecebivelEFrete");
            configuracaoIntegracaoEFrete.URLRecebivel = Request.GetStringParam("URLRecebivelEFrete");
            configuracaoIntegracaoEFrete.URLAutenticacao = Request.GetStringParam("URLAutenticacaoEFrete");
            configuracaoIntegracaoEFrete.APIKey = Request.GetStringParam("APIKeyEFrete");
            configuracaoIntegracaoEFrete.PossuiIntegracaoRecebivel = Request.GetBoolParam("PossuiIntegracaoRecebivelEFrete");
            configuracaoIntegracaoEFrete.EnviarImpostosNaIntegracaoDoCIOT = Request.GetBoolParam("EnviarImpostosNaIntegracaoDoCIOT");
            configuracaoIntegracaoEFrete.VersaoEFrete = Request.GetEnumParam("VersaoEFrete", VersaoEFreteEnum.Versao1);
            configuracaoIntegracaoEFrete.DeduzirImpostosValorTotalFrete = Request.GetBoolParam("DeduzirImpostosValorTotalFrete");
            configuracaoIntegracaoEFrete.EnviarDadosRegulatorioANTT = Request.GetBoolParam("EnviarDadosRegulatorioANTT");
            configuracaoIntegracaoEFrete.ConsultarTagAoIncluirVeiculoNaCarga = Request.GetBoolParam("ConsultarTagAoIncluirVeiculoNaCarga");

            if (configuracaoIntegracaoEFrete.Codigo > 0)
            {
                repIntegracaoEFrete.Atualizar(configuracaoIntegracaoEFrete);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoEFrete.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a e-Frete.", unidadeDeTrabalho);
            }
            else
                repIntegracaoEFrete.Inserir(configuracaoIntegracaoEFrete);
        }

        private void SalvarConfiguracoesIntegracaoOpenTech(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.OpenTech))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoGeralOpenTech repIntegracaoOpenTech = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralOpenTech(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralOpenTech configuracaoIntegracaoOpenTech = repIntegracaoOpenTech.Buscar();

            if (configuracaoIntegracaoOpenTech == null)
                configuracaoIntegracaoOpenTech = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralOpenTech();
            else
                configuracaoIntegracaoOpenTech.Initialize();

            configuracaoIntegracaoOpenTech.EnviarDataNFeNaDataPrevistaOpentech = Request.GetBoolParam("EnviarDataNFeNaDataPrevistaOpentech");
            configuracaoIntegracaoOpenTech.ConsiderarLocalidadeProdutoIntegracaoEntrega = Request.GetBoolParam("ConsiderarLocalidadeProdutoIntegracaoEntrega");

            if (configuracaoIntegracaoOpenTech.Codigo > 0)
            {
                repIntegracaoOpenTech.Atualizar(configuracaoIntegracaoOpenTech);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoOpenTech.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a OpenTech.", unidadeDeTrabalho);
            }
            else
                repIntegracaoOpenTech.Inserir(configuracaoIntegracaoOpenTech);
        }

        private void SalvarConfiguracoesIntegracaoEShip(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Eship))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoEShip repConfiguracaoIntegracaoEShip = new Repositorio.Embarcador.Configuracoes.IntegracaoEShip(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship configuracaoIntegracaoEShip = repConfiguracaoIntegracaoEShip.Buscar();

            if (configuracaoIntegracaoEShip == null)
                configuracaoIntegracaoEShip = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship();
            else
                configuracaoIntegracaoEShip.Initialize();

            configuracaoIntegracaoEShip.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoEship");
            configuracaoIntegracaoEShip.URLComunicacao = Request.GetStringParam("URLComunicacaoEship");
            configuracaoIntegracaoEShip.ApiToken = Request.GetStringParam("ApiKeyEship");

            if (configuracaoIntegracaoEShip.Codigo > 0)
            {
                repConfiguracaoIntegracaoEShip.Atualizar(configuracaoIntegracaoEShip);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoEShip.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a e-Ship.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoEShip.Inserir(configuracaoIntegracaoEShip);
        }

        private void SalvarConfiguracoesIntegracaoYandeh(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Yandeh))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoYandeh repConfiguracaoIntegracaoYandeh = new Repositorio.Embarcador.Configuracoes.IntegracaoYandeh(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYandeh configuracaoIntegracaoYandeh = repConfiguracaoIntegracaoYandeh.Buscar();

            if (configuracaoIntegracaoYandeh == null)
                configuracaoIntegracaoYandeh = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYandeh();
            else
                configuracaoIntegracaoYandeh.Initialize();

            configuracaoIntegracaoYandeh.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoYandeh");
            configuracaoIntegracaoYandeh.URLComunicacao = Request.GetStringParam("URLComunicacaoYandeh");
            configuracaoIntegracaoYandeh.Usuario = Request.GetStringParam("UsuarioYandeh");
            configuracaoIntegracaoYandeh.Senha = Request.GetStringParam("SenhaYandeh");

            if (configuracaoIntegracaoYandeh.Codigo > 0)
            {
                repConfiguracaoIntegracaoYandeh.Atualizar(configuracaoIntegracaoYandeh);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoYandeh.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com Yandeh.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoYandeh.Inserir(configuracaoIntegracaoYandeh);
        }

        private void SalvarConfiguracoesDiageo(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Diageo))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoDiageo repConfiguracaoIntegracaoDiageo = new Repositorio.Embarcador.Configuracoes.IntegracaoDiageo(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDiageo configuracaoIntegracaoDiageo = repConfiguracaoIntegracaoDiageo.Buscar();

            if (configuracaoIntegracaoDiageo == null)
                configuracaoIntegracaoDiageo = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDiageo();
            else
                configuracaoIntegracaoDiageo.Initialize();

            configuracaoIntegracaoDiageo.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoDiageo");
            configuracaoIntegracaoDiageo.Endereco = Request.GetStringParam("EnderecoDiageo");
            configuracaoIntegracaoDiageo.Usuario = Request.GetStringParam("UsuarioDiageo");
            configuracaoIntegracaoDiageo.Senha = Request.GetStringParam("SenhaDiageo");
            configuracaoIntegracaoDiageo.Porta = Request.GetStringParam("PortaDiageo");
            configuracaoIntegracaoDiageo.Outbound = Request.GetStringParam("OutboundDiageo");
            configuracaoIntegracaoDiageo.DiretorioInbound = Request.GetStringParam("DiretorioInboundDiageo");
            configuracaoIntegracaoDiageo.Passivo = Request.GetBoolParam("PassivoDiageo");
            configuracaoIntegracaoDiageo.UtilizarSFTP = Request.GetBoolParam("UtilizarSFTPDiageo");
            configuracaoIntegracaoDiageo.SSL = Request.GetBoolParam("SSLDiageo");

            if (configuracaoIntegracaoDiageo.Codigo > 0)
            {
                repConfiguracaoIntegracaoDiageo.Atualizar(configuracaoIntegracaoDiageo);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoDiageo.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a Diageo.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoDiageo.Inserir(configuracaoIntegracaoDiageo);
        }

        private void SalvarConfiguracoesP44(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.P44))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoP44 repConfiguracaoIntegracaoP44 = new Repositorio.Embarcador.Configuracoes.IntegracaoP44(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoP44 configuracaoIntegracaoP44 = repConfiguracaoIntegracaoP44.Buscar();

            if (configuracaoIntegracaoP44 == null)
                configuracaoIntegracaoP44 = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoP44();
            else
                configuracaoIntegracaoP44.Initialize();

            configuracaoIntegracaoP44.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoP44");
            configuracaoIntegracaoP44.Usuario = Request.GetStringParam("UsuarioP44");
            configuracaoIntegracaoP44.Senha = Request.GetStringParam("SenhaP44");
            configuracaoIntegracaoP44.ClientId = Request.GetStringParam("ClientIdP44");
            configuracaoIntegracaoP44.ClientSecret = Request.GetStringParam("ClientSecretP44");
            configuracaoIntegracaoP44.URLAutenticacao = Request.GetStringParam("URLAutenticacaoP44");
            configuracaoIntegracaoP44.URLAplicacao = Request.GetStringParam("URLAplicacaoP44");
            configuracaoIntegracaoP44.URLIntegracaoPatio = Request.GetStringParam("URLIntegracaoPatioP44");

            if (configuracaoIntegracaoP44.Codigo > 0)
            {
                repConfiguracaoIntegracaoP44.Atualizar(configuracaoIntegracaoP44);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoP44.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a P44.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoP44.Inserir(configuracaoIntegracaoP44);
        }

        private void SalvarConfiguracoesIntegracaoBalancaKIKI(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.BalancaKIKI))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoBalancaKIKI repConfiguracaoIntegracaoBalancaKIKI = new Repositorio.Embarcador.Configuracoes.IntegracaoBalancaKIKI(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBalancaKIKI configuracaoIntegracaoBalancaKIKI = repConfiguracaoIntegracaoBalancaKIKI.Buscar();

            if (configuracaoIntegracaoBalancaKIKI == null)
                configuracaoIntegracaoBalancaKIKI = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBalancaKIKI();
            else
                configuracaoIntegracaoBalancaKIKI.Initialize();

            configuracaoIntegracaoBalancaKIKI.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoBalancaKIKI");
            configuracaoIntegracaoBalancaKIKI.URL = Request.GetStringParam("URLBalancaKIKI");

            if (configuracaoIntegracaoBalancaKIKI.Codigo > 0)
            {
                repConfiguracaoIntegracaoBalancaKIKI.Atualizar(configuracaoIntegracaoBalancaKIKI);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoBalancaKIKI.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Balança KIKI.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoBalancaKIKI.Inserir(configuracaoIntegracaoBalancaKIKI);
        }

        private void SalvarConfiguracoesIntegracaoComprovei(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Comprovei))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoComprovei repositorioConfiguracaoIntegracaoComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei configuracaoIntegracaoComprovei = repositorioConfiguracaoIntegracaoComprovei.Buscar();

            if (configuracaoIntegracaoComprovei == null)
                configuracaoIntegracaoComprovei = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei();
            else
                configuracaoIntegracaoComprovei.Initialize();

            configuracaoIntegracaoComprovei.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoComprovei");
            configuracaoIntegracaoComprovei.URL = Request.GetStringParam("URLComprovei");
            configuracaoIntegracaoComprovei.Usuario = Request.GetStringParam("UsuarioComprovei");
            configuracaoIntegracaoComprovei.Senha = Request.GetStringParam("SenhaComprovei");
            configuracaoIntegracaoComprovei.URLBaseRest = Request.GetStringParam("URLBaseRestComprovei");
            configuracaoIntegracaoComprovei.URLIACanhoto = Request.GetStringParam("URLComproveiIACanhoto");
            configuracaoIntegracaoComprovei.UsuarioIACanhoto = Request.GetStringParam("UsuarioComproveiIACanhoto");
            configuracaoIntegracaoComprovei.SenhaIACanhoto = Request.GetStringParam("SenhaComproveiIACanhoto");
            configuracaoIntegracaoComprovei.PossuiIntegracaoIACanhoto = Request.GetBoolParam("PossuiIntegracaoIACanhoto");
            configuracaoIntegracaoComprovei.URLIntegracaoRetornoGerarCarregamento = Request.GetStringParam("URLIntegracaoRetornoGerarCarregamentoComprovei");
            configuracaoIntegracaoComprovei.URLIntegracaoRetornoConfirmacaoPedidos = Request.GetStringParam("URLIntegracaoRetornoConfirmacaoPedidosComprovei");
            configuracaoIntegracaoComprovei.URLIntegracaoRetornoEnviarDigitalizacaoCanhoto = Request.GetStringParam("URLIntegracaoRetornoEnviarDigitalizacaoCanhotosComprovei");

            if (configuracaoIntegracaoComprovei.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoComprovei.Atualizar(configuracaoIntegracaoComprovei);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoComprovei.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Comprovei.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoComprovei.Inserir(configuracaoIntegracaoComprovei);

        }

        private void SalvarConfiguracoesIntegracaoComproveiRota(Integracao configuracaoIntegracao, UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ComproveiRota))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoComproveiRota repositorioConfiguracaoIntegracaoComproveiRota = new Repositorio.Embarcador.Configuracoes.IntegracaoComproveiRota(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComproveiRota configuracaoIntegracaoComproveiRota = repositorioConfiguracaoIntegracaoComproveiRota.Buscar();

            if (configuracaoIntegracaoComproveiRota == null)
                configuracaoIntegracaoComproveiRota = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComproveiRota();
            else
                configuracaoIntegracaoComproveiRota.Initialize();

            configuracaoIntegracaoComproveiRota.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoComproveiRota");
            configuracaoIntegracaoComproveiRota.URL = Request.GetStringParam("URLComproveiRota");
            configuracaoIntegracaoComproveiRota.URLBaseRest = Request.GetStringParam("URLBaseRestComproveiRota");
            configuracaoIntegracaoComproveiRota.Usuario = Request.GetStringParam("UsuarioComproveiRota");
            configuracaoIntegracaoComproveiRota.Senha = Request.GetStringParam("SenhaComproveiRota");

            if (configuracaoIntegracaoComproveiRota.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoComproveiRota.Atualizar(configuracaoIntegracaoComproveiRota);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoComproveiRota.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Comprovei Rota.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoComproveiRota.Inserir(configuracaoIntegracaoComproveiRota);

        }

        private void SalvarConfiguracoesIntegracaoKMM(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.KMM))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repConfiguracaoIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repConfiguracaoIntegracaoKMM.Buscar();

            if (configuracaoIntegracaoKMM == null)
                configuracaoIntegracaoKMM = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM();
            else
                configuracaoIntegracaoKMM.Initialize();

            configuracaoIntegracaoKMM.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoKMM");
            configuracaoIntegracaoKMM.URL = Request.GetStringParam("URLKMM");
            configuracaoIntegracaoKMM.Usuario = Request.GetStringParam("UsuarioKMM");
            configuracaoIntegracaoKMM.Senha = Request.GetStringParam("SenhaKMM");
            configuracaoIntegracaoKMM.CodGestao = Request.GetIntParam("CodGestaoKMM");
            configuracaoIntegracaoKMM.TokenTimeHours = Request.GetIntParam("TokenTimeHoursKMM");


            if (configuracaoIntegracaoKMM.Codigo > 0)
            {
                repConfiguracaoIntegracaoKMM.Atualizar(configuracaoIntegracaoKMM);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoKMM.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a KMM.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoKMM.Inserir(configuracaoIntegracaoKMM);
        }

        private void SalvarConfiguracoesIntegracaoLogvett(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Logvett))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoLogvett repConfiguracaoIntegracaoLogvett = new Repositorio.Embarcador.Configuracoes.IntegracaoLogvett(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogvett configuracaoIntegracaoLogvett = repConfiguracaoIntegracaoLogvett.Buscar();

            if (configuracaoIntegracaoLogvett == null)
                configuracaoIntegracaoLogvett = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogvett();
            else
                configuracaoIntegracaoLogvett.Initialize();

            configuracaoIntegracaoLogvett.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoLogvett");
            configuracaoIntegracaoLogvett.Usuario = Request.GetStringParam("UsuarioLogvett");
            configuracaoIntegracaoLogvett.Senha = Request.GetStringParam("SenhaLogvett");
            configuracaoIntegracaoLogvett.URLTituloPagar = Request.GetStringParam("URLTituloPagarLogvett");
            configuracaoIntegracaoLogvett.URLBaixarTitulo = Request.GetStringParam("URLBaixarTituloLogvett");

            if (configuracaoIntegracaoLogvett.Codigo > 0)
            {
                repConfiguracaoIntegracaoLogvett.Atualizar(configuracaoIntegracaoLogvett);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoLogvett.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a KMM.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoLogvett.Inserir(configuracaoIntegracaoLogvett);
        }

        private void SalvarConfiguracoesIntegracaoFlora(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Flora))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoFlora repConfiguracaoIntegracaoFlora = new Repositorio.Embarcador.Configuracoes.IntegracaoFlora(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFlora configuracaoIntegracaoFlora = repConfiguracaoIntegracaoFlora.Buscar();

            if (configuracaoIntegracaoFlora == null)
                configuracaoIntegracaoFlora = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFlora();
            else
                configuracaoIntegracaoFlora.Initialize();

            configuracaoIntegracaoFlora.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoFlora");
            configuracaoIntegracaoFlora.EnvioCarga = Request.GetStringParam("EnvioCargaFlora");
            configuracaoIntegracaoFlora.Usuario = Request.GetStringParam("UsuarioFlora");
            configuracaoIntegracaoFlora.Senha = Request.GetStringParam("SenhaFlora");
            configuracaoIntegracaoFlora.URL = Request.GetStringParam("URLFlora");
            configuracaoIntegracaoFlora.CodigoFretePrevisto = Request.GetStringParam("CodigoFretePrevistoFlora");
            configuracaoIntegracaoFlora.CodigoFreteConfirmado = Request.GetStringParam("CodigoFreteConfirmadoFlora");

            if (configuracaoIntegracaoFlora.Codigo > 0)
            {
                repConfiguracaoIntegracaoFlora.Atualizar(configuracaoIntegracaoFlora);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoFlora.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Flora.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoFlora.Inserir(configuracaoIntegracaoFlora);
        }

        private void SalvarConfiguracoesIntegracaoCalisto(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Calisto))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCalisto repConfiguracaoIntegracaoCalisto = new Repositorio.Embarcador.Configuracoes.IntegracaoCalisto(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCalisto configuracaoIntegracaoCalisto = repConfiguracaoIntegracaoCalisto.Buscar();

            if (configuracaoIntegracaoCalisto == null)
                configuracaoIntegracaoCalisto = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCalisto();
            else
                configuracaoIntegracaoCalisto.Initialize();

            configuracaoIntegracaoCalisto.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoCalisto");
            configuracaoIntegracaoCalisto.Usuario = Request.GetStringParam("UsuarioCalisto");
            configuracaoIntegracaoCalisto.Senha = Request.GetStringParam("SenhaCalisto");
            configuracaoIntegracaoCalisto.URL = Request.GetStringParam("URLCalisto");
            configuracaoIntegracaoCalisto.URLContabilizacao = Request.GetStringParam("URLContabilizacao");

            if (configuracaoIntegracaoCalisto.Codigo > 0)
            {
                repConfiguracaoIntegracaoCalisto.Atualizar(configuracaoIntegracaoCalisto);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoCalisto.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Calisto.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoCalisto.Inserir(configuracaoIntegracaoCalisto);
        }

        private void SalvarConfiguracoesIntegracaoTrizy(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Trizy))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repConfiguracaoIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repConfiguracaoIntegracaoTrizy.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoTrizy == null)
                configuracaoIntegracaoTrizy = new IntegracaoTrizy();

            configuracaoIntegracaoTrizy.URLEnvioCarga = Request.GetStringParam("URLEnvioCarga");
            configuracaoIntegracaoTrizy.URLEnvioCancelamentoCarga = Request.GetStringParam("URLEnvioCancelamentoCarga");
            configuracaoIntegracaoTrizy.URLEnvioEventosPatio = Request.GetStringParam("URLEnvioEventosPatio");
            configuracaoIntegracaoTrizy.ValidarIntegracaoPorOperacao = Request.GetBoolParam("ValidarIntegracaoTrizyPorOperacao");
            configuracaoIntegracaoTrizy.IntegrarApenasCargasComControleDeEntrega = configuracaoIntegracaoTrizy.ValidarIntegracaoPorOperacao && Request.GetBoolParam("TrizyIntegrarApenasCargasComControleDeEntrega");
            configuracaoIntegracaoTrizy.PermitirIntegrarMultiplasCargasParaOMesmoMotorista = Request.GetBoolParam("TrizyPermitirIntegrarMultiplasCargasParaOMesmoMotorista");
            configuracaoIntegracaoTrizy.TokenEnvioMS = Request.GetStringParam("TokenEnvioMS");
            configuracaoIntegracaoTrizy.TipoDocumentoPais = Request.GetEnumParam<TipoDocumentoPaisTrizy>("TrizyTipoDocumentoPais", TipoDocumentoPaisTrizy.Brasil);

            configuracaoIntegracaoTrizy.EnviarPDFDocumentosFiscais = Request.GetBoolParam("TrizyEnviarPDFDocumentosFiscais");
            configuracaoIntegracaoTrizy.DocumentosFiscaisEnvioPDF = new List<DocumentosFiscaisTrizy>();
            configuracaoIntegracaoTrizy.DiasIntervaloTracking = Request.GetIntParam("DiasIntervaloTracking", 30);
            configuracaoIntegracaoTrizy.EnviarPatchAtualizacoesEntrega = Request.GetBoolParam("EnviarPatchAtualizacoesEntrega");
            configuracaoIntegracaoTrizy.EnviarNomeFantasiaQuandoPossuir = Request.GetBoolParam("EnviarNomeFantasiaQuandoPossuir");
            configuracaoIntegracaoTrizy.VersaoIntegracao = Request.GetEnumParam<VersaoIntegracaoTrizy>("VersaoIntegracaoTrizy", VersaoIntegracaoTrizy.Versao1);
            configuracaoIntegracaoTrizy.IntegrarOfertasCargas = Request.GetNullableBoolParam("IntegrarOfertasCargas");
            configuracaoIntegracaoTrizy.URLIntegracaoOfertas = Request.GetStringParam("URLIntegracaoOfertas");
            configuracaoIntegracaoTrizy.URLIntegracaoGrupoMotoristas = Request.GetStringParam("URLIntegracaoGrupoMotoristas");

            List<DocumentosFiscaisTrizy> listaDocumentosFiscaisEnvioPDF = Request.GetNullableListParam<DocumentosFiscaisTrizy>("TrizyDocumentosFiscaisEnvioPDF");
            if (configuracaoIntegracaoTrizy.EnviarPDFDocumentosFiscais && (listaDocumentosFiscaisEnvioPDF?.Count ?? 0) > 0)
            {
                foreach (DocumentosFiscaisTrizy codigoDocumentosFiscaisEnvioPDF in listaDocumentosFiscaisEnvioPDF)
                    configuracaoIntegracaoTrizy.DocumentosFiscaisEnvioPDF.Add(codigoDocumentosFiscaisEnvioPDF);
            }

            if (configuracaoIntegracaoTrizy.Codigo > 0)
            {
                repConfiguracaoIntegracaoTrizy.Atualizar(configuracaoIntegracaoTrizy);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoTrizy.GetChanges();

                if (alteracoes != null && alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracaoTrizy, alteracoes, "Alterou a configuração de integração com a Trizy.", unitOfWork);

            }
            else
                repConfiguracaoIntegracaoTrizy.Inserir(configuracaoIntegracaoTrizy);

        }

        private void SalvarConfiguracoesIntegracaoObramax(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Obramax))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoObramax repConfiguracaoIntegracaoObramax = new Repositorio.Embarcador.Configuracoes.IntegracaoObramax(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramax configuracaoIntegracaoObramax = repConfiguracaoIntegracaoObramax.Buscar();

            if (configuracaoIntegracaoObramax == null)
                configuracaoIntegracaoObramax = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramax();
            else
                configuracaoIntegracaoObramax.Initialize();

            configuracaoIntegracaoObramax.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoObramax");
            configuracaoIntegracaoObramax.Endpoint = Request.GetStringParam("EndpointObramax");
            configuracaoIntegracaoObramax.Token = Request.GetStringParam("TokenObramax");
            configuracaoIntegracaoObramax.EndpointPedidoOcorrencia = Request.GetStringParam("EndpointPedidoOcorrenciaObramax");
            configuracaoIntegracaoObramax.CodigoEventoCanhoto = Request.GetIntParam("CodigoEventoCanhotoObramax");


            if (configuracaoIntegracaoObramax.Codigo > 0)
            {
                repConfiguracaoIntegracaoObramax.Atualizar(configuracaoIntegracaoObramax);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoObramax.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Obramax.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoObramax.Inserir(configuracaoIntegracaoObramax);
        }

        private void SalvarConfiguracoesIntegracaoObramaxCTE(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ObramaxCTE))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoObramaxCTE repConfiguracaoIntegracaoObramaxCTE = new Repositorio.Embarcador.Configuracoes.IntegracaoObramaxCTE(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxCTE configuracaoIntegracaoObramaxCTE = repConfiguracaoIntegracaoObramaxCTE.Buscar();

            if (configuracaoIntegracaoObramaxCTE == null)
                configuracaoIntegracaoObramaxCTE = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxCTE();
            else
                configuracaoIntegracaoObramaxCTE.Initialize();

            configuracaoIntegracaoObramaxCTE.PossuiIntegracaoObramaxCTE = Request.GetBoolParam("PossuiIntegracaoObramaxCTE");
            configuracaoIntegracaoObramaxCTE.EndpointObramaxCTE = Request.GetStringParam("EndpointObramaxCTE");
            configuracaoIntegracaoObramaxCTE.TokenObramaxCTE = Request.GetStringParam("TokenObramaxCTE");


            if (configuracaoIntegracaoObramaxCTE.Codigo > 0)
            {
                repConfiguracaoIntegracaoObramaxCTE.Atualizar(configuracaoIntegracaoObramaxCTE);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoObramaxCTE.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Obramax.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoObramaxCTE.Inserir(configuracaoIntegracaoObramaxCTE);
        }

        private void SalvarConfiguracoesIntegracaoObramaxNFE(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ObramaxNFE))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoObramaxNFE repConfiguracaoIntegracaoObramaxNFE = new Repositorio.Embarcador.Configuracoes.IntegracaoObramaxNFE(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxNFE configuracaoIntegracaoObramaxNFE = repConfiguracaoIntegracaoObramaxNFE.Buscar();

            if (configuracaoIntegracaoObramaxNFE == null)
                configuracaoIntegracaoObramaxNFE = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxNFE();
            else
                configuracaoIntegracaoObramaxNFE.Initialize();

            configuracaoIntegracaoObramaxNFE.PossuiIntegracaoObramaxNFE = Request.GetBoolParam("PossuiIntegracaoObramaxNFE");
            configuracaoIntegracaoObramaxNFE.EndpointObramaxNFE = Request.GetStringParam("EndpointObramaxNFE");
            configuracaoIntegracaoObramaxNFE.TokenObramaxNFE = Request.GetStringParam("TokenObramaxNFE");


            if (configuracaoIntegracaoObramaxNFE.Codigo > 0)
            {
                repConfiguracaoIntegracaoObramaxNFE.Atualizar(configuracaoIntegracaoObramaxNFE);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoObramaxNFE.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Obramax.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoObramaxNFE.Inserir(configuracaoIntegracaoObramaxNFE);
        }

        private void SalvarConfiguracoesIntegracaoObramaxProvisao(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ObramaxProvisao))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoObramaxProvisao repConfiguracaoIntegracaoObramaxProvisao = new Repositorio.Embarcador.Configuracoes.IntegracaoObramaxProvisao(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxProvisao configuracaoIntegracaoObramaxProvisao = repConfiguracaoIntegracaoObramaxProvisao.Buscar();

            if (configuracaoIntegracaoObramaxProvisao == null)
                configuracaoIntegracaoObramaxProvisao = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxProvisao();
            else
                configuracaoIntegracaoObramaxProvisao.Initialize();

            configuracaoIntegracaoObramaxProvisao.PossuiIntegracaoObramaxProvisao = Request.GetBoolParam("PossuiIntegracaoObramaxProvisao");
            configuracaoIntegracaoObramaxProvisao.EndpointObramaxProvisao = Request.GetStringParam("EndpointObramaxProvisao");
            configuracaoIntegracaoObramaxProvisao.TokenObramaxProvisao = Request.GetStringParam("TokenObramaxProvisao");

            if (configuracaoIntegracaoObramaxProvisao.Codigo > 0)
            {
                repConfiguracaoIntegracaoObramaxProvisao.Atualizar(configuracaoIntegracaoObramaxProvisao);

                Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, configuracaoIntegracao, configuracaoIntegracaoObramaxProvisao.GetChanges(), "Alterou a configuração de integração com a Obramax.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoObramaxProvisao.Inserir(configuracaoIntegracaoObramaxProvisao);
        }

        private void SalvarConfiguracoesIntegracaoShopee(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Shopee))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoShopee repConfiguracaoIntegracaoShopee = new Repositorio.Embarcador.Configuracoes.IntegracaoShopee(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoShopee configuracaoIntegracaoShopee = repConfiguracaoIntegracaoShopee.Buscar();

            if (configuracaoIntegracaoShopee == null)
                configuracaoIntegracaoShopee = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoShopee();
            else
                configuracaoIntegracaoShopee.Initialize();

            configuracaoIntegracaoShopee.PossuiIntegracaoPacote = Request.GetBoolParam("PossuiIntegracaoPacoteShopee");
            configuracaoIntegracaoShopee.EndpointPacote = Request.GetStringParam("EndpointPacoteShopee");
            configuracaoIntegracaoShopee.Senha = Request.GetStringParam("SenhaShopee");
            configuracaoIntegracaoShopee.Usuario = Request.GetStringParam("UsuarioShopee");

            if (configuracaoIntegracaoShopee.Codigo > 0)
            {
                repConfiguracaoIntegracaoShopee.Atualizar(configuracaoIntegracaoShopee);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoShopee.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Shopee.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoShopee.Inserir(configuracaoIntegracaoShopee);
        }

        private void SalvarConfiguracoesIntegracaoItalac(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Italac))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoItalac repConfiguracaoIntegracaoItalac = new Repositorio.Embarcador.Configuracoes.IntegracaoItalac(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalac configuracaoIntegracaoItalac = repConfiguracaoIntegracaoItalac.Buscar();

            if (configuracaoIntegracaoItalac == null)
                configuracaoIntegracaoItalac = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalac();
            else
                configuracaoIntegracaoItalac.Initialize();

            configuracaoIntegracaoItalac.PossuiIntegracaoItalac = Request.GetBoolParam("PossuiIntegracaoItalac");
            configuracaoIntegracaoItalac.URLItalac = Request.GetStringParam("URLItalac");
            configuracaoIntegracaoItalac.SenhaItalac = Request.GetStringParam("SenhaItalac");
            configuracaoIntegracaoItalac.UsuarioItalac = Request.GetStringParam("UsuarioItalac");

            if (configuracaoIntegracaoItalac.Codigo > 0)
            {
                repConfiguracaoIntegracaoItalac.Atualizar(configuracaoIntegracaoItalac);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoItalac.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Italac.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoItalac.Inserir(configuracaoIntegracaoItalac);
        }

        private void SalvarConfiguracoesIntegracaoItalacFatura(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ItalacFaturas))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoItalacFatura repConfiguracaoIntegracaoItalacFatura = new Repositorio.Embarcador.Configuracoes.IntegracaoItalacFatura(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalacFatura configuracaoIntegracaoItalacFatura = repConfiguracaoIntegracaoItalacFatura.Buscar();

            if (configuracaoIntegracaoItalacFatura == null)
                configuracaoIntegracaoItalacFatura = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalacFatura();
            else
                configuracaoIntegracaoItalacFatura.Initialize();

            configuracaoIntegracaoItalacFatura.PossuiIntegracaoItalacFatura = Request.GetBoolParam("PossuiIntegracaoItalacFatura");
            configuracaoIntegracaoItalacFatura.URLItalacFatura = Request.GetStringParam("URLItalacFatura");
            configuracaoIntegracaoItalacFatura.SenhaItalacFatura = Request.GetStringParam("SenhaItalacFatura");
            configuracaoIntegracaoItalacFatura.UsuarioItalacFatura = Request.GetStringParam("UsuarioItalacFatura");

            if (configuracaoIntegracaoItalacFatura.Codigo > 0)
            {
                repConfiguracaoIntegracaoItalacFatura.Atualizar(configuracaoIntegracaoItalacFatura);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoItalacFatura.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Italac.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoItalacFatura.Inserir(configuracaoIntegracaoItalacFatura);
        }

        private void SalvarConfiguracoesIntegracaoPager(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Italac) && !_tipoIntegracaoExistentes.Contains(TipoIntegracao.Pager))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoPager repositorioIntegracaoPager = new Repositorio.Embarcador.Configuracoes.IntegracaoPager(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPager integracaoPager = repositorioIntegracaoPager.BuscarPrimeiroRegistro();

            if (integracaoPager == null)
                integracaoPager = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPager();
            else
                integracaoPager.Initialize();

            integracaoPager.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoPager");
            integracaoPager.URLIntegracao = Request.GetStringParam("URLIntegracaoPager");
            integracaoPager.Usuario = Request.GetStringParam("UsuarioPager");
            integracaoPager.Senha = Request.GetStringParam("SenhaPager");

            if (integracaoPager.Codigo > 0)
            {
                repositorioIntegracaoPager.Atualizar(integracaoPager);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = integracaoPager.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Pager.", unidadeDeTrabalho);

                return;
            }

            repositorioIntegracaoPager.Inserir(integracaoPager);
        }

        private void SalvarConfiguracoesIntegracaoElectrolux(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Electrolux))
                return;

            Repositorio.LayoutEDI repLayoutEDI = new LayoutEDI(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux repConfiguracaoIntegracaoElectrolux = new Repositorio.Embarcador.Configuracoes.IntegracaoElectrolux(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux configuracaoIntegracaoElectrolux = repConfiguracaoIntegracaoElectrolux.Buscar();


            if (configuracaoIntegracaoElectrolux == null)
                configuracaoIntegracaoElectrolux = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoElectrolux();
            else
                configuracaoIntegracaoElectrolux.Initialize();

            configuracaoIntegracaoElectrolux.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoElectrolux");
            configuracaoIntegracaoElectrolux.Usuario = Request.GetStringParam("UsuarioElectrolux");
            configuracaoIntegracaoElectrolux.Senha = Request.GetStringParam("SenhaElectrolux");
            configuracaoIntegracaoElectrolux.URLCONEMB = Request.GetStringParam("URLConembElectrolux");
            configuracaoIntegracaoElectrolux.URLOCORREN = Request.GetStringParam("URLOcorrenElectrolux");
            configuracaoIntegracaoElectrolux.UrlNotfisLista = Request.GetStringParam("URLNotfisListaElectrolux");
            configuracaoIntegracaoElectrolux.UrlNotfisDetalhada = Request.GetStringParam("URLNotfisDetalhadaElectrolux");

            if (configuracaoIntegracaoElectrolux.Codigo > 0)
            {
                repConfiguracaoIntegracaoElectrolux.Atualizar(configuracaoIntegracaoElectrolux);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoElectrolux.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Electrolux.", unidadeDeTrabalho);
            }
            else
                repConfiguracaoIntegracaoElectrolux.Inserir(configuracaoIntegracaoElectrolux);
        }

        private void SalvarConfiguracoesIntegracaoWhatsApp(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.WhatsApp))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoWhatsApp repositorioConfiguracaoIntegracaoWhatsApp = new Repositorio.Embarcador.Configuracoes.IntegracaoWhatsApp(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWhatsApp configuracaoIntegracaoWhatsApp = repositorioConfiguracaoIntegracaoWhatsApp.Buscar();

            if (configuracaoIntegracaoWhatsApp == null)
                configuracaoIntegracaoWhatsApp = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWhatsApp();
            else
                configuracaoIntegracaoWhatsApp.Initialize();

            configuracaoIntegracaoWhatsApp.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoWhatsApp");
            configuracaoIntegracaoWhatsApp.Token = Request.GetStringParam("TokenWhatsApp");
            configuracaoIntegracaoWhatsApp.IdContaWhatsAppBusiness = Request.GetStringParam("IdContaWhatsApp");
            configuracaoIntegracaoWhatsApp.IdNumeroTelefone = Request.GetStringParam("IdNumeroTelefoneWhatsApp");
            configuracaoIntegracaoWhatsApp.IdAplicativo = Request.GetStringParam("IdAplicativoWhatsApp");

            if (configuracaoIntegracaoWhatsApp.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoWhatsApp.Atualizar(configuracaoIntegracaoWhatsApp);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoWhatsApp.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o WhatsApp.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoWhatsApp.Inserir(configuracaoIntegracaoWhatsApp);
        }

        private void SalvarConfiguracoesIntegracaoBoticarioFreeFlow(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Boticario) && !_tipoIntegracaoExistentes.Contains(TipoIntegracao.Senior))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow repConfiguracaoIntegracaoBoticarioFreeFlow = new Repositorio.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow configIntegracaoBoticarioFreeFlow = repConfiguracaoIntegracaoBoticarioFreeFlow.Buscar();

            if (configIntegracaoBoticarioFreeFlow == null)
                configIntegracaoBoticarioFreeFlow = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow();
            else
                configIntegracaoBoticarioFreeFlow.Initialize();

            configIntegracaoBoticarioFreeFlow.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoBoticarioFreeFlow");
            configIntegracaoBoticarioFreeFlow.URLIntegracao = Request.GetStringParam("URLIntegracaoBoticarioFreeFlow");
            configIntegracaoBoticarioFreeFlow.URLAutenticacao = Request.GetStringParam("URLAutenticacaoBoticarioFreeFlow");
            configIntegracaoBoticarioFreeFlow.ClientSecret = Request.GetStringParam("ClientSecretBoticarioFreeFlow");
            configIntegracaoBoticarioFreeFlow.ClientId = Request.GetStringParam("ClientIdBoticarioFreeFlow");
            configIntegracaoBoticarioFreeFlow.URLConsultaAVIPED = Request.GetStringParam("URLConsultaAVIPED");

            if (configIntegracaoBoticarioFreeFlow.Codigo > 0)
            {
                repConfiguracaoIntegracaoBoticarioFreeFlow.Atualizar(configIntegracaoBoticarioFreeFlow);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configIntegracaoBoticarioFreeFlow.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o Boticario Free Flow.", unitOfWork);
            }
            else
                repConfiguracaoIntegracaoBoticarioFreeFlow.Inserir(configIntegracaoBoticarioFreeFlow);
        }

        private void SalvarConfiguracoesLoggiFaturas(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Loggi))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoLoggiFaturas repositorioConfiguracaoIntegracaoLoggiFaturas = new Repositorio.Embarcador.Configuracoes.IntegracaoLoggiFaturas(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggiFaturas configuracaoIntegracaoLoggiFaturas = repositorioConfiguracaoIntegracaoLoggiFaturas.Buscar();

            if (configuracaoIntegracaoLoggiFaturas == null)
                configuracaoIntegracaoLoggiFaturas = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggiFaturas();
            else
                configuracaoIntegracaoLoggiFaturas.Initialize();

            configuracaoIntegracaoLoggiFaturas.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoLoggiFaturas");
            configuracaoIntegracaoLoggiFaturas.URL = Request.GetStringParam("URLLoggiFaturas");
            configuracaoIntegracaoLoggiFaturas.Usuario = Request.GetStringParam("UsuarioLoggiFaturas");
            configuracaoIntegracaoLoggiFaturas.Senha = Request.GetStringParam("SenhaLoggiFaturas");
            configuracaoIntegracaoLoggiFaturas.NumeroMaterial = Request.GetStringParam("NumeroMaterialLoggiFaturas");

            if (configuracaoIntegracaoLoggiFaturas.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoLoggiFaturas.Atualizar(configuracaoIntegracaoLoggiFaturas);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoLoggiFaturas.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Loggi.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoLoggiFaturas.Inserir(configuracaoIntegracaoLoggiFaturas);
        }

        private void SalvarConfiguracoesRuntec(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Runtec))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoRuntec repositorioConfiguracaoIntegracaoRuntec = new Repositorio.Embarcador.Configuracoes.IntegracaoRuntec(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRuntec configuracaoIntegracaoRuntec = repositorioConfiguracaoIntegracaoRuntec.Buscar();

            if (configuracaoIntegracaoRuntec == null)
                configuracaoIntegracaoRuntec = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRuntec();
            else
                configuracaoIntegracaoRuntec.Initialize();

            configuracaoIntegracaoRuntec.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoRuntec");
            configuracaoIntegracaoRuntec.URL = Request.GetStringParam("URLRuntec");
            configuracaoIntegracaoRuntec.Usuario = Request.GetStringParam("UsuarioRuntec");
            configuracaoIntegracaoRuntec.Senha = Request.GetStringParam("SenhaRuntec");

            if (configuracaoIntegracaoRuntec.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoRuntec.Atualizar(configuracaoIntegracaoRuntec);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoRuntec.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Runtec.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoRuntec.Inserir(configuracaoIntegracaoRuntec);
        }

        private void SalvarConfiguracoesConfirmaFacil(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ConfirmaFacil))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoConfirmaFacil repositorioConfiguracaoIntegracaoConfirmaFacil = new Repositorio.Embarcador.Configuracoes.IntegracaoConfirmaFacil(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConfirmaFacil configuracaoIntegracaoConfirmaFacil = repositorioConfiguracaoIntegracaoConfirmaFacil.Buscar();

            if (configuracaoIntegracaoConfirmaFacil == null)
                configuracaoIntegracaoConfirmaFacil = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConfirmaFacil();
            else
                configuracaoIntegracaoConfirmaFacil.Initialize();

            configuracaoIntegracaoConfirmaFacil.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoConfirmaFacil");
            configuracaoIntegracaoConfirmaFacil.URL = Request.GetStringParam("URLConfirmaFacil");
            configuracaoIntegracaoConfirmaFacil.Email = Request.GetStringParam("EmailConfirmaFacil");
            configuracaoIntegracaoConfirmaFacil.Senha = Request.GetStringParam("SenhaConfirmaFacil");
            configuracaoIntegracaoConfirmaFacil.IDCliente = Request.GetStringParam("IDClienteConfirmaFacil");
            configuracaoIntegracaoConfirmaFacil.IDProduto = Request.GetStringParam("IDProdutoConfirmaFacil");

            if (configuracaoIntegracaoConfirmaFacil.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoConfirmaFacil.Atualizar(configuracaoIntegracaoConfirmaFacil);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoConfirmaFacil.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Confirma Fácil.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoConfirmaFacil.Inserir(configuracaoIntegracaoConfirmaFacil);
        }

        private void SalvarConfiguracoesBind(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Bind))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoBind repositorioIntegracaoBind = new Repositorio.Embarcador.Configuracoes.IntegracaoBind(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBind configuracaoIntegracaoBind = repositorioIntegracaoBind.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoBind == null)
                configuracaoIntegracaoBind = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBind();
            else
                configuracaoIntegracaoBind.Initialize();

            configuracaoIntegracaoBind.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoBind");
            configuracaoIntegracaoBind.URLIntegracao = Request.GetStringParam("URLIntegracaoBind");
            configuracaoIntegracaoBind.APIKeyIntegracao = Request.GetStringParam("APIKeyIntegracaoBind");

            if (configuracaoIntegracaoBind.Codigo > 0)
            {
                repositorioIntegracaoBind.Atualizar(configuracaoIntegracaoBind);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoBind.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Bind.", unidadeDeTrabalho);
            }
            else
                repositorioIntegracaoBind.Inserir(configuracaoIntegracaoBind);
        }

        private void SalvarConfiguracoesVector(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Vector))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoVector repositorioIntegracaoVector = new Repositorio.Embarcador.Configuracoes.IntegracaoVector(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVector configuracaoIntegracaoVector = repositorioIntegracaoVector.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoVector == null)
                configuracaoIntegracaoVector = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVector();
            else
                configuracaoIntegracaoVector.Initialize();

            configuracaoIntegracaoVector.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoVector");
            configuracaoIntegracaoVector.URLIntegracao = Request.GetStringParam("URLIntegracaoVector");
            configuracaoIntegracaoVector.ClientID = Request.GetStringParam("ClientIdIntegracaoVector");
            configuracaoIntegracaoVector.ClientSecret = Request.GetStringParam("ClientSecretIntegracaoVector");

            if (configuracaoIntegracaoVector.Codigo > 0)
            {
                repositorioIntegracaoVector.Atualizar(configuracaoIntegracaoVector);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoVector.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Vector.", unidadeDeTrabalho);

                return;
            }

            repositorioIntegracaoVector.Inserir(configuracaoIntegracaoVector);
        }

        private void SalvarConfiguracoesTrizyEventos(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.TrizyEventos))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrizyEventos repositorioIntegracaoTrizyEventos = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizyEventos(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizyEventos configuracaoIntegracaoTrizyEventos = repositorioIntegracaoTrizyEventos.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoTrizyEventos == null)
                configuracaoIntegracaoTrizyEventos = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizyEventos();
            else
                configuracaoIntegracaoTrizyEventos.Initialize();

            configuracaoIntegracaoTrizyEventos.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoTrizyEventos");
            configuracaoIntegracaoTrizyEventos.URLIntegracao = Request.GetStringParam("URLIntegracaoTrizyEventos");
            configuracaoIntegracaoTrizyEventos.Token = Request.GetStringParam("TokenIntegracaoTrizyEventos");

            if (configuracaoIntegracaoTrizyEventos.Codigo > 0)
            {
                repositorioIntegracaoTrizyEventos.Atualizar(configuracaoIntegracaoTrizyEventos);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoTrizyEventos.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Trizy Eventos.", unidadeDeTrabalho);

                return;
            }

            repositorioIntegracaoTrizyEventos.Inserir(configuracaoIntegracaoTrizyEventos);
        }

        private void SalvarConfiguracoesPiracanjuba(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Piracanjuba))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoPiracanjuba repositorioConfiguracaoIntegracaoPiracanjuba = new Repositorio.Embarcador.Configuracoes.IntegracaoPiracanjuba(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba configuracaoIntegracaoPiracanjuba = repositorioConfiguracaoIntegracaoPiracanjuba.Buscar();

            if (configuracaoIntegracaoPiracanjuba == null)
                configuracaoIntegracaoPiracanjuba = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba();
            else
                configuracaoIntegracaoPiracanjuba.Initialize();

            configuracaoIntegracaoPiracanjuba.AmbienteProducaoPiracanjuba = Request.GetBoolParam("AmbienteProducaoPiracanjuba");
            configuracaoIntegracaoPiracanjuba.URLIntegracaoCanhotoPiracanjuba = Request.GetStringParam("URLIntegracaoCanhotoPiracanjuba");
            configuracaoIntegracaoPiracanjuba.DataFaturamentoNota = Request.GetNullableDateTimeParam("DataFaturamentoNota");
            configuracaoIntegracaoPiracanjuba.URLIntegracaoCanhotoPiracanjubaContingencia = Request.GetStringParam("URLIntegracaoCanhotoPiracanjubaContingencia");
            configuracaoIntegracaoPiracanjuba.URLIntegracaoCargaPiracanjuba = Request.GetStringParam("URLIntegracaoCargaPiracanjuba");
            configuracaoIntegracaoPiracanjuba.StringAmbientePiracanjuba = Request.GetStringParam("StringAmbientePiracanjuba");

            configuracaoIntegracaoPiracanjuba.URLAutenticacao = Request.GetStringParam("URLAutenticacaoPiracanjuba");
            configuracaoIntegracaoPiracanjuba.ClientID = Request.GetStringParam("ClientIDPiracanjuba");
            configuracaoIntegracaoPiracanjuba.ClientSecret = Request.GetStringParam("ClientSecretPiracanjuba");

            if (configuracaoIntegracaoPiracanjuba.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoPiracanjuba.Atualizar(configuracaoIntegracaoPiracanjuba);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoPiracanjuba.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Piracanjuba.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoPiracanjuba.Inserir(configuracaoIntegracaoPiracanjuba);
        }

        private void SalvarConfiguracoesATSLog(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ATSLog))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoATSLog repositorioConfiguracaoIntegracaoATSLog = new Repositorio.Embarcador.Configuracoes.IntegracaoATSLog(unidadeDeTrabalho);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSLog configuracaoIntegracaoATSLog = repositorioConfiguracaoIntegracaoATSLog.BuscarPrimeiroRegistro();

            int codigoLocalidade = Request.GetIntParam("LocalidadeIntegracaoATSLog", 0);

            if (configuracaoIntegracaoATSLog == null)
                configuracaoIntegracaoATSLog = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSLog();
            else
                configuracaoIntegracaoATSLog.Initialize();

            configuracaoIntegracaoATSLog.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoATSLog");
            configuracaoIntegracaoATSLog.URL = Request.GetStringParam("URLIntegracaoATSLog");
            configuracaoIntegracaoATSLog.Usuario = Request.GetStringParam("UsuarioIntegracaoATSLog");
            configuracaoIntegracaoATSLog.Senha = Request.GetStringParam("SenhaIntegracaoATSLog");
            configuracaoIntegracaoATSLog.SecretKey = Request.GetStringParam("SecretKeyIntegracaoATSLog");
            configuracaoIntegracaoATSLog.CNPJCompany = Request.GetStringParam("CNPJCompanyIntegracaoATSLog");
            configuracaoIntegracaoATSLog.NomeCompany = Request.GetStringParam("NomeCompanyIntegracaoATSLog");
            configuracaoIntegracaoATSLog.Localidade = codigoLocalidade > 0 ? repositorioLocalidade.BuscarPorCodigo(codigoLocalidade) : null;

            if (configuracaoIntegracaoATSLog.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoATSLog.Atualizar(configuracaoIntegracaoATSLog);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoATSLog.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a ATSLog.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoATSLog.Inserir(configuracaoIntegracaoATSLog);
        }

        private void SalvarConfiguracoesCamil(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Camil))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCamil repositorioConfiguracaoIntegracaoCamil = new Repositorio.Embarcador.Configuracoes.IntegracaoCamil(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCamil configuracaoIntegracaoCamil = repositorioConfiguracaoIntegracaoCamil.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoCamil == null)
                configuracaoIntegracaoCamil = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCamil();
            else
                configuracaoIntegracaoCamil.Initialize();

            configuracaoIntegracaoCamil.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoCamil");
            configuracaoIntegracaoCamil.URL = Request.GetStringParam("URLIntegracaoCamil");
            configuracaoIntegracaoCamil.ApiKey = Request.GetStringParam("ApiKeyCamil");

            if (configuracaoIntegracaoCamil.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoCamil.Atualizar(configuracaoIntegracaoCamil);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoCamil.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Camil.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoCamil.Inserir(configuracaoIntegracaoCamil);
        }

        private void SalvarConfiguracoesCebrace(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Cebrace))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCebrace repositorioConfiguracaoIntegracaoCebrace = new Repositorio.Embarcador.Configuracoes.IntegracaoCebrace(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCebrace configuracaoIntegracaoCebrace = repositorioConfiguracaoIntegracaoCebrace.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoCebrace == null)
                configuracaoIntegracaoCebrace = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCebrace();
            else
                configuracaoIntegracaoCebrace.Initialize();

            configuracaoIntegracaoCebrace.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoCebrace");
            configuracaoIntegracaoCebrace.URLIntegracao = Request.GetStringParam("URLIntegracaoCebrace");
            configuracaoIntegracaoCebrace.URLAutenticacao = Request.GetStringParam("URLAutenticacaoCebrace");
            configuracaoIntegracaoCebrace.ApiKey = Request.GetStringParam("APIKeyIntegracaoCebrace");

            if (configuracaoIntegracaoCebrace.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoCebrace.Atualizar(configuracaoIntegracaoCebrace);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoCebrace.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com o Cebrace.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoCebrace.Inserir(configuracaoIntegracaoCebrace);
        }

        private void SalvarConfiguracoesIntegracaoBuntech(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Buntech))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoBuntech repositorioConfiguracaoIntegracaoBuntech = new Repositorio.Embarcador.Configuracoes.IntegracaoBuntech(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuntech configuracaoIntegracaoBuntech = repositorioConfiguracaoIntegracaoBuntech.Buscar();

            if (configuracaoIntegracaoBuntech == null)
                configuracaoIntegracaoBuntech = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuntech();
            else
                configuracaoIntegracaoBuntech.Initialize();

            configuracaoIntegracaoBuntech.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoBuntech");
            configuracaoIntegracaoBuntech.URLAutenticacao = Request.GetStringParam("URLAutenticacaoBuntech");
            configuracaoIntegracaoBuntech.URLProvisao = Request.GetStringParam("URLProvisao");

            if (configuracaoIntegracaoBuntech.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoBuntech.Atualizar(configuracaoIntegracaoBuntech);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoBuntech.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Buntech.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoBuntech.Inserir(configuracaoIntegracaoBuntech);
        }

        private void SalvarConfiguracoesIntegracaoRouteasy(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Routeasy))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy repositorioConfiguracaoIntegracaoRouteasy = new Repositorio.Embarcador.Configuracoes.IntegracaoRouteasy(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy configuracaoIntegracaoRouteasy = repositorioConfiguracaoIntegracaoRouteasy.Buscar();

            if (configuracaoIntegracaoRouteasy == null)
                configuracaoIntegracaoRouteasy = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRouteasy();
            else
                configuracaoIntegracaoRouteasy.Initialize();

            configuracaoIntegracaoRouteasy.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoRouteasy");
            configuracaoIntegracaoRouteasy.URL = Request.GetStringParam("URLIntegracaoRouteasy");
            configuracaoIntegracaoRouteasy.APIKey = Request.GetStringParam("APIKeyIntegracaoRouteasy");
            configuracaoIntegracaoRouteasy.ConfiguracaoLoads = Request.GetStringParam("ConfiguracaoLoads");
            configuracaoIntegracaoRouteasy.EnviarDataCriacaoVendaPedidoAbaAdicionaisIntegracao = Request.GetBoolParam("EnviarDataCriacaoVendaPedidoAbaAdicionaisIntegracao");

            if (configuracaoIntegracaoRouteasy.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoRouteasy.Atualizar(configuracaoIntegracaoRouteasy);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoRouteasy.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Routeasy.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoRouteasy.Inserir(configuracaoIntegracaoRouteasy);
        }

        private void SalvarConfiguracoesIntegracaoMondelez(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Mondelez))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoMondelez repositorioConfiguracaoIntegracaoMondelez = new Repositorio.Embarcador.Configuracoes.IntegracaoMondelez(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMondelez configuracaoIntegracaoMondelez = repositorioConfiguracaoIntegracaoMondelez.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoMondelez == null)
                configuracaoIntegracaoMondelez = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMondelez();
            else
                configuracaoIntegracaoMondelez.Initialize();

            configuracaoIntegracaoMondelez.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoMondelez");
            configuracaoIntegracaoMondelez.URLDrivin = Request.GetStringParam("URLDrivinMondelez");
            configuracaoIntegracaoMondelez.ApiKeyDrivin = Request.GetStringParam("ApiKeyDrivinMondelez");
            configuracaoIntegracaoMondelez.ApiKeyDrivinLegado = Request.GetStringParam("ApiKeyDrivinLegadoMondelez");

            if (configuracaoIntegracaoMondelez.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoMondelez.Atualizar(configuracaoIntegracaoMondelez);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoMondelez.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Mondelez.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaoMondelez.Inserir(configuracaoIntegracaoMondelez);
        }

        private void SalvarConfiguracoesIntegracaoGrupoSC(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.GrupoSC))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoGrupoSC repositorioConfiguracaoIntegracaIntegracaoGrupoSC = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoSC(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoSC configuracaoIntegracaoIntegracaoGrupoSC = repositorioConfiguracaoIntegracaIntegracaoGrupoSC.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoIntegracaoGrupoSC == null)
                configuracaoIntegracaoIntegracaoGrupoSC = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoSC();
            else
                configuracaoIntegracaoIntegracaoGrupoSC.Initialize();

            configuracaoIntegracaoIntegracaoGrupoSC.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoGrupoSC");
            configuracaoIntegracaoIntegracaoGrupoSC.URLIntegracao = Request.GetStringParam("URLIntegracaoGrupoSC");
            configuracaoIntegracaoIntegracaoGrupoSC.ApiKey = Request.GetStringParam("ApiKeyGrupoSC");

            if (configuracaoIntegracaoIntegracaoGrupoSC.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaIntegracaoGrupoSC.Atualizar(configuracaoIntegracaoIntegracaoGrupoSC);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoIntegracaoGrupoSC.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a GrupoSC.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaIntegracaoGrupoSC.Inserir(configuracaoIntegracaoIntegracaoGrupoSC);
        }

        private void SalvarConfiguracoesIntegracaoFusion(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Fusion))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoFusion repositorioIntegracaoFusion = new Repositorio.Embarcador.Configuracoes.IntegracaoFusion(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFusion configuracaoIntegracaoFusion = repositorioIntegracaoFusion.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoFusion == null)
                configuracaoIntegracaoFusion = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFusion();
            else
                configuracaoIntegracaoFusion.Initialize();

            configuracaoIntegracaoFusion.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoFusion");
            configuracaoIntegracaoFusion.URLIntegracaoCarga = Request.GetStringParam("URLIntegracaoCargaFusion");
            configuracaoIntegracaoFusion.URLIntegracaoPedido = Request.GetStringParam("URLIntegracaoPedidoFusion");
            configuracaoIntegracaoFusion.Token = Request.GetStringParam("TokenFusion");

            if (configuracaoIntegracaoFusion.Codigo > 0)
            {
                repositorioIntegracaoFusion.Atualizar(configuracaoIntegracaoFusion);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoFusion.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Fusion.", unitOfWork);
            }
            else
                repositorioIntegracaoFusion.Inserir(configuracaoIntegracaoFusion);
        }

        private void SalvarConfiguracoesSalesforce(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Salesforce))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSalesforce repositorioIntegracaoSalesforce = new Repositorio.Embarcador.Configuracoes.IntegracaoSalesforce(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSalesforce configuracaoIntegracaoSalesforce = repositorioIntegracaoSalesforce.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoSalesforce == null)
                configuracaoIntegracaoSalesforce = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSalesforce();
            else
                configuracaoIntegracaoSalesforce.Initialize();

            configuracaoIntegracaoSalesforce.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoSalesforce");
            configuracaoIntegracaoSalesforce.URLBase = Request.GetStringParam("URLBaseSalesforce");
            configuracaoIntegracaoSalesforce.URIToken = Request.GetStringParam("URITokenSalesforce");
            configuracaoIntegracaoSalesforce.URICasoDevolucao = Request.GetStringParam("URICasoDevolucaoSalesforce");
            configuracaoIntegracaoSalesforce.ClientID = Request.GetStringParam("ClientIDSalesforce");
            configuracaoIntegracaoSalesforce.ClientSecret = Request.GetStringParam("ClientSecretSalesforce");

            if (configuracaoIntegracaoSalesforce.Codigo > 0)
            {
                repositorioIntegracaoSalesforce.Atualizar(configuracaoIntegracaoSalesforce);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoSalesforce.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Salesforce.", unitOfWork);
            }
            else
                repositorioIntegracaoSalesforce.Inserir(configuracaoIntegracaoSalesforce);
        }

        private void SalvarConfiguracoesConecttec(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Conecttec))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoConecttec repositorioConfiguracaoIntegracaoConecttec = new Repositorio.Embarcador.Configuracoes.IntegracaoConecttec(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConecttec configuracaoIntegracaoConecttec = repositorioConfiguracaoIntegracaoConecttec.Buscar();

            if (configuracaoIntegracaoConecttec == null)
                configuracaoIntegracaoConecttec = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConecttec();
            else
                configuracaoIntegracaoConecttec.Initialize();

            configuracaoIntegracaoConecttec.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoConecttec");
            configuracaoIntegracaoConecttec.URL = Request.GetStringParam("URLConecttec");
            configuracaoIntegracaoConecttec.ProviderID = Request.GetStringParam("ProviderIDConecttec");
            configuracaoIntegracaoConecttec.StationID = Request.GetStringParam("StationIDConecttec");
            configuracaoIntegracaoConecttec.SecretKEY = Request.GetStringParam("SecretKEYConecttec");
            configuracaoIntegracaoConecttec.BrokerPort = Request.GetIntParam("PortaBrokerConecttec");
            configuracaoIntegracaoConecttec.URLRecebimentoCallback = Request.GetStringParam("URLRecebimentoCallbackConecttec");


            if (configuracaoIntegracaoConecttec.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoConecttec.Atualizar(configuracaoIntegracaoConecttec);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoConecttec.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Conecttec.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoConecttec.Inserir(configuracaoIntegracaoConecttec);
        }
        private void SalvarConfiguracoesMigrate(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Migrate))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoMigrate repositorioConfiguracaoIntegracaoMigrate = new Repositorio.Embarcador.Configuracoes.IntegracaoMigrate(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMigrate configuracaoIntegracaoMigrate = repositorioConfiguracaoIntegracaoMigrate.Buscar();

            if (configuracaoIntegracaoMigrate == null)
                configuracaoIntegracaoMigrate = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMigrate();
            else
                configuracaoIntegracaoMigrate.Initialize();

            configuracaoIntegracaoMigrate.URL = Request.GetStringParam("URLMigrate");

            if (configuracaoIntegracaoMigrate.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoMigrate.Atualizar(configuracaoIntegracaoMigrate);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoMigrate.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Migrate.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoMigrate.Inserir(configuracaoIntegracaoMigrate);
        }
        private void SalvarConfiguracoesIntegracaoMars(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Mars))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoMars repositorioConfiguracaoIntegracaIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoMars(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars configuracaoIntegracaoIntegracao = repositorioConfiguracaoIntegracaIntegracao.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoIntegracao == null)
                configuracaoIntegracaoIntegracao = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars();
            else
                configuracaoIntegracaoIntegracao.Initialize();

            configuracaoIntegracaoIntegracao.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoMars");
            configuracaoIntegracaoIntegracao.URLIntegracaoCargaCTe = Request.GetStringParam("URLIntegracaoCargaCTeMars");
            configuracaoIntegracaoIntegracao.URLAutenticacao = Request.GetStringParam("URLAutenticacaoMars");
            configuracaoIntegracaoIntegracao.URLIntegracaoCanhoto = Request.GetStringParam("URLIntegracaoCanhotoMars");
            configuracaoIntegracaoIntegracao.ClientID = Request.GetStringParam("ClientIDMars");
            configuracaoIntegracaoIntegracao.ClientSecret = Request.GetStringParam("ClientSecretMars");
            configuracaoIntegracaoIntegracao.URLIntegracaoCancelamentosCargas = Request.GetStringParam("URLIntegracaoCancelamentosCargas");
            configuracaoIntegracaoIntegracao.ClientIDCancelamentosCargas = Request.GetStringParam("ClientIDCancelamentosCargas");
            configuracaoIntegracaoIntegracao.ClientSecretCancelamentosCargas = Request.GetStringParam("ClientSecretCancelamentosCargas");
            configuracaoIntegracaoIntegracao.URLAutenticacaoCancelamentosCargas = Request.GetStringParam("URLAutenticacaoCancelamentosCargas");

            if (configuracaoIntegracaoIntegracao.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaIntegracao.Atualizar(configuracaoIntegracaoIntegracao);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoIntegracao.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a GrupoSC.", unitOfWork);
            }
            else
                repositorioConfiguracaoIntegracaIntegracao.Inserir(configuracaoIntegracaoIntegracao);
        }

        private void SalvarConfiguracoesAcessoViaToken(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken repositorioConfiguracaoAcessoViaToken = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken configuracaoAcessoViaToken = repositorioConfiguracaoAcessoViaToken.BuscarPrimeiroRegistro();

            if (configuracaoAcessoViaToken == null)
                configuracaoAcessoViaToken = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken();
            else
                configuracaoAcessoViaToken.Initialize();

            configuracaoAcessoViaToken.GerarUrlAcessoPortalMultiClifor = Request.GetBoolParam("GerarUrlAcessoPortalMultiCliforAcessoViaToken");
            configuracaoAcessoViaToken.Audiencia = Request.GetStringParam("AudienciaAcessoViaToken");
            configuracaoAcessoViaToken.ChaveSecreta = Request.GetStringParam("ChaveSecretaAcessoViaToken");
            configuracaoAcessoViaToken.Emissor = Request.GetStringParam("EmissorAcessoViaToken");

            if (configuracaoAcessoViaToken.Codigo > 0)
            {
                repositorioConfiguracaoAcessoViaToken.Atualizar(configuracaoAcessoViaToken);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoAcessoViaToken.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de Acesso Via Token.", unitOfWork);
            }
            else
                repositorioConfiguracaoAcessoViaToken.Inserir(configuracaoAcessoViaToken);
        }

        private void SalvarConfiguracoesGlobus(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Globus))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoGlobus repIntegracaoGlobus = new Repositorio.Embarcador.Configuracoes.IntegracaoGlobus(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus integracaoGlobus = repIntegracaoGlobus.Buscar();

            if (integracaoGlobus == null)
                integracaoGlobus = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus();
            else
                integracaoGlobus.Initialize();

            integracaoGlobus.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoGlobus");
            integracaoGlobus.IntegrarComContabilidade = Request.GetBoolParam("IntegrarComContabilidadeGlobus");
            integracaoGlobus.IntegrarComEscritaFiscal = Request.GetBoolParam("IntegrarComEscritaFiscalGlobus");
            integracaoGlobus.IntegrarComContasPagar = Request.GetBoolParam("IntegrarComContasPagarGlobus");
            integracaoGlobus.IntegrarComContasReceber = Request.GetBoolParam("IntegrarComContasReceberGlobus");
            integracaoGlobus.ShortCodeEscrituracaoISS = Request.GetIntParam("ShortCodeEscrituracaoISSGlobus");
            integracaoGlobus.ShortCodeNFSe = Request.GetIntParam("ShortCodeNFSeGlobus");
            integracaoGlobus.ShortCodeFinanceiro = Request.GetIntParam("ShortCodeFinanceiroGlobus");
            integracaoGlobus.ShortCodeXML = Request.GetIntParam("ShortCodeXMLGlobus");
            integracaoGlobus.ShortCodeParticipante = Request.GetIntParam("ShortCodeParticipanteGlobus");
            integracaoGlobus.CodigoIntegrarComContabilidade = Request.GetStringParam("CodigoIntegrarComContabilidadeGlobus");
            integracaoGlobus.CodigoIntegrarComEscritaFiscal = Request.GetStringParam("CodigoIntegrarComEscritaFiscalGlobus");
            integracaoGlobus.CodigoIntegrarComContasPagar = Request.GetStringParam("CodigoIntegrarComContasPagarGlobus");
            integracaoGlobus.CodigoIntegrarComContasReceber = Request.GetStringParam("CodigoIntegrarComContasReceberGlobus");
            integracaoGlobus.SistemaIntegrarComContabilidade = Request.GetStringParam("SistemaIntegrarComContabilidadeGlobus");
            integracaoGlobus.SistemaIntegrarComEscritaFiscal = Request.GetStringParam("SistemaIntegrarComEscritaFiscalGlobus");
            integracaoGlobus.SistemaIntegrarComContasPagar = Request.GetStringParam("SistemaIntegrarComContasPagarGlobus");
            integracaoGlobus.SistemaIntegrarComContasReceber = Request.GetStringParam("SistemaIntegrarComContasReceberGlobus");
            integracaoGlobus.URLWebServiceEscrituracaoISS = Request.GetStringParam("URLWebServiceEscrituracaoISSGlobus");
            integracaoGlobus.URLWebServiceNFSe = Request.GetStringParam("URLWebServiceNFSeGlobus");
            integracaoGlobus.URLWebServiceFinanceiro = Request.GetStringParam("URLWebServiceFinanceiroGlobus");
            integracaoGlobus.URLWebServiceXML = Request.GetStringParam("URLWebServiceXMLGlobus");
            integracaoGlobus.URLWebServiceParticipante = Request.GetStringParam("URLWebServiceParticipanteGlobus");
            integracaoGlobus.TokenEscrituracaoISS = Request.GetStringParam("TokenEscrituracaoISSGlobus");
            integracaoGlobus.TokenNFSe = Request.GetStringParam("TokenNFSeGlobus");
            integracaoGlobus.TokenFinanceiro = Request.GetStringParam("TokenFinanceiroGlobus");
            integracaoGlobus.TokenXML = Request.GetStringParam("TokenXMLGlobus");
            integracaoGlobus.TokenParticipante = Request.GetStringParam("TokenParticipanteGlobus");
            integracaoGlobus.Sistema = Request.GetStringParam("SistemaGlobus");
            integracaoGlobus.Usuario = Request.GetStringParam("UsuarioGlobus");
            integracaoGlobus.Grupo = Request.GetStringParam("GrupoGlobus");

            if (integracaoGlobus.Codigo > 0)
            {
                repIntegracaoGlobus.Atualizar(integracaoGlobus);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = integracaoGlobus.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, $"Alterou a configuração de integração com a Trans Sat.", unidadeDeTrabalho);
            }
            else
                repIntegracaoGlobus.Inserir(integracaoGlobus);
        }

        private void SalvarConfiguracaoFS(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.FS))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoFS repositorioConfiguracaoIntegracaoFS = new Repositorio.Embarcador.Configuracoes.IntegracaoFS(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFS configuracaoIntegracaoFS = repositorioConfiguracaoIntegracaoFS.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoFS == null)
                configuracaoIntegracaoFS = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFS();
            else
                configuracaoIntegracaoFS.Initialize();

            configuracaoIntegracaoFS.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoFS");
            configuracaoIntegracaoFS.URLIntegracao = Request.GetStringParam("URLIntegracaoFS");
            configuracaoIntegracaoFS.URLAutenticacao = Request.GetStringParam("URLAutenticacaoFS");
            configuracaoIntegracaoFS.ClientID = Request.GetStringParam("ClientIDFS");
            configuracaoIntegracaoFS.ClientSecret = Request.GetStringParam("ClientSecretFS");

            if (configuracaoIntegracaoFS.Codigo > 0)
            {
                repositorioConfiguracaoIntegracaoFS.Atualizar(configuracaoIntegracaoFS);
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoFS.GetChanges();

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a FS.", unidadeDeTrabalho);
            }
            else
                repositorioConfiguracaoIntegracaoFS.Inserir(configuracaoIntegracaoFS);

        }

        private byte[] ConvertStreamToByteArray(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoTipoIntegracao ObterConfiguracaoDoTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoTipoIntegracao configuracaoTipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(configuracaoTipoIntegracao.TipoIntegracao);

            if (tipoIntegracao != null)
                configuracaoTipoIntegracao.IntegrarCargaTransbordo = tipoIntegracao.IntegrarCargaTransbordo;

            return configuracaoTipoIntegracao;
        }

        private void SalvarConfiguracaoNoTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Integracao.ConfiguracaoTipoIntegracao configuracaoTipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(configuracaoTipoIntegracao.TipoIntegracao);
            if (tipoIntegracao == null) return;

            if (configuracaoTipoIntegracao.IntegrarCargaTransbordo.HasValue)
                tipoIntegracao.IntegrarCargaTransbordo = configuracaoTipoIntegracao.IntegrarCargaTransbordo.Value;

            repositorioTipoIntegracao.Atualizar(tipoIntegracao);
        }

        private async Task SalvarConfiguracoesVedacitAsync(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Vedacit))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoVedacit repositorioIntegracaoVedacit = new Repositorio.Embarcador.Configuracoes.IntegracaoVedacit(unidadeDeTrabalho, cancellationToken);
            IntegracaoVedacit configuracaoIntegracaoVedacit = await repositorioIntegracaoVedacit.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoVedacit == null)
                configuracaoIntegracaoVedacit = new IntegracaoVedacit();
            else
                configuracaoIntegracaoVedacit.Initialize();

            configuracaoIntegracaoVedacit.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoVedacit");
            configuracaoIntegracaoVedacit.URLIntegracao = Request.GetStringParam("URLIntegracaoVedacit");
            configuracaoIntegracaoVedacit.Usuario = Request.GetStringParam("UsuarioIntegracaoVedacit");
            configuracaoIntegracaoVedacit.Senha = Request.GetStringParam("SenhaIntegracaoVedacit");
            configuracaoIntegracaoVedacit.URLIntegracaoCarga = Request.GetStringParam("URLIntegracaoCargaVedacit");
            configuracaoIntegracaoVedacit.UsuarioIntegracaoCarga = Request.GetStringParam("UsuarioIntegracaoCargaVedacit");
            configuracaoIntegracaoVedacit.SenhaIntegracaoCarga = Request.GetStringParam("SenhaIntegracaoCargaVedacit");

            if (configuracaoIntegracaoVedacit.Codigo > 0)
            {
                await repositorioIntegracaoVedacit.AtualizarAsync(configuracaoIntegracaoVedacit);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoVedacit.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Vedacit.", unidadeDeTrabalho);

                return;
            }

            await repositorioIntegracaoVedacit.InserirAsync(configuracaoIntegracaoVedacit);
        }

        private async Task SalvarConfiguracoesJDEFaturasAsync(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.JDEFaturas))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoJDEFaturas repositorioIntegracaoJDEFaturas = new Repositorio.Embarcador.Configuracoes.IntegracaoJDEFaturas(unidadeDeTrabalho, cancellationToken);
            IntegracaoJDEFaturas configuracaoIntegracaoJDEFaturas = await repositorioIntegracaoJDEFaturas.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoJDEFaturas == null)
                configuracaoIntegracaoJDEFaturas = new IntegracaoJDEFaturas();
            else
                configuracaoIntegracaoJDEFaturas.Initialize();

            configuracaoIntegracaoJDEFaturas.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoJDEFaturas");
            configuracaoIntegracaoJDEFaturas.URLIntegracao = Request.GetStringParam("URLIntegracaoJDEFaturas");
            configuracaoIntegracaoJDEFaturas.Usuario = Request.GetStringParam("UsuarioIntegracaoJDEFaturas");
            configuracaoIntegracaoJDEFaturas.Senha = Request.GetStringParam("SenhaIntegracaoJDEFaturas");

            if (configuracaoIntegracaoJDEFaturas.Codigo > 0)
            {
                await repositorioIntegracaoJDEFaturas.AtualizarAsync(configuracaoIntegracaoJDEFaturas);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoJDEFaturas.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a JDE Faturas.", unidadeDeTrabalho);

                return;
            }

            await repositorioIntegracaoJDEFaturas.InserirAsync(configuracaoIntegracaoJDEFaturas);
        }

        private async Task SalvarConfiguracoesTransSatAsync(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.TransSat))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoTransSat repositorioIntegracaoTransSat = new Repositorio.Embarcador.Configuracoes.IntegracaoTransSat(unidadeDeTrabalho, cancellationToken);
            IntegracaoTransSat configuracaoIntegracaoTransSat = await repositorioIntegracaoTransSat.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoTransSat == null)
                configuracaoIntegracaoTransSat = new IntegracaoTransSat();
            else
                configuracaoIntegracaoTransSat.Initialize();

            configuracaoIntegracaoTransSat.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoTransSat");
            configuracaoIntegracaoTransSat.URLWebServiceIntegracaoTransSat = Request.GetStringParam("URLWebServiceTransSat");
            configuracaoIntegracaoTransSat.TokenIntegracaoTransSat = Request.GetStringParam("TokenTransSat");
            configuracaoIntegracaoTransSat.EmailParaReceberRetornoDaGR = Request.GetStringParam("EmailParaReceberRetornoDaGRTransSat");

            if (configuracaoIntegracaoTransSat.Codigo > 0)
            {
                await repositorioIntegracaoTransSat.AtualizarAsync(configuracaoIntegracaoTransSat);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoTransSat.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a TransSat.", unidadeDeTrabalho);

                return;
            }

            await repositorioIntegracaoTransSat.InserirAsync(configuracaoIntegracaoTransSat);
        }

        private async Task SalvarConfiguracoesCassolAsync(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Cassol))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCassol repositorioIntegracaoCassol = new Repositorio.Embarcador.Configuracoes.IntegracaoCassol(unidadeDeTrabalho, cancellationToken);
            IntegracaoCassol configuracaoIntegracaoCassol = await repositorioIntegracaoCassol.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoCassol == null)
                configuracaoIntegracaoCassol = new IntegracaoCassol();
            else
                configuracaoIntegracaoCassol.Initialize();

            configuracaoIntegracaoCassol.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoCassol");
            configuracaoIntegracaoCassol.URLIntegracao = Request.GetStringParam("URLCassol");
            configuracaoIntegracaoCassol.Token = Request.GetStringParam("TokenCassol");

            if (configuracaoIntegracaoCassol.Codigo > 0)
            {
                await repositorioIntegracaoCassol.AtualizarAsync(configuracaoIntegracaoCassol);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoCassol.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Cassol.", unidadeDeTrabalho);

                return;
            }

            await repositorioIntegracaoCassol.InserirAsync(configuracaoIntegracaoCassol);
        }

        private async Task SalvarConfiguracoesEfesusAsync(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Efesus))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoEfesus repositorioIntegracaoEfesus = new Repositorio.Embarcador.Configuracoes.IntegracaoEfesus(unidadeDeTrabalho, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEfesus configuracaoIntegracaoEfesus = await repositorioIntegracaoEfesus.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoEfesus == null)
                configuracaoIntegracaoEfesus = new IntegracaoEfesus();
            else
                configuracaoIntegracaoEfesus.Initialize();

            configuracaoIntegracaoEfesus.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoEfesus");
            configuracaoIntegracaoEfesus.URLIntegracao = Request.GetStringParam("URLAutenticacaoEfesus");
            configuracaoIntegracaoEfesus.Usuario = Request.GetStringParam("UsuarioEfesus");
            configuracaoIntegracaoEfesus.Senha = Request.GetStringParam("SenhaEfesus");

            if (configuracaoIntegracaoEfesus.Codigo > 0)
            {
                await repositorioIntegracaoEfesus.AtualizarAsync(configuracaoIntegracaoEfesus);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoEfesus.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Efesus.", unidadeDeTrabalho);

                return;
            }

            await repositorioIntegracaoEfesus.InserirAsync(configuracaoIntegracaoEfesus);
        }

        private async Task SalvarConfiguracoesIntegracaoOlfarAsync(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Olfar))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoOlfar repositorioConfiguracaoIntegracaoOlfar = new Repositorio.Embarcador.Configuracoes.IntegracaoOlfar(unidadeDeTrabalho, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOlfar configuracaoIntegracaoOlfar = await repositorioConfiguracaoIntegracaoOlfar.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoOlfar == null)
                configuracaoIntegracaoOlfar = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOlfar();
            else
                configuracaoIntegracaoOlfar.Initialize();

            configuracaoIntegracaoOlfar.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoOlfar");
            configuracaoIntegracaoOlfar.URLIntegracao = Request.GetStringParam("URLIntegracaoOlfar");

            if (configuracaoIntegracaoOlfar.Codigo > 0)
            {
                await repositorioConfiguracaoIntegracaoOlfar.AtualizarAsync(configuracaoIntegracaoOlfar);

                await Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadasAsync(Auditado, configuracaoIntegracao, configuracaoIntegracaoOlfar.GetChanges(), "Alterou a configuração de integração com a Olfar.", unidadeDeTrabalho);
            }
            else
                await repositorioConfiguracaoIntegracaoOlfar.InserirAsync(configuracaoIntegracaoOlfar);
        }

        private async Task SalvarConfiguracoesIntegracaoWeberChileAsync(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.WeberChile))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoWeberChile repositorioConfiguracaoIntegracaoWeberChile = new Repositorio.Embarcador.Configuracoes.IntegracaoWeberChile(unidadeDeTrabalho, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWeberChile configuracaoIntegracaoWeberChile = await repositorioConfiguracaoIntegracaoWeberChile.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoWeberChile == null)
                configuracaoIntegracaoWeberChile = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWeberChile();
            else
                configuracaoIntegracaoWeberChile.Initialize();

            configuracaoIntegracaoWeberChile.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoWeberChile");
            configuracaoIntegracaoWeberChile.URLIntegracao = Request.GetStringParam("URLIntegracaoWeberChile");
            configuracaoIntegracaoWeberChile.URLAutenticacao = Request.GetStringParam("URLAutenticacaoWeberChile");
            configuracaoIntegracaoWeberChile.ClientID = Request.GetStringParam("ClientIDWeberChile");
            configuracaoIntegracaoWeberChile.ClientSecret = Request.GetStringParam("ClientSecretWeberChile");
            configuracaoIntegracaoWeberChile.ApiKey = Request.GetStringParam("APIKeyWeberChile");


            if (configuracaoIntegracaoWeberChile.Codigo > 0)
            {
                await repositorioConfiguracaoIntegracaoWeberChile.AtualizarAsync(configuracaoIntegracaoWeberChile);

                await Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadasAsync(Auditado, configuracaoIntegracao, configuracaoIntegracaoWeberChile.GetChanges(), "Alterou a configuração de integração com a WeberChile.", unidadeDeTrabalho);
            }
            else
                await repositorioConfiguracaoIntegracaoWeberChile.InserirAsync(configuracaoIntegracaoWeberChile);
        }

        private async Task SalvarConfiguracoesIntegracaoLactalisAsync(Integracao configuracaoIntegracao, UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Lactalis))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoLactalis repositorioIntegracaoLactalis = new Repositorio.Embarcador.Configuracoes.IntegracaoLactalis(unitOfWork, cancellationToken);
            IntegracaoLactalis configuracaoIntegracaoLactalis = await repositorioIntegracaoLactalis.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoLactalis == null)
                configuracaoIntegracaoLactalis = new IntegracaoLactalis();
            else
                configuracaoIntegracaoLactalis.Initialize();

            configuracaoIntegracaoLactalis.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoLactalis");
            configuracaoIntegracaoLactalis.URLIntegracao = Request.GetStringParam("URLIntegracaoLactalis");
            configuracaoIntegracaoLactalis.URLAutenticacao = Request.GetStringParam("URLAutenticacaoLactalis");
            configuracaoIntegracaoLactalis.Usuario = Request.GetStringParam("UsuarioLactalis");
            configuracaoIntegracaoLactalis.Senha = Request.GetStringParam("SenhaLactalis");

            if (configuracaoIntegracaoLactalis.Codigo > 0)
            {
                await repositorioIntegracaoLactalis.AtualizarAsync(configuracaoIntegracaoLactalis);
                await Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadasAsync(Auditado, configuracaoIntegracao, configuracaoIntegracaoLactalis.GetChanges(), "Alterou a configuração de integração com a Lactalis.", unitOfWork);
                return;
            }

            await repositorioIntegracaoLactalis.InserirAsync(configuracaoIntegracaoLactalis);
        }

        private async Task SalvarConfiguracoesIntegracaoPortalCabotagemAsync(Integracao configuracaoIntegracao, UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.PortalCabotagem))
                return;

            var repositorioIntegracaoPortalCabotagem = new Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem(unitOfWork, cancellationToken);
            var configuracaoIntegracaoPortalCabotagem = await repositorioIntegracaoPortalCabotagem.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoPortalCabotagem == null)
                configuracaoIntegracaoPortalCabotagem = new IntegracaoPortalCabotagem();
            else
                configuracaoIntegracaoPortalCabotagem.Initialize();

            configuracaoIntegracaoPortalCabotagem.AtivarIntegracaoPortalAzureStorage = Request.GetBoolParam("AtivarIntegracaoPortalAzureStoragePortalCabotagem");
            configuracaoIntegracaoPortalCabotagem.Container = Request.GetStringParam("ContainerPortalCabotagem");
            configuracaoIntegracaoPortalCabotagem.StorageAccount = Request.GetStringParam("StorageAccountPortalCabotagem");
            configuracaoIntegracaoPortalCabotagem.URL = Request.GetStringParam("URLPortalCabotagem");
            configuracaoIntegracaoPortalCabotagem.ClienteID = Request.GetStringParam("ClienteIDPortalCabotagem");
            configuracaoIntegracaoPortalCabotagem.Secret = Request.GetStringParam("SecretPortalCabotagem");
            configuracaoIntegracaoPortalCabotagem.AtivarEnvioPDFFatura = Request.GetBoolParam("AtivarEnvioPDFFaturaPortalCabotagem");
            configuracaoIntegracaoPortalCabotagem.AtivarEnvioPDFCTE = Request.GetBoolParam("AtivarEnvioPDFCTEPortalCabotagem");
            configuracaoIntegracaoPortalCabotagem.AtivarEnvioPDFBoleto = Request.GetBoolParam("AtivarEnvioPDFBOLETOPortalCabotagem");
            configuracaoIntegracaoPortalCabotagem.AtivarEnvioXMLCTE = Request.GetBoolParam("AtivarEnvioXMLCTEPortalCabotagem");

            if (configuracaoIntegracaoPortalCabotagem.Codigo > 0)
            {
                await repositorioIntegracaoPortalCabotagem.AtualizarAsync(configuracaoIntegracaoPortalCabotagem);
                await Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadasAsync(Auditado, configuracaoIntegracao, configuracaoIntegracaoPortalCabotagem.GetChanges(), "Alterou a configuração de integração com o Portal Cabotagem.", unitOfWork);
                return;
            }

            await repositorioIntegracaoPortalCabotagem.InserirAsync(configuracaoIntegracaoPortalCabotagem);
        }

        private async Task SalvarConfiguracoesIntegracaoSistemaTransbenAsync(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.SistemaTransben))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSistemaTransben repositorioIntegracaoSistemaTransben = new Repositorio.Embarcador.Configuracoes.IntegracaoSistemaTransben(unidadeDeTrabalho, cancellationToken);
            IntegracaoSistemaTransben configuracaoIntegracaoSistemaTransben = await repositorioIntegracaoSistemaTransben.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoSistemaTransben == null)
                configuracaoIntegracaoSistemaTransben = new IntegracaoSistemaTransben();
            else
                configuracaoIntegracaoSistemaTransben.Initialize();

            configuracaoIntegracaoSistemaTransben.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoSistemaTransben");
            configuracaoIntegracaoSistemaTransben.EnviarDadosCargaParaSistemaTransben = Request.GetBoolParam("EnviarDadosCargaParaSistemaTransben");
            configuracaoIntegracaoSistemaTransben.URLSistemaTransben = Request.GetStringParam("URLIntegracaoSistemaTransben");
            configuracaoIntegracaoSistemaTransben.Usuario = Request.GetStringParam("UsuarioIntegracaoSistemaTransben");
            configuracaoIntegracaoSistemaTransben.Senha = Request.GetStringParam("SenhaIntegracaoSistemaTransben");

            if (configuracaoIntegracaoSistemaTransben.Codigo > 0)
            {
                await repositorioIntegracaoSistemaTransben.AtualizarAsync(configuracaoIntegracaoSistemaTransben);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoSistemaTransben.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a TransSat.", unidadeDeTrabalho);

                return;
            }

            await repositorioIntegracaoSistemaTransben.InserirAsync(configuracaoIntegracaoSistemaTransben);
        }

        private async Task SalvarConfiguracoesIntegracaoATSSmartWebAsync(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.ATSSmartWeb))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoATSSmartWeb repositorioIntegracaoATSSmartWeb = new Repositorio.Embarcador.Configuracoes.IntegracaoATSSmartWeb(unidadeDeTrabalho, cancellationToken);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            IntegracaoATSSmartWeb configuracaoIntegracaoATSSmartWeb = await repositorioIntegracaoATSSmartWeb.BuscarPrimeiroRegistroAsync();

            int codigoLocalidade = Request.GetIntParam("LocalidadeIntegracaoATSSmartWeb", 0);

            if (configuracaoIntegracaoATSSmartWeb == null)
                configuracaoIntegracaoATSSmartWeb = new IntegracaoATSSmartWeb();
            else
                configuracaoIntegracaoATSSmartWeb.Initialize();

            configuracaoIntegracaoATSSmartWeb.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoATSSmartWeb");
            configuracaoIntegracaoATSSmartWeb.SecretKEY = Request.GetStringParam("SecretKeyATSSmartWeb");
            configuracaoIntegracaoATSSmartWeb.URL = Request.GetStringParam("URLIntegracaoATSSmartWeb");
            configuracaoIntegracaoATSSmartWeb.Usuario = Request.GetStringParam("UsuarioIntegracaoATSSmartWeb");
            configuracaoIntegracaoATSSmartWeb.Senha = Request.GetStringParam("SenhaIntegracaoATSSmartWeb");
            configuracaoIntegracaoATSSmartWeb.CNPJCompany = Request.GetStringParam("CNPJCompanyIntegracaoATSSmartWeb");
            configuracaoIntegracaoATSSmartWeb.NomeCompany = Request.GetStringParam("NomeCompanyIntegracaoATSSmartWeb");
            configuracaoIntegracaoATSSmartWeb.Localidade = codigoLocalidade > 0 ? repositorioLocalidade.BuscarPorCodigo(codigoLocalidade) : null;

            if (configuracaoIntegracaoATSSmartWeb.Codigo > 0)
            {
                await repositorioIntegracaoATSSmartWeb.AtualizarAsync(configuracaoIntegracaoATSSmartWeb);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoATSSmartWeb.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a ATS Smart Web.", unidadeDeTrabalho);

                return;
            }

            await repositorioIntegracaoATSSmartWeb.InserirAsync(configuracaoIntegracaoATSSmartWeb);
        }

        private async Task SalvarConfiguracoesIntegracaoVSTrackAsync(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.VSTrack))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoVSTrack repositorioIntegracaoVSTrack = new Repositorio.Embarcador.Configuracoes.IntegracaoVSTrack(unidadeDeTrabalho, cancellationToken);

            IntegracaoVSTrack configuracaoIntegracaoVSTrack = await repositorioIntegracaoVSTrack.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoVSTrack == null)
                configuracaoIntegracaoVSTrack = new IntegracaoVSTrack();
            else
                configuracaoIntegracaoVSTrack.Initialize();

            configuracaoIntegracaoVSTrack.IntegracaoEtapa1Carga = Request.GetBoolParam("IntegracaoEtapa1CargaVSTrack");
            configuracaoIntegracaoVSTrack.IntegracaoEtapa6Carga = Request.GetBoolParam("IntegracaoEtapa6CargaVSTrack");

            configuracaoIntegracaoVSTrack.GrantType = Request.GetStringParam("GrantTypeVSTrack");
            configuracaoIntegracaoVSTrack.URLProducao = Request.GetStringParam("URLProducaoVSTrack");
            configuracaoIntegracaoVSTrack.URLHomologacao = Request.GetStringParam("URLHomologacaoVSTrack");
            configuracaoIntegracaoVSTrack.Password = Request.GetStringParam("PasswordVSTrack");
            configuracaoIntegracaoVSTrack.Username = Request.GetStringParam("UsernameVSTrack");

            if (configuracaoIntegracaoVSTrack.Codigo > 0)
            {
                await repositorioIntegracaoVSTrack.AtualizarAsync(configuracaoIntegracaoVSTrack);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoVSTrack.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a VS Track.", unidadeDeTrabalho);

                return;
            }

            await repositorioIntegracaoVSTrack.InserirAsync(configuracaoIntegracaoVSTrack);
        }

        private async Task SalvarConfiguracoesIntegracaoTrafegusAsync(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Trafegus))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repositorioConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = await repositorioConfiguracaoIntegracaoTrafegus.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoTrafegus == null)
                configuracaoIntegracaoTrafegus = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus();
            else
                configuracaoIntegracaoTrafegus.Initialize();

            configuracaoIntegracaoTrafegus.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoTrafegus");
            configuracaoIntegracaoTrafegus.PGR = Request.GetIntParam("PGRTrafegus");
            configuracaoIntegracaoTrafegus.Url = Request.GetStringParam("URLIntegracaoCargaTrafegus");
            configuracaoIntegracaoTrafegus.Usuario = Request.GetStringParam("UsuarioTrafegus");
            configuracaoIntegracaoTrafegus.Senha = Request.GetStringParam("SenhaTrafegus");

            if (configuracaoIntegracaoTrafegus.Codigo > 0)
            {
                await repositorioConfiguracaoIntegracaoTrafegus.AtualizarAsync(configuracaoIntegracaoTrafegus);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoTrafegus.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Trafegus.", unitOfWork);
            }
            else
            {
                await repositorioConfiguracaoIntegracaoTrafegus.InserirAsync(configuracaoIntegracaoTrafegus);
            }
        }

        private async Task SalvarConfiguracoesIntegracaoYMSAsync(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.YMS))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoYMS repositorioConfiguracaoIntegracaoYMS = new Repositorio.Embarcador.Configuracoes.IntegracaoYMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYMS configuracaoIntegracaoYMS = await repositorioConfiguracaoIntegracaoYMS.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoYMS == null)
                configuracaoIntegracaoYMS = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoYMS();
            else
                configuracaoIntegracaoYMS.Initialize();

            configuracaoIntegracaoYMS.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoYMS");
            configuracaoIntegracaoYMS.URLAutenticacao = Request.GetStringParam("URLIntegracaoAutenticacaoYMS");
            configuracaoIntegracaoYMS.URLIntegracao = Request.GetStringParam("URLIntegracaoCriacaoYMS");
            configuracaoIntegracaoYMS.URLCancelamento = Request.GetStringParam("URLCancelamentoYMS");
            configuracaoIntegracaoYMS.Usuario = Request.GetStringParam("UsuarioYMS");
            configuracaoIntegracaoYMS.Senha = Request.GetStringParam("SenhaYMS");
            configuracaoIntegracaoYMS.TipoAutenticacaoYMS = Request.GetEnumParam<TipoAutenticacaoYMS>("TipoAutenticacaoYMS");
            configuracaoIntegracaoYMS.ParametrosAdicionais = Request.GetStringParam("ParametrosAdicionaisYMS");
            configuracaoIntegracaoYMS.URLIntegracaoAtualizacao = Request.GetStringParam("URLIntegracaoAtualizacaoYMS");

            if (configuracaoIntegracaoYMS.Codigo > 0)
            {
                await repositorioConfiguracaoIntegracaoYMS.AtualizarAsync(configuracaoIntegracaoYMS);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoYMS.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a B3", unitOfWork);
            }
            else
            {
                await repositorioConfiguracaoIntegracaoYMS.InserirAsync(configuracaoIntegracaoYMS);
            }
        }

        private async Task SalvarConfiguracoesIntegracaoSeniorAsync(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Senior))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoSenior repositorioIntegracaoSenior
                = new Repositorio.Embarcador.Configuracoes.IntegracaoSenior(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSenior integracaoSenior = await repositorioIntegracaoSenior.BuscarPrimeiroRegistroAsync();

            if (integracaoSenior == null)
                integracaoSenior = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSenior();
            else
                integracaoSenior.Initialize();

            integracaoSenior.PossuiIntegracao = Request.GetBoolParam("PossuiIntegracaoSenior");
            integracaoSenior.URLAutenticacao = Request.GetStringParam("URLAutenticacaoSenior");
            integracaoSenior.URLIntegracao = Request.GetStringParam("URLIntegracaoSenior");
            integracaoSenior.Usuario = Request.GetStringParam("UsuarioSenior");
            integracaoSenior.Senha = Request.GetStringParam("SenhaSenior");

            if (integracaoSenior.Codigo > 0)
            {
                await repositorioIntegracaoSenior.AtualizarAsync(integracaoSenior);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = integracaoSenior.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Senior", unitOfWork);
            }
            else
            {
                await repositorioIntegracaoSenior.InserirAsync(integracaoSenior);
            }
        }


        private async Task SalvarConfiguracoesIntegracaoHUBAsync(Integracao configuracaoIntegracao, UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.HUB))
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoHUB repositorioIntegracaoHUB = new Repositorio.Embarcador.Configuracoes.IntegracaoHUB(unidadeDeTrabalho, cancellationToken);

            IntegracaoHUB configuracaoIntegracaoHUB = await repositorioIntegracaoHUB.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoHUB == null)
                configuracaoIntegracaoHUB = new IntegracaoHUB();
            else
                configuracaoIntegracaoHUB.Initialize();

            configuracaoIntegracaoHUB.UrlAutenticacaoToken = Request.GetStringParam("UrlAutenticacaoTokenHUB");
            configuracaoIntegracaoHUB.UrlIntegracao = Request.GetStringParam("UrlIntegracaoHUB");

            configuracaoIntegracaoHUB.ConexaoServiceBUS = Request.GetStringParam("ConexaoServiceBUSHUB");
            configuracaoIntegracaoHUB.IdOrganizacao = Request.GetStringParam("IdOrganizacaoHUB");
            configuracaoIntegracaoHUB.ChaveSecreta = Request.GetStringParam("ChaveSecretaHUB");

            if (configuracaoIntegracaoHUB.Codigo > 0)
            {
                await repositorioIntegracaoHUB.AtualizarAsync(configuracaoIntegracaoHUB);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoHUB.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a VS Track.", unidadeDeTrabalho);

                return;
            }

            await repositorioIntegracaoHUB.InserirAsync(configuracaoIntegracaoHUB);
        }

        private async Task SalvarConfiguracoesIntegracaoSkymarkAsync(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (!_tipoIntegracaoExistentes.Contains(TipoIntegracao.Skymark))
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoSkymark repositorioConfiguracaoIntegracaoSkymark = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoSkymark(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoSkymark configuracaoIntegracaoSkymark = await repositorioConfiguracaoIntegracaoSkymark.BuscarPrimeiroRegistroAsync();

            if (configuracaoIntegracaoSkymark == null)
                configuracaoIntegracaoSkymark = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoSkymark();
            else
                configuracaoIntegracaoSkymark.Initialize();

            configuracaoIntegracaoSkymark.HabilitarIntegracao = Request.GetBoolParam("HabilitarIntegracaoSkymark");
            configuracaoIntegracaoSkymark.Integracao = Request.GetStringParam("CampoIntegracaoSkymark");
            configuracaoIntegracaoSkymark.Url = Request.GetStringParam("UrlSkymark");
            configuracaoIntegracaoSkymark.Contrato = Request.GetStringParam("ContratoSkymark");
            configuracaoIntegracaoSkymark.ChaveUm = Request.GetStringParam("ChaveUmSkymark");
            configuracaoIntegracaoSkymark.ChaveDois = Request.GetStringParam("ChaveDoisSkymark");

            if (configuracaoIntegracaoSkymark.Codigo > 0)
            {
                await repositorioConfiguracaoIntegracaoSkymark.AtualizarAsync(configuracaoIntegracaoSkymark);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = configuracaoIntegracaoSkymark.GetChanges();

                if (alteracoes.Count > 0)
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, configuracaoIntegracao, alteracoes, "Alterou a configuração de integração com a Skymark", unitOfWork);
            }
            else
            {
                await repositorioConfiguracaoIntegracaoSkymark.InserirAsync(configuracaoIntegracaoSkymark);
            }
        }

        #endregion Métodos Privados
    }
}
