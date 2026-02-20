using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicos.Embarcador.Configuracoes;
using SGTAdmin.Controllers;
using System.Collections;
using System.Diagnostics;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/Configuracao")]
    public class ConfiguracaoController : BaseController
    {
        #region Construtores

        public ConfiguracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        //LER AS INFORMAÇÕES ABAIXO:

        //FAVOR ADICIONAREM AS NOVAS CONFIGURAÇÕES EM TABELAS RELACIONADAS, JÁ TEM VÁRIAS CRIADAS, SEGUIR EXEMPLO CASO AINDA NÃO TENHA
        //QUANDO CRIAR TABELA NOVA, FAZER O INSERT DO PRIMEIRO REGISTRO
        //NÃO DEVE MAIS ADICIONAR NA TABELA PRINCIPAL T_CONFIGURACAO_EMBARCADOR

        #region Métodos Globais

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (!UsuarioPossuiPermissaoAlterarConfiguracoesSistema())
                    return new JsonpResult(true, false, "Seu usuário não tem permissão de alterar essas configurações.");

                if (this.ClienteAcesso?.URLHomologacao ?? false)
                {
                    if (Request.GetBoolParam("AvisoVencimetoEnvarEmail"))
                        return new JsonpResult(true, false, "Não é possível habilitar o envio de aviso de vencimento de fatura em ambiente de homologação");

                    if (Request.GetBoolParam("CobrancaEnvarEmail"))
                        return new JsonpResult(true, false, "Não é possível habilitar o envio de cobrança de fatura em ambiente de homologação");
                }

                if (Request.GetBoolParam("ambiente_PermitirInformarCapacidadeMaximaParaUploadArquivos"))
                {
                    int capacidadeMaxima = Request.GetIntParam("ambiente_CapacidadeMaximaParaUploadArquivos");

                    if (capacidadeMaxima < 0 || capacidadeMaxima > 25)
                        return new JsonpResult(true, false, "A capacidade máxima para upload deve ser entre 0 e 25.");
                }

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                config.Initialize();

                #region Preenche Configurações

                config.BloquearCancelamentoCargasComDataCarregamentoEDadosTransporteInformados = Request.GetBoolParam("BloquearCancelamentoCargasComDataCarregamentoEDadosTransporteInformados");
                config.ModeloVeicularCargaNaoObrigatorioMontagemCarga = Request.GetBoolParam("ModeloVeicularCargaNaoObrigatorioMontagemCarga");
                config.MotoristaObrigatorioMontagemCarga = Request.GetBoolParam("MotoristaObrigatorioMontagemCarga");
                config.VeiculoObrigatorioMontagemCarga = Request.GetBoolParam("VeiculoObrigatorioMontagemCarga");
                config.NaoPermiteEmitirCargaSemAverbacao = Request.GetBoolParam("NaoPermiteEmitirCargaSemAverbacao");
                config.ImportarValePedagioMDFECarga = Request.GetBoolParam("ImportarValePedagioMDFECarga");
                config.ValidarSituacaoEnvioProgramadoIntegracaoCanhoto = Request.GetBoolParam("ValidarSituacaoEnvioProgramadoIntegracaoCanhoto");
                config.NaoPermiteInformarValorMaiorTerceiroTabelaFrete = Request.GetBoolParam("NaoPermiteInformarValorMaiorTerceiroTabelaFrete");
                config.HabilitarFuncionalidadeProjetoNFTP = Request.GetBoolParam("HabilitarFuncionalidadeProjetoNFTP");
                config.PermitirConfirmacaoImpressaoME = Request.GetBoolParam("PermitirConfirmacaoImpressaoME");
                config.FiltrarBuscaVeiculosPorEmpresa = Request.GetBoolParam("FiltrarBuscaVeiculosPorEmpresa");
                config.PreencherMotoristaAutomaticamenteAoInformarVeiculo = Request.GetBoolParam("PreencherMotoristaAutomaticamenteAoInformarVeiculo");
                config.BuscarProdutoPredominanteNoPedido = Request.GetBoolParam("BuscarProdutoPredominanteNoPedido");
                config.ControlarCanhotosDasNFEs = Request.GetBoolParam("ControlarCanhotosDasNFEs");
                config.UtilizarNFeEmHomologacao = Request.GetBoolParam("UtilizarNFeEmHomologacao");
                config.UtilizarTempoCarregamentoPorPeriodo = Request.GetBoolParam("UtilizarTempoCarregamentoPorPeriodo");
                config.ExibirInformacoesAdicionaisChamado = Request.GetBoolParam("ExibirInformacoesAdicionaisChamado");
                config.ExibirNumeroCargaQuandoExistirCarregamento = Request.GetBoolParam("ExibirNumeroCargaQuandoExistirCarregamento");
                config.RetornarCanhotoParaPendenteAoReceberUmaNotaJaDigitalizada = Request.GetBoolParam("RetornarCanhotoParaPendenteAoReceberUmaNotaJaDigitalizada");
                config.AlterarDataCarregamentoEDescarregamentoPorPeriodo = Request.GetBoolParam("AlterarDataCarregamentoEDescarregamentoPorPeriodo");
                config.PermitirTrocarPedidoCarga = Request.GetBoolParam("PermitirTrocarPedidoCarga");
                config.PermitirAdicionarPedidoOutraFilialCarga = Request.GetBoolParam("PermitirAdicionarPedidoOutraFilialCarga");
                config.PermitirRemoverPedidoCargaComPendenciaDocumentos = Request.GetBoolParam("PermitirRemoverPedidoCargaComPendenciaDocumentos");
                config.PermitirAtualizarInicioViagem = Request.GetBoolParam("PermitirAtualizarInicioViagem");
                config.UtilizarSituacaoNaJanelaDescarregamento = Request.GetBoolParam("UtilizarSituacaoNaJanelaDescarregamento");
                config.ExibirCargaSemValorFreteJanelaCarregamentoTransportador = Request.GetBoolParam("ExibirCargaSemValorFreteJanelaCarregamentoTransportador");
                config.PermitirOperadorInformarValorFreteMaiorQueTabela = Request.GetBoolParam("PermitirOperadorInformarValorFreteMaiorQueTabela");
                config.PermitirRetornoAgNotasFiscais = Request.GetBoolParam("PermitirRetornoAgNotasFiscais");
                config.ObrigatorioInformarDadosContratoFrete = Request.GetBoolParam("ObrigatorioInformarDadosContratoFrete");
                config.ExibirKmUtilizadoContratoFretePorPeriodoVigenciaContrato = Request.GetBoolParam("ExibirKmUtilizadoContratoFretePorPeriodoVigenciaContrato");
                config.PermitirCancelamentoTotalCarga = Request.GetBoolParam("PermitirCancelamentoTotalCarga");
                config.PermitirCancelamentoTotalCargaViaWebService = Request.GetBoolParam("PermitirCancelamentoTotalCargaViaWebService");
                config.PermitirInformarDadosTransportadorCargaEtapaNFe = Request.GetBoolParam("PermitirInformarDadosTransportadorCargaEtapaNFe");
                config.UtilizarIntegracaoPedido = Request.GetBoolParam("UtilizarIntegracaoPedido");
                config.PermiteAdicionarNotaManualmente = Request.GetBoolParam("PermiteAdicionarNotaManualmente");
                config.ValidarValorCargaAoAdicionarNFe = Request.GetBoolParam("ValidarValorCargaAoAdicionarNFe");
                config.PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada = Request.GetBoolParam("PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada");
                config.IndicarIntegracaoNFe = Request.GetBoolParam("IndicarIntegracaoNFe");
                config.UtilizarPedagioBaseCalculoIcmsCteComplementarPorRegraEstado = Request.GetBoolParam("UtilizarPedagioBaseCalculoIcmsCteComplementarPorRegraEstado");
                config.UtilizarValorDescarga = Request.GetBoolParam("UtilizarValorDescarga");
                config.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos = Request.GetBoolParam("ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos");
                config.ExigirNotaFiscalParaCalcularFreteCarga = Request.GetBoolParam("ExigirNotaFiscalParaCalcularFreteCarga");
                config.ArmazenarXMLCTeEmArquivo = Request.GetBoolParam("ArmazenarXMLCTeEmArquivo");
                config.NumeroCargaSequencialUnico = Request.GetBoolParam("NumeroCargaSequencialUnico");
                config.UtilizarNumeroSequencialCargaNoCarregamento = Request.GetBoolParam("UtilizarNumeroSequencialCargaNoCarregamento");
                config.ManterOperacaoUnicaEmCargasAgrupadas = Request.GetBoolParam("ManterOperacaoUnicaEmCargasAgrupadas");
                config.RatearNumeroPalletsModeloVeiculoEntrePedidoPorPeso = Request.GetBoolParam("RatearNumeroPalletsModeloVeiculoEntrePedidoPorPeso");
                config.EnviarDocumentosAutomaticamenteParaImpressao = Request.GetBoolParam("EnviarDocumentosAutomaticamenteParaImpressao");
                config.EnviarMDFeAutomaticamenteParaImpressao = Request.GetBoolParam("EnviarMDFeAutomaticamenteParaImpressao");
                config.FiltrarAlcadasDoUsuario = Request.GetBoolParam("FiltrarAlcadasDoUsuario");
                config.NaoExecutarFecharCarga = Request.GetBoolParam("NaoExecutarFecharCarga");
                config.UsarMesmoNumeroPreCargaGerarCargaViaImportacao = Request.GetBoolParam("UsarMesmoNumeroPreCargaGerarCargaViaImportacao");
                config.CancelarCargaExistenteAutomaticamenteNaImportacaoDePedido = Request.GetBoolParam("CancelarCargaExistenteAutomaticamenteNaImportacaoDePedido");
                config.GerarCargaDeNotasRecebidasPorEmail = Request.GetBoolParam("GerarCargaDeNotasRecebidasPorEmail");
                config.SempreDuplicarCargaCancelada = Request.GetBoolParam("SempreDuplicarCargaCancelada");
                config.DefaultTrueDuplicarCarga = Request.GetBoolParam("DefaultTrueDuplicarCarga");
                config.ExigirChamadoParaAbrirOcorrencia = Request.GetBoolParam("ExigirChamadoParaAbrirOcorrencia");
                config.ImprimirObservacaoPedidoMDFe = Request.GetBoolParam("ImprimirObservacaoPedidoMDFe");
                config.PermitirSalvarDadosTransporteCargaSemSolicitarNFes = Request.GetBoolParam("PermitirSalvarDadosTransporteCargaSemSolicitarNFes");
                config.SolicitarNotasFiscaisAoSalvarDadosTransportador = Request.GetBoolParam("SolicitarNotasFiscaisAoSalvarDadosTransportador");
                config.GerarCargaTrajeto = Request.GetBoolParam("GerarCargaTrajeto");
                config.PermitirAlterarCargaHorarioCarregamentoInferiorAtual = Request.GetBoolParam("PermitirAlterarCargaHorarioCarregamentoInferiorAtual");
                config.PossuiWMS = Request.GetBoolParam("PossuiWMS");
                config.ExigeNumeroDeAprovadoresNasAlcadas = Request.GetBoolParam("ExigeNumeroDeAprovadoresNasAlcadas");
                config.ValidarTabelaFreteNoPedido = Request.GetBoolParam("ValidarTabelaFreteNoPedido");
                config.EncerrarMDFesDeOutrasViagensAutomaticamente = Request.GetBoolParam("EncerrarMDFesDeOutrasViagensAutomaticamente");
                config.EnviarEmailEncerramentoMDFeTransportador = Request.GetBoolParam("EnviarEmailEncerramentoMDFeTransportador");
                config.CargaTransbordoNaEtapaInicial = Request.GetBoolParam("CargaTransbordoNaEtapaInicial");
                config.SempreBuscaCTePorChaveEmIntegracaoViaWS = Request.GetBoolParam("SempreBuscaCTePorChaveEmIntegracaoViaWS");
                config.ValidarDataLiberacaoSeguradora = Request.GetBoolParam("ValidarDataLiberacaoSeguradora");
                config.ValidarDataLiberacaoSeguradoraVeiculo = Request.GetBoolParam("ValidarDataLiberacaoSeguradoraVeiculo");
                config.PermiteEmissaoCargaSomenteComTracao = Request.GetBoolParam("PermiteEmissaoCargaSomenteComTracao");
                config.InformarDadosChegadaVeiculoNoFluxoPatio = Request.GetBoolParam("InformarDadosChegadaVeiculoNoFluxoPatio");
                config.PermiteSelecionarQualquerNaturezaNFEntrada = Request.GetBoolParam("PermiteSelecionarQualquerNaturezaNFEntrada");
                config.IgnorarTipoContratoNoContratoFreteTransportador = Request.GetBoolParam("IgnorarTipoContratoNoContratoFreteTransportador");
                config.GerarCPFRandomicamenteDestinatarioImportacaoPlanilha = Request.GetBoolParam("GerarCPFRandomicamenteDestinatarioImportacaoPlanilha");
                config.ChamadoOcorrenciaUsaRemetente = Request.GetBoolParam("ChamadoOcorrenciaUsaRemetente");
                config.PadraoArmazenamentoFisicoCanhotoCTe = Request.GetBoolParam("PadraoArmazenamentoFisicoCanhotoCTe");
                config.NotaUnicaEmCargas = Request.GetBoolParam("NotaUnicaEmCargas");
                config.ControleComissaoPorTipoOperacao = Request.GetBoolParam("ControleComissaoPorTipoOperacao");

                //config.AverbarRedespacho = Request.GetBoolParam("AverbarRedespacho");
                //config.AverbarSubcontratacao = Request.GetBoolParam("AverbarSubcontratacao");
                config.ExigirCodigoIntegracaoTransportador = Request.GetBoolParam("ExigirCodigoIntegracaoTransportador");
                config.AcertoDeViagemComDiaria = Request.GetBoolParam("AcertoDeViagemComDiaria");
                config.AcertoDeViagemImpressaoDetalhada = Request.GetBoolParam("AcertoDeViagemImpressaoDetalhada");
                config.HabilitarFluxoEntregas = Request.GetBoolParam("HabilitarFluxoEntregas");
                config.UtilizarNumeroNotaFluxoEntregas = Request.GetBoolParam("UtilizarNumeroNotaFluxoEntregas");
                config.ExibirAprovadoresOcorrenciaPortalTransportador = Request.GetBoolParam("ExibirAprovadoresOcorrenciaPortalTransportador");
                config.ExibirCFOPCompra = Request.GetBoolParam("ExibirCFOPCompra");
                config.FinalizarViagemAnteriorAoEntrarFilaCarregamento = Request.GetBoolParam("FinalizarViagemAnteriorAoEntrarFilaCarregamento");
                config.UtilizarFilaCarregamento = Request.GetBoolParam("UtilizarFilaCarregamento");
                config.UtilizarFilaCarregamentoReversa = Request.GetBoolParam("UtilizarFilaCarregamentoReversa");
                config.MarcacaoFilaCarregamentoSomentePorVeiculo = Request.GetBoolParam("MarcacaoFilaCarregamentoSomentePorVeiculo");
                config.ExibirNumeroPedidoJanelaCarregamentoEDescarregamento = Request.GetBoolParam("ExibirNumeroPedidoJanelaCarregamentoEDescarregamento");
                config.UtilizarPreCargaJanelaCarregamento = Request.GetBoolParam("UtilizarPreCargaJanelaCarregamento");
                config.PermitirInformarTipoTransportadorPorDataCarregamentoJanelaCarregamento = Request.GetBoolParam("PermitirInformarTipoTransportadorPorDataCarregamentoJanelaCarregamento");
                config.PermitirImportarAlteracaoDataCarregamentoJanelaCarregamento = Request.GetBoolParam("PermitirImportarAlteracaoDataCarregamentoJanelaCarregamento");
                config.UtilizarDataCarregamentoDaJanelaCarregamentoAoSetarTransportadorPrioritarioPorRotaCarga = Request.GetBoolParam("UtilizarDataCarregamentoDaJanelaCarregamentoAoSetarTransportadorPrioritarioPorRotaCarga");
                config.NaoGerarSenhaAgendamento = Request.GetBoolParam("NaoGerarSenhaAgendamento");
                config.TrocarPreCargaPorCarga = Request.GetBoolParam("TrocarPreCargaPorCarga");
                config.UtilizarProtocoloDaPreCargaNaCarga = Request.GetBoolParam("UtilizarProtocoloDaPreCargaNaCarga");
                config.NaoExigeInformarDisponibilidadeDeVeiculo = Request.GetBoolParam("NaoExigeInformarDisponibilidadeDeVeiculo");
                config.PermitirDesagendarCargaJanelaCarregamento = Request.GetBoolParam("PermitirDesagendarCargaJanelaCarregamento");
                config.ValidarConjuntoVeiculoPermiteEntrarFilaCarregamentoMobile = Request.GetBoolParam("ValidarConjuntoVeiculoPermiteEntrarFilaCarregamentoMobile");
                config.ExibirResumoFilaCarregamentoSomentePorModeloVeicularCarga = Request.GetBoolParam("ExibirResumoFilaCarregamentoSomentePorModeloVeicularCarga");
                config.UtilizarControleVeiculoEmPatio = Request.GetBoolParam("UtilizarControleVeiculoEmPatio");
                config.ValidarTabelaFreteComDataAtual = Request.GetBoolParam("ValidarTabelaFreteComDataAtual");
                config.FluxoDePatioComoMonitoramento = Request.GetBoolParam("FluxoDePatioComoMonitoramento");
                config.UsarContratoFreteAditivo = Request.GetBoolParam("UsarContratoFreteAditivo");
                config.NotificarCanhotosPendentes = Request.GetBoolParam("NotificarCanhotosPendentes");
                config.ExigeAprovacaoDigitalizacaoCanhoto = Request.GetBoolParam("ExigeAprovacaoDigitalizacaoCanhoto");
                config.BaixarCanhotoAposAprovacaoDigitalizacao = Request.GetBoolParam("BaixarCanhotoAposAprovacaoDigitalizacao");
                config.PermitirAdicionarCargaFluxoPatio = Request.GetBoolParam("PermitirAdicionarCargaFluxoPatio");
                config.NaoExibirVariacaoContratoFrete = Request.GetBoolParam("NaoExibirVariacaoContratoFrete");
                config.ValidarToleranciaPesoModeloVeicular = Request.GetBoolParam("ValidarToleranciaPesoModeloVeicular");
                config.GerarDACTEOutrosDocumentosAutomaticamente = Request.GetBoolParam("GerarDACTEOutrosDocumentosAutomaticamente");
                config.CalcularFreteFilialEmissoraPorTabelaDeFrete = Request.GetBoolParam("CalcularFreteFilialEmissoraPorTabelaDeFrete");
                config.GerarCargaDeCTesRecebidosPorEmail = Request.GetBoolParam("GerarCargaDeCTesRecebidosPorEmail");
                config.UsarNumeroCargaParaNumeroCanhotoAvulso = Request.GetBoolParam("UsarNumeroCargaParaNumeroCanhotoAvulso");
                config.ExigirDatasValidadeCadastroMotorista = Request.GetBoolParam("ExigirDatasValidadeCadastroMotorista");
                config.NaoPermitirValorFreteLiquidoZerado = Request.GetBoolParam("NaoPermitirValorFreteLiquidoZerado");
                config.NaoEmitirCargaComValorZerado = Request.GetBoolParam("NaoEmitirCargaComValorZerado");
                config.AlterarEmpresaEmissoraAoAjustarParticipantes = Request.GetBoolParam("AlterarEmpresaEmissoraAoAjustarParticipantes");
                config.PermiteAdicionarPreCargaManual = Request.GetBoolParam("PermiteAdicionarPreCargaManual");
                config.DadosTransporteObrigatorioPreCarga = Request.GetBoolParam("DadosTransporteObrigatorioPreCarga");
                config.TransportadorObrigatorioPreCarga = Request.GetBoolParam("TransportadorObrigatorioPreCarga");
                config.LocalCarregamentoObrigatorioPreCarga = Request.GetBoolParam("LocalCarregamentoObrigatorioPreCarga");
                config.UtilizarNumeroPreCargaPorFilial = Request.GetBoolParam("UtilizarNumeroPreCargaPorFilial");
                config.ReplicarAjusteTabelaFreteTodasTabelas = Request.GetBoolParam("ReplicarAjusteTabelaFreteTodasTabelas");
                config.FronteiraObrigatoriaMontagemCarga = Request.GetBoolParam("FronteiraObrigatoriaMontagemCarga");
                config.TipoCargaObrigatorioMontagemCarga = Request.GetBoolParam("TipoCargaObrigatorioMontagemCarga");
                config.InformarPeriodoCarregamentoMontagemCarga = Request.GetBoolParam("InformarPeriodoCarregamentoMontagemCarga");
                config.TipoOperacaoObrigatorioMontagemCarga = Request.GetBoolParam("TipoOperacaoObrigatorioMontagemCarga");
                config.PermitirTiposOperacoesDistintasMontagemCarga = Request.GetBoolParam("PermitirTiposOperacoesDistintasMontagemCarga");
                config.TransportadorObrigatorioMontagemCarga = Request.GetBoolParam("TransportadorObrigatorioMontagemCarga");
                config.SimulacaoFreteObrigatorioMontagemCarga = Request.GetBoolParam("SimulacaoFreteObrigatorioMontagemCarga");
                config.UtilizarDistanciaRoteirizacaoCarregamentoNaCarga = Request.GetBoolParam("UtilizarDistanciaRoteirizacaoCarregamentoNaCarga");
                config.RoteirizacaoObrigatoriaMontagemCarga = Request.GetBoolParam("RoteirizacaoObrigatoriaMontagemCarga");
                config.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento = Request.GetBoolParam("SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento");
                config.InformaHorarioCarregamentoMontagemCarga = Request.GetBoolParam("InformaHorarioCarregamentoMontagemCarga");
                config.OcultaGerarCarregamentosMontagemCarga = Request.GetBoolParam("OcultaGerarCarregamentosMontagemCarga");
                config.LimparTelaAoSalvarMontagemCarga = Request.GetBoolParam("LimparTelaAoSalvarMontagemCarga");
                config.InformaApoliceSeguroMontagemCarga = Request.GetBoolParam("InformaApoliceSeguroMontagemCarga");
                config.FiltrarPorPedidoSemCarregamentoNaMontagemCarga = Request.GetBoolParam("FiltrarPorPedidoSemCarregamentoNaMontagemCarga");
                config.InformarTipoCondicaoPagamentoMontagemCarga = Request.GetBoolParam("InformarTipoCondicaoPagamentoMontagemCarga");
                config.ExigirTipoSeparacaoMontagemCarga = Request.GetBoolParam("ExigirTipoSeparacaoMontagemCarga");
                //config.UtilizarPlanoViagem = Request.GetBoolParam("UtilizarPlanoViagem");
                config.HabilitarRelatorioBoletimViagem = Request.GetBoolParam("HabilitarRelatorioBoletimViagem");
                config.HabilitarRelatorioDiarioBordo = Request.GetBoolParam("HabilitarRelatorioDiarioBordo");
                config.HabilitarRelatorioDeTroca = Request.GetBoolParam("HabilitarRelatorioDeTroca");
                config.LiberarPedidosParaMontagemCargaCancelada = Request.GetBoolParam("LiberarPedidosParaMontagemCargaCancelada");
                config.UtilizaChat = Request.GetBoolParam("UtilizaChat");
                config.UtilizarDadosTransporteCargaCancelada = Request.GetBoolParam("UtilizarDadosTransporteCargaCancelada");
                config.ObrigarVigenciaNoAjusteFrete = Request.GetBoolParam("ObrigarVigenciaNoAjusteFrete");
                config.UtilizarComponentesCliente = Request.GetBoolParam("UtilizarComponentesCliente");
                config.DesativarMultiplosMotoristasMontagemCarga = Request.GetBoolParam("DesativarMultiplosMotoristasMontagemCarga");
                config.AtivarNovaDefinicaoDoTomadorParaCargasFeederMontagemCarga = Request.GetBoolParam("AtivarNovaDefinicaoDoTomadorParaCargasFeederMontagemCarga");
                config.AuditarConsultasWebService = Request.GetBoolParam("AuditarConsultasWebService");
                config.RetornosDuplicidadeWSSubstituirPorSucesso = Request.GetBoolParam("RetornosDuplicidadeWSSubstituirPorSucesso");
                config.RetornarFalhaAdicionarCargaSeExistirCancelamentoCargaEmAberto = Request.GetBoolParam("RetornarFalhaAdicionarCargaSeExistirCancelamentoCargaEmAberto");
                config.OcultarBuscaRotaNaCarga = Request.GetBoolParam("OcultarBuscaRotaNaCarga");
                config.NaoValidarDataCancelamentoTituloNaBaixaTituloReceberPorCTe = Request.GetBoolParam("NaoValidarDataCancelamentoTituloNaBaixaTituloReceberPorCTe");
                config.ObrigatorioGeracaoBlocosParaCarregamento = Request.GetBoolParam("ObrigatorioGeracaoBlocosParaCarregamento");
                config.AtualizarProdutosPedidoPorIntegracao = Request.GetBoolParam("AtualizarProdutosPedidoPorIntegracao");
                config.AtualizarProdutosCarregamentoPorNota = Request.GetBoolParam("AtualizarProdutosCarregamentoPorNota");
                config.AtualizarPedidoPorIntegracao = Request.GetBoolParam("AtualizarPedidoPorIntegracao");
                config.EnviarEmailFluxoEntrega = Request.GetBoolParam("EnviarEmailFLuxoEntrega");
                config.InformarCienciaOperacaoDocumentoDestinado = Request.GetBoolParam("InformarCienciaOperacaoDocumentoDestinado");
                config.UtilizarAlcadaAprovacaoAlteracaoValorFrete = Request.GetBoolParam("UtilizarAlcadaAprovacaoAlteracaoValorFrete");
                config.UtilizarAlcadaAprovacaoValorTabelaFreteCarga = Request.GetBoolParam("UtilizarAlcadaAprovacaoValorTabelaFreteCarga");
                config.UtilizarAlcadaAprovacaoTabelaFrete = Request.GetBoolParam("UtilizarAlcadaAprovacaoTabelaFrete");
                config.UtilizarAlcadaAprovacaoCarregamento = Request.GetBoolParam("UtilizarAlcadaAprovacaoCarregamento");
                config.UtilizarAlcadaAprovacaoVeiculo = Request.GetBoolParam("UtilizarAlcadaAprovacaoVeiculo");
                config.ExibirSituacaoAjusteTabelaFrete = Request.GetBoolParam("ExibirSituacaoAjusteTabelaFrete");
                config.UtilizarAlcadaAprovacaoLiberacaoEscrituracaoPagamentoCarga = Request.GetBoolParam("UtilizarAlcadaAprovacaoLiberacaoEscrituracaoPagamentoCarga");
                config.UtilizarAlcadaAprovacaoPagamento = Request.GetBoolParam("UtilizarAlcadaAprovacaoPagamento");
                config.UtilizarAlcadaAprovacaoAlteracaoRegraICMS = Request.GetBoolParam("UtilizarAlcadaAprovacaoAlteracaoRegraICMS");
                config.ExibirValoresPedidosNaCarga = Request.GetBoolParam("ExibirValoresPedidosNaCarga");
                config.BloquearCamposTransportadorQuandoEtapaNotas = Request.GetBoolParam("BloquearCamposTransportadorQuandoEtapaNotas");
                config.ValidarServicoPendenteVeiculoExecucaoCarga = Request.GetBoolParam("ValidarServicoPendenteVeiculoExecucaoCarga");
                config.NaoExibirTitulosNaFatura = Request.GetBoolParam("NaoExibirTitulosNaFatura");
                config.NaoExibirNotasFiscaisNaFatura = Request.GetBoolParam("NaoExibirNotasFiscaisNaFatura");
                config.NaoValidarDataCancelamentoTituloNoFechamentoDaFatura = Request.GetBoolParam("NaoValidarDataCancelamentoTituloNoFechamentoDaFatura");
                config.ExigirRotaRoteirizadaNaCarga = Request.GetBoolParam("ExigirRotaRoteirizadaNaCarga");
                config.ExigirCargaRoteirizada = Request.GetBoolParam("ExigirCargaRoteirizada");
                config.TipoUltimoPontoRoteirizacao = Request.GetEnumParam<TipoUltimoPontoRoteirizacao>("TipoUltimoPontoRoteirizacao");
                config.UtilizarControlePallets = Request.GetBoolParam("UtilizarControlePallets");
                config.ArmazenamentoCanhotoComFilial = Request.GetBoolParam("ArmazenamentoCanhotoComFilial");
                config.NaoControlarKMLancadoNoDocumentoEntrada = Request.GetBoolParam("NaoControlarKMLancadoNoDocumentoEntrada");
                config.LancarDocumentoEntradaAbertoSeKMEstiverErrado = Request.GetBoolParam("LancarDocumentoEntradaAbertoSeKMEstiverErrado");
                config.VisualizarTodosItensOrdemCompraDocumentoEntrada = Request.GetBoolParam("VisualizarTodosItensOrdemCompraDocumentoEntrada");
                config.PermitirAutomatizarPagamentoTransportador = Request.GetBoolParam("PermitirAutomatizarPagamentoTransportador");
                config.LiberarSelecaoQualquerVeiculoJanelaTransportador = Request.GetBoolParam("LiberarSelecaoQualquerVeiculoJanelaTransportador");
                config.BloquearVeiculosComMdfeEmAberto = Request.GetBoolParam("BloquearVeiculosComMdfeEmAberto");
                config.LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao = Request.GetBoolParam("LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao");
                config.PermitirTransportadorAlterarModeloVeicular = Request.GetBoolParam("PermitirTransportadorAlterarModeloVeicular");
                config.UtilizarControleJornadaMotorista = Request.GetBoolParam("UtilizarControleJornadaMotorista");
                config.HabilitarFichaMotoristaTodos = Request.GetBoolParam("HabilitarFichaMotoristaTodos");
                config.UtilizarComissaoPorCargo = Request.GetBoolParam("UtilizarComissaoPorCargo");
                config.NaoEmitirDocumentoNasCargas = Request.GetBoolParam("NaoEmitirDocumentoNasCargas");
                config.PermiteBaixarCanhotoApenasComOcorrenciaEntrega = Request.GetBoolParam("PermiteBaixarCanhotoApenasComOcorrenciaEntrega");
                config.CamposSecundariosObrigatoriosPedido = Request.GetBoolParam("CamposSecundariosObrigatoriosPedido");
                config.ExibirPedidoDeColeta = Request.GetBoolParam("ExibirPedidoDeColeta");
                config.ExibirAssociacaoClientesNoPedido = Request.GetBoolParam("ExibirAssociacaoClientesNoPedido");
                config.GerarAutomaticamenteNumeroPedidoEmbarcardorNaoInformado = Request.GetBoolParam("GerarAutomaticamenteNumeroPedidoEmbarcardorNaoInformado");
                config.PermiteSelecionarRotaMontagemCarga = Request.GetBoolParam("PermiteSelecionarRotaMontagemCarga");
                config.ExigirInformarNotasFiscaisNoPedido = Request.GetBoolParam("ExigirInformarNotasFiscaisNoPedido");
                config.CamposSecundariosObrigatoriosOrdemServico = Request.GetBoolParam("CamposSecundariosObrigatoriosOrdemServico");
                config.ImportarCargasMultiEmbarcador = Request.GetBoolParam("ImportarCargasMultiEmbarcador");
                config.ForcarFiltroModeloNaConsultaVeiculo = Request.GetBoolParam("ForcarFiltroModeloNaConsultaVeiculo");
                config.NaoExibirOpcaoParaDelegar = Request.GetBoolParam("NaoExibirOpcaoParaDelegar");
                config.NaoPermitirDelegarAoUsuarioLogado = Request.GetBoolParam("NaoPermitirDelegarAoUsuarioLogado");
                config.RatearValorOcorrenciaPeloValorFreteCTeOriginal = Request.GetBoolParam("RatearValorOcorrenciaPeloValorFreteCTeOriginal");
                config.ExibirEspecieDocumentoCteComplementarOcorrencia = Request.GetBoolParam("ExibirEspecieDocumentoCteComplementarOcorrencia");
                config.ValidarVeiculoVinculadoContratoDeFrete = Request.GetBoolParam("ValidarVeiculoVinculadoContratoDeFrete");
                config.UtilizarContratoPrestacaoServico = Request.GetBoolParam("UtilizarContratoPrestacaoServico");
                config.PermitirAlterarLacres = Request.GetBoolParam("PermitirAlterarLacres");
                config.ExibirTipoLacre = Request.GetBoolParam("ExibirTipoLacre");
                config.NaoUtilizarUsuarioTransportadorTerceiro = Request.GetBoolParam("NaoUtilizarUsuarioTransportadorTerceiro");
                config.ExigePerfilUsuario = Request.GetBoolParam("ExigePerfilUsuario");
                config.UtilizarFreteFilialEmissoraEmbarcador = Request.GetBoolParam("UtilizarFreteFilialEmissoraEmbarcador");
                config.UtilizarPesoEmbalagemProdutoParaRateio = Request.GetBoolParam("UtilizarPesoEmbalagemProdutoParaRateio");
                config.CompararTabelasDeFreteParaCalculo = Request.GetBoolParam("CompararTabelasDeFreteParaCalculo");
                config.UsarPesoProdutoSumarizacaoCarga = Request.GetBoolParam("UsarPesoProdutoSumarizacaoCarga");
                config.CadastrarRotaAutomaticamente = Request.GetBoolParam("CadastrarRotaAutomaticamente");
                config.AbrirRateioDespesaVeiculoAutomaticamente = Request.GetBoolParam("AbrirRateioDespesaVeiculoAutomaticamente");
                config.RatearFretePedidosAposLiberarEmissaoSemNFe = Request.GetBoolParam("RatearFretePedidosAposLiberarEmissaoSemNFe");
                config.NaoUtilizarDeafultParaPagamentoDeTributos = Request.GetBoolParam("NaoUtilizarDeafultParaPagamentoDeTributos");
                config.AvancarCargaAoReceberNotasPorEmail = Request.GetBoolParam("AvancarCargaAoReceberNotasPorEmail");
                config.NaoComprarValePedagioViaIntegracaoSeInformadoManualmenteNaCarga = Request.GetBoolParam("NaoComprarValePedagioViaIntegracaoSeInformadoManualmenteNaCarga");
                config.BloquearDatasRetroativasPedido = Request.GetBoolParam("BloquearDatasRetroativasPedido");
                config.PermitirInformarDataRetiradaCtrnCarga = Request.GetBoolParam("PermitirInformarDataRetiradaCtrnCarga");
                config.PermitirInformarNumeroContainerCarga = Request.GetBoolParam("PermitirInformarNumeroContainerCarga");
                config.PermitirInformarTaraContainerCarga = Request.GetBoolParam("PermitirInformarTaraContainerCarga");
                config.PermitirInformarMaxGrossCarga = Request.GetBoolParam("PermitirInformarMaxGrossCarga");
                config.PermitirInformarAnexoContainerCarga = Request.GetBoolParam("PermitirInformarAnexoContainerCarga");
                config.PermitirInformarDatasCarregamentoCarga = Request.GetBoolParam("PermitirInformarDatasCarregamentoCarga");
                config.RoteirizarPorCidade = Request.GetBoolParam("RoteirizarPorCidade");
                config.ExibirFaixaTemperaturaNaCarga = Request.GetBoolParam("ExibirFaixaTemperaturaNaCarga");
                config.NaoGerarCarregamentoRedespacho = Request.GetBoolParam("NaoGerarCarregamentoRedespacho");
                config.UtilizarControleHigienizacao = Request.GetBoolParam("UtilizarControleHigienizacao");
                config.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem = Request.GetBoolParam("NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem");
                config.NaoSomarDistanciaPedidosIntegracao = Request.GetBoolParam("NaoSomarDistanciaPedidosIntegracao");
                config.BloquearBaixaParcialOuParcelamentoFatura = Request.GetBoolParam("BloquearBaixaParcialOuParcelamentoFatura");
                config.NaoValidarDadosBancariosContratoFrete = Request.GetBoolParam("NaoValidarDadosBancariosContratoFrete");
                config.AjustarTipoOperacaoPeloPeso = Request.GetBoolParam("AjustarTipoOperacaoPeloPeso");
                config.PermitirSalvarDadosParcialmenteInformadosEtapaTransportador = Request.GetBoolParam("PermitirSalvarDadosParcialmenteInformadosEtapaTransportador");
                config.ObterNovaNumeracaoAoDuplicarContratoFreteTerceiro = Request.GetBoolParam("ObterNovaNumeracaoAoDuplicarContratoFreteTerceiro");
                config.AcoplarMotoristaAoVeiculoAoSelecionarNaCarga = Request.GetBoolParam("AcoplarMotoristaAoVeiculoAoSelecionarNaCarga");
                config.GerarCIOTParaTodasAsCargas = Request.GetBoolParam("GerarCIOTParaTodasAsCargas");
                config.ExibirVariacaoNegativaContratoFreteTerceiro = Request.GetBoolParam("ExibirVariacaoNegativaContratoFreteTerceiro");
                config.PermiteEmitirCargaDiferentesOrigensParcialmente = Request.GetBoolParam("PermiteEmitirCargaDiferentesOrigensParcialmente");
                config.OrdenarCargasMobileCrescente = Request.GetBoolParam("OrdenarCargasMobileCrescente");
                config.CentroResultadoPedidoObrigatorio = Request.GetBoolParam("CentroResultadoPedidoObrigatorio");
                config.UtilizaMoedaEstrangeira = Request.GetBoolParam("UtilizaMoedaEstrangeira");
                config.InformarDataViagemExecutadaPedido = Request.GetBoolParam("InformarDataViagemExecutadaPedido");
                config.UtilizarDataEmissaoContratoParaMovimentoFinanceiro = Request.GetBoolParam("UtilizarDataEmissaoContratoParaMovimentoFinanceiro");
                config.NaoFinalizarCargasAutomaticamente = Request.GetBoolParam("NaoFinalizarCargasAutomaticamente");
                config.PermitirSelecionarReboquePedido = Request.GetBoolParam("PermitirSelecionarReboquePedido");
                config.PermiteEmitirCTeComplementarManualmente = Request.GetBoolParam("PermiteEmitirCTeComplementarManualmente");
                config.VincularNotasParciaisPedidoPorProcesso = Request.GetBoolParam("VincularNotasParciaisPedidoPorProcesso");
                config.NaoPermiteEmitirCargaSemSeguro = Request.GetBoolParam("NaoPermiteEmitirCargaSemSeguro");
                config.CriarNotaFiscalTransportePorDocumentoDestinado = Request.GetBoolParam("CriarNotaFiscalTransportePorDocumentoDestinado");
                config.UtilizarRotaFreteInformadoPedido = Request.GetBoolParam("UtilizarRotaFreteInformadoPedido");
                config.NaoAdicionarNumeroPedidoEmbarcadorObservacaoCTe = Request.GetBoolParam("NaoAdicionarNumeroPedidoEmbarcadorObservacaoCTe");
                config.EmitirComplementarRedespachoFilialEmissoraDiferenteUFOrigem = Request.GetBoolParam("EmitirComplementarRedespachoFilialEmissoraDiferenteUFOrigem");
                config.RetornarCTeIntulizadoNoFluxoCancelamento = Request.GetBoolParam("RetornarCTeIntulizadoNoFluxoCancelamento");
                config.ExibirJustificativaCancelamentoCarga = Request.GetBoolParam("ExibirJustificativaCancelamentoCarga");
                config.ArmazenarCentroCustoDestinatario = Request.GetBoolParam("ArmazenarCentroCustoDestinatario");
                config.UtilizarRegraICMSCTeSubcontratacao = Request.GetBoolParam("UtilizarRegraICMSCTeSubcontratacao");
                config.EmitirNFeRemessaNaCarga = Request.GetBoolParam("EmitirNFeRemessaNaCarga");
                config.PermitirAlterarDataCarregamentoCargaNoPedido = Request.GetBoolParam("PermitirAlterarDataCarregamentoCargaNoPedido");
                config.GerarCargaComAgrupamentoNaMontagemCargaComoCargaDeComplemento = Request.GetBoolParam("GerarCargaComAgrupamentoNaMontagemCargaComoCargaDeComplemento");
                config.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga = Request.GetBoolParam("PermiteAdicionarNFeRepetidaParaOutroPedidoCarga");
                config.UtilizarPesoPedidoParaRatearPesoNFeRepetida = Request.GetBoolParam("UtilizarPesoPedidoParaRatearPesoNFeRepetida");
                config.PermiteEncerrarMDFeEmitidoNoEmbarcador = Request.GetBoolParam("PermiteEncerrarMDFeEmitidoNoEmbarcador");
                config.AjustarDataContratoIgualDataFinalizacaoCarga = Request.GetBoolParam("AjustarDataContratoIgualDataFinalizacaoCarga");
                config.NaoBloquearCargaComProblemaIntegracaoGrMotoristaVeiculo = Request.GetBoolParam("NaoBloquearCargaComProblemaIntegracaoGrMotoristaVeiculo");
                config.AvisarMDFeEmitidoEmbarcadorSemSeguroValido = Request.GetBoolParam("AvisarMDFeEmitidoEmbarcadorSemSeguroValido");
                config.GerarCanhotoSempre = Request.GetBoolParam("GerarCanhotoSempre");
                config.OcultarInformacoesFaturamentoAcertoViagem = Request.GetBoolParam("OcultarInformacoesFaturamentoAcertoViagem");
                config.OcultarInformacoesResultadoViagemAcertoViagem = Request.GetBoolParam("OcultarInformacoesResultadoViagemAcertoViagem");
                config.GerarReciboAcertoViagemDetalhado = Request.GetBoolParam("GerarReciboAcertoViagemDetalhado");
                config.NaoPermitirAvancarCargaSemEstoque = Request.GetBoolParam("NaoPermitirAvancarCargaSemEstoque");
                config.GerarNFSeImportacaoEmbarcador = Request.GetBoolParam("GerarNFSeImportacaoEmbarcador");
                config.InformarAjusteManualCargasImportadasEmbarcador = Request.GetBoolParam("InformarAjusteManualCargasImportadasEmbarcador");
                config.FiltrarNotasCompativeisPeloDestinatario = Request.GetBoolParam("FiltrarNotasCompativeisPeloDestinatario");
                config.ExibirFiltrosNotasCompativeisCarga = Request.GetBoolParam("ExibirFiltrosNotasCompativeisCarga");
                config.UtilizarNotaFiscalExistenteNaImportacaoCTeEmbarcador = Request.GetBoolParam("UtilizarNotaFiscalExistenteNaImportacaoCTeEmbarcador");
                config.ControlarAgendamentoSKU = Request.GetBoolParam("ControlarAgendamentoSKU");
                config.NotificarCargaAgConfirmacaoTransportador = Request.GetBoolParam("NotificarCargaAgConfirmacaoTransportador");
                config.DeixarAbastecimentosMesmaDataHoraInconsistentes = Request.GetBoolParam("DeixarAbastecimentosMesmaDataHoraInconsistentes");
                config.GerarOcorrenciaComplementoSubcontratacao = Request.GetBoolParam("GerarOcorrenciaComplementoSubcontratacao");
                config.GerarPreviewDOCCOBFatura = Request.GetBoolParam("GerarPreviewDOCCOBFatura");
                config.NaoUtilizarRegraEntradaDocumentoGrupoNCM = Request.GetBoolParam("NaoUtilizarRegraEntradaDocumentoGrupoNCM");
                config.UtilizarLocalidadePrestacaoPedido = Request.GetBoolParam("UtilizarLocalidadePrestacaoPedido");
                config.PermiteAlterarRotaEmCargaFinalizada = Request.GetBoolParam("PermiteAlterarRotaEmCargaFinalizada");
                config.NaoPreencherSerieCTeManual = Request.GetBoolParam("NaoPreencherSerieCTeManual");
                config.NaoObrigarInformarSegmentoNoAcertoDeViagem = Request.GetBoolParam("NaoObrigarInformarSegmentoNoAcertoDeViagem");
                config.GerarReciboDetalhadoAcertoViagem = Request.GetBoolParam("GerarReciboDetalhadoAcertoViagem");
                config.NaoBuscarDataInicioViagemAcerto = Request.GetBoolParam("NaoBuscarDataInicioViagemAcerto");
                config.UtilizarWebServiceRestATM = Request.GetBoolParam("UtilizarWebServiceRestATM");
                config.UtilizarIntegracaoAverbacaoBradescoEmbarcador = Request.GetBoolParam("UtilizarIntegracaoAverbacaoBradescoEmbarcador");
                config.ExibirJanelaDescargaPorPeriodo = Request.GetBoolParam("ExibirJanelaDescargaPorPeriodo");
                config.NaoUtilizarSerieCargaCTeManual = Request.GetBoolParam("NaoUtilizarSerieCargaCTeManual");
                config.ConsiderarPedagioDescargaVariacaoContratoFreteTerceiro = Request.GetBoolParam("ConsiderarPedagioDescargaVariacaoContratoFreteTerceiro");
                config.NaoDuplicarCargaAoCancelarPorImportacaoXMLCTeCancelado = Request.GetBoolParam("NaoDuplicarCargaAoCancelarPorImportacaoXMLCTeCancelado");
                config.AverbarMDFe = Request.GetBoolParam("AverbarMDFe");
                config.PermitirGerarNotaMesmoPedidoCarga = Request.GetBoolParam("PermitirGerarNotaMesmoPedidoCarga");
                config.PermitirGerarNotaMesmaCarga = Request.GetBoolParam("PermitirGerarNotaMesmaCarga");
                //config.PermiteEnviarMesmaNotaMultiplosPedidosCarga = Request.GetBoolParam("PermiteEnviarMesmaNotaMultiplosPedidosCarga");

                config.NaoPermitirExclusaoPedido = Request.GetNullableBoolParam("NaoPermitirExclusaoPedido");
                config.CalcularFreteCargaJanelaCarregamentoTransportador = Request.GetBoolParam("CalcularFreteCargaJanelaCarregamentoTransportador");
                config.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela = Request.GetBoolParam("DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela");
                config.ExibirValorDetalhadoJanelaCarregamentoTransportador = Request.GetBoolParam("ExibirValorDetalhadoJanelaCarregamentoTransportador");
                config.ExibirLimiteCarregamento = Request.GetBoolParam("ExibirLimiteCarregamento");
                config.ExibirPrevisaoCarregamento = Request.GetBoolParam("ExibirPrevisaoCarregamento");
                config.ExibirDisponibilidadeFrotaCarregamento = Request.GetBoolParam("ExibirDisponibilidadeFrotaCarregamento");
                config.PermitirInformarLacreJanelaCarregamentoTransportador = Request.GetBoolParam("PermitirInformarLacreJanelaCarregamentoTransportador");
                config.PermitirRejeitarCargaJanelaCarregamentoTransportador = Request.GetBoolParam("PermitirRejeitarCargaJanelaCarregamentoTransportador");
                config.BloquearGeracaoCargaComJanelaCarregamentoExcedente = Request.GetBoolParam("BloquearGeracaoCargaComJanelaCarregamentoExcedente");
                config.ExibirTipoDeCargaNaAbaCarregamentoNaMontagemCarga = Request.GetBoolParam("ExibirTipoDeCargaNaAbaCarregamentoNaMontagemCarga");
                config.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga = Request.GetBoolParam("ExibirAbaDetalhePedidoExportacaoNaMontagemCarga");
                config.ValidarCapacidadeModeloVeicularCargaNaMontagemCarga = Request.GetBoolParam("ValidarCapacidadeModeloVeicularCargaNaMontagemCarga");
                config.ValidarValorMaximoPendentePagamento = Request.GetBoolParam("ValidarValorMaximoPendentePagamento");
                config.ObrigarMotivoSolicitacaoFrete = Request.GetBoolParam("ObrigarMotivoSolicitacaoFrete");
                config.RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais = Request.GetBoolParam("RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais");
                config.ObrigatorioInformarDataEnvioCanhoto = Request.GetBoolParam("ObrigatorioInformarDataEnvioCanhoto");
                config.PermitirDisponibilizarCargaParaTransportador = Request.GetBoolParam("PermitirDisponibilizarCargaParaTransportador");
                config.PermitirLiberarCargaSemNFe = Request.GetBoolParam("PermitirLiberarCargaSemNFe");
                config.AvancarEtapaDocumentosEmissaoAoVincularTodasNotasParciaisCarga = Request.GetBoolParam("AvancarEtapaDocumentosEmissaoAoVincularTodasNotasParciaisCarga");

                config.ExigirCategoriaCadastroPessoa = Request.GetBoolParam("ExigirCategoriaCadastroPessoa");
                config.NaoGerarContratoFreteParaCTeEmitidoNoEmbarcador = Request.GetBoolParam("NaoGerarContratoFreteParaCTeEmitidoNoEmbarcador");
                config.PossuiMonitoramento = Request.GetBoolParam("PossuiMonitoramento");
                config.UtilizarBuscaRotaFreteManualCarga = Request.GetBoolParam("UtilizarBuscaRotaFreteManualCarga");
                config.TipoOrdemServicoObrigatorio = Request.GetBoolParam("TipoOrdemServicoObrigatorio");
                config.ReplicarCadastroVeiculoIntegracaoTransportadorDiferente = Request.GetBoolParam("ReplicarCadastroVeiculoIntegracaoTransportadorDiferente");
                config.NaoPermitirCancelarCargaComInicioViagem = Request.GetBoolParam("NaoPermitirCancelarCargaComInicioViagem");
                config.GerarPDFCTeCancelado = Request.GetBoolParam("GerarPDFCTeCancelado");
                config.FiltrarCargasPorParteDoNumero = Request.GetBoolParam("FiltrarCargasPorParteDoNumero");
                config.NaoAtualizarPesoPedidoPelaNFe = Request.GetBoolParam("NaoAtualizarPesoPedidoPelaNFe");
                config.BuscarClientesCadastradosNaIntegracaoDaCarga = Request.GetBoolParam("BuscarClientesCadastradosNaIntegracaoDaCarga");
                config.UtilizarProdutosDiversosNaIntegracaoDaCarga = Request.GetBoolParam("UtilizarProdutosDiversosNaIntegracaoDaCarga");
                config.GerarPedidoImportacaoNotfisEtapaNFe = Request.GetBoolParam("GerarPedidoImportacaoNotfisEtapaNFe");
                config.NaoExigeAceiteTransportadorParaNFDebito = Request.GetBoolParam("NaoExigeAceiteTransportadorParaNFDebito");
                config.ExigirEmailPrincipalCadastroPessoa = Request.GetBoolParam("ExigirEmailPrincipalCadastroPessoa");
                config.ExigirEmailPrincipalCadastroTransportador = Request.GetBoolParam("ExigirEmailPrincipalCadastroTransportador");
                config.FixarOperadorContratouCarga = Request.GetBoolParam("FixarOperadorContratouCarga");
                config.ExibirPrioridadesAutorizacaoOcorrencia = Request.GetBoolParam("ExibirPrioridadesAutorizacaoOcorrencia");
                config.ImprimirPercursoMDFe = Request.GetBoolParam("ImprimirPercursoMDFe");
                config.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada = Request.GetBoolParam("NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada");
                config.AtualizarCargaComVeiculoMDFeManual = Request.GetBoolParam("AtualizarCargaComVeiculoMDFeManual");
                config.ReduzirRetencaoISSValorAReceberNFSManual = Request.GetBoolParam("ReduzirRetencaoISSValorAReceberNFSManual");
                config.ObrigarTerGuaritaParaLancamentoEFinalizacaoCarga = Request.GetBoolParam("ObrigarTerGuaritaParaLancamentoEFinalizacaoCarga");
                config.IncluirTodosAcrescimosEDescontosNoCalculoDeImpostos = Request.GetBoolParam("IncluirTodosAcrescimosEDescontosNoCalculoDeImpostos");
                config.CadastrarMotoristaMobileAutomaticamente = Request.GetBoolParam("CadastrarMotoristaMobileAutomaticamente");
                config.PermitirImportarOcorrencias = Request.GetBoolParam("PermitirImportarOcorrencias");
                config.BloquearCamposOcorrenciaImportadosDoAtendimento = Request.GetBoolParam("BloquearCamposOcorrenciaImportadosDoAtendimento");
                config.TipoImpressaoFatura = Request.GetEnumParam<TipoImpressaoFatura>("TipoImpressaoFatura");
                config.ValidarRENAVAMVeiculo = Request.GetBoolParam("ValidarRENAVAMVeiculo");
                config.ValidarPlacaVeiculo = Request.GetBoolParam("ValidarPlacaVeiculo");

                //Other request types
                config.DescricaoProdutoPredominatePadrao = Request.Params("DescricaoProdutoPredominatePadrao") ?? string.Empty;
                config.EmailsRetornoProblemaGerarCargaEmail = Request.Params("EmailsRetornoProblemaGerarCargaEmail") ?? string.Empty;
                config.EmailsAvisoVencimentoCotratoFrete = Request.Params("EmailsAvisoVencimentoCotratoFrete") ?? string.Empty;
                config.ObservacaoCTePadraoEmbarcador = Request.Params("ObservacaoCTePadraoEmbarcador") ?? string.Empty;
                config.ObservacaoMDFePadraoEmbarcador = Request.Params("ObservacaoMDFePadraoEmbarcador") ?? string.Empty;
                config.CSTCTeSubcontratacao = Request.Params("CSTCTeSubcontratacao") ?? string.Empty;
                config.CampoObsContribuinteCTeCargaRedespacho = Request.Params("CampoObsContribuinteCTeCargaRedespacho") ?? string.Empty;
                config.TextoObsContribuinteCTeCargaRedespacho = Request.Params("TextoObsContribuinteCTeCargaRedespacho") ?? string.Empty;
                config.RegraMontarNumeroPedidoEmbarcadorWebService = Request.GetStringParam("RegraMontarNumeroPedidoEmbarcadorWebService");
                config.MensagemPadraoInformarDadosTransporteJanelaCarregamentoTransportador = Request.GetStringParam("MensagemPadraoInformarDadosTransporteJanelaCarregamentoTransportador");
                config.TempoSegundosParaInicioEmissaoDocumentos = Request.GetIntParam("TempoSegundosParaInicioEmissaoDocumentos");
                config.NumeroTentativasConsultarCargasErroRoteirizacao = Request.GetIntParam("NumeroTentativasConsultarCargasErroRoteirizacao");
                config.TempoMinutosAguardarReconsultarCargasErroRoteirizacao = Request.GetIntParam("TempoMinutosAguardarReconsultarCargasErroRoteirizacao");
                config.RaioMaximoGeoLocalidadeGeoCliente = Request.GetIntParam("RaioMaximoGeoLocalidadeGeoCliente");
                config.MaximoThreadsMontagemCarga = Request.GetIntParam("MaximoThreadsMontagemCarga");
                config.DesabilitarVeiculosInutilizadosDias = Request.GetIntParam("DesabilitarVeiculosInutilizadosDias");
                config.DiasAvisoVencimentoCotratoFrete = Request.GetIntParam("DiasAvisoVencimentoCotratoFrete");
                config.PrazoSolicitacaoOcorrencia = Request.GetIntParam("PrazoSolicitacaoOcorrencia");
                config.NumeroCasasDecimaisQuantidadeProduto = Request.GetIntParam("NumeroCasasDecimaisQuantidadeProduto");
                config.MinutosToleranciaPrevisaoChegadaDocaCarregamento = Request.GetIntParam("MinutosToleranciaPrevisaoChegadaDocaCarregamento");
                config.INTPFAR_LimiteLinhasArquivoEDI = Request.GetIntParam("LimiteLinhasArquivoEDI");
                config.INTPFAR_LinhasNecessariasOutrasInformacoes = Request.GetIntParam("LinhasNecessariasOutrasInformacoes");
                config.INTPFAR_NumeroLinhasFeradasPorCTe = Request.GetIntParam("NumeroLinhasFeradasPorCTe");
                config.QuantidadeRegistrosGridDocumentoEntrada = Request.GetIntParam("QuantidadeRegistrosGridDocumentoEntrada");
                config.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento = Request.GetIntParam("TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento");
                config.FatorMetroCubicoProdutoEmbarcadorIntegracao = Request.GetIntParam("FatorMetroCubicoProdutoEmbarcadorIntegracao");
                config.MetroCubicoPorUnidadePedidoProdutoIntegracao = Request.GetBoolParam("MetroCubicoPorUnidadePedidoProdutoIntegracao");
                config.CubagemPorCaixa = Request.GetBoolParam("CubagemPorCaixa");
                config.TempoMinutosParaReenviarCancelamento = Request.GetIntParam("TempoMinutosParaReenviarCancelamento");
                config.MaxDownloadsPorVez = Request.GetIntParam("MaxDownloadsPorVez");
                config.NumeroTentativasReenvioCteRejeitado = Request.GetIntParam("NumeroTentativasReenvioCteRejeitado");
                config.NumeroTentativasReenvioRotaFrete = Request.GetIntParam("NumeroTentativasReenvioRotaFrete");
                config.QuantidadeCargaPedidoProcessamentoLote = Request.GetIntParam("QuantidadeCargaPedidoProcessamentoLote");
                config.TempoMinutosPermanenciaCliente = Request.GetIntParam("TempoMinutosPermanenciaCliente");
                config.TempoHorasParaRetornoCTeAposFinalizacaoEmissao = Request.GetIntParam("TempoHorasParaRetornoCTeAposFinalizacaoEmissao");
                config.TempoMinutosPermanenciaSubareaCliente = Request.GetIntParam("TempoMinutosPermanenciaSubareaCliente");
                config.VelocidadeMaximaExtremaEntrePosicoes = Request.GetIntParam("VelocidadeMaximaExtremaEntrePosicoes");
                config.RaioPadrao = Request.GetIntParam("RaioPadrao");
                config.TempoPadraoDeEntrega = Request.GetIntParam("TempoPadraoDeEntrega");
                config.TempoPadraoDeColetaParaCalcularPrevisao = Request.GetIntParam("TempoPadraoDeColetaParaCalcularPrevisao");
                config.TempoPadraoDeEntregaParaCalcularPrevisao = Request.GetIntParam("TempoPadraoDeEntregaParaCalcularPrevisao");

                config.JornadaDiariaMotorista = Request.GetNullableTimeParam("JornadaDiariaMotorista");

                config.SituacaoCargaAposConfirmacaoImpressao = Request.GetEnumParam<SituacaoCarga>("SituacaoCargaAposConfirmacaoImpressao");
                config.SituacaoCargaAposEmissaoDocumentos = Request.GetEnumParam<SituacaoCarga>("SituacaoCargaAposEmissaoDocumentos");
                config.SituacaoCargaAposIntegracao = Request.GetEnumParam<SituacaoCarga>("SituacaoCargaAposIntegracao");
                config.SituacaoCargaAposFinalizacaoDaCarga = Request.GetEnumParam<SituacaoCarga>("SituacaoCargaAposFinalizacaoDaCarga");
                config.SistemaIntegracaoPadraoCarga = Request.GetEnumParam<TipoIntegracao>("SistemaIntegracaoPadraoCarga");
                config.TipoEmissaoIntramunicipal = Request.GetEnumParam<TipoEmissaoIntramunicipal>("TipoEmissaoIntramunicipal");
                config.ObrigatorioRegrasOcorrencia = Request.GetEnumParam<SimNao>("ObrigatorioRegrasOcorrencia");
                config.TipoContratoFreteTerceiro = Request.GetEnumParam<TipoContratoFreteTerceiro>("TipoContratoFreteTerceiro");
                config.TipoChamado = Request.GetEnumParam<TipoChamado>("TipoChamado");
                config.TipoGeracaoTituloFatura = Request.GetEnumParam<TipoGeracaoTituloFatura>("TipoGeracaoTituloFatura");
                config.TipoMontagemCargaPadrao = Request.GetEnumParam<TipoMontagemCarga>("TipoMontagemCargaPadrao");
                config.TipoFiltroDataMontagemCarga = Request.GetEnumParam<TipoFiltroDataMontagemCarga>("TipoFiltroDataMontagemCarga");
                config.DiaSemanaNotificarCanhotosPendentes = Request.GetEnumParam<DiaSemana>("DiaSemanaNotificarCanhotosPendentes");
                config.DataCompetenciaDocumentoEntrada = Request.GetEnumParam<DataCompetenciaDocumentoEntrada>("DataCompetenciaDocumentoEntrada");
                config.DataEntradaDocumentoEntrada = Request.GetEnumParam<DataEntradaDocumentoEntrada>("DataEntradaDocumentoEntrada");
                config.TipoMovimentoPagamentoMotorista = Request.GetEnumParam<TipoMovimentoEntidade>("TipoMovimentoPagamentoMotorista");
                config.TipoMovimentoReversaoPagamentoMotorista = Request.GetEnumParam<TipoMovimentoEntidade>("TipoMovimentoReversaoPagamentoMotorista");
                config.TipoRestricaoPalletModeloVeicularCarga = Request.GetEnumParam<TipoRestricaoPalletModeloVeicularCarga>("TipoRestricaoPalletModeloVeicularCarga");
                config.TipoImpressaoPedido = Request.GetEnumParam<TipoImpressaoPedido>("TipoImpressaoPedido");
                config.TipoImpressaoPedidoPrestacaoServico = Request.GetEnumParam<TipoImpressaoPedidoPrestacaoServico>("TipoImpressaoPedidoPrestacaoServico");
                config.TipoFechamentoFrete = Request.GetEnumParam<TipoFechamentoFrete>("TipoFechamentoFrete");
                config.MonitorarPosicaoAtualVeiculo = Request.GetEnumParam<MonitorarPosicaoAtualVeiculo>("MonitorarPosicaoAtualVeiculo");
                config.TipoRecibo = Request.GetEnumParam<TipoRecibo>("TipoRecibo");
                config.Pais = Request.GetEnumParam<TipoPais>("Pais");
                config.TipoPagamentoContratoFrete = Request.GetEnumParam<TipoPagamentoContratoFrete>("TipoPagamentoContratoFrete");

                config.CombustivelPadrao = repProduto.BuscarPorCodigo(Request.GetIntParam("CombustivelPadrao"));
                config.PostoPadrao = repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("PostoPadrao"));
                config.ClienteContratoAditivo = repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("ClienteContratoAditivo"));
                config.GrupoPessoasDocumentosDestinados = repGrupoPessoas.BuscarPorCodigo(Request.GetIntParam("GrupoPessoasDocumentosDestinados"));
                config.ComponenteFreteComplementoFechamento = repComponenteFrete.BuscarPorCodigo(Request.GetIntParam("ComponenteFreteComplementoFechamento"));
                config.ComponenteFreteDescontoSeguro = repComponenteFrete.BuscarPorCodigo(Request.GetIntParam("ComponenteFreteDescontoSeguro"));
                config.ComponenteFreteDescontoFilial = repComponenteFrete.BuscarPorCodigo(Request.GetIntParam("ComponenteFreteDescontoFilial"));
                config.RemetentePadraoImportacaoPlanilhaPedido = repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("RemetentePadraoImportacaoPlanilhaPedido"));
                config.DestinatarioPadraoImportacaoPlanilhaPedido = repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("DestinatarioPadraoImportacaoPlanilhaPedido"));
                config.ModeloVeicularCargaPadraoImportacaoPedido = repModeloVeicularCarga.BuscarPorCodigo(Request.GetIntParam("ModeloVeicularCargaPadraoImportacaoPedido"));
                if (Request.GetIntParam("TipoOperacaoPadraoCargaDistribuidor") > 0)
                    config.TipoOperacaoPadraoCargaDistribuidor = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacaoPadraoCargaDistribuidor"));
                else
                    config.TipoOperacaoPadraoCargaDistribuidor = null;
                config.SituacaoJanelaCarregamentoPadraoPesquisa = Request.GetNullableIntParam("SituacaoJanelaCarregamentoPadraoPesquisa");
                config.TipoDeOcorrenciaCriacaoPedido = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(Request.GetIntParam("TipoDeOcorrenciaCriacaoPedido"));
                config.TipoDeOcorrenciaRecebimentoMercadoria = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(Request.GetIntParam("TipoDeOcorrenciaRecebimentoMercadoria"));
                config.TipoDeOcorrenciaReentrega = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(Request.GetIntParam("TipoDeOcorrenciaReentrega"));
                config.LocalManutencaoPadraoCheckList = repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("LocalManutencaoPadraoCheckList"));
                config.PlanoContaAdiantamentoFornecedor = repPlanoConta.BuscarPorCodigo(Request.GetIntParam("PlanoContaAdiantamentoFornecedor"));
                config.PlanoContaAdiantamentoCliente = repPlanoConta.BuscarPorCodigo(Request.GetIntParam("PlanoContaAdiantamentoCliente"));

                config.BloquearEmissaoComContratoFreteZerado = Request.GetBoolParam("BloquearEmissaoComContratoFreteZerado");
                config.UtilizaEmissaoMultimodal = Request.GetBoolParam("UtilizaEmissaoMultimodal");
                config.PadraoInclusaiISSDesmarcado = Request.GetBoolParam("PadraoInclusaiISSDesmarcado");
                config.PermitirEnviarNumeroPedidoEmbarcadorViaIntegracao = Request.GetBoolParam("PermitirEnviarNumeroPedidoEmbarcadorViaIntegracao");
                config.NotificarAlteracaoCargaAoOperador = Request.GetBoolParam("NotificarAlteracaoCargaAoOperador");
                config.UtilizarSequenciaNumeracaoCargasViaIntegracao = Request.GetBoolParam("UtilizarSequenciaNumeracaoCargasViaIntegracao");
                config.ValidarMunicipioDiferentePedido = Request.GetBoolParam("ValidarMunicipioDiferentePedido");
                config.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga = Request.GetBoolParam("UtilizarPercentualEmRelacaoValorFreteLiquidoCarga");
                config.UtilizarRegraICMSParaDescontarValorICMS = Request.GetBoolParam("UtilizarRegraICMSParaDescontarValorICMS");
                config.ValidarSomenteFreteLiquidoNaImportacaoCTe = Request.GetBoolParam("ValidarSomenteFreteLiquidoNaImportacaoCTe");
                config.ValidarPorRaizDoTransportadorNaImportacaoCTe = Request.GetBoolParam("ValidarPorRaizDoTransportadorNaImportacaoCTe");
                config.NaoValidarDadosParticipantesNaImportacaoCTe = Request.GetBoolParam("NaoValidarDadosParticipantesNaImportacaoCTe");
                config.ImportarPedidoDeixarCargaPendente = Request.GetBoolParam("ImportarPedidoDeixarCargaPendente");
                config.ValidarNotasParciaisEnvioEmissao = Request.GetBoolParam("ValidarNotasParciaisEnvioEmissao");
                config.PercentualImpostoFederal = Request.GetDecimalParam("PercentualImpostoFederal");
                config.PercentualAdiantamentoTerceiroPadrao = Request.GetDecimalParam("PercentualAdiantamentoTerceiroPadrao");
                config.PercentualMinimoAdiantamentoTerceiroPadrao = Request.GetDecimalParam("PercentualMinimoAdiantamentoTerceiroPadrao");
                config.PercentualMaximoAdiantamentoTerceiroPadrao = Request.GetDecimalParam("PercentualMaximoAdiantamentoTerceiroPadrao");
                config.NaoPermitirImpressaoContratoFretePendente = Request.GetBoolParam("NaoPermitirImpressaoContratoFretePendente");
                config.ExigirAceiteTermoUsoSistema = Request.GetBoolParam("ExigirAceiteTermoUsoSistema");
                config.PermitirDownloadDANFE = Request.GetBoolParam("PermitirDownloadDANFE");
                config.PermitirDownloadXmlEtapaNfe = Request.GetBoolParam("PermitirDownloadXmlEtapaNfe");
                config.VerificarNFeEmOutraCargaNaIntegracao = Request.GetBoolParam("VerificarNFeEmOutraCargaNaIntegracao");
                config.ValidarRemetenteDestinatarioUnicoIntegracaoCarga = Request.GetBoolParam("ValidarRemetenteDestinatarioUnicoIntegracaoCarga");
                config.AutomatizarGeracaoLoteEscrituracao = Request.GetBoolParam("AutomatizarGeracaoLoteEscrituracao");
                config.AutomatizarGeracaoLoteEscrituracaoCancelamento = Request.GetBoolParam("AutomatizarGeracaoLoteEscrituracaoCancelamento");
                config.AutomatizarGeracaoLotePagamento = Request.GetBoolParam("AutomatizarGeracaoLotePagamento");
                config.DesabilitarSaldoViagemAcerto = Request.GetBoolParam("DesabilitarSaldoViagemAcerto");
                config.NaoAdicionarCargasTransbordoAcertoViagem = Request.GetBoolParam("NaoAdicionarCargasTransbordoAcertoViagem");
                config.SomarSaldoAtualMotoristaNoAcerto = Request.GetBoolParam("SomarSaldoAtualMotoristaNoAcerto");
                config.BuscarAdiantamentosSemDataInicialAcertoViagem = Request.GetBoolParam("BuscarAdiantamentosSemDataInicialAcertoViagem");
                config.ExibirSaldoPrevistoAcertoViagem = Request.GetBoolParam("ExibirSaldoPrevistoAcertoViagem");
                config.JustificarEntregaForaDoRaio = Request.GetBoolParam("JustificarEntregaForaDoRaio");
                config.GerarCargaMDFeDestinado = Request.GetBoolParam("GerarCargaMDFeDestinado");
                config.LancarFolgaAutomaticamenteNoAcerto = Request.GetBoolParam("LancarFolgaAutomaticamenteNoAcerto");
                config.PermitirInformarDistanciaNoRedespacho = Request.GetBoolParam("PermitirInformarDistanciaNoRedespacho");
                config.NaoImprimirNotasBoletosComRecebedor = Request.GetBoolParam("NaoImprimirNotasBoletosComRecebedor");
                config.ObrigarFotoNaEntrega = Request.GetBoolParam("ObrigarFotoNaEntrega");
                config.ObrigarFotoNaDevolucao = Request.GetBoolParam("ObrigarFotoNaDevolucao");
                config.PermiteQRCodeMobile = Request.GetBoolParam("PermiteQRCodeMobile");
                config.UtilizaAppTrizy = Request.GetBoolParam("UtilizaAppTrizy");
                config.RegistrarChegadaAppEmMetodoDiferenteDoConfirmar = Request.GetBoolParam("RegistrarChegadaAppEmMetodoDiferenteDoConfirmar");
                config.ServerChatURL = Request.GetStringParam("ServerChatURL");
                config.ExibirEntregaAntesEtapaTransporte = Request.GetBoolParam("ExibirEntregaAntesEtapaTransporte");
                config.HorasCargaExibidaNoApp = Request.GetNullableIntParam("HorasCargaExibidaNoApp");
                config.NaoPermitirLancarOcorrenciasEmDuplicidadeNaSequencia = Request.GetBoolParam("NaoPermitirLancarOcorrenciasEmDuplicidadeNaSequencia");
                config.NaoPermitirLancarOcorrenciasDepoisDeOcorrenciaFinalGerada = Request.GetBoolParam("NaoPermitirLancarOcorrenciasDepoisDeOcorrenciaFinalGerada");
                config.GerarCargaDeMDFesNaoVinculadosACargas = Request.GetBoolParam("GerarCargaDeMDFesNaoVinculadosACargas");
                config.GerarCargaDeCTEsNaoVinculadosACargas = Request.GetBoolParam("GerarCargaDeCTEsNaoVinculadosACargas");
                config.CadastrarMotoristaEVeiculoAutomaticamenteCargaImportada = Request.GetBoolParam("CadastrarMotoristaEVeiculoAutomaticamenteCargaImportada");
                config.NaoExigirExpedidorNoRedespacho = Request.GetBoolParam("NaoExigirExpedidorNoRedespacho");
                config.RetornarDataCarregamentoDaCargaNaConsulta = Request.GetBoolParam("RetornarDataCarregamentoDaCargaNaConsulta");
                config.ValidarDestinatarioPedidoDiferentePreCarga = Request.GetBoolParam("ValidarDestinatarioPedidoDiferentePreCarga");
                config.InformarPercentualAdiantamentoCarga = Request.GetBoolParam("InformarPercentualAdiantamentoCarga");
                config.ExibirInformacoesBovinos = Request.GetBoolParam("ExibirInformacoesBovinos");
                config.FiltrarCargasSemDocumentosParaChamados = Request.GetBoolParam("FiltrarCargasSemDocumentosParaChamados");
                config.PermitirAssumirChamadoDeOutroResponsavel = Request.GetBoolParam("PermitirAssumirChamadoDeOutroResponsavel");
                config.UtilizaPgtoCanhoto = Request.GetBoolParam("UtilizaPgtoCanhoto");
                config.NaoCadastrarProdutoAutomaticamenteDocumentoEntrada = Request.GetBoolParam("NaoCadastrarProdutoAutomaticamenteDocumentoEntrada");
                config.PreencherUltimoKMEntradaGuaritaTMS = Request.GetBoolParam("PreencherUltimoKMEntradaGuaritaTMS");
                config.ConfirmarPagamentoMotoristaAutomaticamente = Request.GetBoolParam("ConfirmarPagamentoMotoristaAutomaticamente");

                config.GerarPagamentoBloqueado = Request.GetBoolParam("GerarPagamentoBloqueado");
                config.LiberarPagamentoAoConfirmarEntrega = Request.GetBoolParam("LiberarPagamentoAoConfirmarEntrega");
                config.ConfirmarEntregaDigitilizacaoCanhoto = Request.GetBoolParam("ConfirmarEntregaDigitilizacaoCanhoto");
                config.GerarSomenteDocumentosDesbloqueados = Request.GetBoolParam("GerarSomenteDocumentosDesbloqueados");
                config.HabilitarMultiplaSelecaoEmpresaNFSManual = Request.GetBoolParam("HabilitarMultiplaSelecaoEmpresaNFSManual");
                config.GerarTituloFolhaPagamento = Request.GetBoolParam("GerarTituloFolhaPagamento");
                config.NaoValidarGrupoPessoaNaIntegracao = Request.GetBoolParam("NaoValidarGrupoPessoaNaIntegracao");
                config.RetornarCargasAgrupadasCarregamento = Request.GetBoolParam("RetornarCargasAgrupadasCarregamento");
                config.PermitirPagamentoMotoristaSemCarga = Request.GetBoolParam("PermitirPagamentoMotoristaSemCarga");
                config.ValidarProprietarioVeiculoMovimentacaoPlaca = Request.GetBoolParam("ValidarProprietarioVeiculoMovimentacaoPlaca");
                config.BloquearFechamentoAbastecimentoSemplaca = Request.GetBoolParam("BloquearFechamentoAbastecimentoSemplaca");
                config.AgruparIntegracaoCargaComTipoOperacaoDiferente = Request.GetBoolParam("AgruparIntegracaoCargaComTipoOperacaoDiferente");
                config.NaoGerarCargaDePedidoSemTipoOperacao = Request.GetBoolParam("NaoGerarCargaDePedidoSemTipoOperacao");
                config.IniciarCadastroFuncionarioMotoristaSempreInativo = Request.GetBoolParam("IniciarCadastroFuncionarioMotoristaSempreInativo");
                config.NaoDescontarValorSaldoMotorista = Request.GetBoolParam("NaoDescontarValorSaldoMotorista");
                config.ValidarCargoConsultaFuncionario = Request.GetBoolParam("ValidarCargoConsultaFuncionario");
                config.ExigirDataAutorizacaoParaPagamento = Request.GetBoolParam("ExigirDataAutorizacaoParaPagamento");
                config.NaoProcessarTrocaAlvoViaMonitoramento = Request.GetBoolParam("NaoProcessarTrocaAlvoViaMonitoramento");
                config.GerarFluxoPatioPorCargaAgrupada = Request.GetBoolParam("GerarFluxoPatioPorCargaAgrupada");
                config.GerarFluxoPatioDestino = Request.GetBoolParam("GerarFluxoPatioDestino");
                config.GerarFluxoPatioAoFecharCarga = Request.GetBoolParam("GerarFluxoPatioAoFecharCarga");

                config.UtilizarDadosBancariosDaEmpresa = Request.GetBoolParam("UtilizarDadosBancariosDaEmpresa");
                config.NaoCalcularDIFALParaCSTNaoTributavel = Request.GetBoolParam("NaoCalcularDIFALParaCSTNaoTributavel");
                config.UtilizarParticipantesDaCargaPeloPedido = Request.GetBoolParam("UtilizarParticipantesDaCargaPeloPedido");
                config.UtilizaNumeroDeFrotaParaPesquisaDeVeiculo = Request.GetBoolParam("UtilizaNumeroDeFrotaParaPesquisaDeVeiculo");
                config.ExibirObservacaoAprovadorAutorizacaoOcorrencia = Request.GetBoolParam("ExibirObservacaoAprovadorAutorizacaoOcorrencia");

                config.ObservacaoGeralPedido = Request.GetStringParam("ObservacaoGeralPedido");
                config.BuscarPorCargaPedidoCargasPendentesIntegracao = Request.GetBoolParam("BuscarPorCargaPedidoCargasPendentesIntegracao");
                config.AtualizarRotasQuandoAlterarLocalizacaoCliente = Request.GetBoolParam("AtualizarRotasQuandoAlterarLocalizacaoCliente");
                config.GerarMonitoramentoParaCargaRetornoVazio = Request.GetBoolParam("GerarMonitoramentoParaCargaRetornoVazio");
                config.AgruparCTesDiferentesPedidosMesmoDestinatario = Request.GetBoolParam("AgruparCTesDiferentesPedidosMesmoDestinatario");
                config.UtilizarCodificacaoUTF8ConversaoPDF = Request.GetBoolParam("UtilizarCodificacaoUTF8ConversaoPDF");
                config.SolicitarConfirmacaoPedidoSemMotoristaVeiculo = Request.GetBoolParam("SolicitarConfirmacaoPedidoSemMotoristaVeiculo");
                config.SolicitarConfirmacaoPedidoDuplicado = Request.GetBoolParam("SolicitarConfirmacaoPedidoDuplicado");
                config.SolicitarConfirmacaoMovimentoFinanceiroDuplicado = Request.GetBoolParam("SolicitarConfirmacaoMovimentoFinanceiroDuplicado");
                config.AtualizarVinculoVeiculoMotoristaIntegracaoCarga = Request.GetBoolParam("AtualizarVinculoVeiculoMotoristaIntegracaoCarga");
                config.AtualizarEnderecoMotoristaIntegracaoCarga = Request.GetBoolParam("AtualizarEnderecoMotoristaIntegracaoCarga");
                config.NaoObrigarDataSaidaRetornoPedido = Request.GetBoolParam("NaoObrigarDataSaidaRetornoPedido");
                config.MovimentarKMApenasPelaGuarita = Request.GetBoolParam("MovimentarKMApenasPelaGuarita");
                config.BloquearEmissaoCargaSemTempoRota = Request.GetBoolParam("BloquearEmissaoCargaSemTempoRota");
                config.NaoValidarCodigoBarrasBoletoTituloAPagar = Request.GetBoolParam("NaoValidarCodigoBarrasBoletoTituloAPagar");
                config.EncerrarMDFeAutomaticamente = Request.GetBoolParam("EncerrarMDFeAutomaticamente");
                config.GerarMovimentacaoNaBaixaIndividualmente = Request.GetBoolParam("GerarMovimentacaoNaBaixaIndividualmente");
                config.BloquearAlteracaoVeiculoPortalTransportador = Request.GetBoolParam("BloquearAlteracaoVeiculoPortalTransportador");
                config.UtilizarMesmoNumeroCRTCancelamentos = Request.GetBoolParam("UtilizarMesmoNumeroCRTCancelamentos");
                config.UtilizarMesmoNumeroMICDTACancelamentos = Request.GetBoolParam("UtilizarMesmoNumeroMICDTACancelamentos");
                config.UtilizarCRTAverbacao = Request.GetBoolParam("UtilizarCRTAverbacao");
                config.HabilitarEnvioAbastecimentoExterno = Request.GetBoolParam("HabilitarEnvioAbastecimentoExterno");
                config.AgruparUnidadesMedidasPorDescricao = Request.GetBoolParam("AgruparUnidadesMedidasPorDescricao");
                config.HabilitarFSDA = Request.GetBoolParam("HabilitarFSDA");
                config.UtilizarValorDescontatoComissaoMotoristaInfracao = Request.GetBoolParam("UtilizarValorDescontatoComissaoMotoristaInfracao");
                config.NaoFecharAcertoViagemAteReceberCanhotos = Request.GetBoolParam("NaoFecharAcertoViagemAteReceberCanhotos");
                config.PermiteInformarChamadosNoLancamentoOcorrencia = Request.GetBoolParam("PermiteInformarChamadosNoLancamentoOcorrencia");
                config.NaoExibirInfosAdicionaisGridPatio = Request.GetBoolParam("NaoExibirInfosAdicionaisGridPatio");
                config.ExibirColunaCodigosAgrupadosOcorrencia = Request.GetBoolParam("ExibirColunaCodigosAgrupadosOcorrencia");
                config.ExibirColunaValorFreteCargaOcorrencia = Request.GetBoolParam("ExibirColunaValorFreteCargaOcorrencia");
                config.ExibirAliquotaEtapaFreteCarga = Request.GetBoolParam("ExibirAliquotaEtapaFreteCarga");
                config.ExigirEmpresaTituloFinanceiro = Request.GetBoolParam("ExigirEmpresaTituloFinanceiro");
                config.UsarReciboPagamentoGeracaoAutorizacaoTitulo = Request.GetBoolParam("UsarReciboPagamentoGeracaoAutorizacaoTitulo");
                config.ObrigatorioCadastrarRastreadorNosVeiculos = Request.GetBoolParam("ObrigatorioCadastrarRastreadorNosVeiculos");
                config.IdentificarMonitoramentoStatusViagemEmTransito = Request.GetBoolParam("IdentificarMonitoramentoStatusViagemEmTransito");
                config.IdentificarMonitoramentoStatusViagemEmTransitoKM = Request.GetIntParam("IdentificarMonitoramentoStatusViagemEmTransitoKM");
                config.IdentificarMonitoramentoStatusViagemEmTransitoMinutos = Request.GetIntParam("IdentificarMonitoramentoStatusViagemEmTransitoMinutos");
                config.RealizarMovimentacaoPamcardProximoDiaUtil = Request.GetBoolParam("RealizarMovimentacaoPamcardProximoDiaUtil");
                config.ExibirOpcaoReenviarNotfisComFalhas = Request.GetBoolParam("ExibirOpcaoReenviarNotfisComFalhas");
                config.ExibirOpcaoDownloadPlanilhaRateioOcorrencia = Request.GetBoolParam("ExibirOpcaoDownloadPlanilhaRateioOcorrencia");
                config.RetornarCanhotosViaIntegracaoEmQualquerSituacao = Request.GetBoolParam("RetornarCanhotosViaIntegracaoEmQualquerSituacao");
                config.ExibirNumeroPagerEtapaInicialCarga = Request.GetBoolParam("ExibirNumeroPagerEtapaInicialCarga");
                config.RealizarMovimentacaoPamcardProximoDiaUtil = Request.GetBoolParam("RealizarMovimentacaoPamcardProximoDiaUtil");
                config.ExibirOpcaoReenviarNotfisComFalhas = Request.GetBoolParam("ExibirOpcaoReenviarNotfisComFalhas");
                config.ExibirOpcaoDownloadPlanilhaRateioOcorrencia = Request.GetBoolParam("ExibirOpcaoDownloadPlanilhaRateioOcorrencia");
                config.RetornarCanhotosViaIntegracaoEmQualquerSituacao = Request.GetBoolParam("RetornarCanhotosViaIntegracaoEmQualquerSituacao");
                config.IdentificarVeiculoParado = Request.GetBoolParam("IdentificarVeiculoParado");
                config.IdentificarVeiculoParadoDistancia = Request.GetIntParam("IdentificarVeiculoParadoDistancia");
                config.IdentificarVeiculoParadoTempo = Request.GetIntParam("IdentificarVeiculoParadoTempo");
                config.ExigirDataEntregaNotaClienteCanhotos = Request.GetBoolParam("ExigirDataEntregaNotaClienteCanhotos");
                config.DataBaseCalculoPrevisaoControleEntrega = Request.GetEnumParam<DataBaseCalculoPrevisaoControleEntrega>("DataBaseCalculoPrevisaoControleEntrega");
                config.PermitirAtualizarPrevisaoControleEntrega = Request.GetBoolParam("PermitirAtualizarPrevisaoControleEntrega");
                config.PermitirAtualizarPrevisaoEntregaPedidoControleEntrega = Request.GetBoolParam("PermitirAtualizarPrevisaoEntregaPedidoControleEntrega");
                config.NumeroSerieNotaDebitoPadrao = Request.GetIntParam("NumeroSerieNotaDebitoPadrao");
                config.NumeroSerieNotaCreditoPadrao = Request.GetIntParam("NumeroSerieNotaCreditoPadrao");
                config.RealizarMovimentacaPagamentoMotoristaPelaDataPagamento = Request.GetBoolParam("RealizarMovimentacaPagamentoMotoristaPelaDataPagamento");
                config.HabilitarControleFluxoNFeDevolucaoChamado = Request.GetBoolParam("HabilitarControleFluxoNFeDevolucaoChamado");
                config.ProcessarFilaDocumentosEmLote = Request.GetBoolParam("ProcessarFilaDocumentosEmLote");
                config.IncluirBCCompontentesDesconto = Request.GetBoolParam("IncluirBCCompontentesDesconto");
                config.HoraGeracaoCargaDeCTEsNaoVinculadosACargas = Request.GetTimeParam("HoraGeracaoCargaDeCTEsNaoVinculadosACargas");
                config.PrevisaoEntregaPeriodoUtilHorarioInicial = Request.GetTimeParam("PrevisaoEntregaPeriodoUtilHorarioInicial");
                config.PrevisaoEntregaPeriodoUtilHorarioFinal = Request.GetTimeParam("PrevisaoEntregaPeriodoUtilHorarioFinal");
                config.PrevisaoEntregaTempoUtilDiarioMinutos = Request.GetIntParam("PrevisaoEntregaTempoUtilDiarioMinutos");
                config.PrevisaoEntregaVelocidadeMediaVazio = Request.GetIntParam("PrevisaoEntregaVelocidadeMediaVazio");
                config.PrevisaoEntregaVelocidadeMediaCarregado = Request.GetIntParam("PrevisaoEntregaVelocidadeMediaCarregado");
                config.TokenSMS = Request.GetStringParam("TokenSMS");
                config.SenderSMS = Request.GetStringParam("SenderSMS");
                config.ExpressaoLacreContainer = Request.GetStringParam("ExpressaoLacreContainer");
                config.ApresentarCodigoIntegracaoComNomeFantasiaCliente = Request.GetBoolParam("ApresentarCodigoIntegracaoComNomeFantasiaCliente");
                config.LancarOsServicosDaOrdemDeServicoAutomaticamente = Request.GetBoolParam("LancarOsServicosDaOrdemDeServicoAutomaticamente");
                config.ExibirClassificacaoNFe = Request.GetBoolParam("ExibirClassificacaoNFe");
                config.ExibirSenhaCadastroPessoa = Request.GetBoolParam("ExibirSenhaCadastroPessoa");
                config.ExpedidorIgualRemetente = Request.GetBoolParam("ExpedidorIgualRemetente");
                config.RecebedorIgualDestinatario = Request.GetBoolParam("RecebedorIgualDestinatario");
                config.VisualizarTipoOperacaoDoPedidoPorTomador = Request.GetBoolParam("VisualizarTipoOperacaoDoPedidoPorTomador");
                config.UsarGrupoDeTipoDeOperacaoNoMonitoramento = Request.GetBoolParam("UsarGrupoDeTipoDeOperacaoNoMonitoramento");
                config.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem = Request.GetBoolParam("UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem");
                config.PadraoTagValePedagioVeiculos = Request.GetBoolParam("PadraoTagValePedagioVeiculos");
                config.NaoExibirPessoasChamado = Request.GetBoolParam("NaoExibirPessoasChamado");
                config.SalvarAnaliseEmAnexoAoLiberarOcorrenciaChamado = Request.GetBoolParam("SalvarAnaliseEmAnexoAoLiberarOcorrenciaChamado");
                config.ResponderAnaliseAoLiberarOcorrenciaChamado = Request.GetBoolParam("ResponderAnaliseAoLiberarOcorrenciaChamado");
                config.PermitirEstornarAprovacaoChamadoLiberado = Request.GetBoolParam("PermitirEstornarAprovacaoChamadoLiberado");
                config.UtilizaListaDinamicaDatasChamado = Request.GetBoolParam("UtilizaListaDinamicaDatasChamado");
                config.LinkVideoMobile = Request.GetStringParam("LinkVideoMobile");
                config.NaoFinalizarDocumentoEntradaOSValorDivergente = Request.GetBoolParam("NaoFinalizarDocumentoEntradaOSValorDivergente");
                config.NaoFinalizarDocumentoEntradaOrdemCompraValorDivergente = Request.GetBoolParam("NaoFinalizarDocumentoEntradaOrdemCompraValorDivergente");
                config.NaoFinalizarDocumentoEntradaComAbastecimentoInconsistente = Request.GetBoolParam("NaoFinalizarDocumentoEntradaComAbastecimentoInconsistente");
                config.AtualizarRotaRealizadaDoMonitoramento = Request.GetEnumParam<QuandoProcessarMonitoramento>("AtualizarRotaRealizadaDoMonitoramento");
                config.BuscarCargaPorNumeroPedido = Request.GetBoolParam("BuscarCargaPorNumeroPedido");
                config.GerarOSAutomaticamenteCadastroVeiculoEquipamento = Request.GetBoolParam("GerarOSAutomaticamenteCadastroVeiculoEquipamento");
                config.ValidarRaizCNPJGrupoPessoa = Request.GetBoolParam("ValidarRaizCNPJGrupoPessoa");
                config.PermiteOutrosOperadoresAlterarLancamentoProspeccao = Request.GetBoolParam("PermiteOutrosOperadoresAlterarLancamentoProspeccao");
                config.BloquearSemRegraAprovacaoOrdemServico = Request.GetBoolParam("BloquearSemRegraAprovacaoOrdemServico");
                config.NaoExibirRotaJanelaCarregamento = Request.GetBoolParam("NaoExibirRotaJanelaCarregamento");
                config.NaoExibirLocalCarregamentoJanelaCarregamento = Request.GetBoolParam("NaoExibirLocalCarregamentoJanelaCarregamento");
                config.ArmazenarPDFDANFE = Request.GetBoolParam("ArmazenarPDFDANFE");
                config.DistanciaMinimaPercorridaParaSaidaDoAlvo = Request.GetIntParam("DistanciaMinimaPercorridaParaSaidaDoAlvo");
                config.NaoControlarSituacaoVeiculoOrdemServico = Request.GetBoolParam("NaoControlarSituacaoVeiculoOrdemServico");
                config.PermitirAlterarDataPrevisaoEntregaPedidoNoCarga = Request.GetBoolParam("PermitirAlterarDataPrevisaoEntregaPedidoNoCarga");
                config.NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga = Request.GetBoolParam("NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga");
                config.KMLimiteEntreAbastecimentos = Request.GetIntParam("KMLimiteEntreAbastecimentos");
                config.HorimetroLimiteEntreAbastecimentos = Request.GetIntParam("HorimetroLimiteEntreAbastecimentos");
                config.TempoSemPosicaoParaVeiculoPerderSinal = Request.GetIntParam("TempoSemPosicaoParaVeiculoPerderSinal");
                config.NaoValidarTomadorCTeSubcontratacaoComTomadorPedido = Request.GetBoolParam("NaoValidarTomadorCTeSubcontratacaoComTomadorPedido");
                config.LimitarApenasUmMonitoramentoPorPlaca = Request.GetBoolParam("LimitarApenasUmMonitoramentoPorPlaca");
                config.RetornarCargaPendenciaEmissao = Request.GetBoolParam("RetornarCargaPendenciaEmissao");
                config.UtilizarValorFreteNota = Request.GetBoolParam("UtilizarValorFreteNota");
                config.ImportarEmailCliente = Request.GetBoolParam("ImportarEmailCliente");
                config.AvisarDivergenciaValoresCTeEmitidoEmbarcador = Request.GetBoolParam("AvisarDivergenciaValoresCTeEmitidoEmbarcador");
                config.AdicionarRelatorioRelacaoEntregaDownloadDocumentos = Request.GetBoolParam("AdicionarRelatorioRelacaoEntregaDownloadDocumentos");
                config.ExibirQuantidadeVolumesNF = Request.GetBoolParam("ExibirQuantidadeVolumesNF");
                config.ControlarEstoqueNegativo = Request.GetBoolParam("ControlarEstoqueNegativo");
                config.PermiteCadastrarLatLngEntregaLocalidade = Request.GetBoolParam("PermiteCadastrarLatLngEntregaLocalidade");
                config.NaoUtilizarDataTerminoProgramacaoVeiculo = Request.GetBoolParam("NaoUtilizarDataTerminoProgramacaoVeiculo");
                config.PermitirContatoWhatsApp = Request.GetBoolParam("PermitirContatoWhatsApp");
                config.NaoEncerrarViagemAoEncerrarControleEntrega = Request.GetBoolParam("NaoEncerrarViagemAoEncerrarControleEntrega");
                config.HabilitarWidgetAtendimento = Request.GetBoolParam("HabilitarWidgetAtendimento");
                config.FiltrarWidgetAtendimentoProFiltro = Request.GetBoolParam("FiltrarWidgetAtendimentoProFiltro");
                config.PermiteRemoverReentrega = Request.GetBoolParam("PermiteRemoverReentrega");
                config.UtilizarValidadeServicoPeloGrupoServicoOrdemServico = Request.GetBoolParam("UtilizarValidadeServicoPeloGrupoServicoOrdemServico");
                config.PermitirLancarAvariasSomenteParaProdutosDaCarga = Request.GetBoolParam("PermitirLancarAvariasSomenteParaProdutosDaCarga");
                config.OcultarOcorrenciasGeradasAutomaticamente = Request.GetBoolParam("OcultarOcorrenciasGeradasAutomaticamente");
                config.VisualizarValorNFSeDescontandoISSRetido = Request.GetBoolParam("VisualizarValorNFSeDescontandoISSRetido");
                config.TelaMonitoramentoApresentarCargasQuando = Request.GetEnumParam<QuandoProcessarMonitoramento>("TelaMonitoramentoApresentarCargasQuando");
                config.TelaCargaApresentarUltimaPosicaoVeiculo = Request.GetBoolParam("TelaCargaApresentarUltimaPosicaoVeiculo");
                config.NaoPreencherMotoristaVeiculoAbastecimento = Request.GetBoolParam("NaoPreencherMotoristaVeiculoAbastecimento");
                config.NaoValidarMediaIdealAbastecimento = Request.GetBoolParam("NaoValidarMediaIdealAbastecimento");
                config.NaoValidarMediaIdealDeArlaAbastecimento = Request.GetBoolParam("NaoValidarMediaIdealDeArlaAbastecimento");
                config.ValidarMesmoKMComLitrosDiferenteAbastecimento = Request.GetBoolParam("ValidarMesmoKMComLitrosDiferenteAbastecimento");
                config.NaoDeixarAbastecimentoTerceiroInconsistente = Request.GetBoolParam("NaoDeixarAbastecimentoTerceiroInconsistente");
                config.UsarDataChecklistVeiculo = Request.GetBoolParam("UsarDataChecklistVeiculo");
                config.UtilizaMultiplosLocaisArmazenamento = Request.GetBoolParam("UtilizaMultiplosLocaisArmazenamento");
                config.UtilizaSuprimentoDeGas = Request.GetBoolParam("UtilizaSuprimentoDeGas");
                config.PermiteImportarPlanilhaValoresFreteNFSManual = Request.GetBoolParam("PermiteImportarPlanilhaValoresFreteNFSManual");
                config.VisualizarDatasRaioNoAtendimento = Request.GetBoolParam("VisualizarDatasRaioNoAtendimento");
                config.ExigirClienteResponsavelPeloAtendimento = Request.GetBoolParam("ExigirClienteResponsavelPeloAtendimento");
                config.MonitorarPassagensFronteiras = Request.GetBoolParam("MonitorarPassagensFronteiras");
                config.ValidarLimiteCreditoNoPedido = Request.GetBoolParam("ValidarLimiteCreditoNoPedido");
                config.ObrigarSelecaoRotaQuandoExistirMultiplas = Request.GetBoolParam("ObrigarSelecaoRotaQuandoExistirMultiplas");
                config.AtivarEmissaoSubcontratacaoAgrupado = Request.GetBoolParam("AtivarEmissaoSubcontratacaoAgrupado");
                config.GerarOcorrenciaParaCargaAgrupada = Request.GetBoolParam("GerarOcorrenciaParaCargaAgrupada");
                config.AprovarAutomaticamenteCteEmitidoComValorInferiorAoEsperado = Request.GetBoolParam("AprovarAutomaticamenteCteEmitidoComValorInferiorAoEsperado");
                config.BloquearVeiculoExistenteEmCargaNaoFinalizada = Request.GetBoolParam("BloquearVeiculoExistenteEmCargaNaoFinalizada");
                config.AtivarFaturamentoAutomatico = Request.GetBoolParam("AtivarFaturamentoAutomatico");
                config.EnviarBoletoApenasParaEmailSecundario = Request.GetBoolParam("EnviarBoletoApenasParaEmailSecundario");
                config.FormaPreenchimentoCentroResultadoPedido = Request.GetEnumParam<FormaPreenchimentoCentroResultadoPedido>("FormaPreenchimentoCentroResultadoPedido");
                config.ValidarExistenciaDeConfiguracaoFaturaDoTomador = Request.GetBoolParam("ValidarExistenciaDeConfiguracaoFaturaDoTomador");
                config.ValidarConfiguracaoFaturamentoTomador = Request.GetBoolParam("ValidarConfiguracaoFaturamentoTomador");
                config.ExigirNumeroDocumentoTituloFinanceiro = Request.GetBoolParam("ExigirNumeroDocumentoTituloFinanceiro");
                config.UsaPermissaoControladorRelatorios = Request.GetBoolParam("UsaPermissaoControladorRelatorios");
                config.QuantidadeDiasLimiteVencimentoFaturaManual = Request.GetIntParam("QuantidadeDiasLimiteVencimentoFaturaManual");
                config.BloquearLancamentoServicoDuplicadoOrdemServico = Request.GetBoolParam("BloquearLancamentoServicoDuplicadoOrdemServico");
                config.MonitoramentoStatusViagemQuandoFicarSemStatusManterUltimo = Request.GetBoolParam("MonitoramentoStatusViagemQuandoFicarSemStatusManterUltimo");
                config.MonitoramentoConsiderarPosicaoTardiaParaAtualizarInicioFimEntregaViagem = Request.GetBoolParam("MonitoramentoConsiderarPosicaoTardiaParaAtualizarInicioFimEntregaViagem");
                config.TelaMonitoramentoPadraoFiltroDataInicialFinal = Request.GetBoolParam("TelaMonitoramentoPadraoFiltroDataInicialFinal");
                config.DiasAnterioresPesquisaCarga = Request.GetIntParam("DiasAnterioresPesquisaCarga");
                config.AoInativarMotoristaTransformarEmFuncionario = Request.GetBoolParam("AoInativarMotoristaTransformarEmFuncionario");
                config.CalcularFreteCliente = Request.GetBoolParam("CalcularFreteCliente");
                config.PermitirAtualizarModeloVeicularCargaDoVeiculoNoWebService = Request.GetBoolParam("PermitirAtualizarModeloVeicularCargaDoVeiculoNoWebService");
                config.NaoGerarCTesComValoresZerados = Request.GetBoolParam("NaoGerarCTesComValoresZerados");
                config.BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico = Request.GetBoolParam("BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico");
                config.GerarObservacaoRegraICMSAposObservacaoCTe = Request.GetBoolParam("GerarObservacaoRegraICMSAposObservacaoCTe");
                config.GerarComponentesDeFreteComImpostoIncluso = Request.GetBoolParam("GerarComponentesDeFreteComImpostoIncluso");
                config.ConsultarRegraICMSGeracaoCTeSubstitutoAutomaticamente = Request.GetBoolParam("ConsultarRegraICMSGeracaoCTeSubstitutoAutomaticamente");
                config.EnviarRegraExclusivaCodigoImpostoLayoutINTNC = Request.GetBoolParam("EnviarRegraExclusivaCodigoImpostoLayoutINTNC");
                config.DesconsiderarSobraRateioParaBaseCalculoIBSCBS = Request.GetBoolParam("DesconsiderarSobraRateioParaBaseCalculoIBSCBS");
                config.ValidarICMSTelaCotacaoPedidosRegraICMS = Request.GetBoolParam("ValidarICMSTelaCotacaoPedidosRegraICMS");
                config.PermitirInformarQuilometragemTabelaFreteCliente = Request.GetBoolParam("PermitirInformarQuilometragemTabelaFreteCliente");
                config.TabelaFretePrecisaoDinheiroDois = Request.GetBoolParam("TabelaFretePrecisaoDinheiroDois");
                config.DocumentoImpressaoPadraoCarga = Request.GetStringParam("DocumentoImpressaoPadraoCarga");
                config.NaoGerarAverbacaoCTeQuandoPedidoTiverAverbacao = Request.GetBoolParam("NaoGerarAverbacaoCTeQuandoPedidoTiverAverbacao");
                config.NaoConsiderarProdutosSemPesoParaSumarizarVolumes = Request.GetBoolParam("NaoConsiderarProdutosSemPesoParaSumarizarVolumes");
                config.NaoEmitirDocumentosEmCargasDeReentrega = Request.GetBoolParam("NaoEmitirDocumentosEmCargasDeReentrega");
                config.NaoPermitirInativarFuncionarioComSaldo = Request.GetBoolParam("NaoPermitirInativarFuncionarioComSaldo");
                config.UtilizarPesoLiquidoNFeParaCTeMDFe = Request.GetBoolParam("UtilizarPesoLiquidoNFeParaCTeMDFe");
                config.PermiteInformarModeloVeicularCargaOrigem = Request.GetBoolParam("PermiteInformarModeloVeicularCargaOrigem");
                config.NaoSolicitarAtuorizacaoAbastecimento = Request.GetBoolParam("NaoSolicitarAtuorizacaoAbastecimento");
                config.VelocidadeMinimaAceitaDasTecnologias = Request.GetDecimalParam("VelocidadeMinimaAceitaDasTecnologias");
                config.VelocidadeMaximaAceitaDasTecnologias = Request.GetDecimalParam("VelocidadeMaximaAceitaDasTecnologias");
                config.TemperaturaMinimaAceitaDasTecnologias = Request.GetDecimalParam("TemperaturaMinimaAceitaDasTecnologias");
                config.TemperaturaMaximaAceitaDasTecnologias = Request.GetDecimalParam("TemperaturaMaximaAceitaDasTecnologias");
                config.PreencherDataProgramadaComAtualCheckList = Request.GetBoolParam("PreencherDataProgramadaComAtualCheckList");
                config.TipoCalculoPercentualViagem = Request.GetEnumParam<TipoCalculoPercentualViagem>("TipoCalculoPercentualViagem");
                config.MonitoramentoStatusViagemTipoRegraParaCalcularPercentualViagem = Request.GetNullableEnumParam<MonitoramentoStatusViagemTipoRegra>("MonitoramentoStatusViagemTipoRegraParaCalcularPercentualViagem");
                config.SomenteAutorizadoresPodemDelegarOcorrencia = Request.GetBoolParam("SomenteAutorizadoresPodemDelegarOcorrencia");
                config.ExigirMotivoOcorrencia = Request.GetBoolParam("ExigirMotivoOcorrencia");
                config.NaoRetornarNotasEmDocumentoComplementar = Request.GetBoolParam("NaoRetornarNotasEmDocumentoComplementar");
                config.UtilizarEtiquetaDetalhadaWMS = Request.GetBoolParam("UtilizarEtiquetaDetalhadaWMS");
                config.GerarNumeracaoFaturaAnual = Request.GetBoolParam("GerarNumeracaoFaturaAnual");
                config.ValidarSeExisteVeiculoCadastradoComMesmoNrDeFrota = Request.GetBoolParam("ValidarSeExisteVeiculoCadastradoComMesmoNrDeFrota");
                config.HabilitarAlertaCargasParadas = Request.GetBoolParam("HabilitarAlertaCargasParadas");
                config.TempoMinutosAlertaCargasParadas = Request.GetIntParam("TempoMinutosAlertaCargasParadas");
                config.EmailsAlertaCargasParadas = Request.GetStringParam("EmailsAlertaCargasParadas");
                config.RetornarTitulosNaoGerados = Request.GetBoolParam("RetornarTitulosNaoGerados");
                config.NaoExigirMotivoAprovacaoCTeInconsistente = Request.GetBoolParam("NaoExigirMotivoAprovacaoCTeInconsistente");
                config.NaoRetornarCarregamentosSemData = Request.GetBoolParam("NaoRetornarCarregamentosSemData");
                config.EnviarEmailAnalistasChamado = Request.GetBoolParam("EnviarEmailAnalistasChamado");
                config.QuandoIniciarViagemViaMonitoramento = Request.GetNullableEnumParam<QuandoIniciarViagemViaMonitoramento>("QuandoIniciarViagemViaMonitoramento");
                config.ExigirTipoMovimentoLancamentoMovimentoFinanceiroManual = Request.GetBoolParam("ExigirTipoMovimentoLancamentoMovimentoFinanceiroManual");
                config.QuandoIniciarMonitoramento = Request.GetNullableEnumParam<QuandoIniciarMonitoramento>("QuandoIniciarMonitoramento");
                config.AcaoAoFinalizarMonitoramento = Request.GetNullableEnumParam<AcaoAoFinalizarMonitoramento>("AcaoAoFinalizarMonitoramento");
                config.FinalizarMonitoramentoEmAndamentoDoVeiculoAoIniciar = Request.GetBoolParam("FinalizarMonitoramentoEmAndamentoDoVeiculoAoIniciar");
                config.ExibirFiltroEColunaCodigoPedidoClienteGestaoDocumentos = Request.GetBoolParam("ExibirFiltroEColunaCodigoPedidoClienteGestaoDocumentos");
                config.UtilizarCentroResultadoNoRateioDespesaVeiculo = Request.GetBoolParam("UtilizarCentroResultadoNoRateioDespesaVeiculo");
                config.PedidoOcorrenciaColetaEntregaIntegracaoNova = Request.GetBoolParam("PedidoOcorrenciaColetaEntregaIntegracaoNova");
                config.HabilitarEstadoPassouRaioSemConfirmar = Request.GetBoolParam("HabilitarEstadoPassouRaioSemConfirmar");
                config.HabilitarIconeEntregaAtrasada = Request.GetBoolParam("HabilitarIconeEntregaAtrasada");
                config.TempoMinimoAcionarPassouRaioSemConfirmar = Request.GetIntParam("TempoMinimoAcionarPassouRaioSemConfirmar");
                config.RegistrarEntregasApenasAposAtenderTodasColetas = Request.GetBoolParam("RegistrarEntregasApenasAposAtenderTodasColetas");
                config.HabilitarDescontoGestaoDocumento = Request.GetBoolParam("HabilitarDescontoGestaoDocumento");
                config.CopiarDataTerminoCarregamentoCargaParaPrevisaoEntregaPedidos = Request.GetBoolParam("CopiarDataTerminoCarregamentoCargaParaPrevisaoEntregaPedidos");
                config.LiberarIntegracaoTransportadorDeCargaImportarDocumentoManual = Request.GetBoolParam("LiberarIntegracaoTransportadorDeCargaImportarDocumentoManual");
                config.NaoValidarTabelaFreteMesmaIncidenciaImportacao = Request.GetBoolParam("NaoValidarTabelaFreteMesmaIncidenciaImportacao");
                config.UsarAlcadaAprovacaoGestaoDocumentos = Request.GetBoolParam("UsarAlcadaAprovacaoGestaoDocumentos");
                config.GerarContratoTerceiroSemInformacaoDoFrete = Request.GetBoolParam("GerarContratoTerceiroSemInformacaoDoFrete");
                config.UtilizarMultiplosModelosVeicularesPedido = Request.GetBoolParam("UtilizarMultiplosModelosVeicularesPedido");
                config.SolicitarValorFretePorTonelada = Request.GetBoolParam("SolicitarValorFretePorTonelada");
                config.SolicitarAprovacaoFolgaAcertoViagem = Request.GetBoolParam("SolicitarAprovacaoFolgaAcertoViagem");
                config.ExibirPesoCargaEPesoCubadoGestaoDocumentos = Request.GetBoolParam("ExibirPesoCargaEPesoCubadoGestaoDocumentos");
                config.PermitirDarBaixaFaturasCTe = Request.GetBoolParam("PermitirDarBaixaFaturasCTe");
                config.PermitirConsultaDeValoresPedagio = Request.GetBoolParam("PermitirConsultaDeValoresPedagio");
                config.PermitirEnviarEmailAutorizacaoEmbarque = Request.GetBoolParam("PermitirEnviarEmailAutorizacaoEmbarque");
                config.PermitirCancelarPedidosSemDocumentos = Request.GetBoolParam("PermitirCancelarPedidosSemDocumentos");
                config.ExigirAnexosNoCadastroDoTransportador = Request.GetBoolParam("ExigirAnexosNoCadastroDoTransportador");
                config.VisualizarVeiculosPropriosETerceiros = Request.GetBoolParam("VisualizarVeiculosPropriosETerceiros");
                config.PermitirLancamentoOutrasDespesasDentroPeriodoAcerto = Request.GetBoolParam("PermitirLancamentoOutrasDespesasDentroPeriodoAcerto");
                config.VisualizarReciboPorMotoristaNoAcertoDeViagem = Request.GetBoolParam("VisualizarReciboPorMotoristaNoAcertoDeViagem");
                config.NaoObrigarInformarFrotaNoAcertoDeViagem = Request.GetBoolParam("NaoObrigarInformarFrotaNoAcertoDeViagem");
                config.PermitirAdicionarAnexosCheckListGestaoPatio = Request.GetBoolParam("PermitirAdicionarAnexosCheckListGestaoPatio");
                config.PercentualComissaoPadrao = Request.GetDecimalParam("PercentualComissaoPadrao");
                config.PercentualMediaEquivalente = Request.GetDecimalParam("PercentualMediaEquivalente");
                config.PercentualEquivaleEquivalente = Request.GetDecimalParam("PercentualEquivaleEquivalente");
                config.PercentualAdvertenciaEquivalente = Request.GetDecimalParam("PercentualAdvertenciaEquivalente");
                config.KMLimiteEntreAbastecimentosArla = Request.GetIntParam("KMLimiteEntreAbastecimentosARLA");
                config.NaoUsarPesoNotasPallet = Request.GetBoolParam("NaoUsarPesoNotasPallet");
                config.NaoExibirCodigoIntegracaoDoDestinatarioResumoCarga = Request.GetBoolParam("NaoExibirCodigoIntegracaoDoDestinatarioResumoCarga");

                string msg = ValidarParametrosPrevisaoEntrega(config);
                if (!string.IsNullOrWhiteSpace(msg)) return new JsonpResult(true, false, msg);

                string txtSituacaoCargaAcertoViagem = Request.GetStringParam("SituacaoCargaAcertoViagem");

                if (!string.IsNullOrWhiteSpace(txtSituacaoCargaAcertoViagem))
                {
                    List<int> situacaoCargaAcertoViagem = JsonConvert.DeserializeObject<List<int>>(txtSituacaoCargaAcertoViagem);
                    config.SituacaoCargaAcertoViagem = String.Join("; ", situacaoCargaAcertoViagem);
                    if (!string.IsNullOrEmpty(config.SituacaoCargaAcertoViagem))
                        config.SituacaoCargaAcertoViagem = config.SituacaoCargaAcertoViagem.Replace(" ", "");
                }
                else
                    config.SituacaoCargaAcertoViagem = "";

                preencherSistemasIntegracaoMotorista(unitOfWork, config);
                preencherSistemasIntegracaoVeiculo(unitOfWork, config);
                preencherPaisesMercosul(unitOfWork, config);
                #endregion

                await unitOfWork.StartAsync();

                SalvarConfiguracaoContratoFreteTerceiro(config, unitOfWork);
                SalvarConfiguracaoCanhoto(config, unitOfWork);
                SalvarConfiguracaoCargaEmissaoDocumento(config, unitOfWork);
                SalvarConfiguracaoCargaDadosTransporte(config, unitOfWork);
                SalvarConfiguracaoGeralCarga(config, unitOfWork);
                SalvarConfiguracaoCargaIntegracao(config, unitOfWork);
                SalvarConfiguracaoFatura(config, unitOfWork);
                SalvarConfiguracaoPedido(config, unitOfWork);
                SalvarConfiguracaoOcorrencia(config, unitOfWork);
                SalvarConfiguracaoWebService(config, unitOfWork);
                SalvarConfiguracaoTabelaFrete(config, unitOfWork);
                SalvarConfiguracaoJanelaCarregamento(config, unitOfWork);
                SalvarConfiguracaoDocumentoEntrada(config, unitOfWork);
                SalvarConfiguracaoMobile(config, unitOfWork);
                SalvarConfiguraoAcertoViagem(config, unitOfWork);
                SalvarConfiguracaoAprovacao(config, unitOfWork);
                SalvarConfiguracaoAgendamentoColeta(config, unitOfWork);
                SalvarConfiguracaoVeiculo(config, unitOfWork);
                SalvarConfiguracaoMotorista(config, unitOfWork);
                SalvarConfiguracaoEncerramentoMDFAutomatico(config, unitOfWork);
                SalvarConfiguracaoFinanceiro(config, unitOfWork);
                SalvarConfiguracaoChamado(config, unitOfWork);
                SalvarConfiguracaoControleEntrega(config, unitOfWork);
                SalvarConfiguracaoRedmine(config, unitOfWork);
                SalvarConfiguracaoCargaCalculoFrete(config, unitOfWork);
                SalvarConfiguracaoMontagemCarga(config, unitOfWork);
                SalvarConfiguracaoDocumentosDestinados(config, unitOfWork);
                SalvarConfiguracaoPaletes(config, unitOfWork);
                SalvarConfiguracaoMonitoramento(config, unitOfWork);
                SalvarConfiguracaoGeral(config, unitOfWork);
                SalvarConfiguracaoTransportador(config, unitOfWork);
                SalvarConfiguracaoRoteirizacao(config, unitOfWork);
                SalvarConfiguracaoFilaCarregamento(config, unitOfWork);
                SalvarConfiguracaoProduto(config, unitOfWork);
                SalvarConfiguracaoPortalMultiClifor(config, unitOfWork);
                SalvarConfiguracaoCalculoPrevisao(config, unitOfWork);
                SalvarConfiguracaoBidding(config, unitOfWork);
                SalvarConfiguracaoConfiguracaoFluxoPatio(config, unitOfWork);
                SalvarConfiguracaoConfiguracaoNFSeManual(config, unitOfWork);
                SalvarConfiguracaoConfiguracaoComissaoMotorista(config, unitOfWork);
                SalvarConfiguracaoInfracoes(config, unitOfWork);
                SalvarConfiguracaoAbastecimento(config, unitOfWork);
                SalvarConfiguracaoArquivo(config, unitOfWork);
                SalvarConfiguracaoAmbiente(config, unitOfWork);
                SalvarConfiguracaoPessoa(config, unitOfWork);
                SalvarConfiguracaoRelatorio(config, unitOfWork);
                SalvarConfiguracaoConfiguracaoEnvioEmailCobranca(config, unitOfWork);
                SalvarConfiguracaoMongo(config, unitOfWork);
                SalvarConfiguracaoSSo(config, unitOfWork);
                SalvarConfiguracaoMercosul(config, unitOfWork);
                SalvarConfiguracaoAgendamentoEntrega(config, unitOfWork);
                SalvarConfiguracaoAtendimentoAutomaticoDivergenciaValorFreteCTeEmitidosEmbarcador(config, unitOfWork);
                SalvarConfiguracaoAPIConexaoGeradorExcelKMM(config, unitOfWork);
                SalvarConfiguracaoCotacao(config, unitOfWork);
                SalvarConfiguracaoGeralCIOT(config, unitOfWork);
                SalvarConfiguracaoGeralTipoPagamentoCIOT(config, unitOfWork);
                SalvarConfiguracaoDownloadArquivos(config, unitOfWork);
                SalvarConfiguracaoPaginacaoInterfaces(config, unitOfWork);

                await repConfiguracaoTMS.AtualizarAsync(config, Auditado);

                if (Request.GetBoolParam("ExibirConfirmacao"))
                {
                    int quantidadeAlteracoes = config.GetChanges().Count + config.GetExternalChanges().Count;
                    if (quantidadeAlteracoes > 10)
                        return new JsonpResult(new { msg = $"Você alterou {quantidadeAlteracoes} Configurações. Realmente deseja salvar as alterações?" });
                }

                await unitOfWork.CommitChangesAsync();

                ConfigurationInstance.GetInstance(unitOfWork).AtualizarConfiguracoes(unitOfWork);

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(true, false, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(true, false, "Ocorreu uma falha ao salvar a configuração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                return new JsonpResult(new
                {
                    config.BloquearBaixaParcialOuParcelamentoFatura,
                    config.NaoPermiteEmitirCargaSemAverbacao,
                    config.ValidarSituacaoEnvioProgramadoIntegracaoCanhoto,
                    config.ImportarValePedagioMDFECarga,
                    config.NaoPermiteInformarValorMaiorTerceiroTabelaFrete,
                    config.HabilitarFuncionalidadeProjetoNFTP,
                    config.PermitirConfirmacaoImpressaoME,
                    config.FiltrarBuscaVeiculosPorEmpresa,
                    config.PreencherMotoristaAutomaticamenteAoInformarVeiculo,
                    config.BuscarProdutoPredominanteNoPedido,
                    config.ControlarCanhotosDasNFEs,
                    config.UtilizarNFeEmHomologacao,
                    config.UtilizarTempoCarregamentoPorPeriodo,
                    config.ExibirInformacoesAdicionaisChamado,
                    config.ExibirNumeroCargaQuandoExistirCarregamento,
                    config.RetornarCanhotoParaPendenteAoReceberUmaNotaJaDigitalizada,
                    config.AlterarDataCarregamentoEDescarregamentoPorPeriodo,
                    config.PermitirTrocarPedidoCarga,
                    config.PermitirAdicionarPedidoOutraFilialCarga,
                    config.PermitirRemoverPedidoCargaComPendenciaDocumentos,
                    config.PermitirAtualizarInicioViagem,
                    config.UtilizarSituacaoNaJanelaDescarregamento,
                    config.ExibirCargaSemValorFreteJanelaCarregamentoTransportador,
                    config.PermitirOperadorInformarValorFreteMaiorQueTabela,
                    config.PermitirRetornoAgNotasFiscais,
                    config.ObrigatorioInformarDadosContratoFrete,
                    config.ExibirKmUtilizadoContratoFretePorPeriodoVigenciaContrato,
                    config.PermitirCancelamentoTotalCarga,
                    config.PermitirCancelamentoTotalCargaViaWebService,
                    config.PermitirInformarDadosTransportadorCargaEtapaNFe,
                    config.UtilizarIntegracaoPedido,
                    config.PermiteAdicionarNotaManualmente,
                    config.ValidarValorCargaAoAdicionarNFe,
                    config.PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada,
                    config.IndicarIntegracaoNFe,
                    config.UtilizarPedagioBaseCalculoIcmsCteComplementarPorRegraEstado,
                    config.UtilizarValorDescarga,
                    config.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos,
                    config.ExigirNotaFiscalParaCalcularFreteCarga,
                    config.ArmazenarXMLCTeEmArquivo,
                    config.NumeroCargaSequencialUnico,
                    config.UtilizarNumeroSequencialCargaNoCarregamento,
                    config.ManterOperacaoUnicaEmCargasAgrupadas,
                    config.RatearNumeroPalletsModeloVeiculoEntrePedidoPorPeso,
                    config.EnviarDocumentosAutomaticamenteParaImpressao,
                    config.EnviarMDFeAutomaticamenteParaImpressao,
                    config.NaoExecutarFecharCarga,
                    config.UsarMesmoNumeroPreCargaGerarCargaViaImportacao,
                    config.CancelarCargaExistenteAutomaticamenteNaImportacaoDePedido,
                    config.GerarCargaDeNotasRecebidasPorEmail,
                    config.SempreDuplicarCargaCancelada,
                    config.DefaultTrueDuplicarCarga,
                    config.ExigirChamadoParaAbrirOcorrencia,
                    config.ImprimirObservacaoPedidoMDFe,
                    config.PermitirSalvarDadosTransporteCargaSemSolicitarNFes,
                    config.SolicitarNotasFiscaisAoSalvarDadosTransportador,
                    config.GerarCargaTrajeto,
                    config.PermitirAlterarCargaHorarioCarregamentoInferiorAtual,
                    config.PossuiWMS,
                    config.ValidarTabelaFreteNoPedido,
                    config.EncerrarMDFesDeOutrasViagensAutomaticamente,
                    config.EnviarEmailEncerramentoMDFeTransportador,
                    config.CargaTransbordoNaEtapaInicial,
                    config.SempreBuscaCTePorChaveEmIntegracaoViaWS,
                    config.ValidarDataLiberacaoSeguradora,
                    config.ValidarDataLiberacaoSeguradoraVeiculo,
                    config.PermiteEmissaoCargaSomenteComTracao,
                    config.PermiteSelecionarQualquerNaturezaNFEntrada,
                    config.IgnorarTipoContratoNoContratoFreteTransportador,
                    config.GerarCPFRandomicamenteDestinatarioImportacaoPlanilha,
                    config.ChamadoOcorrenciaUsaRemetente,
                    config.PadraoArmazenamentoFisicoCanhotoCTe,
                    config.NotaUnicaEmCargas,
                    config.ExigirCodigoIntegracaoTransportador,
                    config.AcertoDeViagemComDiaria,
                    config.AcertoDeViagemImpressaoDetalhada,
                    config.HabilitarFluxoEntregas,
                    config.UtilizarNumeroNotaFluxoEntregas,
                    config.ExibirAprovadoresOcorrenciaPortalTransportador,
                    config.ExibirCFOPCompra,
                    config.FinalizarViagemAnteriorAoEntrarFilaCarregamento,
                    config.UtilizarFilaCarregamento,
                    config.UtilizarPreCargaJanelaCarregamento,
                    config.PermitirInformarTipoTransportadorPorDataCarregamentoJanelaCarregamento,
                    config.PermitirImportarAlteracaoDataCarregamentoJanelaCarregamento,
                    config.UtilizarDataCarregamentoDaJanelaCarregamentoAoSetarTransportadorPrioritarioPorRotaCarga,
                    config.NaoGerarSenhaAgendamento,
                    config.TrocarPreCargaPorCarga,
                    config.UtilizarProtocoloDaPreCargaNaCarga,
                    config.NaoExigeInformarDisponibilidadeDeVeiculo,
                    config.PermitirDesagendarCargaJanelaCarregamento,
                    config.UtilizarFilaCarregamentoReversa,
                    config.MarcacaoFilaCarregamentoSomentePorVeiculo,
                    config.ExibirNumeroPedidoJanelaCarregamentoEDescarregamento,
                    config.ValidarConjuntoVeiculoPermiteEntrarFilaCarregamentoMobile,
                    config.ExibirResumoFilaCarregamentoSomentePorModeloVeicularCarga,
                    config.ValidarTabelaFreteComDataAtual,
                    config.UsarContratoFreteAditivo,
                    config.BloquearEmissaoComContratoFreteZerado,
                    config.NotificarCanhotosPendentes,
                    config.ExigeAprovacaoDigitalizacaoCanhoto,
                    config.BaixarCanhotoAposAprovacaoDigitalizacao,
                    config.NaoExibirVariacaoContratoFrete,
                    config.ValidarToleranciaPesoModeloVeicular,
                    config.GerarDACTEOutrosDocumentosAutomaticamente,
                    config.CalcularFreteFilialEmissoraPorTabelaDeFrete,
                    config.ExibirSituacaoAjusteTabelaFrete,
                    config.ExibirValoresPedidosNaCarga,
                    config.BloquearCamposTransportadorQuandoEtapaNotas,
                    config.ValidarServicoPendenteVeiculoExecucaoCarga,
                    config.NaoExibirTitulosNaFatura,
                    config.NaoExibirNotasFiscaisNaFatura,
                    config.ExigirRotaRoteirizadaNaCarga,
                    config.ExigirCargaRoteirizada,
                    config.TipoUltimoPontoRoteirizacao,
                    config.UtilizarControlePallets,
                    config.NaoValidarDataCancelamentoTituloNoFechamentoDaFatura,
                    config.CalcularFreteCargaJanelaCarregamentoTransportador,
                    config.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela,
                    config.ExibirValorDetalhadoJanelaCarregamentoTransportador,
                    config.ExibirLimiteCarregamento,
                    config.ExibirPrevisaoCarregamento,
                    config.ExibirDisponibilidadeFrotaCarregamento,
                    config.PermitirInformarLacreJanelaCarregamentoTransportador,
                    config.PermitirRejeitarCargaJanelaCarregamentoTransportador,
                    config.BloquearGeracaoCargaComJanelaCarregamentoExcedente,
                    config.ExibirTipoDeCargaNaAbaCarregamentoNaMontagemCarga,
                    config.ExibirAbaDetalhePedidoExportacaoNaMontagemCarga,
                    config.ValidarCapacidadeModeloVeicularCargaNaMontagemCarga,
                    config.ArmazenamentoCanhotoComFilial,
                    config.NaoControlarKMLancadoNoDocumentoEntrada,
                    config.LancarDocumentoEntradaAbertoSeKMEstiverErrado,
                    config.VisualizarTodosItensOrdemCompraDocumentoEntrada,
                    config.PermitirAutomatizarPagamentoTransportador,
                    config.NaoPermitirExclusaoPedido,
                    config.LiberarSelecaoQualquerVeiculoJanelaTransportador,
                    config.LiberarSelecaoQualquerVeiculoJanelaTransportadorComConfirmacao,
                    config.BloquearVeiculosComMdfeEmAberto,
                    config.PermitirTransportadorAlterarModeloVeicular,
                    config.NaoEmitirDocumentoNasCargas,
                    ValidarValorMaximoPendentePagamento = config.ValidarValorMaximoPendentePagamento ?? false,
                    config.ObrigarMotivoSolicitacaoFrete,
                    config.RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais,
                    config.DescricaoProdutoPredominatePadrao,
                    config.EmailsRetornoProblemaGerarCargaEmail,
                    config.EmailsAvisoVencimentoCotratoFrete,
                    config.ObservacaoCTePadraoEmbarcador,
                    config.ObservacaoMDFePadraoEmbarcador,
                    config.CSTCTeSubcontratacao,
                    config.CampoObsContribuinteCTeCargaRedespacho,
                    config.TextoObsContribuinteCTeCargaRedespacho,
                    config.ExigirCategoriaCadastroPessoa,
                    config.NaoGerarContratoFreteParaCTeEmitidoNoEmbarcador,
                    config.PossuiMonitoramento,
                    config.UtilizarBuscaRotaFreteManualCarga,
                    config.TipoOrdemServicoObrigatorio,
                    config.GerarCargaDeCTesRecebidosPorEmail,
                    config.UsarNumeroCargaParaNumeroCanhotoAvulso,
                    config.ExigirDatasValidadeCadastroMotorista,
                    config.NaoPermitirValorFreteLiquidoZerado,
                    config.NaoEmitirCargaComValorZerado,
                    config.AlterarEmpresaEmissoraAoAjustarParticipantes,
                    config.PermiteAdicionarPreCargaManual,
                    config.DadosTransporteObrigatorioPreCarga,
                    config.TransportadorObrigatorioPreCarga,
                    config.LocalCarregamentoObrigatorioPreCarga,
                    config.UtilizarNumeroPreCargaPorFilial,
                    config.ReplicarAjusteTabelaFreteTodasTabelas,
                    config.FronteiraObrigatoriaMontagemCarga,
                    config.TipoCargaObrigatorioMontagemCarga,
                    config.InformarPeriodoCarregamentoMontagemCarga,
                    config.TipoOperacaoObrigatorioMontagemCarga,
                    config.PermitirTiposOperacoesDistintasMontagemCarga,
                    config.TransportadorObrigatorioMontagemCarga,
                    config.SimulacaoFreteObrigatorioMontagemCarga,
                    config.UtilizarDistanciaRoteirizacaoCarregamentoNaCarga,
                    config.RoteirizacaoObrigatoriaMontagemCarga,
                    config.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento,
                    config.InformaHorarioCarregamentoMontagemCarga,
                    config.OcultaGerarCarregamentosMontagemCarga,
                    config.LimparTelaAoSalvarMontagemCarga,
                    config.InformaApoliceSeguroMontagemCarga,
                    config.FiltrarPorPedidoSemCarregamentoNaMontagemCarga,
                    config.InformarTipoCondicaoPagamentoMontagemCarga,
                    config.ExigirTipoSeparacaoMontagemCarga,
                    config.HabilitarRelatorioDeTroca,
                    //config.UtilizarPlanoViagem,
                    config.HabilitarRelatorioBoletimViagem,
                    config.HabilitarRelatorioDiarioBordo,
                    config.LiberarPedidosParaMontagemCargaCancelada,
                    config.ObrigarVigenciaNoAjusteFrete,
                    config.UtilizarComponentesCliente,
                    config.DesativarMultiplosMotoristasMontagemCarga,
                    config.AtivarNovaDefinicaoDoTomadorParaCargasFeederMontagemCarga,
                    config.UtilizaChat,
                    config.UtilizarDadosTransporteCargaCancelada,
                    config.AuditarConsultasWebService,
                    config.RetornosDuplicidadeWSSubstituirPorSucesso,
                    config.RetornarFalhaAdicionarCargaSeExistirCancelamentoCargaEmAberto,
                    config.OcultarBuscaRotaNaCarga,
                    config.NaoValidarDataCancelamentoTituloNaBaixaTituloReceberPorCTe,
                    config.ObrigatorioGeracaoBlocosParaCarregamento,
                    config.AtualizarProdutosPedidoPorIntegracao,
                    config.AtualizarProdutosCarregamentoPorNota,
                    config.AtualizarPedidoPorIntegracao,
                    config.EnviarEmailFluxoEntrega,
                    config.InformarCienciaOperacaoDocumentoDestinado,
                    config.PermiteBaixarCanhotoApenasComOcorrenciaEntrega,
                    config.CamposSecundariosObrigatoriosPedido,
                    config.ExibirPedidoDeColeta,
                    config.ExibirAssociacaoClientesNoPedido,
                    config.GerarAutomaticamenteNumeroPedidoEmbarcardorNaoInformado,
                    config.PermiteSelecionarRotaMontagemCarga,
                    config.ExigirInformarNotasFiscaisNoPedido,
                    config.CamposSecundariosObrigatoriosOrdemServico,
                    config.ImportarCargasMultiEmbarcador,
                    config.ForcarFiltroModeloNaConsultaVeiculo,
                    config.ReplicarCadastroVeiculoIntegracaoTransportadorDiferente,
                    config.UtilizarContratoPrestacaoServico,
                    config.ExibirTipoLacre,
                    config.PermitirAlterarLacres,
                    config.NaoUtilizarUsuarioTransportadorTerceiro,
                    config.ExigePerfilUsuario,
                    config.UtilizarFreteFilialEmissoraEmbarcador,
                    config.UtilizarPesoEmbalagemProdutoParaRateio,
                    config.CompararTabelasDeFreteParaCalculo,
                    config.CadastrarRotaAutomaticamente,
                    config.AbrirRateioDespesaVeiculoAutomaticamente,
                    config.RatearFretePedidosAposLiberarEmissaoSemNFe,
                    config.NaoUtilizarDeafultParaPagamentoDeTributos,
                    config.AvancarCargaAoReceberNotasPorEmail,
                    config.BloquearDatasRetroativasPedido,
                    config.PermitirInformarDataRetiradaCtrnCarga,
                    config.PermitirInformarNumeroContainerCarga,
                    config.PermitirInformarTaraContainerCarga,
                    config.PermitirInformarMaxGrossCarga,
                    config.PermitirInformarAnexoContainerCarga,
                    config.PermitirInformarDatasCarregamentoCarga,
                    config.RoteirizarPorCidade,
                    config.ExibirFaixaTemperaturaNaCarga,
                    config.NaoGerarCarregamentoRedespacho,
                    config.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem,
                    config.NaoValidarDadosBancariosContratoFrete,
                    config.AjustarTipoOperacaoPeloPeso,
                    config.PermitirSalvarDadosParcialmenteInformadosEtapaTransportador,
                    config.ObrigatorioInformarDataEnvioCanhoto,
                    config.PermitirDisponibilizarCargaParaTransportador,
                    config.PermitirLiberarCargaSemNFe,
                    config.UtilizarControleHigienizacao,
                    config.PermitirEnviarNumeroPedidoEmbarcadorViaIntegracao,
                    config.NotificarAlteracaoCargaAoOperador,
                    config.UtilizarSequenciaNumeracaoCargasViaIntegracao,
                    config.ValidarMunicipioDiferentePedido,
                    config.UtilizarPercentualEmRelacaoValorFreteLiquidoCarga,
                    config.UtilizarRegraICMSParaDescontarValorICMS,
                    config.ValidarSomenteFreteLiquidoNaImportacaoCTe,
                    config.ValidarPorRaizDoTransportadorNaImportacaoCTe,
                    config.NaoValidarDadosParticipantesNaImportacaoCTe,
                    config.ValidarNotasParciaisEnvioEmissao,
                    config.ImportarPedidoDeixarCargaPendente,
                    config.TempoSegundosParaInicioEmissaoDocumentos,
                    NumeroTentativasConsultarCargasErroRoteirizacao = (config.NumeroTentativasConsultarCargasErroRoteirizacao > 0) ? config.NumeroTentativasConsultarCargasErroRoteirizacao.ToString("n0") : "",
                    TempoMinutosAguardarReconsultarCargasErroRoteirizacao = (config.TempoMinutosAguardarReconsultarCargasErroRoteirizacao > 0) ? config.TempoMinutosAguardarReconsultarCargasErroRoteirizacao.ToString("n0") : "",
                    RaioMaximoGeoLocalidadeGeoCliente = (config.RaioMaximoGeoLocalidadeGeoCliente > 0) ? config.RaioMaximoGeoLocalidadeGeoCliente.ToString("n0") : "",
                    MaximoThreadsMontagemCarga = (config.MaximoThreadsMontagemCarga > 0 ? config.MaximoThreadsMontagemCarga.ToString("n0") : ""),
                    config.NumeroCasasDecimaisQuantidadeProduto,
                    config.DesabilitarVeiculosInutilizadosDias,
                    config.DiasAvisoVencimentoCotratoFrete,
                    config.PrazoSolicitacaoOcorrencia,
                    config.MinutosToleranciaPrevisaoChegadaDocaCarregamento,
                    LimiteLinhasArquivoEDI = config.INTPFAR_LimiteLinhasArquivoEDI,
                    LinhasNecessariasOutrasInformacoes = config.INTPFAR_LinhasNecessariasOutrasInformacoes,
                    NumeroLinhasFeradasPorCTe = config.INTPFAR_NumeroLinhasFeradasPorCTe,
                    QuantidadeRegistrosGridDocumentoEntrada = config.QuantidadeRegistrosGridDocumentoEntrada ?? 0,
                    config.TempoMinutosParaReenviarCancelamento,
                    config.MetroCubicoPorUnidadePedidoProdutoIntegracao,
                    config.CubagemPorCaixa,
                    config.MaxDownloadsPorVez,
                    config.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento,
                    config.FatorMetroCubicoProdutoEmbarcadorIntegracao,
                    config.NumeroTentativasReenvioCteRejeitado,
                    config.NumeroTentativasReenvioRotaFrete,
                    config.ProvisionarDocumentosEmitidos,
                    config.SituacaoCargaAposConfirmacaoImpressao,
                    config.SituacaoCargaAposEmissaoDocumentos,
                    config.SituacaoCargaAposIntegracao,
                    config.SituacaoCargaAposFinalizacaoDaCarga,
                    config.SistemaIntegracaoPadraoCarga,
                    config.TipoEmissaoIntramunicipal,
                    config.ObrigatorioRegrasOcorrencia,
                    config.TipoContratoFreteTerceiro,
                    config.TipoChamado,
                    config.TipoGeracaoTituloFatura,
                    config.TipoMontagemCargaPadrao,
                    config.TipoFiltroDataMontagemCarga,
                    config.DiaSemanaNotificarCanhotosPendentes,
                    config.DataCompetenciaDocumentoEntrada,
                    config.DataEntradaDocumentoEntrada,
                    config.TipoMovimentoPagamentoMotorista,
                    config.TipoMovimentoReversaoPagamentoMotorista,
                    config.TipoRestricaoPalletModeloVeicularCarga,
                    config.TipoImpressaoPedido,
                    config.TipoImpressaoPedidoPrestacaoServico,
                    config.TipoFechamentoFrete,
                    config.ValidarVeiculoVinculadoContratoDeFrete,
                    config.UsarPesoProdutoSumarizacaoCarga,
                    config.NaoSomarDistanciaPedidosIntegracao,
                    config.RatearValorOcorrenciaPeloValorFreteCTeOriginal,
                    config.ExibirEspecieDocumentoCteComplementarOcorrencia,
                    config.RegraMontarNumeroPedidoEmbarcadorWebService,
                    config.MensagemPadraoInformarDadosTransporteJanelaCarregamentoTransportador,
                    config.NaoPermitirCancelarCargaComInicioViagem,
                    config.GerarPDFCTeCancelado,
                    config.FiltrarCargasPorParteDoNumero,
                    config.NaoAtualizarPesoPedidoPelaNFe,
                    config.BuscarClientesCadastradosNaIntegracaoDaCarga,
                    config.UtilizarProdutosDiversosNaIntegracaoDaCarga,
                    config.GerarPedidoImportacaoNotfisEtapaNFe,
                    config.NaoExigeAceiteTransportadorParaNFDebito,
                    config.ExigirEmailPrincipalCadastroPessoa,
                    config.ExigirEmailPrincipalCadastroTransportador,
                    config.NaoPermitirImpressaoContratoFretePendente,
                    config.FixarOperadorContratouCarga,
                    config.ExibirPrioridadesAutorizacaoOcorrencia,
                    config.ExigirAceiteTermoUsoSistema,
                    config.PermitirDownloadDANFE,
                    config.PermitirDownloadXmlEtapaNfe,
                    config.VerificarNFeEmOutraCargaNaIntegracao,
                    config.ValidarRemetenteDestinatarioUnicoIntegracaoCarga,
                    config.AutomatizarGeracaoLoteEscrituracao,
                    config.AutomatizarGeracaoLoteEscrituracaoCancelamento,
                    config.AutomatizarGeracaoLotePagamento,
                    config.DesabilitarSaldoViagemAcerto,
                    config.NaoAdicionarCargasTransbordoAcertoViagem,
                    config.SomarSaldoAtualMotoristaNoAcerto,
                    config.BuscarAdiantamentosSemDataInicialAcertoViagem,
                    config.ExibirSaldoPrevistoAcertoViagem,
                    config.GerarCargaMDFeDestinado,
                    config.LancarFolgaAutomaticamenteNoAcerto,
                    config.PermitirInformarDistanciaNoRedespacho,
                    config.NaoImprimirNotasBoletosComRecebedor,
                    config.ObrigarFotoNaEntrega,
                    config.ObrigarFotoNaDevolucao,
                    config.PermiteQRCodeMobile,
                    config.UtilizaAppTrizy,
                    config.RegistrarChegadaAppEmMetodoDiferenteDoConfirmar,
                    config.ServerChatURL,
                    config.ExibirEntregaAntesEtapaTransporte,
                    config.HorasCargaExibidaNoApp,
                    config.GerarCargaDeMDFesNaoVinculadosACargas,
                    config.PadraoTagValePedagioVeiculos,
                    config.NaoExigirExpedidorNoRedespacho,
                    config.RetornarDataCarregamentoDaCargaNaConsulta,
                    config.ValidarDestinatarioPedidoDiferentePreCarga,
                    config.ControleComissaoPorTipoOperacao,

                    CombustivelPadrao = config.CombustivelPadrao != null ? new { config.CombustivelPadrao.Codigo, config.CombustivelPadrao.Descricao } : null,
                    PostoPadrao = config.PostoPadrao != null ? new { config.PostoPadrao.Codigo, config.PostoPadrao.Descricao } : null,
                    ClienteContratoAditivo = config.ClienteContratoAditivo != null ? new { config.ClienteContratoAditivo.Codigo, config.ClienteContratoAditivo.Descricao } : null,
                    GrupoPessoasDocumentosDestinados = config.GrupoPessoasDocumentosDestinados != null ? new { config.GrupoPessoasDocumentosDestinados.Codigo, config.GrupoPessoasDocumentosDestinados.Descricao } : null,
                    ComponenteFreteComplementoFechamento = config.ComponenteFreteComplementoFechamento != null ? new { config.ComponenteFreteComplementoFechamento.Codigo, config.ComponenteFreteComplementoFechamento.Descricao } : null,
                    ComponenteFreteDescontoSeguro = config.ComponenteFreteDescontoSeguro != null ? new { config.ComponenteFreteDescontoSeguro.Codigo, config.ComponenteFreteDescontoSeguro.Descricao } : null,
                    ComponenteFreteDescontoFilial = config.ComponenteFreteDescontoFilial != null ? new { config.ComponenteFreteDescontoFilial.Codigo, config.ComponenteFreteDescontoFilial.Descricao } : null,
                    RemetentePadraoImportacaoPlanilhaPedido = config.RemetentePadraoImportacaoPlanilhaPedido != null ? new { config.RemetentePadraoImportacaoPlanilhaPedido.Codigo, config.RemetentePadraoImportacaoPlanilhaPedido.Descricao } : null,
                    DestinatarioPadraoImportacaoPlanilhaPedido = config.DestinatarioPadraoImportacaoPlanilhaPedido != null ? new { config.DestinatarioPadraoImportacaoPlanilhaPedido.Codigo, config.DestinatarioPadraoImportacaoPlanilhaPedido.Descricao } : null,
                    ModeloVeicularCargaPadraoImportacaoPedido = config.ModeloVeicularCargaPadraoImportacaoPedido != null ? new { config.ModeloVeicularCargaPadraoImportacaoPedido.Codigo, config.ModeloVeicularCargaPadraoImportacaoPedido.Descricao } : null,
                    TipoOperacaoPadraoCargaDistribuidor = config.TipoOperacaoPadraoCargaDistribuidor != null ? new { config.TipoOperacaoPadraoCargaDistribuidor.Codigo, config.TipoOperacaoPadraoCargaDistribuidor.Descricao } : null,
                    TipoDeOcorrenciaReentrega = config.TipoDeOcorrenciaReentrega != null ? new { config.TipoDeOcorrenciaReentrega.Codigo, config.TipoDeOcorrenciaReentrega.Descricao } : null,
                    SituacaoJanelaCarregamentoPadraoPesquisa = config.SituacaoJanelaCarregamentoPadraoPesquisa.HasValue ? config.SituacaoJanelaCarregamentoPadraoPesquisa.Value.ToString() : "",
                    TipoDeOcorrenciaCriacaoPedido = config.TipoDeOcorrenciaCriacaoPedido != null ? new { config.TipoDeOcorrenciaCriacaoPedido.Codigo, config.TipoDeOcorrenciaCriacaoPedido.Descricao } : null,

                    TipoDeOcorrenciaRecebimentoMercadoria = config.TipoDeOcorrenciaRecebimentoMercadoria != null ? new { config.TipoDeOcorrenciaRecebimentoMercadoria.Codigo, config.TipoDeOcorrenciaRecebimentoMercadoria.Descricao } : null,
                    LocalManutencaoPadraoCheckList = config.LocalManutencaoPadraoCheckList != null ? new { config.LocalManutencaoPadraoCheckList.Codigo, config.LocalManutencaoPadraoCheckList.Descricao } : null,

                    config.PercentualImpostoFederal,
                    config.PercentualAdiantamentoTerceiroPadrao,
                    config.PercentualMinimoAdiantamentoTerceiroPadrao,
                    config.PercentualMaximoAdiantamentoTerceiroPadrao,

                    SituacaoCargaAcertoViagem = !string.IsNullOrWhiteSpace(config.SituacaoCargaAcertoViagem) ? Array.ConvertAll(config.SituacaoCargaAcertoViagem.Split(';'), int.Parse) : new int[0],
                    config.UtilizaEmissaoMultimodal,
                    config.PadraoInclusaiISSDesmarcado,
                    config.NaoPermitirLancarOcorrenciasEmDuplicidadeNaSequencia,
                    config.NaoPermitirLancarOcorrenciasDepoisDeOcorrenciaFinalGerada,
                    config.QuantidadeCargaPedidoProcessamentoLote,
                    config.ObterNovaNumeracaoAoDuplicarContratoFreteTerceiro,
                    config.ObservacaoGeralPedido,
                    config.ImprimirPercursoMDFe,
                    config.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada,
                    config.AtualizarCargaComVeiculoMDFeManual,
                    config.ReduzirRetencaoISSValorAReceberNFSManual,
                    config.ObrigarTerGuaritaParaLancamentoEFinalizacaoCarga,
                    config.IncluirTodosAcrescimosEDescontosNoCalculoDeImpostos,
                    config.CadastrarMotoristaMobileAutomaticamente,
                    config.AcoplarMotoristaAoVeiculoAoSelecionarNaCarga,
                    config.GerarCIOTParaTodasAsCargas,
                    config.ExibirVariacaoNegativaContratoFreteTerceiro,
                    config.OrdenarCargasMobileCrescente,
                    config.PermiteEmitirCargaDiferentesOrigensParcialmente,
                    config.InformarPercentualAdiantamentoCarga,
                    config.CentroResultadoPedidoObrigatorio,
                    config.ExibirInformacoesBovinos,
                    config.UtilizaMoedaEstrangeira,
                    config.FiltrarCargasSemDocumentosParaChamados,
                    config.PermitirAssumirChamadoDeOutroResponsavel,
                    config.UtilizaPgtoCanhoto,
                    config.NaoCadastrarProdutoAutomaticamenteDocumentoEntrada,
                    config.PreencherUltimoKMEntradaGuaritaTMS,
                    config.ConfirmarPagamentoMotoristaAutomaticamente,
                    config.GerarPagamentoBloqueado,
                    config.LiberarPagamentoAoConfirmarEntrega,
                    config.GerarSomenteDocumentosDesbloqueados,
                    config.HabilitarMultiplaSelecaoEmpresaNFSManual,
                    config.InformarDataViagemExecutadaPedido,
                    config.GerarTituloFolhaPagamento,
                    config.NaoValidarGrupoPessoaNaIntegracao,
                    config.RetornarCargasAgrupadasCarregamento,
                    config.PermitirPagamentoMotoristaSemCarga,
                    config.ValidarProprietarioVeiculoMovimentacaoPlaca,
                    config.BloquearFechamentoAbastecimentoSemplaca,
                    config.AgruparIntegracaoCargaComTipoOperacaoDiferente,
                    config.NaoGerarCargaDePedidoSemTipoOperacao,
                    config.TempoMinutosPermanenciaCliente,
                    config.TempoHorasParaRetornoCTeAposFinalizacaoEmissao,
                    config.TempoMinutosPermanenciaSubareaCliente,
                    config.VelocidadeMaximaExtremaEntrePosicoes,
                    config.IniciarCadastroFuncionarioMotoristaSempreInativo,
                    config.NaoDescontarValorSaldoMotorista,
                    config.ValidarCargoConsultaFuncionario,
                    config.ExigirDataAutorizacaoParaPagamento,
                    config.NaoProcessarTrocaAlvoViaMonitoramento,
                    config.UtilizarDataEmissaoContratoParaMovimentoFinanceiro,
                    config.NaoFinalizarCargasAutomaticamente,
                    config.UtilizarDadosBancariosDaEmpresa,
                    config.NaoCalcularDIFALParaCSTNaoTributavel,
                    config.UtilizarParticipantesDaCargaPeloPedido,
                    config.UtilizaNumeroDeFrotaParaPesquisaDeVeiculo,
                    config.BuscarPorCargaPedidoCargasPendentesIntegracao,
                    config.ExibirObservacaoAprovadorAutorizacaoOcorrencia,
                    config.AtualizarRotasQuandoAlterarLocalizacaoCliente,
                    config.GerarMonitoramentoParaCargaRetornoVazio,
                    config.AgruparCTesDiferentesPedidosMesmoDestinatario,
                    config.UtilizarCodificacaoUTF8ConversaoPDF,
                    config.SolicitarConfirmacaoPedidoSemMotoristaVeiculo,
                    config.SolicitarConfirmacaoPedidoDuplicado,
                    config.SolicitarConfirmacaoMovimentoFinanceiroDuplicado,
                    config.MonitorarPosicaoAtualVeiculo,
                    config.Pais,
                    config.TipoRecibo,
                    config.AtualizarVinculoVeiculoMotoristaIntegracaoCarga,
                    config.AtualizarEnderecoMotoristaIntegracaoCarga,
                    config.NaoObrigarDataSaidaRetornoPedido,
                    config.MovimentarKMApenasPelaGuarita,
                    config.BloquearEmissaoCargaSemTempoRota,
                    config.PermitirSelecionarReboquePedido,
                    config.NaoValidarCodigoBarrasBoletoTituloAPagar,
                    config.EncerrarMDFeAutomaticamente,
                    config.GerarMovimentacaoNaBaixaIndividualmente,
                    config.BloquearAlteracaoVeiculoPortalTransportador,
                    config.UtilizarCRTAverbacao,
                    config.UtilizarMesmoNumeroCRTCancelamentos,
                    config.UtilizarMesmoNumeroMICDTACancelamentos,
                    config.HabilitarEnvioAbastecimentoExterno,
                    config.AgruparUnidadesMedidasPorDescricao,
                    config.HabilitarFSDA,
                    config.VincularNotasParciaisPedidoPorProcesso,
                    config.PermiteEmitirCTeComplementarManualmente,
                    config.NaoPermiteEmitirCargaSemSeguro,
                    config.UtilizarValorDescontatoComissaoMotoristaInfracao,
                    config.NaoFecharAcertoViagemAteReceberCanhotos,
                    config.PermiteInformarChamadosNoLancamentoOcorrencia,
                    config.RaioPadrao,
                    config.TempoPadraoDeColetaParaCalcularPrevisao,
                    config.TempoPadraoDeEntregaParaCalcularPrevisao,
                    config.NaoExibirInfosAdicionaisGridPatio,
                    config.JustificarEntregaForaDoRaio,
                    config.ExibirColunaCodigosAgrupadosOcorrencia,
                    config.ExibirColunaValorFreteCargaOcorrencia,
                    config.CriarNotaFiscalTransportePorDocumentoDestinado,
                    config.ExibirAliquotaEtapaFreteCarga,
                    config.ExigirEmpresaTituloFinanceiro,
                    config.UtilizarRotaFreteInformadoPedido,
                    config.ObrigatorioCadastrarRastreadorNosVeiculos,
                    config.NaoAdicionarNumeroPedidoEmbarcadorObservacaoCTe,
                    config.ExibirJustificativaCancelamentoCarga,
                    config.EmitirComplementarRedespachoFilialEmissoraDiferenteUFOrigem,
                    config.RetornarCTeIntulizadoNoFluxoCancelamento,
                    config.PermitirImportarOcorrencias,
                    config.AvancarEtapaDocumentosEmissaoAoVincularTodasNotasParciaisCarga,
                    config.ArmazenarCentroCustoDestinatario,
                    config.PermitirAlterarDataCarregamentoCargaNoPedido,
                    config.GerarCargaComAgrupamentoNaMontagemCargaComoCargaDeComplemento,
                    config.EmitirNFeRemessaNaCarga,
                    config.UtilizarRegraICMSCTeSubcontratacao,
                    config.BloquearCamposOcorrenciaImportadosDoAtendimento,
                    config.TipoImpressaoFatura,
                    config.ValidarRENAVAMVeiculo,
                    config.ValidarPlacaVeiculo,
                    config.IdentificarMonitoramentoStatusViagemEmTransito,
                    config.IdentificarMonitoramentoStatusViagemEmTransitoKM,
                    config.IdentificarMonitoramentoStatusViagemEmTransitoMinutos,
                    config.RealizarMovimentacaoPamcardProximoDiaUtil,
                    config.ExibirOpcaoReenviarNotfisComFalhas,
                    config.ExibirOpcaoDownloadPlanilhaRateioOcorrencia,
                    config.RetornarCanhotosViaIntegracaoEmQualquerSituacao,
                    config.ExibirNumeroPagerEtapaInicialCarga,
                    config.IdentificarVeiculoParado,
                    config.IdentificarVeiculoParadoDistancia,
                    config.IdentificarVeiculoParadoTempo,
                    config.ExigirDataEntregaNotaClienteCanhotos,
                    config.DataBaseCalculoPrevisaoControleEntrega,
                    config.NumeroSerieNotaDebitoPadrao,
                    config.NumeroSerieNotaCreditoPadrao,
                    config.RealizarMovimentacaPagamentoMotoristaPelaDataPagamento,
                    config.HabilitarControleFluxoNFeDevolucaoChamado,
                    config.ProcessarFilaDocumentosEmLote,
                    config.IncluirBCCompontentesDesconto,
                    config.PrevisaoEntregaPeriodoUtilHorarioInicial,
                    config.HoraGeracaoCargaDeCTEsNaoVinculadosACargas,
                    config.GerarCargaDeCTEsNaoVinculadosACargas,
                    config.CadastrarMotoristaEVeiculoAutomaticamenteCargaImportada,
                    config.PrevisaoEntregaPeriodoUtilHorarioFinal,
                    config.PrevisaoEntregaTempoUtilDiarioMinutos,
                    config.PrevisaoEntregaVelocidadeMediaVazio,
                    config.PrevisaoEntregaVelocidadeMediaCarregado,
                    config.TokenSMS,
                    config.SenderSMS,
                    config.PermiteEncerrarMDFeEmitidoNoEmbarcador,
                    config.UsarReciboPagamentoGeracaoAutorizacaoTitulo,
                    config.ExpressaoLacreContainer,
                    config.ApresentarCodigoIntegracaoComNomeFantasiaCliente,
                    config.LancarOsServicosDaOrdemDeServicoAutomaticamente,
                    config.AjustarDataContratoIgualDataFinalizacaoCarga,
                    config.RecebedorIgualDestinatario,
                    config.ExpedidorIgualRemetente,
                    config.VisualizarTipoOperacaoDoPedidoPorTomador,
                    config.ExibirClassificacaoNFe,
                    config.ExibirSenhaCadastroPessoa,
                    config.UsarGrupoDeTipoDeOperacaoNoMonitoramento,
                    config.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem,
                    config.NaoBloquearCargaComProblemaIntegracaoGrMotoristaVeiculo,
                    config.AvisarMDFeEmitidoEmbarcadorSemSeguroValido,
                    config.GerarCanhotoSempre,
                    config.OcultarInformacoesFaturamentoAcertoViagem,
                    config.OcultarInformacoesResultadoViagemAcertoViagem,
                    config.GerarReciboAcertoViagemDetalhado,
                    config.NaoPermitirAvancarCargaSemEstoque,
                    config.NaoExibirPessoasChamado,
                    config.SalvarAnaliseEmAnexoAoLiberarOcorrenciaChamado,
                    config.ResponderAnaliseAoLiberarOcorrenciaChamado,
                    config.PermitirEstornarAprovacaoChamadoLiberado,
                    config.UtilizaListaDinamicaDatasChamado,
                    config.LinkVideoMobile,
                    config.NaoFinalizarDocumentoEntradaOSValorDivergente,
                    config.NaoFinalizarDocumentoEntradaOrdemCompraValorDivergente,
                    config.NaoFinalizarDocumentoEntradaComAbastecimentoInconsistente,
                    config.AtualizarRotaRealizadaDoMonitoramento,
                    config.BuscarCargaPorNumeroPedido,
                    config.GerarOSAutomaticamenteCadastroVeiculoEquipamento,
                    config.ValidarRaizCNPJGrupoPessoa,
                    config.PermiteOutrosOperadoresAlterarLancamentoProspeccao,
                    config.ControlarAgendamentoSKU,
                    config.NotificarCargaAgConfirmacaoTransportador,
                    config.DeixarAbastecimentosMesmaDataHoraInconsistentes,
                    config.NaoExibirRotaJanelaCarregamento,
                    config.NaoExibirLocalCarregamentoJanelaCarregamento,
                    config.ArmazenarPDFDANFE,
                    config.FiltrarNotasCompativeisPeloDestinatario,
                    config.ExibirFiltrosNotasCompativeisCarga,
                    config.DistanciaMinimaPercorridaParaSaidaDoAlvo,
                    config.GerarOcorrenciaComplementoSubcontratacao,
                    config.NaoControlarSituacaoVeiculoOrdemServico,
                    config.GerarPreviewDOCCOBFatura,
                    config.PermitirAlterarDataPrevisaoEntregaPedidoNoCarga,
                    config.NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga,
                    config.KMLimiteEntreAbastecimentos,
                    config.HorimetroLimiteEntreAbastecimentos,
                    config.TempoSemPosicaoParaVeiculoPerderSinal,
                    config.NaoValidarTomadorCTeSubcontratacaoComTomadorPedido,
                    config.LimitarApenasUmMonitoramentoPorPlaca,
                    config.RetornarCargaPendenciaEmissao,
                    config.UtilizarValorFreteNota,
                    config.ImportarEmailCliente,
                    config.AvisarDivergenciaValoresCTeEmitidoEmbarcador,
                    config.AdicionarRelatorioRelacaoEntregaDownloadDocumentos,
                    config.ExibirQuantidadeVolumesNF,
                    config.ControlarEstoqueNegativo,
                    config.PermiteCadastrarLatLngEntregaLocalidade,
                    config.NaoUtilizarDataTerminoProgramacaoVeiculo,
                    config.UtilizarLocalidadePrestacaoPedido,
                    config.UtilizarValidadeServicoPeloGrupoServicoOrdemServico,
                    config.PermitirLancarAvariasSomenteParaProdutosDaCarga,
                    config.OcultarOcorrenciasGeradasAutomaticamente,
                    config.NaoUtilizarRegraEntradaDocumentoGrupoNCM,
                    config.PermiteAlterarRotaEmCargaFinalizada,
                    config.NaoPreencherSerieCTeManual,
                    config.NaoObrigarInformarSegmentoNoAcertoDeViagem,
                    config.VisualizarValorNFSeDescontandoISSRetido,
                    config.GerarReciboDetalhadoAcertoViagem,
                    config.NaoBuscarDataInicioViagemAcerto,
                    config.TelaMonitoramentoApresentarCargasQuando,
                    config.TelaCargaApresentarUltimaPosicaoVeiculo,
                    config.UtilizarWebServiceRestATM,
                    config.UtilizarIntegracaoAverbacaoBradescoEmbarcador,
                    config.ExibirJanelaDescargaPorPeriodo,
                    config.NaoPreencherMotoristaVeiculoAbastecimento,
                    config.NaoValidarMediaIdealAbastecimento,
                    config.NaoValidarMediaIdealDeArlaAbastecimento,
                    config.ValidarMesmoKMComLitrosDiferenteAbastecimento,
                    config.NaoDeixarAbastecimentoTerceiroInconsistente,
                    config.UsarDataChecklistVeiculo,
                    config.NaoUtilizarSerieCargaCTeManual,
                    config.UtilizaMultiplosLocaisArmazenamento,
                    config.UtilizaSuprimentoDeGas,
                    config.PermiteImportarPlanilhaValoresFreteNFSManual,
                    config.ConsiderarPedagioDescargaVariacaoContratoFreteTerceiro,
                    config.VisualizarDatasRaioNoAtendimento,
                    config.ExigirClienteResponsavelPeloAtendimento,
                    config.MonitorarPassagensFronteiras,
                    config.ValidarLimiteCreditoNoPedido,
                    config.ObrigarSelecaoRotaQuandoExistirMultiplas,
                    config.AtivarEmissaoSubcontratacaoAgrupado,
                    config.NaoDuplicarCargaAoCancelarPorImportacaoXMLCTeCancelado,
                    config.ModeloVeicularCargaNaoObrigatorioMontagemCarga,
                    config.BloquearCancelamentoCargasComDataCarregamentoEDadosTransporteInformados,
                    config.MotoristaObrigatorioMontagemCarga,
                    config.VeiculoObrigatorioMontagemCarga,
                    config.GerarOcorrenciaParaCargaAgrupada,
                    config.AprovarAutomaticamenteCteEmitidoComValorInferiorAoEsperado,
                    config.BloquearVeiculoExistenteEmCargaNaoFinalizada,
                    config.AtivarFaturamentoAutomatico,
                    config.EnviarBoletoApenasParaEmailSecundario,
                    config.FormaPreenchimentoCentroResultadoPedido,
                    config.ValidarExistenciaDeConfiguracaoFaturaDoTomador,
                    config.ValidarConfiguracaoFaturamentoTomador,
                    config.ExigirNumeroDocumentoTituloFinanceiro,
                    config.UsaPermissaoControladorRelatorios,
                    config.QuantidadeDiasLimiteVencimentoFaturaManual,
                    config.BloquearLancamentoServicoDuplicadoOrdemServico,
                    config.AverbarMDFe,
                    config.PermitirGerarNotaMesmoPedidoCarga,
                    config.PermitirGerarNotaMesmaCarga,
                    //config.PermiteEnviarMesmaNotaMultiplosPedidosCarga,
                    config.MonitoramentoStatusViagemQuandoFicarSemStatusManterUltimo,
                    config.MonitoramentoConsiderarPosicaoTardiaParaAtualizarInicioFimEntregaViagem,
                    config.TelaMonitoramentoPadraoFiltroDataInicialFinal,
                    config.DiasAnterioresPesquisaCarga,
                    config.AoInativarMotoristaTransformarEmFuncionario,
                    config.NaoGerarCTesComValoresZerados,
                    config.CalcularFreteCliente,
                    config.PermitirInformarQuilometragemTabelaFreteCliente,
                    config.TabelaFretePrecisaoDinheiroDois,
                    config.DocumentoImpressaoPadraoCarga,
                    config.NaoGerarAverbacaoCTeQuandoPedidoTiverAverbacao,
                    config.NaoEmitirDocumentosEmCargasDeReentrega,
                    config.NaoPermitirInativarFuncionarioComSaldo,
                    config.NaoConsiderarProdutosSemPesoParaSumarizarVolumes,
                    config.PermiteInformarModeloVeicularCargaOrigem,
                    config.UtilizarPesoLiquidoNFeParaCTeMDFe,
                    config.NaoSolicitarAtuorizacaoAbastecimento,
                    config.VelocidadeMinimaAceitaDasTecnologias,
                    config.VelocidadeMaximaAceitaDasTecnologias,
                    config.TemperaturaMinimaAceitaDasTecnologias,
                    config.TemperaturaMaximaAceitaDasTecnologias,
                    config.PreencherDataProgramadaComAtualCheckList,
                    config.TipoCalculoPercentualViagem,
                    config.MonitoramentoStatusViagemTipoRegraParaCalcularPercentualViagem,
                    config.SomenteAutorizadoresPodemDelegarOcorrencia,
                    config.ExigirMotivoOcorrencia,
                    config.NaoRetornarNotasEmDocumentoComplementar,
                    config.UtilizarEtiquetaDetalhadaWMS,
                    config.GerarNumeracaoFaturaAnual,
                    config.ValidarSeExisteVeiculoCadastradoComMesmoNrDeFrota,
                    config.HabilitarAlertaCargasParadas,
                    config.TempoMinutosAlertaCargasParadas,
                    config.EmailsAlertaCargasParadas,
                    config.RetornarTitulosNaoGerados,
                    config.NaoExigirMotivoAprovacaoCTeInconsistente,
                    config.NaoRetornarCarregamentosSemData,
                    config.EnviarEmailAnalistasChamado,
                    config.QuandoIniciarViagemViaMonitoramento,
                    config.ExigirTipoMovimentoLancamentoMovimentoFinanceiroManual,
                    config.QuandoIniciarMonitoramento,
                    config.AcaoAoFinalizarMonitoramento,
                    config.FinalizarMonitoramentoEmAndamentoDoVeiculoAoIniciar,
                    config.ExibirFiltroEColunaCodigoPedidoClienteGestaoDocumentos,
                    config.UtilizarCentroResultadoNoRateioDespesaVeiculo,
                    config.RegistrarEntregasApenasAposAtenderTodasColetas,
                    config.HabilitarDescontoGestaoDocumento,
                    config.CopiarDataTerminoCarregamentoCargaParaPrevisaoEntregaPedidos,
                    config.LiberarIntegracaoTransportadorDeCargaImportarDocumentoManual,
                    config.NaoValidarTabelaFreteMesmaIncidenciaImportacao,
                    config.GerarContratoTerceiroSemInformacaoDoFrete,
                    config.UtilizarMultiplosModelosVeicularesPedido,
                    config.SolicitarValorFretePorTonelada,
                    config.PermitirDarBaixaFaturasCTe,
                    config.PermitirConsultaDeValoresPedagio,
                    PlanoContaAdiantamentoCliente = config.PlanoContaAdiantamentoCliente != null ? new { config.PlanoContaAdiantamentoCliente.Codigo, config.PlanoContaAdiantamentoCliente.Descricao } : null,
                    PlanoContaAdiantamentoFornecedor = config.PlanoContaAdiantamentoFornecedor != null ? new { config.PlanoContaAdiantamentoFornecedor.Codigo, config.PlanoContaAdiantamentoFornecedor.Descricao } : null,
                    config.SolicitarAprovacaoFolgaAcertoViagem,
                    config.ExibirPesoCargaEPesoCubadoGestaoDocumentos,
                    config.PermitirEnviarEmailAutorizacaoEmbarque,
                    config.PercentualComissaoPadrao,
                    config.PercentualMediaEquivalente,
                    config.PercentualEquivaleEquivalente,
                    config.PercentualAdvertenciaEquivalente,
                    config.PermitirCancelarPedidosSemDocumentos,
                    config.ExigirAnexosNoCadastroDoTransportador,
                    config.VisualizarVeiculosPropriosETerceiros,
                    config.PedidoOcorrenciaColetaEntregaIntegracaoNova,
                    config.TipoPagamentoContratoFrete,
                    config.GerarNFSeImportacaoEmbarcador,
                    config.InformarAjusteManualCargasImportadasEmbarcador,
                    config.GerarComponentesDeFreteComImpostoIncluso,
                    config.ConsultarRegraICMSGeracaoCTeSubstitutoAutomaticamente,
                    config.EnviarRegraExclusivaCodigoImpostoLayoutINTNC,
                    config.DesconsiderarSobraRateioParaBaseCalculoIBSCBS,
                    config.ValidarICMSTelaCotacaoPedidosRegraICMS,
                    config.NaoComprarValePedagioViaIntegracaoSeInformadoManualmenteNaCarga,
                    config.NaoExibirCodigoIntegracaoDoDestinatarioResumoCarga,
                    config.BuscarMotoristaDaCargaLancamentoAbastecimentoAutomatico,
                    config.UtilizarNotaFiscalExistenteNaImportacaoCTeEmbarcador,
                    config.GerarObservacaoRegraICMSAposObservacaoCTe,
                    config.PermitirAtualizarModeloVeicularCargaDoVeiculoNoWebService,
                    KMLimiteEntreAbastecimentosARLA = config.KMLimiteEntreAbastecimentosArla,
                    NaoUsarPesoNotasPallet = config.NaoUsarPesoNotasPallet,
                    ConfiguracaoControleEntrega = ObterConfiguracaoControleEntrega(config, unitOfWork),
                    ConfiguracaoMotorista = obterConfiguracoesMotorista(config),
                    ConfiguracaoVeiculo = obterConfiguracoesVeiculo(config),
                    ConfiguracaoPaisMercosul = obterConfiguracoesPaisMercosul(unitOfWork),
                    ConfiguracaoAcertoViagem = ObterConfiguracaoAcertoViagem(config, unitOfWork),
                    ConfiguracaoCargaEmissaoDocumento = ObterConfiguracaoCargaEmissaoDocumento(unitOfWork),
                    ConfiguracaoGeralCarga = ObterConfiguracaoGeralCarga(config, unitOfWork),
                    ConfiguracaoContratoFreteTerceiro = ObterConfiguracaoContratoFreteTerceiro(unitOfWork),
                    ConfiguracaoFatura = ObterConfiguracaoFatura(unitOfWork),
                    ConfiguracaoPedido = ObterConfiguracaoPedido(unitOfWork),
                    ConfiguracaoOcorrencia = ObterConfiguracaoOcorrencia(unitOfWork),
                    ConfiguracaoWebService = ObterConfiguracaoWebService(unitOfWork),
                    ConfiguracaoTabelaFrete = ObterConfiguracaoTabelaFrete(unitOfWork),
                    ConfiguracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento(unitOfWork),
                    ConfiguracaoDocumentoEntrada = ObterConfiguracaoDocumentoEntrada(unitOfWork),
                    ConfiguracaoCargaDadosTransporte = ObterConfiguracaoCargaDadosTransporte(unitOfWork),
                    ConfiguracaoCargaIntegracao = ObterConfiguracaoCargaIntegracao(unitOfWork),
                    ConfiguracaoMobile = ObterConfiguracaoMobile(unitOfWork),
                    ConfiguracaoAprovacao = ObterConfiguracaoAprovacao(config, unitOfWork),
                    ConfiguracaoAgendamentoColeta = ObterConfiguracaoAgendamentoColeta(unitOfWork),
                    ConfiguracaoAbaVeiculo = ObterConfiguracaoVeiculo(unitOfWork),
                    ConfiguracaoAbaMotorista = ObterConfiguracaoMotoristaUsuario(unitOfWork),
                    ConfiguracaoEncerramentoMDFeAutomatico = ObterConfiguracaoEncerramentoMDFeAutomatico(unitOfWork),
                    ConfiguracaoFinanceiro = ObterConfiguracaoFinanceiro(unitOfWork),
                    ConfiguracaoChamado = ObterConfiguracaoChamado(unitOfWork),
                    ConfiguracaoCargaCalculoFrete = ObterConfiguracaoCargaCalculoFrete(unitOfWork),
                    ConfiguracaoMontagemCarga = ObterConfiguracaoMontagemCarga(unitOfWork),
                    ConfiguracaoRedMine = ObterConfiguracaoRedMine(unitOfWork),
                    ConfiguracaoDocumentosDestinados = ObterConfiguracaoDocumentosDestinados(unitOfWork),
                    ConfiguracaoPaletes = ObterConfiguracaoPaletes(unitOfWork),
                    ConfiguracaoMonitoramento = ObterConfiguracaoMonitoramento(unitOfWork),
                    ConfiguracaoGeral = ObterConfiguracaoGeral(unitOfWork),
                    ConfiguracaoCanhoto = ObterConfiguracaoCanhoto(unitOfWork),
                    ConfiguracaoTransportador = ObterConfiguracaoTransportador(unitOfWork),
                    ConfiguracaoRoteirizacao = ObterConfiguracaoRoteirizacao(unitOfWork),
                    ConfiguracaoFilaCarregamento = ObterConfiguracaoFilaCarregamento(unitOfWork),
                    ConfiguracaoProduto = ObterConfiguracaoProduto(unitOfWork),
                    ConfiguracaoCalculoPrevisao = ObterConfiguracaoCalculoPrevisao(unitOfWork),
                    ConfiguracaoPortalMultiClifor = ObterConfiguracaoPortalMultiClifor(unitOfWork),
                    ConfiguracaoPortalFluxoPatio = ObterConfiguracaoPortalFluxoPatio(unitOfWork, config),
                    ConfiguracaoPortalNFSeManual = ObterConfiguracaoPortalNFSeManual(unitOfWork, config),
                    ConfiguracaoComissaoMotorista = ObterConfiguracaoComissaoMotorista(unitOfWork, config),
                    ConfiguracaoEnvioEmailCobranca = ObterConfiguracaoEnvioEmailCobranca(unitOfWork, config),
                    ConfiguracaoInfracoes = ObterConfiguracaoInfracoes(unitOfWork, config),
                    ConfiguracaoBidding = ObterConfiguracaoBidding(unitOfWork),
                    ConfiguracaoAbastecimento = ObterConfiguracaoAbastecimento(unitOfWork, config),
                    ConfiguracaoArquivo = ObterConfiguracaoArquivo(unitOfWork),
                    ConfiguracaoAmbiente = ObterConfiguracaoAmbiente(unitOfWork),
                    ConfiguracaoPessoa = ObterConfiguracaoPessoa(unitOfWork),
                    ConfiguracaoRelatorio = ObterConfiguracaoRelatorio(unitOfWork, config),
                    ConfiguracaoMongo = ObterConfiguracaoMongo(unitOfWork),
                    ConfiguracaoSSo = ObterConfiguracaoSSo(unitOfWork),
                    ConfiguracaoMercosul = ObterConfiguracaoMercosul(unitOfWork),
                    ConfiguracaoAgendamentoEntrega = ObterConfiguracaoAgendamentoEntrega(unitOfWork),
                    ConfiguracaoAtendimentoAutomaticoDivergenciaValorFreteCTeEmitidosEmbarcador = ObterConfiguracaoAtendimentoAutomaticoDivergenciaValorFreteCTeEmitidosEmbarcador(unitOfWork),
                    ConfiguracaoAPIDeConexaoComGeradorExcelKMM = ObterConfiguracaoAPIDeConexaoComGeradorExcelKMM(unitOfWork),
                    ConfiguracaoCotacao = ObterConfiguracaoCotacao(unitOfWork),
                    ConfiguracaoGeralCIOT = ObterConfiguracaoGeralCIOT(unitOfWork, config),
                    ConfiguracaoGeralTipoPagamentoCIOT = ObterConfiguracaoGeralTipoPagamentoCIOT(unitOfWork, config),
                    ConfiguracaoGeralDownloadArquivos = ObterConfiguracaoDownloadArquivos(unitOfWork),
                    ConfiguracaoPaginacaoInterfaces = ObterConfiguracaoPaginacaoInterfaces(unitOfWork, config),
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os as configurações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarCanhotosRetroativos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (!this.Usuario.UsuarioMultisoftware && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    return new JsonpResult(true, false, "Seu usuário não tem permissão de alterar essas configurações.");

                Servicos.Embarcador.Canhotos.Canhoto.GerarCanhotosRetroativosNFe(TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    Servicos.Embarcador.Canhotos.Canhoto.GerarCanhotosRetroativosCTeSubcontratacao(TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador);
                    Servicos.Embarcador.Canhotos.Canhoto.GerarCanhotosRetroativosCTe(TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao processar os conhotos retroativos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReiniciarIntegradores()
        {
            string arquivoPathListener = @"F:\Integradores\ReiniciarListenerOracle.bat";
            string arquivoPathReinciarIntegrador = @"F:\Integradores\IntegradorGerenciador.exe";
            int contador = 0;

            if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivoPathListener) || !Utilidades.IO.FileStorageService.Storage.Exists(arquivoPathReinciarIntegrador))
                return new JsonpResult(false, false, "Arquivos de integradores inesistentes na pasta entre em contato com N3.");

            while (contador < 2)
            {
                try
                {
                    if (contador == 0)
                        ExecutarProcessos(arquivoPathListener);
                    else if (contador == 1)
                        ExecutarProcessos(arquivoPathReinciarIntegrador);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, false, "Falha ao reiniciar integradores");
                }

                contador++;

                Thread.Sleep(3000);
            }

            return new JsonpResult(true);
        }

        public async Task<IActionResult> ProcessarReceitasDespesasVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    return new JsonpResult(true, false, "Seu usuário não tem permissão de alterar essas configurações.");

                Servicos.Embarcador.Veiculo.ReceitaDespesa.ProcessarReceitasEDespesas(unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao processar as receitas/despesas por veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReinciarIntegradores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {


                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao processar as receitas/despesas por veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterSituacaoesCarga()
        {
            try
            {
                ArrayList retorno = new ArrayList();

                //retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.ProntoTransporte, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.ProntoTransporte.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransbordo, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransbordo.ObterDescricao() });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada.ObterDescricao() });
                //retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PermiteCTeManual, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PermiteCTeManual.ObterDescricao() });                


                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar as situações da carga.");
            }
        }

        public async Task<IActionResult> EnviarArquivoPoliticaPrivacidadeMobile()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if ((this.Usuario.UsuarioInterno == null) && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    return new JsonpResult(true, false, "Seu usuário não tem permissão de alterar essas configurações.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                string caminho = ObterCaminhoArquivos(unitOfWork);

                Servicos.DTO.CustomFile arquivo = files[0];
                string extensao = System.IO.Path.GetExtension(arquivo.FileName).ToLower();

                if (!extensao.Equals(".pdf"))
                    return new JsonpResult(false, true, "Arquivo com extensão inválida! Permitido apenas PDF.");

                string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivo.FileName);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoCompleto))
                    Utilidades.IO.FileStorageService.Storage.Delete(caminhoCompleto);

                arquivo.SaveAs(caminhoCompleto);

                config.CaminhoArquivoPoliticaPrivacidadeMobile = caminhoCompleto;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, config, null, $"Atualizou o arquivo de Política de Privacidade Mobile.", unitOfWork);

                unitOfWork.Start();

                repConfiguracaoTMS.Atualizar(config);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar o(s) arquivo(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarArquivoCertificadoSSo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if ((this.Usuario.UsuarioInterno == null) && (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    return new JsonpResult(true, false, "Seu usuário não tem permissão de alterar essas configurações.");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO repositorioConfiguracaoSSo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO configuracaoSSo = repositorioConfiguracaoSSo.BuscarPrimeiroRegistro();

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                string caminho = ObterCaminhoArquivos(unitOfWork);

                Servicos.DTO.CustomFile arquivo = files[0];
                string extensao = System.IO.Path.GetExtension(arquivo.FileName).ToLower();

                if (!extensao.Equals(".cer") && !extensao.Equals(".xml"))
                    return new JsonpResult(false, true, "Arquivo com extensão inválida! Permitido apenas CER e XML.");

                string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivo.FileName);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoCompleto))
                    Utilidades.IO.FileStorageService.Storage.Delete(caminhoCompleto);

                arquivo.SaveAs(caminhoCompleto);

                configuracaoSSo.CaminhoArquivoCertificado = caminhoCompleto;
                //Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoSSo, null, $"Atualizou o Certificado de Autenticação SSo.", unitOfWork);

                unitOfWork.Start();

                repositorioConfiguracaoSSo.Atualizar(configuracaoSSo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar o(s) arquivo(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarConfiguracaoLogs()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoLog repConfiguracaoLog = new Repositorio.Embarcador.Configuracoes.ConfiguracaoLog(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLog config = repConfiguracaoLog.BuscarConfiguracaoPadrao();

                return new JsonpResult(new
                {
                    UtilizaLogArquivo = config?.UtilizaLogArquivo ?? true,
                    UtilizaLogWeb = config?.UtilizaLogWeb ?? false,
                    ProtocoloLogWeb = config?.ProtocoloLogWeb ?? ProtocoloLogWeb.UDP,
                    GravarLogError = config?.GravarLogError ?? true,
                    GravarLogInfo = config?.GravarLogInfo ?? false,
                    GravarLogAdvertencia = config?.GravarLogAdvertencia ?? true,
                    GravarLogDebug = config?.GravarLogDebug ?? false,
                    Url = config?.Url ?? string.Empty,
                    Porta = config?.Porta ?? 0,

                    UtilizaGraylog = config?.UtilizaGraylog ?? false,
                    ProtocoloLogGraylog = config?.ProtocoloLogGraylog ?? ProtocoloLogWeb.TCP,
                    UrlGraylog = config?.UrlGraylog ?? string.Empty,
                    PortaGraylog = config?.PortaGraylog ?? 0
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os as configuração logs.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoLogs()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoLog repConfiguracaoLog = new Repositorio.Embarcador.Configuracoes.ConfiguracaoLog(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLog config = repConfiguracaoLog.BuscarConfiguracaoPadrao();

                bool adicionarNovoRegistro = false;
                if (config == null)
                {
                    adicionarNovoRegistro = true;
                    config = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLog();
                }

                bool utilizaLogArquivo = Request.GetBoolParam("UtilizaLogArquivo");
                bool utilizaLogWeb = Request.GetBoolParam("UtilizaLogWeb");
                ProtocoloLogWeb protocoloLogWeb = Request.GetEnumParam<ProtocoloLogWeb>("ProtocoloLogWeb");
                bool gravarLogError = Request.GetBoolParam("GravarLogError");
                bool gravarLogInfo = Request.GetBoolParam("GravarLogInfo");
                bool gravarLogAdvertencia = Request.GetBoolParam("GravarLogAdvertencia");
                bool gravarLogDebug = Request.GetBoolParam("GravarLogDebug");
                string url = Request.GetStringParam("Url");
                Int32 porta = Request.GetIntParam("Porta");

                bool utilizaGraylog = Request.GetBoolParam("UtilizaGraylog");
                ProtocoloLogWeb protocoloLogGraylog = Request.GetEnumParam<ProtocoloLogWeb>("ProtocoloLogGraylog");
                string urlGraylog = Request.GetStringParam("UrlGraylog");
                Int32 portaGraylog = Request.GetIntParam("PortaGraylog");

                unitOfWork.Start();

                config.UtilizaLogArquivo = utilizaLogArquivo;
                config.UtilizaLogWeb = utilizaLogWeb;
                config.ProtocoloLogWeb = protocoloLogWeb;
                config.GravarLogError = gravarLogError;
                config.GravarLogInfo = gravarLogInfo;
                config.GravarLogAdvertencia = gravarLogAdvertencia;
                config.GravarLogDebug = gravarLogDebug;
                config.Url = url;
                config.Porta = porta;

                config.UtilizaGraylog = utilizaGraylog;
                config.ProtocoloLogGraylog = protocoloLogGraylog;
                config.UrlGraylog = urlGraylog;
                config.PortaGraylog = portaGraylog;

                if (adicionarNovoRegistro)
                    repConfiguracaoLog.Inserir(config);
                else
                    repConfiguracaoLog.Atualizar(config);

                unitOfWork.CommitChanges();

                Servicos.Log.RecarregarConfiguracao();

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(true, false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(true, false, "Ocorreu uma falha ao salvar a configuração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados - Obter Objeto Configuração

        private dynamic ObterConfiguracaoAprovacao(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao repositorioConfiguracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = repositorioConfiguracaoAprovacao.BuscarConfiguracaoPadrao();

            return new
            {
                config.BloquearSemRegraAprovacaoOrdemServico,
                config.ExigeNumeroDeAprovadoresNasAlcadas,
                config.FiltrarAlcadasDoUsuario,
                config.NaoExibirOpcaoParaDelegar,
                config.NaoPermitirDelegarAoUsuarioLogado,
                config.UsarAlcadaAprovacaoGestaoDocumentos,
                config.UtilizarAlcadaAprovacaoAlteracaoValorFrete,
                config.UtilizarAlcadaAprovacaoValorTabelaFreteCarga,
                config.UtilizarAlcadaAprovacaoTabelaFrete,
                config.UtilizarAlcadaAprovacaoCarregamento,
                config.UtilizarAlcadaAprovacaoVeiculo,
                config.UtilizarAlcadaAprovacaoLiberacaoEscrituracaoPagamentoCarga,
                config.UtilizarAlcadaAprovacaoPagamento,
                config.UtilizarAlcadaAprovacaoAlteracaoRegraICMS,
                configuracaoAprovacao.PermitirDelegarParaUsuarioComTodasAlcadasRejeitadas,
                configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente,
                configuracaoAprovacao.CriarAprovacaoCargaAoConfirmarDocumentos,
            };
        }

        private dynamic ObterConfiguracaoAgendamentoColeta(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();

            return new
            {
                configuracaoAgendamentoColeta.RemoverEtapaAgendamentoAgendamentoColeta,
                configuracaoAgendamentoColeta.PermitirTransportadorCadastrarAgendamentoColeta,
                configuracaoAgendamentoColeta.ConsultarSomenteTransportadoresPermitidosCadastro,
                configuracaoAgendamentoColeta.GerarAutomaticamenteSenhaPedidosAgendas,
                configuracaoAgendamentoColeta.MostrarTipoDeOperacaoNoPortalMultiEmbarcadorAgendamentoColeta,
                configuracaoAgendamentoColeta.CalcularDataDeEntregaPorTempoDeDescargaDaRota,
                configuracaoAgendamentoColeta.TempoPadraoDeDescargaMinutos,
                configuracaoAgendamentoColeta.UtilizaRazaoSocialNaVisaoDoAgendamento,
                configuracaoAgendamentoColeta.EnviarEmailDeNotificacaoAutomaticamenteAoTransportadorDaCarga,
                ModeloEmailNotificacaoAutomaticaTransportador = new
                {
                    Codigo = configuracaoAgendamentoColeta.ModeloEmail?.Codigo ?? 0,
                    Descricao = configuracaoAgendamentoColeta.ModeloEmail?.Descricao ?? string.Empty
                },
            };
        }

        private dynamic ObterConfiguracaoCargaEmissaoDocumento(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadrao();

            return new
            {
                ConsultarDocumentosDestinadosCarga = configuracaoCargaEmissaoDocumento?.ConsultarDocumentosDestinadosCarga ?? false,
                ValidarDataPrevisaoEntregaPedidoMenorDataAtual = configuracaoCargaEmissaoDocumento?.ValidarDataPrevisaoEntregaPedidoMenorDataAtual ?? false,
                ValidarDataPrevisaoSaidaPedidoMenorDataAtual = configuracaoCargaEmissaoDocumento?.ValidarDataPrevisaoSaidaPedidoMenorDataAtual ?? false,
                NaoAlterarCentroResultadoMotorista = configuracaoCargaEmissaoDocumento?.NaoAlterarCentroResultadoMotorista ?? false,
                BloquearEmissaoTomadorSemEmail = configuracaoCargaEmissaoDocumento?.BloquearEmissaoTomadorSemEmail ?? false,
                UtilizarNumeroOutroDocumento = configuracaoCargaEmissaoDocumento?.UtilizarNumeroOutroDocumento ?? false,
                NaoPermitirNFSComMultiplosCentrosResultado = configuracaoCargaEmissaoDocumento?.NaoPermitirNFSComMultiplosCentrosResultado ?? false,
                configuracaoCargaEmissaoDocumento.BloquearEmissaoCargaTerceirosSemValePedagio,
                configuracaoCargaEmissaoDocumento.AtivarEnvioDocumentacaoFinalizacaoCarga,
                configuracaoCargaEmissaoDocumento.NaoComprarValePedagio,
                configuracaoCargaEmissaoDocumento.NaoPermitirAcessarDocumentosAntesCargaEmTransporte,
                configuracaoCargaEmissaoDocumento.ControlarValoresComponentesCTe,
                configuracaoCargaEmissaoDocumento.NaoEnviarEmailDocumentoEmitidoProprietarioVeiculo,
                configuracaoCargaEmissaoDocumento.NaoPermitirEmissaoComMesmaOrigemEDestino,
                configuracaoCargaEmissaoDocumento.UsaFluxoSubstituicaoFaseada
            };
        }

        private dynamic ObterConfiguracaoCargaCalculoFrete(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete repConfiguracaoCargaCalculoFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete configuracaoCargaCalculoFrete = repConfiguracaoCargaCalculoFrete.BuscarConfiguracaoPadrao();

            return new
            {
                ValorMaximoCalculoFrete = configuracaoCargaCalculoFrete?.ValorMaximoCalculoFrete ?? 0m,
            };
        }

        private dynamic ObterConfiguracaoGeralCarga(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracao = repositorio.BuscarPrimeiroRegistro();

            return new
            {
                config.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga,
                config.UtilizarPesoPedidoParaRatearPesoNFeRepetida,
                configuracao.PermitirAlterarInformacoesAgrupamentoCarga,
                configuracao.ObrigatorioOperadorResponsavelCancelamentoCarga,
                configuracao.CalcularPautaFiscal,
                configuracao.NaoPermitirRemoverUltimoPedidoCarga,
                configuracao.PermitirRemoverCargasAgrupamentoCarga,
                configuracao.AssumirSempreTipoOperacaoDoPedido,
                configuracao.UtilizaControleDeEntregaManual,
                configuracao.NaoValidarLicencaVeiculoParaCargaRedespacho,
                configuracao.UtilizarPesoProdutoParaCalcularPesoCarga,
                configuracao.PadraoGeracaoNumeroCarga,
                configuracao.AlertarTransportadorCancelamentoCarga,
                configuracao.TrocarFilialQuandoExpedidorForUmaFilial,
                configuracao.HabilitarRelatorioDeEmbarque,
                configuracao.ExibirMensagemAlertaPrevisaoEntregaNaMesmaData,
                configuracao.ManterTransportadorUnicoEmCargasAgrupadas,
                configuracao.LimitePesoDocumentoCarga,
                configuracao.LimiteValorDocumentoCarga,
                configuracao.LimiteTaraPedidosCarga,
                configuracao.UtilizarProgramacaoCarga,
                configuracao.PrefixoParaCargasGeradasViaCarregamento,
                configuracao.PermitirAgrupamentoDeCargasOrdenavel,
                configuracao.PermitirGerarRegistroDeDesembarqueNoCIOT,
                configuracao.AtualizarVinculoVeiculoMotoristaIntegracao,
                configuracao.NaoPermitirGerarRedespachoDeCargasDeRedespacho,
                configuracao.AverbarMDFeSomenteEmCargasComCIOT,
                configuracao.ExigirConfiguracaoTerceiroParaGerarCIOTParaTodos,
                configuracao.PermitirCancelarDocumentosCargaPeloCancelamentoCarga,
                configuracao.HabilitarEnvioDocumentacaoCargaPorEmail,
                configuracao.SelecionarSomenteOperacoesDeRedespachoNaTelaDeRedespacho,
                configuracao.NotificarNovaCargaAposConfirmacaoDocumentos,
                configuracao.InformarCentroResultadoNaEtapaUmDaCarga,
                configuracao.UtilizaRegrasDeAprovacaoParaCancelamentoDaCarga,
                configuracao.PermitirCancelamentoDaCargaSomenteComDocumentosEmitidos,
                TipoIntegracaoCancelamentoPadrao = configuracao.TipoIntegracaoCancelamentoPadrao?.Tipo ?? 0,
                configuracao.ConsiderarApenasUmaVezKMParaPedidosComMesmoDestinoOrigemCarga,
                configuracao.PermitirAlterarEmpresaNoCTeManual,
                configuracao.ObrigarJustificativaCancelamentoCarga,
                configuracao.FinalizarCargaAutomaticamenteAposEncerramentoMDFe,
                configuracao.AtualizarDataEmissaoParaDataAtualQuandoReemitirCTeRejeitado,
                configuracao.RetornarPedidosInseridosManualmenteAoGerarCarga,
                configuracao.AoCancelarCargaManterPedidosEmAberto,
                configuracao.RealizarIntegracaoDadosCancelamentoCarga,
                configuracao.UtilizarConfiguracaoTipoOperacaoGeracaoCargaPorPedido,
                configuracao.PadraoVisualizacaoOperadorLogistico,
                configuracao.NaoVincularAutomaticamenteDocumentosEmitidosEmbarcador,
                configuracao.PermitirAvancarCargasEmitidasEmbarcadorPorTipoOperacao,
                configuracao.AtualizarDadosDosPedidosComDadosDaCarga,
                configuracao.AjustarValorFreteAposAprovacaoPreCTe,
                configuracao.VisualizarLegendaCargaAcordoTipoOperacao,
                configuracao.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga,
                configuracao.EnviarEmailPreviaCustoParaTransportadores,
                configuracao.PermiteInformarRemetenteLancamentoNotaManualCarga,
                configuracao.NaoGerarPDFDocumentosComNotasFiscais,
                configuracao.PermiteSelecionarMultiplasCargasParaRedespacho,
                configuracao.GerarRedespachoDeCargasAgrupadas,
                configuracao.NaoEncerrarMDFeDeFormaAutomaticaAoConfirmarDadosDeTransporte,
                configuracao.PermiteExcluirAgendamentoDaCargaJanelaDescarga,
                configuracao.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo,
                configuracao.NaoPermitirEncerrarCIOTEncerrarCarga,
                configuracao.DesabilitarUtilizacaoCreditoOperadores,
                configuracao.UsarPrioridadeDaCargaParaImpressaoDeObservacaoNoCTE,
                ConverterXMLNotaFiscalParaByteArrayAoImportarNaCarga = configuracao?.ConverterXMLNotaFiscalParaByteArrayAoImportarNaCarga ?? false,
                RemoverVinculoNotaPedidoAbertoAoCancelarCarga = configuracao?.RemoverVinculoNotaPedidoAbertoAoCancelarCarga ?? false,
                configuracao.GerarOutrosDocumentosNaImportacaoDeCTeComplementar,
                configuracao.GerarNumerodeCargaAlfanumerico,
                configuracao.TamanhoNumerodeCargaAlfanumerico,
                configuracao.ValidarValorLimiteApoliceComValorNFe,
                configuracao.NaoPermitirAlterarDadosCargaQuandoTiverIntegracaoIntegrada,
                configuracao.AtribuirValorMercadoriaCTeNotasFiscaisDocumentosEmitidosEmbarcador,
                configuracao.PermitirEncaixarPedidosComReentregaSolicitada,
                configuracao.ValidarContratanteOrigemVPIntegracaoPamcard,
                configuracao.NaoPermitirAvançarEtapaUmCargaComTransportadorSemApoliceVigente,
                configuracao.GerarCargaComFluxoFilialEmissoraComExpedidor,
                configuracao.ConsiderarDataEmissaoCTECalculoEmbarquePrevisaoEntrega,
                configuracao.ConsiderarConfiguracaoNoTipoDeOperacaoParaParticipantesDosDocumentosAoGerarCargaEspelho,
                configuracao.ProcessarDadosTransporteAoFecharCarga,
                configuracao.UtilizarEmpresaFilialEmissoraNoArquivoEDI,
                configuracao.NaoUtilizarCodigoCargaOrigemNaObservacaoCTe,
                configuracao.RecalcularFreteAoDuplicarCargaCancelamentoDocumento,
                configuracao.PermiteInformarFreteOperadorFilialEmissora,
                configuracao.NaoConsiderarRecebedorAoCalcularNumeroEntregasEmissaoPorPedido,
                configuracao.PermiteHabilitarContingenciaEPECAutomaticamente,
                configuracao.PermiteReceberNotaFiscalViaIntegracaoNasEtapasFreteETransportador,
                configuracao.TempoMinutosParaEnvioProgramadoIntegracao,
                configuracao.SetarCargaComoBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual,
                configuracao.PermitirDesvincularGerarCopiaCTeRejeitadoCarga,
                configuracao.PermitirSalvarApenasTransportadorEtapaUmCarga,
                configuracao.ExigirConfirmacaoEtapaFreteNoFluxoNotaAposFrete,
                configuracao.PermitirReverterAnulacaoGerencialTelaCancelamento,
                configuracao.AtivarCancelamentoDeFaturaETituloAoFluxoDeCancelamentoNaCarga,
                configuracao.NaoPermitirAtribuirVeiculoCargaSeExistirMonitoramentoAtivoParaPlaca,
                configuracao.ConsiderarFilialDaTransportadoraParaCompraDoValePedagioQuandoForEFrete,
                configuracao.UtilizarDistanciaRoteirizacaoNaCarga,
                configuracao.InformarDocaNaEtapaUmDaCarga,
                configuracao.CancelarValePedagioQuandoGerarCargaTransbordo,
                configuracao.ValidarModeloVeicularVeiculoCargaEtapaFrete,
                configuracao.ObrigatoriedadeCIOTEmissaoMDFe,
                configuracao.PermitirInformarRecebedorAoCriarUmRedespachoManual,
                configuracao.UtilizarDataCarregamentoAoCriarCargaViaIntegracao,
                configuracao.IniciarConfirmacaoDocumentosFiscaisCargaPorThread,
                PermitirFiltrarCargasNaEmissaoManualCteSemTerCtesImportados = configuracao?.PermitirFiltrarCargasNaEmissaoManualCteSemTerCtesImportados ?? false,
                PermitirRemoverMultiplosPedidosCarga = configuracao?.PermitirRemoverMultiplosPedidosCarga ?? false,
                SolicitarJustificativaAoRemoverPedidoCarga = configuracao?.SolicitarJustificativaAoRemoverPedidoCarga ?? false,
                configuracao.PermitirInformarValorFreteOperadorMesmoComFreteConfirmadoPeloTransportador,
                configuracao.PararCargaQuandoNaoInformadoCIOT,
                configuracao.NaoAplicarICMSMetodoAtualizarFrete,
            };
        }

        private dynamic ObterConfiguracaoContratoFreteTerceiro(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();

            return new
            {
                DiasVencimentoAdiantamentoContratoFreteTerceiro = configuracaoContratoFreteTerceiro.DiasVencimentoAdiantamento,
                DiasVencimentoSaldoContratoFreteTerceiro = configuracaoContratoFreteTerceiro.DiasVencimentoSaldo,
                ObservacaoContratoFreteTerceiro = configuracaoContratoFreteTerceiro.Observacao,
                ReterImpostosContratoFreteTerceiro = configuracaoContratoFreteTerceiro.ReterImpostos,
                RetencaoPorRaizCNPJ = configuracaoContratoFreteTerceiro.RetencaoPorRaizCNPJ,
                TextoAdicionalContratoFreteTerceiro = configuracaoContratoFreteTerceiro.TextoAdicional,
                CalcularAdiantamentoComPedagioContratoFreteTerceiro = configuracaoContratoFreteTerceiro.CalcularAdiantamentoComPedagio,
                configuracaoContratoFreteTerceiro.UtilizarTaxaPagamentoContratoFreteTerceiro,
                configuracaoContratoFreteTerceiro.UtilizarNovoLayoutPagamentoAgregado,
                configuracaoContratoFreteTerceiro.PercentualAdiantamentoFreteTerceiros,
                configuracaoContratoFreteTerceiro.ExibirPedidosImpressaoContratoFrete,
                NaoConsiderarDescontoCalculoImpostosContratoFreteTerceiro = configuracaoContratoFreteTerceiro.NaoConsiderarDescontoCalculoImpostos,
                configuracaoContratoFreteTerceiro.HabilitarLayoutFaturaPagamentoAgregado,
                configuracaoContratoFreteTerceiro.GerarCargaTerceiroApenasProvedorPedido,
                configuracaoContratoFreteTerceiro.ObrigarAnexosContratoTransportadorFrete,
                configuracaoContratoFreteTerceiro.GerarNumeroContratoTransportadorFreteSequencial,
                configuracaoContratoFreteTerceiro.NaoSubtrairValePedagioDoContrato,
                configuracaoContratoFreteTerceiro.UtilizarFechamentoDeAgregado,
                configuracaoContratoFreteTerceiro.EmAcrescimoDescontoCiotNaoAlteraImpostos,
                configuracaoContratoFreteTerceiro.PermiteAlterarDadosContratoIndependenteSituacao,
                configuracaoContratoFreteTerceiro.PermitirAutorizarPagamentoCIOTComCanhotosRecebidos,
                configuracaoContratoFreteTerceiro.GerarCIOTMarcadoAoCadastrarTransportadorTerceiro,
                configuracaoContratoFreteTerceiro.PermitirInformarPercentual100AdiantamentoCarga,
            };
        }

        private dynamic ObterConfiguracaoFatura(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();

            return new
            {
                PermitirVencimentoRetroativoFatura = configuracaoFatura?.PermitirVencimentoRetroativoFatura ?? false,
                PreencherPeriodoFaturaComDataAtual = configuracaoFatura?.PreencherPeriodoFaturaComDataAtual ?? false,
                InformarDataCancelamentoCancelamentoFatura = configuracaoFatura?.InformarDataCancelamentoCancelamentoFatura ?? false,
                DisponbilizarProvisaoContraPartidaParaCancelamento = configuracaoFatura?.DisponbilizarProvisaoContraPartidaParaCancelamento ?? false,
                HabilitarLayoutFaturaNFSManual = configuracaoFatura?.HabilitarLayoutFaturaNFSManual ?? false
            };
        }

        private dynamic ObterConfiguracaoPedido(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();

            return new
            {
                configuracaoPedido.ApagarCampoRotaAoDuplicarPedido,
                configuracaoPedido.ConcatenarNumeroPreCargaNoPedido,
                configuracaoPedido.PermitirInformarAcrescimoDescontoNoPedido,
                configuracaoPedido.PermitirMudarStatusPedidoParaCanceladoAposVinculoCarga,
                configuracaoPedido.PessoasNaoObrigatorioProdutoEmbarcador,
                configuracaoPedido.NaoApagarCamposDatasAoDuplicarPedido,
                FormatoDataCarregamentoImportacaoPedido = configuracaoPedido.FormatoDataCarregamento,
                FormatoHoraCarregamentoImportacaoPedido = configuracaoPedido.FormatoHoraCarregamento,
                configuracaoPedido.NaoPermitirImportarPedidosExistentes,
                configuracaoPedido.PermitirBuscarValoresTabelaFrete,
                FilialPadraoImportacaoPedido = configuracaoPedido.FilialPadraoImportacaoPedido != null ? new { configuracaoPedido.FilialPadraoImportacaoPedido.Codigo, configuracaoPedido.FilialPadraoImportacaoPedido.Descricao } : null,
                configuracaoPedido.PermitirCriarPedidoApenasMotoristaSituacaoTrabalhando,
                configuracaoPedido.BuscarEmpresaPeloProprietarioDoVeiculo,
                configuracaoPedido.UtilizarRelatorioPedidoComoStatusEntrega,
                configuracaoPedido.ExibirCamposRecebimentoPedidoIntegracao,
                configuracaoPedido.NaoPermitirAlterarCentroResultadoPedido,
                configuracaoPedido.NaoPermitirInformarExpedidorNoPedido,
                configuracaoPedido.BloquearInsercaoNotaComEmitenteDiferenteRemetentePedido,
                configuracaoPedido.BloquearDuplicarPedido,
                configuracaoPedido.BloquearPedidoAoIntegrar,
                configuracaoPedido.UtilizarEnderecoExpedidorRecebedorPedido,
                configuracaoPedido.NaoValidarMesmaViagemEMesmoContainer,
                configuracaoPedido.QuantidadeCargasEmAberto,
                configuracaoPedido.AtivarValidacaoCriacaoCarga,
                configuracaoPedido.NaoPreencherRotaFreteAutomaticamente,
                configuracaoPedido.NaoExibirPedidosDoDiaAgendamentoPedidos,
                configuracaoPedido.ImportarOcorrenciasDePedidosPorPlanilhas,
                configuracaoPedido.NaoSelecionarModeloVeicularAutomaticamente,
                configuracaoPedido.NaoSubstituirEmpresaNaGeracaoCarga,
                ModeloEmailAgendamentoPedido = configuracaoPedido.ModeloEmailAgendamentoPedido,
                configuracaoPedido.ValidarCadastroContainerPelaFormulaGlobal,
                configuracaoPedido.EnviarEmailTransportadorEntregaEmAtraso,
                configuracaoPedido.PermitirConsultaMassivaDePedidos,
                configuracaoPedido.PermiteInformarPedidoDeSubstituicao,
                configuracaoPedido.IgnorarValidacoesDatasPrevisaoAoEditarPedido,
                configuracaoPedido.HerdarNotasImportadasPedido,
                configuracaoPedido.GerarCargaAutomaticamenteNoPedido,
                configuracaoPedido.UtilizarBloqueioPessoasGrupoApenasParaTomadorDoPedido,
                configuracaoPedido.AtualizarCamposPedidoPorPlanilha,
                configuracaoPedido.NaoPermitirInformarVeiculoDuplicadoPedidoCargaAberta,
                configuracaoPedido.ExibirHoraCarregamentoEmPedidosDeColetaECodigosIntegracao,
                configuracaoPedido.HabilitarBIDTransportePedido,
                configuracaoPedido.UsarFatorConversaoProdutoEmPedidoPaletizado,
                configuracaoPedido.PermitirSelecionarCentroDeCarregamentoNoPedido,
                configuracaoPedido.UtilizarCampoDeMotivoDePedido,
                configuracaoPedido.AjustarParticipantesPedidoCTeEmitidoEmbarcador,
                configuracaoPedido.NaoLevarNumeroCotacaoParaPedidoGerado,
                configuracaoPedido.ImportarParalelizando,
                configuracaoPedido.SempreConsiderarDestinatarioInformadoNoPedido,
                configuracaoPedido.ExigirRotaRoteirizadaNoPedido,
                configuracaoPedido.ExibirAuditoriaPedidos,
                configuracaoPedido.UtilizarParametrosBuscaAutomaticaClienteImportacao,
                configuracaoPedido.RemoverObservacoesDeEntregaAoRemoverPedidoCarga,
                configuracaoPedido.AtualizarCargaAoImportarPlanilha,
                HabilitarOpcoesDeDuplicacaoDoPedidoParaDevolucaoTotalParcial = configuracaoPedido?.HabilitarOpcoesDeDuplicacaoDoPedidoParaDevolucaoTotalParcial ?? false,
                QuantidadeDiasDataColeta = configuracaoPedido.QuantidadeDiasDataColeta > 0 ? configuracaoPedido.QuantidadeDiasDataColeta : 120,
            };
        }

        private dynamic ObterConfiguracaoOcorrencia(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            return new
            {
                VisualizarUltimoUsuarioDelegadoOcorrencia = configuracaoOcorrencia?.VisualizarUltimoUsuarioDelegadoOcorrencia ?? false,
                PermiteInformarCentroResultadoAprovacaoOcorrencia = configuracaoOcorrencia?.PermiteInformarCentroResultadoAprovacaoOcorrencia ?? false,
                PermiteGerarOcorrenciaCargaAnulada = configuracaoOcorrencia?.PermiteGerarOcorrenciaCargaAnulada ?? false,
                ExibirDestinatarioOcorrencia = configuracaoOcorrencia?.ExibirDestinatarioOcorrencia ?? false,
                EnviarXMLDANFEClienteOcorrenciaPedido = configuracaoOcorrencia?.EnviarXMLDANFEClienteOcorrenciaPedido ?? false,
                configuracaoOcorrencia.PermiteDownloadCompactadoArquivoOcorrencia,
                configuracaoOcorrencia.NaoImprimirTipoOcorrenciaNaObservacaoCTeComplementar,
                configuracaoOcorrencia.ExibirCampoInformativoPagadorAutorizacaoOcorrencia,
                configuracaoOcorrencia.SalvarDocumentosDoCteAnteriorAoImportarCTeComplementar,
                configuracaoOcorrencia.InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos,
                configuracaoOcorrencia.UtilizarBonificacaoParaTransportadoresViaOcorrencia,
                configuracaoOcorrencia.PermitirDefinirCSTnoTipoDeOcorrencia,
                configuracaoOcorrencia.PermiteAdicionarMaisOcorrenciaMesmoEvento,
                configuracaoOcorrencia.UtilizarNumeroTentativasTempoIntervaloIntegracaoOcorrenciaPersonalizado,
                configuracaoOcorrencia.IntervaloMinutosEntreIntegracoes,
                configuracaoOcorrencia.NumeroTentativasIntegracao,
                PermiteInformarMaisDeUmaOcorrenciaPorNFe = configuracaoOcorrencia?.PermiteInformarMaisDeUmaOcorrenciaPorNFe ?? false,
                GerarObservacaoSubstitutoSomenteNumeroCTeAnterior = configuracaoOcorrencia?.GerarObservacaoSubstitutoSomenteNumeroCTeAnterior ?? false,
                configuracaoOcorrencia.HabilitarImportacaoOcorrenciaViaNOTFIS,
                configuracaoOcorrencia.IgnorarSituacaoDasNotasAoGerarOcorrencia,
                configuracaoOcorrencia.PermitirReabrirOcorrenciaEmCasoDeRejeicao,
                configuracaoOcorrencia.PermitirIncluirOcorrenciaPorSelecaoNotasFiscaisCTe,
                configuracaoOcorrencia.NaoGerarAtendimentoDuplicadoParaMesmaOcorrencia,
                configuracaoOcorrencia.TrazerCentroResultadoOcorrencia,
                configuracaoOcorrencia.UtilizaUsuarioPadraoParaGeracaoOcorrenciaPorEDI,
                configuracaoOcorrencia.ExibirTodosCTesDaCargaNaAutorizacaoDeOcorrencia,
                UsuarioPadraoParaGeracaoOcorrenciaPorEDI = new { Codigo = configuracaoOcorrencia.UsuarioPadraoParaGeracaoOcorrenciaPorEDI?.Codigo ?? 0, Descricao = configuracaoOcorrencia.UsuarioPadraoParaGeracaoOcorrenciaPorEDI?.Descricao ?? string.Empty },
                PermitirVinculoAutomaticoEntreOcorreciaEAtendimento = configuracaoOcorrencia?.PermitirVinculoAutomaticoEntreOcorreciaEAtendimento ?? false
            };
        }

        private dynamic ObterConfiguracaoWebService(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

            return new
            {
                configuracaoWebService.AtualizarDadosVeiculoIntegracaoCarga,
                configuracaoWebService.AdicionarVeiculoTipoReboqueComoReboqueAoAdicionarCarga,
                configuracaoWebService.QuantidadeHorasPreencherDataCarregamentoCarga,
                configuracaoWebService.IgnorarCamposEssenciais,
                configuracaoWebService.NaoRetornarImagemCanhoto,
                configuracaoWebService.RetornarEntregasRejeitadas,
                configuracaoWebService.SelecionarRotaFreteAoAdicionarPedido,
                configuracaoWebService.NaoRoteirizarRotaNovamente,
                configuracaoWebService.SempreUtilizarTomadorEnviadoNoPedido,
                configuracaoWebService.UtilizarCodigosDeCadastroComoEnredecoSecundario,
                configuracaoWebService.FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoaDentroDeEnderecos,
                configuracaoWebService.CadastroAutomaticoPessoaExterior,
                configuracaoWebService.TornarCampoINSSeReterImpostoTrazerComoSim,
                configuracaoWebService.AtivarValidacaoDosProdutosNoAdicionarCarga,
                configuracaoWebService.PermitirUsarDescricaoFaixaTemperatura,
                configuracaoWebService.RetornarApenasCarregamentosPendentesComTransportadora,
                configuracaoWebService.NaoSobrePorInformacoesViaIntegracao,
                configuracaoWebService.BloquearInclusaoCargaComMesmoNumeroPedidoEmbarcador,
                configuracaoWebService.RetornarCarregamentosSomenteCargasEmAgNF,
                configuracaoWebService.NaoPermitirSolicitarCancelamentoCargaViaIntegracaoViagemIniciada,
                configuracaoWebService.NaoRetornarNFSeVinculadaNFSManualMetodoBuscarNFSs,
                configuracaoWebService.PermiteReceberDataCriacaoPedidoERP,
                configuracaoWebService.NaoRecalcularFreteAoAdicionarRemoverPedido,
                configuracaoWebService.AtualizarTodosCadastrosMotoristasMesmoCodigoIntegracao,
                configuracaoWebService.SalvarRegiaoNoClienteParaPreencherRegiaoDestinoDosPedidos,
                configuracaoWebService.NaoPermitirGerarNFSeComMesmaNumeracao,
                configuracaoWebService.SempreSeguirConfiguracaoOcorrenciaQuandoAdicionadaPeloMetodoAdicionarOcorrencia,
                configuracaoWebService.HabilitarFluxoPedidoEcommerce,
                configuracaoWebService.AtualizarNumeroPedidoVinculado,
                configuracaoWebService.RetornarDadosRedespachoTransbordoComInformacoesCargaOrigemConsultada,
                configuracaoWebService.NaoValidarTipoDeVeiculoNoMetodoInformarDadosTransporteCarga,
                configuracaoWebService.CadastrarVeiculoAoInformarDadosTransporteCarga,
                configuracaoWebService.NaoRetornarCargasCanceladasMetodoBuscarPendetesNotasFiscais,
                configuracaoWebService.NaoPermitirConfirmarEntregaSituacaoCargaEmAndamento,
                configuracaoWebService.RetornarCargasEmQualquerEtapaNoMetodoBuscarCargaPendenteIntegracao,
                configuracaoWebService.RetornarApenasOcorrenciasFinalizadasMetodoBuscarOcorrenciasPendentesIntegracao,
                configuracaoWebService.PermitirRemoverDataPrevisaoDataPagamentoMetodoInformarPrevisaoPagamentoCTe,
                configuracaoWebService.DesvincularPreenchimentoDasDatasNosMetodosInformarPrevisaoPagamentoCTeConfirmarPagamentoCTe,
                configuracaoWebService.GerarLogMetodosREST,
                configuracaoWebService.PermitirAlterarNumeroCargaQuandoForCarga,
                configuracaoWebService.PermitirRemoverVeiculoNoMetodoInformarDadosTransporteCarga,
                configuracaoWebService.AoSalvarDocumentoTransporteValidarSituacaoCarga,
                configuracaoWebService.NaoVincularReboqueNaTracaoAoAcionarMetodoGerarCarregamento,
                configuracaoWebService.NaoFiltrarSequencialCargaNoMetodoAdicionarCargaPedido,
                configuracaoWebService.QuandoGeradoPreCteRetornarInformacaoDeFreteCTeIntegrado,
                FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoa = configuracaoWebService.FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoaDentroDeEnderecos,
            };
        }

        private dynamic ObterConfiguracaoTabelaFrete(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

            return new
            {
                PermitirInformarLeadTimeTabelaFreteCliente = configuracaoTabelaFrete.PermitirInformarLeadTimeTabelaFreteCliente,
                ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais = configuracaoTabelaFrete.ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais,
                UtilizarIntegracaoAlteracaoTabelaFrete = configuracaoTabelaFrete.UtilizarIntegracaoAlteracaoTabelaFrete,
                UtilizarVigenciaConfiguracaoDescargaCliente = configuracaoTabelaFrete.UtilizarVigenciaConfiguracaoDescargaCliente,
                MostrarRegistroSomenteComValoresNaAprovacao = configuracaoTabelaFrete.MostrarRegistroSomenteComValoresNaAprovacao,
                ObrigatorioInformarTransportadorAjusteTabelaFrete = configuracaoTabelaFrete.ObrigatorioInformarTransportadorAjusteTabelaFrete,
                ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete = configuracaoTabelaFrete.ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete,
                NaoBuscarAutomaticamenteVigenciaTabelaFrete = configuracaoTabelaFrete.NaoBuscarAutomaticamenteVigenciaTabelaFrete,
                ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo = configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo,
                GravarAuditoriaImportarTabelaFrete = configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete,
                configuracaoTabelaFrete.ArredondarValorDoComponenteDePedagioParaProximoInteiro,
                configuracaoTabelaFrete.SalvarPlacasVeiculosAoSalvarModelosVeiculos,
                configuracaoTabelaFrete.NaoPermiteEdicoesEmValoresNaConsultaDeTabelaFrete
            };
        }

        private dynamic ObterConfiguracaoJanelaCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

            return new
            {
                ExibirOpcaoLiberarParaTransportador = configuracaoJanelaCarregamento.ExibirOpcaoLiberarParaTransportador,
                ExibirOpcaoMultiModalAgendamentoColeta = configuracaoJanelaCarregamento.ExibirOpcaoMultiModalAgendamentoColeta,
                SugerirDataEntregaAgendamentoColeta = configuracaoJanelaCarregamento.SugerirDataEntregaAgendamentoColeta,
                GerarFluxoPatioCargaComExpedidor = configuracaoJanelaCarregamento.GerarFluxoPatioCargaComExpedidor,
                DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga = configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga,
                AtualizarDataInicialColetaAoAlterarHorarioCarregamento = configuracaoJanelaCarregamento.AtualizarDataInicialColetaAoAlterarHorarioCarregamento,
                BloquearGeracaoJanelaParaCargaRedespacho = configuracaoJanelaCarregamento.BloquearGeracaoJanelaParaCargaRedespacho,
                AtivarPlanejamentoFrotaCarga = configuracaoJanelaCarregamento.AtivarPlanejamentoFrotaCarga,
                AtivarPlanejamentoFrotaNoPlanejamentoVeiculo = configuracaoJanelaCarregamento.AtivarPlanejamentoFrotaNoPlanejamentoVeiculo,
                NaoPermitirRecalcularValorFreteInformadoPeloTransportador = configuracaoJanelaCarregamento.NaoPermitirRecalcularValorFreteInformadoPeloTransportador,
                NaoEnviarEmailAlteracaoDataCarregamento = configuracaoJanelaCarregamento.NaoEnviarEmailAlteracaoDataCarregamento,
                BloquearVeiculoSemTagValePedagioAtiva = configuracaoJanelaCarregamento.BloquearVeiculoSemTagValePedagioAtiva,
                DiasParaPermitirInformarHorarioCarregamento = configuracaoJanelaCarregamento.DiasParaPermitirInformarHorarioCarregamento > 0 ? configuracaoJanelaCarregamento.DiasParaPermitirInformarHorarioCarregamento.ToString("n0") : "",
                EncaixarHorarioRetiradaProduto = configuracaoJanelaCarregamento.EncaixarHorarioRetiradaProduto,
                LiberarCargaParaCotacaoAoLiberarParaTransportadores = configuracaoJanelaCarregamento.LiberarCargaParaCotacaoAoLiberarParaTransportadores,
                PermitirLiberarCargaParaTransportadoresTerceiros = configuracaoJanelaCarregamento?.PermitirLiberarCargaParaTransportadoresTerceiros ?? false,
                GerarJanelaDeCarregamento = configuracaoJanelaCarregamento?.GerarJanelaDeCarregamento ?? false,
                UtilizarCentroDescarregamentoPorTipoCarga = configuracaoJanelaCarregamento?.UtilizarCentroDescarregamentoPorTipoCarga ?? false,
                ExibirDetalhesAgendamentoJanelaTransportador = configuracaoJanelaCarregamento?.ExibirDetalhesAgendamentoJanelaTransportador ?? false,
                UtilizarPeriodoDescarregamentoExclusivo = configuracaoJanelaCarregamento?.UtilizarPeriodoDescarregamentoExclusivo ?? false,
                DisponibilizarCargaParaTransportadoresPorPrioridade = configuracaoJanelaCarregamento?.DisponibilizarCargaParaTransportadoresPorPrioridade ?? false,
                ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga = configuracaoJanelaCarregamento?.ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga ?? false,
                NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento = configuracaoJanelaCarregamento.NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento,
                TempoPermitirReagendamentoHoras = configuracaoJanelaCarregamento.TempoPermitirReagendamentoHoras > 0 ? configuracaoJanelaCarregamento.TempoPermitirReagendamentoHoras.ToString("n0") : "",
                PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga = configuracaoJanelaCarregamento.PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga
            };
        }

        private dynamic ObterConfiguracaoDocumentoEntrada(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();

            return new
            {
                BloquearFinalizacaoComFluxoCompraAberto = configuracaoDocumentoEntrada?.BloquearFinalizacaoComFluxoCompraAberto ?? false,
                PermitirSelecionarOSFinalizadaDocumentoEntrada = configuracaoDocumentoEntrada?.PermitirSelecionarOSFinalizadaDocumentoEntrada ?? false,
                FormulaCustoPadrao = configuracaoDocumentoEntrada?.FormulaCustoPadrao ?? "",
                BloquearCadastroProdutoComMesmoCodigo = configuracaoDocumentoEntrada?.BloquearCadastroProdutoComMesmoCodigo ?? false
            };
        }

        private dynamic ObterConfiguracaoCargaDadosTransporte(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoCargaDadosTransporte = repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistro();

            return new
            {
                configuracaoCargaDadosTransporte.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte,
                configuracaoCargaDadosTransporte.ExigirQueVeiculoCavaloTenhaReboqueVinculado,
                configuracaoCargaDadosTransporte.ExigirProtocoloLiberacaoSemIntegracaoGR,
                configuracaoCargaDadosTransporte.ExigirQueApolicePropriaTransportadorEstejaValida
            };
        }

        private dynamic ObterConfiguracaoCargaIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repositorioConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaDadosTransporte = repositorioConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();

            return new
            {
                AceitarPedidosComPendenciasDeProdutos = configuracaoCargaDadosTransporte?.AceitarPedidosComPendenciasDeProdutos ?? false,
                NaoRetornarDocumentosAnteriores = configuracaoCargaDadosTransporte?.NaoRetornarDocumentosAnteriores ?? false
            };
        }

        private dynamic ObterConfiguracaoMobile(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile repConfiguracaoMobile = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile configuracaoMobile = repConfiguracaoMobile.BuscarConfiguracaoPadrao();

            return new
            {
                ValidarRaioCliente = configuracaoMobile.ValidarRaioCliente,
                RetornarMultiplasCargasApp = configuracaoMobile.RetornarMultiplasCargasApp,
                DiasLimiteRetornarMultiplasCargas = configuracaoMobile.DiasLimiteRetornarMultiplasCargas,
                MenuCarga = configuracaoMobile.MenuCarga,
                MenuServicos = configuracaoMobile.MenuServicos,
                MenuOcorrencias = configuracaoMobile.MenuOcorrencias,
                MenuExtratoViagem = configuracaoMobile.MenuExtratoViagem,
                MenuPontosParada = configuracaoMobile.MenuPontosParada,
                MenuServicosViagem = configuracaoMobile.MenuServicosViagem,
                MenuRH = configuracaoMobile.MenuRH,
                DataReferenciaBusca = configuracaoMobile.DataReferenciaBusca,
                IntervaloInicial = configuracaoMobile.IntervaloInicial,
                IntervaloFinal = configuracaoMobile.IntervaloFinal,
                HabilitarAlertaMotorista = configuracaoMobile.HabilitarAlertaMotorista,
                MinutosNotificarMotoristaAlertaViagem = configuracaoMobile.MinutosNotificarAlertaMotoristaViagem,
            };
        }

        private dynamic ObterConfiguracaoAcertoViagem(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarConfiguracaoPadrao();

            return new
            {
                PermitirLancamentoOutrasDespesasDentroPeriodoAcerto = config.PermitirLancamentoOutrasDespesasDentroPeriodoAcerto,
                VisualizarReciboPorMotoristaNoAcertoDeViagem = config.VisualizarReciboPorMotoristaNoAcertoDeViagem,
                NaoObrigarInformarFrotaNoAcertoDeViagem = config.NaoObrigarInformarFrotaNoAcertoDeViagem,
                NaoFecharAcertoViagemAteReceberPallets = configuraoAcertoViagem?.NaoFecharAcertoViagemAteReceberPallets ?? false,
                VisualizarPalletsCanhotosNasCargas = configuraoAcertoViagem?.VisualizarPalletsCanhotosNasCargas ?? false,
                HabilitarFormaRecebimentoTituloAoMotorista = configuraoAcertoViagem?.HabilitarFormaRecebimentoTituloAoMotorista ?? false,
                HabilitarLancamentoTacografo = configuraoAcertoViagem?.HabilitarLancamentoTacografo ?? false,
                SepararValoresAdiantamentoMotoristaPorTipo = configuraoAcertoViagem?.SepararValoresAdiantamentoMotoristaPorTipo ?? false,
                HabilitarInformacaoAcertoMotorista = configuraoAcertoViagem?.HabilitarInformacaoAcertoMotorista ?? false,
                HabilitarControlarOutrasDespesas = configuraoAcertoViagem?.HabilitarControlarOutrasDespesas ?? false,
                TextoRecibo = configuraoAcertoViagem?.TextoRecibo ?? string.Empty,
            };
        }

        private dynamic ObterConfiguracaoVeiculo(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

            return new
            {
                configuracaoVeiculo.ObrigatorioSegmentoVeiculo,
                configuracaoVeiculo.VisualizarApenasVeiculosAtivos,
                configuracaoVeiculo.ObrigatorioInformarAnoFabricacao,
                configuracaoVeiculo.ObrigatorioInformarReboqueParaVeiculosDoTipoRodadoCavalo,
                configuracaoVeiculo.ManterVinculoMotoristaEmFolga,
                configuracaoVeiculo.NaoPermitirAlterarKMVeiculoEquipamentoPneuPelaOrdemServico,
                configuracaoVeiculo.BloquearAlteracaoCentroResultadoNaMovimentacaoPlaca,
                configuracaoVeiculo.NaoPermitirQueTransportadorInativeVeiculo,
                configuracaoVeiculo.KMLimiteAberturaOrdemServico,
                configuracaoVeiculo.NaoPermitirRealizarCadastroPlacaBloqueada,
                configuracaoVeiculo.NaoPermitirUtilizarVeiculoEmManutencao,
                configuracaoVeiculo.LancamentoServicoManualNaOSMarcadadoPorDefault,
                configuracaoVeiculo.NaoPermitirVincularPneuVeiculoAbastecimentoAberto,
                configuracaoVeiculo.NaoPermitirCadastrarVeiculoSemRastreador,
                configuracaoVeiculo.CriarVinculoFrotaCargaForaDoPlanejamentoFrota,
                configuracaoVeiculo.NaoAlterarCentroResultadoVeiculosEmissaoCargas,
                configuracaoVeiculo.NaoPermitirRealizarFechamentoOrdemServicoCustoZerado,
                configuracaoVeiculo.ValidarExisteCargaMesmoNumeroCIOT,
                configuracaoVeiculo.RemoverNumeroCIOTEncerrado,
                configuracaoVeiculo.ObrigatorioInformarModeloVeicularCargaNoWebService,
                configuracaoVeiculo.SalvarTransportadorTerceiroComoGerarCIOT,
                configuracaoVeiculo.ExibirAbaDeEixosNoModeloVeicular,
                configuracaoVeiculo.NaoMostrarMotivoBloqueio,
                configuracaoVeiculo.ObrigarANTTVeiculoValidarSalvarDadosTransporte,
                configuracaoVeiculo.UtilizarMesmoGestorParaTodaComposicao,
                configuracaoVeiculo.CadastrarVeiculoMotoristaBRK,
                configuracaoVeiculo.ValidarTAGDigitalCom,
                configuracaoVeiculo.InformarKmMovimentacaoPlaca,
                configuracaoVeiculo.AtualizarHistoricoSituacaoVeiculo
            };
        }

        private dynamic ObterConfiguracaoMotoristaUsuario(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repConfiguracaoMotorista.BuscarConfiguracaoPadrao();

            return new
            {
                configuracaoMotorista.NaoPermitirTransportadoAlterarDataValidadeSeguradora,
                configuracaoMotorista.BloquearCamposMotoristaLGPD,
                configuracaoMotorista.ExibirCamposSuspensaoMotorista,
                configuracaoMotorista.BloquearChecklistMotoristaSemLicencaVinculada,
                configuracaoMotorista.NaoPermitirInativarMotoristaComSaldoNoExtrato,
                configuracaoMotorista.PermitirCadastrarMotoristaEstrangeiro,
                configuracaoMotorista.NaoValidarHoraNoPagamentoMotorista,
                configuracaoMotorista.MotoristaUsarFotoDoApp,
                configuracaoMotorista.ExibirConfiguracoesPortalTransportador,
                configuracaoMotorista.NaoPermitirRealizarCadastroMotoristaBloqueado,
                configuracaoMotorista.HabilitarControleSituacaoColaboradorParaMotoristasTerceiros,
                configuracaoMotorista.PermiteDuplicarCadastroMotorista,
                configuracaoMotorista.DiasAntecidenciaParaComunicarMotoristaVencimentoLicenca,
                configuracaoMotorista.MensagemPersonalizadaMotoristaBloqueado,
                configuracaoMotorista.NaoGerarPreTripMotoristasIgnorados,
                configuracaoMotorista.HabilitarUsoCentroResultadoComissaoMotorista,
                MotoristasIgnorados = (
                    from config in configuracaoMotorista.MotoristasIgnorados
                    select new
                    {
                        NomeMotoristaIgnorado = config
                    }
                ).ToList()
            };
        }

        private dynamic ObterConfiguracaoEncerramentoMDFeAutomatico(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe repConfiguracaoEncerramentoAutomaticoMDFe = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe configuracaoEncerramentoAutomatico = repConfiguracaoEncerramentoAutomaticoMDFe.BuscarConfiguracaoPadrao();

            return new
            {
                configuracaoEncerramentoAutomatico.DiasEncerramentoAutomaticoMDFE
            };
        }

        private dynamic ObterConfiguracaoFinanceiro(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            return new
            {
                configuracaoFinanceiro.ValidarDuplicidadeTituloSemData,
                configuracaoFinanceiro.AtivarControleDespesas,
                configuracaoFinanceiro.DiasDeFechamentoParaGeracaoPagamentoEscrituracaoAutomatico,
                configuracaoFinanceiro.GerarLotesAposEmissaoDaCarga,
                configuracaoFinanceiro.GerarLotePagamentoAposDigitalizacaoDoCanhoto,
                configuracaoFinanceiro.GerarLotesProvisaoAposEmissaoDaCarga,
                configuracaoFinanceiro.NaoGerarLoteProvisaoParaCargaAguardandoImportarCTeOuLancarNFS,
                configuracaoFinanceiro.NaoGerarLoteProvisaoParaOcorrencia,
                configuracaoFinanceiro.DesbloquearPagamentoPorCanhoto,
                configuracaoFinanceiro.ExibirNumeroPedidoEmbarcadorGestaoDocumentos,
                configuracaoFinanceiro.PermitirDeixarDocumentoEmTratativa,
                Despachante = configuracaoFinanceiro.Despachante != null ? new { configuracaoFinanceiro.Despachante.Codigo, configuracaoFinanceiro.Despachante.Descricao } : null,
                configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo,
                configuracaoFinanceiro.MovimentacaoFinanceiraParaTitulosDeProvisao,
                configuracaoFinanceiro.UtilizarValorDesproporcionalRateioDespesaVeiculo,
                configuracaoFinanceiro.UtilizarCustoParaRealizarRateiosSobreDocumentoEntrada,
                configuracaoFinanceiro.GerarMovimentoPelaDataVencimentoContratoFinanceiro,
                configuracaoFinanceiro.AutomatizarGeracaoLoteProvisao,
                configuracaoFinanceiro.UtilizarDataVencimentoTituloMovimentoContrato,
                configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins,
                configuracaoFinanceiro.NaoObrigarTipoOperacaoFatura,
                configuracaoFinanceiro.HabilitarOpcaoGerarFaturasApenasCanhotosAprovados,
                configuracaoFinanceiro.AtivarColunaCSTConsultaDocumentosFatura,
                configuracaoFinanceiro.AtivarColunaNumeroContainerConsultaDocumentosFatura,
                configuracaoFinanceiro.ExigirInformarFilialEmissaoFaturas,
                configuracaoFinanceiro.RatearMovimentosDescontosAcrescimosBaixaTitulosPagar,
                configuracaoFinanceiro.PermitirConfirmarDocumentosFaturaApenasComCtesEscriturados,
                configuracaoFinanceiro.NaoValidarCondicaoPagamentoFechamentoLotePagamento,
                configuracaoFinanceiro.ValidarDataPrevisaoPagamentoEDataPagamentoNoCancelamentoDosCTes,
                configuracaoFinanceiro.GerarLotesPagamentoIndividuaisPorDocumento,
                configuracaoFinanceiro.SomarValorISSNoTotalReceberGeracaoLoteProvisao,
                configuracaoFinanceiro.HorasLiberacaoDocumentoPagamentoTransportadorComCertificado,
                configuracaoFinanceiro.HorasLiberacaoDocumentoPagamentoTransportadorSemCertificado,
                configuracaoFinanceiro.NaoGerarAutomaticamenteLotesCancelados,
                MinutosAguardarGeracaoLotePagamento = ConverterMinutosAguardarGeracaoLotePagamentoParaHoras(configuracaoFinanceiro.MinutosAguardarGeracaoLotePagamento),
                MinutosAguardarGeracaoLotePagamentoUltimoDiaMes = ConverterMinutosAguardarGeracaoLotePagamentoParaHoras(configuracaoFinanceiro.MinutosAguardarGeracaoLotePagamentoUltimoDiaMes),
                configuracaoFinanceiro.QuantidadeDiasAbertoEstornoProvisao,
                configuracaoFinanceiro.HabilitaIntervaloTempoLiberaDocumentoEmitidoEscrituracao,
                configuracaoFinanceiro.GerarDoumentoProvisaoAoReceberNotaFiscal,
                configuracaoFinanceiro.NaoPermitirProvisionarSemCalculoFrete,
                configuracaoFinanceiro.GerarEstornoProvisaoAutomaticoAposEscrituracao,
                configuracaoFinanceiro.GerarEstornoProvisaoAutomaticoAposLiberacaoPagamento,
                configuracaoFinanceiro.UtilizarEstornoProvisaoDeFormaAutomatizada,
                configuracaoFinanceiro.RateioProvisaoPorGrupoProduto,
                configuracaoFinanceiro.GerarLoteProvisaoIndividualNfe,
                configuracaoFinanceiro.UtilizarFechamentoAutomaticoProvisao,
                ConfiguracaoIntervaloTempoLiberaDocumentoEmitidoEscrituracao = ObterConfiguracaoIntervaloTempoLiberaDocumentoEmitidoEscrituracao(configuracaoFinanceiro, unitOfWork),
                configuracaoFinanceiro.NaoGerarPagamentoParaMotoristaTerceiro,
                configuracaoFinanceiro.EfetuarVinculoCentroResultadoCTeSubstituto,
                configuracaoFinanceiro.EfetuarCancelamentoDePagamentoAoCancelarCarga,
                configuracaoFinanceiro.BloqueioEnvioIntegracoesCargasAnuladaseCanceladas,
                configuracaoFinanceiro.DelayFaturamentoAutomatico,
                configuracaoFinanceiro.QuantidadeDocumentosLotePagamentoGeradoAutomatico,
                configuracaoFinanceiro.QuantidadeMinimaDocumentosLotePagamentoGeradoAutomatico,
                configuracaoFinanceiro.TravarFluxoCompraCasoFornecedorDivergenteNaOrdemCompra,
                configuracaoFinanceiro.GerarIntegracaoContabilizacaoCtesApos,
                configuracaoFinanceiro.DelayIntegracaoContabilizacaoCtes,
                configuracaoFinanceiro.AgruparProvisoesPorNotaFiscalFechamentoMensal,
                configuracaoFinanceiro.NaoPermitirGerarLotesPagamentosDocumentosBloqueados,
                configuracaoFinanceiro.NaoPermitirReenviarIntegracoesPagamentoSeCancelado,
                configuracaoFinanceiro.UtilizarConfiguracoesTransportadorParaFatura,
                configuracaoFinanceiro.GerarLotePagamentoSomenteParaCTe,
                MotivoCancelamentoPagamentoPadrao = configuracaoFinanceiro.MotivoCancelamentoPagamentoPadrao != null ?
                    new
                    {
                        configuracaoFinanceiro.MotivoCancelamentoPagamentoPadrao.Codigo,
                        configuracaoFinanceiro.MotivoCancelamentoPagamentoPadrao.Descricao
                    } : null,
                configuracaoFinanceiro.BaixaTitulosRenegociacaoGerarNovoTituloPorDocumento,
                configuracaoFinanceiro.PermitirProvisionamentoDeNotasCTesNaTelaProvisao,
                configuracaoFinanceiro.UtilizarPreenchimentoTomadorFaturaConfiguracao,
                configuracaoFinanceiro.ManterValorMoedaConfirmarDocumentosFatura,
                configuracaoFinanceiro.UtilizarEmpresaFilialImpressaoReciboPagamentoMotorista,
            };
        }

        private static string ConverterMinutosAguardarGeracaoLotePagamentoParaHoras(int? totalMinutos)
        {
            if (!totalMinutos.HasValue)
                return "";

            int horas = totalMinutos.Value / 60;
            int minutos = totalMinutos.Value % 60;
            return $"{horas:D3}:{minutos:D2}";
        }

        private static int? ConverterHorasAguardarGeracaoLotePagamentoParaMinutos(string totalHoras)
        {
            if (string.IsNullOrEmpty(totalHoras))
                return null;

            string[] parts = totalHoras.Split(':');

            if (parts.Length != 2)
                return null;

            int horas = int.Parse(parts[0]);
            int minutos = int.Parse(parts[1]);

            return horas * 60 + minutos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao> ObterConfiguracaoIntervaloTempoLiberaDocumentoEmitidoEscrituracao(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(unitOfWork);
            var documentosEmittidosEscrituracoes = repositorio.BuscarPorCondiguracaoFinanceira(configuracaoFinanceiro.Codigo);

            return documentosEmittidosEscrituracoes.Select(o => new Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao()
            {
                Codigo = o.Codigo,
                DiaFinal = o.DiaFinal,
                DiaInicial = o.DiaInicial,
                IntervaloHora = o.IntervaloHora

            }).ToList();
        }

        private dynamic ObterConfiguracaoChamado(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();

            return new
            {
                configuracaoChamado.BloquearAberturaChamadoRetencaoQuandoPossuirReentrega,
                configuracaoChamado.ObrigatorioInformarNotaNaDevolucaoParcialChamado,
                configuracaoChamado.HabilitarArvoreDecisaoEscalationList,
                configuracaoChamado.EscalarAutomaticamenteNivelExcederTempo,
                configuracaoChamado.FinalizarEntregaQuandoDevolucaoParcial,
                configuracaoChamado.ObrigatorioInformarNotaFiscalParaAberturaChamado,
                configuracaoChamado.CalcularValorDasDevolucoes,
                configuracaoChamado.PermitirRegistrarObservacoesSemVisualizacaoTransportadora,
                configuracaoChamado.AtivarAlertaChamadosMais48hAberto,
                configuracaoChamado.PermitirAbrirChamadoParaEntregaJaRealizada,
                configuracaoChamado.PermiteFinalizarAtendimentoComOcorrenciaRejeitada,
                configuracaoChamado.VincularPrimeiroPedidoDoClienteAoAbrirChamado,
                configuracaoChamado.PermitirSelecionarCteApenasComNfeVinculadaOcorrencia,
                PermitirGerarAtendimentoPorPedido = configuracaoChamado?.PermitirGerarAtendimentoPorPedido ?? false,
                FazerGestaoCriticidade = configuracaoChamado?.FazerGestaoCriticidade ?? false,
                PermitirAtualizarChamadoStatus = configuracaoChamado?.PermitirAtualizarChamadoStatus ?? false,
                OcultarTomadorNoAtendimento = configuracaoChamado?.OcultarTomadorNoAtendimento ?? false,
                BloquearEstornoAtendimentosFinalizadosPortalTransportador = configuracaoChamado?.BloquearEstornoAtendimentosFinalizadosPortalTransportador ?? false,
            };
        }

        private dynamic ObterConfiguracaoRedMine(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRedMine repConfiguracaoredMine = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRedMine(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRedMine configuracaoRedMine = repConfiguracaoredMine.BuscarConfiguracaoPadrao();

            return new
            {
                ClienteRedMine = configuracaoRedMine?.ClienteRedMine ?? "",
                CodigoUsuarioDestino = configuracaoRedMine?.CodigoUsuarioDestino ?? null
            };
        }

        private dynamic ObterConfiguracaoMontagemCarga(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();

            return new
            {
                configuracaoMontagemCarga.ExigirDefinicaoTipoCarregamentoPedido,
                configuracaoMontagemCarga.AtualizarInformacoesPedidosPorCarga,
                configuracaoMontagemCarga.ApresentaOpcaoRemoverCancelarPedidos,
                configuracaoMontagemCarga.ApresentaOpcaoCancelarReserva,
                configuracaoMontagemCarga.FiltroPeriodoVazioAoIniciar,
                configuracaoMontagemCarga.DataAtualNovoCarregamento,
                configuracaoMontagemCarga.OcultarBipagem,
                configuracaoMontagemCarga.UtilizarDataPrevisaoSaidaVeiculo,
                configuracaoMontagemCarga.NaoPermitirPedidosTomadoresDiferentesMesmoCarregamento,
                configuracaoMontagemCarga.GerarUnicoBlocoPorRecebedor,
                configuracaoMontagemCarga.PermitirGerarCarregamentoPedidoBloqueado,
                configuracaoMontagemCarga.AtivarTratativaDuplicidadeEmissaoCargasFeeder,
                configuracaoMontagemCarga.ConsiderarPesoNFSaldoPedido,
                configuracaoMontagemCarga.TipoControleSaldoPedido,
                configuracaoMontagemCarga.GerarCargaAoConfirmarIntegracaoCarregamento,
                configuracaoMontagemCarga.NaoRetornarIntegracaoCarregamentoSeSomenteDadosTransporteForemAlterados,
                configuracaoMontagemCarga.RoteirizarAutomaticamenteAposRoteirizadoAoAdicionarRemoverPedido,
                configuracaoMontagemCarga.ExibirAlertaRestricaoEntregaClienteCardCarregamento,
                configuracaoMontagemCarga.AtivarMontagemCargaPorNFe,
                configuracaoMontagemCarga.UtilizarFiliaisHabilitadasTransportarMontagemCargaMapa,
                configuracaoMontagemCarga.FiltrarPedidosOndeRecebedorTransportadorNoPortalDoTransportador,
                configuracaoMontagemCarga.VencedorSimuladorFreteEmpresaPedido,
                configuracaoMontagemCarga.PermitirEditarPedidosAtravesTelaMontagemCargaMapa,
                configuracaoMontagemCarga.IgnorarRotaFretePedidosMontagemCargaMapa,
                configuracaoMontagemCarga.ManterPedidosComMesmoAgrupadorNaMesmaCarga,
                configuracaoMontagemCarga.ExibirPedidosFormatoGrid,
                configuracaoMontagemCarga.ExibirListagemNotasFiscais,
                configuracaoMontagemCarga.ConsiderarSomentePesoOuCubagemAoGerarBloco,
                configuracaoMontagemCarga.FiltrarPedidosVinculadoOutrasCarga,
            };
        }

        private dynamic ObterConfiguracaoDocumentosDestinados(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado repConfiguracaoDocumentoDestinado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado configuracaoDocumentoDestinado = repConfiguracaoDocumentoDestinado.BuscarConfiguracaoPadrao();

            return new
            {
                configuracaoDocumentoDestinado.BloquearLancamentoDocumentosTipoEntrada,
                configuracaoDocumentoDestinado.SalvarDocumentosRecebidosEmailDestinados,
                configuracaoDocumentoDestinado.VincularCteNaOcorrenciaApartirDaObservacao,
                configuracaoDocumentoDestinado.NaoInutilizarCTEsFiscalmenteApenasGerencialmente,
                configuracaoDocumentoDestinado.NaoReutilizarNumeracaoAposAnularGerencialmente,
                configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha
            };
        }

        private dynamic ObterConfiguracaoPaletes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes repConfiguracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = repConfiguracaoPaletes.BuscarConfiguracaoPadrao();

            return new
            {
                UtilizarControlePaletesModeloVeicular = configuracaoPaletes.UtilizarControlePaletesPorModeloVeicular,
                LiquidarPalletAutomaticamente = configuracaoPaletes.LiquidarPalletAutomaticamente,
                QteDiasParaLiquidarPallet = configuracaoPaletes.QteDiasParaLiquidarPallet,
                NaoExibirDevolucaoPaletesSemNotaFiscal = configuracaoPaletes.NaoExibirDevolucaoPaletesSemNotaFiscal,
                TipoOcorrenciaPadraoPallet = new
                {
                    Codigo = configuracaoPaletes.TipoOcorrencia?.Codigo ?? 0,
                    Descricao = configuracaoPaletes.TipoOcorrencia?.Descricao ?? string.Empty
                },
                UtilizarControlePaletesPorCliente = configuracaoPaletes.UtilizarControlePaletesPorCliente,
                LimiteDiasParaDevolucaoDePallet = configuracaoPaletes.LimiteDiasParaDevolucaoDePallet,
                NotificarPaletesPendentes = configuracaoPaletes.NotificarPaletesPendentes,
                DiaSemanaNotificarPaletesPendentes = configuracaoPaletes.DiaSemanaNotificarPaletesPendentes
            };
        }

        private dynamic ObterConfiguracaoProduto(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto repConfiguracaoProduto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoProduto configuracaoProduto = repConfiguracaoProduto.BuscarConfiguracaoPadrao();

            return new
            {
                LayoutEtiquetaProduto = configuracaoProduto.LayoutEtiquetaProduto,
                ControlarEstoqueReserva = configuracaoProduto.ControlarEstoqueReserva,
                RealizarValidacaoComEstoqueDePosicaoAoFecharOrdemDeServico = configuracaoProduto.RealizarValidacaoComEstoqueDePosicaoAoFecharOrdemDeServico,
                SalvarProdutosDaNotaFiscal = configuracaoProduto?.SalvarProdutosDaNotaFiscal ?? false
            };
        }

        private dynamic ObterConfiguracaoCalculoPrevisao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao repConfiguracaoPrevisao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao configuracaoPrevisao = repConfiguracaoPrevisao.BuscarConfiguracaoPadrao();

            return new
            {
                DesconsiderarSabadosCalculoPrevisao = configuracaoPrevisao.DesconsiderarSabadosCalculoPrevisao,
                DesconsiderarDomingosCalculoPrevisao = configuracaoPrevisao.DesconsiderarDomingosCalculoPrevisao,
                DesconsiderarFeriadosCalculoPrevisao = configuracaoPrevisao.DesconsiderarFeriadosCalculoPrevisao,
                ConsiderarJornadaMotorita = configuracaoPrevisao.ConsiderarJornadaMotorita,
                HorarioInicialAlmoco = configuracaoPrevisao.HorarioInicialAlmoco,
                MinutosIntervalo = configuracaoPrevisao.MinutosIntervalo
            };
        }

        private dynamic ObterConfiguracaoCanhoto(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto> repositorioConfiguracaoCanhoto = new Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto>(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarPrimeiroRegistro();

            return new
            {
                configuracaoCanhoto.PrazoDiasAposDataEmissao,
                configuracaoCanhoto.NotificarCanhotosPendentesTodosOsDias,
                configuracaoCanhoto.DisponibilizarOpcaoDeCanhotoExtraviado,
                configuracaoCanhoto.PrazoParaReverterDigitalizacaoAposAprovacao,
                configuracaoCanhoto.NaoPermitirReceberCanhotosNaoDigitalizados,
                configuracaoCanhoto.NotificarTransportadorCanhotosQueEstaoComDigitalizacaoRejeitada,
                configuracaoCanhoto.PermitirAuditarCanhotosFinalizados,
                configuracaoCanhoto.TamanhoMaximoDaImagemDoCanhoto,
                configuracaoCanhoto.ValidarSituacaoDigitalizacaoCanhotosAoSumarizarDocumentoFaturamento,
                configuracaoCanhoto.MensagemRodapeEmailCanhotosPendentes,
                configuracaoCanhoto.LiberarParaPagamentoAposDigitalizacaCanhoto,
                configuracaoCanhoto.GerarCanhotoParaNotasTipoPallet,
                configuracaoCanhoto.PermitirBloquearDocumentoManualmente,
                configuracaoCanhoto.PermitirMultiplaSelecaoLancamentoLotePagamento,
                configuracaoCanhoto.PermitirAlterarImagemCanhotoDigitalizada,
                configuracaoCanhoto.RejeitarCanhotosNaoValidadosPeloOCR,
                configuracaoCanhoto.ExibirCanhotosSemVinculoComCarga,
                configuracaoCanhoto.ExigirDataEntregaNotaClienteCanhotosReceberFisicamente,
                configuracaoCanhoto.PermitirImportarCanhotoNFFaturada,
                configuracaoCanhoto.PermitirAtualizarSituacaoCanhotoPorImportacao,
                configuracaoCanhoto.PermitirAtualizarSituacaoCanhotoAvulsoPorImportacao,
                configuracaoCanhoto.PermitirImportarDocumentosFiltroSemChaveNFe,
                configuracaoCanhoto.IntegrarCanhotosComValidadorIAComprovei,
                configuracaoCanhoto.ReenviarUmaVezIntegracaoCasoRetornarFalhaNaValidacaoDoNumeroDoCanhotoEOuFormatoDoCanhoto,
                configuracaoCanhoto.ObterNumeroNotaFiscalPorObjetoOcr,
                configuracaoCanhoto.RetornarMetodoBuscarEntregasRealizadasPendentesIntegracaoSomenteCanhotoDigitalizado,
                configuracaoCanhoto.PermitirEnviarImagemParaMultiplosCanhotos,
                configuracaoCanhoto.NaoAtualizarTelaCanhotosAposAprovacaoRejeicao,
                configuracaoCanhoto.AprovarAutomaticamenteADigitalizacaoDosCanhotosCasoAValidacaoDaIAComproveiSejaCompleta,
                configuracaoCanhoto.PermitirAprovarDigitalizacaoDeCanhotoRejeitado,
                configuracaoCanhoto.ValidarSituacaoEntregaAoEnviarImagemCanhotoManualmente,
                configuracaoCanhoto.FlexibilidadeParaValidacaoNaIAComprovei,
                configuracaoCanhoto.NaoIntegrarIAComproveiCanhotosDeNotasDevolvidas,
                configuracaoCanhoto.PermitirRetornarStatusCanhotoNaAPIDigitalizacao,
                configuracaoCanhoto.HabilitarFluxoAnaliseCanhotoRejeitadoIA,
                configuracaoCanhoto.ValidacaoNumero,
                configuracaoCanhoto.ValidacaoAssinatura,
                configuracaoCanhoto.ValidacaoEncontrouData,
                configuracaoCanhoto.ValidacaoCanhoto,
                configuracaoCanhoto.EfetuarIntegracaoApenasCanhotosDigitalizados,
                configuracaoCanhoto.CompactarImagemCanhotoIaComproveiCasoTamanhoUltrapasseUmMB,
                configuracaoCanhoto.RetornarSomenteCanhotoComNFeEntregueEmBuscarCanhotosNotasFiscaisDigitalizados,
            };
        }

        private dynamic ObterConfiguracaoMonitoramento(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

            return new
            {
                configuracaoMonitoramento.TelaMonitoramentoFiltroFilialDaCarga,
                configuracaoMonitoramento.AgruparVeiculosMapaPosicaoFrota,
                configuracaoMonitoramento.EnviarAlertasMonitoramentoEmail,
                configuracaoMonitoramento.EmailsAlertaMonitoramento,
                configuracaoMonitoramento.AtualizarMonitoramentoAoGerarMDFeManual,
                configuracaoMonitoramento.AtualizarStatusViagemMonitoramentoAoIniciarViagem,
                configuracaoMonitoramento.ManterMonitoramentosDeCargasCanceladasAoReceberNovaCarga,
                configuracaoMonitoramento.NaoGerarNovoMonitoramentoCarga,
                configuracaoMonitoramento.ValorMinimoMonitoramentoCritico,
                configuracaoMonitoramento.TelaMonitoramentoAtualizarGridAoReceberAtualizacoesOnTime,
                configuracaoMonitoramento.IgnorarStatusViagemMonitoramentoAnterioresTransito,
                configuracaoMonitoramento.IdentificarEntradaEmAlvoComPosicaoUnicaIgnorandoTemposDePermanencia,
                configuracaoMonitoramento.IdentificarCarregamentoAoIniciarOuFinalizarMonitoramentosConsecutivos,
                configuracaoMonitoramento.FinalizarMonitoramentoAoGerarTransbordoCarga,
                configuracaoMonitoramento.GerarDadosSumarizadosDasParadasAoFinalizarOMonitoramento,
                configuracaoMonitoramento.GerarMonitoramentoAoFecharCarga,
                configuracaoMonitoramento.ConsiderarDataAuditoriaComoDataFimDoMonitoramento,
                configuracaoMonitoramento.FrequenciaCapturaPosicoesAppTrizy,
                configuracaoMonitoramento.FinalizarAutomaticamenteAlertasDoMonitoramentoAoFinalizarViagem,
                configuracaoMonitoramento.FinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente,
                DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente = configuracaoMonitoramento.DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente <= 0 ? 30 : configuracaoMonitoramento.DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente,
                configuracaoMonitoramento.FinalizarAutomaticamenteMonitoramentosEmAndamento,
                configuracaoMonitoramento.DiasFinalizarAutomaticamenteMonitoramentoEmAndamento,
                configuracaoMonitoramento.FinalizarAutomaticamenteMonitoramentosPrevisaoUltimaEntrega,
                configuracaoMonitoramento.DiasFinalizarMonitoramentoPrevisaoUltimaEntrega,
                configuracaoMonitoramento.DistanciaMaximaRotaCurta,
                configuracaoMonitoramento.TempoPermitidoPermanenciaEmCarregamento,
                configuracaoMonitoramento.TempoPermitidoPermanenciaNoCliente,
                configuracaoMonitoramento.HabilitarVinculoPermanenciasComHistoricoStatusViagem
            };
        }

        private dynamic ObterConfiguracaoGeral(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            return new
            {
                NaoCarregarPlanoEntradaSaidaTipoPagamento = configuracaoGeral?.NaoCarregarPlanoEntradaSaidaTipoPagamento ?? false,
                AtivarConsultaSegregacaoPorEmpresa = configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false,
                PermitirVincularVeiculoMotoristaViaPlanilha = configuracaoGeral?.PermitirVincularVeiculoMotoristaViaPlanilha ?? false,
                PermitirCriacaoDiretaMalotes = configuracaoGeral?.PermitirCriacaoDiretaMalotes ?? false,
                TransformarJanelaDeDescarregamentoEmMultiplaSelecao = configuracaoGeral?.TransformarJanelaDeDescarregamentoEmMultiplaSelecao ?? false,
                ControlarOrganizacaoProdutos = configuracaoGeral?.ControlarOrganizacaoProdutos ?? false,
                FiltrarPedidosSemFiltroPorFilialNoPortalDoFornecedor = configuracaoGeral?.FiltrarPedidosSemFiltroPorFilialNoPortalDoFornecedor ?? false,
                AgruparRelatorioOrdemColetaGuaritaPorDestinatario = configuracaoGeral?.AgruparRelatorioOrdemColetaGuaritaPorDestinatario ?? false,
                ValidarTransportadorNaoInformadoNaImportacaoDocumento = configuracaoGeral?.ValidarTransportadorNaoInformadoNaImportacaoDocumento ?? false,
                PermitirAgendamentoPedidosSemCarga = configuracaoGeral?.PermitirAgendamentoPedidosSemCarga ?? false,
                NaoPermitirDesabilitarCompraValePedagioVeiculo = configuracaoGeral?.NaoPermitirDesabilitarCompraValePedagioVeiculo ?? false,
                ImprimeOrdemServiçoCNPJMatriz = configuracaoGeral?.ImprimeOrdemServiçoCNPJMatriz ?? false,
                AlterarModeloDocumentoNFSManual = configuracaoGeral?.AlterarModeloDocumentoNFSManual ?? false,
                NaoDescontarValorDescontoItemAosAbastecimentosGeradosDocumentoEntrada = configuracaoGeral?.NaoDescontarValorDescontoItemAosAbastecimentosGeradosDocumentoEntrada ?? false,
                EnviarApenasEmailDiarioTaxasDescargaPendenteAprovacao = configuracaoGeral?.EnviarApenasEmailDiarioTaxasDescargaPendenteAprovacao ?? false,
                HabilitarEnvioPorSMSDeDocumentos = configuracaoGeral?.HabilitarEnvioPorSMSDeDocumentos ?? false,
                VisualizarGNRESemValidacaoDocumentos = configuracaoGeral?.VisualizarGNRESemValidacaoDocumentos ?? false,
                NaoPermitirFinalizarViagemDetalhesFimViagem = configuracaoGeral?.NaoPermitirFinalizarViagemDetalhesFimViagem ?? false,
                PermitirAdicionarMotoristaCargaMDFeManual = configuracaoGeral?.PermitirAdicionarMotoristaCargaMDFeManual ?? false,
                HabilitarCadastroArmazem = configuracaoGeral?.HabilitarCadastroArmazem ?? false,
                NaoBloquearEmissaoNFSeManualSemDANFSE = configuracaoGeral?.NaoBloquearEmissaoNFSeManualSemDANFSE ?? false,
                TipoRomaneio = configuracaoGeral?.TipoRomaneio ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRomaneio.Padrao,
                GerarRegistrosReceberGNREParaCTesComCST90 = configuracaoGeral?.GerarRegistrosReceberGNREParaCTesComCST90 ?? false,
                VisualizarPermitirAlterarDataEntregaNaConfirmacaoCanhoto = configuracaoGeral?.VisualizarPermitirAlterarDataEntregaNaConfirmacaoCanhoto ?? false,
                UtilizarLocalidadeTomadorNFSManual = configuracaoGeral?.UtilizarLocalidadeTomadorNFSManual ?? false,
                NaoGerarOcorrenciaCTeImportadosEmailEmbarcador = configuracaoGeral?.NaoGerarOcorrenciaCTeImportadosEmailEmbarcador ?? false,
                ProcessarXMLNotasFiscaisAssincrono = configuracaoGeral?.ProcessarXMLNotasFiscaisAssincrono ?? false,
                NaoPermitirCancelamentoNFSManualSeHouverIntegracao = configuracaoGeral?.NaoPermitirCancelamentoNFSManualSeHouverIntegracao ?? false,
                RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodo = configuracaoGeral?.RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodo ?? false,
                RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodoDias = configuracaoGeral?.RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodoDias ?? 0,
                PermitirImpressaoDAMDFEContingencia = configuracaoGeral?.PermitirImpressaoDAMDFEContingencia ?? false,
                HabilitarFuncionalidadesProjetoGollum = configuracaoGeral?.HabilitarFuncionalidadesProjetoGollum ?? false,
                EnviarCTeApenasParaTomador = configuracaoGeral?.EnviarCTeApenasParaTomador ?? false,
                PermiteRealizarConsultaVPUtilizandoModeloVeicularCarga = configuracaoGeral?.PermiteRealizarConsultaVPUtilizandoModeloVeicularCarga ?? false,
                PermiteSelecionarPlacaPorTipoVeiculoTransbordo = configuracaoGeral?.PermiteSelecionarPlacaPorTipoVeiculoTransbordo ?? false
            };
        }

        private dynamic ObterConfiguracaoTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repositorioConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repositorioConfiguracaoTransportador.BuscarConfiguracaoPadrao();

            return new
            {
                configuracaoTransportador.PermitirCadastrarTransportadorInformacoesMinimas,
                configuracaoTransportador.AtivarControleCarregamentoNavio,
                configuracaoTransportador.ExigirRetencaoISSQuandoMunicipioPrestacaoForDiferenteTransportador,
                configuracaoTransportador.PermitirTransportadorRetornarEtapaNFe,
                configuracaoTransportador.EnviarEmailDocumentoRejeitadoAuditoriaFrete,
                configuracaoTransportador.NaoAtualizarNomeFantasiaClienteAlterarDadosTransportador,
                configuracaoTransportador.PermitirInformarEmpresaFavorecidaNosDadosBancarios,
                configuracaoTransportador.ExisteTransportadorPadraoContratacao,
                configuracaoTransportador.NaoGerarAutomaticamenteUsuarioAcessoPortalTransportador,
                configuracaoTransportador.NaoHabilitarDetalhesCarga,
                configuracaoTransportador.NotificarTransportadorProcessoShareRotas,
                configuracaoTransportador.HabilitarSpotCargaAposLimiteHoras,
                TransportadorPadraoContratacao = new
                {
                    Codigo = configuracaoTransportador.TransportadorPadraoContratacao?.Codigo ?? 0,
                    Descricao = configuracaoTransportador.TransportadorPadraoContratacao?.Descricao ?? string.Empty
                }
            };

        }

        private dynamic ObterConfiguracaoArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo ConfiguracaoArquivo = repConfiguracaoArquivo.BuscarPrimeiroRegistro();

            return new
            {
                arquivo_CaminhoRelatorios = ConfiguracaoArquivo.CaminhoRelatorios,
                arquivo_CaminhoTempArquivosImportacao = ConfiguracaoArquivo.CaminhoTempArquivosImportacao,
                arquivo_CaminhoCanhotos = ConfiguracaoArquivo.CaminhoCanhotos,
                arquivo_CaminhoCanhotosAvulsos = ConfiguracaoArquivo.CaminhoCanhotosAvulsos,
                arquivo_CaminhoXMLNotaFiscalComprovanteEntrega = ConfiguracaoArquivo.CaminhoXMLNotaFiscalComprovanteEntrega,
                arquivo_CaminhoArquivosIntegracao = ConfiguracaoArquivo.CaminhoArquivosIntegracao,
                arquivo_CaminhoRelatoriosEmbarcador = ConfiguracaoArquivo.CaminhoRelatoriosEmbarcador,
                arquivo_CaminhoLogoEmbarcador = ConfiguracaoArquivo.CaminhoLogoEmbarcador,
                arquivo_CaminhoDocumentosFiscaisEmbarcador = ConfiguracaoArquivo.CaminhoDocumentosFiscaisEmbarcador,
                arquivo_Anexos = ConfiguracaoArquivo.Anexos,
                arquivo_CaminhoGeradorRelatorios = ConfiguracaoArquivo.CaminhoGeradorRelatorios,
                arquivo_CaminhoArquivosEmpresas = ConfiguracaoArquivo.CaminhoArquivosEmpresas,
                arquivo_CaminhoRelatoriosCrystal = ConfiguracaoArquivo.CaminhoRelatoriosCrystal,
                arquivo_CaminhoRetornoXMLIntegrador = ConfiguracaoArquivo.CaminhoRetornoXMLIntegrador,
                arquivo_CaminhoArquivos = ConfiguracaoArquivo.CaminhoArquivos,
                arquivo_CaminhoArquivosIntegracaoEDI = ConfiguracaoArquivo.CaminhoArquivosIntegracaoEDI,
                arquivo_CaminhoArquivosImportacaoBoleto = ConfiguracaoArquivo.CaminhoArquivosImportacaoBoleto,
                arquivo_CaminhoOcorrencias = ConfiguracaoArquivo.CaminhoOcorrencias,
                arquivo_CaminhoOcorrenciasMobiles = ConfiguracaoArquivo.CaminhoOcorrenciasMobiles,
                arquivo_CaminhoArquivosImportacaoXMLNotaFiscal = ConfiguracaoArquivo.CaminhoArquivosImportacaoXMLNotaFiscal,
                arquivo_CaminhoDestinoXML = ConfiguracaoArquivo.CaminhoDestinoXML,
                arquivo_CaminhoCanhotosAntigos = ConfiguracaoArquivo.CaminhoCanhotosAntigos,
                arquivo_CaminhoRaiz = ConfiguracaoArquivo.CaminhoRaiz,
                arquivo_CaminhoGuia = ConfiguracaoArquivo.CaminhoGuia,
                arquivo_CaminhoDanfeSMS = ConfiguracaoArquivo.CaminhoDanfeSMS,
                arquivo_CaminhoRaizFTP = ConfiguracaoArquivo.CaminhoRaizFTP
            };
        }

        private dynamic ObterConfiguracaoAmbiente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente ConfiguracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();

            return new
            {
                ambiente_AmbienteProducao = ConfiguracaoAmbiente.AmbienteProducao,
                ambiente_AmbienteSeguro = ConfiguracaoAmbiente.AmbienteSeguro,
                ambiente_RecalcularICMSNaEmissaoCTe = ConfiguracaoAmbiente.RecalcularICMSNaEmissaoCTe,
                ambiente_AplicarValorICMSNoComplemento = ConfiguracaoAmbiente.AplicarValorICMSNoComplemento,
                ambiente_AdicionarCTesFilaConsulta = ConfiguracaoAmbiente.AdicionarCTesFilaConsulta,
                ambiente_NaoCalcularDIFALParaCSTNaoTributavel = ConfiguracaoAmbiente.NaoCalcularDIFALParaCSTNaoTributavel,
                ambiente_NaoUtilizarColetaNaBuscaRotaFrete = ConfiguracaoAmbiente.NaoUtilizarColetaNaBuscaRotaFrete,
                ambiente_OcultarConteudoColog = ConfiguracaoAmbiente.OcultarConteudoColog,
                ambiente_ConsultarPeloCustoDaRota = ConfiguracaoAmbiente.ConsultarPeloCustoDaRota,
                ambiente_CalcularHorarioDoCarregamento = ConfiguracaoAmbiente.CalcularHorarioDoCarregamento,
                ambiente_EnviarTodasNotificacoesPorEmail = ConfiguracaoAmbiente.EnviarTodasNotificacoesPorEmail,
                ambiente_CalcularFreteFechamento = ConfiguracaoAmbiente.CalcularFreteFechamento,
                ambiente_GerarDocumentoFechamento = ConfiguracaoAmbiente.GerarDocumentoFechamento,
                ambiente_NovoLayoutPortalFornecedor = ConfiguracaoAmbiente.NovoLayoutPortalFornecedor,
                ambiente_NovoLayoutCabotagem = ConfiguracaoAmbiente.NovoLayoutCabotagem,
                ambiente_UtilizarIntegracaoSaintGobainNova = ConfiguracaoAmbiente.UtilizarIntegracaoSaintGobainNova,
                ambiente_FiltrarCargasPorProprietario = ConfiguracaoAmbiente.FiltrarCargasPorProprietario,
                ambiente_CargaControleEntrega_Habilitar_ImportacaoCargaFluvial = ConfiguracaoAmbiente.CargaControleEntrega_Habilitar_ImportacaoCargaFluvial,
                ambiente_FTPPassivo = ConfiguracaoAmbiente.FTPPassivo,
                ambiente_UtilizaSFTP = ConfiguracaoAmbiente.UtilizaSFTP,
                ambiente_GerarNotFisPorNota = ConfiguracaoAmbiente.GerarNotFisPorNota,
                ambiente_UtilizarMetodoImportacaoTabelaFretePorServico = ConfiguracaoAmbiente.UtilizarMetodoImportacaoTabelaFretePorServico,
                ambiente_UtilizarLayoutImportacaoTabelaFreteGPA = ConfiguracaoAmbiente.UtilizarLayoutImportacaoTabelaFreteGPA,
                ambiente_ExibirSituacaoIntegracaoXMLGPA = ConfiguracaoAmbiente.ExibirSituacaoIntegracaoXMLGPA,
                ambiente_ProcessarCTeMultiCTe = ConfiguracaoAmbiente.ProcessarCTeMultiCTe,
                ambiente_NaoUtilizarCNPJTransportador = ConfiguracaoAmbiente.NaoUtilizarCNPJTransportador,
                ambiente_BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe = ConfiguracaoAmbiente.BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe,
                ambiente_SempreUsarAtividadeCliente = ConfiguracaoAmbiente.SempreUsarAtividadeCliente,
                ambiente_AtualizarFantasiaClienteIntegracaoCTe = ConfiguracaoAmbiente.AtualizarFantasiaClienteIntegracaoCTe,
                ambiente_CadastrarMotoristaIntegracaoCTe = ConfiguracaoAmbiente.CadastrarMotoristaIntegracaoCTe,
                ambiente_CTeUtilizaProprietarioCadastro = ConfiguracaoAmbiente.CTeUtilizaProprietarioCadastro,
                ambiente_CTeCarregarVinculosVeiculosCadastro = ConfiguracaoAmbiente.CTeCarregarVinculosVeiculosCadastro,
                ambiente_CTeAtualizaTipoVeiculo = ConfiguracaoAmbiente.CTeAtualizaTipoVeiculo,
                ambiente_NaoAtualizarCadastroVeiculo = ConfiguracaoAmbiente.NaoAtualizarCadastroVeiculo,
                ambiente_AgruparQuantidadesImportacaoCTe = ConfiguracaoAmbiente.AgruparQuantidadesImportacaoCTe,
                ambiente_EncerraMDFeAutomatico = ConfiguracaoAmbiente.EncerraMDFeAutomatico,
                ambiente_EnviaContingenciaMDFeAutomatico = ConfiguracaoAmbiente.EnviaContingenciaMDFeAutomatico,
                ambiente_EnviarCertificadoOracle = ConfiguracaoAmbiente.EnviarCertificadoOracle,
                ambiente_EnviarCertificadoKeyVault = ConfiguracaoAmbiente.EnviarCertificadoKeyVault,
                ambiente_PermitirInformarCapacidadeMaximaParaUploadArquivos = ConfiguracaoAmbiente.PermitirInformarCapacidadeMaximaParaUploadArquivos,
                ambiente_DesabilitarPopUpsDeNotificacao = ConfiguracaoAmbiente.DesabilitarPopUpsDeNotificacao,
                ambiente_Codigo = ConfiguracaoAmbiente.Codigo,
                ambiente_IdentificacaoAmbiente = ConfiguracaoAmbiente.IdentificacaoAmbiente,
                ambiente_CodigoLocalidadeNaoCadastrada = ConfiguracaoAmbiente.CodigoLocalidadeNaoCadastrada,
                ambiente_CodificacaoEDI = ConfiguracaoAmbiente.CodificacaoEDI,
                ambiente_LinkCotacaoCompra = ConfiguracaoAmbiente.LinkCotacaoCompra,
                ambiente_LogoPersonalizadaFornecedor = ConfiguracaoAmbiente.LogoPersonalizadaFornecedor,
                ambiente_LayoutPersonalizadoFornecedor = ConfiguracaoAmbiente.LayoutPersonalizadoFornecedor,
                ambiente_ConcessionariasComDescontos = ConfiguracaoAmbiente.ConcessionariasComDescontos,
                ambiente_PercentualDescontoConcessionarias = ConfiguracaoAmbiente.PercentualDescontoConcessionarias,
                ambiente_PlacaPadraoConsultaValorPedagio = ConfiguracaoAmbiente.PlacaPadraoConsultaValorPedagio,
                ambiente_APIOCRLink = ConfiguracaoAmbiente.APIOCRLink,
                ambiente_APIOCRKey = ConfiguracaoAmbiente.APIOCRKey,
                ambiente_QuantidadeSelecaoAgrupamentoCargaAutomatico = ConfiguracaoAmbiente.QuantidadeSelecaoAgrupamentoCargaAutomatico,
                ambiente_QuantidadeCargasAgrupamentoCargaAutomatico = ConfiguracaoAmbiente.QuantidadeCargasAgrupamentoCargaAutomatico,
                ambiente_HorarioExecucaoThreadDiaria = ConfiguracaoAmbiente.HorarioExecucaoThreadDiaria,
                ambiente_FornecedorTMS = ConfiguracaoAmbiente.FornecedorTMS,
                ambiente_TipoArmazenamento = ConfiguracaoAmbiente.TipoArmazenamento,
                ambiente_TipoArmazenamentoLeitorOCR = ConfiguracaoAmbiente.TipoArmazenamentoLeitorOCR,
                ambiente_EnderecoFTP = ConfiguracaoAmbiente.EnderecoFTP,
                ambiente_UsuarioFTP = ConfiguracaoAmbiente.UsuarioFTP,
                ambiente_SenhaFTP = ConfiguracaoAmbiente.SenhaFTP,
                ambiente_PortaFTP = ConfiguracaoAmbiente.PortaFTP,
                ambiente_PrefixosFTP = ConfiguracaoAmbiente.PrefixosFTP,
                ambiente_EmailsFTP = ConfiguracaoAmbiente.EmailsFTP,
                ambiente_CodigoEmpresaMultisoftware = ConfiguracaoAmbiente.CodigoEmpresaMultisoftware,
                ambiente_MinutosParaConsultaNatura = ConfiguracaoAmbiente.MinutosParaConsultaNatura,
                ambiente_FiliaisNatura = ConfiguracaoAmbiente.FiliaisNatura,
                ambiente_WebServiceConsultaCTe = ConfiguracaoAmbiente.WebServiceConsultaCTe,
                ambiente_LimparMotoristaIntegracaoVeiculo = ConfiguracaoAmbiente.LimparMotoristaIntegracaoVeiculo,
                ambiente_LoginAD = ConfiguracaoAmbiente.LoginAD,
                ambiente_RegerarDACTEOracle = ConfiguracaoAmbiente.RegerarDACTEOracle,
                ambiente_ReenviarErroIntegracaoCTe = ConfiguracaoAmbiente.ReenviarErroIntegracaoCTe,
                ambiente_AtualizarTipoEmpresa = ConfiguracaoAmbiente.AtualizarTipoEmpresa,
                ambiente_ValidarNFeJaImportada = ConfiguracaoAmbiente.ValidarNFeJaImportada,
                ambiente_UtilizaOptanteSimplesNacionalDaIntegracao = ConfiguracaoAmbiente.UtilizaOptanteSimplesNacionalDaIntegracao,
                ambiente_ReenviarErroIntegracaoMDFe = ConfiguracaoAmbiente.ReenviarErroIntegracaoMDFe,
                ambiente_EncerraMDFeAutomaticoComMesmaData = ConfiguracaoAmbiente.EncerraMDFeAutomaticoComMesmaData,
                ambiente_EncerraMDFeAntesDaEmissao = ConfiguracaoAmbiente.EncerraMDFeAntesDaEmissao,
                ambiente_EncerraMDFeAutomaticoOutrosSistemas = ConfiguracaoAmbiente.EncerraMDFeAutomaticoOutrosSistemas,
                ambiente_EnviarEmailMDFeClientes = ConfiguracaoAmbiente.EnviarEmailMDFeClientes,
                ambiente_UtilizarDocaDoComplementoFilial = ConfiguracaoAmbiente.UtilizarDocaDoComplementoFilial,
                ambiente_RetornarModeloVeiculo = ConfiguracaoAmbiente.RetornarModeloVeiculo,
                ambiente_MDFeUtilizaDadosVeiculoCadastro = ConfiguracaoAmbiente.MDFeUtilizaDadosVeiculoCadastro,
                ambiente_MDFeUtilizaVeiculoReboqueComoTracao = ConfiguracaoAmbiente.MDFeUtilizaVeiculoReboqueComoTracao,
                ambiente_GerarCTeDasNFSeAutorizadas = ConfiguracaoAmbiente.GerarCTeDasNFSeAutorizadas,
                ambiente_IncluirISSNFSeLocalidadeTomadorDiferentePrestador = ConfiguracaoAmbiente.IncluirISSNFSeLocalidadeTomadorDiferentePrestador,
                ambiente_IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples = ConfiguracaoAmbiente.IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples,
                ambiente_AtualizarValorFrete_AtualizarICMS = ConfiguracaoAmbiente.AtualizarValorFrete_AtualizarICMS,
                ambiente_ConsultarDuplicidadeOracle = ConfiguracaoAmbiente.ConsultarDuplicidadeOracle,
                ambiente_EnviarIntegracaoMagalogNoRetorno = ConfiguracaoAmbiente.EnviarIntegracaoMagalogNoRetorno,
                ambiente_EnviarIntegracaoErroMDFeMagalog = ConfiguracaoAmbiente.EnviarIntegracaoErroMDFeMagalog,
                ambiente_EmpresasUsuariosMultiCTe = ConfiguracaoAmbiente.EmpresasUsuariosMultiCTe,
                ambiente_PesoMaximoIntegracaoCarga = ConfiguracaoAmbiente.PesoMaximoIntegracaoCarga,
                ambiente_CapacidadeMaximaParaUploadArquivos = ConfiguracaoAmbiente.CapacidadeMaximaParaUploadArquivos,
                ambiente_TipoProtocolo = ConfiguracaoAmbiente.TipoProtocolo,
            };
        }

        private dynamic ObterConfiguracaoRoteirizacao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

            return new
            {
                configuracaoRoteirizacao.OrdenarLocalidades,
                configuracaoRoteirizacao.NaoCalcularTempoDeViagemAutomatico,
                configuracaoRoteirizacao.ColetasSempreInicioRotaOrdenadaCliente,
                configuracaoRoteirizacao.CadastrarNovaRotaDeveSerParaTipoOperacaoCarga,
                configuracaoRoteirizacao.NumeroDiasParaConsultaPracaPedagio,
                configuracaoRoteirizacao.SempreUtilizarRotaParaBuscarPracasPedagio,
                configuracaoRoteirizacao.IgnorarOutroEnderecoPedidoComRecebedor
            };
        }

        private dynamic ObterConfiguracaoFilaCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento repositorioConfiguracaoFilaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento configuracaoFilaCarregamento = repositorioConfiguracaoFilaCarregamento.BuscarPrimeiroRegistro();

            return new
            {
                DiasFiltrarDataProgramada = (configuracaoFilaCarregamento.DiasFiltrarDataProgramada > 0) ? configuracaoFilaCarregamento.DiasFiltrarDataProgramada.ToString("n0") : "",
                NaoPermitirAdicionarVeiculoEmMaisDeUmaFilaCarregamentoSimultaneamente = configuracaoFilaCarregamento?.NaoPermitirAdicionarVeiculoEmMaisDeUmaFilaCarregamentoSimultaneamente ?? false,
                InformarAreaCDAdicionarVeiculo = configuracaoFilaCarregamento?.InformarAreaCDAdicionarVeiculo ?? false,
                PermiteAvancarPrimeiraEtapaCargaAoAlocarDadosTransportePelaFilaCarregamento = configuracaoFilaCarregamento?.PermiteAvancarPrimeiraEtapaCargaAoAlocarDadosTransportePelaFilaCarregamento ?? false,
                AtualizarFilaCarregamentoAoAlterarDadosTransporteNaCarga = configuracaoFilaCarregamento?.AtualizarFilaCarregamentoAoAlterarDadosTransporteNaCarga ?? false
            };

        }

        private dynamic ObterConfiguracaoPortalMultiClifor(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor repositorioConfiguracaoPortalMultiClifor = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor configuracaoPortalMultiClifor = repositorioConfiguracaoPortalMultiClifor.BuscarConfiguracaoPadrao();

            return new
            {
                MostrarNoAcompanhamentoDePedidosDeslocamentoVazio = configuracaoPortalMultiClifor.MostrarNoAcompanhamentoDePedidosDeslocamentoVazio,
                FiltrarPedidosPorRemetenteRetiradaProduto = configuracaoPortalMultiClifor.FiltrarPedidosPorRemetenteRetiradaProduto,
                HabilitarAcessoTodosClientes = configuracaoPortalMultiClifor.HabilitarAcessoTodosClientes,
                SenhaPadraoAcessoPortal = configuracaoPortalMultiClifor.SenhaPadraoAcessoPortal,
                configuracaoPortalMultiClifor.CodigoReportMenuBI,
                DesabilitarIconeNotificacao = configuracaoPortalMultiClifor?.DesabilitarIconeNotificacao ?? false,
                DesabilitarFiltrosBI = configuracaoPortalMultiClifor?.DesabilitarFiltrosBI ?? false
            };
        }

        private dynamic ObterConfiguracaoPortalFluxoPatio(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio repositorioConfiguracaoFluxoPatio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio configuracaoFluxoPatio = repositorioConfiguracaoFluxoPatio.BuscarConfiguracaoPadrao();

            return new
            {
                config.InformarDadosChegadaVeiculoNoFluxoPatio,
                config.UtilizarControleVeiculoEmPatio,
                config.FluxoDePatioComoMonitoramento,
                config.PermitirAdicionarCargaFluxoPatio,
                config.GerarFluxoPatioAoFecharCarga,
                config.GerarFluxoPatioPorCargaAgrupada,
                config.GerarFluxoPatioDestino,
                config.PermitirAdicionarAnexosCheckListGestaoPatio,
                configuracaoFluxoPatio.TipoComprovanteSaida,
                configuracaoFluxoPatio.PermitirBaixarRomaneioNaEtapaFimCarregamento,
                configuracaoFluxoPatio.ValidarPesoCargaComPesagemVeiculo,
                configuracaoFluxoPatio.PermiteAlocarVeiculoSemConjuntoCarga,
                configuracaoFluxoPatio.RegistrarPosicaoVeiculoSubareaAoReceberEvento
            };
        }

        private dynamic ObterConfiguracaoPortalNFSeManual(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoNFSeManual repositorioConfiguracaoNFSeManual = new Repositorio.Embarcador.Configuracoes.ConfiguracaoNFSeManual(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoNFSeManual configuracaoNFSeManual = repositorioConfiguracaoNFSeManual.BuscarConfiguracaoPadrao();

            return new
            {
                configuracaoNFSeManual.UtilizarNumeroSerieInformadoTelaQuandoEmitidoModeloDocumentoNaoFiscal,
                configuracaoNFSeManual.ValidarLocalidadePrestacaoTransportadorConfiguracaoNFSe,
                configuracaoNFSeManual.ValidarExistenciaParaInserirNFSe
            };
        }

        private dynamic ObterConfiguracaoComissaoMotorista(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista repositorioConfiguracaoComissaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista configuracaoComissaoMotorista = repositorioConfiguracaoComissaoMotorista.BuscarConfiguracaoPadrao();

            return new
            {
                PercentualComissaoMotorista = configuracaoComissaoMotorista.Percentual,
                PercentualBasecalculoComissaoMotorista = configuracaoComissaoMotorista.PercentualDaBaseDeCalculo,
                DataBaseComissaoMotorista = configuracaoComissaoMotorista.DataBase,
                UtilizaControlePercentualExecucao = configuracaoComissaoMotorista.UtilizaControlePercentualExecucao,
                BloquearAlteracaoValorFreteLiquido = configuracaoComissaoMotorista.BloquearAlteracaoValorFreteLiquido
            };
        }

        private dynamic ObterConfiguracaoEnvioEmailCobranca(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca repositorioConfiguracaoEnvioEmailCobranca = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca configuracaoEnvioEmailCobranca = repositorioConfiguracaoEnvioEmailCobranca.BuscarConfiguracaoPadrao();

            return new
            {
                AvisoVencimetoEnvarEmail = configuracaoEnvioEmailCobranca.AvisoVencimetoEnvarEmail,
                AvisoVencimetoQunatidadeDias = configuracaoEnvioEmailCobranca.AvisoVencimetoQunatidadeDias,
                AvisoVencimetoEnviarDiariamente = configuracaoEnvioEmailCobranca.AvisoVencimetoEnviarDiariamente,
                AvisoVencimetoAssunto = configuracaoEnvioEmailCobranca.AvisoVencimetoAssunto,
                AvisoVencimetoMensagem = configuracaoEnvioEmailCobranca.AvisoVencimetoMensagem,
                CobrancaEnvarEmail = configuracaoEnvioEmailCobranca.CobrancaEnvarEmail,
                CobrancaQunatidadeDias = configuracaoEnvioEmailCobranca.CobrancaQunatidadeDias,
                CobrancaAssunto = configuracaoEnvioEmailCobranca.CobrancaAssunto,
                CobrancaMensagem = configuracaoEnvioEmailCobranca.CobrancaMensagem,

            };
        }

        private dynamic ObterConfiguracaoInfracoes(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoInfracoes repConfiguracaoInfracoes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoInfracoes(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoInfracoes configuracaoInfracoes = repConfiguracaoInfracoes.BuscarConfiguracaoPadrao();

            return new
            {
                FormulaInfracaoPadrao = configuracaoInfracoes?.FormulaInfracaoPadrao ?? "",
            };
        }

        private dynamic ObterConfiguracaoBidding(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = repositorioConfiguracaoBidding.BuscarConfiguracaoPadrao();

            return new
            {
                CalcularKMMedioRotaPorOrigemDestino = configuracaoBidding?.CalcularKMMedioRotaPorOrigemDestino ?? false,
                PermiteAdicionarRotaSemInformarKMMedio = configuracaoBidding?.PermiteAdicionarRotaSemInformarKMMedio ?? false,
                PermiteSelecionarMaisDeUmaOfertaPorBidding = configuracaoBidding?.PermiteSelecionarMaisDeUmaOfertaPorBidding ?? false,
                PermiteRemoverObrigatoriedadeDatas = configuracaoBidding?.PermiteRemoverObrigatoriedadeDatas ?? false,
                TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding = configuracaoBidding?.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding ?? false,
                PermiteOfertarQuandoAceitacaoIndForMenorCemPorcento = configuracaoBidding?.PermiteOfertarQuandoAceitacaoIndForMenorCemPorcento ?? false,
                InformePorcentagemAceitacaoInd = configuracaoBidding?.InformePorcentagemAceitacaoInd ?? 0,
            };
        }

        private dynamic ObterConfiguracaoAbastecimento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos repositorioConfiguracaoAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAbastecimentos configuracaoAbastecimento = repositorioConfiguracaoAbastecimento.BuscarConfiguracaoPadrao();

            return new
            {
                GerarRequisicaoAutomaticaParaVeiculosVinculados = configuracaoAbastecimento?.GerarRequisicaoAutomaticaParaVeiculosVinculados ?? false,
                MotivoCompraAbastecimento = new { Codigo = configuracaoAbastecimento?.MotivoCompraAbastecimento?.Codigo ?? 0, Descricao = configuracaoAbastecimento?.MotivoCompraAbastecimento?.Descricao ?? "" },
                UtilizarCustoMedioParaLancamentoAbastecimentos = configuracaoAbastecimento?.UtilizarCustoMedioParaLancamentoAbastecimentos ?? false,
            };
        }

        private dynamic ObterConfiguracaoPessoa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa repositorioConfiguracaoPessoa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa configuracaoPessoa = repositorioConfiguracaoPessoa.BuscarConfiguracaoPadrao();

            return new
            {
                PermitirCadastroDeTelefoneInternacional = configuracaoPessoa?.PermitirCadastroDeTelefoneInternacional ?? false,
                ExigeQueSuasEntregasSejamAgendadas = configuracaoPessoa?.ExigeQueSuasEntregasSejamAgendadas ?? false,
                NaoEnviarXMLCTEPorEmailParaTipoServico = configuracaoPessoa?.NaoEnviarXMLCTEPorEmailParaTipoServico ?? false,
                TipoServicoCTeEmail = configuracaoPessoa?.TiposServicosCTe ?? new List<Dominio.Enumeradores.TipoServico>(),
                NaoExigirTrocaDeSenhaCasoCadastroPorIntegracao = configuracaoPessoa?.NaoExigirTrocaDeSenhaCasoCadastroPorIntegracao ?? false,
            };
        }

        private dynamic ObterConfiguracaoRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio repositorioConfiguracaoRelatorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio = repositorioConfiguracaoRelatorio.BuscarConfiguracaoPadrao();

            return new
            {
                config.QuantidadeMaximaDiasRelatorios,
                config.QuantidadeMaximaRegistrosRelatorios,
                config.UtilizarDadosCargaRelatorioPedido,
                config.HabilitarHoraFiltroDataInicialFinalRelatorioCargas,
                config.UtilizarExportacaoRelatorioCSV,
                config.ExportarCNPJEChaveDeAcessoFormatado,
                config.InformacaoAdicionalMotoristaOrdemColeta,
                config.NaoExibirValorFreteCTeComplementarRelatorioCTe,
                ExibirTodasCargasNoRelatorioDeValePedagio = configuracaoRelatorio?.ExibirTodasCargasNoRelatorioDeValePedagio ?? false,
                RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes = configuracaoRelatorio?.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes ?? false

            };
        }

        private dynamic ObterConfiguracaoMongo(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMongo repositorioConfiguracaoMongo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMongo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMongo configuracaoMongo = repositorioConfiguracaoMongo.BuscarPrimeiroRegistro();

            return new
            {
                BancoMongo = configuracaoMongo.Banco,
                ServidorMongo = configuracaoMongo.Servidor,
                PortaMongo = configuracaoMongo.Porta,
                UsaTlsMongo = configuracaoMongo.UsaTls,
                UtilizaCosmosDbMongo = configuracaoMongo.UtilizaCosmosDb,
                TimeoutMongo = configuracaoMongo.Timeout,
                UsuarioHangfireMongo = configuracaoMongo.UsuarioHangfire,
                SenhaHangfireMongo = configuracaoMongo.SenhaHangfire,
                UsuarioMongo = configuracaoMongo.Usuario,
                SenhaMongo = configuracaoMongo.Senha
            };
        }

        private dynamic ObterConfiguracaoSSo(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO repositorioConfiguracaoSSo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO configuracaoSSo = repositorioConfiguracaoSSo.BuscarPrimeiroRegistro();

            return new
            {
                AtivoSSo = configuracaoSSo?.Ativo ?? false,
                ClientIdSSo = configuracaoSSo?.ClientId ?? string.Empty,
                ClientSecretSSo = configuracaoSSo?.ClientSecret ?? string.Empty,
                DisplaySSo = configuracaoSSo?.Display ?? string.Empty,
                TipoSSo = configuracaoSSo?.TipoSSo ?? Dominio.Enumeradores.TipoSso.OAuth2,
                UrlAccessTokenSSo = configuracaoSSo?.UrlAccessToken ?? string.Empty,
                UrlAutenticacaoSSo = configuracaoSSo?.UrlAutenticacao ?? string.Empty,
                UrlRefreshTokenSSo = configuracaoSSo?.UrlRefreshToken ?? string.Empty,
                UrlRevokeTokenSSo = configuracaoSSo?.UrlRevokeToken ?? string.Empty,
                UrlDominioSSo = configuracaoSSo?.UrlDominio ?? string.Empty
            };
        }

        private dynamic ObterConfiguracaoMercosul(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMercosul repositorioConfiguracaoMercosul = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMercosul(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMercosul configuracaoMercosul = repositorioConfiguracaoMercosul.BuscarPrimeiroRegistro();

            return new
            {

                UtilizarCRTAverbacao = configuracaoMercosul?.UtilizarCRTAverbacao ?? false,
                UtilizarMesmoNumeroCRTCancelamentos = configuracaoMercosul?.UtilizarMesmoNumeroCRTCancelamentos ?? false,
                UtilizarMesmoNumeroMICDTACancelamentos = configuracaoMercosul?.UtilizarMesmoNumeroMICDTACancelamentos ?? false,

            };
        }
        private dynamic ObterConfiguracaoCotacao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCotacao repositorioConfiguracaoCotacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCotacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCotacao configuracaoCotacao = repositorioConfiguracaoCotacao.BuscarPrimeiroRegistro();

            return new
            {

                GravarNumeroCotacaoObservacaoInternaAoCriarPedido = configuracaoCotacao?.GravarNumeroCotacaoObservacaoInternaAoCriarPedido ?? false,

            };
        }

        private dynamic ObterConfiguracaoAgendamentoEntrega(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega repositorioConfiguracaoAgendamentoEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega configuracaoAgendamentoEntrega = repositorioConfiguracaoAgendamentoEntrega.BuscarPrimeiroRegistro();

            return new
            {
                configuracaoAgendamentoEntrega.VisualizarTelaDeAgendamentoPorEntrega,
            };
        }

        private dynamic ObterConfiguracaoAtendimentoAutomaticoDivergenciaValorFreteCTeEmitidosEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAtendimentoAutomatico repositorioConfiguracaoAtendimentoAutomatico = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAtendimentoAutomatico(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAtendimentoAutomatico configuracaoAtendimentoAutomatico = repositorioConfiguracaoAtendimentoAutomatico.BuscarPrimeiroRegistro();

            return new
            {
                configuracaoAtendimentoAutomatico.GerarAtendimentoDivergenciaValorTabelaCTeEmitidoEmbarcador,
                MotivoChamado = new { Codigo = configuracaoAtendimentoAutomatico.MotivoChamado?.Codigo ?? 0, Descricao = configuracaoAtendimentoAutomatico.MotivoChamado?.Descricao ?? "" },
            };
        }

        private dynamic ObterConfiguracaoAPIDeConexaoComGeradorExcelKMM(Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM repositorioConfiguracaoAPIConexaoGeradorExcelKMM = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM configuracaoAPIConexaoGeradorExcelKMM = repositorioConfiguracaoAPIConexaoGeradorExcelKMM.BuscarPrimeiroRegistro();

            return new
            {
                UsarApiDeConexaoComGeradorExcelKMM = configuracaoAPIConexaoGeradorExcelKMM?.UsarApiDeConexaoComGeradorExcelKMM ?? false,
            };

        }

        private dynamic ObterConfiguracaoGeralCIOT(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config)
        {
            Repositorio.Embarcador.CIOT.ConfiguracaoGeralCIOT repositorioConfiguracaoGeralCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoGeralCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralCIOT configuracaoGeralCIOT = repositorioConfiguracaoGeralCIOT.BuscarConfiguracaoPadrao();

            if (configuracaoGeralCIOT != null)
            {
                return new
                {
                    configuracaoGeralCIOT.TipoGeracaoCIOT,
                    configuracaoGeralCIOT.TipoFavorecidoCIOT,
                    configuracaoGeralCIOT.TipoQuitacaoCIOT,
                    configuracaoGeralCIOT.TipoAdiantamentoCIOT,
                };
            }
            else return new Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralCIOT();
        }

        private dynamic ObterConfiguracaoGeralTipoPagamentoCIOT(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config)
        {
            Repositorio.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT repositorioConfiguracaoGeralTipoPagamentoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT(unitOfWork);
            List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT> tiposPagamento = repositorioConfiguracaoGeralTipoPagamentoCIOT.BuscarConfiguracaoPadrao();
            return new
            {
                TiposPagamentoCIOTOperadora = (from obj in tiposPagamento
                                               select new
                                               {
                                                   obj.Codigo,
                                                   obj.TipoPagamentoCIOT,
                                                   OperadoraCIOT = obj.Operadora,
                                                   DescricaoTipoPagamentoCIOT = obj.TipoPagamentoCIOT.ObterDescricao(),
                                                   DescricaoOperadoraCIOT = obj.Operadora.ObterDescricao(),
                                               }).ToList(),
            };
        }

        private dynamic ObterConfiguracaoDownloadArquivos(Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos repositorioConfiguracaoDownloadArquivos = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos configuracaoDownloadArquivos = repositorioConfiguracaoDownloadArquivos.BuscarPrimeiroRegistro();

            return new
            {
                PermitirBaixarArquivosConembOcorenManualmente = configuracaoDownloadArquivos?.PermitirBaixarArquivosConembOcorenManualmente ?? false,
            };

        }

        private dynamic ObterConfiguracaoPaginacaoInterfaces(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces repositorioConfiguracaoPaginacaoInterfaces = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces(unitOfWork);
            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces> listaInterfacesPaginacao = repositorioConfiguracaoPaginacaoInterfaces.BuscarConfiguracaoPadrao();
            return new
            {
                GridConfiguracoesPaginacaoInterfaces = (from obj in listaInterfacesPaginacao
                                                        select new
                                                        {
                                                            obj.Codigo,
                                                            obj.Interface,
                                                            obj.Dias,
                                                            DescricaoInterface = obj.Interface.ObterDescricaoInterfaces(),
                                                        }).ToList(),
            };
        }

        #endregion Métodos Privados - Obter Objeto Configuração

        #region Métodos Privados - Salvar configurações em tabelas específicas

        private void SalvarConfiguracaoAprovacao(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao repositorioConfiguracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = repositorioConfiguracaoAprovacao.BuscarConfiguracaoPadrao();

            configuracaoAprovacao.Initialize();
            configuracaoAprovacao.PermitirDelegarParaUsuarioComTodasAlcadasRejeitadas = Request.GetBoolParam("PermitirDelegarParaUsuarioComTodasAlcadasRejeitadas");
            configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente = Request.GetBoolParam("UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente");
            configuracaoAprovacao.CriarAprovacaoCargaAoConfirmarDocumentos = Request.GetBoolParam("CriarAprovacaoCargaAoConfirmarDocumentos");

            repositorioConfiguracaoAprovacao.Atualizar(configuracaoAprovacao);
            config.SetExternalChanges(configuracaoAprovacao.GetCurrentChanges());
        }

        private void SalvarConfiguracaoAgendamentoColeta(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();

            configuracaoAgendamentoColeta.Initialize();
            configuracaoAgendamentoColeta.RemoverEtapaAgendamentoAgendamentoColeta = Request.GetBoolParam("RemoverEtapaAgendamentoAgendamentoColeta");
            configuracaoAgendamentoColeta.PermitirTransportadorCadastrarAgendamentoColeta = Request.GetBoolParam("PermitirTransportadorCadastrarAgendamentoColeta");
            configuracaoAgendamentoColeta.ConsultarSomenteTransportadoresPermitidosCadastro = Request.GetBoolParam("ConsultarSomenteTransportadoresPermitidosCadastro");
            configuracaoAgendamentoColeta.GerarAutomaticamenteSenhaPedidosAgendas = Request.GetBoolParam("GerarAutomaticamenteSenhaPedidosAgendas");
            configuracaoAgendamentoColeta.MostrarTipoDeOperacaoNoPortalMultiEmbarcadorAgendamentoColeta = Request.GetBoolParam("MostrarTipoDeOperacaoNoPortalMultiEmbarcadorAgendamentoColeta");
            configuracaoAgendamentoColeta.CalcularDataDeEntregaPorTempoDeDescargaDaRota = Request.GetBoolParam("CalcularDataDeEntregaPorTempoDeDescargaDaRota");
            configuracaoAgendamentoColeta.TempoPadraoDeDescargaMinutos = Request.GetIntParam("TempoPadraoDeDescargaMinutos");
            configuracaoAgendamentoColeta.UtilizaRazaoSocialNaVisaoDoAgendamento = Request.GetBoolParam("UtilizaRazaoSocialNaVisaoDoAgendamento");
            configuracaoAgendamentoColeta.EnviarEmailDeNotificacaoAutomaticamenteAoTransportadorDaCarga = Request.GetBoolParam("EnviarEmailDeNotificacaoAutomaticamenteAoTransportadorDaCarga");
            configuracaoAgendamentoColeta.ModeloEmail = configuracaoAgendamentoColeta.EnviarEmailDeNotificacaoAutomaticamenteAoTransportadorDaCarga ? new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail() { Codigo = Request.GetIntParam("ModeloEmailNotificacaoAutomaticaTransportador") } : null;

            repositorioConfiguracaoAgendamentoColeta.Atualizar(configuracaoAgendamentoColeta);
            config.SetExternalChanges(configuracaoAgendamentoColeta.GetCurrentChanges());
        }

        private void SalvarConfiguracaoContratoFreteTerceiro(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();

            configuracaoContratoFreteTerceiro.Initialize();
            configuracaoContratoFreteTerceiro.DiasVencimentoAdiantamento = Request.GetIntParam("DiasVencimentoAdiantamentoContratoFreteTerceiro");
            configuracaoContratoFreteTerceiro.DiasVencimentoSaldo = Request.GetIntParam("DiasVencimentoSaldoContratoFreteTerceiro");
            configuracaoContratoFreteTerceiro.Observacao = Request.GetNullableStringParam("ObservacaoContratoFreteTerceiro");
            configuracaoContratoFreteTerceiro.TextoAdicional = Request.GetNullableStringParam("TextoAdicionalContratoFreteTerceiro");
            configuracaoContratoFreteTerceiro.ReterImpostos = Request.GetBoolParam("ReterImpostosContratoFreteTerceiro");
            configuracaoContratoFreteTerceiro.RetencaoPorRaizCNPJ = Request.GetBoolParam("RetencaoPorRaizCNPJ");
            configuracaoContratoFreteTerceiro.CalcularAdiantamentoComPedagio = Request.GetBoolParam("CalcularAdiantamentoComPedagioContratoFreteTerceiro");
            configuracaoContratoFreteTerceiro.UtilizarTaxaPagamentoContratoFreteTerceiro = Request.GetBoolParam("UtilizarTaxaPagamentoContratoFreteTerceiro");
            configuracaoContratoFreteTerceiro.UtilizarNovoLayoutPagamentoAgregado = Request.GetBoolParam("UtilizarNovoLayoutPagamentoAgregado");
            configuracaoContratoFreteTerceiro.PercentualAdiantamentoFreteTerceiros = Request.GetDecimalParam("PercentualAdiantamentoFreteTerceiros");
            configuracaoContratoFreteTerceiro.ExibirPedidosImpressaoContratoFrete = Request.GetBoolParam("ExibirPedidosImpressaoContratoFrete");
            configuracaoContratoFreteTerceiro.NaoConsiderarDescontoCalculoImpostos = Request.GetBoolParam("NaoConsiderarDescontoCalculoImpostosContratoFreteTerceiro");
            configuracaoContratoFreteTerceiro.HabilitarLayoutFaturaPagamentoAgregado = Request.GetBoolParam("HabilitarLayoutFaturaPagamentoAgregado");
            configuracaoContratoFreteTerceiro.GerarCargaTerceiroApenasProvedorPedido = Request.GetBoolParam("GerarCargaTerceiroApenasProvedorPedido");
            configuracaoContratoFreteTerceiro.ObrigarAnexosContratoTransportadorFrete = Request.GetBoolParam("ObrigarAnexosContratoTransportadorFrete");
            configuracaoContratoFreteTerceiro.GerarNumeroContratoTransportadorFreteSequencial = Request.GetBoolParam("GerarNumeroContratoTransportadorFreteSequencial");
            configuracaoContratoFreteTerceiro.NaoSubtrairValePedagioDoContrato = Request.GetBoolParam("NaoSubtrairValePedagioDoContrato");
            configuracaoContratoFreteTerceiro.UtilizarFechamentoDeAgregado = Request.GetBoolParam("UtilizarFechamentoDeAgregado");
            configuracaoContratoFreteTerceiro.EmAcrescimoDescontoCiotNaoAlteraImpostos = Request.GetBoolParam("EmAcrescimoDescontoCiotNaoAlteraImpostos");
            configuracaoContratoFreteTerceiro.PermiteAlterarDadosContratoIndependenteSituacao = Request.GetBoolParam("PermiteAlterarDadosContratoIndependenteSituacao");
            configuracaoContratoFreteTerceiro.PermitirAutorizarPagamentoCIOTComCanhotosRecebidos = Request.GetBoolParam("PermitirAutorizarPagamentoCIOTComCanhotosRecebidos");
            configuracaoContratoFreteTerceiro.GerarCIOTMarcadoAoCadastrarTransportadorTerceiro = Request.GetBoolParam("GerarCIOTMarcadoAoCadastrarTransportadorTerceiro");
            configuracaoContratoFreteTerceiro.PermitirInformarPercentual100AdiantamentoCarga = Request.GetBoolParam("PermitirInformarPercentual100AdiantamentoCarga");

            repConfiguracaoContratoFreteTerceiro.Atualizar(configuracaoContratoFreteTerceiro);
            config.SetExternalChanges(configuracaoContratoFreteTerceiro.GetCurrentChanges());
        }

        private void SalvarConfiguracaoMonitoramento(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

            configuracaoMonitoramento.Initialize();
            configuracaoMonitoramento.TelaMonitoramentoFiltroFilialDaCarga = Request.GetBoolParam("TelaMonitoramentoFiltroFilialDaCarga");
            configuracaoMonitoramento.AgruparVeiculosMapaPosicaoFrota = Request.GetBoolParam("AgruparVeiculosMapaPosicaoFrota");
            configuracaoMonitoramento.EnviarAlertasMonitoramentoEmail = Request.GetBoolParam("EnviarAlertasMonitoramentoEmail");
            configuracaoMonitoramento.EmailsAlertaMonitoramento = Request.GetStringParam("EmailsAlertaMonitoramento");
            configuracaoMonitoramento.AtualizarMonitoramentoAoGerarMDFeManual = Request.GetBoolParam("AtualizarMonitoramentoAoGerarMDFeManual");
            configuracaoMonitoramento.AtualizarStatusViagemMonitoramentoAoIniciarViagem = Request.GetBoolParam("AtualizarStatusViagemMonitoramentoAoIniciarViagem");
            configuracaoMonitoramento.ManterMonitoramentosDeCargasCanceladasAoReceberNovaCarga = Request.GetBoolParam("ManterMonitoramentosDeCargasCanceladasAoReceberNovaCarga");
            configuracaoMonitoramento.NaoGerarNovoMonitoramentoCarga = Request.GetBoolParam("NaoGerarNovoMonitoramentoCarga");
            configuracaoMonitoramento.ValorMinimoMonitoramentoCritico = Request.GetDecimalParam("ValorMinimoMonitoramentoCritico");
            configuracaoMonitoramento.TelaMonitoramentoAtualizarGridAoReceberAtualizacoesOnTime = Request.GetBoolParam("TelaMonitoramentoAtualizarGridAoReceberAtualizacoesOnTime");
            configuracaoMonitoramento.IgnorarStatusViagemMonitoramentoAnterioresTransito = Request.GetBoolParam("IgnorarStatusViagemMonitoramentoAnterioresTransito");
            configuracaoMonitoramento.IdentificarEntradaEmAlvoComPosicaoUnicaIgnorandoTemposDePermanencia = Request.GetBoolParam("IdentificarEntradaEmAlvoComPosicaoUnicaIgnorandoTemposDePermanencia");
            configuracaoMonitoramento.IdentificarCarregamentoAoIniciarOuFinalizarMonitoramentosConsecutivos = Request.GetBoolParam("IdentificarCarregamentoAoIniciarOuFinalizarMonitoramentosConsecutivos");
            configuracaoMonitoramento.FinalizarMonitoramentoAoGerarTransbordoCarga = Request.GetBoolParam("FinalizarMonitoramentoAoGerarTransbordoCarga");
            configuracaoMonitoramento.GerarDadosSumarizadosDasParadasAoFinalizarOMonitoramento = Request.GetBoolParam("GerarDadosSumarizadosDasParadasAoFinalizarOMonitoramento");
            configuracaoMonitoramento.GerarMonitoramentoAoFecharCarga = Request.GetBoolParam("GerarMonitoramentoAoFecharCarga");
            configuracaoMonitoramento.ConsiderarDataAuditoriaComoDataFimDoMonitoramento = Request.GetBoolParam("ConsiderarDataAuditoriaComoDataFimDoMonitoramento");
            configuracaoMonitoramento.FrequenciaCapturaPosicoesAppTrizy = Request.GetNullableEnumParam<FrequenciaTrackingAppTrizy>("FrequenciaCapturaPosicoesAppTrizy") ?? FrequenciaTrackingAppTrizy.High;
            configuracaoMonitoramento.FinalizarAutomaticamenteAlertasDoMonitoramentoAoFinalizarViagem = Request.GetBoolParam("FinalizarAutomaticamenteAlertasDoMonitoramentoAoFinalizarViagem");
            configuracaoMonitoramento.FinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente = Request.GetBoolParam("FinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente");
            configuracaoMonitoramento.DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente = Request.GetIntParam("DiasParaFinalizarAutomaticamenteAlertasDoMonitoramentoPeriodicamente", 30);
            configuracaoMonitoramento.DistanciaMaximaRotaCurta = Request.GetIntParam("DistanciaMaximaRotaCurta");

            configuracaoMonitoramento.FinalizarAutomaticamenteMonitoramentosEmAndamento = Request.GetBoolParam("FinalizarAutomaticamenteMonitoramentosEmAndamento");
            configuracaoMonitoramento.DiasFinalizarAutomaticamenteMonitoramentoEmAndamento = Request.GetIntParam("DiasFinalizarAutomaticamenteMonitoramentoEmAndamento", 0);

            configuracaoMonitoramento.FinalizarAutomaticamenteMonitoramentosPrevisaoUltimaEntrega = Request.GetBoolParam("FinalizarAutomaticamenteMonitoramentosPrevisaoUltimaEntrega");
            configuracaoMonitoramento.DiasFinalizarMonitoramentoPrevisaoUltimaEntrega = Request.GetIntParam("DiasFinalizarMonitoramentoPrevisaoUltimaEntrega", 0);
            configuracaoMonitoramento.TempoPermitidoPermanenciaEmCarregamento = Request.GetIntParam("TempoPermitidoPermanenciaEmCarregamento", 0);
            configuracaoMonitoramento.TempoPermitidoPermanenciaNoCliente = Request.GetIntParam("TempoPermitidoPermanenciaNoCliente", 0);
            configuracaoMonitoramento.UtilizarModalAntigoDetalhesMonitoramento = Request.GetBoolParam("UtilizarModalAntigoDetalhesMonitoramento");
            configuracaoMonitoramento.HabilitarVinculoPermanenciasComHistoricoStatusViagem = Request.GetBoolParam("HabilitarVinculoPermanenciasComHistoricoStatusViagem");

            repConfiguracaoMonitoramento.Atualizar(configuracaoMonitoramento);
            config.SetExternalChanges(configuracaoMonitoramento.GetCurrentChanges());
        }

        private void SalvarConfiguracaoCanhoto(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto> repositorioCanhoto = new Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto>(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioCanhoto.BuscarPrimeiroRegistro();

            configuracaoCanhoto.Initialize();
            configuracaoCanhoto.PrazoDiasAposDataEmissao = Request.GetIntParam("PrazoDiasAposDataEmissao");
            configuracaoCanhoto.NotificarCanhotosPendentesTodosOsDias = Request.GetBoolParam("NotificarCanhotosPendentesTodosOsDias");
            configuracaoCanhoto.DisponibilizarOpcaoDeCanhotoExtraviado = Request.GetBoolParam("DisponibilizarOpcaoDeCanhotoExtraviado");
            configuracaoCanhoto.PermitirAuditarCanhotosFinalizados = Request.GetBoolParam("PermitirAuditarCanhotosFinalizados");
            configuracaoCanhoto.NaoPermitirReceberCanhotosNaoDigitalizados = Request.GetBoolParam("NaoPermitirReceberCanhotosNaoDigitalizados");
            configuracaoCanhoto.NotificarTransportadorCanhotosQueEstaoComDigitalizacaoRejeitada = Request.GetBoolParam("NotificarTransportadorCanhotosQueEstaoComDigitalizacaoRejeitada");
            configuracaoCanhoto.PrazoParaReverterDigitalizacaoAposAprovacao = Request.GetIntParam("PrazoParaReverterDigitalizacaoAposAprovacao");
            configuracaoCanhoto.TamanhoMaximoDaImagemDoCanhoto = Request.GetIntParam("TamanhoMaximoDaImagemDoCanhoto");
            configuracaoCanhoto.ValidarSituacaoDigitalizacaoCanhotosAoSumarizarDocumentoFaturamento = Request.GetBoolParam("ValidarSituacaoDigitalizacaoCanhotosAoSumarizarDocumentoFaturamento");
            configuracaoCanhoto.MensagemRodapeEmailCanhotosPendentes = Request.GetBoolParam("MensagemRodapeEmailCanhotosPendentes");
            configuracaoCanhoto.LiberarParaPagamentoAposDigitalizacaCanhoto = Request.GetBoolParam("LiberarParaPagamentoAposDigitalizacaCanhoto");
            configuracaoCanhoto.GerarCanhotoParaNotasTipoPallet = Request.GetBoolParam("GerarCanhotoParaNotasTipoPallet");
            configuracaoCanhoto.PermitirBloquearDocumentoManualmente = Request.GetBoolParam("PermitirBloquearDocumentoManualmente");
            configuracaoCanhoto.PermitirMultiplaSelecaoLancamentoLotePagamento = Request.GetBoolParam("PermitirMultiplaSelecaoLancamentoLotePagamento");
            configuracaoCanhoto.PermitirAlterarImagemCanhotoDigitalizada = Request.GetBoolParam("PermitirAlterarImagemCanhotoDigitalizada");
            configuracaoCanhoto.RejeitarCanhotosNaoValidadosPeloOCR = Request.GetBoolParam("RejeitarCanhotosNaoValidadosPeloOCR");
            configuracaoCanhoto.ExibirCanhotosSemVinculoComCarga = Request.GetBoolParam("ExibirCanhotosSemVinculoComCarga");
            configuracaoCanhoto.ExigirDataEntregaNotaClienteCanhotosReceberFisicamente = Request.GetBoolParam("ExigirDataEntregaNotaClienteCanhotosReceberFisicamente");
            configuracaoCanhoto.PermitirImportarCanhotoNFFaturada = Request.GetBoolParam("PermitirImportarCanhotoNFFaturada");
            configuracaoCanhoto.PermitirAtualizarSituacaoCanhotoPorImportacao = Request.GetBoolParam("PermitirAtualizarSituacaoCanhotoPorImportacao");
            configuracaoCanhoto.PermitirAtualizarSituacaoCanhotoAvulsoPorImportacao = Request.GetBoolParam("PermitirAtualizarSituacaoCanhotoAvulsoPorImportacao");
            configuracaoCanhoto.PermitirImportarDocumentosFiltroSemChaveNFe = Request.GetBoolParam("PermitirImportarDocumentosFiltroSemChaveNFe");
            configuracaoCanhoto.IntegrarCanhotosComValidadorIAComprovei = Request.GetBoolParam("IntegrarCanhotosComValidadorIAComprovei");
            configuracaoCanhoto.ReenviarUmaVezIntegracaoCasoRetornarFalhaNaValidacaoDoNumeroDoCanhotoEOuFormatoDoCanhoto = Request.GetBoolParam("ReenviarUmaVezIntegracaoCasoRetornarFalhaNaValidacaoDoNumeroDoCanhotoEOuFormatoDoCanhoto");
            configuracaoCanhoto.PermitirEnviarImagemParaMultiplosCanhotos = Request.GetBoolParam("PermitirEnviarImagemParaMultiplosCanhotos");
            configuracaoCanhoto.ObterNumeroNotaFiscalPorObjetoOcr = Request.GetBoolParam("ObterNumeroNotaFiscalPorObjetoOcr");
            configuracaoCanhoto.RetornarMetodoBuscarEntregasRealizadasPendentesIntegracaoSomenteCanhotoDigitalizado = Request.GetBoolParam("RetornarMetodoBuscarEntregasRealizadasPendentesIntegracaoSomenteCanhotoDigitalizado");
            configuracaoCanhoto.NaoAtualizarTelaCanhotosAposAprovacaoRejeicao = Request.GetBoolParam("NaoAtualizarTelaCanhotosAposAprovacaoRejeicao");
            configuracaoCanhoto.AprovarAutomaticamenteADigitalizacaoDosCanhotosCasoAValidacaoDaIAComproveiSejaCompleta = Request.GetBoolParam("AprovarAutomaticamenteADigitalizacaoDosCanhotosCasoAValidacaoDaIAComproveiSejaCompleta");
            configuracaoCanhoto.PermitirAprovarDigitalizacaoDeCanhotoRejeitado = Request.GetBoolParam("PermitirAprovarDigitalizacaoDeCanhotoRejeitado");
            configuracaoCanhoto.ValidarSituacaoEntregaAoEnviarImagemCanhotoManualmente = Request.GetBoolParam("ValidarSituacaoEntregaAoEnviarImagemCanhotoManualmente");
            configuracaoCanhoto.FlexibilidadeParaValidacaoNaIAComprovei = Request.GetDoubleParam("FlexibilidadeParaValidacaoNaIAComprovei");
            configuracaoCanhoto.NaoIntegrarIAComproveiCanhotosDeNotasDevolvidas = configuracaoCanhoto.IntegrarCanhotosComValidadorIAComprovei && Request.GetBoolParam("NaoIntegrarIAComproveiCanhotosDeNotasDevolvidas");
            configuracaoCanhoto.PermitirRetornarStatusCanhotoNaAPIDigitalizacao = Request.GetBoolParam("PermitirRetornarStatusCanhotoNaAPIDigitalizacao");
            configuracaoCanhoto.HabilitarFluxoAnaliseCanhotoRejeitadoIA = Request.GetBoolParam("HabilitarFluxoAnaliseCanhotoRejeitadoIA");
            configuracaoCanhoto.ValidacaoNumero = Request.GetNullableDoubleParam("ValidacaoNumero");
            configuracaoCanhoto.ValidacaoAssinatura = Request.GetNullableDoubleParam("ValidacaoAssinatura");
            configuracaoCanhoto.ValidacaoEncontrouData = Request.GetNullableDoubleParam("ValidacaoEncontrouData");
            configuracaoCanhoto.ValidacaoCanhoto = Request.GetNullableDoubleParam("ValidacaoCanhoto");
            configuracaoCanhoto.EfetuarIntegracaoApenasCanhotosDigitalizados = Request.GetBoolParam("EfetuarIntegracaoApenasCanhotosDigitalizados");
            configuracaoCanhoto.CompactarImagemCanhotoIaComproveiCasoTamanhoUltrapasseUmMB = Request.GetBoolParam("CompactarImagemCanhotoIaComproveiCasoTamanhoUltrapasseUmMB");
            configuracaoCanhoto.RetornarSomenteCanhotoComNFeEntregueEmBuscarCanhotosNotasFiscaisDigitalizados = Request.GetBoolParam("RetornarSomenteCanhotoComNFeEntregueEmBuscarCanhotosNotasFiscaisDigitalizados");

            repositorioCanhoto.Atualizar(configuracaoCanhoto);
            config.SetExternalChanges(configuracaoCanhoto.GetCurrentChanges());
        }

        private void SalvarConfiguracaoCargaEmissaoDocumento(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadrao();

            configuracaoCargaEmissaoDocumento.Initialize();
            configuracaoCargaEmissaoDocumento.ValidarDataPrevisaoEntregaPedidoMenorDataAtual = Request.GetBoolParam("ValidarDataPrevisaoEntregaPedidoMenorDataAtual");
            configuracaoCargaEmissaoDocumento.ValidarDataPrevisaoSaidaPedidoMenorDataAtual = Request.GetBoolParam("ValidarDataPrevisaoSaidaPedidoMenorDataAtual");
            configuracaoCargaEmissaoDocumento.NaoAlterarCentroResultadoMotorista = Request.GetBoolParam("NaoAlterarCentroResultadoMotorista");
            configuracaoCargaEmissaoDocumento.BloquearEmissaoTomadorSemEmail = Request.GetBoolParam("BloquearEmissaoTomadorSemEmail");
            configuracaoCargaEmissaoDocumento.ConsultarDocumentosDestinadosCarga = Request.GetBoolParam("ConsultarDocumentosDestinadosCarga");
            configuracaoCargaEmissaoDocumento.UtilizarNumeroOutroDocumento = Request.GetBoolParam("UtilizarNumeroOutroDocumento");
            configuracaoCargaEmissaoDocumento.NaoPermitirNFSComMultiplosCentrosResultado = Request.GetBoolParam("NaoPermitirNFSComMultiplosCentrosResultado");
            configuracaoCargaEmissaoDocumento.BloquearEmissaoCargaTerceirosSemValePedagio = Request.GetBoolParam("BloquearEmissaoCargaTerceirosSemValePedagio");
            configuracaoCargaEmissaoDocumento.AtivarEnvioDocumentacaoFinalizacaoCarga = Request.GetBoolParam("AtivarEnvioDocumentacaoFinalizacaoCarga");
            configuracaoCargaEmissaoDocumento.NaoComprarValePedagio = Request.GetBoolParam("NaoComprarValePedagio");
            configuracaoCargaEmissaoDocumento.NaoPermitirAcessarDocumentosAntesCargaEmTransporte = Request.GetBoolParam("NaoPermitirAcessarDocumentosAntesCargaEmTransporte");
            configuracaoCargaEmissaoDocumento.ControlarValoresComponentesCTe = Request.GetBoolParam("ControlarValoresComponentesCTe");
            configuracaoCargaEmissaoDocumento.NaoEnviarEmailDocumentoEmitidoProprietarioVeiculo = Request.GetBoolParam("NaoEnviarEmailDocumentoEmitidoProprietarioVeiculo");
            configuracaoCargaEmissaoDocumento.NaoPermitirEmissaoComMesmaOrigemEDestino = Request.GetBoolParam("NaoPermitirEmissaoComMesmaOrigemEDestino");
            configuracaoCargaEmissaoDocumento.UsaFluxoSubstituicaoFaseada = Request.GetBoolParam("UsaFluxoSubstituicaoFaseada");

            repConfiguracaoCargaEmissaoDocumento.Atualizar(configuracaoCargaEmissaoDocumento);

            config.SetExternalChanges(configuracaoCargaEmissaoDocumento.GetCurrentChanges());
        }

        private void SalvarConfiguracaoCargaCalculoFrete(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete repConfiguracaoCargaCalculoFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaCalculoFrete configuracaoCargaCalculoFrete = repConfiguracaoCargaCalculoFrete.BuscarConfiguracaoPadrao();

            configuracaoCargaCalculoFrete.Initialize();
            configuracaoCargaCalculoFrete.ValorMaximoCalculoFrete = Request.GetDecimalParam("ValorMaximoCalculoFrete");

            repConfiguracaoCargaCalculoFrete.Atualizar(configuracaoCargaCalculoFrete);
            config.SetExternalChanges(configuracaoCargaCalculoFrete.GetCurrentChanges());
        }

        private void SalvarConfiguracaoGeralCarga(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracao = repositorio.BuscarPrimeiroRegistro();

            configuracao.Initialize();
            configuracao.PermitirAlterarInformacoesAgrupamentoCarga = Request.GetBoolParam("PermitirAlterarInformacoesAgrupamentoCarga");
            configuracao.ObrigatorioOperadorResponsavelCancelamentoCarga = Request.GetBoolParam("ObrigatorioOperadorResponsavelCancelamentoCarga");
            configuracao.CalcularPautaFiscal = Request.GetBoolParam("CalcularPautaFiscal");
            configuracao.NaoPermitirRemoverUltimoPedidoCarga = Request.GetBoolParam("NaoPermitirRemoverUltimoPedidoCarga");
            configuracao.PermitirRemoverCargasAgrupamentoCarga = Request.GetBoolParam("PermitirRemoverCargasAgrupamentoCarga");
            configuracao.NaoValidarLicencaVeiculoParaCargaRedespacho = Request.GetBoolParam("NaoValidarLicencaVeiculoParaCargaRedespacho");
            configuracao.UtilizarPesoProdutoParaCalcularPesoCarga = Request.GetBoolParam("UtilizarPesoProdutoParaCalcularPesoCarga");
            configuracao.PadraoGeracaoNumeroCarga = Request.GetStringParam("PadraoGeracaoNumeroCarga");
            configuracao.AlertarTransportadorCancelamentoCarga = Request.GetBoolParam("AlertarTransportadorCancelamentoCarga");
            configuracao.TrocarFilialQuandoExpedidorForUmaFilial = Request.GetBoolParam("TrocarFilialQuandoExpedidorForUmaFilial");
            configuracao.HabilitarRelatorioDeEmbarque = Request.GetBoolParam("HabilitarRelatorioDeEmbarque");
            configuracao.ExibirMensagemAlertaPrevisaoEntregaNaMesmaData = Request.GetBoolParam("ExibirMensagemAlertaPrevisaoEntregaNaMesmaData");
            configuracao.ManterTransportadorUnicoEmCargasAgrupadas = Request.GetBoolParam("ManterTransportadorUnicoEmCargasAgrupadas");
            configuracao.LimitePesoDocumentoCarga = Request.GetDecimalParam("LimitePesoDocumentoCarga");
            configuracao.LimiteValorDocumentoCarga = Request.GetDecimalParam("LimiteValorDocumentoCarga");
            configuracao.LimiteTaraPedidosCarga = Request.GetDecimalParam("LimiteTaraPedidosCarga");
            configuracao.UtilizarProgramacaoCarga = Request.GetBoolParam("UtilizarProgramacaoCarga");
            configuracao.PrefixoParaCargasGeradasViaCarregamento = Request.GetStringParam("PrefixoParaCargasGeradasViaCarregamento");
            configuracao.PermitirAgrupamentoDeCargasOrdenavel = Request.GetBoolParam("PermitirAgrupamentoDeCargasOrdenavel");
            configuracao.PermitirGerarRegistroDeDesembarqueNoCIOT = Request.GetBoolParam("PermitirGerarRegistroDeDesembarqueNoCIOT");
            configuracao.AtualizarVinculoVeiculoMotoristaIntegracao = Request.GetBoolParam("AtualizarVinculoVeiculoMotoristaIntegracao");
            configuracao.NaoPermitirGerarRedespachoDeCargasDeRedespacho = Request.GetBoolParam("NaoPermitirGerarRedespachoDeCargasDeRedespacho");
            configuracao.AverbarMDFeSomenteEmCargasComCIOT = Request.GetBoolParam("AverbarMDFeSomenteEmCargasComCIOT");
            configuracao.ExigirConfiguracaoTerceiroParaGerarCIOTParaTodos = Request.GetBoolParam("ExigirConfiguracaoTerceiroParaGerarCIOTParaTodos");
            configuracao.PermitirCancelarDocumentosCargaPeloCancelamentoCarga = Request.GetBoolParam("PermitirCancelarDocumentosCargaPeloCancelamentoCarga");
            configuracao.HabilitarEnvioDocumentacaoCargaPorEmail = Request.GetBoolParam("HabilitarEnvioDocumentacaoCargaPorEmail");
            configuracao.SelecionarSomenteOperacoesDeRedespachoNaTelaDeRedespacho = Request.GetBoolParam("SelecionarSomenteOperacoesDeRedespachoNaTelaDeRedespacho");
            configuracao.NotificarNovaCargaAposConfirmacaoDocumentos = Request.GetBoolParam("NotificarNovaCargaAposConfirmacaoDocumentos");
            configuracao.InformarCentroResultadoNaEtapaUmDaCarga = Request.GetBoolParam("InformarCentroResultadoNaEtapaUmDaCarga");
            configuracao.UtilizaRegrasDeAprovacaoParaCancelamentoDaCarga = Request.GetBoolParam("UtilizaRegrasDeAprovacaoParaCancelamentoDaCarga");
            configuracao.ConsiderarApenasUmaVezKMParaPedidosComMesmoDestinoOrigemCarga = Request.GetBoolParam("ConsiderarApenasUmaVezKMParaPedidosComMesmoDestinoOrigemCarga");
            configuracao.PermitirAlterarEmpresaNoCTeManual = Request.GetBoolParam("PermitirAlterarEmpresaNoCTeManual");
            configuracao.ObrigarJustificativaCancelamentoCarga = Request.GetBoolParam("ObrigarJustificativaCancelamentoCarga");
            configuracao.PermitirCancelamentoDaCargaSomenteComDocumentosEmitidos = Request.GetBoolParam("PermitirCancelamentoDaCargaSomenteComDocumentosEmitidos");
            configuracao.FinalizarCargaAutomaticamenteAposEncerramentoMDFe = Request.GetBoolParam("FinalizarCargaAutomaticamenteAposEncerramentoMDFe");
            configuracao.AtualizarDataEmissaoParaDataAtualQuandoReemitirCTeRejeitado = Request.GetBoolParam("AtualizarDataEmissaoParaDataAtualQuandoReemitirCTeRejeitado");
            configuracao.RetornarPedidosInseridosManualmenteAoGerarCarga = Request.GetBoolParam("RetornarPedidosInseridosManualmenteAoGerarCarga");
            configuracao.AoCancelarCargaManterPedidosEmAberto = Request.GetBoolParam("AoCancelarCargaManterPedidosEmAberto");
            configuracao.RealizarIntegracaoDadosCancelamentoCarga = Request.GetBoolParam("RealizarIntegracaoDadosCancelamentoCarga");
            configuracao.UtilizarConfiguracaoTipoOperacaoGeracaoCargaPorPedido = Request.GetBoolParam("UtilizarConfiguracaoTipoOperacaoGeracaoCargaPorPedido");
            configuracao.PadraoVisualizacaoOperadorLogistico = Request.GetBoolParam("PadraoVisualizacaoOperadorLogistico");
            configuracao.NaoVincularAutomaticamenteDocumentosEmitidosEmbarcador = Request.GetBoolParam("NaoVincularAutomaticamenteDocumentosEmitidosEmbarcador");
            configuracao.PermitirAvancarCargasEmitidasEmbarcadorPorTipoOperacao = Request.GetBoolParam("PermitirAvancarCargasEmitidasEmbarcadorPorTipoOperacao");
            configuracao.AtualizarDadosDosPedidosComDadosDaCarga = Request.GetBoolParam("AtualizarDadosDosPedidosComDadosDaCarga");
            configuracao.AjustarValorFreteAposAprovacaoPreCTe = Request.GetBoolParam("AjustarValorFreteAposAprovacaoPreCTe");
            configuracao.VisualizarLegendaCargaAcordoTipoOperacao = Request.GetBoolParam("VisualizarLegendaCargaAcordoTipoOperacao");
            configuracao.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga = Request.GetBoolParam("CancelarCIOTAutomaticamenteFluxoCancelamentoCarga");
            configuracao.EnviarEmailPreviaCustoParaTransportadores = Request.GetBoolParam("EnviarEmailPreviaCustoParaTransportadores");
            configuracao.PermiteInformarRemetenteLancamentoNotaManualCarga = Request.GetBoolParam("PermiteInformarRemetenteLancamentoNotaManualCarga");
            configuracao.NaoGerarPDFDocumentosComNotasFiscais = Request.GetBoolParam("NaoGerarPDFDocumentosComNotasFiscais");
            configuracao.UsarPrioridadeDaCargaParaImpressaoDeObservacaoNoCTE = Request.GetBoolParam("UsarPrioridadeDaCargaParaImpressaoDeObservacaoNoCTE");
            configuracao.PermiteSelecionarMultiplasCargasParaRedespacho = Request.GetBoolParam("PermiteSelecionarMultiplasCargasParaRedespacho");
            configuracao.GerarRedespachoDeCargasAgrupadas = Request.GetBoolParam("GerarRedespachoDeCargasAgrupadas");
            configuracao.NaoEncerrarMDFeDeFormaAutomaticaAoConfirmarDadosDeTransporte = Request.GetBoolParam("NaoEncerrarMDFeDeFormaAutomaticaAoConfirmarDadosDeTransporte");
            configuracao.PermiteExcluirAgendamentoDaCargaJanelaDescarga = Request.GetBoolParam("PermiteExcluirAgendamentoDaCargaJanelaDescarga");
            configuracao.AssumirSempreTipoOperacaoDoPedido = Request.GetBoolParam("AssumirSempreTipoOperacaoDoPedido");
            configuracao.UtilizaControleDeEntregaManual = Request.GetBoolParam("UtilizaControleDeEntregaManual");
            configuracao.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo = Request.GetBoolParam("PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo");
            configuracao.NaoPermitirEncerrarCIOTEncerrarCarga = Request.GetBoolParam("NaoPermitirEncerrarCIOTEncerrarCarga");
            configuracao.DesabilitarUtilizacaoCreditoOperadores = Request.GetBoolParam("DesabilitarUtilizacaoCreditoOperadores");
            configuracao.ConverterXMLNotaFiscalParaByteArrayAoImportarNaCarga = Request.GetBoolParam("ConverterXMLNotaFiscalParaByteArrayAoImportarNaCarga");
            configuracao.RemoverVinculoNotaPedidoAbertoAoCancelarCarga = Request.GetBoolParam("RemoverVinculoNotaPedidoAbertoAoCancelarCarga");
            configuracao.GerarOutrosDocumentosNaImportacaoDeCTeComplementar = Request.GetBoolParam("GerarOutrosDocumentosNaImportacaoDeCTeComplementar");
            configuracao.GerarNumerodeCargaAlfanumerico = Request.GetBoolParam("GerarNumerodeCargaAlfanumerico");
            configuracao.TamanhoNumerodeCargaAlfanumerico = Request.GetEnumParam<TamanhoNumerodeCargaAlfanumerico>("TamanhoNumerodeCargaAlfanumerico");
            configuracao.ValidarValorLimiteApoliceComValorNFe = Request.GetBoolParam("ValidarValorLimiteApoliceComValorNFe");
            configuracao.NaoPermitirAlterarDadosCargaQuandoTiverIntegracaoIntegrada = Request.GetBoolParam("NaoPermitirAlterarDadosCargaQuandoTiverIntegracaoIntegrada");
            configuracao.AtribuirValorMercadoriaCTeNotasFiscaisDocumentosEmitidosEmbarcador = Request.GetBoolParam("AtribuirValorMercadoriaCTeNotasFiscaisDocumentosEmitidosEmbarcador");
            configuracao.PermitirEncaixarPedidosComReentregaSolicitada = Request.GetBoolParam("PermitirEncaixarPedidosComReentregaSolicitada");
            configuracao.NaoPermitirAvançarEtapaUmCargaComTransportadorSemApoliceVigente = Request.GetBoolParam("NaoPermitirAvançarEtapaUmCargaComTransportadorSemApoliceVigente");
            configuracao.ConsiderarDataEmissaoCTECalculoEmbarquePrevisaoEntrega = Request.GetBoolParam("ConsiderarDataEmissaoCTECalculoEmbarquePrevisaoEntrega");
            configuracao.ValidarContratanteOrigemVPIntegracaoPamcard = Request.GetBoolParam("ValidarContratanteOrigemVPIntegracaoPamcard");
            configuracao.GerarCargaComFluxoFilialEmissoraComExpedidor = Request.GetBoolParam("GerarCargaComFluxoFilialEmissoraComExpedidor");
            configuracao.ConsiderarConfiguracaoNoTipoDeOperacaoParaParticipantesDosDocumentosAoGerarCargaEspelho = Request.GetBoolParam("ConsiderarConfiguracaoNoTipoDeOperacaoParaParticipantesDosDocumentosAoGerarCargaEspelho");
            configuracao.ProcessarDadosTransporteAoFecharCarga = Request.GetBoolParam("ProcessarDadosTransporteAoFecharCarga");
            configuracao.UtilizarEmpresaFilialEmissoraNoArquivoEDI = Request.GetBoolParam("UtilizarEmpresaFilialEmissoraNoArquivoEDI");
            configuracao.NaoUtilizarCodigoCargaOrigemNaObservacaoCTe = Request.GetBoolParam("NaoUtilizarCodigoCargaOrigemNaObservacaoCTe");
            configuracao.PermiteReceberNotaFiscalViaIntegracaoNasEtapasFreteETransportador = Request.GetBoolParam("PermiteReceberNotaFiscalViaIntegracaoNasEtapasFreteETransportador");
            configuracao.PermiteInformarFreteOperadorFilialEmissora = Request.GetBoolParam("PermiteInformarFreteOperadorFilialEmissora");
            configuracao.RecalcularFreteAoDuplicarCargaCancelamentoDocumento = Request.GetBoolParam("RecalcularFreteAoDuplicarCargaCancelamentoDocumento");
            configuracao.NaoConsiderarRecebedorAoCalcularNumeroEntregasEmissaoPorPedido = Request.GetBoolParam("NaoConsiderarRecebedorAoCalcularNumeroEntregasEmissaoPorPedido");
            configuracao.PermiteHabilitarContingenciaEPECAutomaticamente = Request.GetBoolParam("PermiteHabilitarContingenciaEPECAutomaticamente");
            configuracao.TempoMinutosParaEnvioProgramadoIntegracao = Request.GetIntParam("TempoMinutosParaEnvioProgramadoIntegracao");
            configuracao.SetarCargaComoBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual = Request.GetBoolParam("SetarCargaComoBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual");
            configuracao.PermitirDesvincularGerarCopiaCTeRejeitadoCarga = Request.GetBoolParam("PermitirDesvincularGerarCopiaCTeRejeitadoCarga");
            configuracao.UtilizarDistanciaRoteirizacaoNaCarga = Request.GetNullableBoolParam("UtilizarDistanciaRoteirizacaoNaCarga");
            configuracao.PermitirSalvarApenasTransportadorEtapaUmCarga = Request.GetBoolParam("PermitirSalvarApenasTransportadorEtapaUmCarga");
            configuracao.ExigirConfirmacaoEtapaFreteNoFluxoNotaAposFrete = Request.GetBoolParam("ExigirConfirmacaoEtapaFreteNoFluxoNotaAposFrete");
            configuracao.PermitirReverterAnulacaoGerencialTelaCancelamento = Request.GetBoolParam("PermitirReverterAnulacaoGerencialTelaCancelamento");
            configuracao.AtivarCancelamentoDeFaturaETituloAoFluxoDeCancelamentoNaCarga = Request.GetBoolParam("AtivarCancelamentoDeFaturaETituloAoFluxoDeCancelamentoNaCarga");
            configuracao.NaoPermitirAtribuirVeiculoCargaSeExistirMonitoramentoAtivoParaPlaca = Request.GetBoolParam("NaoPermitirAtribuirVeiculoCargaSeExistirMonitoramentoAtivoParaPlaca");
            configuracao.ConsiderarFilialDaTransportadoraParaCompraDoValePedagioQuandoForEFrete = Request.GetBoolParam("ConsiderarFilialDaTransportadoraParaCompraDoValePedagioQuandoForEFrete");
            configuracao.InformarDocaNaEtapaUmDaCarga = Request.GetBoolParam("InformarDocaNaEtapaUmDaCarga");
            configuracao.CancelarValePedagioQuandoGerarCargaTransbordo = Request.GetBoolParam("CancelarValePedagioQuandoGerarCargaTransbordo");
            configuracao.ValidarModeloVeicularVeiculoCargaEtapaFrete = Request.GetBoolParam("ValidarModeloVeicularVeiculoCargaEtapaFrete");
            configuracao.ObrigatoriedadeCIOTEmissaoMDFe = Request.GetBoolParam("ObrigatoriedadeCIOTEmissaoMDFe");
            configuracao.PermitirInformarRecebedorAoCriarUmRedespachoManual = Request.GetBoolParam("PermitirInformarRecebedorAoCriarUmRedespachoManual");
            configuracao.UtilizarDataCarregamentoAoCriarCargaViaIntegracao = Request.GetBoolParam("UtilizarDataCarregamentoAoCriarCargaViaIntegracao");
            configuracao.PermitirFiltrarCargasNaEmissaoManualCteSemTerCtesImportados = Request.GetBoolParam("PermitirFiltrarCargasNaEmissaoManualCteSemTerCtesImportados");
            configuracao.PermitirRemoverMultiplosPedidosCarga = Request.GetBoolParam("PermitirRemoverMultiplosPedidosCarga");
            configuracao.SolicitarJustificativaAoRemoverPedidoCarga = Request.GetBoolParam("SolicitarJustificativaAoRemoverPedidoCarga");
            configuracao.IniciarConfirmacaoDocumentosFiscaisCargaPorThread = Request.GetBoolParam("IniciarConfirmacaoDocumentosFiscaisCargaPorThread");
            configuracao.PermitirInformarValorFreteOperadorMesmoComFreteConfirmadoPeloTransportador = Request.GetBoolParam("PermitirInformarValorFreteOperadorMesmoComFreteConfirmadoPeloTransportador");
            configuracao.PararCargaQuandoNaoInformadoCIOT = Request.GetBoolParam("PararCargaQuandoNaoInformadoCIOT");
            configuracao.NaoAplicarICMSMetodoAtualizarFrete = Request.GetBoolParam("NaoAplicarICMSMetodoAtualizarFrete");

            TipoIntegracao tipoIntegracao = Request.GetEnumParam<TipoIntegracao>("TipoIntegracaoCancelamentoPadrao");
            configuracao.TipoIntegracaoCancelamentoPadrao = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            repositorio.Atualizar(configuracao);
            config.SetExternalChanges(configuracao.GetCurrentChanges());
        }

        private void SalvarConfiguracaoCargaDadosTransporte(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracao = repositorio.BuscarPrimeiroRegistro();

            configuracao.Initialize();
            configuracao.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte = Request.GetBoolParam("RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte");
            configuracao.ExigirQueVeiculoCavaloTenhaReboqueVinculado = Request.GetBoolParam("ExigirQueVeiculoCavaloTenhaReboqueVinculado");
            configuracao.ExigirProtocoloLiberacaoSemIntegracaoGR = Request.GetBoolParam("ExigirProtocoloLiberacaoSemIntegracaoGR");
            configuracao.ExigirQueApolicePropriaTransportadorEstejaValida = Request.GetBoolParam("ExigirQueApolicePropriaTransportadorEstejaValida");

            repositorio.Atualizar(configuracao);
            config.SetExternalChanges(configuracao.GetCurrentChanges());
        }

        private void SalvarConfiguracaoCargaIntegracao(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracao = repositorio.BuscarPrimeiroRegistro();

            configuracao.Initialize();
            configuracao.AceitarPedidosComPendenciasDeProdutos = Request.GetBoolParam("AceitarPedidosComPendenciasDeProdutos");
            configuracao.NaoRetornarDocumentosAnteriores = Request.GetBoolParam("NaoRetornarDocumentosAnteriores");

            repositorio.Atualizar(configuracao);
            config.SetExternalChanges(configuracao.GetCurrentChanges());
        }

        private void SalvarConfiguracaoFatura(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();

            configuracaoFatura.Initialize();
            configuracaoFatura.PermitirVencimentoRetroativoFatura = Request.GetBoolParam("PermitirVencimentoRetroativoFatura");
            configuracaoFatura.PreencherPeriodoFaturaComDataAtual = Request.GetBoolParam("PreencherPeriodoFaturaComDataAtual");
            configuracaoFatura.InformarDataCancelamentoCancelamentoFatura = Request.GetBoolParam("InformarDataCancelamentoCancelamentoFatura");
            configuracaoFatura.DisponbilizarProvisaoContraPartidaParaCancelamento = Request.GetBoolParam("DisponbilizarProvisaoContraPartidaParaCancelamento");
            configuracaoFatura.HabilitarLayoutFaturaNFSManual = Request.GetBoolParam("HabilitarLayoutFaturaNFSManual");

            repConfiguracaoFatura.Atualizar(configuracaoFatura);
            config.SetExternalChanges(configuracaoFatura.GetCurrentChanges());
        }

        private void SalvarConfiguracaoPedido(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();

            int codigoFilialPadraoImportacaoPedido = Request.GetIntParam("FilialPadraoImportacaoPedido");

            configuracaoPedido.Initialize();
            configuracaoPedido.ApagarCampoRotaAoDuplicarPedido = Request.GetBoolParam("ApagarCampoRotaAoDuplicarPedido");
            configuracaoPedido.PessoasNaoObrigatorioProdutoEmbarcador = Request.GetBoolParam("PessoasNaoObrigatorioProdutoEmbarcador");
            configuracaoPedido.NaoApagarCamposDatasAoDuplicarPedido = Request.GetBoolParam("NaoApagarCamposDatasAoDuplicarPedido");
            configuracaoPedido.NaoPermitirImportarPedidosExistentes = Request.GetBoolParam("NaoPermitirImportarPedidosExistentes");
            configuracaoPedido.ConcatenarNumeroPreCargaNoPedido = Request.GetBoolParam("ConcatenarNumeroPreCargaNoPedido");
            configuracaoPedido.PermitirMudarStatusPedidoParaCanceladoAposVinculoCarga = Request.GetBoolParam("PermitirMudarStatusPedidoParaCanceladoAposVinculoCarga");
            configuracaoPedido.PermitirInformarAcrescimoDescontoNoPedido = Request.GetBoolParam("PermitirInformarAcrescimoDescontoNoPedido");
            configuracaoPedido.PermitirBuscarValoresTabelaFrete = Request.GetBoolParam("PermitirBuscarValoresTabelaFrete");
            configuracaoPedido.BuscarEmpresaPeloProprietarioDoVeiculo = Request.GetBoolParam("BuscarEmpresaPeloProprietarioDoVeiculo");
            configuracaoPedido.PermitirCriarPedidoApenasMotoristaSituacaoTrabalhando = Request.GetBoolParam("PermitirCriarPedidoApenasMotoristaSituacaoTrabalhando");
            configuracaoPedido.UtilizarRelatorioPedidoComoStatusEntrega = Request.GetBoolParam("UtilizarRelatorioPedidoComoStatusEntrega");
            configuracaoPedido.ExibirCamposRecebimentoPedidoIntegracao = Request.GetBoolParam("ExibirCamposRecebimentoPedidoIntegracao");
            configuracaoPedido.NaoPermitirAlterarCentroResultadoPedido = Request.GetBoolParam("NaoPermitirAlterarCentroResultadoPedido");
            configuracaoPedido.NaoPermitirInformarExpedidorNoPedido = Request.GetBoolParam("NaoPermitirInformarExpedidorNoPedido");
            configuracaoPedido.FormatoDataCarregamento = Request.GetNullableEnumParam<FormatoData>("FormatoDataCarregamentoImportacaoPedido");
            configuracaoPedido.FormatoHoraCarregamento = Request.GetNullableEnumParam<FormatoHora>("FormatoHoraCarregamentoImportacaoPedido");
            configuracaoPedido.FilialPadraoImportacaoPedido = codigoFilialPadraoImportacaoPedido > 0 ? repFilial.BuscarPorCodigo(codigoFilialPadraoImportacaoPedido) : null;
            configuracaoPedido.BloquearInsercaoNotaComEmitenteDiferenteRemetentePedido = Request.GetBoolParam("BloquearInsercaoNotaComEmitenteDiferenteRemetentePedido");
            configuracaoPedido.BloquearDuplicarPedido = Request.GetBoolParam("BloquearDuplicarPedido");
            configuracaoPedido.BloquearPedidoAoIntegrar = Request.GetBoolParam("BloquearPedidoAoIntegrar");
            configuracaoPedido.UtilizarEnderecoExpedidorRecebedorPedido = Request.GetBoolParam("UtilizarEnderecoExpedidorRecebedorPedido");
            configuracaoPedido.NaoValidarMesmaViagemEMesmoContainer = Request.GetBoolParam("NaoValidarMesmaViagemEMesmoContainer");
            configuracaoPedido.AtivarValidacaoCriacaoCarga = Request.GetBoolParam("AtivarValidacaoCriacaoCarga");
            configuracaoPedido.NaoPreencherRotaFreteAutomaticamente = Request.GetBoolParam("NaoPreencherRotaFreteAutomaticamente");
            configuracaoPedido.QuantidadeCargasEmAberto = Request.GetIntParam("QuantidadeCargasEmAberto");
            configuracaoPedido.NaoExibirPedidosDoDiaAgendamentoPedidos = Request.GetBoolParam("NaoExibirPedidosDoDiaAgendamentoPedidos");
            configuracaoPedido.ImportarOcorrenciasDePedidosPorPlanilhas = Request.GetBoolParam("ImportarOcorrenciasDePedidosPorPlanilhas");
            configuracaoPedido.NaoSelecionarModeloVeicularAutomaticamente = Request.GetBoolParam("NaoSelecionarModeloVeicularAutomaticamente");
            configuracaoPedido.NaoSubstituirEmpresaNaGeracaoCarga = Request.GetBoolParam("NaoSubstituirEmpresaNaGeracaoCarga");
            configuracaoPedido.ModeloEmailAgendamentoPedido = Request.GetEnumParam("ModeloEmailAgendamentoPedido", ModeloEmailAgendamentoPedido.Modelo1);
            configuracaoPedido.ValidarCadastroContainerPelaFormulaGlobal = Request.GetBoolParam("ValidarCadastroContainerPelaFormulaGlobal");
            configuracaoPedido.EnviarEmailTransportadorEntregaEmAtraso = Request.GetBoolParam("EnviarEmailTransportadorEntregaEmAtraso");
            configuracaoPedido.PermitirConsultaMassivaDePedidos = Request.GetBoolParam("PermitirConsultaMassivaDePedidos");
            configuracaoPedido.PermiteInformarPedidoDeSubstituicao = Request.GetBoolParam("PermiteInformarPedidoDeSubstituicao");
            configuracaoPedido.IgnorarValidacoesDatasPrevisaoAoEditarPedido = Request.GetBoolParam("IgnorarValidacoesDatasPrevisaoAoEditarPedido");
            configuracaoPedido.HerdarNotasImportadasPedido = Request.GetBoolParam("HerdarNotasImportadasPedido");
            configuracaoPedido.GerarCargaAutomaticamenteNoPedido = Request.GetBoolParam("GerarCargaAutomaticamenteNoPedido");
            configuracaoPedido.UtilizarBloqueioPessoasGrupoApenasParaTomadorDoPedido = Request.GetBoolParam("UtilizarBloqueioPessoasGrupoApenasParaTomadorDoPedido");
            configuracaoPedido.AtualizarCamposPedidoPorPlanilha = Request.GetBoolParam("AtualizarCamposPedidoPorPlanilha");
            configuracaoPedido.NaoPermitirInformarVeiculoDuplicadoPedidoCargaAberta = Request.GetBoolParam("NaoPermitirInformarVeiculoDuplicadoPedidoCargaAberta");
            configuracaoPedido.ExibirHoraCarregamentoEmPedidosDeColetaECodigosIntegracao = Request.GetBoolParam("ExibirHoraCarregamentoEmPedidosDeColetaECodigosIntegracao");
            configuracaoPedido.HabilitarBIDTransportePedido = Request.GetBoolParam("HabilitarBIDTransportePedido");
            configuracaoPedido.UsarFatorConversaoProdutoEmPedidoPaletizado = Request.GetBoolParam("UsarFatorConversaoProdutoEmPedidoPaletizado");
            configuracaoPedido.PermitirSelecionarCentroDeCarregamentoNoPedido = Request.GetBoolParam("PermitirSelecionarCentroDeCarregamentoNoPedido");
            configuracaoPedido.UtilizarCampoDeMotivoDePedido = Request.GetBoolParam("UtilizarCampoDeMotivoDePedido");
            configuracaoPedido.AjustarParticipantesPedidoCTeEmitidoEmbarcador = Request.GetBoolParam("AjustarParticipantesPedidoCTeEmitidoEmbarcador");
            configuracaoPedido.NaoLevarNumeroCotacaoParaPedidoGerado = Request.GetBoolParam("NaoLevarNumeroCotacaoParaPedidoGerado");
            configuracaoPedido.ImportarParalelizando = Request.GetBoolParam("ImportarParalelizando");
            configuracaoPedido.SempreConsiderarDestinatarioInformadoNoPedido = Request.GetBoolParam("SempreConsiderarDestinatarioInformadoNoPedido");
            configuracaoPedido.ExigirRotaRoteirizadaNoPedido = Request.GetBoolParam("ExigirRotaRoteirizadaNoPedido");
            configuracaoPedido.ExibirAuditoriaPedidos = Request.GetBoolParam("ExibirAuditoriaPedidos");
            configuracaoPedido.UtilizarParametrosBuscaAutomaticaClienteImportacao = Request.GetBoolParam("UtilizarParametrosBuscaAutomaticaClienteImportacao");
            configuracaoPedido.RemoverObservacoesDeEntregaAoRemoverPedidoCarga = Request.GetBoolParam("RemoverObservacoesDeEntregaAoRemoverPedidoCarga");
            configuracaoPedido.HabilitarOpcoesDeDuplicacaoDoPedidoParaDevolucaoTotalParcial = Request.GetBoolParam("HabilitarOpcoesDeDuplicacaoDoPedidoParaDevolucaoTotalParcial");
            configuracaoPedido.AtualizarCargaAoImportarPlanilha = Request.GetBoolParam("AtualizarCargaAoImportarPlanilha");
            configuracaoPedido.QuantidadeDiasDataColeta = Request.GetIntParam("QuantidadeDiasDataColeta");

            repConfiguracaoPedido.Atualizar(configuracaoPedido);
            config.SetExternalChanges(configuracaoPedido.GetCurrentChanges());
        }

        private void SalvarConfiguracaoOcorrencia(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

            configuracaoOcorrencia.Initialize();
            configuracaoOcorrencia.VisualizarUltimoUsuarioDelegadoOcorrencia = Request.GetBoolParam("VisualizarUltimoUsuarioDelegadoOcorrencia");
            configuracaoOcorrencia.PermiteInformarCentroResultadoAprovacaoOcorrencia = Request.GetBoolParam("PermiteInformarCentroResultadoAprovacaoOcorrencia");
            configuracaoOcorrencia.PermiteGerarOcorrenciaCargaAnulada = Request.GetBoolParam("PermiteGerarOcorrenciaCargaAnulada");
            configuracaoOcorrencia.ExibirDestinatarioOcorrencia = Request.GetBoolParam("ExibirDestinatarioOcorrencia");
            configuracaoOcorrencia.EnviarXMLDANFEClienteOcorrenciaPedido = Request.GetBoolParam("EnviarXMLDANFEClienteOcorrenciaPedido");
            configuracaoOcorrencia.PermiteDownloadCompactadoArquivoOcorrencia = Request.GetBoolParam("PermiteDownloadCompactadoArquivoOcorrencia");
            configuracaoOcorrencia.NaoImprimirTipoOcorrenciaNaObservacaoCTeComplementar = Request.GetBoolParam("NaoImprimirTipoOcorrenciaNaObservacaoCTeComplementar");
            configuracaoOcorrencia.ExibirCampoInformativoPagadorAutorizacaoOcorrencia = Request.GetBoolParam("ExibirCampoInformativoPagadorAutorizacaoOcorrencia");
            configuracaoOcorrencia.SalvarDocumentosDoCteAnteriorAoImportarCTeComplementar = Request.GetBoolParam("SalvarDocumentosDoCteAnteriorAoImportarCTeComplementar");
            configuracaoOcorrencia.InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos = Request.GetBoolParam("InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos");
            configuracaoOcorrencia.UtilizarBonificacaoParaTransportadoresViaOcorrencia = Request.GetBoolParam("UtilizarBonificacaoParaTransportadoresViaOcorrencia");
            configuracaoOcorrencia.PermitirDefinirCSTnoTipoDeOcorrencia = Request.GetBoolParam("PermitirDefinirCSTnoTipoDeOcorrencia");
            configuracaoOcorrencia.PermiteAdicionarMaisOcorrenciaMesmoEvento = Request.GetBoolParam("PermiteAdicionarMaisOcorrenciaMesmoEvento");
            configuracaoOcorrencia.PermiteInformarMaisDeUmaOcorrenciaPorNFe = Request.GetBoolParam("PermiteInformarMaisDeUmaOcorrenciaPorNFe");
            configuracaoOcorrencia.GerarObservacaoSubstitutoSomenteNumeroCTeAnterior = Request.GetBoolParam("GerarObservacaoSubstitutoSomenteNumeroCTeAnterior");
            configuracaoOcorrencia.HabilitarImportacaoOcorrenciaViaNOTFIS = Request.GetBoolParam("HabilitarImportacaoOcorrenciaViaNOTFIS");
            configuracaoOcorrencia.UtilizarNumeroTentativasTempoIntervaloIntegracaoOcorrenciaPersonalizado = Request.GetBoolParam("UtilizarNumeroTentativasTempoIntervaloIntegracaoOcorrenciaPersonalizado");
            configuracaoOcorrencia.NumeroTentativasIntegracao = Request.GetIntParam("NumeroTentativasIntegracao");
            configuracaoOcorrencia.IntervaloMinutosEntreIntegracoes = Request.GetIntParam("IntervaloMinutosEntreIntegracoes");
            configuracaoOcorrencia.IgnorarSituacaoDasNotasAoGerarOcorrencia = Request.GetBoolParam("IgnorarSituacaoDasNotasAoGerarOcorrencia");
            configuracaoOcorrencia.PermitirReabrirOcorrenciaEmCasoDeRejeicao = Request.GetBoolParam("PermitirReabrirOcorrenciaEmCasoDeRejeicao");
            configuracaoOcorrencia.PermitirIncluirOcorrenciaPorSelecaoNotasFiscaisCTe = Request.GetBoolParam("PermitirIncluirOcorrenciaPorSelecaoNotasFiscaisCTe");
            configuracaoOcorrencia.ExibirTodosCTesDaCargaNaAutorizacaoDeOcorrencia = Request.GetBoolParam("ExibirTodosCTesDaCargaNaAutorizacaoDeOcorrencia");
            configuracaoOcorrencia.NaoGerarAtendimentoDuplicadoParaMesmaOcorrencia = Request.GetBoolParam("NaoGerarAtendimentoDuplicadoParaMesmaOcorrencia");
            configuracaoOcorrencia.TrazerCentroResultadoOcorrencia = Request.GetBoolParam("TrazerCentroResultadoOcorrencia");
            configuracaoOcorrencia.UtilizaUsuarioPadraoParaGeracaoOcorrenciaPorEDI = Request.GetBoolParam("UtilizaUsuarioPadraoParaGeracaoOcorrenciaPorEDI");
            configuracaoOcorrencia.UsuarioPadraoParaGeracaoOcorrenciaPorEDI = repositorioUsuario.BuscarPorCodigo(Request.GetIntParam("UsuarioPadraoParaGeracaoOcorrenciaPorEDI"));
            configuracaoOcorrencia.PermitirVinculoAutomaticoEntreOcorreciaEAtendimento = Request.GetBoolParam("PermitirVinculoAutomaticoEntreOcorreciaEAtendimento");

            repConfiguracaoOcorrencia.Atualizar(configuracaoOcorrencia);
            config.SetExternalChanges(configuracaoOcorrencia.GetCurrentChanges());
        }

        private void SalvarConfiguracaoWebService(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

            configuracaoWebService.Initialize();

            configuracaoWebService.AtualizarDadosVeiculoIntegracaoCarga = Request.GetBoolParam("AtualizarDadosVeiculoIntegracaoCarga");
            configuracaoWebService.AdicionarVeiculoTipoReboqueComoReboqueAoAdicionarCarga = Request.GetBoolParam("AdicionarVeiculoTipoReboqueComoReboqueAoAdicionarCarga");
            configuracaoWebService.QuantidadeHorasPreencherDataCarregamentoCarga = Request.GetIntParam("QuantidadeHorasPreencherDataCarregamentoCarga");
            configuracaoWebService.IgnorarCamposEssenciais = Request.GetBoolParam("IgnorarCamposEssenciais");
            configuracaoWebService.NaoRetornarImagemCanhoto = Request.GetBoolParam("NaoRetornarImagemCanhoto");
            configuracaoWebService.RetornarEntregasRejeitadas = Request.GetBoolParam("RetornarEntregasRejeitadas");
            configuracaoWebService.SelecionarRotaFreteAoAdicionarPedido = Request.GetBoolParam("SelecionarRotaFreteAoAdicionarPedido");
            configuracaoWebService.NaoRoteirizarRotaNovamente = Request.GetBoolParam("NaoRoteirizarRotaNovamente");
            configuracaoWebService.SempreUtilizarTomadorEnviadoNoPedido = Request.GetBoolParam("SempreUtilizarTomadorEnviadoNoPedido");
            configuracaoWebService.UtilizarCodigosDeCadastroComoEnredecoSecundario = Request.GetBoolParam("UtilizarCodigosDeCadastroComoEnredecoSecundario");
            configuracaoWebService.FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoaDentroDeEnderecos = Request.GetBoolParam("FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoa");
            configuracaoWebService.CadastroAutomaticoPessoaExterior = Request.GetBoolParam("CadastroAutomaticoPessoaExterior");
            configuracaoWebService.TornarCampoINSSeReterImpostoTrazerComoSim = Request.GetBoolParam("TornarCampoINSSeReterImpostoTrazerComoSim");
            configuracaoWebService.AtivarValidacaoDosProdutosNoAdicionarCarga = Request.GetBoolParam("AtivarValidacaoDosProdutosNoAdicionarCarga");
            configuracaoWebService.PermitirUsarDescricaoFaixaTemperatura = Request.GetBoolParam("PermitirUsarDescricaoFaixaTemperatura");
            configuracaoWebService.RetornarApenasCarregamentosPendentesComTransportadora = Request.GetBoolParam("RetornarApenasCarregamentosPendentesComTransportadora");
            configuracaoWebService.NaoSobrePorInformacoesViaIntegracao = Request.GetBoolParam("NaoSobrePorInformacoesViaIntegracao");
            configuracaoWebService.BloquearInclusaoCargaComMesmoNumeroPedidoEmbarcador = Request.GetBoolParam("BloquearInclusaoCargaComMesmoNumeroPedidoEmbarcador");
            configuracaoWebService.RetornarCarregamentosSomenteCargasEmAgNF = Request.GetBoolParam("RetornarCarregamentosSomenteCargasEmAgNF");
            configuracaoWebService.NaoPermitirSolicitarCancelamentoCargaViaIntegracaoViagemIniciada = Request.GetBoolParam("NaoPermitirSolicitarCancelamentoCargaViaIntegracaoViagemIniciada");
            configuracaoWebService.NaoRetornarNFSeVinculadaNFSManualMetodoBuscarNFSs = Request.GetBoolParam("NaoRetornarNFSeVinculadaNFSManualMetodoBuscarNFSs");
            configuracaoWebService.PermiteReceberDataCriacaoPedidoERP = Request.GetBoolParam("PermiteReceberDataCriacaoPedidoERP");
            configuracaoWebService.NaoRecalcularFreteAoAdicionarRemoverPedido = Request.GetBoolParam("NaoRecalcularFreteAoAdicionarRemoverPedido");
            configuracaoWebService.AtualizarTodosCadastrosMotoristasMesmoCodigoIntegracao = Request.GetBoolParam("AtualizarTodosCadastrosMotoristasMesmoCodigoIntegracao");
            configuracaoWebService.SalvarRegiaoNoClienteParaPreencherRegiaoDestinoDosPedidos = Request.GetBoolParam("SalvarRegiaoNoClienteParaPreencherRegiaoDestinoDosPedidos");
            configuracaoWebService.NaoPermitirGerarNFSeComMesmaNumeracao = Request.GetBoolParam("NaoPermitirGerarNFSeComMesmaNumeracao");
            configuracaoWebService.SempreSeguirConfiguracaoOcorrenciaQuandoAdicionadaPeloMetodoAdicionarOcorrencia = Request.GetBoolParam("SempreSeguirConfiguracaoOcorrenciaQuandoAdicionadaPeloMetodoAdicionarOcorrencia");
            configuracaoWebService.HabilitarFluxoPedidoEcommerce = Request.GetBoolParam("HabilitarFluxoPedidoEcommerce");
            configuracaoWebService.AtualizarNumeroPedidoVinculado = Request.GetBoolParam("AtualizarNumeroPedidoVinculado");
            configuracaoWebService.RetornarDadosRedespachoTransbordoComInformacoesCargaOrigemConsultada = Request.GetBoolParam("RetornarDadosRedespachoTransbordoComInformacoesCargaOrigemConsultada");
            configuracaoWebService.NaoValidarTipoDeVeiculoNoMetodoInformarDadosTransporteCarga = Request.GetBoolParam("NaoValidarTipoDeVeiculoNoMetodoInformarDadosTransporteCarga");
            configuracaoWebService.CadastrarVeiculoAoInformarDadosTransporteCarga = Request.GetBoolParam("CadastrarVeiculoAoInformarDadosTransporteCarga");
            configuracaoWebService.NaoRetornarCargasCanceladasMetodoBuscarPendetesNotasFiscais = Request.GetBoolParam("NaoRetornarCargasCanceladasMetodoBuscarPendetesNotasFiscais");
            configuracaoWebService.NaoPermitirConfirmarEntregaSituacaoCargaEmAndamento = Request.GetBoolParam("NaoPermitirConfirmarEntregaSituacaoCargaEmAndamento");
            configuracaoWebService.RetornarCargasEmQualquerEtapaNoMetodoBuscarCargaPendenteIntegracao = Request.GetBoolParam("RetornarCargasEmQualquerEtapaNoMetodoBuscarCargaPendenteIntegracao");
            configuracaoWebService.RetornarApenasOcorrenciasFinalizadasMetodoBuscarOcorrenciasPendentesIntegracao = Request.GetBoolParam("RetornarApenasOcorrenciasFinalizadasMetodoBuscarOcorrenciasPendentesIntegracao");
            configuracaoWebService.PermitirRemoverDataPrevisaoDataPagamentoMetodoInformarPrevisaoPagamentoCTe = Request.GetBoolParam("PermitirRemoverDataPrevisaoDataPagamentoMetodoInformarPrevisaoPagamentoCTe");
            configuracaoWebService.DesvincularPreenchimentoDasDatasNosMetodosInformarPrevisaoPagamentoCTeConfirmarPagamentoCTe = Request.GetBoolParam("DesvincularPreenchimentoDasDatasNosMetodosInformarPrevisaoPagamentoCTeConfirmarPagamentoCTe");
            configuracaoWebService.GerarLogMetodosREST = Request.GetBoolParam("GerarLogMetodosREST");
            configuracaoWebService.PermitirAlterarNumeroCargaQuandoForCarga = Request.GetBoolParam("PermitirAlterarNumeroCargaQuandoForCarga");
            configuracaoWebService.NaoVincularReboqueNaTracaoAoAcionarMetodoGerarCarregamento = Request.GetBoolParam("NaoVincularReboqueNaTracaoAoAcionarMetodoGerarCarregamento");
            configuracaoWebService.PermitirRemoverVeiculoNoMetodoInformarDadosTransporteCarga = Request.GetBoolParam("PermitirRemoverVeiculoNoMetodoInformarDadosTransporteCarga");
            configuracaoWebService.AoSalvarDocumentoTransporteValidarSituacaoCarga = Request.GetBoolParam("AoSalvarDocumentoTransporteValidarSituacaoCarga");
            configuracaoWebService.NaoFiltrarSequencialCargaNoMetodoAdicionarCargaPedido = Request.GetBoolParam("NaoFiltrarSequencialCargaNoMetodoAdicionarCargaPedido");
            configuracaoWebService.QuandoGeradoPreCteRetornarInformacaoDeFreteCTeIntegrado = Request.GetBoolParam("QuandoGeradoPreCteRetornarInformacaoDeFreteCTeIntegrado");

            repositorioConfiguracaoWebService.Atualizar(configuracaoWebService);
            config.SetExternalChanges(configuracaoWebService.GetCurrentChanges());
        }

        private void SalvarConfiguracaoTabelaFrete(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();

            configuracaoTabelaFrete.Initialize();
            configuracaoTabelaFrete.PermitirInformarLeadTimeTabelaFreteCliente = Request.GetBoolParam("PermitirInformarLeadTimeTabelaFreteCliente");
            configuracaoTabelaFrete.ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais = Request.GetBoolParam("ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais");
            configuracaoTabelaFrete.UtilizarIntegracaoAlteracaoTabelaFrete = Request.GetBoolParam("UtilizarIntegracaoAlteracaoTabelaFrete");
            configuracaoTabelaFrete.UtilizarVigenciaConfiguracaoDescargaCliente = Request.GetBoolParam("UtilizarVigenciaConfiguracaoDescargaCliente");
            configuracaoTabelaFrete.MostrarRegistroSomenteComValoresNaAprovacao = Request.GetBoolParam("MostrarRegistroSomenteComValoresNaAprovacao");
            configuracaoTabelaFrete.ObrigatorioInformarTransportadorAjusteTabelaFrete = Request.GetBoolParam("ObrigatorioInformarTransportadorAjusteTabelaFrete");
            configuracaoTabelaFrete.ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete = Request.GetBoolParam("ObrigatorioInformarContratoTransporteFreteAjusteTabelaFrete");
            configuracaoTabelaFrete.NaoBuscarAutomaticamenteVigenciaTabelaFrete = Request.GetBoolParam("NaoBuscarAutomaticamenteVigenciaTabelaFrete");
            configuracaoTabelaFrete.ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo = Request.GetBoolParam("ImportarTabelaFreteClienteInformandoOrigensDestinosEmDiferentesColunasNoMesmoArquivo");
            configuracaoTabelaFrete.GravarAuditoriaImportarTabelaFrete = Request.GetBoolParam("GravarAuditoriaImportarTabelaFrete");
            configuracaoTabelaFrete.ArredondarValorDoComponenteDePedagioParaProximoInteiro = Request.GetBoolParam("ArredondarValorDoComponenteDePedagioParaProximoInteiro");
            configuracaoTabelaFrete.SalvarPlacasVeiculosAoSalvarModelosVeiculos = Request.GetBoolParam("SalvarPlacasVeiculosAoSalvarModelosVeiculos");
            configuracaoTabelaFrete.NaoPermiteEdicoesEmValoresNaConsultaDeTabelaFrete = Request.GetBoolParam("NaoPermiteEdicoesEmValoresNaConsultaDeTabelaFrete");

            repositorioConfiguracaoTabelaFrete.Atualizar(configuracaoTabelaFrete);
            config.SetExternalChanges(configuracaoTabelaFrete.GetCurrentChanges());
        }

        private void SalvarConfiguracaoJanelaCarregamento(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

            configuracaoJanelaCarregamento.Initialize();
            configuracaoJanelaCarregamento.ExibirOpcaoLiberarParaTransportador = Request.GetBoolParam("ExibirOpcaoLiberarParaTransportador");
            configuracaoJanelaCarregamento.ExibirOpcaoMultiModalAgendamentoColeta = Request.GetBoolParam("ExibirOpcaoMultiModalAgendamentoColeta");
            configuracaoJanelaCarregamento.SugerirDataEntregaAgendamentoColeta = Request.GetBoolParam("SugerirDataEntregaAgendamentoColeta");
            configuracaoJanelaCarregamento.GerarFluxoPatioCargaComExpedidor = Request.GetBoolParam("GerarFluxoPatioCargaComExpedidor");
            configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga = Request.GetBoolParam("DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga");
            configuracaoJanelaCarregamento.AtualizarDataInicialColetaAoAlterarHorarioCarregamento = Request.GetBoolParam("AtualizarDataInicialColetaAoAlterarHorarioCarregamento");
            configuracaoJanelaCarregamento.BloquearGeracaoJanelaParaCargaRedespacho = Request.GetBoolParam("BloquearGeracaoJanelaParaCargaRedespacho");
            configuracaoJanelaCarregamento.AtivarPlanejamentoFrotaCarga = Request.GetBoolParam("AtivarPlanejamentoFrotaCarga");
            configuracaoJanelaCarregamento.AtivarPlanejamentoFrotaNoPlanejamentoVeiculo = Request.GetBoolParam("AtivarPlanejamentoFrotaNoPlanejamentoVeiculo");
            configuracaoJanelaCarregamento.NaoPermitirRecalcularValorFreteInformadoPeloTransportador = Request.GetBoolParam("NaoPermitirRecalcularValorFreteInformadoPeloTransportador");
            configuracaoJanelaCarregamento.NaoEnviarEmailAlteracaoDataCarregamento = Request.GetBoolParam("NaoEnviarEmailAlteracaoDataCarregamento");
            configuracaoJanelaCarregamento.BloquearVeiculoSemTagValePedagioAtiva = Request.GetBoolParam("BloquearVeiculoSemTagValePedagioAtiva");
            configuracaoJanelaCarregamento.DiasParaPermitirInformarHorarioCarregamento = Request.GetIntParam("DiasParaPermitirInformarHorarioCarregamento");
            configuracaoJanelaCarregamento.EncaixarHorarioRetiradaProduto = Request.GetBoolParam("EncaixarHorarioRetiradaProduto");
            configuracaoJanelaCarregamento.LiberarCargaParaCotacaoAoLiberarParaTransportadores = Request.GetBoolParam("LiberarCargaParaCotacaoAoLiberarParaTransportadores");
            configuracaoJanelaCarregamento.PermitirLiberarCargaParaTransportadoresTerceiros = Request.GetBoolParam("PermitirLiberarCargaParaTransportadoresTerceiros");
            configuracaoJanelaCarregamento.GerarJanelaDeCarregamento = Request.GetBoolParam("GerarJanelaDeCarregamento");
            configuracaoJanelaCarregamento.UtilizarCentroDescarregamentoPorTipoCarga = Request.GetBoolParam("UtilizarCentroDescarregamentoPorTipoCarga");
            configuracaoJanelaCarregamento.ExibirDetalhesAgendamentoJanelaTransportador = Request.GetBoolParam("ExibirDetalhesAgendamentoJanelaTransportador");
            configuracaoJanelaCarregamento.UtilizarPeriodoDescarregamentoExclusivo = Request.GetBoolParam("UtilizarPeriodoDescarregamentoExclusivo");
            configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorPrioridade = Request.GetBoolParam("DisponibilizarCargaParaTransportadoresPorPrioridade");
            configuracaoJanelaCarregamento.ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga = Request.GetBoolParam("ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga");
            configuracaoJanelaCarregamento.NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento = Request.GetBoolParam("NaoCancelarCargaAoAplicarStatusFinalizadorJanelaDescarregamento");
            configuracaoJanelaCarregamento.TempoPermitirReagendamentoHoras = Request.GetIntParam("TempoPermitirReagendamentoHoras");
            configuracaoJanelaCarregamento.PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga = Request.GetBoolParam("PermitirTransportadorInformarPlacasEMotoristaAoDeclararInteresseCarga");

            repositorioConfiguracaoJanelaCarregamento.Atualizar(configuracaoJanelaCarregamento);
            config.SetExternalChanges(configuracaoJanelaCarregamento.GetCurrentChanges());
        }

        private void SalvarConfiguracaoDocumentoEntrada(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();

            configuracaoDocumentoEntrada.Initialize();
            configuracaoDocumentoEntrada.BloquearCadastroProdutoComMesmoCodigo = Request.GetBoolParam("BloquearCadastroProdutoComMesmoCodigo");
            configuracaoDocumentoEntrada.BloquearFinalizacaoComFluxoCompraAberto = Request.GetBoolParam("BloquearFinalizacaoComFluxoCompraAberto");
            configuracaoDocumentoEntrada.PermitirSelecionarOSFinalizadaDocumentoEntrada = Request.GetBoolParam("PermitirSelecionarOSFinalizadaDocumentoEntrada");
            configuracaoDocumentoEntrada.FormulaCustoPadrao = Request.GetStringParam("FormulaCustoPadrao");

            repConfiguracaoDocumentoEntrada.Atualizar(configuracaoDocumentoEntrada);
            config.SetExternalChanges(configuracaoDocumentoEntrada.GetCurrentChanges());
        }

        private void SalvarConfiguracaoMobile(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile repConfiguracaoMobile = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile configuracaoMobile = repConfiguracaoMobile.BuscarConfiguracaoPadrao();

            configuracaoMobile.Initialize();
            configuracaoMobile.ValidarRaioCliente = Request.GetBoolParam("ValidarRaioCliente");
            configuracaoMobile.RetornarMultiplasCargasApp = Request.GetBoolParam("RetornarMultiplasCargasApp");
            configuracaoMobile.DiasLimiteRetornarMultiplasCargas = Request.GetIntParam("DiasLimiteRetornarMultiplasCargas");
            configuracaoMobile.MenuCarga = Request.GetBoolParam("MenuCarga");
            configuracaoMobile.MenuServicos = Request.GetBoolParam("MenuServicos");
            configuracaoMobile.MenuOcorrencias = Request.GetBoolParam("MenuOcorrencias");
            configuracaoMobile.MenuExtratoViagem = Request.GetBoolParam("MenuExtratoViagem");
            configuracaoMobile.MenuPontosParada = Request.GetBoolParam("MenuPontosParada");
            configuracaoMobile.MenuServicosViagem = Request.GetBoolParam("MenuServicosViagem");
            configuracaoMobile.MenuRH = Request.GetBoolParam("MenuRH");
            configuracaoMobile.DataReferenciaBusca = Request.GetEnumParam<DataReferenciaBusca>("DataReferenciaBusca");
            configuracaoMobile.IntervaloInicial = Request.GetIntParam("IntervaloInicial");
            configuracaoMobile.IntervaloFinal = Request.GetIntParam("IntervaloFinal");
            configuracaoMobile.HabilitarAlertaMotorista = Request.GetBoolParam("HabilitarAlertaMotorista");
            configuracaoMobile.MinutosNotificarAlertaMotoristaViagem = Request.GetIntParam("MinutosNotificarMotoristaAlertaViagem");

            repConfiguracaoMobile.Atualizar(configuracaoMobile);
            config.SetExternalChanges(configuracaoMobile.GetCurrentChanges());
        }

        private void SalvarConfiguraoAcertoViagem(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarConfiguracaoPadrao();

            configuraoAcertoViagem.Initialize();
            configuraoAcertoViagem.NaoFecharAcertoViagemAteReceberPallets = Request.GetBoolParam("NaoFecharAcertoViagemAteReceberPallets");
            configuraoAcertoViagem.VisualizarPalletsCanhotosNasCargas = Request.GetBoolParam("VisualizarPalletsCanhotosNasCargas");
            configuraoAcertoViagem.HabilitarFormaRecebimentoTituloAoMotorista = Request.GetBoolParam("HabilitarFormaRecebimentoTituloAoMotorista");
            configuraoAcertoViagem.HabilitarLancamentoTacografo = Request.GetBoolParam("HabilitarLancamentoTacografo");
            configuraoAcertoViagem.SepararValoresAdiantamentoMotoristaPorTipo = Request.GetBoolParam("SepararValoresAdiantamentoMotoristaPorTipo");
            configuraoAcertoViagem.HabilitarInformacaoAcertoMotorista = Request.GetBoolParam("HabilitarInformacaoAcertoMotorista");
            configuraoAcertoViagem.HabilitarControlarOutrasDespesas = Request.GetBoolParam("HabilitarControlarOutrasDespesas");
            configuraoAcertoViagem.TextoRecibo = Request.GetStringParam("TextoRecibo");

            repConfiguraoAcertoViagem.Atualizar(configuraoAcertoViagem);
            config.SetExternalChanges(configuraoAcertoViagem.GetCurrentChanges());
        }

        private void SalvarConfiguracaoVeiculo(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

            configuracaoVeiculo.Initialize();
            configuracaoVeiculo.ObrigatorioSegmentoVeiculo = Request.GetBoolParam("ObrigatorioSegmentoVeiculo");
            configuracaoVeiculo.VisualizarApenasVeiculosAtivos = Request.GetBoolParam("VisualizarApenasVeiculosAtivos");
            configuracaoVeiculo.ObrigatorioInformarAnoFabricacao = Request.GetBoolParam("ObrigatorioInformarAnoFabricacao");
            configuracaoVeiculo.ObrigatorioInformarReboqueParaVeiculosDoTipoRodadoCavalo = Request.GetBoolParam("ObrigatorioInformarReboqueParaVeiculosDoTipoRodadoCavalo");
            configuracaoVeiculo.ManterVinculoMotoristaEmFolga = Request.GetBoolParam("ManterVinculoMotoristaEmFolga");
            configuracaoVeiculo.NaoPermitirAlterarKMVeiculoEquipamentoPneuPelaOrdemServico = Request.GetBoolParam("NaoPermitirAlterarKMVeiculoEquipamentoPneuPelaOrdemServico");
            configuracaoVeiculo.KMLimiteAberturaOrdemServico = Request.GetIntParam("KMLimiteAberturaOrdemServico");
            configuracaoVeiculo.NaoPermitirQueTransportadorInativeVeiculo = Request.GetBoolParam("NaoPermitirQueTransportadorInativeVeiculo");
            configuracaoVeiculo.NaoPermitirRealizarCadastroPlacaBloqueada = Request.GetBoolParam("NaoPermitirRealizarCadastroPlacaBloqueada");
            configuracaoVeiculo.NaoPermitirUtilizarVeiculoEmManutencao = Request.GetBoolParam("NaoPermitirUtilizarVeiculoEmManutencao");
            configuracaoVeiculo.LancamentoServicoManualNaOSMarcadadoPorDefault = Request.GetBoolParam("LancamentoServicoManualNaOSMarcadadoPorDefault");
            configuracaoVeiculo.NaoPermitirVincularPneuVeiculoAbastecimentoAberto = Request.GetBoolParam("NaoPermitirVincularPneuVeiculoAbastecimentoAberto");
            configuracaoVeiculo.NaoPermitirCadastrarVeiculoSemRastreador = Request.GetBoolParam("NaoPermitirCadastrarVeiculoSemRastreador");
            configuracaoVeiculo.CriarVinculoFrotaCargaForaDoPlanejamentoFrota = Request.GetBoolParam("CriarVinculoFrotaCargaForaDoPlanejamentoFrota");
            configuracaoVeiculo.NaoAlterarCentroResultadoVeiculosEmissaoCargas = Request.GetBoolParam("NaoAlterarCentroResultadoVeiculosEmissaoCargas");
            configuracaoVeiculo.NaoPermitirRealizarFechamentoOrdemServicoCustoZerado = Request.GetBoolParam("NaoPermitirRealizarFechamentoOrdemServicoCustoZerado");
            configuracaoVeiculo.ValidarExisteCargaMesmoNumeroCIOT = Request.GetBoolParam("ValidarExisteCargaMesmoNumeroCIOT");
            configuracaoVeiculo.RemoverNumeroCIOTEncerrado = Request.GetBoolParam("RemoverNumeroCIOTEncerrado");
            configuracaoVeiculo.ObrigatorioInformarModeloVeicularCargaNoWebService = Request.GetBoolParam("ObrigatorioInformarModeloVeicularCargaNoWebService");
            configuracaoVeiculo.SalvarTransportadorTerceiroComoGerarCIOT = Request.GetBoolParam("SalvarTransportadorTerceiroComoGerarCIOT");
            configuracaoVeiculo.NaoMostrarMotivoBloqueio = Request.GetBoolParam("NaoMostrarMotivoBloqueio");
            configuracaoVeiculo.ExibirAbaDeEixosNoModeloVeicular = Request.GetBoolParam("ExibirAbaDeEixosNoModeloVeicular");
            configuracaoVeiculo.ObrigarANTTVeiculoValidarSalvarDadosTransporte = Request.GetBoolParam("ObrigarANTTVeiculoValidarSalvarDadosTransporte");
            configuracaoVeiculo.UtilizarMesmoGestorParaTodaComposicao = Request.GetBoolParam("UtilizarMesmoGestorParaTodaComposicao");
            configuracaoVeiculo.CadastrarVeiculoMotoristaBRK = Request.GetBoolParam("CadastrarVeiculoMotoristaBRK");
            configuracaoVeiculo.BloquearAlteracaoCentroResultadoNaMovimentacaoPlaca = Request.GetBoolParam("BloquearAlteracaoCentroResultadoNaMovimentacaoPlaca");
            configuracaoVeiculo.ValidarTAGDigitalCom = Request.GetBoolParam("ValidarTAGDigitalCom");
            configuracaoVeiculo.InformarKmMovimentacaoPlaca = Request.GetBoolParam("InformarKmMovimentacaoPlaca");
            configuracaoVeiculo.AtualizarHistoricoSituacaoVeiculo = Request.GetBoolParam("AtualizarHistoricoSituacaoVeiculo");

            repConfiguracaoVeiculo.Atualizar(configuracaoVeiculo);
            config.SetExternalChanges(configuracaoVeiculo.GetCurrentChanges());
        }

        private void SalvarConfiguracaoMotorista(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repConfiguracaoMotorista.BuscarConfiguracaoPadrao();

            configuracaoMotorista.Initialize();
            configuracaoMotorista.NaoPermitirTransportadoAlterarDataValidadeSeguradora = Request.GetBoolParam("NaoPermitirTransportadoAlterarDataValidadeSeguradora");
            configuracaoMotorista.BloquearCamposMotoristaLGPD = Request.GetBoolParam("BloquearCamposMotoristaLGPD");
            configuracaoMotorista.ExibirCamposSuspensaoMotorista = Request.GetBoolParam("ExibirCamposSuspensaoMotorista");
            configuracaoMotorista.BloquearChecklistMotoristaSemLicencaVinculada = Request.GetBoolParam("BloquearChecklistMotoristaSemLicencaVinculada");
            configuracaoMotorista.NaoPermitirInativarMotoristaComSaldoNoExtrato = Request.GetBoolParam("NaoPermitirInativarMotoristaComSaldoNoExtrato");
            configuracaoMotorista.PermitirCadastrarMotoristaEstrangeiro = Request.GetBoolParam("PermitirCadastrarMotoristaEstrangeiro");
            configuracaoMotorista.NaoValidarHoraNoPagamentoMotorista = Request.GetBoolParam("NaoValidarHoraNoPagamentoMotorista");
            configuracaoMotorista.MotoristaUsarFotoDoApp = Request.GetBoolParam("MotoristaUsarFotoDoApp");
            configuracaoMotorista.ExibirConfiguracoesPortalTransportador = Request.GetBoolParam("ExibirConfiguracoesPortalTransportador");
            configuracaoMotorista.NaoPermitirRealizarCadastroMotoristaBloqueado = Request.GetBoolParam("NaoPermitirRealizarCadastroMotoristaBloqueado");
            configuracaoMotorista.PermiteDuplicarCadastroMotorista = Request.GetBoolParam("PermiteDuplicarCadastroMotorista");
            configuracaoMotorista.DiasAntecidenciaParaComunicarMotoristaVencimentoLicenca = Request.GetIntParam("DiasAntecidenciaParaComunicarMotoristaVencimentoLicenca");
            configuracaoMotorista.MensagemPersonalizadaMotoristaBloqueado = Request.GetStringParam("MensagemPersonalizadaMotoristaBloqueado");
            configuracaoMotorista.NaoGerarPreTripMotoristasIgnorados = Request.GetBoolParam("NaoGerarPreTripMotoristasIgnorados");
            configuracaoMotorista.HabilitarUsoCentroResultadoComissaoMotorista = Request.GetBoolParam("HabilitarUsoCentroResultadoComissaoMotorista");
            configuracaoMotorista.HabilitarControleSituacaoColaboradorParaMotoristasTerceiros = Request.GetBoolParam("HabilitarControleSituacaoColaboradorParaMotoristasTerceiros");
            if (configuracaoMotorista.MotoristasIgnorados == null)
                configuracaoMotorista.MotoristasIgnorados = new List<string>();
            else
                configuracaoMotorista.MotoristasIgnorados.Clear();

            dynamic dynMotoristasIgnorados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("MotoristasIgnorados"));
            foreach (dynamic motoristaIgnorado in dynMotoristasIgnorados)
            {
                configuracaoMotorista.MotoristasIgnorados.Add((string)motoristaIgnorado.NomeMotoristaIgnorado);
            }

            //configuracaoMotorista.MotoristasIgnorados = Request.GetListParam<string>("MotoristasIgnorados");

            repConfiguracaoMotorista.Atualizar(configuracaoMotorista);
            config.SetExternalChanges(configuracaoMotorista.GetCurrentChanges());
        }

        private void SalvarConfiguracaoEncerramentoMDFAutomatico(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe repConfiguracaoEncerramentoMDFe = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe configuracaoEncerramentoAutomaticoMDFe = repConfiguracaoEncerramentoMDFe.BuscarConfiguracaoPadrao();

            configuracaoEncerramentoAutomaticoMDFe.Initialize();
            configuracaoEncerramentoAutomaticoMDFe.DiasEncerramentoAutomaticoMDFE = Request.GetIntParam("DiasEncerramentoAutomaticoMDFE");

            repConfiguracaoEncerramentoMDFe.Atualizar(configuracaoEncerramentoAutomaticoMDFe);
            config.SetExternalChanges(configuracaoEncerramentoAutomaticoMDFe.GetCurrentChanges());
        }

        private void SalvarConfiguracaoFinanceiro(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento repMotivoCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento(unitOfWork);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            configuracaoFinanceiro.Initialize();
            configuracaoFinanceiro.ValidarDuplicidadeTituloSemData = Request.GetBoolParam("ValidarDuplicidadeTituloSemData");
            configuracaoFinanceiro.AtivarControleDespesas = Request.GetBoolParam("AtivarControleDespesas");
            configuracaoFinanceiro.DiasDeFechamentoParaGeracaoPagamentoEscrituracaoAutomatico = Request.GetStringParam("DiasDeFechamentoParaGeracaoPagamentoEscrituracaoAutomatico");
            configuracaoFinanceiro.GerarLotesAposEmissaoDaCarga = Request.GetBoolParam("GerarLotesAposEmissaoDaCarga");
            configuracaoFinanceiro.GerarLotePagamentoAposDigitalizacaoDoCanhoto = Request.GetBoolParam("GerarLotePagamentoAposDigitalizacaoDoCanhoto");
            configuracaoFinanceiro.GerarLotesProvisaoAposEmissaoDaCarga = Request.GetBoolParam("GerarLotesProvisaoAposEmissaoDaCarga");
            configuracaoFinanceiro.NaoGerarLoteProvisaoParaCargaAguardandoImportarCTeOuLancarNFS = Request.GetBoolParam("NaoGerarLoteProvisaoParaCargaAguardandoImportarCTeOuLancarNFS");
            configuracaoFinanceiro.NaoGerarLoteProvisaoParaOcorrencia = Request.GetBoolParam("NaoGerarLoteProvisaoParaOcorrencia");
            configuracaoFinanceiro.DesbloquearPagamentoPorCanhoto = Request.GetBoolParam("DesbloquearPagamentoPorCanhoto");

            configuracaoFinanceiro.ExibirNumeroPedidoEmbarcadorGestaoDocumentos = Request.GetBoolParam("ExibirNumeroPedidoEmbarcadorGestaoDocumentos");
            configuracaoFinanceiro.PermitirDeixarDocumentoEmTratativa = Request.GetBoolParam("PermitirDeixarDocumentoEmTratativa");
            if (Request.GetLongParam("Despachante") > 0)
                configuracaoFinanceiro.Despachante = repCliente.BuscarPorCPFCNPJ(Request.GetLongParam("Despachante"));
            else
                configuracaoFinanceiro.Despachante = null;
            configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo = Request.GetIntParam("QuantidadeDiasLimiteVencimentoTitulo");
            configuracaoFinanceiro.MovimentacaoFinanceiraParaTitulosDeProvisao = Request.GetBoolParam("MovimentacaoFinanceiraParaTitulosDeProvisao");
            configuracaoFinanceiro.UtilizarValorDesproporcionalRateioDespesaVeiculo = Request.GetBoolParam("UtilizarValorDesproporcionalRateioDespesaVeiculo");
            configuracaoFinanceiro.PermitirProvisionamentoDeNotasCTesNaTelaProvisao = Request.GetBoolParam("PermitirProvisionamentoDeNotasCTesNaTelaProvisao");
            configuracaoFinanceiro.GerarMovimentoPelaDataVencimentoContratoFinanceiro = Request.GetBoolParam("GerarMovimentoPelaDataVencimentoContratoFinanceiro");
            configuracaoFinanceiro.AutomatizarGeracaoLoteProvisao = Request.GetBoolParam("AutomatizarGeracaoLoteProvisao");
            configuracaoFinanceiro.UtilizarCustoParaRealizarRateiosSobreDocumentoEntrada = Request.GetBoolParam("UtilizarCustoParaRealizarRateiosSobreDocumentoEntrada");
            configuracaoFinanceiro.UtilizarDataVencimentoTituloMovimentoContrato = Request.GetBoolParam("UtilizarDataVencimentoTituloMovimentoContrato");
            configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins = Request.GetBoolParam("NaoIncluirICMSBaseCalculoPisCofins");
            configuracaoFinanceiro.NaoObrigarTipoOperacaoFatura = Request.GetBoolParam("NaoObrigarTipoOperacaoFatura");
            configuracaoFinanceiro.HabilitarOpcaoGerarFaturasApenasCanhotosAprovados = Request.GetBoolParam("HabilitarOpcaoGerarFaturasApenasCanhotosAprovados");
            configuracaoFinanceiro.AtivarColunaCSTConsultaDocumentosFatura = Request.GetBoolParam("AtivarColunaCSTConsultaDocumentosFatura");
            configuracaoFinanceiro.AtivarColunaNumeroContainerConsultaDocumentosFatura = Request.GetBoolParam("AtivarColunaNumeroContainerConsultaDocumentosFatura");
            configuracaoFinanceiro.ExigirInformarFilialEmissaoFaturas = Request.GetBoolParam("ExigirInformarFilialEmissaoFaturas");

            configuracaoFinanceiro.RatearMovimentosDescontosAcrescimosBaixaTitulosPagar = Request.GetBoolParam("RatearMovimentosDescontosAcrescimosBaixaTitulosPagar");
            configuracaoFinanceiro.PermitirConfirmarDocumentosFaturaApenasComCtesEscriturados = Request.GetBoolParam("PermitirConfirmarDocumentosFaturaApenasComCtesEscriturados");
            configuracaoFinanceiro.NaoValidarCondicaoPagamentoFechamentoLotePagamento = Request.GetBoolParam("NaoValidarCondicaoPagamentoFechamentoLotePagamento");
            configuracaoFinanceiro.ValidarDataPrevisaoPagamentoEDataPagamentoNoCancelamentoDosCTes = Request.GetBoolParam("ValidarDataPrevisaoPagamentoEDataPagamentoNoCancelamentoDosCTes");
            configuracaoFinanceiro.GerarLotesPagamentoIndividuaisPorDocumento = Request.GetBoolParam("GerarLotesPagamentoIndividuaisPorDocumento");
            configuracaoFinanceiro.SomarValorISSNoTotalReceberGeracaoLoteProvisao = Request.GetBoolParam("SomarValorISSNoTotalReceberGeracaoLoteProvisao");
            configuracaoFinanceiro.HorasLiberacaoDocumentoPagamentoTransportadorComCertificado = Request.GetIntParam("HorasLiberacaoDocumentoPagamentoTransportadorComCertificado");
            configuracaoFinanceiro.HorasLiberacaoDocumentoPagamentoTransportadorSemCertificado = Request.GetIntParam("HorasLiberacaoDocumentoPagamentoTransportadorSemCertificado");
            configuracaoFinanceiro.NaoGerarAutomaticamenteLotesCancelados = Request.GetBoolParam("NaoGerarAutomaticamenteLotesCancelados");
            configuracaoFinanceiro.GerarEstornoProvisaoAutomaticoAposEscrituracao = Request.GetBoolParam("GerarEstornoProvisaoAutomaticoAposEscrituracao");
            configuracaoFinanceiro.GerarEstornoProvisaoAutomaticoAposLiberacaoPagamento = Request.GetBoolParam("GerarEstornoProvisaoAutomaticoAposLiberacaoPagamento");
            configuracaoFinanceiro.UtilizarEstornoProvisaoDeFormaAutomatizada = Request.GetBoolParam("UtilizarEstornoProvisaoDeFormaAutomatizada");
            configuracaoFinanceiro.RateioProvisaoPorGrupoProduto = Request.GetBoolParam("RateioProvisaoPorGrupoProduto");
            configuracaoFinanceiro.GerarLoteProvisaoIndividualNfe = Request.GetBoolParam("GerarLoteProvisaoIndividualNfe");
            configuracaoFinanceiro.UtilizarFechamentoAutomaticoProvisao = Request.GetBoolParam("UtilizarFechamentoAutomaticoProvisao");
            configuracaoFinanceiro.NaoPermitirProvisionarSemCalculoFrete = Request.GetBoolParam("NaoPermitirProvisionarSemCalculoFrete");
            configuracaoFinanceiro.GerarDoumentoProvisaoAoReceberNotaFiscal = Request.GetBoolParam("GerarDoumentoProvisaoAoReceberNotaFiscal");
            configuracaoFinanceiro.HabilitaIntervaloTempoLiberaDocumentoEmitidoEscrituracao = Request.GetBoolParam("HabilitaIntervaloTempoLiberaDocumentoEmitidoEscrituracao");
            configuracaoFinanceiro.MinutosAguardarGeracaoLotePagamento = ConverterHorasAguardarGeracaoLotePagamentoParaMinutos(Request.GetStringParam("MinutosAguardarGeracaoLotePagamento"));
            configuracaoFinanceiro.MinutosAguardarGeracaoLotePagamentoUltimoDiaMes = ConverterHorasAguardarGeracaoLotePagamentoParaMinutos(Request.GetStringParam("MinutosAguardarGeracaoLotePagamentoUltimoDiaMes"));
            configuracaoFinanceiro.DelayFaturamentoAutomatico = Request.GetIntParam("DelayFaturamentoAutomatico");
            configuracaoFinanceiro.QuantidadeDocumentosLotePagamentoGeradoAutomatico = Request.GetIntParam("QuantidadeDocumentosLotePagamentoGeradoAutomatico");
            configuracaoFinanceiro.QuantidadeMinimaDocumentosLotePagamentoGeradoAutomatico = Request.GetIntParam("QuantidadeMinimaDocumentosLotePagamentoGeradoAutomatico");
            configuracaoFinanceiro.QuantidadeDiasAbertoEstornoProvisao = Request.GetIntParam("QuantidadeDiasAbertoEstornoProvisao");
            configuracaoFinanceiro.NaoGerarPagamentoParaMotoristaTerceiro = Request.GetBoolParam("NaoGerarPagamentoParaMotoristaTerceiro");
            configuracaoFinanceiro.EfetuarVinculoCentroResultadoCTeSubstituto = Request.GetBoolParam("EfetuarVinculoCentroResultadoCTeSubstituto");
            configuracaoFinanceiro.EfetuarCancelamentoDePagamentoAoCancelarCarga = Request.GetBoolParam("EfetuarCancelamentoDePagamentoAoCancelarCarga");
            configuracaoFinanceiro.BloqueioEnvioIntegracoesCargasAnuladaseCanceladas = Request.GetBoolParam("BloqueioEnvioIntegracoesCargasAnuladaseCanceladas");
            configuracaoFinanceiro.MotivoCancelamentoPagamentoPadrao = repMotivoCancelamentoPagamento.BuscarPorCodigo(Request.GetIntParam("MotivoCancelamentoPagamentoPadrao"));
            configuracaoFinanceiro.TravarFluxoCompraCasoFornecedorDivergenteNaOrdemCompra = Request.GetBoolParam("TravarFluxoCompraCasoFornecedorDivergenteNaOrdemCompra");
            configuracaoFinanceiro.AgruparProvisoesPorNotaFiscalFechamentoMensal = Request.GetBoolParam("AgruparProvisoesPorNotaFiscalFechamentoMensal");
            configuracaoFinanceiro.NaoPermitirGerarLotesPagamentosDocumentosBloqueados = Request.GetBoolParam("NaoPermitirGerarLotesPagamentosDocumentosBloqueados");
            configuracaoFinanceiro.NaoPermitirReenviarIntegracoesPagamentoSeCancelado = Request.GetBoolParam("NaoPermitirReenviarIntegracoesPagamentoSeCancelado");
            configuracaoFinanceiro.GerarIntegracaoContabilizacaoCtesApos = Request.GetBoolParam("GerarIntegracaoContabilizacaoCtesApos");
            configuracaoFinanceiro.DelayIntegracaoContabilizacaoCtes = Request.GetIntParam("DelayIntegracaoContabilizacaoCtes");
            configuracaoFinanceiro.UtilizarConfiguracoesTransportadorParaFatura = Request.GetBoolParam("UtilizarConfiguracoesTransportadorParaFatura");
            configuracaoFinanceiro.BaixaTitulosRenegociacaoGerarNovoTituloPorDocumento = Request.GetBoolParam("BaixaTitulosRenegociacaoGerarNovoTituloPorDocumento");
            configuracaoFinanceiro.GerarLotePagamentoSomenteParaCTe = Request.GetBoolParam("GerarLotePagamentoSomenteParaCTe");
            configuracaoFinanceiro.UtilizarPreenchimentoTomadorFaturaConfiguracao = Request.GetBoolParam("UtilizarPreenchimentoTomadorFaturaConfiguracao");
            configuracaoFinanceiro.ManterValorMoedaConfirmarDocumentosFatura = Request.GetBoolParam("ManterValorMoedaConfirmarDocumentosFatura");
            configuracaoFinanceiro.UtilizarEmpresaFilialImpressaoReciboPagamentoMotorista = Request.GetBoolParam("UtilizarEmpresaFilialImpressaoReciboPagamentoMotorista");

            if (configuracaoFinanceiro.HabilitaIntervaloTempoLiberaDocumentoEmitidoEscrituracao ?? false)
                SalvarConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(configuracaoFinanceiro, unitOfWork);

            repConfiguracaoFinanceiro.Atualizar(configuracaoFinanceiro);
            config.SetExternalChanges(configuracaoFinanceiro.GetCurrentChanges());
        }

        private void SalvarConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao servicoConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao = new Servicos.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(unitOfWork);

            servicoConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao.SalvarConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(configuracaoFinanceiro, Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao>>(Request.Params("ConfiguracaoIntervaloTempoLiberaDocumentoEmitidoEscrituracao")));
        }

        private void SalvarConfiguracaoChamado(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();

            configuracaoChamado.Initialize();
            configuracaoChamado.BloquearAberturaChamadoRetencaoQuandoPossuirReentrega = Request.GetBoolParam("BloquearAberturaChamadoRetencaoQuandoPossuirReentrega");
            configuracaoChamado.ObrigatorioInformarNotaNaDevolucaoParcialChamado = Request.GetBoolParam("ObrigatorioInformarNotaNaDevolucaoParcialChamado");
            configuracaoChamado.HabilitarArvoreDecisaoEscalationList = Request.GetBoolParam("HabilitarArvoreDecisaoEscalationList");
            configuracaoChamado.EscalarAutomaticamenteNivelExcederTempo = Request.GetBoolParam("EscalarAutomaticamenteNivelExcederTempo");
            configuracaoChamado.FinalizarEntregaQuandoDevolucaoParcial = Request.GetBoolParam("FinalizarEntregaQuandoDevolucaoParcial");
            configuracaoChamado.ObrigatorioInformarNotaFiscalParaAberturaChamado = Request.GetBoolParam("ObrigatorioInformarNotaFiscalParaAberturaChamado");
            configuracaoChamado.CalcularValorDasDevolucoes = Request.GetBoolParam("CalcularValorDasDevolucoes");
            configuracaoChamado.PermitirRegistrarObservacoesSemVisualizacaoTransportadora = Request.GetBoolParam("PermitirRegistrarObservacoesSemVisualizacaoTransportadora");
            configuracaoChamado.AtivarAlertaChamadosMais48hAberto = Request.GetBoolParam("AtivarAlertaChamadosMais48hAberto");
            configuracaoChamado.PermitirAbrirChamadoParaEntregaJaRealizada = Request.GetBoolParam("PermitirAbrirChamadoParaEntregaJaRealizada");
            configuracaoChamado.PermiteFinalizarAtendimentoComOcorrenciaRejeitada = Request.GetBoolParam("PermiteFinalizarAtendimentoComOcorrenciaRejeitada");
            configuracaoChamado.VincularPrimeiroPedidoDoClienteAoAbrirChamado = Request.GetBoolParam("VincularPrimeiroPedidoDoClienteAoAbrirChamado");
            configuracaoChamado.PermitirSelecionarCteApenasComNfeVinculadaOcorrencia = Request.GetBoolParam("PermitirSelecionarCteApenasComNfeVinculadaOcorrencia");
            configuracaoChamado.PermitirGerarAtendimentoPorPedido = Request.GetBoolParam("PermitirGerarAtendimentoPorPedido");
            configuracaoChamado.FazerGestaoCriticidade = Request.GetBoolParam("FazerGestaoCriticidade");
            configuracaoChamado.PermitirAtualizarChamadoStatus = Request.GetBoolParam("PermitirAtualizarChamadoStatus");
            configuracaoChamado.OcultarTomadorNoAtendimento = Request.GetBoolParam("OcultarTomadorNoAtendimento");
            configuracaoChamado.BloquearEstornoAtendimentosFinalizadosPortalTransportador = Request.GetBoolParam("BloquearEstornoAtendimentosFinalizadosPortalTransportador");

            repConfiguracaoChamado.Atualizar(configuracaoChamado);
            config.SetExternalChanges(configuracaoChamado.GetCurrentChanges());
        }

        private void SalvarConfiguracaoControleEntrega(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

            configuracaoControleEntrega.Initialize();
            configuracaoControleEntrega.UtilizarPrevisaoEntregaPedidoComoDataPrevista = Request.GetBoolParam("UtilizarPrevisaoEntregaPedidoComoDataPrevista");
            configuracaoControleEntrega.UtilizarMaiorDataColetaPrevistaComoDataPrevistaParaEntregaUnica = Request.GetBoolParam("UtilizarMaiorDataColetaPrevistaComoDataPrevistaParaEntregaUnica");
            configuracaoControleEntrega.TempoInicioViagemAposEmissaoDoc = Request.GetIntParam("TempoInicioViagemAposEmissaoDoc");
            configuracaoControleEntrega.TempoInicioViagemAposFinalizacaoFluxoPatio = Request.GetIntParam("TempoInicioViagemAposFinalizacaoFluxoPatio");
            configuracaoControleEntrega.ObrigatorioInformarFreetime = Request.GetBoolParam("ObrigatorioInformarFreetimeCadastroRotas");
            configuracaoControleEntrega.ExibirOpcaoAjustarEntregaOnTime = Request.GetBoolParam("ExibirOpcaoAjustarEntregaOnTime");
            configuracaoControleEntrega.ExibirPacotesOcorrenciaControleEntrega = Request.GetBoolParam("ExibirPacotesOcorrenciaControleEntrega");
            configuracaoControleEntrega.PermitirReordenarEntregasAoAddPedido = Request.GetBoolParam("PermitirReordenarEntregasAoAddPedido");
            configuracaoControleEntrega.HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos = Request.GetNullableTimeParam("HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos");
            configuracaoControleEntrega.HoraFimPadraoEntrega = Request.GetNullableTimeParam("HoraFimPadraoEntrega");
            configuracaoControleEntrega.PermiteAlterarAgendamentoDaEntregaNoAcompanhamentoDeCargas = Request.GetBoolParam("PermiteAlterarAgendamentoDaEntregaNoAcompanhamentoDeCargas");
            configuracaoControleEntrega.PermitirAbrirAtendimentoViaControleEntrega = Request.GetBoolParam("PermitirAbrirAtendimentoViaControleEntrega");
            configuracaoControleEntrega.RecalcularPrevisaoAoIniciarViagem = Request.GetBoolParam("RecalcularPrevisaoAoIniciarViagem");
            configuracaoControleEntrega.ExibirDataEntregaNotaControleEntrega = Request.GetBoolParam("ExibirDataEntregaNotaControleEntrega");
            configuracaoControleEntrega.PermiteExibirCargaCancelada = Request.GetBoolParam("PermiteExibirCargaCancelada");
            configuracaoControleEntrega.NaoPermitirConfirmacaoEntregaPortalTransportadorSemDigitalizacaoCanhotos = Request.GetBoolParam("NaoPermitirConfirmacaoEntregaPortalTransportadorSemDigitalizacaoCanhotos");
            configuracaoControleEntrega.PermitirAlterarDataAgendamentoEntregaTransportador = Request.GetBoolParam("PermitirAlterarDataAgendamentoEntregaTransportador");
            configuracaoControleEntrega.ConsiderarCargaOrigemParaEntregasTransbordadas = Request.GetBoolParam("ConsiderarCargaOrigemParaEntregasTransbordadas");
            configuracaoControleEntrega.RejeitarEntregaNotaFiscalAoRejeitarCanhoto = Request.GetBoolParam("RejeitarEntregaNotaFiscalAoRejeitarCanhoto");
            configuracaoControleEntrega.ConsiderarMediaDeVelocidadeDasUltimasCincoPosicoes = Request.GetBoolParam("ConsiderarMediaDeVelocidadeDasUltimasCincoPosicoes");
            configuracaoControleEntrega.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida = Request.GetBoolParam("BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida");
            configuracaoControleEntrega.UtilizarLeadTimeDaTabelaDeFreteParaCalculoDaPrevisaoDeEntrega = Request.GetBoolParam("UtilizarLeadTimeDaTabelaDeFreteParaCalculoDaPrevisaoDeEntrega");
            configuracaoControleEntrega.CalcularDataAgendamentoAutomaticamenteDataFaturamento = Request.GetBoolParam("CalcularDataAgendamentoAutomaticamenteDataFaturamento");
            configuracaoControleEntrega.PermitirEnvioCanhotosPeloPortalTransportadorControleEntregas = Request.GetBoolParam("PermitirEnvioCanhotosPeloPortalTransportadorControleEntregas");
            configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona = Request.GetBoolParam("TornarFinalizacaoDeEntregasAssincrona");
            configuracaoControleEntrega.PermitirEnvioNovasOcorrenciasComMesmoCadastroTipoOcorrencia = Request.GetBoolParam("PermitirEnvioNovasOcorrenciasComMesmoCadastroTipoOcorrencia");
            configuracaoControleEntrega.PermitirBuscarCargasAgrupadasAoPesquisarNumero = Request.GetBoolParam("PermitirBuscarCargasAgrupadasAoPesquisarNumero");
            configuracaoControleEntrega.EncerrarMDFeAutomaticamenteAoFinalizarEntregas = Request.GetBoolParam("EncerrarMDFeAutomaticamenteAoFinalizarEntregas");
            configuracaoControleEntrega.PermitirAjustarEntregasEtapasAnterioresIntegracao = Request.GetBoolParam("PermitirAjustarEntregasEtapasAnterioresIntegracao");
            configuracaoControleEntrega.PermitirBloqueioFinalizacaoEntrega = Request.GetBoolParam("PermitirBloqueioFinalizacaoEntrega");
            configuracaoControleEntrega.PossuiNotaCobertura = Request.GetBoolParam("PossuiNotaCobertura");
            configuracaoControleEntrega.TempoReprocessarCargaEntregasSemNotas = Request.GetIntParam("TempoReprocessarCargaEntregasSemNotas");

            repConfiguracaoControleEntrega.Atualizar(configuracaoControleEntrega);
            config.SetExternalChanges(configuracaoControleEntrega.GetCurrentChanges());
        }

        private void SalvarConfiguracaoRedmine(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRedMine repConfiguracaoRedMine = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRedMine(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRedMine configuracaoRedMine = repConfiguracaoRedMine.BuscarConfiguracaoPadrao();

            configuracaoRedMine.Initialize();
            configuracaoRedMine.ClienteRedMine = Request.GetStringParam("ClienteRedMine");
            configuracaoRedMine.CodigoUsuarioDestino = Request.GetIntParam("CodigoUsuarioDestino");

            repConfiguracaoRedMine.Atualizar(configuracaoRedMine);
            config.SetExternalChanges(configuracaoRedMine.GetCurrentChanges());
        }

        private void SalvarConfiguracaoMontagemCarga(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();

            configuracaoMontagemCarga.Initialize();
            configuracaoMontagemCarga.ExigirDefinicaoTipoCarregamentoPedido = Request.GetBoolParam("ExigirDefinicaoTipoCarregamentoPedido");
            configuracaoMontagemCarga.AtualizarInformacoesPedidosPorCarga = Request.GetBoolParam("AtualizarInformacoesPedidosPorCarga");
            configuracaoMontagemCarga.ApresentaOpcaoRemoverCancelarPedidos = Request.GetBoolParam("ApresentaOpcaoRemoverCancelarPedidos");
            configuracaoMontagemCarga.ApresentaOpcaoCancelarReserva = Request.GetBoolParam("ApresentaOpcaoCancelarReserva");
            configuracaoMontagemCarga.FiltroPeriodoVazioAoIniciar = Request.GetBoolParam("FiltroPeriodoVazioAoIniciar");
            configuracaoMontagemCarga.DataAtualNovoCarregamento = Request.GetBoolParam("DataAtualNovoCarregamento");
            configuracaoMontagemCarga.OcultarBipagem = Request.GetBoolParam("OcultarBipagem");
            configuracaoMontagemCarga.UtilizarDataPrevisaoSaidaVeiculo = Request.GetBoolParam("UtilizarDataPrevisaoSaidaVeiculo");
            configuracaoMontagemCarga.NaoPermitirPedidosTomadoresDiferentesMesmoCarregamento = Request.GetBoolParam("NaoPermitirPedidosTomadoresDiferentesMesmoCarregamento");
            configuracaoMontagemCarga.GerarUnicoBlocoPorRecebedor = Request.GetBoolParam("GerarUnicoBlocoPorRecebedor");
            configuracaoMontagemCarga.PermitirGerarCarregamentoPedidoBloqueado = Request.GetBoolParam("PermitirGerarCarregamentoPedidoBloqueado");
            configuracaoMontagemCarga.AtivarTratativaDuplicidadeEmissaoCargasFeeder = Request.GetBoolParam("AtivarTratativaDuplicidadeEmissaoCargasFeeder");
            configuracaoMontagemCarga.TipoControleSaldoPedido = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoControleSaldoPedido>("TipoControleSaldoPedido", TipoControleSaldoPedido.Peso);
            configuracaoMontagemCarga.GerarCargaAoConfirmarIntegracaoCarregamento = Request.GetBoolParam("GerarCargaAoConfirmarIntegracaoCarregamento");
            configuracaoMontagemCarga.NaoRetornarIntegracaoCarregamentoSeSomenteDadosTransporteForemAlterados = Request.GetBoolParam("NaoRetornarIntegracaoCarregamentoSeSomenteDadosTransporteForemAlterados");
            configuracaoMontagemCarga.RoteirizarAutomaticamenteAposRoteirizadoAoAdicionarRemoverPedido = Request.GetBoolParam("RoteirizarAutomaticamenteAposRoteirizadoAoAdicionarRemoverPedido");
            configuracaoMontagemCarga.ExibirAlertaRestricaoEntregaClienteCardCarregamento = Request.GetBoolParam("ExibirAlertaRestricaoEntregaClienteCardCarregamento");
            configuracaoMontagemCarga.AtivarMontagemCargaPorNFe = Request.GetBoolParam("AtivarMontagemCargaPorNFe");
            configuracaoMontagemCarga.UtilizarFiliaisHabilitadasTransportarMontagemCargaMapa = Request.GetBoolParam("UtilizarFiliaisHabilitadasTransportarMontagemCargaMapa");
            configuracaoMontagemCarga.FiltrarPedidosOndeRecebedorTransportadorNoPortalDoTransportador = Request.GetBoolParam("FiltrarPedidosOndeRecebedorTransportadorNoPortalDoTransportador");
            configuracaoMontagemCarga.VencedorSimuladorFreteEmpresaPedido = Request.GetBoolParam("VencedorSimuladorFreteEmpresaPedido");
            configuracaoMontagemCarga.PermitirEditarPedidosAtravesTelaMontagemCargaMapa = Request.GetBoolParam("PermitirEditarPedidosAtravesTelaMontagemCargaMapa");
            configuracaoMontagemCarga.IgnorarRotaFretePedidosMontagemCargaMapa = Request.GetBoolParam("IgnorarRotaFretePedidosMontagemCargaMapa");
            configuracaoMontagemCarga.ManterPedidosComMesmoAgrupadorNaMesmaCarga = Request.GetBoolParam("ManterPedidosComMesmoAgrupadorNaMesmaCarga");
            configuracaoMontagemCarga.ExibirPedidosFormatoGrid = Request.GetBoolParam("ExibirPedidosFormatoGrid");
            configuracaoMontagemCarga.ExibirListagemNotasFiscais = Request.GetBoolParam("ExibirListagemNotasFiscais");
            configuracaoMontagemCarga.ConsiderarSomentePesoOuCubagemAoGerarBloco = Request.GetBoolParam("ConsiderarSomentePesoOuCubagemAoGerarBloco");
            configuracaoMontagemCarga.FiltrarPedidosVinculadoOutrasCarga = Request.GetBoolParam("FiltrarPedidosVinculadoOutrasCarga");

            repositorioConfiguracaoMontagemCarga.Atualizar(configuracaoMontagemCarga);
            config.SetExternalChanges(configuracaoMontagemCarga.GetCurrentChanges());
        }

        private void SalvarConfiguracaoDocumentosDestinados(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado repConfiguracaoDocumentoDestinado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado configuracaoDocumentoDestinado = repConfiguracaoDocumentoDestinado.BuscarConfiguracaoPadrao();

            configuracaoDocumentoDestinado.Initialize();
            configuracaoDocumentoDestinado.BloquearLancamentoDocumentosTipoEntrada = Request.GetBoolParam("BloquearLancamentoDocumentosTipoEntrada");
            configuracaoDocumentoDestinado.SalvarDocumentosRecebidosEmailDestinados = Request.GetBoolParam("SalvarDocumentosRecebidosEmailDestinados");
            configuracaoDocumentoDestinado.VincularCteNaOcorrenciaApartirDaObservacao = Request.GetBoolParam("VincularCteNaOcorrenciaApartirDaObservacao");
            configuracaoDocumentoDestinado.NaoInutilizarCTEsFiscalmenteApenasGerencialmente = Request.GetBoolParam("NaoInutilizarCTEsFiscalmenteApenasGerencialmente");
            configuracaoDocumentoDestinado.NaoReutilizarNumeracaoAposAnularGerencialmente = Request.GetBoolParam("NaoReutilizarNumeracaoAposAnularGerencialmente");
            configuracaoDocumentoDestinado.NaoSalvarXmlApenasNaFalha = Request.GetBoolParam("NaoSalvarXmlApenasNaFalha");


            repConfiguracaoDocumentoDestinado.Atualizar(configuracaoDocumentoDestinado);
            config.SetExternalChanges(configuracaoDocumentoDestinado.GetCurrentChanges());
        }

        private void SalvarConfiguracaoPaletes(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes repConfiguracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = repConfiguracaoPaletes.BuscarConfiguracaoPadrao();
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            configuracaoPaletes.Initialize();
            configuracaoPaletes.UtilizarControlePaletesPorModeloVeicular = Request.GetBoolParam("UtilizarControlePaletesModeloVeicular");
            configuracaoPaletes.LiquidarPalletAutomaticamente = Request.GetBoolParam("LiquidarPalletAutomaticamente");
            configuracaoPaletes.QteDiasParaLiquidarPallet = Request.GetIntParam("QteDiasParaLiquidarPallet");
            configuracaoPaletes.NaoExibirDevolucaoPaletesSemNotaFiscal = Request.GetBoolParam("NaoExibirDevolucaoPaletesSemNotaFiscal");
            configuracaoPaletes.UtilizarControlePaletesPorCliente = Request.GetBoolParam("UtilizarControlePaletesPorCliente");
            configuracaoPaletes.LimiteDiasParaDevolucaoDePallet = Request.GetIntParam("LimiteDiasParaDevolucaoDePallet");
            configuracaoPaletes.NotificarPaletesPendentes = Request.GetBoolParam("NotificarPaletesPendentes");
            configuracaoPaletes.DiaSemanaNotificarPaletesPendentes = Request.GetEnumParam<DiaSemana>("DiaSemanaNotificarPaletesPendentes");

            int codigoTipoOcorrencia = Request.GetIntParam("TipoOcorrenciaPadraoPallet");

            if (codigoTipoOcorrencia > 0)
                configuracaoPaletes.TipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoTipoOcorrencia);

            repConfiguracaoPaletes.Atualizar(configuracaoPaletes);
            config.SetExternalChanges(configuracaoPaletes.GetCurrentChanges());
        }

        private void SalvarConfiguracaoProduto(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto repConfiguracaoProduto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoProduto configuracaoProduto = repConfiguracaoProduto.BuscarConfiguracaoPadrao();

            configuracaoProduto.Initialize();
            configuracaoProduto.LayoutEtiquetaProduto = Request.GetEnumParam<LayoutEtiquetaProduto>("LayoutEtiquetaProduto");
            configuracaoProduto.ControlarEstoqueReserva = Request.GetBoolParam("ControlarEstoqueReserva");
            configuracaoProduto.RealizarValidacaoComEstoqueDePosicaoAoFecharOrdemDeServico = Request.GetBoolParam("RealizarValidacaoComEstoqueDePosicaoAoFecharOrdemDeServico");
            configuracaoProduto.SalvarProdutosDaNotaFiscal = Request.GetBoolParam("SalvarProdutosDaNotaFiscal");

            repConfiguracaoProduto.Atualizar(configuracaoProduto);
            config.SetExternalChanges(configuracaoProduto.GetCurrentChanges());
        }

        private void SalvarConfiguracaoCalculoPrevisao(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao repConfiguracaoPrevisao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCalculoPrevisao configuracaoCalculoPrevisao = repConfiguracaoPrevisao.BuscarConfiguracaoPadrao();

            configuracaoCalculoPrevisao.Initialize();
            configuracaoCalculoPrevisao.DesconsiderarSabadosCalculoPrevisao = Request.GetBoolParam("DesconsiderarSabadosCalculoPrevisao");
            configuracaoCalculoPrevisao.DesconsiderarFeriadosCalculoPrevisao = Request.GetBoolParam("DesconsiderarFeriadosCalculoPrevisao");
            configuracaoCalculoPrevisao.DesconsiderarDomingosCalculoPrevisao = Request.GetBoolParam("DesconsiderarDomingosCalculoPrevisao");
            configuracaoCalculoPrevisao.ConsiderarJornadaMotorita = Request.GetBoolParam("ConsiderarJornadaMotorita");
            configuracaoCalculoPrevisao.HorarioInicialAlmoco = Request.GetTimeParam("HorarioInicialAlmoco");
            configuracaoCalculoPrevisao.MinutosIntervalo = Request.GetIntParam("MinutosIntervalo");

            repConfiguracaoPrevisao.Atualizar(configuracaoCalculoPrevisao);
            config.SetExternalChanges(configuracaoCalculoPrevisao.GetCurrentChanges());
        }

        private void SalvarConfiguracaoConfiguracaoFluxoPatio(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio repConfiguracaoFluxoPatio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio configuracaoFluxoPatio = repConfiguracaoFluxoPatio.BuscarConfiguracaoPadrao();

            configuracaoFluxoPatio?.Initialize();
            configuracaoFluxoPatio.TipoComprovanteSaida = Request.GetEnumParam<TipoComprovanteSaida>("TipoComprovanteSaida");
            configuracaoFluxoPatio.PermitirBaixarRomaneioNaEtapaFimCarregamento = Request.GetBoolParam("PermitirBaixarRomaneioNaEtapaFimCarregamento");
            configuracaoFluxoPatio.ValidarPesoCargaComPesagemVeiculo = Request.GetBoolParam("ValidarPesoCargaComPesagemVeiculo");
            configuracaoFluxoPatio.PermiteAlocarVeiculoSemConjuntoCarga = Request.GetBoolParam("PermiteAlocarVeiculoSemConjuntoCarga");
            configuracaoFluxoPatio.RegistrarPosicaoVeiculoSubareaAoReceberEvento = Request.GetBoolParam("RegistrarPosicaoVeiculoSubareaAoReceberEvento");

            repConfiguracaoFluxoPatio.Atualizar(configuracaoFluxoPatio);
            config.SetExternalChanges(configuracaoFluxoPatio.GetCurrentChanges());
        }

        private void SalvarConfiguracaoConfiguracaoNFSeManual(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoNFSeManual repConfiguracaoNFSeManual = new Repositorio.Embarcador.Configuracoes.ConfiguracaoNFSeManual(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoNFSeManual configuracaoNFSeManual = repConfiguracaoNFSeManual.BuscarConfiguracaoPadrao();

            configuracaoNFSeManual?.Initialize();
            configuracaoNFSeManual.UtilizarNumeroSerieInformadoTelaQuandoEmitidoModeloDocumentoNaoFiscal = Request.GetBoolParam("UtilizarNumeroSerieInformadoTelaQuandoEmitidoModeloDocumentoNaoFiscal");
            configuracaoNFSeManual.ValidarLocalidadePrestacaoTransportadorConfiguracaoNFSe = Request.GetBoolParam("ValidarLocalidadePrestacaoTransportadorConfiguracaoNFSe");
            configuracaoNFSeManual.ValidarExistenciaParaInserirNFSe = Request.GetBoolParam("ValidarExistenciaParaInserirNFSe");

            repConfiguracaoNFSeManual.Atualizar(configuracaoNFSeManual);
            config.SetExternalChanges(configuracaoNFSeManual.GetCurrentChanges());
        }

        private void SalvarConfiguracaoConfiguracaoComissaoMotorista(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista repositorioComissaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista configuracaoComissaoMotorista = repositorioComissaoMotorista.BuscarConfiguracaoPadrao();

            configuracaoComissaoMotorista?.Initialize();
            configuracaoComissaoMotorista.Percentual = Request.GetDecimalParam("PercentualComissaoMotorista");
            configuracaoComissaoMotorista.PercentualDaBaseDeCalculo = Request.GetDecimalParam("PercentualBasecalculoComissaoMotorista");
            configuracaoComissaoMotorista.DataBase = Request.GetEnumParam<DataBaseComissaoMotorista>("DataBaseComissaoMotorista");
            configuracaoComissaoMotorista.UtilizaControlePercentualExecucao = Request.GetBoolParam("UtilizaControlePercentualExecucao");
            configuracaoComissaoMotorista.BloquearAlteracaoValorFreteLiquido = Request.GetBoolParam("BloquearAlteracaoValorFreteLiquido");

            repositorioComissaoMotorista.Atualizar(configuracaoComissaoMotorista);
            config.SetExternalChanges(configuracaoComissaoMotorista.GetCurrentChanges());
        }

        private void SalvarConfiguracaoInfracoes(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoInfracoes repositorioInfracoes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoInfracoes(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoInfracoes configuracaoInfracoes = repositorioInfracoes.BuscarConfiguracaoPadrao();

            configuracaoInfracoes?.Initialize();
            configuracaoInfracoes.FormulaInfracaoPadrao = Request.GetStringParam("FormulaInfracaoPadrao");

            repositorioInfracoes.Atualizar(configuracaoInfracoes);
            config.SetExternalChanges(configuracaoInfracoes.GetCurrentChanges());
        }

        private void SalvarConfiguracaoGeral(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            configuracaoGeral.Initialize();
            configuracaoGeral.NaoCarregarPlanoEntradaSaidaTipoPagamento = Request.GetBoolParam("NaoCarregarPlanoEntradaSaidaTipoPagamento");
            configuracaoGeral.AtivarConsultaSegregacaoPorEmpresa = Request.GetBoolParam("AtivarConsultaSegregacaoPorEmpresa");
            configuracaoGeral.PermitirVincularVeiculoMotoristaViaPlanilha = Request.GetBoolParam("PermitirVincularVeiculoMotoristaViaPlanilha");
            configuracaoGeral.PermitirCriacaoDiretaMalotes = Request.GetBoolParam("PermitirCriacaoDiretaMalotes");
            configuracaoGeral.TransformarJanelaDeDescarregamentoEmMultiplaSelecao = Request.GetBoolParam("TransformarJanelaDeDescarregamentoEmMultiplaSelecao");
            configuracaoGeral.ControlarOrganizacaoProdutos = Request.GetBoolParam("ControlarOrganizacaoProdutos");
            configuracaoGeral.FiltrarPedidosSemFiltroPorFilialNoPortalDoFornecedor = Request.GetBoolParam("FiltrarPedidosSemFiltroPorFilialNoPortalDoFornecedor");
            configuracaoGeral.AgruparRelatorioOrdemColetaGuaritaPorDestinatario = Request.GetBoolParam("AgruparRelatorioOrdemColetaGuaritaPorDestinatario");
            configuracaoGeral.ValidarTransportadorNaoInformadoNaImportacaoDocumento = Request.GetBoolParam("ValidarTransportadorNaoInformadoNaImportacaoDocumento");
            configuracaoGeral.PermitirAgendamentoPedidosSemCarga = Request.GetBoolParam("PermitirAgendamentoPedidosSemCarga");
            configuracaoGeral.NaoPermitirDesabilitarCompraValePedagioVeiculo = Request.GetBoolParam("NaoPermitirDesabilitarCompraValePedagioVeiculo");
            configuracaoGeral.ImprimeOrdemServiçoCNPJMatriz = Request.GetBoolParam("ImprimeOrdemServiçoCNPJMatriz");
            configuracaoGeral.AlterarModeloDocumentoNFSManual = Request.GetBoolParam("AlterarModeloDocumentoNFSManual");
            configuracaoGeral.NaoDescontarValorDescontoItemAosAbastecimentosGeradosDocumentoEntrada = Request.GetBoolParam("NaoDescontarValorDescontoItemAosAbastecimentosGeradosDocumentoEntrada");
            configuracaoGeral.EnviarApenasEmailDiarioTaxasDescargaPendenteAprovacao = Request.GetBoolParam("EnviarApenasEmailDiarioTaxasDescargaPendenteAprovacao");
            configuracaoGeral.HabilitarEnvioPorSMSDeDocumentos = Request.GetBoolParam("HabilitarEnvioPorSMSDeDocumentos");
            configuracaoGeral.NaoPermitirFinalizarViagemDetalhesFimViagem = Request.GetBoolParam("NaoPermitirFinalizarViagemDetalhesFimViagem");
            configuracaoGeral.PermitirAdicionarMotoristaCargaMDFeManual = Request.GetBoolParam("PermitirAdicionarMotoristaCargaMDFeManual");
            configuracaoGeral.HabilitarCadastroArmazem = Request.GetBoolParam("HabilitarCadastroArmazem");
            configuracaoGeral.NaoBloquearEmissaoNFSeManualSemDANFSE = Request.GetBoolParam("NaoBloquearEmissaoNFSeManualSemDANFSE");
            configuracaoGeral.TipoRomaneio = Request.GetEnumParam<TipoRomaneio>("TipoRomaneio");
            configuracaoGeral.GerarRegistrosReceberGNREParaCTesComCST90 = Request.GetBoolParam("GerarRegistrosReceberGNREParaCTesComCST90");
            configuracaoGeral.VisualizarPermitirAlterarDataEntregaNaConfirmacaoCanhoto = Request.GetBoolParam("VisualizarPermitirAlterarDataEntregaNaConfirmacaoCanhoto");
            configuracaoGeral.UtilizarLocalidadeTomadorNFSManual = Request.GetBoolParam("UtilizarLocalidadeTomadorNFSManual");
            configuracaoGeral.NaoGerarOcorrenciaCTeImportadosEmailEmbarcador = Request.GetBoolParam("NaoGerarOcorrenciaCTeImportadosEmailEmbarcador");
            configuracaoGeral.ProcessarXMLNotasFiscaisAssincrono = Request.GetBoolParam("ProcessarXMLNotasFiscaisAssincrono");
            configuracaoGeral.NaoPermitirCancelamentoNFSManualSeHouverIntegracao = Request.GetBoolParam("NaoPermitirCancelamentoNFSManualSeHouverIntegracao");
            configuracaoGeral.RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodo = Request.GetBoolParam("RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodo");
            configuracaoGeral.RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodoDias = Request.GetIntParam("RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodoDias");
            configuracaoGeral.PermitirImpressaoDAMDFEContingencia = Request.GetBoolParam("PermitirImpressaoDAMDFEContingencia");
            configuracaoGeral.HabilitarFuncionalidadesProjetoGollum = Request.GetBoolParam("HabilitarFuncionalidadesProjetoGollum");
            configuracaoGeral.EnviarCTeApenasParaTomador = Request.GetBoolParam("EnviarCTeApenasParaTomador");
            configuracaoGeral.PermiteRealizarConsultaVPUtilizandoModeloVeicularCarga = Request.GetBoolParam("PermiteRealizarConsultaVPUtilizandoModeloVeicularCarga");
            configuracaoGeral.PermiteSelecionarPlacaPorTipoVeiculoTransbordo = Request.GetBoolParam("PermiteSelecionarPlacaPorTipoVeiculoTransbordo");
            configuracaoGeral.VisualizarGNRESemValidacaoDocumentos = Request.GetBoolParam("VisualizarGNRESemValidacaoDocumentos");

            repConfiguracaoGeral.Atualizar(configuracaoGeral);
            config.SetExternalChanges(configuracaoGeral.GetCurrentChanges());
        }

        private void SalvarConfiguracaoArquivo(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo ConfiguracaoArquivo = repConfiguracaoArquivo.BuscarPrimeiroRegistro();
            ConfiguracaoArquivo.Initialize();

            ConfiguracaoArquivo.CaminhoRelatorios = Request.GetStringParam("arquivo_CaminhoRelatorios");
            ConfiguracaoArquivo.CaminhoTempArquivosImportacao = Request.GetStringParam("arquivo_CaminhoTempArquivosImportacao");
            ConfiguracaoArquivo.CaminhoCanhotos = Request.GetStringParam("arquivo_CaminhoCanhotos");
            ConfiguracaoArquivo.CaminhoCanhotosAvulsos = Request.GetStringParam("arquivo_CaminhoCanhotosAvulsos");
            ConfiguracaoArquivo.CaminhoXMLNotaFiscalComprovanteEntrega = Request.GetStringParam("arquivo_CaminhoXMLNotaFiscalComprovanteEntrega");
            ConfiguracaoArquivo.CaminhoArquivosIntegracao = Request.GetStringParam("arquivo_CaminhoArquivosIntegracao");
            ConfiguracaoArquivo.CaminhoRelatoriosEmbarcador = Request.GetStringParam("arquivo_CaminhoRelatoriosEmbarcador");
            ConfiguracaoArquivo.CaminhoLogoEmbarcador = Request.GetStringParam("arquivo_CaminhoLogoEmbarcador");
            ConfiguracaoArquivo.CaminhoDocumentosFiscaisEmbarcador = Request.GetStringParam("arquivo_CaminhoDocumentosFiscaisEmbarcador");
            ConfiguracaoArquivo.Anexos = Request.GetStringParam("arquivo_Anexos");
            ConfiguracaoArquivo.CaminhoGeradorRelatorios = Request.GetStringParam("arquivo_CaminhoGeradorRelatorios");
            ConfiguracaoArquivo.CaminhoArquivosEmpresas = Request.GetStringParam("arquivo_CaminhoArquivosEmpresas");
            ConfiguracaoArquivo.CaminhoRelatoriosCrystal = Request.GetStringParam("arquivo_CaminhoRelatoriosCrystal");
            ConfiguracaoArquivo.CaminhoRetornoXMLIntegrador = Request.GetStringParam("arquivo_CaminhoRetornoXMLIntegrador");
            ConfiguracaoArquivo.CaminhoArquivos = Request.GetStringParam("arquivo_CaminhoArquivos");
            ConfiguracaoArquivo.CaminhoArquivosIntegracaoEDI = Request.GetStringParam("arquivo_CaminhoArquivosIntegracaoEDI");
            ConfiguracaoArquivo.CaminhoArquivosImportacaoBoleto = Request.GetStringParam("arquivo_CaminhoArquivosImportacaoBoleto");
            ConfiguracaoArquivo.CaminhoOcorrencias = Request.GetStringParam("arquivo_CaminhoOcorrencias");
            ConfiguracaoArquivo.CaminhoOcorrenciasMobiles = Request.GetStringParam("arquivo_CaminhoOcorrenciasMobiles");
            ConfiguracaoArquivo.CaminhoArquivosImportacaoXMLNotaFiscal = Request.GetStringParam("arquivo_CaminhoArquivosImportacaoXMLNotaFiscal");
            ConfiguracaoArquivo.CaminhoDestinoXML = Request.GetStringParam("arquivo_CaminhoDestinoXML");
            ConfiguracaoArquivo.CaminhoCanhotosAntigos = Request.GetStringParam("arquivo_CaminhoCanhotosAntigos");
            ConfiguracaoArquivo.CaminhoRaiz = Request.GetStringParam("arquivo_CaminhoRaiz");
            ConfiguracaoArquivo.CaminhoGuia = Request.GetStringParam("arquivo_CaminhoGuia");
            ConfiguracaoArquivo.CaminhoDanfeSMS = Request.GetStringParam("arquivo_CaminhoDanfeSMS");
            ConfiguracaoArquivo.CaminhoRaizFTP = Request.GetStringParam("arquivo_CaminhoRaizFTP");

            repConfiguracaoArquivo.Atualizar(ConfiguracaoArquivo);
            config.SetExternalChanges(ConfiguracaoArquivo.GetCurrentChanges());
        }

        private void SalvarConfiguracaoAmbiente(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();
            configuracaoAmbiente.Initialize();

            configuracaoAmbiente.AmbienteProducao = Request.GetBoolParam("ambiente_AmbienteProducao");
            configuracaoAmbiente.AmbienteSeguro = Request.GetBoolParam("ambiente_AmbienteSeguro");
            configuracaoAmbiente.RecalcularICMSNaEmissaoCTe = Request.GetBoolParam("ambiente_RecalcularICMSNaEmissaoCTe");
            configuracaoAmbiente.AplicarValorICMSNoComplemento = Request.GetBoolParam("ambiente_AplicarValorICMSNoComplemento");
            configuracaoAmbiente.AdicionarCTesFilaConsulta = Request.GetBoolParam("ambiente_AdicionarCTesFilaConsulta");
            configuracaoAmbiente.NaoCalcularDIFALParaCSTNaoTributavel = Request.GetBoolParam("ambiente_NaoCalcularDIFALParaCSTNaoTributavel");
            configuracaoAmbiente.NaoUtilizarColetaNaBuscaRotaFrete = Request.GetBoolParam("ambiente_NaoUtilizarColetaNaBuscaRotaFrete");
            configuracaoAmbiente.OcultarConteudoColog = Request.GetBoolParam("ambiente_OcultarConteudoColog");
            configuracaoAmbiente.ConsultarPeloCustoDaRota = Request.GetBoolParam("ambiente_ConsultarPeloCustoDaRota");
            configuracaoAmbiente.CalcularHorarioDoCarregamento = Request.GetBoolParam("ambiente_CalcularHorarioDoCarregamento");
            configuracaoAmbiente.EnviarTodasNotificacoesPorEmail = Request.GetBoolParam("ambiente_EnviarTodasNotificacoesPorEmail");
            configuracaoAmbiente.CalcularFreteFechamento = Request.GetBoolParam("ambiente_CalcularFreteFechamento");
            configuracaoAmbiente.GerarDocumentoFechamento = Request.GetBoolParam("ambiente_GerarDocumentoFechamento");
            configuracaoAmbiente.NovoLayoutPortalFornecedor = Request.GetBoolParam("ambiente_NovoLayoutPortalFornecedor");
            configuracaoAmbiente.NovoLayoutCabotagem = Request.GetBoolParam("ambiente_NovoLayoutCabotagem");
            configuracaoAmbiente.UtilizarIntegracaoSaintGobainNova = Request.GetBoolParam("ambiente_UtilizarIntegracaoSaintGobainNova");
            configuracaoAmbiente.FiltrarCargasPorProprietario = Request.GetBoolParam("ambiente_FiltrarCargasPorProprietario");
            configuracaoAmbiente.CargaControleEntrega_Habilitar_ImportacaoCargaFluvial = Request.GetBoolParam("ambiente_CargaControleEntrega_Habilitar_ImportacaoCargaFluvial");
            configuracaoAmbiente.FTPPassivo = Request.GetBoolParam("ambiente_FTPPassivo");
            configuracaoAmbiente.UtilizaSFTP = Request.GetBoolParam("ambiente_UtilizaSFTP");
            configuracaoAmbiente.GerarNotFisPorNota = Request.GetBoolParam("ambiente_GerarNotFisPorNota");
            configuracaoAmbiente.UtilizarMetodoImportacaoTabelaFretePorServico = Request.GetBoolParam("ambiente_UtilizarMetodoImportacaoTabelaFretePorServico");
            configuracaoAmbiente.UtilizarLayoutImportacaoTabelaFreteGPA = Request.GetBoolParam("ambiente_UtilizarLayoutImportacaoTabelaFreteGPA");
            configuracaoAmbiente.ExibirSituacaoIntegracaoXMLGPA = Request.GetBoolParam("ambiente_ExibirSituacaoIntegracaoXMLGPA");
            configuracaoAmbiente.ProcessarCTeMultiCTe = Request.GetBoolParam("ambiente_ProcessarCTeMultiCTe");
            configuracaoAmbiente.NaoUtilizarCNPJTransportador = Request.GetBoolParam("ambiente_NaoUtilizarCNPJTransportador");
            configuracaoAmbiente.BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe = Request.GetBoolParam("ambiente_BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe");
            configuracaoAmbiente.SempreUsarAtividadeCliente = Request.GetBoolParam("ambiente_SempreUsarAtividadeCliente");
            configuracaoAmbiente.AtualizarFantasiaClienteIntegracaoCTe = Request.GetBoolParam("ambiente_AtualizarFantasiaClienteIntegracaoCTe");
            configuracaoAmbiente.CadastrarMotoristaIntegracaoCTe = Request.GetBoolParam("ambiente_CadastrarMotoristaIntegracaoCTe");
            configuracaoAmbiente.CTeUtilizaProprietarioCadastro = Request.GetBoolParam("ambiente_CTeUtilizaProprietarioCadastro");
            configuracaoAmbiente.CTeCarregarVinculosVeiculosCadastro = Request.GetBoolParam("ambiente_CTeCarregarVinculosVeiculosCadastro");
            configuracaoAmbiente.CTeAtualizaTipoVeiculo = Request.GetBoolParam("ambiente_CTeAtualizaTipoVeiculo");
            configuracaoAmbiente.NaoAtualizarCadastroVeiculo = Request.GetBoolParam("ambiente_NaoAtualizarCadastroVeiculo");
            configuracaoAmbiente.AgruparQuantidadesImportacaoCTe = Request.GetBoolParam("ambiente_AgruparQuantidadesImportacaoCTe");
            configuracaoAmbiente.EncerraMDFeAutomatico = Request.GetBoolParam("ambiente_EncerraMDFeAutomatico");
            configuracaoAmbiente.EnviaContingenciaMDFeAutomatico = Request.GetBoolParam("ambiente_EnviaContingenciaMDFeAutomatico");
            configuracaoAmbiente.EnviarCertificadoOracle = Request.GetBoolParam("ambiente_EnviarCertificadoOracle");
            configuracaoAmbiente.EnviarCertificadoKeyVault = Request.GetBoolParam("ambiente_EnviarCertificadoKeyVault");
            configuracaoAmbiente.PermitirInformarCapacidadeMaximaParaUploadArquivos = Request.GetBoolParam("ambiente_PermitirInformarCapacidadeMaximaParaUploadArquivos");

            configuracaoAmbiente.IdentificacaoAmbiente = Request.GetStringParam("ambiente_IdentificacaoAmbiente");
            configuracaoAmbiente.CodigoLocalidadeNaoCadastrada = Request.GetStringParam("ambiente_CodigoLocalidadeNaoCadastrada");
            configuracaoAmbiente.CodificacaoEDI = Request.GetStringParam("ambiente_CodificacaoEDI");
            configuracaoAmbiente.LinkCotacaoCompra = Request.GetStringParam("ambiente_LinkCotacaoCompra");
            configuracaoAmbiente.LogoPersonalizadaFornecedor = Request.GetStringParam("ambiente_LogoPersonalizadaFornecedor");
            configuracaoAmbiente.LayoutPersonalizadoFornecedor = Request.GetStringParam("ambiente_LayoutPersonalizadoFornecedor");
            configuracaoAmbiente.ConcessionariasComDescontos = Request.GetStringParam("ambiente_ConcessionariasComDescontos");
            configuracaoAmbiente.PercentualDescontoConcessionarias = Request.GetStringParam("ambiente_PercentualDescontoConcessionarias");
            configuracaoAmbiente.PlacaPadraoConsultaValorPedagio = Request.GetStringParam("ambiente_PlacaPadraoConsultaValorPedagio");
            configuracaoAmbiente.APIOCRLink = Request.GetStringParam("ambiente_APIOCRLink");
            configuracaoAmbiente.APIOCRKey = Request.GetStringParam("ambiente_APIOCRKey");
            configuracaoAmbiente.QuantidadeSelecaoAgrupamentoCargaAutomatico = Request.GetStringParam("ambiente_QuantidadeSelecaoAgrupamentoCargaAutomatico");
            configuracaoAmbiente.QuantidadeCargasAgrupamentoCargaAutomatico = Request.GetStringParam("ambiente_QuantidadeCargasAgrupamentoCargaAutomatico");
            configuracaoAmbiente.HorarioExecucaoThreadDiaria = Request.GetStringParam("ambiente_HorarioExecucaoThreadDiaria");
            configuracaoAmbiente.FornecedorTMS = Request.GetStringParam("ambiente_FornecedorTMS");
            configuracaoAmbiente.TipoArmazenamento = Request.GetStringParam("ambiente_TipoArmazenamento");
            configuracaoAmbiente.TipoArmazenamentoLeitorOCR = Request.GetStringParam("ambiente_TipoArmazenamentoLeitorOCR");
            configuracaoAmbiente.EnderecoFTP = Request.GetStringParam("ambiente_EnderecoFTP");
            configuracaoAmbiente.UsuarioFTP = Request.GetStringParam("ambiente_UsuarioFTP");
            configuracaoAmbiente.SenhaFTP = Request.GetStringParam("ambiente_SenhaFTP");
            configuracaoAmbiente.PortaFTP = Request.GetStringParam("ambiente_PortaFTP");
            configuracaoAmbiente.PrefixosFTP = Request.GetStringParam("ambiente_PrefixosFTP");
            configuracaoAmbiente.EmailsFTP = Request.GetStringParam("ambiente_EmailsFTP");
            configuracaoAmbiente.CodigoEmpresaMultisoftware = Request.GetStringParam("ambiente_CodigoEmpresaMultisoftware");
            configuracaoAmbiente.MinutosParaConsultaNatura = Request.GetStringParam("ambiente_MinutosParaConsultaNatura");
            configuracaoAmbiente.FiliaisNatura = Request.GetStringParam("ambiente_FiliaisNatura");
            configuracaoAmbiente.WebServiceConsultaCTe = Request.GetStringParam("ambiente_WebServiceConsultaCTe");

            configuracaoAmbiente.LimparMotoristaIntegracaoVeiculo = Request.GetBoolParam("ambiente_LimparMotoristaIntegracaoVeiculo");
            configuracaoAmbiente.LoginAD = Request.GetBoolParam("ambiente_LoginAD");
            configuracaoAmbiente.RegerarDACTEOracle = Request.GetBoolParam("ambiente_RegerarDACTEOracle");
            configuracaoAmbiente.ReenviarErroIntegracaoCTe = Request.GetBoolParam("ambiente_ReenviarErroIntegracaoCTe");
            configuracaoAmbiente.AtualizarTipoEmpresa = Request.GetBoolParam("ambiente_AtualizarTipoEmpresa");
            configuracaoAmbiente.ValidarNFeJaImportada = Request.GetBoolParam("ambiente_ValidarNFeJaImportada");
            configuracaoAmbiente.UtilizaOptanteSimplesNacionalDaIntegracao = Request.GetBoolParam("ambiente_UtilizaOptanteSimplesNacionalDaIntegracao");
            configuracaoAmbiente.ReenviarErroIntegracaoMDFe = Request.GetBoolParam("ambiente_ReenviarErroIntegracaoMDFe");
            configuracaoAmbiente.EncerraMDFeAutomaticoComMesmaData = Request.GetBoolParam("ambiente_EncerraMDFeAutomaticoComMesmaData");
            configuracaoAmbiente.EncerraMDFeAntesDaEmissao = Request.GetBoolParam("ambiente_EncerraMDFeAntesDaEmissao");
            configuracaoAmbiente.EncerraMDFeAutomaticoOutrosSistemas = Request.GetBoolParam("ambiente_EncerraMDFeAutomaticoOutrosSistemas");
            configuracaoAmbiente.EnviarEmailMDFeClientes = Request.GetBoolParam("ambiente_EnviarEmailMDFeClientes");
            configuracaoAmbiente.UtilizarDocaDoComplementoFilial = Request.GetBoolParam("ambiente_UtilizarDocaDoComplementoFilial");
            configuracaoAmbiente.RetornarModeloVeiculo = Request.GetBoolParam("ambiente_RetornarModeloVeiculo");
            configuracaoAmbiente.MDFeUtilizaDadosVeiculoCadastro = Request.GetBoolParam("ambiente_MDFeUtilizaDadosVeiculoCadastro");
            configuracaoAmbiente.MDFeUtilizaVeiculoReboqueComoTracao = Request.GetBoolParam("ambiente_MDFeUtilizaVeiculoReboqueComoTracao");
            configuracaoAmbiente.GerarCTeDasNFSeAutorizadas = Request.GetBoolParam("ambiente_GerarCTeDasNFSeAutorizadas");
            configuracaoAmbiente.IncluirISSNFSeLocalidadeTomadorDiferentePrestador = Request.GetBoolParam("ambiente_IncluirISSNFSeLocalidadeTomadorDiferentePrestador");
            configuracaoAmbiente.IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples = Request.GetBoolParam("ambiente_IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples");
            configuracaoAmbiente.AtualizarValorFrete_AtualizarICMS = Request.GetBoolParam("ambiente_AtualizarValorFrete_AtualizarICMS");
            configuracaoAmbiente.ConsultarDuplicidadeOracle = Request.GetBoolParam("ambiente_ConsultarDuplicidadeOracle");
            configuracaoAmbiente.EnviarIntegracaoMagalogNoRetorno = Request.GetBoolParam("ambiente_EnviarIntegracaoMagalogNoRetorno");
            configuracaoAmbiente.EnviarIntegracaoErroMDFeMagalog = Request.GetBoolParam("ambiente_EnviarIntegracaoErroMDFeMagalog");
            configuracaoAmbiente.WebServiceConsultaCTe = Request.GetStringParam("ambiente_WebServiceConsultaCTe");
            configuracaoAmbiente.EmpresasUsuariosMultiCTe = Request.GetStringParam("ambiente_EmpresasUsuariosMultiCTe");
            configuracaoAmbiente.PesoMaximoIntegracaoCarga = Request.GetDecimalParam("ambiente_PesoMaximoIntegracaoCarga");
            configuracaoAmbiente.CapacidadeMaximaParaUploadArquivos = Request.GetIntParam("ambiente_CapacidadeMaximaParaUploadArquivos");
            configuracaoAmbiente.TipoProtocolo = Request.GetEnumParam<TipoProtocolo>("ambiente_TipoProtocolo");
            configuracaoAmbiente.DesabilitarPopUpsDeNotificacao = Request.GetBoolParam("ambiente_DesabilitarPopUpsDeNotificacao");

            repConfiguracaoAmbiente.Atualizar(configuracaoAmbiente);
            config.SetExternalChanges(configuracaoAmbiente.GetCurrentChanges());
        }

        private void SalvarConfiguracaoTransportador(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repositorioConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repositorioConfiguracaoTransportador.BuscarConfiguracaoPadrao();

            configuracaoTransportador.Initialize();
            configuracaoTransportador.PermitirCadastrarTransportadorInformacoesMinimas = Request.GetBoolParam("PermitirCadastrarTransportadorInformacoesMinimas");
            configuracaoTransportador.PermitirInformarEmpresaFavorecidaNosDadosBancarios = Request.GetBoolParam("PermitirInformarEmpresaFavorecidaNosDadosBancarios");
            configuracaoTransportador.AtivarControleCarregamentoNavio = Request.GetBoolParam("AtivarControleCarregamentoNavio");
            configuracaoTransportador.ExigirRetencaoISSQuandoMunicipioPrestacaoForDiferenteTransportador = Request.GetBoolParam("ExigirRetencaoISSQuandoMunicipioPrestacaoForDiferenteTransportador");
            configuracaoTransportador.PermitirTransportadorRetornarEtapaNFe = Request.GetBoolParam("PermitirTransportadorRetornarEtapaNFe");
            configuracaoTransportador.NaoAtualizarNomeFantasiaClienteAlterarDadosTransportador = Request.GetBoolParam("NaoAtualizarNomeFantasiaClienteAlterarDadosTransportador");
            configuracaoTransportador.ExisteTransportadorPadraoContratacao = Request.GetBoolParam("ExisteTransportadorPadraoContratacao");
            configuracaoTransportador.EnviarEmailDocumentoRejeitadoAuditoriaFrete = Request.GetBoolParam("EnviarEmailDocumentoRejeitadoAuditoriaFrete");
            configuracaoTransportador.NaoGerarAutomaticamenteUsuarioAcessoPortalTransportador = Request.GetBoolParam("NaoGerarAutomaticamenteUsuarioAcessoPortalTransportador");
            configuracaoTransportador.NaoHabilitarDetalhesCarga = Request.GetBoolParam("NaoHabilitarDetalhesCarga");
            configuracaoTransportador.NotificarTransportadorProcessoShareRotas = Request.GetBoolParam("NotificarTransportadorProcessoShareRotas");
            configuracaoTransportador.HabilitarSpotCargaAposLimiteHoras = Request.GetBoolParam("HabilitarSpotCargaAposLimiteHoras");
            configuracaoTransportador.TransportadorPadraoContratacao = repositorioEmpresa.BuscarPorCodigo(Request.GetIntParam("TransportadorPadraoContratacao"));

            repositorioConfiguracaoTransportador.Atualizar(configuracaoTransportador);
            config.SetExternalChanges(configuracaoTransportador.GetCurrentChanges());
        }

        private void SalvarConfiguracaoRoteirizacao(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

            configuracaoRoteirizacao.Initialize();
            configuracaoRoteirizacao.OrdenarLocalidades = Request.GetBoolParam("OrdenarLocalidades");
            configuracaoRoteirizacao.NaoCalcularTempoDeViagemAutomatico = Request.GetBoolParam("NaoCalcularTempoDeViagemAutomatico");
            configuracaoRoteirizacao.ColetasSempreInicioRotaOrdenadaCliente = Request.GetBoolParam("ColetasSempreInicioRotaOrdenadaCliente");
            configuracaoRoteirizacao.CadastrarNovaRotaDeveSerParaTipoOperacaoCarga = Request.GetBoolParam("CadastrarNovaRotaDeveSerParaTipoOperacaoCarga");
            configuracaoRoteirizacao.NumeroDiasParaConsultaPracaPedagio = Request.GetIntParam("NumeroDiasParaConsultaPracaPedagio");
            configuracaoRoteirizacao.SempreUtilizarRotaParaBuscarPracasPedagio = Request.GetBoolParam("SempreUtilizarRotaParaBuscarPracasPedagio");
            configuracaoRoteirizacao.IgnorarOutroEnderecoPedidoComRecebedor = Request.GetBoolParam("IgnorarOutroEnderecoPedidoComRecebedor");

            repositorioConfiguracaoRoteirizacao.Atualizar(configuracaoRoteirizacao);
            config.SetExternalChanges(configuracaoRoteirizacao.GetCurrentChanges());
        }

        private void SalvarConfiguracaoFilaCarregamento(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento repositorioConfiguracaoFilaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento configuracaoFilaCarregamento = repositorioConfiguracaoFilaCarregamento.BuscarPrimeiroRegistro();

            configuracaoFilaCarregamento.Initialize();
            configuracaoFilaCarregamento.DiasFiltrarDataProgramada = Request.GetIntParam("DiasFiltrarDataProgramada");
            configuracaoFilaCarregamento.NaoPermitirAdicionarVeiculoEmMaisDeUmaFilaCarregamentoSimultaneamente = Request.GetBoolParam("NaoPermitirAdicionarVeiculoEmMaisDeUmaFilaCarregamentoSimultaneamente");
            configuracaoFilaCarregamento.InformarAreaCDAdicionarVeiculo = Request.GetBoolParam("InformarAreaCDAdicionarVeiculo");
            configuracaoFilaCarregamento.PermiteAvancarPrimeiraEtapaCargaAoAlocarDadosTransportePelaFilaCarregamento = Request.GetBoolParam("PermiteAvancarPrimeiraEtapaCargaAoAlocarDadosTransportePelaFilaCarregamento");
            configuracaoFilaCarregamento.AtualizarFilaCarregamentoAoAlterarDadosTransporteNaCarga = Request.GetBoolParam("AtualizarFilaCarregamentoAoAlterarDadosTransporteNaCarga");

            repositorioConfiguracaoFilaCarregamento.Atualizar(configuracaoFilaCarregamento);
            config.SetExternalChanges(configuracaoFilaCarregamento.GetCurrentChanges());
        }

        private void SalvarConfiguracaoPortalMultiClifor(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor repositorioConfiguracaoPortalMultiClifor = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor configuracaoPortalMultiClifor = repositorioConfiguracaoPortalMultiClifor.BuscarConfiguracaoPadrao();

            configuracaoPortalMultiClifor.Initialize();
            configuracaoPortalMultiClifor.MostrarNoAcompanhamentoDePedidosDeslocamentoVazio = Request.GetBoolParam("MostrarNoAcompanhamentoDePedidosDeslocamentoVazio");
            configuracaoPortalMultiClifor.FiltrarPedidosPorRemetenteRetiradaProduto = Request.GetBoolParam("FiltrarPedidosPorRemetenteRetiradaProduto");
            configuracaoPortalMultiClifor.CodigoReportMenuBI = Request.GetNullableIntParam("CodigoReportMenuBI");
            configuracaoPortalMultiClifor.HabilitarAcessoTodosClientes = Request.GetBoolParam("HabilitarAcessoTodosClientes");
            configuracaoPortalMultiClifor.SenhaPadraoAcessoPortal = Request.GetStringParam("SenhaPadraoAcessoPortal");
            configuracaoPortalMultiClifor.DesabilitarIconeNotificacao = Request.GetBoolParam("DesabilitarIconeNotificacao");
            configuracaoPortalMultiClifor.DesabilitarFiltrosBI = Request.GetBoolParam("DesabilitarFiltrosBI");

            repositorioConfiguracaoPortalMultiClifor.Atualizar(configuracaoPortalMultiClifor);
            if (configuracaoPortalMultiClifor.HabilitarAcessoTodosClientes)
            {
                repCliente.HabilitarAcessoFornecedorParaTodosClientes();
            }
            configuracao.SetExternalChanges(configuracaoPortalMultiClifor.GetCurrentChanges());
        }

        private void SalvarConfiguracaoBidding(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding repositorioConfiguracaoBidding = new Repositorio.Embarcador.Configuracoes.ConfiguracaoBidding(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoBidding configuracaoBidding = repositorioConfiguracaoBidding.BuscarConfiguracaoPadrao();

            configuracaoBidding.Initialize();
            configuracaoBidding.CalcularKMMedioRotaPorOrigemDestino = Request.GetBoolParam("CalcularKMMedioRotaPorOrigemDestino");
            configuracaoBidding.PermiteAdicionarRotaSemInformarKMMedio = Request.GetBoolParam("PermiteAdicionarRotaSemInformarKMMedio");
            configuracaoBidding.PermiteSelecionarMaisDeUmaOfertaPorBidding = Request.GetBoolParam("PermiteSelecionarMaisDeUmaOfertaPorBidding");
            configuracaoBidding.PermiteRemoverObrigatoriedadeDatas = Request.GetBoolParam("PermiteRemoverObrigatoriedadeDatas");
            configuracaoBidding.TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding = Request.GetBoolParam("TransportadorUtilizaProcessoAutomatizadoAvancoEtapasBidding");
            configuracaoBidding.PermiteOfertarQuandoAceitacaoIndForMenorCemPorcento = Request.GetBoolParam("PermiteOfertarQuandoAceitacaoIndForMenorCemPorcento");
            configuracaoBidding.InformePorcentagemAceitacaoInd = Request.GetIntParam("InformePorcentagemAceitacaoInd");

            repositorioConfiguracaoBidding.Atualizar(configuracaoBidding);
            config.SetExternalChanges(configuracaoBidding.GetCurrentChanges());
        }

        private void SalvarConfiguracaoAbastecimento(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos repositorioConfiguracaoAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAbastecimentos(unitOfWork);
            Repositorio.Embarcador.Compras.MotivoCompra repositorioMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAbastecimentos configuracaoAbastecimento = repositorioConfiguracaoAbastecimento.BuscarConfiguracaoPadrao();

            int codigoMotivoCompra = Request.GetIntParam("MotivoCompraAbastecimento");

            configuracaoAbastecimento.Initialize();
            configuracaoAbastecimento.GerarRequisicaoAutomaticaParaVeiculosVinculados = Request.GetBoolParam("GerarRequisicaoAutomaticaParaVeiculosVinculados");
            configuracaoAbastecimento.UtilizarCustoMedioParaLancamentoAbastecimentos = Request.GetBoolParam("UtilizarCustoMedioParaLancamentoAbastecimentos");
            configuracaoAbastecimento.MotivoCompraAbastecimento = codigoMotivoCompra > 0 ? repositorioMotivoCompra.BuscarPorCodigo(codigoMotivoCompra) : null;

            repositorioConfiguracaoAbastecimento.Atualizar(configuracaoAbastecimento);
            config.SetExternalChanges(configuracaoAbastecimento.GetCurrentChanges());
        }

        private void SalvarConfiguracaoPessoa(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa repositorioConfiguracaoPessoa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa configuracaoPessoa = repositorioConfiguracaoPessoa.BuscarConfiguracaoPadrao();

            configuracaoPessoa.Initialize();

            configuracaoPessoa.PermitirCadastroDeTelefoneInternacional = Request.GetBoolParam("PermitirCadastroDeTelefoneInternacional");
            configuracaoPessoa.ExigeQueSuasEntregasSejamAgendadas = Request.GetBoolParam("ExigeQueSuasEntregasSejamAgendadas");
            configuracaoPessoa.NaoEnviarXMLCTEPorEmailParaTipoServico = Request.GetBoolParam("NaoEnviarXMLCTEPorEmailParaTipoServico");
            configuracaoPessoa.NaoExigirTrocaDeSenhaCasoCadastroPorIntegracao = Request.GetBoolParam("NaoExigirTrocaDeSenhaCasoCadastroPorIntegracao");

            List<Dominio.Enumeradores.TipoServico> tiposServicoCTe = Request.GetListEnumParam<Dominio.Enumeradores.TipoServico>("TipoServicoCTeEmail");

            if (configuracaoPessoa.TiposServicosCTe == null)
                configuracaoPessoa.TiposServicosCTe = new List<Dominio.Enumeradores.TipoServico>();
            else
                configuracaoPessoa.TiposServicosCTe.Clear();

            foreach (var tipoServico in tiposServicoCTe)
                configuracaoPessoa.TiposServicosCTe.Add(tipoServico);

            repositorioConfiguracaoPessoa.Atualizar(configuracaoPessoa);
            config.SetExternalChanges(configuracaoPessoa.GetCurrentChanges());
        }

        private void SalvarConfiguracaoRelatorio(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio repositorioConfiguracaoRelatorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio = repositorioConfiguracaoRelatorio.BuscarConfiguracaoPadrao();

            configuracaoRelatorio.Initialize();

            config.QuantidadeMaximaDiasRelatorios = Request.GetIntParam("QuantidadeMaximaDiasRelatorios");
            config.QuantidadeMaximaRegistrosRelatorios = Request.GetIntParam("QuantidadeMaximaRegistrosRelatorios");
            config.UtilizarDadosCargaRelatorioPedido = Request.GetBoolParam("UtilizarDadosCargaRelatorioPedido");
            config.HabilitarHoraFiltroDataInicialFinalRelatorioCargas = Request.GetBoolParam("HabilitarHoraFiltroDataInicialFinalRelatorioCargas");
            config.UtilizarExportacaoRelatorioCSV = Request.GetBoolParam("UtilizarExportacaoRelatorioCSV");
            config.ExportarCNPJEChaveDeAcessoFormatado = Request.GetBoolParam("ExportarCNPJEChaveDeAcessoFormatado");
            config.InformacaoAdicionalMotoristaOrdemColeta = Request.GetBoolParam("InformacaoAdicionalMotoristaOrdemColeta");
            config.NaoExibirValorFreteCTeComplementarRelatorioCTe = Request.GetBoolParam("NaoExibirValorFreteCTeComplementarRelatorioCTe");
            configuracaoRelatorio.ExibirTodasCargasNoRelatorioDeValePedagio = Request.GetBoolParam("ExibirTodasCargasNoRelatorioDeValePedagio");
            configuracaoRelatorio.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes = Request.GetBoolParam("RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes");

            repositorioConfiguracaoRelatorio.Atualizar(configuracaoRelatorio);
            config.SetExternalChanges(configuracaoRelatorio.GetCurrentChanges());
        }

        private void SalvarConfiguracaoConfiguracaoEnvioEmailCobranca(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca repositorioConfiguracaoEnvioEmailCobranca = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEnvioEmailCobranca configuracaoEnvioEmailCobranca = repositorioConfiguracaoEnvioEmailCobranca.BuscarConfiguracaoPadrao();

            configuracaoEnvioEmailCobranca.Initialize();

            configuracaoEnvioEmailCobranca.AvisoVencimetoEnvarEmail = Request.GetBoolParam("AvisoVencimetoEnvarEmail");
            configuracaoEnvioEmailCobranca.AvisoVencimetoQunatidadeDias = Request.GetIntParam("AvisoVencimetoQunatidadeDias");
            configuracaoEnvioEmailCobranca.AvisoVencimetoEnviarDiariamente = Request.GetBoolParam("AvisoVencimetoEnviarDiariamente");
            configuracaoEnvioEmailCobranca.AvisoVencimetoAssunto = Request.GetNullableStringParam("AvisoVencimetoAssunto");
            configuracaoEnvioEmailCobranca.AvisoVencimetoMensagem = Request.GetNullableStringParam("AvisoVencimetoMensagem");
            configuracaoEnvioEmailCobranca.CobrancaEnvarEmail = Request.GetBoolParam("CobrancaEnvarEmail");
            configuracaoEnvioEmailCobranca.CobrancaQunatidadeDias = Request.GetIntParam("CobrancaQunatidadeDias");
            configuracaoEnvioEmailCobranca.CobrancaAssunto = Request.GetNullableStringParam("CobrancaAssunto");
            configuracaoEnvioEmailCobranca.CobrancaMensagem = Request.GetNullableStringParam("CobrancaMensagem");

            repositorioConfiguracaoEnvioEmailCobranca.Atualizar(configuracaoEnvioEmailCobranca);
            config.SetExternalChanges(configuracaoEnvioEmailCobranca.GetCurrentChanges());
        }

        private void SalvarConfiguracaoMongo(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMongo repositorioConfiguracaoMongo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMongo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMongo configuracaoMongo = repositorioConfiguracaoMongo.BuscarPrimeiroRegistro();

            configuracaoMongo.Initialize();

            configuracaoMongo.Banco = Request.GetStringParam("BancoMongo");
            configuracaoMongo.Servidor = Request.GetStringParam("ServidorMongo");
            configuracaoMongo.Porta = Request.GetIntParam("PortaMongo");
            configuracaoMongo.UsaTls = Request.GetBoolParam("UsaTlsMongo");
            configuracaoMongo.UtilizaCosmosDb = Request.GetBoolParam("UtilizaCosmosDbMongo");
            configuracaoMongo.Timeout = Request.GetIntParam("TimeoutMongo");
            configuracaoMongo.UsuarioHangfire = Request.GetStringParam("UsuarioHangfireMongo");
            configuracaoMongo.SenhaHangfire = Request.GetStringParam("SenhaHangfireMongo");
            configuracaoMongo.Usuario = Request.GetStringParam("UsuarioMongo");
            configuracaoMongo.Senha = Request.GetStringParam("SenhaMongo");

            repositorioConfiguracaoMongo.Atualizar(configuracaoMongo);
            config.SetExternalChanges(configuracaoMongo.GetCurrentChanges());
        }

        private void SalvarConfiguracaoSSo(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO repositorioConfiguracaoSSo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO configuracaoSSo = repositorioConfiguracaoSSo.BuscarPrimeiroRegistro();

            if (configuracaoSSo == null)
                configuracaoSSo = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO();
            else
                configuracaoSSo.Initialize();

            configuracaoSSo.Ativo = Request.GetBoolParam("AtivoSSo");
            configuracaoSSo.ClientId = Request.GetStringParam("ClientIdSSo");
            configuracaoSSo.ClientSecret = Request.GetStringParam("ClientSecretSSo");
            configuracaoSSo.Display = Request.GetStringParam("DisplaySSo");
            configuracaoSSo.TipoSSo = Request.GetEnumParam<Dominio.Enumeradores.TipoSso>("TipoSSo");
            configuracaoSSo.UrlAccessToken = Request.GetStringParam("UrlAccessTokenSSo");
            configuracaoSSo.UrlAutenticacao = Request.GetStringParam("UrlAutenticacaoSSo");
            configuracaoSSo.UrlRefreshToken = Request.GetStringParam("UrlRefreshTokenSSo");
            configuracaoSSo.UrlRevokeToken = Request.GetStringParam("UrlRevokeTokenSSo");
            configuracaoSSo.UrlDominio = Request.GetStringParam("UrlDominioSSo");

            if (configuracaoSSo.Codigo == 0)
                repositorioConfiguracaoSSo.Inserir(configuracaoSSo);
            else
            {
                repositorioConfiguracaoSSo.Atualizar(configuracaoSSo);
                config.SetExternalChanges(configuracaoSSo.GetCurrentChanges());
            }
        }

        private void SalvarConfiguracaoMercosul(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMercosul repositorioConfiguracaoMercosul = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMercosul(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMercosul configuracaoMercosul = repositorioConfiguracaoMercosul.BuscarPrimeiroRegistro();

            configuracaoMercosul.Initialize();

            configuracaoMercosul.UtilizarMesmoNumeroCRTCancelamentos = Request.GetBoolParam("UtilizarMesmoNumeroCRTCancelamentos");
            configuracaoMercosul.UtilizarMesmoNumeroMICDTACancelamentos = Request.GetBoolParam("UtilizarMesmoNumeroMICDTACancelamentos");
            configuracaoMercosul.UtilizarCRTAverbacao = Request.GetBoolParam("UtilizarCRTAverbacao");

            repositorioConfiguracaoMercosul.Atualizar(configuracaoMercosul);
            config.SetExternalChanges(configuracaoMercosul.GetCurrentChanges());
        }

        private void SalvarConfiguracaoCotacao(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCotacao repConfiguracaoCotacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCotacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCotacao configuracaoCotacao = repConfiguracaoCotacao.BuscarConfiguracaoPadrao();

            configuracaoCotacao.Initialize();
            configuracaoCotacao.GravarNumeroCotacaoObservacaoInternaAoCriarPedido = Request.GetBoolParam("GravarNumeroCotacaoObservacaoInternaAoCriarPedido");

            repConfiguracaoCotacao.Atualizar(configuracaoCotacao);
            config.SetExternalChanges(configuracaoCotacao.GetCurrentChanges());
        }

        private void SalvarConfiguracaoAgendamentoEntrega(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega repositorioConfiguracaoAgendamentoEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega configuracaoAgendamentoEntrega = repositorioConfiguracaoAgendamentoEntrega.BuscarPrimeiroRegistro();

            configuracaoAgendamentoEntrega.Initialize();

            configuracaoAgendamentoEntrega.VisualizarTelaDeAgendamentoPorEntrega = Request.GetBoolParam("VisualizarTelaDeAgendamentoPorEntrega");
            configuracaoAgendamentoEntrega.PermiteInformarDataDeAgendamentoEReagendamentoRetroativamente = Request.GetBoolParam("PermiteInformarDataDeAgendamentoEReagendamentoRetroativamente");

            repositorioConfiguracaoAgendamentoEntrega.Atualizar(configuracaoAgendamentoEntrega);
            config.SetExternalChanges(configuracaoAgendamentoEntrega.GetCurrentChanges());
        }

        private void SalvarConfiguracaoAtendimentoAutomaticoDivergenciaValorFreteCTeEmitidosEmbarcador(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAtendimentoAutomatico repositorioConfiguracaoAtendimentoAutomatico = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAtendimentoAutomatico(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAtendimentoAutomatico configuracaoAtendimentoAutomatico = repositorioConfiguracaoAtendimentoAutomatico.BuscarPrimeiroRegistro();
            Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

            configuracaoAtendimentoAutomatico.Initialize();

            configuracaoAtendimentoAutomatico.GerarAtendimentoDivergenciaValorTabelaCTeEmitidoEmbarcador = Request.GetBoolParam("GerarAtendimentoDivergenciaValoresCTeEmitidoEmbarcador");

            int codigoMotivoChamado = Request.GetIntParam("MotivoAtendimentoDivergenciaValoresCTeEmitidoEmbarcador");

            if (codigoMotivoChamado > 0)
                configuracaoAtendimentoAutomatico.MotivoChamado = repMotivoChamado.BuscarPorCodigo(codigoMotivoChamado);
            else
                configuracaoAtendimentoAutomatico.MotivoChamado = null;

            repositorioConfiguracaoAtendimentoAutomatico.Atualizar(configuracaoAtendimentoAutomatico);
            config.SetExternalChanges(configuracaoAtendimentoAutomatico.GetCurrentChanges());
        }

        private void SalvarConfiguracaoAPIConexaoGeradorExcelKMM(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM repositorioConfiguracaoAPIConexaoGeradorExcelKMM = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM configuracaoAPIConexaoGeradorExcelKMM = repositorioConfiguracaoAPIConexaoGeradorExcelKMM.BuscarPrimeiroRegistro();

            if (configuracaoAPIConexaoGeradorExcelKMM == null)
                configuracaoAPIConexaoGeradorExcelKMM = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAPIConexaoGeradorExcelKMM();
            else
                configuracaoAPIConexaoGeradorExcelKMM.Initialize();

            configuracaoAPIConexaoGeradorExcelKMM.UsarApiDeConexaoComGeradorExcelKMM = Request.GetBoolParam("UsarApiDeConexaoComGeradorExcelKMM");

            if (configuracaoAPIConexaoGeradorExcelKMM.Codigo == 0)
                repositorioConfiguracaoAPIConexaoGeradorExcelKMM.Inserir(configuracaoAPIConexaoGeradorExcelKMM);
            else
            {
                repositorioConfiguracaoAPIConexaoGeradorExcelKMM.Atualizar(configuracaoAPIConexaoGeradorExcelKMM);
                config.SetExternalChanges(configuracaoAPIConexaoGeradorExcelKMM.GetCurrentChanges());
            }

        }

        private void SalvarConfiguracaoGeralCIOT(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CIOT.ConfiguracaoGeralCIOT repositorioConfiguracaoGeralCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoGeralCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralCIOT configuracaoGeralCIOT = repositorioConfiguracaoGeralCIOT.BuscarConfiguracaoPadrao();
            if (configuracaoGeralCIOT == null)
                configuracaoGeralCIOT = new Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralCIOT();

            configuracaoGeralCIOT.Initialize();
            configuracaoGeralCIOT.TipoGeracaoCIOT = Request.GetEnumParam<TipoGeracaoCIOT>("TipoGeracaoCIOT");
            configuracaoGeralCIOT.TipoFavorecidoCIOT = Request.GetEnumParam<TipoFavorecidoCIOT>("TipoFavorecidoCIOT");
            configuracaoGeralCIOT.TipoQuitacaoCIOT = Request.GetEnumParam<TipoQuitacaoCIOT>("TipoQuitacaoCIOT");
            configuracaoGeralCIOT.TipoAdiantamentoCIOT = Request.GetEnumParam<TipoQuitacaoCIOT>("TipoAdiantamentoCIOT");

            if (configuracaoGeralCIOT.Codigo > 0)
                repositorioConfiguracaoGeralCIOT.Atualizar(configuracaoGeralCIOT);
            else
                repositorioConfiguracaoGeralCIOT.Inserir(configuracaoGeralCIOT);
            config.SetExternalChanges(configuracaoGeralCIOT.GetCurrentChanges());
        }

        private void SalvarConfiguracaoGeralTipoPagamentoCIOT(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT repositorioConfiguracaoGeralTipoPagamentoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT(unitOfWork);

            List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT> tiposPagamento = repositorioConfiguracaoGeralTipoPagamentoCIOT.BuscarConfiguracaoPadrao();

            dynamic dynTiposPagamento = JsonConvert.DeserializeObject<dynamic>((string)Request.Params("TiposPagamentoCIOTOperadora"));

            List<int> codigos = new List<int>();

            foreach (dynamic obj in dynTiposPagamento)
                if (obj.Codigo != null)
                    codigos.Add((int)obj.Codigo);

            List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT> listaTiposPagamentoDeletar = tiposPagamento.Where(obj => !codigos.Contains(obj.Codigo)).ToList();

            for (int i = 0; i < listaTiposPagamentoDeletar.Count; i++)
            {
                Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT tipoPagamentoDeletar = listaTiposPagamentoDeletar[i];

                repositorioConfiguracaoGeralTipoPagamentoCIOT.Deletar(tipoPagamentoDeletar);
            }

            foreach (dynamic obj in dynTiposPagamento)
            {
                Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT tipoPagamento = null;

                int codigo = 0;
                if (obj.Codigo != null && int.TryParse((string)obj.Codigo ?? "", out codigo))
                    tipoPagamento = repositorioConfiguracaoGeralTipoPagamentoCIOT.BuscarPorCodigo(codigo, true);

                if (tipoPagamento == null)
                    tipoPagamento = new Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT();

                tipoPagamento.TipoPagamentoCIOT = ((string)obj.TipoPagamentoCIOT).ToEnum<TipoPagamentoCIOT>();

                tipoPagamento.Operadora = ((string)obj.OperadoraCIOT).ToEnum<OperadoraCIOT>();

                if (tipoPagamento.Codigo > 0)
                    repositorioConfiguracaoGeralTipoPagamentoCIOT.Atualizar(tipoPagamento);
                else
                    repositorioConfiguracaoGeralTipoPagamentoCIOT.Inserir(tipoPagamento);
            }
        }

        private void SalvarConfiguracaoDownloadArquivos(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos repositorioConfiguracaoDownloadArquivos = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos configuracaoDownloadArquivos = repositorioConfiguracaoDownloadArquivos.BuscarPrimeiroRegistro();

            if (configuracaoDownloadArquivos == null)
                configuracaoDownloadArquivos = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos();
            else
                configuracaoDownloadArquivos.Initialize();

            configuracaoDownloadArquivos.PermitirBaixarArquivosConembOcorenManualmente = Request.GetBoolParam("PermitirBaixarArquivosConembOcorenManualmente");

            if (configuracaoDownloadArquivos.Codigo == 0)
                repositorioConfiguracaoDownloadArquivos.Inserir(configuracaoDownloadArquivos);
            else
            {
                repositorioConfiguracaoDownloadArquivos.Atualizar(configuracaoDownloadArquivos);
                config.SetExternalChanges(configuracaoDownloadArquivos.GetCurrentChanges());
            }

        }

        private void SalvarConfiguracaoPaginacaoInterfaces(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces repositorioConfiguracaoPaginacaoInterfaces = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces> interfacesPaginacao = repositorioConfiguracaoPaginacaoInterfaces.BuscarConfiguracaoPadrao();

            dynamic dynInterfacesPaginacao = JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridConfiguracoesPaginacaoInterfaces"));

            List<int> codigos = new List<int>();

            foreach (dynamic obj in dynInterfacesPaginacao)
                if (obj.Codigo != null)
                    codigos.Add((int)obj.Codigo);

            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces> listaInterfacesPaginacaoDeletar = interfacesPaginacao.Where(obj => !codigos.Contains(obj.Codigo)).ToList();

            for (int i = 0; i < listaInterfacesPaginacaoDeletar.Count; i++)
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces interfacePaginacaoDeletar = listaInterfacesPaginacaoDeletar[i];

                repositorioConfiguracaoPaginacaoInterfaces.Deletar(interfacePaginacaoDeletar);
            }

            foreach (dynamic obj in dynInterfacesPaginacao)
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces interfacePaginacao = null;

                int codigo = 0;
                if (obj.Codigo != null && int.TryParse((string)obj.Codigo ?? "", out codigo))
                    interfacePaginacao = repositorioConfiguracaoPaginacaoInterfaces.BuscarPorCodigo(codigo, true);

                if (interfacePaginacao == null)
                    interfacePaginacao = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces();

                interfacePaginacao.Interface = ((string)obj.Interface).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ConfiguracaoPaginacaoInterfaces>();

                interfacePaginacao.Dias = (int)obj.Dias;

                if (interfacePaginacao.Codigo > 0)
                    repositorioConfiguracaoPaginacaoInterfaces.Atualizar(interfacePaginacao);
                else
                    repositorioConfiguracaoPaginacaoInterfaces.Inserir(interfacePaginacao);
            }
        }

        #endregion Métodos Privados - Salvar configurações em tabelas específicas

        #region Métodos Privados

        private void ExecutarProcessos(string caminhoexe)
        {
            try
            {
                using (Process processo = new Process())
                {
                    processo.StartInfo.FileName = caminhoexe;
                    processo.Start();
                    processo.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        private dynamic ObterConfiguracaoControleEntrega(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

            return new
            {
                configuracao.ConfirmarEntregaDigitilizacaoCanhoto,
                configuracao.PermitirContatoWhatsApp,
                configuracao.PermitirAtualizarPrevisaoControleEntrega,
                configuracao.PermitirAtualizarPrevisaoEntregaPedidoControleEntrega,
                configuracao.NaoEncerrarViagemAoEncerrarControleEntrega,
                configuracao.HabilitarWidgetAtendimento,
                configuracao.FiltrarWidgetAtendimentoProFiltro,
                configuracao.PermiteRemoverReentrega,
                configuracao.HabilitarEstadoPassouRaioSemConfirmar,
                configuracao.HabilitarIconeEntregaAtrasada,
                configuracao.TempoPadraoDeEntrega,
                configuracao.TempoMinimoAcionarPassouRaioSemConfirmar,
                configuracaoControleEntrega.UtilizarPrevisaoEntregaPedidoComoDataPrevista,
                configuracaoControleEntrega.UtilizarMaiorDataColetaPrevistaComoDataPrevistaParaEntregaUnica,
                configuracaoControleEntrega.TempoInicioViagemAposEmissaoDoc,
                configuracaoControleEntrega.TempoInicioViagemAposFinalizacaoFluxoPatio,
                ObrigatorioInformarFreetimeCadastroRotas = configuracaoControleEntrega.ObrigatorioInformarFreetime,
                configuracaoControleEntrega.ExibirOpcaoAjustarEntregaOnTime,
                configuracaoControleEntrega.ExibirPacotesOcorrenciaControleEntrega,
                configuracaoControleEntrega.PermitirReordenarEntregasAoAddPedido,
                configuracaoControleEntrega.HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos,
                configuracaoControleEntrega.HoraFimPadraoEntrega,
                configuracaoControleEntrega.PermiteAlterarAgendamentoDaEntregaNoAcompanhamentoDeCargas,
                configuracaoControleEntrega.PermitirAbrirAtendimentoViaControleEntrega,
                configuracaoControleEntrega.RecalcularPrevisaoAoIniciarViagem,
                configuracaoControleEntrega.ExibirDataEntregaNotaControleEntrega,
                configuracaoControleEntrega.PermiteExibirCargaCancelada,
                configuracaoControleEntrega.NaoPermitirConfirmacaoEntregaPortalTransportadorSemDigitalizacaoCanhotos,
                configuracaoControleEntrega.PermitirAlterarDataAgendamentoEntregaTransportador,
                configuracaoControleEntrega.ConsiderarCargaOrigemParaEntregasTransbordadas,
                configuracaoControleEntrega.RejeitarEntregaNotaFiscalAoRejeitarCanhoto,
                configuracaoControleEntrega.ConsiderarMediaDeVelocidadeDasUltimasCincoPosicoes,
                configuracaoControleEntrega.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida,
                configuracaoControleEntrega.UtilizarLeadTimeDaTabelaDeFreteParaCalculoDaPrevisaoDeEntrega,
                configuracaoControleEntrega.CalcularDataAgendamentoAutomaticamenteDataFaturamento,
                configuracaoControleEntrega.PermitirEnvioCanhotosPeloPortalTransportadorControleEntregas,
                configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona,
                configuracaoControleEntrega.PermitirEnvioNovasOcorrenciasComMesmoCadastroTipoOcorrencia,
                configuracaoControleEntrega.PermitirBuscarCargasAgrupadasAoPesquisarNumero,
                configuracaoControleEntrega.EncerrarMDFeAutomaticamenteAoFinalizarEntregas,
                configuracaoControleEntrega.PermitirAjustarEntregasEtapasAnterioresIntegracao,
                configuracaoControleEntrega.PermitirBloqueioFinalizacaoEntrega,
                configuracaoControleEntrega.PossuiNotaCobertura,
                configuracaoControleEntrega.TempoReprocessarCargaEntregasSemNotas
            };
        }

        private dynamic obterConfiguracoesMotorista(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            return new
            {
                JornadaDiariaMotorista = configuracao.JornadaDiariaMotorista?.ToString(@"hh\:mm"),
                configuracao.UtilizarControleJornadaMotorista,
                configuracao.HabilitarFichaMotoristaTodos,
                configuracao.UtilizarComissaoPorCargo,
                configuracao.PercentualComissaoPadrao,
                configuracao.PercentualMediaEquivalente,
                configuracao.PercentualEquivaleEquivalente,
                configuracao.PercentualAdvertenciaEquivalente,
                SistemasIntegracaoMotorista = (
                    from configuracaoMotorista in configuracao.TiposIntegracaoValidarMotorista
                    select new
                    {
                        Tipo = (int)configuracaoMotorista.Tipo,
                        Descricao = configuracaoMotorista.Tipo.ObterDescricao()
                    }
                ).ToList()
            };
        }

        private dynamic obterConfiguracoesVeiculo(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            return new
            {
                SistemasIntegracaoVeiculo = (
                    from configuracaoVeiculo in configuracao.TiposIntegracaoValidarVeiculo
                    select new
                    {
                        Tipo = (int)configuracaoVeiculo.Tipo,
                        Descricao = configuracaoVeiculo.Tipo.ObterDescricao()
                    }
                ).ToList()
            };
        }
        private dynamic obterConfiguracoesPaisMercosul(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.PaisMercosul repPaisMercosul = new Repositorio.Embarcador.Configuracoes.PaisMercosul(unitOfWork);
            List<Dominio.Entidades.Embarcador.Configuracoes.PaisMercosul> paisesMercosul = repPaisMercosul.BuscarTodos();

            return new
            {
                PaisesMercosul = (
                    from Paises in paisesMercosul
                    select new
                    {
                        Codigo = Paises.Codigo,
                        Pais = Paises.Pais,
                        Empresa = Paises.Empresa,
                        UltimoCrt = Paises.UltimoCrt,
                        UltimoMicDta = Paises.UltimoMicDta
                    }
                ).ToList()
            };
        }
        private void preencherSistemasIntegracaoMotorista(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorio = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<string> listaTipoIntegracaoMotorista = JsonConvert.DeserializeObject<List<string>>(Request.Params("SistemasIntegracaoMotorista"));

            configuracao.TiposIntegracaoValidarMotorista?.Clear();

            configuracao.TiposIntegracaoValidarMotorista = new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            configuracao.ProvisionarDocumentosEmitidos = Request.GetBoolParam("ProvisionarDocumentosEmitidos");

            configuracao.DescricaoProdutoPredominatePadrao = Request.Params("DescricaoProdutoPredominatePadrao") ?? string.Empty;
            configuracao.EmailsRetornoProblemaGerarCargaEmail = Request.Params("EmailsRetornoProblemaGerarCargaEmail") ?? string.Empty;
            configuracao.EmailsAvisoVencimentoCotratoFrete = Request.Params("EmailsAvisoVencimentoCotratoFrete") ?? string.Empty;
            foreach (string tipoIntegracaoMotorista in listaTipoIntegracaoMotorista)
            {
                TipoIntegracao? tipo = tipoIntegracaoMotorista.ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>();

                if (!tipo.HasValue)
                    throw new ControllerException($"Alguma Integração da aba Motorista é inválida.");

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorio.BuscarPorTipo(tipo.Value);

                if (tipoIntegracao == null)
                    throw new ControllerException($"A Integração: {tipo.Value.ObterDescricao()} da aba Motorista não está cadastrada no sistema ou está inativa.");

                configuracao.TiposIntegracaoValidarMotorista.Add(tipoIntegracao);
            }
        }

        private void preencherSistemasIntegracaoVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorio = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<string> listaTipoIntegracaoVeiculo = JsonConvert.DeserializeObject<List<string>>(Request.Params("SistemasIntegracaoVeiculo"));

            configuracao.TiposIntegracaoValidarVeiculo?.Clear();

            configuracao.TiposIntegracaoValidarVeiculo = new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            foreach (string tipoIntegracaoVeiculo in listaTipoIntegracaoVeiculo)
            {
                TipoIntegracao? tipo = tipoIntegracaoVeiculo.ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>();

                if (!tipo.HasValue)
                    throw new ControllerException("Alguma Integração da aba Veículo é inválida.");

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorio.BuscarPorTipo(tipo.Value);

                if (tipoIntegracao == null)
                    throw new ControllerException($"A Integração: {tipo.Value.ObterDescricao()} da aba Veículo não está cadastrada no sistema ou está inativa.");

                configuracao.TiposIntegracaoValidarVeiculo.Add(tipoIntegracao);
            }
        }

        private void preencherPaisesMercosul(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Configuracoes.PaisMercosul repositorioPaisesMercosul = new Repositorio.Embarcador.Configuracoes.PaisMercosul(unitOfWork);

            var jsonString = Request.Params("PaisesMercosul");

            List<List<string>> linhasPaises = new List<List<string>>();

            try
            {
                linhasPaises = JsonConvert.DeserializeObject<List<List<string>>>(jsonString);
            }
            catch
            {
                try
                {
                    var listaSimples = JsonConvert.DeserializeObject<List<string>>(jsonString);
                    const int tamanhoGrupo = 4;
                    for (int i = 0; i < listaSimples.Count; i += tamanhoGrupo)
                    {
                        var subLista = listaSimples.Skip(i).Take(tamanhoGrupo).ToList();
                        linhasPaises.Add(subLista);
                    }
                }
                catch
                {
                    throw new Exception("O formato do JSON não é válido.");
                }
            }

            var listaPaisesMercosul = linhasPaises.Select(linha => new Dominio.Entidades.Embarcador.Configuracoes.PaisMercosul
            {
                Codigo = 0,
                Pais = linha.ElementAtOrDefault(0) ?? "Sem Nome",
                Empresa = linha.ElementAtOrDefault(1) ?? "Empresa Padrão",
                UltimoCrt = int.TryParse(linha.ElementAtOrDefault(2), out var crt) ? crt : 0,
                UltimoMicDta = int.TryParse(linha.ElementAtOrDefault(3), out var micDta) ? micDta : 0
            }).ToList();

            repositorioPaisesMercosul.InserirLista(listaPaisesMercosul);

        }

        private string ValidarParametrosPrevisaoEntrega(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            string msg = string.Empty;
            if (
                (configuracao.PrevisaoEntregaPeriodoUtilHorarioInicial > TimeSpan.Zero && configuracao.PrevisaoEntregaPeriodoUtilHorarioFinal == TimeSpan.Zero) ||
                (configuracao.PrevisaoEntregaPeriodoUtilHorarioInicial == TimeSpan.Zero && configuracao.PrevisaoEntregaPeriodoUtilHorarioFinal > TimeSpan.Zero) ||
                (configuracao.PrevisaoEntregaPeriodoUtilHorarioInicial > TimeSpan.Zero && configuracao.PrevisaoEntregaPeriodoUtilHorarioFinal > TimeSpan.Zero && configuracao.PrevisaoEntregaPeriodoUtilHorarioInicial >= configuracao.PrevisaoEntregaPeriodoUtilHorarioFinal)
            )
            {
                return "No controle de entrega, ao informar o período útil na previsão de entrega, o horário inicial deve ser menor que o o horário final.";
            }

            if (configuracao.PrevisaoEntregaVelocidadeMediaVazio > 0 &&
                configuracao.PrevisaoEntregaVelocidadeMediaCarregado > 0 &&
                configuracao.PrevisaoEntregaVelocidadeMediaVazio < configuracao.PrevisaoEntregaVelocidadeMediaCarregado
            )
            {
                return "No controle de entrega, a velocidade média do veículo vazio deve ser maior que a velocidade média do veículo carregado.";
            }

            return msg;
        }

        private string ObterCaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Configuracao" });
        }

        #endregion Métodos Privados
    }
}