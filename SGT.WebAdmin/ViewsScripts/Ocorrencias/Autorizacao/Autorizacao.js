/// <reference path="AutorizarRegras.js" />
/// <reference path="Delegar.js" />
/// <reference path="HistoricoAutorizacao.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/MotivoRejeicaoOcorrencia.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumEtapaAutorizacaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumResponsavelOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumAprovacaoRejeicao.js" />
/// <reference path="../../Enumeradores/EnumAutorizacaoOcorrenciaPagamento.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="../../Enumeradores/EnumPeriodoPagamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _ocorrencia;
var _valores;
var _rejeicao;
var _analisarAutorizacao;
var _autorizacao;
var _gridOcorrencia;
var _pesquisaOcorrencias;

var _etapaAutorizacaoOcorrencia = [
    { text: "Todas", value: EnumEtapaAutorizacaoOcorrencia.Todas },
    { text: "Aprovação da Ocorrência", value: EnumEtapaAutorizacaoOcorrencia.AprovacaoOcorrencia },
    { text: "Emissão da Ocorrência", value: EnumEtapaAutorizacaoOcorrencia.EmissaoOcorrencia }
];

var _responsavelOcorrencia = [
    { text: "Conf. CTe Anterior", value: EnumResponsavelOcorrencia.ConformeCTeAnterior },
    { text: "Remetente", value: EnumResponsavelOcorrencia.Remetente },
    { text: "Destinatário", value: EnumResponsavelOcorrencia.Destinatario }
    //{ text: "Outro", value: EnumResponsavelOcorrencia.Outros } //Aguardando definição do GPA para liberar alteração
];

var AnalisarAutorizacao = function () {
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Usuario.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });

    this.Analisar = PropertyEntity({ eventClick: analisarAprovacaoClick, type: types.event, text: ko.observable(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Analisar) });
};

var Autorizacao = function () {
    this.Regras = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.Tomador = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Responsavel.getFieldDescription(), val: ko.observable(EnumResponsavelOcorrencia.Destinatario), enable: ko.observable(true), options: _responsavelOcorrencia, def: EnumResponsavelOcorrencia.ConformeCTeAnterior, visible: ko.observable(false), permiteSelecionarTomador: ko.observable(false), eventChange: responsavelChange });
    this.Pagamento = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Pagamento.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), options: EnumAutorizacaoOcorrenciaPagamento.obterOpcoes(), def: "", visible: ko.observable(false) });
    this.ClienteTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: true, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Tomador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.PercentualJurosParcela = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, maxlength: 15, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.PercentualJuros.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false) });
    this.QuantidadeParcelas = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Parcelas.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false), maxlength: 3, configInt: { precision: 0, allowZero: true } });
    this.PeriodoPagamento = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.PeriodoPagamento, val: ko.observable(EnumPeriodoPagamento.Selecione), options: EnumPeriodoPagamento.obterOpcoesPesquisa(), def: EnumPeriodoPagamento.Selecione, enable: ko.observable(true) });

    this.Justificativa = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Justificativa.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), enable: true, idBtnSearch: guid(), visible: ko.observable(false) });
    this.JustificativaAprovacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.JutificativaAprovacao.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), enable: true, idBtnSearch: guid(), visible: ko.observable(false) });
    this.CentroResultado = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CentroResultado.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), enable: true, idBtnSearch: guid(), visible: ko.observable(false) });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Motivo.getRequiredFieldDescription(), maxlength: 1000, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(false) });

    this.Rejeitar = PropertyEntity({ eventClick: rejeitarOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Rejeitar, visible: ko.observable(false) });
    this.Aprovar = PropertyEntity({ eventClick: aprovarOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Aprovar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Cancelar, visible: ko.observable(false) });
    this.AprovarTodas = PropertyEntity({ type: types.event, eventClick: aprovarMultiplasRegrasClick, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AprovarRegras, visible: ko.observable(false) });
    this.AnalisarAprovacao = PropertyEntity({ type: types.event, eventClick: analisarAprovacaoPorUsuarioLogadoClick, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AnalisarAprovacao, visible: ko.observable(false) });
    this.RemoverAnaliseAprovacao = PropertyEntity({ type: types.event, eventClick: removerAnaliseAprovacaoClick, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.RemoverAnaliseAprovacao, visible: ko.observable(false) });
    this.AssumirOcorrencia = PropertyEntity({ type: types.event, eventClick: assumirOcorrenciaClick, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AssumirOcorrencia, visible: ko.observable(_CONFIGURACAO_TMS.PermiteAssumirOcorrencia) });
};

var RejeitarSelecionados = function () {
    this.Justificativa = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Justificativa.getRequiredFieldDescription(), type: types.entity, codEntity: ko.observable(0), required: true, enable: true, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Motivo.getRequiredFieldDescription(), maxlength: 1000, required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.Rejeitar = PropertyEntity({ eventClick: rejeitarOcorrenciasSelecionadosClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Rejeitar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRejeicaoSelecionadosClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Cancelar, visible: ko.observable(true) });
};

var AprovarSelecionados = function () {
    this.CodigoAprovacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CodigoAprovacao.getFieldDescription(), maxlength: 500, required: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.Aprovar = PropertyEntity({ eventClick: aprovarOcorrenciasSelecionadasClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Aprovar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAprovacaoSelecionadosClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Cancelar, visible: ko.observable(true) });
};

var AprovarMultiplasOcorrenciasSelecionados = function () {
    this.PercentualJurosParcela = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: true }, maxlength: 15, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.PercentualJuros.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(true), required: ko.observable(false) });
    this.QuantidadeParcelas = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Parcelas.getFieldDescription(), enable: ko.observable(false), visible: ko.observable(true), required: ko.observable(false), maxlength: 3, configInt: { precision: 0, allowZero: true } });
    this.PeriodoPagamento = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.PeriodoPagamento, val: ko.observable(EnumPeriodoPagamento.Selecione), options: EnumPeriodoPagamento.obterOpcoesPesquisa(), def: EnumPeriodoPagamento.Selecione, enable: ko.observable(true) });

    this.AprovarMultiplasOcorrencias = PropertyEntity({ eventClick: aprovarMultiplasOcorrenciasSelecionadasClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Aprovar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAprovacaoMultiplosSelecionadosClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Cancelar, visible: ko.observable(true) });
};

var Ocorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ValorOcorrencia.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.NumeroOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NumeroOcorrencia.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.Solicitante = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SolicitanteOcorrencia.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.DataOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DataOcorrencia.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.CodigoCarga = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Carga.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.Distancia = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Distancia.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.ModeloVeicularCarga = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ModeloVeicularCarga.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.TipoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.TipoOcorrencia.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.TipoDocumentoCreditoDebito = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.TipoCreditoDebito.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.ComponenteFrete = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ComponenteFrete.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Situacao.getFieldDescription(), issue: 411, visible: ko.observable(true), val: ko.observable("") });
    this.Remetentes = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Remetente.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.Cliente = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Cliente.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.Filial = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Filial.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.Transportador = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Transportador.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.Terceiro = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Terceiro.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.DataHoraPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.PrevisaoEntrega.getFieldDescription() });
    this.Origens = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Origens.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.Destinos = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Destinos.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.Motoristas = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Motoristas.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") })
    this.Veiculos = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Veiculos.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.ParametroData = PropertyEntity({ text: ko.observable(""), defText: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ParametroData.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.ParametroDataInicio = PropertyEntity({ text: ko.observable(""), defText: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ParametroDataInicio.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.ParametroDataFim = PropertyEntity({ text: ko.observable(""), defText: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ParametroDataFim.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.ParametroPeriodoHoras = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ParametroPeriodoHoras.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.ParametroApenasReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ParametroApenasReboque.getFieldDescription()), visible: ko.observable(true), val: ko.observable("") });
    this.ParametroTexto = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ParametroGLog.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.DataChegadaVeiculo = PropertyEntity({ text: ko.observable(""), visible: ko.observable(true), val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Observacao.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.MotivoCancelamento = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.MotivoCancelamento.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.ObservacaoImpressa = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ObservacaoImpressa.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.CodigoAprovacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.CodigoAprovacao.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.Pais == EnumPaises.Exterior) });
    this.DataEntradaRaio = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DataEntradaRaio.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.DataSaidaRaio = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DataSaidaRaio.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });

    this.Pedido = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Pedido.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.NotaFiscal = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NotaFiscal.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });

    //Botões
    this.BaixarRateio = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DownloadPlanilha, visible: ko.observable(_CONFIGURACAO_TMS.ExibirOpcaoDownloadPlanilhaRateioOcorrencia), eventClick: baixarRateio });
    this.ReenviarIntegracao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ReenviarIntegracao, visible: ko.observable(true), eventClick: reenviarIntegracao });
    this.SalvarObservacaoAprovador = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SalvarObservacaoAprovador, eventClick: salvarObservacaoAprovador, enable: ko.observable(true), visible: ko.observable(false) });

    //Campos editáveis
    this.ObservacaoAprovador = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ObservacoesAprovador.getFieldDescription(), maxlength: 5000, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.ExibirObservacaoAprovadorAutorizacaoOcorrencia) });

    // Campos para filtro
    this.Usuario = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.EtapaAutorizacao = PropertyEntity({ val: ko.observable(0), def: 0 });
};

var AnexosOcorrencias = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Anexos = PropertyEntity({ type: types.local, val: ko.observableArray([]), });
    this.DownloadAnexo = PropertyEntity({ eventClick: downloadAnexoClick, type: types.event });
    this.EnviarAnexo = PropertyEntity({ eventClick: adicionarAnexoAutorizacaoClick, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AdicionarAnexo, visible: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, eventChange: enviarArquivoAnexo, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Arquivo, val: ko.observable(""), visible: ko.observable(false) });
};

var PesquisaOcorrencias = function () {
    this.DataInicial = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DataInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DataFinal.getFieldDescription(), getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;
    this.TipoDocumentoCreditoDebito = PropertyEntity({ val: ko.observable(EnumTipoDocumentoCreditoDebito.Todos), options: EnumTipoDocumentoCreditoDebito.obterOpcoesPesquisa(), def: EnumTipoDocumentoCreditoDebito.Todos, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.TipoCreditoDebito, visible: ko.observable(true) });

    this.NumeroOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NumeroOcorrencia.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoOcorrencia.AutorizacaoPendente), options: EnumSituacaoOcorrencia.obterOpcoesPesquisaAutorizacao(), def: EnumSituacaoOcorrencia.AutorizacaoPendente, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Situacao, issue: 411 });
    this.Situacao.val.subscribe(function () {
        exibirMultiplasOpcoes();
    });
    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NumeroCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.EtapaAutorizacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.EtapaAutorizacao.getFieldDescription(), issue: 908, val: ko.observable(EnumEtapaAutorizacaoOcorrencia.Todas), options: EnumAutorizacaoOcorrencia.obterOpcoesPesquisaEtapaAutorizacaoOcorrencia(), def: EnumEtapaAutorizacaoOcorrencia.Todas });
    this.PrioridadeAprovacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.PrioridadeAprovacao.getRequiredFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), def: new Array(), options: EnumPrioridadeAutorizacao.obterOpcoes() });

    this.Usuario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Usuario, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Ocorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.TipoOcorrencia.getFieldDescription(), issue: 410, idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Transportador.getFieldDescription(), issue: 69, idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Filial.getFieldDescription(), idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Destino.getFieldDescription(), idBtnSearch: guid() });

    this.NumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NumeroPedidoCliente.getFieldDescription(), maxlength: 150, required: false });
    this.ClienteComplementar = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ClienteComplementar.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Vendedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Vendedor.getFieldDescription(), idBtnSearch: guid() });
    this.Supervisor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Supervisor.getFieldDescription(), idBtnSearch: guid() });
    this.Gerente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Gerente.getFieldDescription(), idBtnSearch: guid() });
    this.UFDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.UFDestino.getFieldDescription(), idBtnSearch: guid() });
    this.NumeroNF = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NumeroNF.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.MarcarTodas, visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarOcorrencias();
        }, type: types.event, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function (e) {
            limparFiltrosAutorizacaoOcorrencia();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.LimparFiltros, idGrid: guid(), visible: ko.observable(true)
    });

    this.AprovarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: aprovarMultiplasOcorrenciasClick, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AprovarOcorrencias, visible: ko.observable(false) });
    this.RejeitarTodas = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), eventClick: rejeitarMultiplasRegrasClick, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.RejeitarOcorrencias });
    this.DelegarOcorrencias = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: delegarMultiplasOcorrenciasClick, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.DelegarOcorrencias, visible: ko.observable(false) });
    this.VerificarAprovacaoTodas = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: verificarAprovacaoMultiplasOcorrenciasClick, text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VerificarAprovacoesOcorrencias, visible: ko.observable(false) });
    this.ValorTotalOcorrencias = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ValorTotalOcorrencias.getFieldDescription(), visible: ko.observable(false), def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, configDecimal: { allowZero: true } });
};

//*******EVENTOS******* 

function loadAutorizacao() {
    _ocorrencia = new Ocorrencia();
    KoBindings(_ocorrencia, "knockoutOcorrencia");

    _autorizacao = new Autorizacao();
    KoBindings(_autorizacao, "knockoutAutorizacao");

    _rejeicao = new RejeitarSelecionados();
    KoBindings(_rejeicao, "knockoutRejeicaoOcorrencia");

    _aprovacao = new AprovarSelecionados();
    KoBindings(_aprovacao, "knockoutAprovacaoOcorrencia");

    _aprovacaoMultiplasOcorrencias = new AprovarMultiplasOcorrenciasSelecionados();
    KoBindings(_aprovacaoMultiplasOcorrencias, "knockoutAprovacaoMultiplasOcorrencias");

    _analisarAutorizacao = new AnalisarAutorizacao();
    KoBindings(_analisarAutorizacao, "knockoutAnalisarAutorizacao");

    _anexosOcorrencias = new AnexosOcorrencias();
    KoBindings(_anexosOcorrencias, "knockoutAnexosAutorizacao");

    _pesquisaOcorrencias = new PesquisaOcorrencias();
    KoBindings(_pesquisaOcorrencias, "knockoutPesquisaOcorrencias", false, _pesquisaOcorrencias.Pesquisar.id);

    _autorizacao.Pagamento.visible(_CONFIGURACAO_TMS.ExibirCampoInformativoPagadorAutorizacaoOcorrencia);

    // Busca componentes pesquisa
    new BuscarTipoOcorrencia(_pesquisaOcorrencias.Ocorrencia);
    new BuscarTransportadores(_pesquisaOcorrencias.Transportador);
    new BuscarFilial(_pesquisaOcorrencias.Filial);
    new BuscarLocalidades(_pesquisaOcorrencias.Destino);
    new BuscarFuncionario(_pesquisaOcorrencias.Usuario);
    new BuscarFuncionario(_analisarAutorizacao.Usuario);
    new BuscarMotivoRejeicaoOcorrencia(_rejeicao.Justificativa, null, EnumAprovacaoRejeicao.Rejeicao);
    new BuscarClientes(_autorizacao.ClienteTomador);
    new BuscarClienteComplementar(_pesquisaOcorrencias.ClienteComplementar);
    new BuscarFuncionario(_pesquisaOcorrencias.Gerente);
    new BuscarFuncionario(_pesquisaOcorrencias.Supervisor);
    new BuscarFuncionario(_pesquisaOcorrencias.Vendedor);
    new BuscarEstados(_pesquisaOcorrencias.UFDestino);

    // Load modulos
    loadAnexosCargas();
    loadDelegar();
    loadRegras();
    loadDetalhesCargaSumarizada();
    LoadResumoChamado();
    loadDocumentosComplementares();
    loadCTes();
    loadAnexosContratoVeiculo();
    LoadManobristas();
    loadClientes();
    loadHistoricoAutorizacao();

    // Filtrar Alcadas Do Usuario
    loadDadosUsuarioLogado(buscarOcorrencias);

    // Valida se a tela está sendo carregado pelo link de acesso enviado via e-mail
    if (CODIGO_OCORRENCIA_VIA_TOKEN_ACESSO_AUTORIZACAO_OCORRENCIA.val() != "")
        carregarOcorrenciaUsuarioAcessadoViaLink(CODIGO_OCORRENCIA_VIA_TOKEN_ACESSO_AUTORIZACAO_OCORRENCIA.val());
}

function loadDadosUsuarioLogado(callback) {
    if (_CONFIGURACAO_TMS.FiltrarAlcadasDoUsuario)
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                _pesquisaOcorrencias.Usuario.multiplesEntities([{ Codigo: retorno.Data.Codigo, Descricao: retorno.Data.Nome }]);

                callback();
            }
        });
    else
        callback();
}

function baixarRateio() {
    var dados = {
        Ocorrencia: _ocorrencia.Codigo.val()
    };

    executarDownload("CargaOcorrencia/DownloadPlanilhaRateio", dados);
}

function reenviarIntegracao() {
    executarReST("Ocorrencia/ReenviarIntegracao", { Codigo: _ocorrencia.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ReenviarIntegracao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.IntegracaoReenviadaComSucesso);
                _ocorrencia.ReenviarIntegracao.visible(false);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function salvarObservacaoAprovador() {
    executarReST("AutorizacaoOcorrencia/SalvarObservacaoAprovador", { Codigo: _ocorrencia.Codigo.val(), ObservacaoAprovador: _ocorrencia.ObservacaoAprovador.val(), CodigoAprovacao: _ocorrencia.CodigoAprovacao.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ObservacaoAprovador, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.SalvaComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function atualizarOcorrencia() {
    BuscarPorCodigo(_ocorrencia, "AutorizacaoOcorrencia/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                CarregarDelegar(arg.Data.EnumSituacao);
                AtualizarGridRegras();

                _autorizacao.AnalisarAprovacao.visible(arg.Data.PermitirAnalisarOcorrencia);
                _autorizacao.RemoverAnaliseAprovacao.visible(arg.Data.PermitirRemoverAnaliseOcorrencia);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    }, null);
}

function buscarOcorrencias() {
    //-- Cabecalho
    var detalhes = { descricao: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Detalhes, id: "clasEditar", evento: "onclick", metodo: detalharOcorrencia, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhes);

    //-- Reseta
    _pesquisaOcorrencias.SelecionarTodos.val(false);
    _pesquisaOcorrencias.AprovarTodas.visible(false);
    _pesquisaOcorrencias.RejeitarTodas.visible(false);
    _pesquisaOcorrencias.DelegarOcorrencias.visible(false);
    _pesquisaOcorrencias.VerificarAprovacaoTodas.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaOcorrencias.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    };

    var ordenacaoPadrao = { column: 1, dir: orderDir.asc };

    var configExportacao = {
        url: "AutorizacaoOcorrencia/ExportarPesquisa",
        titulo: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AutorizacaoOcorrencias
    };

    _gridOcorrencia = new GridView("grid-autorizacao-ocorrencias", "AutorizacaoOcorrencia/Pesquisa", _pesquisaOcorrencias, menuOpcoes, ordenacaoPadrao, 25, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridOcorrencia.SetPermitirEdicaoColunas(true);
    _gridOcorrencia.SetSalvarPreferenciasGrid(true);
    _gridOcorrencia.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var situacaoPesquisa = _pesquisaOcorrencias.Situacao.val();
    var possuiSelecionado = _gridOcorrencia.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaOcorrencias.SelecionarTodos.val();
    var situacaoPermiteSelecao = (situacaoPesquisa == EnumSituacaoOcorrencia.AgAprovacao || situacaoPesquisa == EnumSituacaoOcorrencia.AgAutorizacaoEmissao || situacaoPesquisa == EnumSituacaoOcorrencia.AutorizacaoPendente);
    var situacaoPermiteSelecaoDelegar = !_CONFIGURACAO_TMS.NaoExibirOpcaoParaDelegar && (situacaoPesquisa == EnumSituacaoOcorrencia.AgAprovacao || situacaoPesquisa == EnumSituacaoOcorrencia.AgAutorizacaoEmissao || situacaoPesquisa == EnumSituacaoOcorrencia.AutorizacaoPendente);

    let total = 0;
    let ocorrencias = _gridOcorrencia.ObterMultiplosSelecionados();

    for (let i = 0; i < ocorrencias.length; i++) {
        total += Globalize.parseFloat(ocorrencias[i].Valor);
    }
    _pesquisaOcorrencias.ValorTotalOcorrencias.val(total.toLocaleString('pt-br', { minimumFractionDigits: 2 }));

    // Esconde todas opções
    _pesquisaOcorrencias.AprovarTodas.visible(false);
    _pesquisaOcorrencias.RejeitarTodas.visible(false);
    _pesquisaOcorrencias.DelegarOcorrencias.visible(false);
    _pesquisaOcorrencias.VerificarAprovacaoTodas.visible(false);

    if (possuiSelecionado || selecionadoTodos) {
        if (situacaoPermiteSelecao) {
            _pesquisaOcorrencias.AprovarTodas.visible(true);
            _pesquisaOcorrencias.RejeitarTodas.visible(true);
            _pesquisaOcorrencias.ValorTotalOcorrencias.visible(true);
            _pesquisaOcorrencias.VerificarAprovacaoTodas.visible(_CONFIGURACAO_TMS.UsuarioMultisoftware);
        }

        if (situacaoPermiteSelecaoDelegar)
            _pesquisaOcorrencias.DelegarOcorrencias.visible(true);
    }
}

function analisarAprovacaoPorUsuarioLogadoClick() {
    exibirConfirmacao(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Confirmacao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceRealmenteDesejaAnalisarAprovacao, function () {
        executarReST("AutorizacaoOcorrencia/AnalisarAprovacao", { Codigo: _ocorrencia.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    AtualizarGridRegras();

                    _autorizacao.AnalisarAprovacao.visible(false);
                    _autorizacao.RemoverAnaliseAprovacao.visible(retorno.Data.PermitirRemoverAnaliseOcorrencia);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function analisarAprovacaoClick() {
    if (!ValidarCamposObrigatorios(_analisarAutorizacao)) {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
        return;
    }

    executarReST("AutorizacaoOcorrencia/AnalisarAprovacao", { Codigo: _ocorrencia.Codigo.val(), Usuario: _analisarAutorizacao.Usuario.codEntity() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                fecharModalAnalisarAprovacao();
                AtualizarGridRegras();

                _autorizacao.AnalisarAprovacao.visible(false);
                _autorizacao.RemoverAnaliseAprovacao.visible(retorno.Data.PermitirRemoverAnaliseOcorrencia);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function analisarAprovacaoModalClick() {
    exibirModalAnalisarAprovacao();
}

function removerAnaliseAprovacaoClick() {
    exibirConfirmacao(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Confirmacao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceRealmenteDesejaRemoverAnaliseAprovacao, function () {
        executarReST("AutorizacaoOcorrencia/RemoverAnaliseAprovacao", { Codigo: _ocorrencia.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    AtualizarGridRegras();

                    _autorizacao.AnalisarAprovacao.visible(true);
                    _autorizacao.RemoverAnaliseAprovacao.visible(false);
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        })
    });
}

function rejeitarOcorrenciasSelecionadosClick() {
    exibirConfirmacao(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Confirmacao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceRelmenteDesejarReprovarTodasOcorrencias, function () {
        var dados = RetornarObjetoPesquisa(_pesquisaOcorrencias);
        var rejeicao = RetornarObjetoPesquisa(_rejeicao);

        dados.Justificativa = rejeicao.Justificativa;
        dados.Motivo = rejeicao.Motivo;
        dados.SelecionarTodos = _pesquisaOcorrencias.SelecionarTodos.val();
        dados.OcorrenciasSelecionadas = JSON.stringify(_gridOcorrencia.ObterMultiplosSelecionados());
        dados.OcorrenciasNaoSelecionadas = JSON.stringify(_gridOcorrencia.ObterMultiplosNaoSelecionados());

        executarReST("AutorizacaoOcorrencia/ReprovarMultiplasOcorrencias", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.RegrasModificadas > 0) {
                        if (arg.Data.RegrasModificadas > 1)
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AlcadasOcorrenciasForamReprovadas.format(arg.Data.RegrasModificadas));
                        else
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AlcadaOcorrenciaFoiReprovada.format(arg.Data.RegrasModificadas));
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NenhumaAlcadaPendenteParaSeuUsuario);
                    buscarOcorrencias();
                    cancelarRejeicaoSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function rejeitarMultiplasRegrasClick() {
    LimparCampos(_rejeicao);
    Global.abrirModal('divModalRejeitarOcorrencia');
}

function aprovarMultiplasOcorrenciasClick() {
    if (_CONFIGURACAO_TMS.Pais == EnumPaises.Exterior) {
        LimparCampos(_aprovacao);
        Global.abrirModal('divModalAprovarOcorrencia');
    }
    else {
        aprovarOcorrenciasSelecionadasClick()
    }
}


function delegarMultiplasOcorrenciasClick() {
    Global.abrirModal('divModalDelegarOcorrencia');
}

function aprovarOcorrenciasSelecionadasClick() {
    let ocorrenciasComNecessidadeDeIncluirParcelas = _gridOcorrencia.ObterMultiplosSelecionados().filter((registro) => (!string.IsNullOrWhiteSpace(registro.ExibirParcelasNaAprovacao) && registro.ExibirParcelasNaAprovacao));

    if (ocorrenciasComNecessidadeDeIncluirParcelas.length > 0)
        Global.abrirModal('divModalAprovarMultiplasOcorrencias');
    else
        efetuarAprovacaoMultiplasOcorrencias();
}

function aprovarMultiplasOcorrenciasSelecionadasClick() {
    efetuarAprovacaoMultiplasOcorrencias();
    Global.fecharModal('divModalAprovarMultiplasOcorrencias');
}

function efetuarAprovacaoMultiplasOcorrencias() {
    exibirConfirmacao(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Confirmacao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceRelmenteDesejarAprovarTodasOcorrencias, function () {

        var dados = RetornarObjetoPesquisa(_pesquisaOcorrencias);
        var aprovacao = RetornarObjetoPesquisa(_aprovacao);

        dados.CodigoAprovacao = aprovacao.CodigoAprovacao;
        dados.SelecionarTodos = _pesquisaOcorrencias.SelecionarTodos.val();
        dados.OcorrenciasSelecionadas = JSON.stringify(_gridOcorrencia.ObterMultiplosSelecionados());
        dados.OcorrenciasNaoSelecionadas = JSON.stringify(_gridOcorrencia.ObterMultiplosNaoSelecionados());
        dados.QuantidadeParcelas = _aprovacaoMultiplasOcorrencias.QuantidadeParcelas.val();
        dados.PercentualJurosParcela = _aprovacaoMultiplasOcorrencias.PercentualJurosParcela.val();
        dados.PeriodoPagamento = _aprovacaoMultiplasOcorrencias.PeriodoPagamento.val();

        executarReST("AutorizacaoOcorrencia/AprovarMultiplasOcorrencias", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    var data = arg.Data;

                    if (data.RegrasModificadas > 1)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AlcadasOcorrenciasForamAprovadas.format(data.RegrasModificadas));
                    else if (data.RegrasModificadas == 1)
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AlcadaOcorrenciaFoiaPROVADA.format(data.RegrasModificadas));
                    else if (data.RegrasExigemMotivo == 0)
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.NenhumaAlcadaPendenteParaSeuUsuario);

                    if (data.RegrasExigemMotivo > 0)
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.HaAlcadasOcorrenciasQueExigemJustificativaParaAprovacao.format(data.RegrasExigemMotivo));

                    buscarOcorrencias();
                    cancelarAprovacaoSelecionadosClick();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function verificarAprovacaoMultiplasOcorrenciasClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaOcorrencias);

    dados.SelecionarTodos = _pesquisaOcorrencias.SelecionarTodos.val();
    dados.OcorrenciasSelecionadas = JSON.stringify(_gridOcorrencia.ObterMultiplosSelecionados());
    dados.OcorrenciasNaoSelecionadas = JSON.stringify(_gridOcorrencia.ObterMultiplosNaoSelecionados());

    executarReST("AutorizacaoOcorrencia/VerificarAprovacoesMultiplasOcorrencias", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.AprovacoesVerificadasComSucesso);
                buscarOcorrencias();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function exibirModalAnalisarAprovacao() {
    Global.abrirModal('divModalAnalisarAprovacao');
    $("#divModalAnalisarAprovacao").one('hidden.bs.modal', function () {
        LimparCampos(_analisarAutorizacao);
    });
}

function fecharModalAnalisarAprovacao() {
    Global.fecharModal('divModalAnalisarAprovacao');
}

function assumirOcorrenciaClick() {
    exibirConfirmacao(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.Confirmacao, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.VoceRealmenteDesejaAssumirEssaOcorrencia, function () {
        executarReST("AutorizacaoOcorrencia/AssumirOcorrencia", { Codigo: _ocorrencia.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.OcorrenciaAssumida, Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ResponsavelPelaOcorrenciaPassouSerVoce);
                    AtualizarGridRegras();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

function detalharOcorrencia(ocorrenciaGrid, row) {
    _rowGridOcorrenciaAtual = row;
    _codigoOcorrenciaAtual = ocorrenciaGrid.Codigo;

    limparCamposOcorrencia();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaOcorrencias);
    _ocorrencia.Codigo.val(ocorrenciaGrid.Codigo);
    _ocorrencia.Usuario.val(pesquisa.Usuario);
    _ocorrencia.EtapaAutorizacao.val(pesquisa.EtapaAutorizacao);

    BuscarPorCodigo(_ocorrencia, "AutorizacaoOcorrencia/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                if (arg.Data.ErroIntegracaoComGPA)
                    _ocorrencia.ReenviarIntegracao.visible(true);
                else
                    _ocorrencia.ReenviarIntegracao.visible(false);

                if (arg.Data.CalculaValorPorTabelaFrete)
                    _ocorrencia.ParametroApenasReboque.text(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ReboqueTracao);

                var situacao = arg.Data.EnumSituacao;
                _ocorrencia.ObservacaoAprovador.enable(true);
                _ocorrencia.CodigoAprovacao.enable(true);
                _ocorrencia.SalvarObservacaoAprovador.visible(true);
                if (situacao !== EnumSituacaoOcorrencia.AutorizacaoPendente && situacao !== EnumSituacaoOcorrencia.AgAprovacao && situacao !== EnumSituacaoOcorrencia.AgAutorizacaoEmissao) {
                    _ocorrencia.ObservacaoAprovador.enable(false);
                }

                if (arg.Data.DataChegadaVeiculo) {
                    _ocorrencia.DataChegadaVeiculo.val(arg.Data.DataChegadaVeiculo.DataChegadaVeiculo);
                    _ocorrencia.DataChegadaVeiculo.text(arg.Data.DataChegadaVeiculo.Descricao);
                } else {
                    _ocorrencia.DataChegadaVeiculo.val("");
                }

                _ocorrencia.ParametroDataInicio.text(_ocorrencia.ParametroDataInicio.defText);
                if (arg.Data.DescricaoParametroDataInicio)
                    _ocorrencia.ParametroDataInicio.text(arg.Data.DescricaoParametroDataInicio + ":");

                _ocorrencia.ParametroDataFim.text(_ocorrencia.ParametroDataFim.defText);
                if (arg.Data.DescricaoParametroDataFim)
                    _ocorrencia.ParametroDataFim.text(arg.Data.DescricaoParametroDataFim + ":");

                _ocorrencia.ParametroData.text(_ocorrencia.ParametroData.defText);
                if (arg.Data.DescricaoParametroData)
                    _ocorrencia.ParametroData.text(arg.Data.DescricaoParametroData + ":");

                BuscarMiniaturasEExibirGridAnexos();
                CarregarDocumentos(_ocorrencia.Codigo.val());
                CarregarCTes(_ocorrencia.Codigo.val());
                CarregarDelegar(situacao);
                CarregarCargas(_ocorrencia.Codigo.val());
                CarregarContratoVeiculo(_ocorrencia.Codigo.val());
                CarregarHistoricoAutorizacao(_ocorrencia.Codigo.val());

                PreecherResumoChamado(arg.Data.Chamado);
                PreecherResumoChamados(arg.Data.Chamados);
                PreecherManobristas(arg.Data.Manobristas);
                CarregarClientes(_ocorrencia.Codigo.val());

                _ocorrencia.BaixarRateio.visible(_CONFIGURACAO_TMS.ExibirOpcaoDownloadPlanilhaRateioOcorrencia);
                if (arg.Data.OcorrenciaPorPeriodo) {
                    CarregarAnexosCargas();
                    CarregarAnexosContratoVeiculo();

                    var situacaoPermitePlanilhaRateio = (situacao == EnumSituacaoOcorrencia.Finalizada) && arg.Data.OcorrenciaPorPeriodo && _CONFIGURACAO_TMS.ExibirOpcaoDownloadPlanilhaRateioOcorrencia;
                    _ocorrencia.BaixarRateio.visible(situacaoPermitePlanilhaRateio);
                }

                AtualizarGridRegras();

                _autorizacao.Tomador.enable(true);
                _autorizacao.AnalisarAprovacao.visible(arg.Data.PermitirAnalisarOcorrencia);
                _autorizacao.RemoverAnaliseAprovacao.visible(arg.Data.PermitirRemoverAnaliseOcorrencia);
                _autorizacao.PercentualJurosParcela.visible(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.PercentualJurosParcela.required(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.PercentualJurosParcela.val(arg.Data.PercentualJurosParcela);
                _autorizacao.PeriodoPagamento.val(arg.Data.PeriodoPagamento);
                _autorizacao.QuantidadeParcelas.visible(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.QuantidadeParcelas.required(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.QuantidadeParcelas.val(arg.Data.QuantidadeParcelas);
                _autorizacao.Pagamento.val(arg.Data.Pagamento);
                _autorizacao.Pagamento.enable(arg.Data.PermitirAnalisarOcorrencia);

                if (arg.Data.PermiteSelecionarTomador) {

                    _autorizacao.Tomador.permiteSelecionarTomador(true);
                    _autorizacao.Tomador.visible(true);

                    if (arg.Data.Tomador != null)
                        _autorizacao.Tomador.val(arg.Data.Tomador);
                    else
                        _autorizacao.Tomador.val(EnumResponsavelOcorrencia.ConformeCTeAnterior);

                    if (arg.Data.CodigoClienteTomador != "") {
                        _autorizacao.ClienteTomador.codEntity(arg.Data.CodigoClienteTomador);
                        _autorizacao.ClienteTomador.val(arg.Data.NomeClienteTomador);
                    }

                    if (_autorizacao.Tomador.val() == EnumResponsavelOcorrencia.Outros)
                        _autorizacao.ClienteTomador.visible(true);
                    else {
                        _autorizacao.ClienteTomador.visible(false);
                        _autorizacao.ClienteTomador.val("");
                        _autorizacao.ClienteTomador.codEntity(0);
                    }
                }
                else {

                    if (arg.Data.TomadorPadrao != null) {
                        _autorizacao.Tomador.permiteSelecionarTomador(true);

                        _autorizacao.Tomador.val(arg.Data.TomadorPadrao);
                        _autorizacao.Tomador.visible(true);
                        _autorizacao.Tomador.enable(false);

                        if (_autorizacao.Tomador.val() == EnumResponsavelOcorrencia.Outros)
                            _autorizacao.ClienteTomador.visible(true);
                        else {
                            _autorizacao.ClienteTomador.visible(false);
                            _autorizacao.ClienteTomador.val("");
                            _autorizacao.ClienteTomador.codEntity(0);
                        }
                    }
                    else
                        _autorizacao.Tomador.permiteSelecionarTomador(false);
                }

                // Abre modal da ocorrencia
                Global.abrirModal("divModalOcorrencia");
                $("#divModalOcorrencia").one('hidden.bs.modal', function () {
                    limparCamposOcorrencia();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

function cancelarRejeicaoSelecionadosClick() {
    LimparCampos(_rejeicao);
    Global.fecharModal("divModalRejeitarOcorrencia");
}

function cancelarAprovacaoMultiplosSelecionadosClick() {
    LimparCampos(_aprovacaoMultiplasOcorrencias);
    Global.fecharModal("divModalAprovarMultiplasOcorrencias");
}

function cancelarAprovacaoSelecionadosClick() {
    LimparCampos(_aprovacao);
    Global.fecharModal("divModalAprovarOcorrencia");
}

function limparCamposOcorrencia() {
    resetarTabs();
    //limparAnexos();
    limparRegras();
    LimparResumoChamado();
    LimparManobristas();
}

function resetarTabs() {
    Global.ResetarAbas();
}

function responsavelChange(e, sender) {
    if (_autorizacao.Tomador.val() == EnumResponsavelOcorrencia.Outros)
        _autorizacao.ClienteTomador.visible(true);
    else {
        _autorizacao.ClienteTomador.visible(false);
        _autorizacao.ClienteTomador.val("");
        _autorizacao.ClienteTomador.codEntity(0);
    }
}

function buscarOcorrenciaPeloLinkDeAcessoCliente(ocorrenciaGrid, row) {
    _rowGridOcorrenciaAtual = row;
    _codigoOcorrenciaAtual = ocorrenciaGrid.Codigo;

    limparCamposOcorrencia();
    var pesquisa = RetornarObjetoPesquisa(_pesquisaOcorrencias);
    _ocorrencia.Codigo.val(ocorrenciaGrid.Codigo);
    _ocorrencia.Usuario.val(pesquisa.Usuario);
    _ocorrencia.EtapaAutorizacao.val(pesquisa.EtapaAutorizacao);

    BuscarPorCodigo(_ocorrencia, "AutorizacaoOcorrencia/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                if (arg.Data.ErroIntegracaoComGPA)
                    _ocorrencia.ReenviarIntegracao.visible(true);
                else
                    _ocorrencia.ReenviarIntegracao.visible(false);

                if (arg.Data.CalculaValorPorTabelaFrete)
                    _ocorrencia.ParametroApenasReboque.text(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ReboqueTracao);

                var situacao = arg.Data.EnumSituacao;
                _ocorrencia.ObservacaoAprovador.enable(true);
                _ocorrencia.SalvarObservacaoAprovador.visible(_CONFIGURACAO_TMS.ExibirObservacaoAprovadorAutorizacaoOcorrencia);
                if (situacao !== EnumSituacaoOcorrencia.AutorizacaoPendente && situacao !== EnumSituacaoOcorrencia.AgAprovacao && situacao !== EnumSituacaoOcorrencia.AgAutorizacaoEmissao) {
                    _ocorrencia.ObservacaoAprovador.enable(false);
                    _ocorrencia.SalvarObservacaoAprovador.visible(false);
                }

                if (arg.Data.DataChegadaVeiculo) {
                    _ocorrencia.DataChegadaVeiculo.val(arg.Data.DataChegadaVeiculo.DataChegadaVeiculo);
                    _ocorrencia.DataChegadaVeiculo.text(arg.Data.DataChegadaVeiculo.Descricao);
                } else {
                    _ocorrencia.DataChegadaVeiculo.val("");
                }

                _ocorrencia.ParametroDataInicio.text(_ocorrencia.ParametroDataInicio.defText);
                if (arg.Data.DescricaoParametroDataInicio)
                    _ocorrencia.ParametroDataInicio.text(arg.Data.DescricaoParametroDataInicio + ":");

                _ocorrencia.ParametroDataFim.text(_ocorrencia.ParametroDataFim.defText);
                if (arg.Data.DescricaoParametroDataFim)
                    _ocorrencia.ParametroDataFim.text(arg.Data.DescricaoParametroDataFim + ":");

                _ocorrencia.ParametroData.text(_ocorrencia.ParametroData.defText);
                if (arg.Data.DescricaoParametroData)
                    _ocorrencia.ParametroData.text(arg.Data.DescricaoParametroData + ":");

                BuscarMiniaturasEExibirGridAnexos();
                CarregarDocumentos(_ocorrencia.Codigo.val());
                CarregarCTes(_ocorrencia.Codigo.val());
                CarregarDelegar(situacao);
                CarregarCargas(_ocorrencia.Codigo.val());
                CarregarContratoVeiculo(_ocorrencia.Codigo.val());
                CarregarHistoricoAutorizacao(_ocorrencia.Codigo.val());

                PreecherResumoChamado(arg.Data.Chamado);
                PreecherResumoChamados(arg.Data.Chamados);
                PreecherManobristas(arg.Data.Manobristas);
                CarregarClientes(_ocorrencia.Codigo.val());

                _ocorrencia.BaixarRateio.visible(_CONFIGURACAO_TMS.ExibirOpcaoDownloadPlanilhaRateioOcorrencia);
                if (arg.Data.OcorrenciaPorPeriodo) {
                    CarregarAnexosCargas();
                    CarregarAnexosContratoVeiculo();

                    var situacaoPermitePlanilhaRateio = (situacao == EnumSituacaoOcorrencia.Finalizada) && arg.Data.OcorrenciaPorPeriodo && _CONFIGURACAO_TMS.ExibirOpcaoDownloadPlanilhaRateioOcorrencia;
                    _ocorrencia.BaixarRateio.visible(situacaoPermitePlanilhaRateio);
                }

                AtualizarGridRegras();

                _autorizacao.Tomador.enable(true);
                _autorizacao.AnalisarAprovacao.visible(arg.Data.PermitirAnalisarOcorrencia);
                _autorizacao.RemoverAnaliseAprovacao.visible(arg.Data.PermitirRemoverAnaliseOcorrencia);
                _autorizacao.PercentualJurosParcela.visible(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.PercentualJurosParcela.required(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.PercentualJurosParcela.val(arg.Data.PercentualJurosParcela);
                _autorizacao.PeriodoPagamento.val(arg.Data.PeriodoPagamento);
                _autorizacao.QuantidadeParcelas.visible(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.QuantidadeParcelas.required(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.QuantidadeParcelas.val(arg.Data.QuantidadeParcelas);
                _autorizacao.Pagamento.val(arg.Data.Pagamento);
                _autorizacao.Pagamento.enable(arg.Data.PermitirAnalisarOcorrencia);

                if (arg.Data.PermiteSelecionarTomador) {

                    _autorizacao.Tomador.permiteSelecionarTomador(true);
                    _autorizacao.Tomador.visible(true);

                    if (arg.Data.Tomador != null)
                        _autorizacao.Tomador.val(arg.Data.Tomador);
                    else
                        _autorizacao.Tomador.val(EnumResponsavelOcorrencia.ConformeCTeAnterior);

                    if (arg.Data.CodigoClienteTomador != "") {
                        _autorizacao.ClienteTomador.codEntity(arg.Data.CodigoClienteTomador);
                        _autorizacao.ClienteTomador.val(arg.Data.NomeClienteTomador);
                    }

                    if (_autorizacao.Tomador.val() == EnumResponsavelOcorrencia.Outros)
                        _autorizacao.ClienteTomador.visible(true);
                    else {
                        _autorizacao.ClienteTomador.visible(false);
                        _autorizacao.ClienteTomador.val("");
                        _autorizacao.ClienteTomador.codEntity(0);
                    }
                }
                else {

                    if (arg.Data.TomadorPadrao != null) {
                        _autorizacao.Tomador.permiteSelecionarTomador(true);

                        _autorizacao.Tomador.val(arg.Data.TomadorPadrao);
                        _autorizacao.Tomador.visible(true);
                        _autorizacao.Tomador.enable(false);

                        if (_autorizacao.Tomador.val() == EnumResponsavelOcorrencia.Outros)
                            _autorizacao.ClienteTomador.visible(true);
                        else {
                            _autorizacao.ClienteTomador.visible(false);
                            _autorizacao.ClienteTomador.val("");
                            _autorizacao.ClienteTomador.codEntity(0);
                        }
                    }
                    else
                        _autorizacao.Tomador.permiteSelecionarTomador(false);
                }

                // Abre modal da ocorrencia
                Global.abrirModal("divModalOcorrencia");
                $("#divModalOcorrencia").one('hidden.bs.modal', function () {
                    limparCamposOcorrencia();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

function BuscarMiniaturasEExibirGridAnexos() {
    if (_anexosOcorrencias.Anexos.val() != "")
        _anexosOcorrencias.Anexos.val.removeAll();
    iniciarRequisicao();

    executarReST('AutorizacaoOcorrencia/ObterMiniaturasOcorrencia', { Codigo: _ocorrencia.Codigo.val() }, function (arg) {
        if (arg.Success) {
            const anexos = [];
            for (let i = 0; i < arg.Data.Imagens.length; i++) {
                const obj = arg.Data.Imagens[i];
                anexos.push(obj);
            }

            _anexosOcorrencias.Anexos.val(anexos);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
        finalizarRequisicao();
    });
}

function adicionarAnexoAutorizacaoClick() {
    $("#" + _anexosOcorrencias.Arquivo.id).trigger("click");
}

function enviarArquivoAnexo(e, sender) {
    if (_anexosOcorrencias.Arquivo.val() != "") {
        var file = document.getElementById(_anexosOcorrencias.Arquivo.id);

        var formData = new FormData();
        formData.append("upload", file.files[0]);

        var dados = {
            Codigo: _ocorrencia.Codigo.val()
        };

        enviarArquivo("AutorizacaoOcorrencia/AnexarArquivosAutorizacao", dados, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    BuscarMiniaturasEExibirGridAnexos()
                } else {
                    BuscarMiniaturasEExibirGridAnexos();
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function downloadAnexoClick(e) {
    if (e.Codigo > 0 && e.GuidNomeArquivo != "") {
        const data = { Codigo: e.Codigo }
        executarDownload("OcorrenciaAnexos/DownloadAnexo", data);
    }
}

function limparFiltrosAutorizacaoOcorrencia() {
    LimparCampos(_pesquisaOcorrencias);
}

function carregarOcorrenciaUsuarioAcessadoViaLink(tokenAcesso) {
    _ocorrencia.Codigo.val(tokenAcesso);

    BuscarPorCodigo(_ocorrencia, "AutorizacaoOcorrencia/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                if (arg.Data.ErroIntegracaoComGPA)
                    _ocorrencia.ReenviarIntegracao.visible(true);
                else
                    _ocorrencia.ReenviarIntegracao.visible(false);

                if (arg.Data.CalculaValorPorTabelaFrete)
                    _ocorrencia.ParametroApenasReboque.text(Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ReboqueTracao);

                var situacao = arg.Data.EnumSituacao;
                _ocorrencia.ObservacaoAprovador.enable(true);
                _ocorrencia.SalvarObservacaoAprovador.visible(_CONFIGURACAO_TMS.ExibirObservacaoAprovadorAutorizacaoOcorrencia);
                if (situacao !== EnumSituacaoOcorrencia.AutorizacaoPendente && situacao !== EnumSituacaoOcorrencia.AgAprovacao && situacao !== EnumSituacaoOcorrencia.AgAutorizacaoEmissao) {
                    _ocorrencia.ObservacaoAprovador.enable(false);
                    _ocorrencia.SalvarObservacaoAprovador.visible(false);
                }

                if (arg.Data.DataChegadaVeiculo) {
                    _ocorrencia.DataChegadaVeiculo.val(arg.Data.DataChegadaVeiculo.DataChegadaVeiculo);
                    _ocorrencia.DataChegadaVeiculo.text(arg.Data.DataChegadaVeiculo.Descricao);
                } else {
                    _ocorrencia.DataChegadaVeiculo.val("");
                }

                _ocorrencia.ParametroDataInicio.text(_ocorrencia.ParametroDataInicio.defText);
                if (arg.Data.DescricaoParametroDataInicio)
                    _ocorrencia.ParametroDataInicio.text(arg.Data.DescricaoParametroDataInicio + ":");

                _ocorrencia.ParametroDataFim.text(_ocorrencia.ParametroDataFim.defText);
                if (arg.Data.DescricaoParametroDataFim)
                    _ocorrencia.ParametroDataFim.text(arg.Data.DescricaoParametroDataFim + ":");

                _ocorrencia.ParametroData.text(_ocorrencia.ParametroData.defText);
                if (arg.Data.DescricaoParametroData)
                    _ocorrencia.ParametroData.text(arg.Data.DescricaoParametroData + ":");

                BuscarMiniaturasEExibirGridAnexos();
                CarregarDocumentos(_ocorrencia.Codigo.val());
                CarregarCTes(_ocorrencia.Codigo.val());
                CarregarDelegar(situacao);
                CarregarCargas(_ocorrencia.Codigo.val());
                CarregarContratoVeiculo(_ocorrencia.Codigo.val());
                CarregarHistoricoAutorizacao(_ocorrencia.Codigo.val());

                PreecherResumoChamado(arg.Data.Chamado);
                PreecherResumoChamados(arg.Data.Chamados);
                PreecherManobristas(arg.Data.Manobristas);
                CarregarClientes(_ocorrencia.Codigo.val());

                _ocorrencia.BaixarRateio.visible(_CONFIGURACAO_TMS.ExibirOpcaoDownloadPlanilhaRateioOcorrencia);
                if (arg.Data.OcorrenciaPorPeriodo) {
                    CarregarAnexosCargas();
                    CarregarAnexosContratoVeiculo();

                    var situacaoPermitePlanilhaRateio = (situacao == EnumSituacaoOcorrencia.Finalizada) && arg.Data.OcorrenciaPorPeriodo && _CONFIGURACAO_TMS.ExibirOpcaoDownloadPlanilhaRateioOcorrencia;
                    _ocorrencia.BaixarRateio.visible(situacaoPermitePlanilhaRateio);
                }

                AtualizarGridRegras();

                _autorizacao.Tomador.enable(true);
                _autorizacao.AnalisarAprovacao.visible(arg.Data.PermitirAnalisarOcorrencia);
                _autorizacao.RemoverAnaliseAprovacao.visible(arg.Data.PermitirRemoverAnaliseOcorrencia);
                _autorizacao.PercentualJurosParcela.visible(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.PercentualJurosParcela.required(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.PercentualJurosParcela.val(arg.Data.PercentualJurosParcela);
                _autorizacao.PeriodoPagamento.val(arg.Data.PeriodoPagamento);
                _autorizacao.QuantidadeParcelas.visible(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.QuantidadeParcelas.required(arg.Data.ExibirParcelasNaAprovacao);
                _autorizacao.QuantidadeParcelas.val(arg.Data.QuantidadeParcelas);
                _autorizacao.Pagamento.val(arg.Data.Pagamento);
                _autorizacao.Pagamento.enable(arg.Data.PermitirAnalisarOcorrencia);

                if (arg.Data.PermiteSelecionarTomador) {

                    _autorizacao.Tomador.permiteSelecionarTomador(true);
                    _autorizacao.Tomador.visible(true);

                    if (arg.Data.Tomador != null)
                        _autorizacao.Tomador.val(arg.Data.Tomador);
                    else
                        _autorizacao.Tomador.val(EnumResponsavelOcorrencia.ConformeCTeAnterior);

                    if (arg.Data.CodigoClienteTomador != "") {
                        _autorizacao.ClienteTomador.codEntity(arg.Data.CodigoClienteTomador);
                        _autorizacao.ClienteTomador.val(arg.Data.NomeClienteTomador);
                    }

                    if (_autorizacao.Tomador.val() == EnumResponsavelOcorrencia.Outros)
                        _autorizacao.ClienteTomador.visible(true);
                    else {
                        _autorizacao.ClienteTomador.visible(false);
                        _autorizacao.ClienteTomador.val("");
                        _autorizacao.ClienteTomador.codEntity(0);
                    }
                }
                else {

                    if (arg.Data.TomadorPadrao != null) {
                        _autorizacao.Tomador.permiteSelecionarTomador(true);

                        _autorizacao.Tomador.val(arg.Data.TomadorPadrao);
                        _autorizacao.Tomador.visible(true);
                        _autorizacao.Tomador.enable(false);

                        if (_autorizacao.Tomador.val() == EnumResponsavelOcorrencia.Outros)
                            _autorizacao.ClienteTomador.visible(true);
                        else {
                            _autorizacao.ClienteTomador.visible(false);
                            _autorizacao.ClienteTomador.val("");
                            _autorizacao.ClienteTomador.codEntity(0);
                        }
                    }
                    else
                        _autorizacao.Tomador.permiteSelecionarTomador(false);
                }

                // Abre modal da ocorrencia
                Global.abrirModal("divModalOcorrencia");
                $("#divModalOcorrencia").one('hidden.bs.modal', function () {
                    limparCamposOcorrencia();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}