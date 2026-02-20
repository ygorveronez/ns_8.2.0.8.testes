/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Buscas.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/MapaDraw.js" />
/// <reference path="../../../js/Global/MapaGoogle.js" />
/// <reference path="../../../js/Global/Mapa.js"/>
/// <reference path="../../../js/Global/MapaGoogleSearch.js"/>
/// <reference path="../../../js/Global/Charts.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../Global/ModeloFiltroPesquisa/ConfiguracaoModeloFiltroPesquisa.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatus.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatusViagemTipoRegra.js" />
/// <reference path="../../Enumeradores/EnumAlertaMonitorStatus.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoFiltroCliente.js" />
/// <reference path="../../Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Fronteira.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/CanalVenda.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/TipoTrecho.js" />
/// <reference path="../Tracking/Tracking.lib.js" />
/// <reference path="../../Logistica/Monitoramento/MonitoramentoSignalR.js" />

// #region Objetos Globais do Arquivo
var _gridMonitoramentoNovo;
var _pesquisaMonitoramentoMapa;
var _cabecalhoMonitoramentoMapa;
var _configuracaoIndicadorUsuarioMonitoramentoMapa;
var _configuracaoLegendaUsuarioMonitoramentoMapa;
var _salvarConfiguracaoUsuarioMonitoramentoMapa;
var _mapaEntregas;
var _dadosGrid;
var _dadosLocaisRaioProximidade = [];
var _playerMonitoramento;
var _timerInterval = null;
var _timerAtualizacaoVeiculosEmRaio = null;
var _veiculosEmLocaisRaioProximidade;
var _gridVeiculosEmLocaisRaioProximidade;
var _localSelecionado;
var _totalSeconds = 300;
var _countSeconds = _totalSeconds;
var _listaGridsVeiculosEmRaio = [];
var _PermissoesPersonalizadas;
var _enviarNotificacaoApp;
var _gridEnviarNotificacaoApp;
var usarGrupoStatusViagem = false;
var mostrarCarrossel = false;
var htmlCarrosselItems = {};

var _situacaoIntegracao = [{ value: null, text: "Todas" },
{ value: EnumSituacaoIntegracaoCarga.AgIntegracao, text: "Aguardando Integração" },
{ value: EnumSituacaoIntegracaoCarga.Integrado, text: "Integrado" },
{ value: EnumSituacaoIntegracaoCarga.ProblemaIntegracao, text: "Falha na Integração" }];

// #endregion Objetos Globais do Arquivo

// #region Classes
var PesquisaMonitoramentoNovo = function () {
    this.PesquisarNovoMonitoramento = PropertyEntity({
        eventClick: function (e) {
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) {
                function sincronizarStatusViagem() {
                    if (_pesquisaMonitoramentoMapa.PersonalizadoStatusViagem &&
                        _pesquisaMonitoramentoMapa.PersonalizadoStatusViagem.val()) {
                        var valoresPersonalizados = _pesquisaMonitoramentoMapa.PersonalizadoStatusViagem.val();
                        _pesquisaMonitoramentoMapa.StatusViagem.val(valoresPersonalizados);
                        $("#" + _pesquisaMonitoramentoMapa.StatusViagem.id).selectpicker('refresh');
                    }
                }

                sincronizarStatusViagem();
                limparSelecaoCarrossel();
                recarregarDadosMonitoramentoNovo(false, true, false);
                $('#legendasMapa').show();
                _cabecalhoMonitoramentoMapa.CofiguracaoMonitoramentoMapa.visible(true);

                Global.fecharModal('divModalFiltrosMonitoramentoMapa');
            }

        }, type: types.event, text: Localization.Resources.Gerais.Geral.Filtrar, idGrid: guid(), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function (e) {
            LimparCampos(_pesquisaMonitoramentoMapa);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.LimparFiltros, idGrid: guid(), visible: ko.observable(true)
    });

    // FAVOR SEMPRE COLOCAR A PROPRIEDADE VISIBLE KO.OBSERVABLE NOS FILTROS
    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Carga, col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.Carga, val: ko.observable(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Veiculos, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), placeholder: Localization.Resources.Logistica.Monitoramento.Veiculo, val: ko.observable("") });
    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Pedido, col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.Pedido, val: ko.observable(), visible: ko.observable(true) });
    this.NumeroNotaFiscal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.NotaFiscal, col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.NotaFiscal, val: ko.observable(), visible: ko.observable(true) });
    this.MonitoramentoStatus = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.SituacaoMonitoramento, getType: typesKnockout.selectMultiple, val: ko.observableArray([]), options: EnumMonitoramentoStatus.obterOpcoes(), def: [], placeholder: Localization.Resources.Logistica.Monitoramento.StatusMonitoramento, visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.GrupoPessoa, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false), val: ko.observable(""), visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Cliente, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false), val: ko.observable(""), visible: ko.observable(true) });
    this.FuncionarioVendedor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Vendedor, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.StatusViagem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EtapaMonitoramento, val: ko.observable(), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });
    this.GrupoTipoOperacao = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TipoDeOperacao, val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Gerais.Geral.Transportador), issue: 69, idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe), val: ko.observable("") });
    this.Expedidor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Expedidor, idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
    this.SituacaoCarga = PropertyEntity({ val: ko.observable(0), visible: ko.observable(false) });
    this.TendenciaEntrega = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.GrupoTipoOperacaoIndicador = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(null) });
    this.ComAlertas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(null) });
    this.GrupoStatusViagem = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.MonitoramentoStatusViagemTipoRegra = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicioCriacaoCarga, getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable(""), placeholder: Localization.Resources.Gerais.Geral.DataInicial, val: ko.observable(), visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFimCriacaoCarga, getType: typesKnockout.dateTime, val: ko.observable(null), cssClass: ko.observable(""), placeholder: Localization.Resources.Gerais.Geral.DataFinal, val: ko.observable(), visible: ko.observable(true) });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    if (_CONFIGURACAO_TMS.TelaMonitoramentoPadraoFiltroDataInicialFinal) {
        this.DataInicial.val(Global.Data(EnumTipoOperacaoDate.Subtract, 2, EnumTipoOperacaoObjetoDate.Days));
        this.DataFinal.val(Global.Data(EnumTipoOperacaoDate.Add, 3, EnumTipoOperacaoObjetoDate.Days));
    }
    this.FiltroCliente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.FiltrarClientes, val: ko.observable(EnumMonitoramentoFiltroCliente.Nenhum), options: EnumMonitoramentoFiltroCliente.obterOpcoes(), def: EnumMonitoramentoFiltroCliente.Nenhum, visible: ko.observable(true) });
    this.CategoriaPessoa = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Categoria, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(false), val: ko.observable(""), visible: ko.observable(true) });
    this.SomenteRastreados = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Logistica.Monitoramento.Apenasmonitoramentosonline, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Filial, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true), placeholder: Localization.Resources.Gerais.Geral.Filial, val: ko.observable() });
    this.NumeroEXP = PropertyEntity({ text: Localization.Resources.Gerais.Geral.NumeroEXP, maxlength: 150, visible: ko.observable(true), val: ko.observable() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoDeOperacao, idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable() });
    this.DataEntregaPedidoInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataEntregaPedidoInicial, getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.DataEntregaPedidoInicial, visible: ko.observable(true) });
    this.DataEntregaPedidoFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataEntregaPedidoFinal, getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.DataEntregaPedidoFinal, visible: ko.observable(true) });
    this.PrevisaoEntregaInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaInicio, getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaInicio, visible: ko.observable(true) });
    this.PrevisaoEntregaFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaFinal, getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaFinal, visible: ko.observable(true) });
    this.InicioViagemPrevistaInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicialDaPrevisaoDeInicioDeViagem, getType: typesKnockout.dateTime, val: ko.observable(), def: "", visible: ko.observable(true) });
    this.InicioViagemPrevistaFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFinalDaPrevisaoDeInicioDeViagem, getType: typesKnockout.dateTime, val: ko.observable(), def: "", visible: ko.observable(true) });

    this.DataEmissaoNFeInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicioEmissaoNFe, getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.DataInicioEmissaoNFe, visible: ko.observable(true) });
    this.DataEmissaoNFeFim = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFimEmissaoNFe, getType: typesKnockout.dateTime, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.DataFimEmissaoNFe, visible: ko.observable(true) });
    this.VeiculosComContratoDeFrete = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.Apenasmonitoramentoscomveiculoscomcontratodefrete, visible: ko.observable(true) });
    this.SomenteUltimoPorCarga = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Logistica.Monitoramento.Apenasultimomonitoramentoporcarga, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.DescricaoAlerta = PropertyEntity();

    this.Origem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Origem, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(), visible: ko.observable(true) });
    this.Destino = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Destino, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), placeholder: Localization.Resources.Logistica.Monitoramento.Destino, val: ko.observable(), visible: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EstadoOrigem, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), placeholder: Localization.Resources.Logistica.Monitoramento.EstadoOrigem, val: ko.observable(), visible: ko.observable(true) });
    this.EstadoDestino = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EstadoDestino, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), placeholder: Localization.Resources.Logistica.Monitoramento.EstadoDestino, val: ko.observable(), visible: ko.observable(true) });
    this.NaoExibirResumosAlerta = PropertyEntity({ val: ko.observable(true), text: Localization.Resources.Logistica.Monitoramento.NaoExibirGraficoDeAlertas, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.CentroDeResultado, idBtnSearch: guid(), val: ko.observable(), visible: ko.observable(true) });
    this.ResponsavelVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.ResponsavelPeloVeiculo, idBtnSearch: guid(), val: ko.observable(), visible: ko.observable(true) });
    this.FronteiraRotaFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.FronteiraRotaFrete, idBtnSearch: guid(), val: ko.observable(), visible: ko.observable(true) });
    this.PaisOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.PaisOrigem, idBtnSearch: guid(), val: ko.observable(), visible: ko.observable(true) });
    this.PaisDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.PaisDestino, idBtnSearch: guid(), val: ko.observable(), visible: ko.observable(true) });
    this.ApenasMonitoramentosCriticos = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Logistica.Monitoramento.ApenasMonitoramentosCriticos, visible: ko.observable(true) });
    this.DataInicioCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicioCarregamento, getType: typesKnockout.dateTime, val: ko.observable(""), enable: ko.observable(true), def: "", visible: ko.observable(true) });
    this.DataFimCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFimCarregamento, getType: typesKnockout.dateTime, val: ko.observable(Global.DataAtual()), def: "", enable: ko.observable(true), visible: ko.observable(true) });
    this.PossuiRecebedor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.PossuiRecebedor, val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: 9, visible: ko.observable(true) });
    this.PossuiExpedidor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.PossuiExpedidor, val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: 9, visible: ko.observable(true) });
    this.Recebedores = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Recebedor, idBtnSearch: guid(), val: ko.observable(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Destinatario, idBtnSearch: guid(), val: ko.observable(), visible: ko.observable(true) });
    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Remetente, idBtnSearch: guid(), val: ko.observable(), visible: ko.observable(true) });
    this.CodigoCargaEmbarcadorMulti = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Logistica.Monitoramento.MultiplosNumerosCarga), idBtnSearch: guid(), visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "CodigoCargaEmbarcador" }, val: ko.observable() });
    this.TipoTrecho = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.TipoTrecho, idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable() });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.TipoCarga, idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable() });
    this.Produtos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Produtos, idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable() });
    this.DataRealEntrega = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataRealEntrega, getType: typesKnockout.date, val: ko.observable(), placeholder: Localization.Resources.Gerais.Geral.DataEntregaPedidoInicial, visible: ko.observable(true) });
    this.VeiculosEmLocaisTracking = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Logistica.Monitoramento.VeiculosEmLocais, visible: ko.observable(true) });
    this.MostrarRaiosProximidade = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Logistica.Monitoramento.MostrarRaiosProximidade, visible: ko.observable(true) });
    this.LocaisTracking = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.LocaisTracking, idBtnSearch: guid(), visible: ko.observable(false), val: ko.observable() });
    this.LocaisRaioProximidade = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Locais, idBtnSearch: guid(), visible: ko.observable(false), val: ko.observable() });
    this.AbaPersonalizada = PropertyEntity({ type: types.string, val: ko.observable(""), text: ko.observable("") });
    this.FiltrosPersonalizados = PropertyEntity({ eventClick: criarAbaFiltrosPersonalizados, type: types.event, visible: ko.observable(false), enable: ko.observable(true) });
    this.DataAgendamentoPedidoInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicialAgendamentoPedido, getType: typesKnockout.dateTime, val: ko.observable(), def: "", visible: ko.observable(true) });
    this.DataAgendamentoPedidoFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFinalAgendamentoPedido, getType: typesKnockout.dateTime, val: ko.observable(), def: "", visible: ko.observable(true) });
    this.DataColetaPedidoInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicialColetaPedido, getType: typesKnockout.dateTime, val: ko.observable(), def: "", visible: ko.observable(true) });
    this.DataColetaPedidoFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFinalColetaPedido, getType: typesKnockout.dateTime, val: ko.observable(), def: "", visible: ko.observable(true) });
    this.DataMonitoramentoInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicioMonitoramento, getType: typesKnockout.dateTime, val: ko.observable(), def: "", visible: ko.observable(true) });
    this.DataMonitoramentoFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFimMonitoramento, getType: typesKnockout.dateTime, val: ko.observable(), def: "", visible: ko.observable(true) });
    this.NumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.NumeroPedidoCliente, col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.Carga, val: ko.observable(), visible: ko.observable(true) });
    this.ClientesComplementar = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.ClienteComplementar, idBtnSearch: guid(), visible: ko.observable(true) });
    this.CanalVenda = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.CanalVenda, idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoIntegracaoSM = PropertyEntity({ text: Localization.Resources.Gerais.Geral.SituacaoIntegracao, options: _situacaoIntegracao, val: ko.observable(_situacaoIntegracao[0]), def: _situacaoIntegracao[0], issue: 0, visible: ko.observable(true) });
    this.VeiculoNoRaio = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Logistica.Monitoramento.VeiculoNoRaio, visible: ko.observable(true) });
    this.TipoCobrancaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoCobrancaMultimodal.Nenhum), options: EnumTipoCobrancaMultimodal.obterOpcoes(), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ModalTransporte, def: EnumTipoCobrancaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.TipoMercadoria = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.TipoMercadoria, visible: ko.observable(true) });
    this.EquipeVendas = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.EquipeVendas, visible: ko.observable(true) });
    this.EscritorioVenda = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.EscritorioVenda, visible: ko.observable(true) });
    this.RotaFrete = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.RotaFrete, visible: ko.observable(true) });
    this.Mesoregiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Mesoregiao, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Regiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Regiao, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Matriz = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.Matriz, visible: ko.observable(true) });
    this.Parqueada = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Parqueada, val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(true) });
    this.Vendedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Vendedor, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Supervisor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Supervisor, idBtnSearch: guid(), visible: ko.observable(true) });
    this.ColetaNoPrazo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.ColetaNoPrazo, val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(true) });
    this.EntregaNoPrazo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EntregaNoPrazo, val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(true) });
    this.TendenciaProximaColeta = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TendenciaColeta, val: ko.observable(EnumFiltroTendenciaPrazoEntrega.Nenhum), options: EnumFiltroTendenciaPrazoEntrega.obterOpcoes(), def: [EnumFiltroTendenciaPrazoEntrega.Nenhum], getType: typesKnockout.selectMultiple, visible: ko.observable(true) });
    this.TendenciaProximaEntrega = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TendenciaEntrega, val: ko.observable(EnumFiltroTendenciaPrazoEntrega.Nenhum), options: EnumFiltroTendenciaPrazoEntrega.obterOpcoes(), def: [EnumFiltroTendenciaPrazoEntrega.Nenhum], getType: typesKnockout.selectMultiple, visible: ko.observable(true) });
    this.TipoAlertaEvento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.TipoAlerta.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: Localization.Resources.Logistica.Monitoramento.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.ModeloFiltrosPesquisa = PropertyEntity({
        type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ModeloDeFiltroDePesquisa, idBtnSearch: guid(),
        tipoFiltroPesquisa: EnumCodigoFiltroPesquisa.Monitoramento,
    });

    this.CarregarFiltrosPesquisa = PropertyEntity({
        eventClick: function (e) {
            abrirBuscaFiltrosManual(e);
        }, type: types.event, text: "Carregar Filtro", idFade: guid(), visible: ko.observable(true)
    });
    this.ConfiguracaoModeloFiltroPesquisa = PropertyEntity({ eventClick: function (e) { abrirConfiguracaoModeloFiltroPesquisa(EnumCodigoFiltroPesquisa.Monitoramento, _pesquisaMonitoramentoMapa) }, type: types.event, text: Localization.Resources.Gerais.Geral.SalvarFiltro, visible: ko.observable(true) });

    this.CodigoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.Monitoramento.Motorista, idBtnSearch: guid(), visible: ko.observable(true) });

    this.MostrarRaiosProximidade.val.subscribe(function (checked) {
        _pesquisaMonitoramentoMapa.LocaisRaioProximidade.visible(checked);
        if (!checked)
            LimparCampo(_pesquisaMonitoramentoMapa.LocaisRaioProximidade);
    });
    this.MostrarRaiosProximidade.val.subscribe(function (val) {
        $("#legenda-Veiculos-em-Locais-Raio-Proximidade").hide();
        if (val)
            $("#legenda-Veiculos-em-Locais-Raio-Proximidade").show();
    });



    // SE ADICIONOU UM NOVO FILTRO FAVOR ADICIONAR O PERSONALIZADO ABAIXO TAMBÉM E SEU CHECKBOX NO FiltroPersonalizado.js (ISSO É TEMPORÁRIO 24/04/2024)
    //FAVOR SEGUIR OS EXEMPLOS ABAIXO ESPECIFICOS PARA CADA TIPO DE CAMPO, O MESMO PARA O HTML.
    this.PersonalizadoCodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Carga, col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.Carga, val: ko.observable(this.CodigoCargaEmbarcador.val), visible: ko.observable(false), def: ko.observable(this.CodigoCargaEmbarcador.val) });
    this.PersonalizadoVeiculo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Veiculos, type: types.multiplesEntities, multiplesEntities: this.Veiculo.multiplesEntities, codEntity: this.Veiculo.codEntity, val: this.Veiculo.val, idBtnSearch: guid(), visible: ko.observable(false), placeholder: Localization.Resources.Logistica.Monitoramento.Veiculo });
    this.PersonalizadoNumeroPedido = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Pedido, col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.Pedido, visible: ko.observable(false), codEntity: this.NumeroPedido.codEntity, val: ko.observable(this.NumeroPedido.val), def: ko.observable(this.NumeroPedido.val) });
    this.PersonalizadoNumeroNotaFiscal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.NotaFiscal, col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.NotaFiscal, val: ko.observable(this.NumeroNotaFiscal.val), visible: ko.observable(false), def: ko.observable(this.NumeroNotaFiscal.val) });
    this.PersonalizadoMonitoramentoStatus = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.SituacaoMonitoramento, getType: typesKnockout.selectMultiple, val: this.MonitoramentoStatus.val, options: EnumMonitoramentoStatus.obterOpcoes(), def: [], placeholder: Localization.Resources.Logistica.Monitoramento.StatusMonitoramento, visible: ko.observable(false) });
    this.PersonalizadoFiltroCliente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.FiltrarClientes, val: this.FiltroCliente.val, options: EnumMonitoramentoFiltroCliente.obterOpcoes(), def: EnumMonitoramentoFiltroCliente.Nenhum, visible: ko.observable(false) });
    this.PersonalizadoGrupoPessoa = PropertyEntity({ type: types.entity, codEntity: this.GrupoPessoa.codEntity, val: this.GrupoPessoa.val, text: Localization.Resources.Logistica.Monitoramento.GrupoPessoa, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(false) });
    this.PersonalizadoFuncionarioVendedor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Vendedor, type: types.entity, codEntity: this.FuncionarioVendedor.codEntity, idBtnSearch: guid(), visible: ko.observable(false), val: this.FuncionarioVendedor.val });
    this.PersonalizadoStatusViagem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EtapaMonitoramento, val: this.StatusViagem.val, def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(false) });
    this.PersonalizadoTransportador = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.Transportador.multiplesEntities, codEntity: this.Transportador.codEntity, val: this.Transportador.val, text: ko.observable(Localization.Resources.Gerais.Geral.Transportador), issue: 69, idBtnSearch: guid(), visible: ko.observable(false) });
    this.PersonalizadoExpedidor = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.Expedidor.multiplesEntities, codEntity: this.Expedidor.codEntity, val: this.Expedidor.val, text: Localization.Resources.Logistica.Monitoramento.Expedidor, idBtnSearch: guid(), visible: ko.observable(false) });
    this.PersonalizadoDataInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicioCriacaoCarga, getType: typesKnockout.dateTime, val: ko.observable(this.DataInicial.val), def: ko.observable(this.DataInicial.val), cssClass: ko.observable(""), placeholder: Localization.Resources.Gerais.Geral.DataInicial, visible: ko.observable(false) });
    this.PersonalizadoDataFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFimCriacaoCarga, getType: typesKnockout.dateTime, val: ko.observable(this.DataFinal.val), def: ko.observable(this.DataFinal.val), cssClass: ko.observable(""), placeholder: Localization.Resources.Gerais.Geral.DataFinal, visible: ko.observable(false) });
    this.PersonalizadoCliente = PropertyEntity({ type: types.entity, codEntity: this.Cliente.codEntity, val: this.Cliente.val, text: Localization.Resources.Logistica.Monitoramento.Cliente, idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(false) });
    this.PersonalizadoCategoriaPessoa = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Categoria, type: types.entity, codEntity: this.CategoriaPessoa.codEntity, idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(false), val: this.CategoriaPessoa.val });
    this.PersonalizadoSomenteRastreados = PropertyEntity({ val: ko.observable(this.SomenteRastreados.val), text: Localization.Resources.Logistica.Monitoramento.Apenasmonitoramentosonline, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PersonalizadoFilial = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Filial, type: types.multiplesEntities, multiplesEntities: this.Filial.multiplesEntities, codEntity: this.Filial.codEntity, idBtnSearch: guid(), visible: ko.observable(false), placeholder: Localization.Resources.Gerais.Geral.Filial, val: ko.observable(this.Filial.val) });
    this.PersonalizadoNumeroEXP = PropertyEntity({ text: Localization.Resources.Gerais.Geral.NumeroEXP, maxlength: 150, visible: ko.observable(false), val: ko.observable(this.NumeroEXP.val), def: ko.observable(this.NumeroEXP.val) });
    this.PersonalizadoTipoOperacao = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.TipoOperacao.multiplesEntities, codEntity: this.TipoOperacao.codEntity, text: Localization.Resources.Gerais.Geral.TipoDeOperacao, idBtnSearch: guid(), visible: ko.observable(false), val: this.TipoOperacao.val });
    this.PersonalizadoDataEntregaPedidoInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataEntregaPedidoInicial, getType: typesKnockout.dateTime, val: ko.observable(this.DataEntregaPedidoInicio.val), def: ko.observable(this.DataEntregaPedidoInicio.val), placeholder: Localization.Resources.Gerais.Geral.DataEntregaPedidoInicial, visible: ko.observable(false) });
    this.PersonalizadoDataEntregaPedidoFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataEntregaPedidoFinal, getType: typesKnockout.dateTime, val: ko.observable(this.DataEntregaPedidoFinal.val), def: ko.observable(this.DataEntregaPedidoFinal.val), placeholder: Localization.Resources.Gerais.Geral.DataEntregaPedidoFinal, visible: ko.observable(false) });
    this.PersonalizadoPrevisaoEntregaInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaInicio, getType: typesKnockout.dateTime, val: ko.observable(this.PrevisaoEntregaInicio.val), def: ko.observable(this.PrevisaoEntregaInicio.val), placeholder: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaInicio, visible: ko.observable(false) });
    this.PersonalizadoPrevisaoEntregaFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaFinal, getType: typesKnockout.dateTime, val: ko.observable(this.PrevisaoEntregaFinal.val), def: ko.observable(this.PrevisaoEntregaFinal.val), placeholder: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaFinal, visible: ko.observable(false) });
    this.PersonalizadoInicioViagemPrevistaInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicialDaPrevisaoDeInicioDeViagem, getType: typesKnockout.dateTime, val: ko.observable(this.InicioViagemPrevistaInicial.val), def: ko.observable(this.InicioViagemPrevistaInicial.val), visible: ko.observable(false) });
    this.PersonalizadoInicioViagemPrevistaFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFinalDaPrevisaoDeInicioDeViagem, getType: typesKnockout.dateTime, val: ko.observable(this.InicioViagemPrevistaFinal.val), def: ko.observable(this.InicioViagemPrevistaFinal.val), visible: ko.observable(false) });
    this.PersonalizadoDataEmissaoNFeInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicioEmissaoNFe, getType: typesKnockout.dateTime, val: ko.observable(this.DataEmissaoNFeInicio.val), def: ko.observable(this.DataEmissaoNFeInicio.val), placeholder: Localization.Resources.Gerais.Geral.DataInicioEmissaoNFe, visible: ko.observable(false) });
    this.PersonalizadoDataEmissaoNFeFim = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFimEmissaoNFe, getType: typesKnockout.dateTime, val: ko.observable(this.DataEmissaoNFeFim.val), def: ko.observable(this.DataEmissaoNFeFim.val), placeholder: Localization.Resources.Gerais.Geral.DataFimEmissaoNFe, visible: ko.observable(false) });
    this.PersonalizadoVeiculosComContratoDeFrete = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.Apenasmonitoramentoscomveiculoscomcontratodefrete, visible: ko.observable(false), val: this.VeiculosComContratoDeFrete.val });
    this.PersonalizadoOrigem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Origem, type: types.multiplesEntities, multiplesEntities: this.Origem.multiplesEntities, codEntity: this.Origem.codEntity, idBtnSearch: guid(), visible: ko.observable(false), val: this.Origem.val });
    this.PersonalizadoDestino = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Destino, type: types.multiplesEntities, multiplesEntities: this.Destino.multiplesEntities, codEntity: this.Destino.codEntity, idBtnSearch: guid(), placeholder: Localization.Resources.Logistica.Monitoramento.Destino, visible: ko.observable(false), val: this.Destino.val });
    this.PersonalizadoEstadoOrigem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EstadoOrigem, type: types.multiplesEntities, multiplesEntities: this.EstadoOrigem.multiplesEntities, codEntity: this.EstadoOrigem.codEntity, idBtnSearch: guid(), placeholder: Localization.Resources.Logistica.Monitoramento.EstadoOrigem, visible: ko.observable(false), val: this.EstadoOrigem.val });
    this.PersonalizadoEstadoDestino = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EstadoDestino, type: types.multiplesEntities, multiplesEntities: this.EstadoDestino.multiplesEntities, codEntity: this.EstadoDestino.codEntity, idBtnSearch: guid(), placeholder: Localization.Resources.Logistica.Monitoramento.EstadoDestino, visible: ko.observable(false), val: this.EstadoDestino.val });
    this.PersonalizadoNaoExibirResumosAlerta = PropertyEntity({ val: ko.observable(true), text: Localization.Resources.Logistica.Monitoramento.NaoExibirGraficoDeAlertas, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.PersonalizadoCentroResultado = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.CentroResultado.multiplesEntities, codEntity: this.CentroResultado.codEntity, text: Localization.Resources.Logistica.Monitoramento.CentroDeResultado, idBtnSearch: guid(), visible: ko.observable(false), val: this.CentroResultado.val });
    this.PersonalizadoResponsavelVeiculo = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.ResponsavelVeiculo.multiplesEntities, codEntity: this.ResponsavelVeiculo.codEntity, text: Localization.Resources.Logistica.Monitoramento.ResponsavelPeloVeiculo, idBtnSearch: guid(), visible: ko.observable(false), val: this.ResponsavelVeiculo.val });
    this.PersonalizadoFronteiraRotaFrete = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.FronteiraRotaFrete.multiplesEntities, codEntity: this.FronteiraRotaFrete.codEntity, text: Localization.Resources.Logistica.Monitoramento.FronteiraRotaFrete, idBtnSearch: guid(), visible: ko.observable(false), val: this.FronteiraRotaFrete.val });
    this.PersonalizadoPaisOrigem = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.PaisOrigem.multiplesEntities, codEntity: this.PaisOrigem.codEntity, text: Localization.Resources.Logistica.Monitoramento.PaisOrigem, idBtnSearch: guid(), visible: ko.observable(false), val: this.PaisOrigem.val });
    this.PersonalizadoPaisDestino = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.PaisDestino.multiplesEntities, codEntity: this.PaisDestino.codEntity, text: Localization.Resources.Logistica.Monitoramento.PaisDestino, idBtnSearch: guid(), visible: ko.observable(false), val: this.PaisDestino.val });
    this.PersonalizadoApenasMonitoramentosCriticos = PropertyEntity({ getType: typesKnockout.bool, val: this.ApenasMonitoramentosCriticos.val, def: false, text: Localization.Resources.Logistica.Monitoramento.ApenasMonitoramentosCriticos, visible: ko.observable(false) });
    this.PersonalizadoDataInicioCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicioCarregamento, getType: typesKnockout.dateTime, val: ko.observable(this.DataInicioCarregamento.val), def: ko.observable(this.DataInicioCarregamento.val), enable: ko.observable(true), visible: ko.observable(false) });
    this.PersonalizadoDataFimCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFimCarregamento, getType: typesKnockout.dateTime, val: ko.observable(this.DataFimCarregamento.val), def: ko.observable(this.DataFimCarregamento.val), enable: ko.observable(true), visible: ko.observable(false) });
    this.PersonalizadoPossuiRecebedor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.PossuiRecebedor, val: this.PossuiRecebedor.val, options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(false) });
    this.PersonalizadoPossuiExpedidor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.PossuiExpedidor, val: this.PossuiExpedidor.val, options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(false) });
    this.PersonalizadoRecebedores = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.Recebedores.multiplesEntities, codEntity: this.Recebedores.codEntity, text: Localization.Resources.Logistica.Monitoramento.Recebedor, idBtnSearch: guid(), visible: ko.observable(false), val: this.Recebedores.val });
    this.PersonalizadoDestinatario = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.Destinatario.multiplesEntities, codEntity: this.Destinatario.codEntity, text: Localization.Resources.Logistica.Monitoramento.Destinatario, idBtnSearch: guid(), visible: ko.observable(false), val: ko.observable(this.Destinatario.val) });
    this.PersonalizadoRemetente = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.Remetente.multiplesEntities, codEntity: this.Remetente.codEntity, text: Localization.Resources.Logistica.Monitoramento.Remetente, idBtnSearch: guid(), visible: ko.observable(false), val: this.Remetente.val });
    this.PersonalizadoCodigoCargaEmbarcadorMulti = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.CodigoCargaEmbarcadorMulti.multiplesEntities, codEntity: this.CodigoCargaEmbarcadorMulti.codEntity, text: ko.observable(Localization.Resources.Logistica.Monitoramento.MultiplosNumerosCarga), idBtnSearch: guid(), visible: ko.observable(false), multiplesEntitiesConfig: { propDescricao: "CodigoCargaEmbarcador" }, val: this.CodigoCargaEmbarcadorMulti.val });
    this.PersonalizadoTipoTrecho = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.TipoTrecho.multiplesEntities, codEntity: this.TipoTrecho.codEntity, text: Localization.Resources.Logistica.Monitoramento.TipoTrecho, idBtnSearch: guid(), visible: ko.observable(false), val: this.TipoTrecho.val });
    this.PersonalizadoTipoCarga = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.TipoCarga.multiplesEntities, codEntity: this.TipoCarga.codEntity, text: Localization.Resources.Logistica.Monitoramento.TipoCarga, idBtnSearch: guid(), visible: ko.observable(false), val: this.TipoCarga.val });
    this.PersonalizadoProdutos = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.Produtos.multiplesEntities, codEntity: this.Produtos.codEntity, text: Localization.Resources.Logistica.Monitoramento.Produtos, idBtnSearch: guid(), visible: ko.observable(false), val: this.Produtos.val });
    this.PersonalizadoDataRealEntrega = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataRealEntrega, getType: typesKnockout.date, val: ko.observable(this.DataRealEntrega.val), def: ko.observable(this.DataRealEntrega.val), placeholder: Localization.Resources.Gerais.Geral.DataEntregaPedidoInicial, visible: ko.observable(false) });
    this.PersonalizadoVeiculosEmLocaisTracking = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(this.VeiculosEmLocaisTracking), def: false, text: Localization.Resources.Logistica.Monitoramento.VeiculosEmLocais, visible: ko.observable(false) });
    this.PersonalizadoVeiculoNoRaio = PropertyEntity({ getType: typesKnockout.bool, val: this.VeiculoNoRaio.val, def: false, text: Localization.Resources.Logistica.Monitoramento.VeiculoNoRaio, visible: ko.observable(false) });
    this.PersonalizadoLocaisTracking = PropertyEntity({ type: types.multiplesEntities, multiplesEntities: this.LocaisTracking.multiplesEntities, codEntity: this.LocaisTracking.codEntity, text: Localization.Resources.Logistica.Monitoramento.LocaisTracking, idBtnSearch: guid(), visible: ko.observable(false), val: this.LocaisTracking.val });
    this.PersonalizadoCodigoMotorista = PropertyEntity({ type: types.entity, codEntity: this.CodigoMotorista.codEntity, val: this.CodigoMotorista.val, text: Localization.Resources.Logistica.Monitoramento.Motorista, idBtnSearch: guid(), visible: ko.observable(false) });
    this.PersonalizadoCanalVenda = PropertyEntity({ type: types.entity, codEntity: this.CanalVenda.codEntity, val: this.CanalVenda.val, text: Localization.Resources.Logistica.Monitoramento.CanalVenda, idBtnSearch: guid(), visible: ko.observable(false) });
    this.PersonalizadoTipoCobrancaMultimodal = PropertyEntity({ val: this.TipoCobrancaMultimodal.val, options: EnumTipoCobrancaMultimodal.obterOpcoes(), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ModalTransporte, def: EnumTipoCobrancaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.PersonalizadoMesoregiao = PropertyEntity({ type: types.multiplesEntities, codEntity: this.Mesoregiao.codEntity, text: Localization.Resources.Pessoas.Pessoa.Mesoregiao, idBtnSearch: guid(), visible: ko.observable(true), val: this.Mesoregiao.val });
    this.PersonalizadoRegiao = PropertyEntity({ type: types.multiplesEntities, codEntity: this.Regiao.codEntity, text: Localization.Resources.Pessoas.Pessoa.Regiao, idBtnSearch: guid(), visible: ko.observable(true), val: this.Regiao.val });
    this.PersonalizadoVendedor = PropertyEntity({ type: types.multiplesEntities, codEntity: this.Vendedor.codEntity, text: Localization.Resources.Logistica.Monitoramento.Vendedor, idBtnSearch: guid(), visible: ko.observable(true), val: this.Vendedor.val });
    this.PersonalizadoSupervisor = PropertyEntity({ type: types.multiplesEntities, codEntity: this.Supervisor.codEntity, text: Localization.Resources.Logistica.Monitoramento.Supervisor, idBtnSearch: guid(), visible: ko.observable(true), val: this.Supervisor });
    this.PersonalizadoColetaNoPrazo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.ColetaNoPrazo, val: this.ColetaNoPrazo.val, options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(false) });
    this.PersonalizadoEntregaNoPrazo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EntregaNoPrazo.getFieldDescription(), val: this.EntregaNoPrazo.val, options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(false) });
    this.PersonalizadoTendenciaProximaColeta = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TendenciaColeta, val: this.TendenciaProximaColeta.val, options: EnumFiltroTendenciaPrazoEntrega.obterOpcoes(), def: EnumFiltroTendenciaPrazoEntrega.Nenhum, getType: typesKnockout.selectMultiple, visible: ko.observable(false) });
    this.PersonalizadoTendenciaProximaEntrega = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TendenciaEntrega, val: this.TendenciaProximaEntrega.val, options: EnumFiltroTendenciaPrazoEntrega.obterOpcoes(), def: EnumFiltroTendenciaPrazoEntrega.Nenhum, getType: typesKnockout.selectMultiple, visible: ko.observable(false) });
    this.PersonalizadoTipoMercadoria = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TipoMercadoria, col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.Carga, val: ko.observable(this.TipoMercadoria.val), visible: ko.observable(false), def: ko.observable(this.TipoMercadoria.val) });
    this.PersonalizadoRotaFrete = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.RotaFrete, col: 12, placeholder: Localization.Resources.Logistica.Monitoramento.RotaFrete, val: ko.observable(this.RotaFrete.val), visible: ko.observable(false), def: ko.observable(this.RotaFrete.val) });
    this.PersonalizadoParqueada = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Parqueada, val: this.Parqueada.val, options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(false) });
    this.PersonalizadoSituacaoIntegracaoSM = PropertyEntity({ text: Localization.Resources.Gerais.Geral.SituacaoIntegracao, options: _situacaoIntegracao, val: ko.observable(_situacaoIntegracao[0]), def: _situacaoIntegracao[0], issue: 0, visible: ko.observable(true) });
    this.PersonalizadoSomenteUltimoPorCarga = PropertyEntity({ getType: typesKnockout.bool, val: this.SomenteUltimoPorCarga.val, def: false, text: Localization.Resources.Logistica.Monitoramento.Apenasultimomonitoramentoporcarga, visible: ko.observable(false) });
    this.RastreadorOnlineOffline = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(null) });

    this.PersonalizadoSituacaoIntegracaoSM.val.subscribe(function (val) {
        _pesquisaMonitoramentoMapa.SituacaoIntegracaoSM.val(_pesquisaMonitoramentoMapa.PersonalizadoSituacaoIntegracaoSM.val());
    });
};

var FiltrosMapaMonitoramentoNovo = function () {
    this.MapaMonitoramentoFiltroStatusViagem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: false, text: Localization.Resources.Logistica.Monitoramento.EtapaMonitoramento, visible: ko.observable(true) });
    this.MapaMonitoramentoFiltroGrupoStatusViagem = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento), def: false, text: Localization.Resources.Logistica.Monitoramento.ExibirPorGrupoDeStatus, visible: ko.observable(true) });
    this.MapaMonitoramentoFiltroSituacaoCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: false, text: Localization.Resources.Logistica.Monitoramento.SituacaoDaCarga, visible: ko.observable(true) });
    this.MapaMonitoramentoFiltroAlerta = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: false, text: Localization.Resources.Logistica.Monitoramento.Alertas, visible: ko.observable(true) });
    this.MapaMonitoramentoFiltroStatusAlerta = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: false, text: Localization.Resources.Logistica.Monitoramento.StatusAlertas, visible: ko.observable(true) });
    this.MapaMonitoramentoVeiculosEmLocaisRaioProximidade = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Logistica.Monitoramento.VeiculosEmLocaisRaioProximidade, visible: ko.observable(true) });

    this.MapaMonitoramentoTendenciaEntrega = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Logistica.Monitoramento.TendenciaEntrega, visible: ko.observable(true) });

    this.MapaMonitoramentoFiltroStatusViagem.val.subscribe(function (val) {
        $("#legenda-StatusViagem").hide();
        if (val)
            $("#legenda-StatusViagem").show();
        else
            _configuracaoLegendaUsuarioMonitoramentoMapa.StatusViagem.val(val);
    });
    this.MapaMonitoramentoFiltroSituacaoCarga.val.subscribe(function (val) {
        $("#legenda-SituacaoCarga").hide();
        if (val)
            $("#legenda-SituacaoCarga").show();
    });
    this.MapaMonitoramentoFiltroAlerta.val.subscribe(function (val) {
        $("#legenda-Alertas").hide();
        if (val)
            $("#legenda-Alertas").show();
    });
    this.MapaMonitoramentoFiltroStatusAlerta.val.subscribe(function (val) {
        $("#legenda-StatusAlertas").hide();
        if (val)
            $("#legenda-StatusAlertas").show();
    });

    this.MapaMonitoramentoVeiculosEmLocaisRaioProximidade.val.subscribe(function (val) {
        $("#legenda-Veiculos-em-Locais-Raio-Proximidade").hide();
        if (val && _pesquisaMonitoramentoMapa.MostrarRaiosProximidade.val())
            $("#legenda-Veiculos-em-Locais-Raio-Proximidade").show();
    });

    this.MapaMonitoramentoTendenciaEntrega.val.subscribe(function (val) {
        $("#legenda-Tendencia-Entrega").hide();
        if (val)
            $("#legenda-Tendencia-Entrega").show();
    }
    );
}
var PlayerMonitoramento = function () {
    this.Play = PropertyEntity({
        type: types.event, text: "Play", idGrid: guid(),
        eventClick: function (e) {
            if (_timerInterval == null) {
                $('#' + _playerMonitoramento.Play.id).addClass("disabled");
                $('#' + _playerMonitoramento.Pause.id).removeClass("disabled");
                _timerInterval = setInterval(function () {
                    if (_countSeconds == 0) {
                        _countSeconds = _totalSeconds + 1;
                        var posicao = _mapaEntregas.getCenter();

                        recarregarDadosMonitoramentoNovo(false, true, {
                            Zoom: _mapaEntregas.getZoom(),
                            Latitude: posicao.lat,
                            Longitude: posicao.lng
                        });
                    }
                    _countSeconds--;
                    $('#knockoutPlayerMonitoramento span.count-seconds').html(_countSeconds);
                }, 1000);
            }
        }
    });
    this.Pause = PropertyEntity({
        type: types.event, text: "Pause", idGrid: guid(),
        eventClick: function (e) {
            if (_timerInterval != null) {
                $('#' + _playerMonitoramento.Play.id).removeClass("disabled");
                $('#' + _playerMonitoramento.Pause.id).addClass("disabled");
                clearInterval(_timerInterval);
                _timerInterval = null;
            }
        }
    });
    this.Refresh = PropertyEntity({
        type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, idGrid: guid(),
        eventClick: function (e) {
            var posicao = _mapaEntregas.getCenter();

            recarregarDadosMonitoramentoNovo(false, true, {
                Zoom: _mapaEntregas.getZoom(),
                Latitude: posicao.lat,
                Longitude: posicao.lng
            })
        }
    });
};

var CabecalhoMonitoramentoMapa = function () {
    this.BotaoVisualizacaoTexto = PropertyEntity({ val: ko.observable("Visualizar por ") });

    this.MostrarMapaMonitoramento = PropertyEntity({
        eventClick: function (e) {
            controlarExibicoesMapaEGridMonitoramentoMapa(EnumControlarExibicaoMapaGrid.Mapa);
            e.BotaoVisualizacaoTexto.val("Mapa Monitoramento");
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), text: "Mapa Monitoramento"
    });
    this.MostrarGridMonitoramento = PropertyEntity({
        eventClick: function (e) {
            controlarExibicoesMapaEGridMonitoramentoMapa(EnumControlarExibicaoMapaGrid.Grid);
            e.BotaoVisualizacaoTexto.val("Grid Monitoramento");
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), text: "Grid Monitoramento"
    });
    this.MostrarGridEMapaMonitoramento = PropertyEntity({
        eventClick: function (e) {
            controlarExibicoesMapaEGridMonitoramentoMapa(EnumControlarExibicaoMapaGrid.MapaEGrid);
            e.BotaoVisualizacaoTexto.val("Mapa e Grid Monitoramento");
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true), text: "Mapa e Grid Monitoramento"
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            Global.abrirModal('divModalFiltrosMonitoramentoMapa');
        }, type: types.event, idFade: guid(), visible: ko.observable(true)
    });
    this.CofiguracaoMonitoramentoMapa = PropertyEntity({
        eventClick: function (e) {
            Global.abrirModal('divModalConfiguracaoMonitoramentoMapa');
        }, type: types.event, idFade: guid(), visible: ko.observable(true)
    });
}

var ConfiguracaoIndicadorUsuarioMonitoramentoMapa = function () {
    this.SelecionarTodos = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.SelecionarTodos, visible: ko.observable(true), val: ko.observable(false) });
    this.StatusViagem = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.EtapaMonitoramento, visible: ko.observable(true) });
    this.SituacaoCarga = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.SituacaoDaCarga, visible: ko.observable(true) });
    this.Alertas = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.Alertas, visible: ko.observable(true) });
    this.StatusAlertas = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.StatusAlertas, visible: ko.observable(true) });
    this.TendenciaProximaEntrega = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.TendenciaProximaParada, visible: ko.observable(true) });
    this.RastreadorOnlineOffline = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.Rastreador, visible: ko.observable(true) });
    this.GrupoTipoOperacao = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.GrupoTiposOperacao, visible: ko.observable(true) });
    this.ExibirGridLateral = PropertyEntity({ val: ko.observable(false), def: true, getType: typesKnockout.bool, text: "Exibir Grid Lateral", visible: ko.observable(true) });

    this.SelecionarTodos.val.subscribe(function (val) {
        selecionarTodosCheckboxesDaClasse(_configuracaoIndicadorUsuarioMonitoramentoMapa, val);
    });

    this.StatusViagem.val.subscribe(function (val) {
        if (val) {
            adicionarCarouselItem("StatusDaViagem", Localization.Resources.Logistica.Monitoramento.EtapaMonitoramento);
        } else {
            removerCarouselItem("StatusDaViagem");
        }

    });
    this.SituacaoCarga.val.subscribe(function (val) {
        if (val) {
            adicionarCarouselItem("SituacaoDaCarga", Localization.Resources.Logistica.Monitoramento.SituacaoDaCarga);
        } else {
            removerCarouselItem("SituacaoDaCarga");
        }
    });
    this.Alertas.val.subscribe(function (val) {
        if (val) {
            adicionarCarouselItem("Alertas", Localization.Resources.Logistica.Monitoramento.Alertas);
        } else {
            removerCarouselItem("Alertas");
        }
    });
    this.TendenciaProximaEntrega.val.subscribe(function (val) {
        if (val) {
            adicionarCarouselItem("TendenciaEntrega", Localization.Resources.Logistica.Monitoramento.TendenciaProximaParada);
        } else {
            removerCarouselItem("TendenciaEntrega");
        }
    });
    this.RastreadorOnlineOffline.val.subscribe(function (val) {
        if (val) {
            adicionarCarouselItem("FarolEspelhamento", Localization.Resources.Logistica.Monitoramento.FarolDeEspelhamento);
        } else {
            removerCarouselItem("FarolEspelhamento");
        }
    });
    this.GrupoTipoOperacao.val.subscribe(function (val) {
        if (val) {
            adicionarCarouselItem("GrupoTipoOperacao", Localization.Resources.Logistica.Monitoramento.GrupoTiposOperacao);
        } else {
            removerCarouselItem("GrupoTipoOperacao");
        }
    });
}

var ConfiguracaoLegendaUsuarioMonitoramentoMapa = function () {
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Gerais.Geral.SelecionarTodos, visible: ko.observable(true) });
    this.StatusViagem = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.EtapaMonitoramento, visible: ko.observable(true) });
    this.SituacaoCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.SituacaoDaCarga, visible: ko.observable(true) });
    this.Alertas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.Alertas, visible: ko.observable(true) });
    this.StatusAlertas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.StatusAlertas, visible: ko.observable(true) });
    this.VeiculosEmLocaisRaioProximidade = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.VeiculosEmLocaisRaioProximidade, visible: ko.observable(false) });
    this.TendenciaProximaEntrega = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.TendenciaEntrega, visible: ko.observable(true) });
    this.ExibirPorGrupoStatusViagem = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.ExibirPorGrupoDeStatus, visible: ko.observable(true) });
    this.RastreadorOnlineOffline = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Logistica.Monitoramento.ExibirPorGrupoDeStatus, visible: ko.observable(true) });

    this.SelecionarTodos.val.subscribe(function (val) {
        selecionarTodosCheckboxesDaClasse(_configuracaoLegendaUsuarioMonitoramentoMapa, val);
    });

    this.StatusViagem.val.subscribe(function (val) {
        $("#legenda-StatusViagem").hide();
        if (val)
            $("#legenda-StatusViagem").show();
        else
            _configuracaoLegendaUsuarioMonitoramentoMapa.StatusViagem.val(val);
    });
    this.SituacaoCarga.val.subscribe(function (val) {
        $("#legenda-SituacaoCarga").hide();
        if (val)
            $("#legenda-SituacaoCarga").show();
    });
    this.Alertas.val.subscribe(function (val) {
        $("#legenda-Alertas").hide();
        if (val)
            $("#legenda-Alertas").show();
    });
    this.StatusAlertas.val.subscribe(function (val) {
        $("#legenda-StatusAlertas").hide();
        if (val)
            $("#legenda-StatusAlertas").show();
    });

    this.TendenciaProximaEntrega.val.subscribe(function (val) {
        $("#legenda-Tendencia-Entrega").hide();
        if (val)
            $("#legenda-Tendencia-Entrega").show();
    });
}

var SalvarConfiguracaoUsuarioMonitoramentoMapa = function () {
    this.Salvar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, visible: ko.observable(true), eventClick: salvarConfiguracaoUsuarioMonitoramentoMapa });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDroppable() {
    $("#container-grid-monitoramentoNovo").droppable({
        drop: itemSoltado,
        hoverClass: "ui-state-active backgroundDropHover",
    });
}

function loadMonitoramentoNovo() {
    loadPesquisaMonitoramentoNovo();
    loadFiltrosPersonalizadosMonitoramentoNovo();
    loadGridMonitoramentoNovo();
    buscarFiltrosPesquisaPersonalizado();

    buscarDetalhesOperador(function () {

        buscaStatusViagemMonitoramentoNovo(function () {
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) {
                loadDroppable();
            }

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Fornecedor) {
                _pesquisaMonitoramentoMapa.NaoExibirResumosAlerta.val(true);
                _pesquisaMonitoramentoMapa.NaoExibirResumosAlerta.visible(false);
            }

            BuscarTransportadores(_pesquisaMonitoramentoMapa.Transportador, null, null, true);
            BuscarClientes(_pesquisaMonitoramentoMapa.Cliente);
            BuscarCategoriaPessoa(_pesquisaMonitoramentoMapa.CategoriaPessoa);
            BuscarGruposPessoas(_pesquisaMonitoramentoMapa.GrupoPessoa);
            BuscarFilial(_pesquisaMonitoramentoMapa.Filial);
            BuscarVeiculos(_pesquisaMonitoramentoMapa.Veiculo);
            BuscarFuncionario(_pesquisaMonitoramentoMapa.FuncionarioVendedor);
            BuscarTiposOperacao(_pesquisaMonitoramentoMapa.TipoOperacao);
            BuscarClientes(_pesquisaMonitoramentoMapa.Expedidor);
            BuscarLocalidades(_pesquisaMonitoramentoMapa.Origem);
            BuscarLocalidades(_pesquisaMonitoramentoMapa.Destino);
            BuscarEstados(_pesquisaMonitoramentoMapa.EstadoOrigem);
            BuscarEstados(_pesquisaMonitoramentoMapa.EstadoDestino);
            BuscarFuncionario(_pesquisaMonitoramentoMapa.ResponsavelVeiculo);
            BuscarCentroResultado(_pesquisaMonitoramentoMapa.CentroResultado);
            BuscarClientes(_pesquisaMonitoramentoMapa.FronteiraRotaFrete, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
            BuscarPaises(_pesquisaMonitoramentoMapa.PaisOrigem);
            BuscarPaises(_pesquisaMonitoramentoMapa.PaisDestino);
            BuscarLocais(_pesquisaMonitoramentoMapa.LocaisTracking, null, null, 4);
            BuscarLocais(_pesquisaMonitoramentoMapa.LocaisRaioProximidade, null, null, 7);
            BuscarCargas(_pesquisaMonitoramentoMapa.CodigoCargaEmbarcadorMulti);
            BuscarClientes(_pesquisaMonitoramentoMapa.Destinatario);
            BuscarClientes(_pesquisaMonitoramentoMapa.Remetente);
            BuscarClientes(_pesquisaMonitoramentoMapa.Recebedores);
            BuscarClienteComplementar(_pesquisaMonitoramentoMapa.ClientesComplementar);
            BuscarTiposTrecho(_pesquisaMonitoramentoMapa.TipoTrecho);
            BuscarMotoristas(_pesquisaMonitoramentoMapa.CodigoMotorista);
            BuscarTiposdeCarga(_pesquisaMonitoramentoMapa.TipoCarga);
            BuscarProdutos(_pesquisaMonitoramentoMapa.Produtos);
            BuscarCanaisVenda(_pesquisaMonitoramentoMapa.CanalVenda);
            BuscarMesoRegiao(_pesquisaMonitoramentoMapa.Mesoregiao);
            BuscarFuncionario(_pesquisaMonitoramentoMapa.Vendedor);
            BuscarFuncionario(_pesquisaMonitoramentoMapa.Supervisor);
            BuscarRegioes(_pesquisaMonitoramentoMapa.Regiao);
            BuscarTransportadores(_pesquisaMonitoramentoMapa.PersonalizadoTransportador, null, null, true);
            BuscarClientes(_pesquisaMonitoramentoMapa.PersonalizadoCliente);
            BuscarCategoriaPessoa(_pesquisaMonitoramentoMapa.PersonalizadoCategoriaPessoa);
            BuscarGruposPessoas(_pesquisaMonitoramentoMapa.PersonalizadoGrupoPessoa);
            BuscarFilial(_pesquisaMonitoramentoMapa.PersonalizadoFilial);
            BuscarVeiculos(_pesquisaMonitoramentoMapa.PersonalizadoVeiculo);
            BuscarFuncionario(_pesquisaMonitoramentoMapa.PersonalizadoFuncionarioVendedor);
            BuscarTiposOperacao(_pesquisaMonitoramentoMapa.PersonalizadoTipoOperacao);
            BuscarClientes(_pesquisaMonitoramentoMapa.PersonalizadoExpedidor);
            BuscarLocalidades(_pesquisaMonitoramentoMapa.PersonalizadoOrigem);
            BuscarLocalidades(_pesquisaMonitoramentoMapa.PersonalizadoDestino);
            BuscarEstados(_pesquisaMonitoramentoMapa.PersonalizadoEstadoOrigem);
            BuscarEstados(_pesquisaMonitoramentoMapa.PersonalizadoEstadoDestino);
            BuscarFuncionario(_pesquisaMonitoramentoMapa.PersonalizadoResponsavelVeiculo);
            BuscarCentroResultado(_pesquisaMonitoramentoMapa.PersonalizadoCentroResultado);
            BuscarClientes(_pesquisaMonitoramentoMapa.PersonalizadoFronteiraRotaFrete, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
            BuscarPaises(_pesquisaMonitoramentoMapa.PersonalizadoPaisOrigem);
            BuscarPaises(_pesquisaMonitoramentoMapa.PersonalizadoPaisDestino);
            BuscarLocais(_pesquisaMonitoramentoMapa.PersonalizadoLocaisTracking, null, null, 4);
            BuscarCargas(_pesquisaMonitoramentoMapa.PersonalizadoCodigoCargaEmbarcadorMulti);
            BuscarClientes(_pesquisaMonitoramentoMapa.PersonalizadoDestinatario);
            BuscarClientes(_pesquisaMonitoramentoMapa.PersonalizadoRemetente);
            BuscarClientes(_pesquisaMonitoramentoMapa.PersonalizadoRecebedores);
            BuscarTiposTrecho(_pesquisaMonitoramentoMapa.PersonalizadoTipoTrecho);
            BuscarMotoristas(_pesquisaMonitoramentoMapa.PersonalizadoCodigoMotorista);
            BuscarTiposdeCarga(_pesquisaMonitoramentoMapa.PersonalizadoTipoCarga);
            BuscarProdutos(_pesquisaMonitoramentoMapa.PersonalizadoProdutos);
            BuscarCanaisVenda(_pesquisaMonitoramentoMapa.PersonalizadoCanalVenda);
            BuscarMesoRegiao(_pesquisaMonitoramentoMapa.PersonalizadoMesoregiao);
            BuscarRegioes(_pesquisaMonitoramentoMapa.PersonalizadoRegiao);
            BuscarFuncionario(_pesquisaMonitoramentoMapa.PersonalizadoVendedor);
            BuscarFuncionario(_pesquisaMonitoramentoMapa.PersonalizadoSupervisor);
            BuscarEventosMonitoramento(_pesquisaMonitoramentoMapa.TipoAlertaEvento);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _pesquisaMonitoramentoMapa.Filial.visible(false);
                _pesquisaMonitoramentoMapa.Transportador.visible(false);
                _pesquisaMonitoramentoMapa.GrupoPessoa.visible(true);
                _pesquisaMonitoramentoMapa.VeiculosComContratoDeFrete.visible(false);
                _pesquisaMonitoramentoMapa.NumeroEXP.visible(false);
            }

            let cssClass = "col col-xs-12 col-sm-2 col-md-2 col-lg-2";

            if (_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramento) {
                buscaGrupoTipoOperacao(_pesquisaMonitoramentoMapa);
                _pesquisaMonitoramentoMapa.GrupoTipoOperacao.visible(true);
                if (_CONFIGURACAO_TMS.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem) {
                    _pesquisaMonitoramentoMapa.StatusViagem.visible(false);
                } else {
                    _pesquisaMonitoramentoMapa.StatusViagem.visible(true);
                    cssClass = "col col-xs-12 col-sm-1 col-md-1 col-lg-1";
                }
            } else {
                _pesquisaMonitoramentoMapa.GrupoTipoOperacao.visible(false);
            }
            _pesquisaMonitoramentoMapa.DataInicial.cssClass(cssClass);
            _pesquisaMonitoramentoMapa.DataFinal.cssClass(cssClass);

            $("#" + _pesquisaMonitoramentoMapa.FiltroCliente.id).change(verificarPesquisaFiltroClienteMonitoramento);
            $("#" + _pesquisaMonitoramentoMapa.PersonalizadoFiltroCliente.id).change(verificarPesquisaFiltroClienteMonitoramento);

            $('.nav-tabs a').click(function (e) {
                e.preventDefault();
                $('#tabsFiltros .tab-content').each(function (i, tabContent) {
                    $(tabContent).children().each(function (z, el) {
                        $(el).removeClass('active');
                    });
                });
                $(this).tab('show');
            });

            loadCRUDTratativaAlerta();
            loadFiltroPesquisaMonitoramentoNovo();
            loadHistoricoMonitoramento();
            loadAlteracaoDataPrevisoes();
            loadCRUDAdicionarPosicao();
            loadEnviarNotificacaoApp();
            obterConfiguracaoUsuarioMonitoramentoMapa();

            $("#knoutMapaMonitoramento").show();
            loadMapaMonitoramento();
            _mapaEntregas = new MapaMonitoramento(_mapaMonitoramentoMapa, _mapaGoogle);
            loadPlayerMonitoramento();
            loadMonitoramentoControleEntrega(function () {
                registraComponente();
                loadEtapasControleEntrega();

                isMobile = $(window).width() <= 980;
                _containerControleEntrega = new ContainerControleEntrega();
                KoBindings(_containerControleEntrega, "knoutContainerControleEntrega");
            });

            carregarPreferenciasExibicaoMonitoramento();
            carregarFiltrosPesquisaInicialMonitoramentoNovo();

            if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.MonitoramentoMapa_PermiteAlterarAbaFiltrosPersonalizados, _PermissoesPersonalizadasMonitoramentoNovo))
                _pesquisaMonitoramentoMapa.FiltrosPersonalizados.visible(true);
        }, _pesquisaMonitoramentoMapa.StatusViagem, _pesquisaMonitoramentoMapa.PersonalizadoStatusViagem);
    });
}
function loadPlayerMonitoramento() {
    $('#knockoutPlayerMonitoramento span.count-seconds').html(_countSeconds);
    _playerMonitoramento = new PlayerMonitoramento();
    KoBindings(_playerMonitoramento, "knockoutPlayerMonitoramento", false, _playerMonitoramento.Play.id);
    $('#' + _playerMonitoramento.Pause.id).addClass("disable");

}

function abrirBuscaFiltrosManual(e) {
    var buscaFiltros = new BuscarModeloFiltroPesquisa(e.ModeloFiltrosPesquisa, function (retorno) {
        if (retorno.Codigo !== 0) {
            e.ModeloFiltrosPesquisa.codEntity(retorno.Codigo);
            e.ModeloFiltrosPesquisa.val(retorno.ModeloDescricao);

            PreencherJsonFiltroPesquisaMonitoramentoNovo(_pesquisaMonitoramentoMapa, retorno.Dados);
        }
    },
        EnumCodigoFiltroPesquisa.Monitoramento);

    buscaFiltros.AbrirBusca();
}

function verificarPesquisaFiltroClienteMonitoramento() {
    var categoria = false, cliente = false;
    if (_pesquisaMonitoramentoMapa.FiltroCliente.val() != EnumMonitoramentoFiltroCliente.Nenhum || _pesquisaMonitoramentoMapa.PersonalizadoFiltroCliente.val() != EnumMonitoramentoFiltroCliente.Nenhum) {
        categoria = true;
        cliente = true;
    }
    _pesquisaMonitoramentoMapa.CategoriaPessoa.enable(categoria);
    _pesquisaMonitoramentoMapa.Cliente.enable(cliente);
    _pesquisaMonitoramentoMapa.PersonalizadoCliente.enable(cliente);
}

function loadPesquisaMonitoramentoNovo() {
    _pesquisaMonitoramentoMapa = new PesquisaMonitoramentoNovo();
    KoBindings(_pesquisaMonitoramentoMapa, "knockoutPesquisaMonitoramentoNovo", false, _pesquisaMonitoramentoMapa.PesquisarNovoMonitoramento.id);

    _cabecalhoMonitoramentoMapa = new CabecalhoMonitoramentoMapa();
    KoBindings(_cabecalhoMonitoramentoMapa, "knockoutCabecalhoMonitoramentoMapa", false);

    _configuracaoIndicadorUsuarioMonitoramentoMapa = new ConfiguracaoIndicadorUsuarioMonitoramentoMapa();
    KoBindings(_configuracaoIndicadorUsuarioMonitoramentoMapa, "knockoutConfiguracaoIndicadorUsuarioMonitoramentoMapa", false);

    _configuracaoLegendaUsuarioMonitoramentoMapa = new ConfiguracaoLegendaUsuarioMonitoramentoMapa();
    KoBindings(_configuracaoLegendaUsuarioMonitoramentoMapa, "knockoutConfiguracaoLegendaUsuarioMonitoramentoMapa", false);

    _salvarConfiguracaoUsuarioMonitoramentoMapa = new SalvarConfiguracaoUsuarioMonitoramentoMapa();
    KoBindings(_salvarConfiguracaoUsuarioMonitoramentoMapa, "knockoutSalvarConfiguracaoUsuarioMonitoramentoMapa", false);
}

function loadGridMonitoramentoNovo() {
    const draggableRows = false;
    const limiteRegistros = 100;
    const totalRegistrosPorPagina = 100;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Fornecedor) {
        const opcaoModalDetalhesTorre = { descricao: Localization.Resources.Logistica.Monitoramento.DetalhesTorre, id: guid(), evento: "onclick", metodo: visualizarDetalhesTorreClick, tamanho: "10", icone: "" };
        const opcaoDetalheMonitoramento = { descricao: Localization.Resources.Logistica.Monitoramento.DetalhesDoMonitoramento, id: guid(), evento: "onclick", metodo: visualizarDetalhesMonitoramentoClick, tamanho: "10", icone: "" };
        const opcaoAlertas = { descricao: Localization.Resources.Logistica.Monitoramento.DetalhesDosAlertas, id: guid(), evento: "onclick", metodo: visualizarAlertas, tamanho: "10", icone: "", visible: ko.observable() };
        const opcaoResumoCarga = { descricao: Localization.Resources.Logistica.Monitoramento.ResumoDasEntregas, id: guid(), evento: "onclick", metodo: visualizarResumoDaCargaClick, tamanho: "10", icone: "" };
        const opcaoDetalhesCarga = { descricao: Localization.Resources.Logistica.Monitoramento.DetalhesDaCarga, id: guid(), evento: "onclick", metodo: carregarDetalhesCargaClick, tamanho: "10", icone: "" };
        const opcaoDetalhesPedidos = { descricao: Localization.Resources.Logistica.Monitoramento.DetalhesDosPedidos, id: guid(), evento: "onclick", metodo: exibirModalDetalhesPedidoClick, tamanho: "10", icone: "" };
        const opcaoDetalhesEntrega = { descricao: Localization.Resources.Logistica.Monitoramento.DetalhesDaEntrega, id: guid(), evento: "onclick", metodo: visualizarDetalhesEntregaClick, tamanho: "10", icone: "" };
        const opcaoVisualizarMapa = { descricao: Localization.Resources.Logistica.Monitoramento.VisualizarNoMapa, id: guid(), evento: "onclick", metodo: visualizarMapaClick, tamanho: "10", icone: "" };
        const opcaoHistoricos = { descricao: Localization.Resources.Logistica.Monitoramento.Historicos, id: guid(), evento: "onclick", metodo: visualizarHistoricosClick, tamanho: "8", icone: "" };
        const opcaoAlterarMonitoramentoCarga = { descricao: Localization.Resources.Logistica.Monitoramento.BuscarHistoricoMonitoramentoCargaCanceladaCcompativel, id: guid(), evento: "onclick", metodo: visualizarCargasCanceladasCompativeis, tamanho: "10", icone: "" };
        const opcaoAlterarNumeroRastreador = { descricao: Localization.Resources.Logistica.Monitoramento.AlterarNumeroDoRastreador, id: guid(), evento: "onclick", metodo: visualizarAlteracaoNumeroRastreador, tamanho: "10", icone: "", visibilidade: visualizarOpcaoAlterarNumeroRastreador };
        const opcaoAlterarDataPrevisoes = { descricao: Localization.Resources.Logistica.Monitoramento.AlterarDatasPrevisoes, id: guid(), evento: "onclick", metodo: visualizarAlteracaoDataPrevisoes, tamanho: "10", icone: "", visibilidade: visualizarOpcaoAlteracaoDataPrevisoes };
        const opcaoAdicionarPosicaoManual = { descricao: Localization.Resources.Logistica.Monitoramento.AdicionarPosicaoVeiculo, id: guid(), evento: "onclick", metodo: visualizarAdicionarPosicaoManualmente, tamanho: "10", icone: "", visibilidade: visualizarOpcaoMonitoramentoOff };
        const opcaoEnviarNotificacaoApp = { descricao: Localization.Resources.Logistica.Monitoramento.NotificarMotoristaSuperApp, id: guid(), evento: "onclick", metodo: exibirModalEnviarNotificacaoAppClick, tamanho: "10", icone: "", visibilidade: _CONFIGURACAO_TMS.UtilizaAppTrizy };

        const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [opcaoModalDetalhesTorre, opcaoDetalheMonitoramento, opcaoAlertas, opcaoResumoCarga, opcaoDetalhesCarga, opcaoDetalhesPedidos, opcaoDetalhesEntrega, opcaoVisualizarMapa, opcaoHistoricos, opcaoAlterarMonitoramentoCarga, opcaoAlterarNumeroRastreador, opcaoAlterarDataPrevisoes, opcaoAdicionarPosicaoManual, opcaoEnviarNotificacaoApp], tamanho: 5, };
        const configuracoesExportacao = { url: "MonitoramentoNovo/ExportarPesquisa", titulo: Localization.Resources.Logistica.Monitoramento.Monitoramentos, exportarPorRelatorio: true };

        _gridMonitoramentoNovo = new GridView("grid-monitoramentoNovo", "MonitoramentoNovo/Pesquisa", _pesquisaMonitoramentoMapa, menuOpcoes, null, totalRegistrosPorPagina, null, true, draggableRows, undefined, limiteRegistros, undefined, configuracoesExportacao, undefined, undefined, undefined, gridMonitoramentoCallbackColumnDefault);
    } else {
        // portal cliente
        const opcaoVisualizarMapa = { descricao: Localization.Resources.Logistica.Monitoramento.VisualizarNoMapa, id: guid(), evento: "onclick", metodo: visualizarMapaClick, tamanho: "10", icone: "" };
        const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [opcaoVisualizarMapa], tamanho: 5, };
        const configuracoesExportacao = { url: "Monitoramento/ExportarPesquisa", titulo: Localization.Resources.Logistica.Monitoramento.Monitoramentos };

        _gridMonitoramentoNovo = new GridView("grid-monitoramentoNovo", "MonitoramentoNovo/Pesquisa", _pesquisaMonitoramentoMapa, menuOpcoes, null, totalRegistrosPorPagina, null, true, draggableRows, undefined, limiteRegistros, undefined, configuracoesExportacao, undefined, undefined, undefined, gridMonitoramentoCallbackColumnDefault, null);
    }
    _gridMonitoramentoNovo.SetCallbackDrawGridView(RecarregarComFiltros);
    _gridMonitoramentoNovo.SetPermitirEdicaoColunas(true);
    _gridMonitoramentoNovo.SetSalvarPreferenciasGrid(true);
    _gridMonitoramentoNovo.SetHabilitarModelosGrid(true);
    _gridMonitoramentoNovo.SetHabilitarScrollHorizontal(true, 200);

    if (_CONFIGURACAO_TMS.UtilizaAppTrizy)
        $('#bCargasNotificacao').show();

    atualizarVisibilidadeCardListaLateral([]);
}

function RecarregarComFiltros() {
    _mapaEntregas.RecarregarMapaComDadosFiltrados(_mapaEntregas, _mapaEntregas.ObterDadosFiltrados(_mapaEntregas, _gridMonitoramentoNovo), _dadosLocaisRaioProximidade);
    _mapaEntregas.RecarregarFiltrosAplicados(_mapaEntregas);
}

function loadFiltroPesquisaMonitoramentoNovo() {
    var data = { TipoFiltro: EnumCodigoFiltroPesquisa.Monitoramento };

    executarReST("ModeloFiltroPesquisa/ObterFiltroPesquisaPadrao", data, function (res) {
        if (res.Success && Boolean(res.Data)) {
            PreencherJsonFiltroPesquisaMonitoramentoNovo(_pesquisaMonitoramentoMapa, res.Data.Dados);
            _pesquisaMonitoramentoMapa.ModeloFiltrosPesquisa.codEntity(res.Data.Codigo);
            _pesquisaMonitoramentoMapa.ModeloFiltrosPesquisa.val(res.Data.Descricao);
        }
    });
}
//RODOLFO TREVISOL - TEMPORARIO ATÉ DEFINIÇÃO DO FORMATO CORRETO DE SALVAR A ABA PERSONALIZADA.
function PreencherJsonFiltroPesquisaMonitoramentoNovo(knocout, jsonFiltroPesquisa) {
    let filtroPesquisa = JSON.parse(jsonFiltroPesquisa);
    PreencherObjetoFiltroPesquisaMonitoramentoNovo(knocout, filtroPesquisa)
}

function PreencherObjetoFiltroPesquisaMonitoramentoNovo(knocout, filtroPesquisa) {
    if ((knocout) && (filtroPesquisa)) {
        $.each(knocout, function (propName, prop) {
            if (!propName.includes('Personalizado')) {
                let objFiltroPesquisa = ObterFiltroPesquisaPorDescricao(propName, filtroPesquisa);
                if (objFiltroPesquisa) {
                    if (prop.type == types.entity) {
                        prop.codEntity(objFiltroPesquisa.codEntity);
                        prop.val(objFiltroPesquisa.val);
                        prop.entityDescription(objFiltroPesquisa.entityDescription);
                    }
                    else if (prop.type == types.multiplesEntities) {
                        if ($.isArray(objFiltroPesquisa)) {
                            prop.multiplesEntities(objFiltroPesquisa);
                        }
                    }
                    else if (prop.type == types.map) {

                        prop.val(objFiltroPesquisa.val);
                    }
                    else if (prop.type == types.event) {
                        //Não mapear eventos
                    }
                    else {
                        //Implementar outros tipos
                    }
                }
            }
        });
    }
}
//RODOLFO TREVISOL - TEMPORARIO ATÉ DEFINIÇÃO DO FORMATO CORRETO DE SALVAR A ABA PERSONALIZADA.

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function itemSoltado(event, ui) {
    var idContainerDestino = event.target.id;
    var idContainerOrigem = "container-" + $(ui.draggable[0]).parent().parent()[0].id;
}

function buscaGrupoTipoOperacao() {
    executarReST("GrupoTipoOperacao/BuscarTodos", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _pesquisaMonitoramentoMapa.GrupoTipoOperacao.options(arg.Data.GrupoTipoOperacao);

                $("#" + _pesquisaMonitoramentoMapa.GrupoTipoOperacao.id).selectpicker('refresh');

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function visualizarDetalhesMonitoramentoClick(filaSelecionada) {
    exibirDetalhesMonitoramentoPorCodigo(filaSelecionada.Codigo);
}

function visualizarDetalhesTorreClick(filaSelecionada) {
    loadModalDetalhesTorre(filaSelecionada.Carga);
}

function visualizarAlertas(filaSelecionada) {
    atualizaTituloModalCargaMonitoramento(filaSelecionada);
    ExibirModalAlertas();
    loadAlertaDetalhe();
    loadGridAlertas(filaSelecionada.Carga, _pesquisaMonitoramentoMapa.DescricaoAlerta.val());
}

function atualizaTituloModalCargaMonitoramento(row) {
    $(".title-carga-codigo-embarcador").html(row.CargaEmbarcador);
    $(".title-carga-placa").html(
        (row.Tracao ? row.Tracao : "") + " " + (row.Reboques ? row.Reboques : "")
    );
}

function visualizarDetalhesCarga(filaSelecionada) {
    buscarDetalhesOperador(function () {
        atualizaTituloModalCarga(filaSelecionada);
        ObterDetalhesCargaFluxo(filaSelecionada.Carga);
    });
}

function carregarDetalhesCargaClick(linhaSelecionada) {
    buscarDetalhesOperador(function () {
        buscaStatusViagemMonitoramentoNovo(function () {
            loadDetalhesCarga(function () {

                visualizarDetalhesCargaClick(linhaSelecionada);

            });
        }, _pesquisaMonitoramentoMapa.StatusViagem, _pesquisaMonitoramentoMapa.PersonalizadoStatusViagem);
    });
}

function event_FiltroMapa_Click(tipo, element) {
    var filtro = {
        Type: tipo,
        Id: element.target.id,
        Value: element.target.value,
        Checked: element.target.checked
    };

    if (filtro.Checked)
        adicionarNosFiltros(filtro);
    else
        removerDosFiltros(filtro);

    RecarregarComFiltros();
}

//---------------------------Definição de funções para habilitar fullscreen na Div do Mapa, mantendo as legendas e assumindo Mapa como tela inteira.---------------------------------------
const fullscreenDivMapa = document.getElementById('divMapa');
const fullscreenDivMapaMonitoramento = document.getElementById('divMapaMonitoramento');
function mapaMonitoramentoFullScreen() {
    window.addEventListener('resize', resizeOverlay);
    resizeOverlay();
    if (!isFullscreen())
        fullscreenDivMapa.requestFullscreen();
    else
        document.exitFullscreen();
}
//------------------------------------------------------------------
function exibirModalDetalhesPedidoClick(carga) {
    loadDetalhesPedidosTorreControle(carga.Carga);
}
function alertaCardMapClickMonitoramento(carga) {
    loadTratativaAlerta({ CodigoAlerta: parseInt(carga.CodigoUltimoAlerta) }, []);
}

function exibirLegendaMapaMonitoramentoNovo() {
    Global.abrirModal('divModalLegendaMapaMonitoramentoNovo');
}

function criarAbaFiltrosPersonalizados() {
    Global.abrirModal('divModalCriarFiltroPersonalizado');
}

function controlarExibicoesMapaEGridMonitoramentoMapa(opcaoExibir) {
    if (opcaoExibir == EnumControlarExibicaoMapaGrid.Mapa) {
        $('.panel-mapa-monitoramento').show();
        $('.panel-grid-monitoramento').hide();
        $('.botao-dropdown-mapa').addClass('active');
        $('.botao-dropdown-grid, .botao-dropdown-grid-e-mapa').removeClass('active');
    } else if (opcaoExibir == EnumControlarExibicaoMapaGrid.Grid) {
        $('.panel-grid-monitoramento').show();
        $('.panel-mapa-monitoramento').hide();
        $('.botao-dropdown-grid').addClass('active');
        $('.botao-dropdown-mapa, .botao-dropdown-grid-e-mapa').removeClass('active');
    } else if (opcaoExibir == EnumControlarExibicaoMapaGrid.MapaEGrid) {
        $('.panel-grid-monitoramento, .panel-mapa-monitoramento').show();
        $('.botao-dropdown-grid-e-mapa').addClass('active');
        $('.botao-dropdown-grid, .botao-dropdown-mapa').removeClass('active');
    }
    localStorage.setItem('visualizacao-monitoramento-preferencia', opcaoExibir);
}

function salvarConfiguracaoUsuarioMonitoramentoMapa() {
    let data = {
        ConfiguracaoLegendaUsuarioMonitoramentoMapa: JSON.stringify(RetornarObjetoPesquisa(_configuracaoLegendaUsuarioMonitoramentoMapa)),
        ConfiguracaoIndicadorUsuarioMonitoramentoMapa: JSON.stringify(RetornarObjetoPesquisa(_configuracaoIndicadorUsuarioMonitoramentoMapa))
    };
    executarReST("MonitoramentoNovo/SalvarConfiguracaoUsuarioMonitoramentoMapa", data, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.Success, Localization.Resources.Gerais.Geral.Success, retorno.Msg);
            obterConfiguracaoUsuarioMonitoramentoMapa();
            Global.fecharModal('divModalConfiguracaoMonitoramentoMapa');
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas
function recarregarDadosMonitoramentoNovo(isModalDetalhesLocal, recontarFiltroCarrossel, MantemZoomMapa) {
    _gridMonitoramentoNovo.CarregarGrid(function (dados) {
        var data = RetornarObjetoPesquisa(_pesquisaMonitoramentoMapa);
        executarReST("MonitoramentoNovo/PesquisaMapa", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    _dadosGrid = arg.Data;

                    backup_cargasNoMapa = new Array();
                    _mapaEntregas._filtrosMapaAplicados = new Array();
                    _mapaEntregas.LimparMapa(_mapaEntregas);
                    _mapaEntregas.Load(_dadosGrid, _mapaMonitoramentoMapa, _mapaGoogle);
                    backup_cargasNoMapa = _mapaEntregas._cargasNoMapa;
                    _gridFiltradaPorStatus = 0;
                    buscarLocaisRaioProximidade();

                    if (recontarFiltroCarrossel)
                        consultarFiltroCarrossel();

                    if (isModalDetalhesLocal) {
                        carregarDadosAtualizadosGridVeiculosEmRaio(_mapaEntregas._cargasNoMapa);
                    }

                    if (MantemZoomMapa) {
                        _mapaEntregas.setView([MantemZoomMapa.Latitude, MantemZoomMapa.Longitude], 4);
                        _mapaEntregas.Latitude = MantemZoomMapa.Latitude;
                        _mapaEntregas.Longitude = MantemZoomMapa.Longitude;
                        setTimeout(function () {
                            _mapaEntregas.setZoom(MantemZoomMapa.Zoom);
                        }, 400);
                    }

                    if (_configuracaoIndicadorUsuarioMonitoramentoMapa.ExibirGridLateral.val()) {
                        loadSidebarMapaTabelaMonitoramento();
                        updateSidebarFrotaFromVeiculosMonitoramento(_dadosGrid);
                    }

                    atualizarVisibilidadeCardListaLateral(_dadosGrid);

                } else {
                    atualizarVisibilidadeCardListaLateral([]);
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            }
        });
    });
}

// #endregion Funções Públicas

// #region Funções Privadas
function adicionarNosFiltros(filtro) {
    _mapaEntregas._filtrosMapaAplicados.push(filtro);
}

function removerDosFiltros(filtro) {
    var novoArrayFiltros = new Array();
    for (var i = 0; i < _mapaEntregas._filtrosMapaAplicados.length; i++) {
        if (_mapaEntregas._filtrosMapaAplicados[i].Id != filtro.Id)
            novoArrayFiltros.push(_mapaEntregas._filtrosMapaAplicados[i]);
    }
    _mapaEntregas._filtrosMapaAplicados = novoArrayFiltros;
}

function existeNosFiltros(id) {
    var existe = false;
    for (var i = 0; i < _mapaEntregas._filtrosMapaAplicados.length; i++) {
        if (_mapaEntregas._filtrosMapaAplicados[i].Id == id) {
            existe = true;
            break;
        }
    }
    return existe
}

function resizeOverlay() {
    if (isFullscreen()) {
        fullscreenDivMapa.style.width = window.innerWidth + 'px';
        fullscreenDivMapa.style.height = window.innerHeight + 'px';
        fullscreenDivMapaMonitoramento.style.width = window.innerWidth + 'px';
        fullscreenDivMapaMonitoramento.style.height = window.innerHeight + 'px';
    } else {
        // Limpar tamanho para que a div volte ao tamanho original.
        fullscreenDivMapa.style.width = '';
        fullscreenDivMapa.style.height = '';
        fullscreenDivMapaMonitoramento.style.width = '';
        fullscreenDivMapaMonitoramento.style.height = '';
    }
}

function isFullscreen() {
    return document.fullscreenElement;
}

function buscarLocaisRaioProximidade() {
    if (!(_pesquisaMonitoramentoMapa.MostrarRaiosProximidade.val() && _pesquisaMonitoramentoMapa.LocaisRaioProximidade.val()))
        return;

    var data = { LocaisRaioProximidade: JSON.stringify(_pesquisaMonitoramentoMapa.LocaisRaioProximidade.multiplesEntities()) };
    executarReST("MonitoramentoNovo/ObterLocaisRaioProximidade", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data !== false) {
                _dadosLocaisRaioProximidade = retorno.Data;
                _mapaEntregas.LoadLocaisRaioProximidade(_dadosLocaisRaioProximidade);
                if (_dadosLocaisRaioProximidade && _dadosLocaisRaioProximidade.Locais && _dadosLocaisRaioProximidade.Locais.length > 0)
                    _configuracaoLegendaUsuarioMonitoramentoMapa.VeiculosEmLocaisRaioProximidade.val(true);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function obterDadosLocalRaioProximidade(local) {
    _localSelecionado = local;
    var htmlInfoLocal = "";

    var listaVeiculosNoRaioDoLocal = new Array();
    var listaVeiculosPorRaioNoLocal = new Array();
    listaVeiculosNoRaioDoLocal = _mapaEntregas._veiculosEmRaioProximidade[local.Codigo];
    var quantidadeVeiculosEmRaio = listaVeiculosNoRaioDoLocal == undefined ? 0 : listaVeiculosNoRaioDoLocal.length

    $("#veiculosNoRaioLocal").html("");
    $("#infoLocal").html("");
    $("#descricaoLocal").text("Detalhes do local - " + local.Descricao);
    _listaGridsVeiculosEmRaio = [];

    htmlInfoLocal +=
        '   <div class="row" style="margin-bottom:24px;">' +
        '      <div class="col-12 col-md-6">' +
        '          <div class="col"><span class="font-500 text-muted" style="font-size: 12px;">NOME DO LOCAL</span></div>' +
        '          <div class="col" style="font-size: 16px;font-weight: 500;">' + (local.Descricao) + '</div>' +
        '      </div>' +
        '      <div class="col-12 col-md-6">' +
        '          <div class="col"><span class="font-500 text-muted" style="font-size: 12px;">RAIOS CADASTRADOS</span></div>' +
        '          <div class="col" style="font-size: 16px;font-weight: 500;">' + (local.Raios.length) + '</div>' +
        '      </div>' +
        '   </div>' +
        '   <div style="margin-bottom:16px;">' +
        '       <h4 style="font-weight: 700">Veículos atualmente nos Raios do Local ' + '(' + quantidadeVeiculosEmRaio + ')' + '</h4>' +
        '   </div>'

    $("#infoLocal").html(htmlInfoLocal);

    if (listaVeiculosNoRaioDoLocal) {
        listaVeiculosPorRaioNoLocal = Object.groupBy(listaVeiculosNoRaioDoLocal, (x) => x.CodigoRaioProximidade);

        var header = [
            { data: "Codigo", visible: false },
            { data: "Carga", title: "Carga", width: "20%" },
            { data: "CidadeOrigem", title: "Origem", width: "20%" },
            { data: "CidadeDestino", title: "Destino ", width: "20%" },
            { data: "Produtos", title: "Produtos", width: "20%" },
            { data: "TipoCarga", title: "Tipo de Carga", width: "20%" },
            { data: "Tracao", title: "Tração", width: "20%" },
            { data: "Reboques", title: "Reboques", width: "20%" },
        ];
        var keys = Object.keys(listaVeiculosPorRaioNoLocal);

        for (i = 0; i < keys.length; i++) {
            var key = keys[i];

            var htmlTables = "";

            htmlTables +=
                ' <div>' +
                '   <h4 style="font-weight: 700">' + listaVeiculosPorRaioNoLocal[key][0].DescricaoRaioProximidade + "(" + listaVeiculosPorRaioNoLocal[key][0].RaioRaioProximidade + ")" + '</h4>' +
                '   <table width="100%" class="table table-bordered table-hover" id="table' + key + '" cellspacing="0"></table>' +
                ' </div>'

            $("#veiculosNoRaioLocal").append(htmlTables);

            idBasicDataTable = "table" + key;

            _gridVeiculosEmLocaisRaioProximidade = new BasicDataTable(idBasicDataTable, header, null);
            _gridVeiculosEmLocaisRaioProximidade.CarregarGrid(listaVeiculosPorRaioNoLocal[key]);

            _listaGridsVeiculosEmRaio.push({ Id: idBasicDataTable, IdLocal: local.Codigo, IdRaio: key, Grid: _gridVeiculosEmLocaisRaioProximidade });
        }
    }

    Global.abrirModal('divModalDetalhesLocalRaioProximidade');
    countSeconds();
}

function countSeconds() {
    limparTimer();
    if (_timerAtualizacaoVeiculosEmRaio == null) {
        _timerAtualizacaoVeiculosEmRaio = setInterval(function () {
            if (_countSeconds == 0) {
                _countSeconds = _totalSeconds + 1;
                recarregarDadosMonitoramentoNovo(true, true, false);
            }
            _countSeconds--;
            $('#contador-segundos-atualizacao-veiculos span.contador-segundos').html(_countSeconds);
        }, 1000);
    }
}

function carregarDadosAtualizadosGridVeiculosEmRaio(cargasNoMapa) {
    if (cargasNoMapa && _localSelecionado && _listaGridsVeiculosEmRaio && _listaGridsVeiculosEmRaio.length > 0) {
        for (i = 0; i < _listaGridsVeiculosEmRaio.length; i++) {
            var listaVeiculosNoRaio = cargasNoMapa.filter((x) => x.CodigoRaioProximidade == _listaGridsVeiculosEmRaio[i].IdRaio);
            _listaGridsVeiculosEmRaio[i].Grid.CarregarGrid(listaVeiculosNoRaio);
        }

        countSeconds();
    }
}

function limparTimer() {
    _timerAtualizacaoVeiculosEmRaio = null;
    _totalSeconds = 300;
    _countSeconds = _totalSeconds;
}

function buscaStatusViagemMonitoramentoNovo(callback, statusViagem, statusViagemPersonalizado) {
    executarReST("MonitoramentoStatusViagem/BuscarTodos", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var selected = [];
                for (var i = 0; i < arg.Data.StatusViagem.length; i++) {
                    if (arg.Data.StatusViagem[i].selected == 'selected') {
                        selected.push(arg.Data.StatusViagem[i].value);
                    }
                }
                statusViagem.options(arg.Data.StatusViagem);
                statusViagem.val(selected);

                if (statusViagemPersonalizado != undefined) {
                    statusViagemPersonalizado.options(arg.Data.StatusViagem);
                    statusViagemPersonalizado.val(selected);
                    $("#" + statusViagemPersonalizado.id).selectpicker('refresh');
                }

                $("#" + statusViagem.id).selectpicker('refresh');

                if (statusViagemPersonalizado && statusViagemPersonalizado.val()) {
                    statusViagem.val(statusViagemPersonalizado.val());
                    $("#" + statusViagem.id).selectpicker('refresh');
                }

                if (callback) callback();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function exibirModalEnviarNotificacaoAppClick(carga) {
    let listaCargas;
    if (carga)
        listaCargas = [carga];
    else
        listaCargas = _mapaEntregas.ObterDadosFiltrados(_mapaEntregas, _gridMonitoramentoNovo);
    exibirModalEnviarNotificacaoApp(listaCargas, true);
}

function carregarPreferenciasExibicaoMonitoramento() {
    const preferenciaSalva = localStorage.getItem('visualizacao-monitoramento-preferencia');

    const preferencia = preferenciaSalva !== null
        ? parseInt(preferenciaSalva)
        : EnumControlarExibicaoMapaGrid.MapaEGrid;

    switch(preferencia) {
        case EnumControlarExibicaoMapaGrid.Mapa:
            _cabecalhoMonitoramentoMapa.BotaoVisualizacaoTexto.val("Mapa Monitoramento");
            break;
        case EnumControlarExibicaoMapaGrid.Grid:
            _cabecalhoMonitoramentoMapa.BotaoVisualizacaoTexto.val("Grid Monitoramento");
            break;
        case EnumControlarExibicaoMapaGrid.MapaEGrid:
            _cabecalhoMonitoramentoMapa.BotaoVisualizacaoTexto.val("Mapa e Grid Monitoramento");
            break;
    }

    controlarExibicoesMapaEGridMonitoramentoMapa(preferencia);
}

function obterConfiguracaoUsuarioMonitoramentoMapa() {
    executarReST("MonitoramentoNovo/ObterConfiguracaoUsuarioMonitoramentoMapa", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                let configuracaoExibicaoIndicadores = JSON.parse(retorno.Data.ConfiguracaoExibicaoIndicadores);
                let configuracaoExibicaoLegendaMapa = JSON.parse(retorno.Data.ConfiguracaoExibicaoLegendaMapa);

                PreencherObjetoKnout(_configuracaoIndicadorUsuarioMonitoramentoMapa, { Data: configuracaoExibicaoIndicadores });
                PreencherObjetoKnout(_configuracaoLegendaUsuarioMonitoramentoMapa, { Data: configuracaoExibicaoLegendaMapa });

                atualizarExibicaoGridLateral();
                controlarVisibilidadeLegendasMapa();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function controlarVisibilidadeLegendasMapa() {
    if (_configuracaoLegendaUsuarioMonitoramentoMapa.StatusViagem.val())
        $("#legenda-StatusViagem").show();
    else
        $("#legenda-StatusViagem").hide();

    if (_configuracaoLegendaUsuarioMonitoramentoMapa.SituacaoCarga.val())
        $("#legenda-SituacaoCarga").show();
    else
        $("#legenda-SituacaoCarga").hide();

    if (_configuracaoLegendaUsuarioMonitoramentoMapa.Alertas.val())
        $("#legenda-Alertas").show();
    else
        $("#legenda-Alertas").hide();

    if (_configuracaoLegendaUsuarioMonitoramentoMapa.StatusAlertas.val())
        $("#legenda-StatusAlertas").show();
    else
        $("#legenda-StatusAlertas").hide();

    if (_configuracaoLegendaUsuarioMonitoramentoMapa.VeiculosEmLocaisRaioProximidade.val())
        $("#legenda-Veiculos-em-Locais-Raio-Proximidad").show();
    else
        $("#legenda-Veiculos-em-Locais-Raio-Proximidad").hide();

    if (_configuracaoLegendaUsuarioMonitoramentoMapa.TendenciaProximaEntrega.val())
        $("#legenda-Tendencia-Entrega").show();
    else
        $("#legenda-Tendencia-Entrega").hide();
}

function InserirItensStatusViagemFiltroCarrossel(dados) {
    const listaStatusViagem = usarGrupoStatusViagem
        ? Object.groupBy(dados, (x) => x.GrupoStatusViagemCodigo)
        : Object.groupBy(dados, (x) => x.TiporRegraViagem);

    const listaSituacaoCarga = Object.groupBy(dados, (x) => x.SituacaoCarga);
    const listaTendenciaEntrega = Object.groupBy(
        dados.filter(x => x.TendenciaProximaParada !== 0),
        (x) => x.TendenciaProximaParada
    );

    const listaAlertas = Object.groupBy(dados, (x) => x.PossuiAlertaEmAberto);
    const listaFarolEspelhamentoOnline = dados.filter(x => x.RastreadorOnlineOffline === 3);
    const listaFarolEspelhamentoOffline = dados.filter(x => x.RastreadorOnlineOffline === 1 || x.RastreadorOnlineOffline === 4);
    const listaGrupoTipoOperacao = Object.groupBy(dados, (x) => x.CodigoGrupoTipoOperacao);

    function gerarItensFiltro(lista, descricaoFunc, corFunc) {
        const keyStatus = Object.keys(lista);
        const itens = [];

        for (let i = 0; i < keyStatus.length; i++) {
            const chave = keyStatus[i];
            itens.push({
                Enum: chave,
                Descricao: descricaoFunc(lista[chave][0]),
                Cor: corFunc(lista[chave][0]),
                Quantidade: lista[chave].length
            });
        }

        return itens;
    }

    function desenharListaFiltro(carrosselId, lista, tipoFiltro) {
        for (let i = 0; i < lista.length; i++) {
            const item = lista[i];
            desenharCarrosselFiltro(carrosselId, item.Descricao, item.Enum, item.Cor, item.Quantidade, tipoFiltro);
        }
    }

    function limparEDesenharFiltro(lista, carrosselId, descricaoFunc, corFunc, tipoFiltro) {
        if (lista) {
            limparDivFiltroCarrossel(carrosselId);
            const itens = gerarItensFiltro(lista, descricaoFunc, corFunc);
            desenharListaFiltro(carrosselId, itens, tipoFiltro);
        }
    }

    if (listaStatusViagem) {
        if (usarGrupoStatusViagem) {
            limparEDesenharFiltro(
                listaStatusViagem,
                'view-selection-items-StatusDaViagem',
                item => item.GrupoStatusViagemDescricao,
                item => item.GrupoStatusViagemCor,
                'StatusViagem'
            );
        } else {
            limparEDesenharFiltro(
                listaStatusViagem,
                'view-selection-items-StatusDaViagem',
                item => item.StatusViagem,
                item => item.CorStatusViagem,
                'StatusViagem'
            );
        }
    }

    if (listaSituacaoCarga) {
        limparEDesenharFiltro(
            listaSituacaoCarga,
            'view-selection-items-SituacaoDaCarga',
            item => item.SituacaoCargaDescricao,
            item => item.SituacaoCargaCor,
            'SituacaoCarga'
        );
    }

    if (listaTendenciaEntrega) {
        limparEDesenharFiltro(
            listaTendenciaEntrega,
            'view-selection-items-TendenciaEntrega',
            item => item.TendenciaEntregaDescricao,
            item => item.CorTendenciaEntrega,
            'TendenciaEntrega'
        );
    }

    if (listaAlertas) {
        limparEDesenharFiltro(
            listaAlertas,
            'view-selection-items-Alertas',
            item => item.PossuiAlertaEmAberto == true ? 'Com Alertas' : 'Sem Alertas',
            () => "",
            'Alertas'
        );
    }

    if (listaGrupoTipoOperacao) {
        limparEDesenharFiltro(
            listaGrupoTipoOperacao,
            'view-selection-items-GrupoTipoOperacao',
            item => item.GrupoTipoOperacao,
            item => item.GrupoTipoOperacaoCor,
            'GrupoTipoOperacao'
        );
    }

    if (listaFarolEspelhamentoOnline) {
        limparDivFiltroCarrossel('view-selection-items-FarolEspelhamento');

        const quantidadeOnline = listaFarolEspelhamentoOnline.length;
        const quantidadeOffline = listaFarolEspelhamentoOffline.length;
        const total = quantidadeOnline + quantidadeOffline;

        const porcentagemOnline = total > 0 ? ((quantidadeOnline / total) * 100).toFixed(0) : (quantidadeOnline > 0 ? 100 : 0);
        const porcentagemOffline = total > 0 ? ((quantidadeOffline / total) * 100).toFixed(0) : (quantidadeOffline > 0 ? 100 : 0);

        let listaOnlineOffline = [];

        listaOnlineOffline.push({
            Enum: 1,
            Descricao: `Online - ${porcentagemOnline}%`,
            Cor: "#00C853",
            Quantidade: quantidadeOnline,
            Percentual: Number(porcentagemOnline)
        });

        listaOnlineOffline.push({
            Enum: 0,
            Descricao: `Offline - ${porcentagemOffline}%`,
            Cor: "#D50000",
            Quantidade: quantidadeOffline,
            Percentual: Number(porcentagemOffline)
        });

        desenharListaFiltro('view-selection-items-FarolEspelhamento', listaOnlineOffline, 'FarolEspelhamento');
    }

    atualizarIndicadores();
    const carouselItems = document.querySelectorAll(".carousel-item");
    const indicatorButtons = document.querySelectorAll(".carousel-indicators .indicator-button");

    carouselItems.forEach((item, index) => {
        const itemId = item.id;
        const indicatorButton = indicatorButtons[index];

        if (itemId && indicatorButton) {
            htmlCarrosselItems[itemId] = {
                carouselItemHTML: item.outerHTML,
                indicatorButtonHTML: indicatorButton.outerHTML
            };
        }
    });

    if (!_configuracaoIndicadorUsuarioMonitoramentoMapa.StatusViagem.val())
        removerCarouselItem('StatusDaViagem');
    if (!_configuracaoIndicadorUsuarioMonitoramentoMapa.SituacaoCarga.val())
        removerCarouselItem('SituacaoDaCarga');
    if (!_configuracaoIndicadorUsuarioMonitoramentoMapa.TendenciaProximaEntrega.val())
        removerCarouselItem('TendenciaEntrega');
    if (!_configuracaoIndicadorUsuarioMonitoramentoMapa.Alertas.val())
        removerCarouselItem('Alertas');
    if (!_configuracaoIndicadorUsuarioMonitoramentoMapa.RastreadorOnlineOffline.val())
        removerCarouselItem('FarolEspelhamento');
    if (!_configuracaoIndicadorUsuarioMonitoramentoMapa.GrupoTipoOperacao.val()) {
        removerCarouselItem('GrupoTipoOperacao');

    }
        
}

function desenharCarrosselFiltro(seletor, descricao, id, cor, quantidade, tipoFiltro) {
    if (!cor)
        cor = "#3EA7DB";

    $('#' + seletor).append(
        '<div class="view-selection" id="view-select-filters" onclick="FiltrarOpcoesFiltroCarroselClick(' + id + ', \'' + tipoFiltro + '\')" data-toggle="tooltip" data-placement="top" title="' + descricao + '">' +
        '  <div id="' + tipoFiltro + id + '" class="view-select-button">' +
        '     <div class="icon-number" style="color:' + cor + '">' + quantidade + '</div>' +
        '     <div style="width: 84px;overflow: hidden;text-overflow: ellipsis;white-space: nowrap; text-align: center;">' + descricao + '</div>' +
        '   </div>' +
        '</div>'
    );
}

function consultarFiltroCarrossel() {
    let data = RetornarObjetoPesquisa(_pesquisaMonitoramentoMapa);
    executarReST("MonitoramentoNovo/ObterDadosFiltroCarrosselMonitoramentoMapa", data, function (arg) {
        if (arg.Success) {

            if (arg.Data && arg.Data.length > 0) {
                InserirItensStatusViagemFiltroCarrossel(arg.Data);
                $('#carousel-filters').show();
                inicializarNavegacaoCarrosel();
            }
            else {
                $('#carousel-filters').hide();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

    });
}

function limparDivFiltroCarrossel(idDiv) {
    $('#' + idDiv).html('');
}

function FiltrarOpcoesFiltroCarroselClick(codigoStatus, tipoFiltro) {
    limparSelecaoCarrossel();

    if ($('#' + tipoFiltro + codigoStatus).hasClass("active")) {
        _pesquisaMonitoramentoMapa.GrupoStatusViagem.val(null);
        _pesquisaMonitoramentoMapa.MonitoramentoStatusViagemTipoRegra.val(null);
        _pesquisaMonitoramentoMapa.SituacaoCarga.val(null);
        _pesquisaMonitoramentoMapa.TendenciaEntrega.val(null);
        _pesquisaMonitoramentoMapa.RastreadorOnlineOffline.val(null);
        _pesquisaMonitoramentoMapa.ComAlertas.val(null);
        _pesquisaMonitoramentoMapa.GrupoTipoOperacao.val(null);

        recarregarDadosMonitoramentoNovo(false, false, false);
        $(".view-select-button.active").removeClass('active');
        return;
    } else {
        $(".view-select-button.active").removeClass('active');
        $('#' + tipoFiltro + codigoStatus).addClass('active');
    }

    if (tipoFiltro == 'StatusViagem') {
        if (usarGrupoStatusViagem)
            _pesquisaMonitoramentoMapa.GrupoStatusViagem.val(codigoStatus);
        else
            _pesquisaMonitoramentoMapa.MonitoramentoStatusViagemTipoRegra.val(codigoStatus == 0 ? -1 : codigoStatus);
    }

    else if (tipoFiltro == 'SituacaoCarga') {
        _pesquisaMonitoramentoMapa.SituacaoCarga.val(codigoStatus);
    }

    else if (tipoFiltro == 'TendenciaEntrega') {
        _pesquisaMonitoramentoMapa.TendenciaEntrega.val(codigoStatus);
    }

    else if (tipoFiltro == 'FarolEspelhamento') {
        _pesquisaMonitoramentoMapa.RastreadorOnlineOffline.val(codigoStatus);
    }

    else if (tipoFiltro == 'GrupoTipoOperacao') {
        _pesquisaMonitoramentoMapa.GrupoTipoOperacaoIndicador.val(codigoStatus);
    }

    else {
        if (codigoStatus == true)
            _pesquisaMonitoramentoMapa.ComAlertas.val(true);
        else
            _pesquisaMonitoramentoMapa.ComAlertas.val(false);
    }

    recarregarDadosMonitoramentoNovo(false, false, false);
}

function atualizarCarrosselVisibilidade() {
    let carousel = document.getElementById("carousel-filters");
    let itens = carousel.querySelectorAll(".carousel-item");
    if (itens.length === 0) {
        carousel.className = 'carousel d-none'
    } else {
        carousel.className = "carousel";
        inicializarNavegacaoCarrosel();
    }
}

function inicializarNavegacaoCarrosel() {
    var $carousel = $('#carousel-filters');
    var $wrap = $('.carousel-indicators.indicators-with-arrows');
    if (!$carousel.length || !$wrap.length) return;
    var inst = bootstrap.Carousel.getInstance($carousel[0])
        || bootstrap.Carousel.getOrCreateInstance($carousel[0], { wrap: false });
    var $prevWrap = $('#indicator-prev');
    var $nextWrap = $('#indicator-next');
    var $prevBtn = $prevWrap.find('[data-bs-slide="prev"]');
    var $nextBtn = $nextWrap.find('[data-bs-slide="next"]');

    function refresh() {
        inst = bootstrap.Carousel.getInstance($carousel[0])
            || bootstrap.Carousel.getOrCreateInstance($carousel[0], { wrap: false });

        var wrapEnabled = inst && inst._config && inst._config.wrap !== false; 
        var $items = $carousel.find('.carousel-item');
        var idx = $items.index($items.filter('.active'));

        $prevWrap.removeClass('is-disabled');
        $nextWrap.removeClass('is-disabled');
        $prevBtn.prop('disabled', false);
        $nextBtn.prop('disabled', false);

        if (!wrapEnabled) {
            if (idx <= 0) { $prevWrap.addClass('is-disabled'); $prevBtn.prop('disabled', true); }
            if (idx >= $items.length - 1) {
                $nextWrap.addClass('is-disabled');
                $nextBtn.prop('disabled', true);
            }
        }
    }
    $carousel.off('slid.bs.carousel.monitoramento').on('slid.bs.carousel.monitoramento', refresh);
    refresh();
}


function atualizarIndicadores() {
    const wrap = document.querySelector(".carousel-indicators");
    if (!wrap) return;
    wrap.querySelectorAll(".indicator-button").forEach(el => el.remove());
    const itens = document.querySelectorAll(".carousel-item");
    const anchorNext = document.getElementById("indicator-next");
    itens.forEach((item, index) => {
        const btn = document.createElement("button");
        btn.type = "button";
        btn.setAttribute("data-bs-target", "#carousel-filters");
        btn.setAttribute("data-bs-slide-to", index);
        btn.className = "indicator-button" + (index === 0 ? " active" : "");
        if (anchorNext && anchorNext.parentNode === wrap) {
            wrap.insertBefore(btn, anchorNext);
        } else {
            wrap.appendChild(btn);
        }
    });
}

function atualizarClasseAtiva() {
    let itens = document.querySelectorAll(".carousel-item");
    itens.forEach(item => item.classList.remove("active"));
    if (itens.length > 0) {
        itens[0].classList.add("active");
    }
}
function adicionarCarouselItem(itemId, textContent) {
    let carouselInner = document.querySelector(".carousel-inner");
    let itemExistente = document.getElementById(itemId);

    if (!itemExistente && htmlCarrosselItems[itemId]) {
        let template = document.createElement("div");
        template.innerHTML = htmlCarrosselItems[itemId].carouselItemHTML;
        let novoItem = template.firstElementChild;
        carouselInner.appendChild(novoItem);

        let indicatorsContainer = document.querySelector(".carousel-indicators");
        let indicatorTemplate = document.createElement("div");
        indicatorTemplate.innerHTML = htmlCarrosselItems[itemId].indicatorButtonHTML;
        let novoIndicator = indicatorTemplate.firstElementChild;
        indicatorsContainer.appendChild(novoIndicator);

        atualizarIndicadores();
        atualizarClasseAtiva();
        atualizarCarrosselVisibilidade();
    }
}


function removerCarouselItem(itemId) {
    let item = document.getElementById(itemId);
    if (item) {
        item.remove();

        atualizarClasseAtiva();
        atualizarIndicadores();
        atualizarCarrosselVisibilidade();
    }
}

function visualizarOpcaoAlteracaoDataPrevisoes(registroGrid) {
    return _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe && registroGrid.Status != EnumMonitoramentoStatus.Finalizado && registroGrid.Status != EnumMonitoramentoStatus.Cancelado;
}

function limparSelecaoCarrossel() {
    LimparCampo(_pesquisaMonitoramentoMapa.GrupoStatusViagem);
    LimparCampo(_pesquisaMonitoramentoMapa.MonitoramentoStatusViagemTipoRegra);
    LimparCampo(_pesquisaMonitoramentoMapa.SituacaoCarga);
    LimparCampo(_pesquisaMonitoramentoMapa.TendenciaEntrega);
    LimparCampo(_pesquisaMonitoramentoMapa.ComAlertas);
    LimparCampo(_pesquisaMonitoramentoMapa.RastreadorOnlineOffline);
    LimparCampo(_pesquisaMonitoramentoMapa.GrupoTipoOperacaoIndicador);
}

function carregarFiltrosPesquisaInicialMonitoramentoNovo() {
    const codigo = sessionStorage.getItem('codigoCarga');
    if (codigo) {
        LimparCampos(_pesquisaMonitoramentoMapa);
        _pesquisaMonitoramentoMapa.CodigoCargaEmbarcador.val(codigo || '');
        sessionStorage.removeItem('codigoCarga');
        recarregarDadosMonitoramentoNovo(false, true, false);
    }
}

function loadSidebarMapaTabelaMonitoramento() {
    if (!window.ko) return;

    const el = document.getElementById('lista-frota-container-mt');
    if (!el || el.getAttribute('data-ko-bound') === '1') return;

    function sidebarFrotaMapaTabelaMonitoramento() {
        const self = this;
        self.query = ko.observable('');
        self.sortMode = ko.observable('placa');
        self.items = ko.observableArray([]);

        self.orderAsc = ko.observable(false);
        self.toggleOrder = function () { self.orderAsc(!self.orderAsc()); };
        self.ordered = ko.pureComputed(function () {
            const base = self.filtered().slice();
            base.forEach(function (v, i) { if (typeof v.idx !== 'number') v.idx = i; });
            return base.sort(function (a, b) {
                return self.orderAsc() ? (a.idx - b.idx) : (b.idx - a.idx);
            });
        });

        self.filtered = ko.pureComputed(function () {
            const q = (self.query() || '').trim().toLowerCase();
            let arr = self.items();
            if (q) arr = arr.filter(v =>
                (v.placa || '').toLowerCase().includes(q) ||
                (v.carga || '').toLowerCase().includes(q)
            );

            if (self.sortMode() === 'status') {
                arr = arr.slice().sort((a, b) => {
                    if (a.online === b.online) return (a.placa || '').localeCompare(b.placa || '');
                    return a.online ? -1 : 1;
                });
            } else {
                arr = arr.slice().sort((a, b) => (a.placa || '').localeCompare(b.placa || ''));
            }
            return arr;
        });

        self.verNoMapa = function (v) {
            document.querySelectorAll('.pf-card.pf-card--custom').forEach(function (card) {
                card.classList.remove('pf-card--active');
            });

            if (!v || isNaN(v.lat) || isNaN(v.lng) || parseInt(v.lat) === 0 || parseInt(v.lng) === 0) {
                exibirMensagem(
                    tipoMensagem.aviso,
                    Localization.Resources.Gerais.Geral.Aviso,
                    Localization.Resources.Logistica.Monitoramento.LocalizacaoIndisponivelMapa
                );

                return;
            }

            // Centraliza o mapa na posição do veículo (zoom 16 é um bom valor para detalhe, ajuste se necessário)
            if (_mapaEntregas && typeof _mapaEntregas.setView === "function") {
                _mapaEntregas.setView([v.lat, v.lng], 16);
            }

            var card = document.querySelector('.pf-card.pf-card--custom[data-id="' + v.codigoMonitoramento + '"]');

            if (card) {
                card.classList.add('pf-card--active');
            }
        };

        self.abrirModalAlerta = function (item) {
            item.Carga = item.cargaCodigo;
            item.CargaEmbarcador = item.carga;
            item.Tracao = item.placa;
            item.Reboques = item.reboques;
            visualizarAlertas(item); 
        };

        self.abrirDetalhesTorre = function (item) {
            item.Carga = item.cargaCodigo;
            visualizarDetalhesTorreClick(item); 
        };
    }

    _vmSidebarMTMonitoramento = new sidebarFrotaMapaTabelaMonitoramento();
    ko.applyBindings(_vmSidebarMTMonitoramento, el);
    el.setAttribute('data-ko-bound', '1');

    const wrap = document.getElementById('pf-wrap-mt');
    const handle = wrap ? wrap.querySelector('.pf-handle') : null;

    handle.onclick = null;
    handle.addEventListener('click', function () {
        wrap.classList.toggle('is-closed');
        setTimeout(function () {
            if (window._novoMapaMT && _novoMapaMT.invalidateSize)
                _novoMapaMT.invalidateSize();
        }, 260);
    });
}

function updateSidebarFrotaFromVeiculosMonitoramento(dados) {
    if (!_vmSidebarMTMonitoramento) return;

    var mapDraw = new MapaDraw();
    
    const itens = Array.isArray(dados) ? dados.map(function (v, i) {

        return {
            id: v.Veiculo,
            carga: v.CargaEmbarcador,
            placa: v.Tracao || '-',
            cargaCodigo: v.Carga,
            reboques: v.Reboques,
            online: v.RastreadorOnlineOffline === 3,
            percentualViagem: Number(v.PercentualViagem) > 0 ? Number(v.PercentualViagem) : 0,
            lat: v.Latitude,
            lng: v.Longitude,
            wifi: ObterIconeWifi(ObterIconeStatusTracking(v.RastreadorOnlineOffline)),
            codigoMonitoramento: v.Codigo,
            iconeUltimoAlertaExibirTela: v.IconeUltimoAlertaExibirTela
        };
    }) : [];

    _vmSidebarMTMonitoramento.items(itens);
}

function ObterIconeWifi (svg) {
    return 'data:image/svg+xml,' + encodeURIComponent(svg);
};

function atualizarExibicaoGridLateral() {
    var mostrar = _configuracaoIndicadorUsuarioMonitoramentoMapa.ExibirGridLateral.val();

    var pfListAside = document.querySelector('#pf-wrap-mt > aside.pf-list');
    if (pfListAside) {
        pfListAside.style.display = mostrar ? '' : 'none';
    }

    var pfHandle = document.querySelector('#pf-wrap-mt .pf-handle');
    if (pfHandle) {
        pfHandle.style.display = mostrar ? '' : 'none';
    }
}

function atualizarVisibilidadeCardListaLateral(dados) {
    var pfListItems = document.querySelector('#pf-wrap-mt .pf-list__items');
    if (!pfListItems) return;
    if (Array.isArray(dados) && dados.length > 0) {
        pfListItems.style.display = '';
    } else {
        pfListItems.style.display = 'none';
    }
}
// #endregion Funções Privadas
