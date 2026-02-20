/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Configuracao/Sistema/OperadorLogistica.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/NotaFiscal.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Consultas/CanalVenda.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumStatusViagemControleEntrega.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumAlertaMonitorStatus.js" />
/// <reference path="Fluxo.js" />
/// <reference path="MontaEtapa.js" />
/// <reference path="Entrega.js" />
/// <reference path="GridMonitoramento.js" />
/// <reference path="Posicao.js" />
/// <reference path="InicioViagem.js" />
/// <reference path="FimViagem.js" />
/// <reference path="Alertas.js" />
/// <reference path="Evento.js" />
/// <reference path="ChatMobile.js" />
/// <reference path="DataAgendamento.js" />
/// <reference path="DataReagendamento.js" />
/// <reference path="RaioXCarga.js" />
/// <reference path="Monitoramento.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatus.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaControleEntrega;
var _containerControleEntrega;
var _cabecalhoPesquisaControleEntrega;
var _configuracaoExibicoesControleEntrega;
var _itensPorPagina = 100;
var isMobile = false;
var _executarPesquisa = false;
var _page = 1;
var _etapaAtualFluxo = null;
var _controleEntregaVisaoPrevisao = false;
var urlControleEntrega = "cargas/controleentrega";

var funcionarioPedido = {
    Vendedor: [],
    Supervisor: [],
    Gerente: []
};

var timeoutControleEntrega = 180000;
var opcoesSituacaoCarga = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? EnumSituacoesCarga.obterOpcoesEmbarcador() : EnumSituacoesCarga.obterOpcoesTMS();

var PesquisaControleEntrega = function () {
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Filial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Transportadores.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Vendedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Vendedor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Supervisor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Supervisor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Gerente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Gerente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataCriacaoInicio.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(Global.DataAtual()) });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataCriacaoFim.getFieldDescription(), getType: typesKnockout.date });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.NumeroCarga.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Pedido = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Pedidos.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Veiculos = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Veiculos.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "Placa", propCodigo: "Codigo" } });
    this.ExibirSomenteCargasComVeiculo = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirSomenteCargasComVeiculoDefinido, getType: typesKnockout.bool, val: ko.observable(true) });
    this.StatusViagem = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.SituacaoControleDeEntregas.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), def: new Array(), options: EnumStatusViagemControleEntrega.obterOpcoes() });
    this.ResponsavelEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.ResponsavelEntrega.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroNotaFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.NumeroNotasFiscais, idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroNota = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.NumeroNotaFiscal.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroPedidoCliente = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.NumeroPedidoCliente.getFieldDescription()), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataCarregamentoCargaInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataCarregamentoInicial.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataCarregamentoCargaFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataCarregamentoFinal.getFieldDescription(), getType: typesKnockout.dateTime });
    this.ExibirSomenteCargasComChamadoAberto = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirSomenteCargasComAtendimentoEmAberto, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomenteCargasComChatNaoLido = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirSomenteCargasComChatNaoLido, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomenteCargasComReentrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirSomenteCargasComPedidosDeReentrega, getType: typesKnockout.bool, val: ko.observable(false) });
    this.ExibirSomenteCargasComMotoristaAppDesatualizado = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirSomenteCargasComAppDesatualizado, getType: typesKnockout.bool, val: ko.observable(false) });
    this.OrdernarResultadosPorDataCriacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.OrdernarResultadosPorDataCriacao, getType: typesKnockout.bool, val: ko.observable(false) });
    this.SomenteCargasCriticas = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirSomenteCargasCriticas, getType: typesKnockout.bool, val: ko.observable(false) });
    this.SomenteCargasComPesquisaRecebedorPendenteResposta = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.SomenteCargasComPesquisaRecebedorPendenteResposta, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.ExisteCheckListAtivo) });
    this.CargasComEstadiasGeradas = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CargasComEstadiasGeradas.getFieldDescription(), val: ko.observable(null), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: null });
    this.SomenteCargaComEstadiaConfiguraca = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ExibirSomenteCargaComEstadiaConfiguraca, getType: typesKnockout.bool, val: ko.observable(false) });
    this.FilialVenda = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.FilialDeVenda.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Emitente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Emitente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Expedidor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataEntregaPedidoInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataEntregaPedidoInicial.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataEntregaPedidoFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataEntregaPedidoFinal.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataPrevisaoEntregaPedidoInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PrevisaoEntregaPedidoInicial.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataPrevisaoEntregaPedidoFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PrevisaoEntregaPedidoFinal.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataPrevisaoInicioViagemInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PrevisaoInicioViagemInicial.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataPrevisaoInicioViagemFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PrevisaoInicioViagemFinal.getFieldDescription(), getType: typesKnockout.dateTime });
    this.SerieNota = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.SerieNotaFiscal.getFieldDescription(), maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.DataEmissaoNotaDe = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.EmissaoNotaFiscalDe.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.DataEmissaoNotaAte = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Ate.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.DataEmissaoNotaDe.dateRangeLimit = this.DataEmissaoNotaAte;
    this.DataEmissaoNotaAte.dateRangeInit = this.DataEmissaoNotaDe;
    this.NumeroCTeDe = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroCteDe.getFieldDescription(), maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.NumeroCTeAte = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Ate.getFieldDescription(), maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.SerieCTe = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.SerieCte.getFieldDescription(), maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.DataEmissaoCTeDe = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.EmissaoCteDe.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.DataEmissaoCTeAte = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Ate.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.DataEmissaoCTeDe.dateRangeLimit = this.DataEmissaoCTeAte;
    this.DataEmissaoCTeAte.dateRangeInit = this.DataEmissaoCTeDe;
    this.EmpresaDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.EmpresaDestino.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.CidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.CidadeOrigem.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.CidadeDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.CidadeDestino.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.EstadoOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.EstadoOrigem.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.EstadoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.EstadoDestino.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.NumeroSolicitacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroSolicitacao.getFieldDescription(), maxlength: 100, enable: ko.observable(true), getType: typesKnockout.string, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.NumeroPedidoCF = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroPedidoCliente.getFieldDescription(), maxlength: 100, enable: ko.observable(true), getType: typesKnockout.string, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroPedidoEmbarcador.getFieldDescription(), maxlength: 100, enable: ko.observable(true), getType: typesKnockout.string, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.GrupoDePessoasDaCarga.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.TipoDaCarga.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.CodigoCargaEmbarcadorMulti = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.ControleEntrega.MultiplosNumerosCarga.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "CodigoCargaEmbarcador" } });
    this.Recebedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Recebedor.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CanalEntrega = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.ControleEntrega.CanalEntrega.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CanalVenda = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.ControleEntrega.CanalVenda.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.CentroDeCarregamento.getFieldDescription(), idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.CentroResultado.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.RetornarInformacoesMonitoramento = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.RetornarInformacoesDoMonitoramento, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.RetornarCargasQueMonitoro = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.RetornarApenasCargasQueEuMonitoro, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false) });
    this.DataProgramadaDescargaInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PrevisaoProgramadaDaDescargaInicial.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataProgramadaDescargaFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PrevisaoProgramadaDaDescargaFinal.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataAgendamentoInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataAgendamentoInicial.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataAgendamentoFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataAgendamentoFinal.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataProgramadaColetaInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataProgramadaColetaInicial.getFieldDescription(), getType: typesKnockout.dateTime });
    this.DataProgramadaColetaFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataProgramadaColetaFinal.getFieldDescription(), getType: typesKnockout.dateTime });
    this.CodigoSap = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.CodigoSAP), val: ko.observable(""), def: "", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiTMS) });
    this.PossuiRecebedor = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PossuiRecebedor.getFieldDescription(), val: ko.observable(null), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: null });
    this.PossuiExpedidor = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PossuiExpedidor.getFieldDescription(), val: ko.observable(null), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: null });
    this.ResponsavelVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.ResponsavelVeiculo.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.DataInicioViagemInicial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioViagemDe.getFieldDescription(), getType: typesKnockout.date });
    this.DataInicioViagemFinal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioViagemAte.getFieldDescription(), getType: typesKnockout.date });
    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Remetente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoTrecho = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.TipoTrecho.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoCarga = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.ControleEntrega.SituacaoCarga, val: ko.observable(EnumSituacoesCarga.NaLogistica), options: opcoesSituacaoCarga });
    this.StatusDaViagem = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.EtapaDoMonitoramento.getFieldDescription(), val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });
    this.OcorrenciaEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.OcorrenciaEntrega, val: ko.observable([]), def: new Array(), getType: typesKnockout.selectMultiple, options: ko.observable([]), visible: ko.observable(true) });
    this.TendenciaEntrega = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.ControleEntrega.FiltroTendenciaEntrega, val: ko.observable(EnumFiltroTendenciaPrazoEntrega.Nenhum), options: EnumFiltroTendenciaPrazoEntrega.obterOpcoes(), def: EnumFiltroTendenciaPrazoEntrega.Nenhum });

    this.TendenciaColeta = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Cargas.ControleEntrega.FiltroTendenciaColeta, val: ko.observable(EnumFiltroTendenciaPrazoEntrega.Nenhum), options: EnumFiltroTendenciaPrazoEntrega.obterOpcoes(), def: EnumFiltroTendenciaPrazoEntrega.Nenhum });

    this.ClienteComplementar = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.ClienteComplementar.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.MesoRegiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Mesoregiao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Regiao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Regiao.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoAlerta = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.TipoAlerta.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.NumeroOrdemPedido = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.NumeroOrdem), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.VeiculosNoRaio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.VeiculosNoRaio, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.TipoCobrancaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoCobrancaMultimodal.Nenhum), options: EnumTipoCobrancaMultimodal.obterOpcoes(), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ModalTransporte.getFieldDescription(), def: EnumTipoCobrancaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true) });
    this.MonitoramentoStatus = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.SituacaoDoMonitoramento.getFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), def: new Array(), options: EnumMonitoramentoStatus.obterOpcoes(), placeholder: Localization.Resources.Cargas.ControleEntrega.StatusMonitoramento });

    this.TipoMercadoria = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.TipoMercadoria.getFieldDescription() });

    this.EquipeVendas = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.EquipeVendas.getFieldDescription() });

    this.EscritorioVenda = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.EscritorioVenda.getFieldDescription() });
    this.RotaFrete = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Cargas.ControleEntrega.RotaFrete.getFieldDescription() });
    this.Matriz = PropertyEntity({ type: types.string, val: ko.observable(""), text: Localization.Resources.Logistica.Monitoramento.Matriz.getFieldDescription() });
    this.Parqueada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Parqueada.getFieldDescription(), val: ko.observable(null), options: Global.ObterOpcoesPesquisaBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: null });
    this.DataInicioAbate = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioAbate, getType: typesKnockout.dateTime });
    this.DataFimAbate = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataFimAbate, getType: typesKnockout.dateTime });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.DataEntregaPedidoInicial.dateRangeLimit = this.DataEntregaPedidoFinal;
    this.DataEntregaPedidoFinal.dateRangeInit = this.DataEntregaPedidoInicial;
    this.DataPrevisaoEntregaPedidoInicial.dateRangeLimit = this.DataPrevisaoEntregaPedidoFinal;
    this.DataPrevisaoEntregaPedidoFinal.dateRangeInit = this.DataPrevisaoEntregaPedidoInicial;
    this.DataInicioViagemInicial.dateRangeLimit = this.DataInicioViagemFinal;
    this.DataInicioViagemFinal.dateRangeInit = this.DataInicioViagemInicial;
    this.DataAgendamentoInicial.dateRangeLimit = this.DataAgendamentoFinal;
    this.DataAgendamentoFinal.dateRangeInit = this.DataAgendamentoInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            $('.aviso-filtragem-dados').hide();
            Global.fecharModal('divModalFiltrosPesquisa');
            $("#controle-entrega-conteudo-container").show();
            limparFiltrosControleEntregaRapido();
            ExibirCheckBoxCargaEntrega(false);
            obterControleEntregas(1, false, false);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }

            ValidarVisualizacaoSelecionarCargas();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }

            ValidarVisualizacaoSelecionarCargas();

        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function (e) {
            limparFiltrosControleEntrega();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.LimparFiltros, idGrid: guid(), visible: ko.observable(true)
    });
};

var ConfigurarSubscribesNosCheckBoxes = () => {
    _KnoutsEntregas.forEach(entrega => {
        entrega.Selecionado.val.subscribe(function (novoValor) {
            if (novoValor) entrega.CorFundo.val('#DBEEFF');
            else entrega.CorFundo.val(null);
            AtualizarBotaoConfirmarAssumir();
        })
        entrega.CorFundo.val(null);
        }
    );
};

var AtualizarBotaoConfirmarAssumir = () => {
    var algumSelecionado = _KnoutsEntregas.some(entrega =>
        entrega.PodeSelecionar() && entrega.Selecionado.val() === true
    );
    _containerControleEntrega.DelegarCargas.enable(algumSelecionado);
};

var ContainerControleEntrega = function () {
    this.Entregas = PropertyEntity({ val: ko.observable([]) });
    this.AssumirMonitoramentos = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: AssumirMonitoramentoClick, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.SelecionarCargas), visible: ko.observable(_operadorLogistica.PermitirAssumirCargasControleEntrega || _operadorLogistica.OperadorSupervisor) });
    this.DelegarCargas = PropertyEntity({ type: types.event, eventClick: DelegarCargasClick, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DelegarCargas), enable: ko.observable(false), visible: ko.observable(false) });
    this.AssumirCargas = PropertyEntity({ type: types.event, eventClick: AssumirCargasClick, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.AssumirCargas), enable: ko.observable(false), visible: ko.observable(false) });
    this.CancelarAcaoMonitoramentos = PropertyEntity({ eventClick: CancelarMonitoramentoClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.CancelarSelecao, visible: ko.observable(false) });
    this.SelecionarTodosCheckbox = PropertyEntity({ val: ko.observable(false), visible: ko.observable(false) })
    this.SelecionarTodosCheckbox.val.subscribe((value) => {
        selecionarTodosCheckBox(value);
    });

    this.ModoSelecao = PropertyEntity({ type: types.local, val: ko.observable(false) });
    this.CorFundo = PropertyEntity({ type: types.local, val: ko.observable(null) });
    this.PermiteDelegarMonitoramento = PropertyEntity({ val: ko.observable(true) });

    this.ImportacaoCargaFluvial = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Cargas.ControleEntrega.ImportacaoDeCargasFluviais,
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv,.txt",
        cssClass: "btn-default",
        UrlImportacao: "ControleEntrega/ImportacaoCargaFluvial",
        UrlConfiguracao: "ControleEntrega/ConfiguracaoImportacaoCargaFluvial",
        CodigoControleImportacao: EnumCodigoControleImportacao.O031_ControleEntregaCargaFluvial,
        CallbackImportacao: function () {
            obterControleEntregas(1, false, false);
        }
    });
    this.ConfiguracaoWidget = PropertyEntity({ type: types.event, eventClick: ConfiguracaoWidgetClick });
};

var ConfiguracaoExibicoesControleEntrega = function () {
    this.ConfiguracaoWidget = PropertyEntity({ type: types.event, eventClick: ConfiguracaoWidgetClick });
    this.ExibirLegenda = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: ExibirLegendaClick, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.ExibirLegenda), visible: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            Global.abrirModal("divModalFiltrosPesquisa");
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.PesquisaTermo = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable(EnumTipoPesquisa.Carga), options: EnumTipoPesquisa.obterOpcoes(), def: EnumTipoPesquisa.Carga });
    this.PesquisarPor = PropertyEntity({ val: ko.observable(""), def: "", visible: ko.observable(true), type: types.string });

    this.PesquisaRapida = PropertyEntity({
        eventClick: function (e) {
            $('.aviso-filtragem-dados').hide();
            $("#controle-entrega-conteudo-container").show();
            ExibirCheckBoxCargaEntrega(false);
            limparFiltrosControleEntrega();

            if (this.PesquisaTermo.val() === EnumTipoPesquisa.Carga )
                _pesquisaControleEntrega.CodigoCargaEmbarcador.val(this.PesquisarPor.val());
            else if (this.PesquisaTermo.val() === EnumTipoPesquisa.NotaFiscal)
                _pesquisaControleEntrega.NumeroNota.val(this.PesquisarPor.val());
            else if (this.PesquisaTermo.val() === EnumTipoPesquisa.Pedido)
                _pesquisaControleEntrega.Pedido.val(this.PesquisarPor.val());

            obterControleEntregas(1, false, false);
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

async function loadEtapasControleEntrega() {
    $('.aviso-filtragem-dados').show();
    loadPosicao();
    loadInicioViagem();
    loadFimViagem(false);
    loadEntrega();
    loadInformarInicioPreTrip();
    loadInformarFimPreTrip();
}

function carregarHTMLComponenteControleEntrega(callback) {
    var componente = 'ComponenteControleEntrega.html?dyn=' + guid();
    if (_controleEntregaVisaoPrevisao) {
        componente = 'ComponenteControleEntregaPrevisao.html?dyn=' + guid();
        _itensPorPagina = 10;
    }

    $.get('Content/Static/Carga/ControleEntrega/' + componente, function (html) {
        $('#ControleEntregaContent').html(html);

        $.get("Content/Static/Chat/ChatMotorista.html?dyn=" + guid(), function (htmlModal) {
            $("#chatMotorista").html(htmlModal);

            $.get('Content/Static/Carga/ControleEntrega/Modal.html?dyn=' + guid(), function (html) {
                $('#ControleEntregaModal').html(html);
                LocalizeCurrentPage();
                inicializarEventosModalConfiguracaoWidget();
                callback();
            });
        });

    });
}
function exibirGrid() {

    if (!_controleEntregaVisaoPrevisao)
        $("#wid-id-6").show();
}

function loadControleEntrega() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {
            carregarHTMLComponenteControleEntrega(function () {
                registraComponente();
                loadEtapasControleEntrega();
                isMobile = $(window).width() <= 980;

                _containerControleEntrega = new ContainerControleEntrega();
                KoBindings(_containerControleEntrega, "knoutContainerControleEntrega");

                _pesquisaControleEntrega = new PesquisaControleEntrega();
                KoBindings(_pesquisaControleEntrega, "knoutPesquisaControleEntrega", false);

                _configuracaoExibicoesControleEntrega = new ConfiguracaoExibicoesControleEntrega();
                KoBindings(_configuracaoExibicoesControleEntrega, "knockoutConfiguracaoExibicoesControleEntrega");

                loadConfiguracaoWidget(function () {
                    exibirGrid();
                });
                loadFiltroPesquisa();

                _configuracaoExibicoesControleEntrega.ExibirLegenda.visible(!_controleEntregaVisaoPrevisao);
                new BuscarEmpresa(_pesquisaControleEntrega.EmpresaDestino);
                new BuscarLocalidades(_pesquisaControleEntrega.CidadeOrigem);
                new BuscarLocalidades(_pesquisaControleEntrega.CidadeDestino);
                new BuscarEstados(_pesquisaControleEntrega.EstadoOrigem);
                new BuscarEstados(_pesquisaControleEntrega.EstadoDestino);
                new BuscarGruposPessoas(_pesquisaControleEntrega.GrupoPessoa);
                new BuscarTiposOperacao(_pesquisaControleEntrega.TipoOperacao, null, null, null, null);
                new BuscarTiposdeCarga(_pesquisaControleEntrega.TipoCarga);
                new BuscarCanaisEntrega(_pesquisaControleEntrega.CanalEntrega);
                new BuscarCanaisVenda(_pesquisaControleEntrega.CanalVenda);

                loadAtendimento();

                new BuscarXMLNotaFiscal(_pesquisaControleEntrega.NumeroNotaFiscal);
                new BuscarClientes(_pesquisaControleEntrega.Destinatario);
                new BuscarClientes(_pesquisaControleEntrega.Remetente);
                new BuscarClientes(_pesquisaControleEntrega.Emitente);
                new BuscarClientes(_pesquisaControleEntrega.Expedidor);
                new BuscarFilial(_pesquisaControleEntrega.Filial);
                new BuscarFilial(_pesquisaControleEntrega.FilialVenda, null, null, false, null, null, true);
                new BuscarMotoristas(_pesquisaControleEntrega.Motorista);
                new BuscarTransportadores(_pesquisaControleEntrega.Transportador);
                new BuscarVeiculos(_pesquisaControleEntrega.Veiculos);
                new BuscarFuncionario(_pesquisaControleEntrega.ResponsavelEntrega);
                new BuscarFuncionario(_pesquisaControleEntrega.Vendedor);
                new BuscarFuncionario(_pesquisaControleEntrega.Supervisor);
                new BuscarFuncionario(_pesquisaControleEntrega.Gerente);

                new BuscarCargas(_pesquisaControleEntrega.CodigoCargaEmbarcadorMulti);
                new BuscarClientes(_pesquisaControleEntrega.Recebedor, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
                new BuscarCentrosCarregamento(_pesquisaControleEntrega.CentroCarregamento);
                new BuscarCentroResultado(_pesquisaControleEntrega.CentroResultado);
                new BuscarFuncionario(_pesquisaControleEntrega.ResponsavelVeiculo);
                BuscarTiposTrecho(_pesquisaControleEntrega.TipoTrecho);
                BuscarClienteComplementar(_pesquisaControleEntrega.ClienteComplementar);
                BuscarMesoRegiao(_pesquisaControleEntrega.MesoRegiao);
                BuscarRegioes(_pesquisaControleEntrega.Regiao);

                BuscarEventosMonitoramento(_pesquisaControleEntrega.TipoAlerta);

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                    _pesquisaControleEntrega.Filial.visible(false);
                    _pesquisaControleEntrega.FilialVenda.visible(false);
                    _pesquisaControleEntrega.ResponsavelEntrega.visible(false);
                    _pesquisaControleEntrega.Transportador.text(Localization.Resources.Cargas.ControleEntrega.EmpresaFilial.getFieldDescription());
                }
                _pesquisaControleEntrega.RetornarInformacoesMonitoramento.visible(_CONFIGURACAO_TMS.PossuiMonitoramento);
                _pesquisaControleEntrega.RetornarCargasQueMonitoro.visible(_CONFIGURACAO_TMS.PossuiMonitoramento);

                buscarStatusViagem(function () {
                    loadChamado();
                    LoadConexaoSignalRControleColetaEntrega();
                    loadRaioXCarga();
                    loadAdicionarPedidoEntrega();
                    loadReordenarEntrega();
                    ControlarCamposTransportador();
                    ObterHabilitarImportacaoCargaFluvial();
                    setTimeout(atualizacaoControleEntregaAutomatica, timeoutControleEntrega);
                    loadLoteComprovanteEntrega(true);
                    loadHistoricoMonitoramento();
                    loadAssinatura();
                    loadConferenciaProduto();
                    loadDetalhesMonitoramento();
                    buscarTipoOcorrencia(_pesquisaControleEntrega.OcorrenciaEntrega);
                }, _pesquisaControleEntrega.StatusDaViagem);
            });
        });
    });
}

//*******MÉTODOS*******

function buscarStatusViagem(callback, statusViagem) {

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
                LimparCampo(statusViagem);
                callback();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function buscarTipoOcorrencia() {
    executarReST("TipoOcorrencia/BuscarOcorrenciasUtilizadasControleEntrega", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _pesquisaControleEntrega.OcorrenciaEntrega.options(arg.Data.TiposOcorrencia);
                LimparCampo(_pesquisaControleEntrega.OcorrenciaEntrega);

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function registraComponente() {
    if (ko.components.isRegistered('carga-entrega'))
        return;

    ko.components.register('carga-entrega', {
        viewModel: Entrega,
        template: {
            element: 'carga-entrega-template',
            afterRender: afterRenderEntrega
        }
    });
}

function afterRenderEntrega(element, data) {
    renderizarGridMonitoramento(element, data);
}

function limparFiltrosControleEntrega() {
    LimparCampos(_pesquisaControleEntrega);
}

function limparFiltrosControleEntregaRapido() {
    LimparCampos(_configuracaoExibicoesControleEntrega);
}

function obterControleEntregas(page, eventoPorPaginacao, atualizacaoAutomatica) {
    var p = new promise.Promise();

    var data = RetornarObjetoPesquisa(_pesquisaControleEntrega);

    funcionarioPedido.Vendedor = JSON.parse(data.Vendedor);
    funcionarioPedido.Supervisor = JSON.parse(data.Supervisor);
    funcionarioPedido.Gerente = JSON.parse(data.Gerente);

    data.FiltroPesquisa = RetornarJsonFiltroPesquisa(_pesquisaControleEntrega);

    if (atualizacaoAutomatica == undefined)
        atualizacaoAutomatica = false;

    data.inicio = _itensPorPagina * (page - 1);
    data.limite = _itensPorPagina;

    executarReST("ControleEntrega/ObterControleEntrega", data, function (arg) {
        if (arg.Success) {
            _KnoutsEntregas = new Array();

            if (arg.Data !== false) {
                $('[rel=popover-hover]').each(function () {
                    let popover = bootstrap.Popover.getOrCreateInstance(this);
                    popover.dispose();
                });

                _containerControleEntrega.Entregas.val(arg.Data);

                if (!atualizacaoAutomatica)
                    expandirComponentes();

                if (!eventoPorPaginacao)
                    componentePaginacao(arg.QuantidadeRegistros);

                setTimeout(function () {
                    ConfigurarSubscribesNosCheckBoxes();
                    $("[rel=popover-hover]").each(function () {
                        bootstrap.Popover.getOrCreateInstance(this, { placement: "top", html: true, trigger: "hover", container: "body", delay: { "show": 500, "hide": 100 } });
                    });
                }, 1000);
            } else {
                if (!atualizacaoAutomatica)
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            if (!atualizacaoAutomatica)
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

        p.done();

    }, function () { }, !atualizacaoAutomatica);

    if (_CONFIGURACAO_TMS.FiltrarWidgetAtendimentoProFiltro)
        AtualizarListaAtendimentos(false);

    return p;
}

function componentePaginacao(totalRegistros) {
    if (totalRegistros > 0) {

        function montarMensagemPaginacao() {
            var resumo = Localization.Resources.Gerais.Geral.ExibindoAteDeRegistros;
            resumo = resumo
                .replace("_START_", Globalize.format((_page - 1) * _itensPorPagina + 1, "n0"))
                .replace("_END_", Globalize.format(Math.min(_itensPorPagina * _page, totalRegistros), "n0"))
                .replace("_TOTAL_", Globalize.format(totalRegistros, "n0"));

            var $resumo = $('<ul style="float:left" class="dataTables_info">' + resumo + '</ul>');
            $("#paginacao-controle-entrega").find(".dataTables_info").remove();
            $("#paginacao-controle-entrega").prepend($resumo);
        }

        montarMensagemPaginacao();

        var $ul = $('<ul style="float:right" class="pagination"></ul>');

        var paginas = Math.ceil(totalRegistros / _itensPorPagina);

        $("#paginacao-controle-entrega").addClass('dataTables_info');
        $("#paginacao-controle-entrega").empty().append($ul);

        _executarPesquisa = false;

        $ul.twbsPagination({
            first: Localization.Resources.Cargas.ControleEntrega.Primeiro,
            prev: Localization.Resources.Cargas.ControleEntrega.Anterior,
            next: Localization.Resources.Cargas.ControleEntrega.Proximo,
            last: Localization.Resources.Cargas.ControleEntrega.Ultimo,
            totalPages: paginas,
            visiblePages: 5,
            onPageClick: null,
            onPageClick: function (event, page) {
                _page = page;
                montarMensagemPaginacao();
                if (_executarPesquisa) {
                    obterControleEntregas(page, true);
                }
                _executarPesquisa = true;
            }
        });
    } else {
        $("#paginacao-controle-entrega").html('<span>' + Localization.Resources.Gerais.Geral.NenhumRegistroEncontrado + '</span>');
    }
}

function obterDetalhesCargaControleEntrega(fluxoEntrega) {
    var data = { Carga: fluxoEntrega.Carga.val() };
    executarReST("Carga/BuscarCargaPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#fdsCarga").html('<button type="button" class="btn-close" data-bs-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px; top: 6px;"><i class="fal fa-times"></i></button>');
                var knoutCarga = GerarTagHTMLDaCarga("fdsCarga", arg.Data, false);
                $("#fdsCarga .container-carga").addClass("mb-0");
                _cargaAtual = knoutCarga;
                Global.abrirModal("divModalDetalhesCargaFDS");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function cargaRightMouseClick(sender, e) {
    adicionarEventoMenu(sender, e);
}

function mensagemClick(sender, e) {
    _etapaAtualFluxo = sender;
    abrirChatClick();
    e.stopPropagation();
}

function exibeModalEtapa(id, onShow) {
    let $modal = $(id);
    $modal.one("show.bs.modal", onShow || function () { });

    Global.abrirModal(id);

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            Global.abrirModal(id);
    });

}

function atualizarControleEntrega() {
    if (_etapaAtualFluxo == null || _etapaAtualFluxo.Carga == undefined)
        return;

    var data = { Carga: _etapaAtualFluxo.Carga.val() }
    executarReST("ControleEntrega/BuscarPorCodigo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                preencherFluxoControleEntrega(_etapaAtualFluxo, arg.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function expandirComponentes() {
    expandirJarviswidget("#wid-id-4", true);
    expandirJarviswidget("#wid-id-6", false);
}

function expandirJarviswidget(id, expandir) {

    if (expandir)
        $(id).removeClass('jarviswidget-collapsed').children('div').slideDown('fast');
    else
        $(id).addClass('jarviswidget-collapsed').children('div').slideUp('fast');

}

function visualizarDadosCargaClick() {
    obterDetalhesCargaControleEntrega(_etapaAtualFluxo);
}

function visualizarDadosMapaClick() {

    var dadosMapa = { Carga: _etapaAtualFluxo.Carga.val(), Veiculo: _etapaAtualFluxo.CodigoVeiculo.val(), IDEquipamento: _etapaAtualFluxo.IDEquipamento.val() };

    visualizarDadosMapaControleEntregaClick(dadosMapa)
}

function alterarDataAgendamentoClick() {
    loadEventosDataAgendamento(_etapaAtualFluxo.Carga.val());
}

function alterarDataReagendamentoClick() {
    loadEventosDataReagendamento(_etapaAtualFluxo.Carga.val(), _etapaAtualFluxo.DataReagendamento.val());
}


function adicionarEventosClick() {
    loadEventosControleEntrega(_etapaAtualFluxo.Carga.val());
}

function raioXCargaClick() {
    var dados = { Carga: _etapaAtualFluxo.Carga.val(), Veiculo: _etapaAtualFluxo.CodigoVeiculo.val(), IDEquipamento: _etapaAtualFluxo.IDEquipamento.val() };
    exibirModalRaioXCarga(dados);
}

function visualizarDetalhesTorre() {
    loadModalDetalhesTorre(_etapaAtualFluxo.Carga.val());
}

function abrirChatClick() {
    //loadMobileChatControleEntrega({
    //    Carga: _etapaAtualFluxo.Carga.val(),
    //    NumeroMotorista: _etapaAtualFluxo.NumeroMotorista.val(),
    //    NomeMotorista: _etapaAtualFluxo.NomeMotorista.val(),
    //    AddPromotor: _etapaAtualFluxo.PermiteAdicionarPromotor.val(),
    //});

    loadChatModal(_etapaAtualFluxo.Carga.val(), false);
}

function gerarDemonstrativoEstadiaClick() {
    executarDownload("ControleEntrega/RelatorioDemonstrativoEstadias", { Carga: _etapaAtualFluxo.Carga.val() });
}

function gerarRelatorioBoletimViagem() {
    executarDownload("CargaImpressaoDocumentos/RelatorioBoletimViagemEmbarque", { Carga: _etapaAtualFluxo.Carga.val() });
}

function adicionarEventoMenu(sender, e) {
    _etapaAtualFluxo = sender;

    if (_etapaAtualFluxo.PermiteAdicionarColeta.val()) {
        $("#liMenuColeta").show();
    } else {
        $("#liMenuColeta").hide();
    }

    if (_etapaAtualFluxo.PermiteAdicionarReentrega.val() && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) {
        $("#liMenuPedidoReentrega").show();
    } else {
        $("#liMenuPedidoReentrega").hide();
    }

    if (_etapaAtualFluxo.PermiteAdicionarEntrega.val()) {
        $("#liMenuPedidoEntrega").show();
    } else {
        $("#liMenuPedidoEntrega").hide();
    }

    if (_pesquisaControleEntrega)
        if (_pesquisaControleEntrega.RetornarInformacoesMonitoramento.val()) {
            $("#liMenuHistoricoMonitoramento").show();
        } else {
            $("#liMenuHistoricoMonitoramento").hide();
        }

    if (_etapaAtualFluxo.PermiteReordenarEntrega.val()) {
        $("#liMenuReordenarEntrega").show();
    } else {
        $("#liMenuReordenarEntrega").hide();
    }

    if (_etapaAtualFluxo.PermiteDownloadBoletimViagem.val()) {
        $("#liMenuDownloadBoletimEmbarque").show();
    } else {
        $("#liMenuDownloadBoletimEmbarque").hide();
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        $("#limenuEventos").hide();
    } else {
        $("#limenuEventos").show();
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        $("#limenuDataAgendamento").show();
        $("#limenuDataReagendamento").show();
        $("#liMenuDemonstrativoEstadia").show();
    } else {
        $("#limenuDataAgendamento").hide();
        $("#limenuDataReagendamento").hide();
        $("#liMenuDemonstrativoEstadia").hide();
    }

    if (_etapaAtualFluxo.DataFimViagem.val() != "") {
        $("#liMenuReordenarEntrega").hide();
        $("#limenuEventos").hide();
    } else {
        $("#liMenuReordenarEntrega").show();
        $("#limenuEventos").show();
    }

    exibirMenuCarga(e);
}

function exibirMenuCarga(e) {
    var $document = $(document);
    var $menu = $("#ulMenuControleEntrega");

    $menu.css({
        display: 'block',
        left: 150 + $document.scrollLeft() + 'px',
        top: e.clientY - 120 + $document.scrollTop() + 'px'
    });

    var mouseLeaveHandle = function () {
        var $this = $(this);

        var timeOut = setTimeout(function () {
            $this.hide();

            $menu.off("mouseleave", mouseLeaveHandle);
        }, 400);

        $menu.one("mouseenter", function () {
            clearTimeout(timeOut);
        });
    };

    $menu.on("mouseleave", mouseLeaveHandle);
}

function loadFiltroPesquisa() {
    var data = { TipoFiltro: 0 };

    executarReST("ModeloFiltroPesquisa/ObterFiltroPesquisaPadrao", data, function (res) {
        if (res.Success && Boolean(res.Data)) {
            PreencherJsonFiltroPesquisa(_pesquisaControleEntrega, res.Data.Dados);

            $("#" + _pesquisaControleEntrega.StatusViagem.id).selectpicker('val', _pesquisaControleEntrega.StatusViagem.val());
        }
        else
            _pesquisaControleEntrega.DataInicial.val(Global.DataAtual());
    });
}

function ExibirLegendaClick() {
    Global.abrirModal("divModalLegenda");
}

function ValidarVisualizacaoSelecionarCargas() {
    if (_containerControleEntrega && _containerControleEntrega.DelegarCargas.visible())
        AssumirMonitoramentoClick();
    else
        ExibirCheckBoxCargaEntrega(false);
}
function AssumirMonitoramentoClick() {
    ExibirCheckBoxCargaEntrega(true);
    _containerControleEntrega.AssumirMonitoramentos.visible(false);
    _containerControleEntrega.DelegarCargas.visible(true);
    _containerControleEntrega.AssumirCargas.visible(true);   
    _containerControleEntrega.CancelarAcaoMonitoramentos.visible(true);
}

function CancelarMonitoramentoClick() {
    ExibirCheckBoxCargaEntrega(false);
}

function DelegarCargasClick() {
    let cargasSelecionadas = _KnoutsEntregas.filter(entrega => entrega.Selecionado.val());

    if (cargasSelecionadas.length == 0) {
        ExibirCheckBoxCargaEntrega(false);
        return;
    }

    let podeDelegarMonitoramentoParaUsuarios = _containerControleEntrega.PermiteDelegarMonitoramento.val();
    exibirModalDelegarMonitoramento(podeDelegarMonitoramentoParaUsuarios, cargasSelecionadas);
}

function AssumirCargasClick() {
    let cargasSelecionadas = _KnoutsEntregas.filter(entrega => entrega.Selecionado.val());

    if (cargasSelecionadas.length == 0) {
        ExibirCheckBoxCargaEntrega(false);
        return;
    }

    exibirModalDelegarMonitoramento(false, cargasSelecionadas);
}

function ExibirCheckBoxCargaEntrega(exibir) {
    _containerControleEntrega.ModoSelecao.val(exibir);
    _containerControleEntrega.SelecionarTodosCheckbox.visible(exibir);
    if (exibir == false) {
        _containerControleEntrega.AssumirMonitoramentos.visible(true);
        _containerControleEntrega.DelegarCargas.visible(false);
        _containerControleEntrega.AssumirCargas.visible(false);
        _containerControleEntrega.CancelarAcaoMonitoramentos.visible(false);
        selecionarTodosCheckBox(false);
    }
}

async function selecionarTodosCheckBox(selecionar) {
    _KnoutsEntregas.forEach((entrega) => {
        if (entrega.PodeSelecionar()) {
            entrega.Selecionado.val(selecionar);
        }
    })
}

function ConfiguracaoWidgetClick() {
    BuscarConfiguracaoWidget(function () {
        capturarEstadoInicialConfiguracaoWidgetDetalhesEntrega();
        Global.abrirModal("divModalConfiguracao");
    });  
}

function atualizacaoControleEntregaAutomatica() {

    if ((document.URL) && (document.URL.toLowerCase().includes(urlControleEntrega))) {

        obterControleEntregas(1, false, true).then(function () { setTimeout(atualizacaoControleEntregaAutomatica, timeoutControleEntrega) });
    }
}

function ControlarCamposTransportador() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaControleEntrega.Transportador.visible(false);
    }
}

function ObterHabilitarImportacaoCargaFluvial() {
    executarReST("ControleEntrega/HabilitarImportacaoCargaFluvial", {}, function (arg) {
        if (arg.Success) {
            _containerControleEntrega.ImportacaoCargaFluvial.visible(arg.Data.Habilitar);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ImportacaoCargaFluvialClick() {

}

function imprimirMinutaClick(e) {
    executarDownload("ControleEntrega/ImprimirMinuta", { Codigo: e.Carga.val() });
}