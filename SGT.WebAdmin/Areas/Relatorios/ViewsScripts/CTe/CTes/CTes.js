/// <reference path="../../../../../ViewsScripts/Consultas/Container.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Porto.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/PedidoViagemNavio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOcorrencia.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCustoViagem.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumFatura.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoDocumentoCreditoDebito.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumICMSCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPropostaMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoServicoMultimodal.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCarroceria.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoProprietarioVeiculo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCargaMercante.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCTes, _pesquisaCTes, _CRUDRelatorio, _CRUDFiltrosRelatorio;
var _relatorioCTes;

var _situacaoCTe = [
    { text: "Autorizado", value: "A" },
    { text: "Pendente", value: "P" },
    { text: "Enviado", value: "E" },
    { text: "Rejeitado", value: "R" },
    { text: "Cancelado", value: "C" },
    { text: "Anulado Gerencialmente", value: "G" },
    { text: "Inutilizado", value: "I" },
    { text: "Denegado", value: "D" },
    { text: "Em Digitação", value: "S" },
    { text: "Em Cancelamento", value: "K" },
    { text: "Em Inutilização", value: "L" },
    { text: "Anulado", value: "Z" }
];

var _situacaoPago = [
    { text: "Todos", value: "" },
    { text: "Pago", value: true },
    { text: "Pendente", value: false }
];

var _tipoPropriedadeVeiculo = [
    { text: "Todos", value: "" },
    { text: "Próprio", value: "P" },
    { text: "Terceiro", value: "T" },
    { text: "Outros", value: "O" }
];

var _tipoCTeVinculadoCarga = [
    { text: "Todos", value: "" },
    { text: "CT-e com Carga", value: true },
    { text: "CT-e sem Carga", value: false }
];

var _tipoCargaEmissaoFinalizada = [
    { text: "Todas", value: "" },
    { text: "Finalizada", value: true },
    { text: "Pendente", value: false }
];

var _situacaoFatura = [
    { text: "Todas", value: "" },
    { text: "Em Andamento", value: EnumSituacoesFatura.EmAndamento },
    { text: "Fechada", value: EnumSituacoesFatura.Fechado }
];

var _situacaoFaturamentoCTe = [
    { text: "Todos", value: "" },
    { text: "Faturado", value: true },
    { text: "Não Faturado", value: false }
];

var _serieConfig = {
    precision: 0,
    allowZero: false,
    allowNegative: false,
    thousands: ""
};

var _opcoesPossuiDataEntrega = [
    { text: "Todos", value: "" },
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var _tipoCarroceria = [
    { text: "Não Aplicado", value: "00" },
    { text: "Aberta", value: "01" },
    { text: "Fechada / Baú", value: "02" },
    { text: "Graneleira", value: "03" },
    { text: "Porta Container", value: "04" },
    { text: "Utilitário", value: "05" },
    { text: "Sider", value: "06" }
];

var _tipoTomador = [
    { value: EnumTipoTomador.Destinatario, text: "Destinatário" },
    { value: EnumTipoTomador.Expedidor, text: "Expedidor" },
    { value: EnumTipoTomador.Recebedor, text: "Recebedor" },
    { value: EnumTipoTomador.Remetente, text: "Remetente" },
    { value: EnumTipoTomador.Outros, text: "Outros" }
];

var _opcoesPermiteGerarFaturamento = [
    { text: "Todos", value: "" },
    { text: "Sim", value: true },
    { text: "Não", value: false}
]

var _opcoesTipoEmissao = [
    { text: "Todos", value: "00" },
    { text: "Emissão Carga", value: "01" },
    { text: "Emissão Manual", value: "02" },
    { text: "Emissão Agrupado", value: "03" }
]

var PesquisaCTes = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    var dataAtual = moment().format("DD/MM/YYYY");

    this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: ko.observable(false) });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Emissão Final: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: ko.observable(false) });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.DataInicialAutorizacao = PropertyEntity({ text: "Data Autorização Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalAutorizacao = PropertyEntity({ text: "Data Autorização Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialAutorizacao.dateRangeLimit = this.DataFinalAutorizacao;
    this.DataFinalAutorizacao.dateRangeInit = this.DataInicialAutorizacao;

    this.DataInicialCancelamento = PropertyEntity({ text: "Data Cancelamento Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalCancelamento = PropertyEntity({ text: "Data Cancelamento Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialCancelamento.dateRangeLimit = this.DataFinalCancelamento;
    this.DataFinalCancelamento.dateRangeInit = this.DataInicialCancelamento;

    this.DataInicialAnulacao = PropertyEntity({ text: "Data Anulação Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalAnulacao = PropertyEntity({ text: "Data Anulação Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialAnulacao.dateRangeLimit = this.DataFinalAnulacao;
    this.DataFinalAnulacao.dateRangeInit = this.DataInicialAnulacao;

    this.DataInicialImportacao = PropertyEntity({ text: "Data Importação Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalImportacao = PropertyEntity({ text: "Data Importação Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialImportacao.dateRangeLimit = this.DataFinalImportacao;
    this.DataFinalImportacao.dateRangeInit = this.DataInicialImportacao;

    this.DataInicialFatura = PropertyEntity({ text: "Data Fatura Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalFatura = PropertyEntity({ text: "Data Fatura Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialFatura.dateRangeLimit = this.DataFinalFatura;
    this.DataFinalFatura.dateRangeInit = this.DataInicialFatura;

    this.DataInicialEntrega = PropertyEntity({ text: "Data Entrega Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalEntrega = PropertyEntity({ text: "Data Entrega Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialEntrega.dateRangeLimit = this.DataFinalEntrega;
    this.DataFinalEntrega.dateRangeInit = this.DataInicialEntrega;

    this.DataInicialColeta = PropertyEntity({ text: "Data Coleta Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalColeta = PropertyEntity({ text: "Data Coleta Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialColeta.dateRangeLimit = this.DataFinalColeta;
    this.DataFinalColeta.dateRangeInit = this.DataInicialColeta;

    this.DataPagamentoInicial = PropertyEntity({ text: "Data Pagamento Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataPagamentoFinal = PropertyEntity({ text: "Data Pagamento Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataPagamentoInicial.dateRangeLimit = this.DataPagamentoFinal;
    this.DataPagamentoFinal.dateRangeInit = this.DataPagamentoInicial;

    this.DataVencimentoInicial = PropertyEntity({ text: "Data Vencimento Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataVencimentoFinal = PropertyEntity({ text: "Data Vencimento Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataVencimentoInicial.dateRangeLimit = this.DataVencimentoFinal;
    this.DataVencimentoFinal.dateRangeInit = this.DataVencimentoInicial;

    this.DataConfirmacaoDocumentosInicial = PropertyEntity({ text: "Data Inicial Confir. Doc.: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataConfirmacaoDocumentosFinal = PropertyEntity({ text: "Data Final Confir. Doc.: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataConfirmacaoDocumentosInicial.dateRangeLimit = this.DataConfirmacaoDocumentosFinal;
    this.DataConfirmacaoDocumentosFinal.dateRangeInit = this.DataConfirmacaoDocumentosInicial;

    this.PossuiDataEntrega = PropertyEntity({ text: "Possui Data de Entrega:", options: _opcoesPossuiDataEntrega, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.PossuiNFSManual = PropertyEntity({ text: "Possui NFS Manual Gerada:", options: _opcoesPossuiDataEntrega, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });

    this.NumeroInicial = PropertyEntity({ text: "Núm. Inicial: ", getType: typesKnockout.int, visible: ko.observable(true) });
    this.NumeroFinal = PropertyEntity({ text: "Núm. Final: ", getType: typesKnockout.int, visible: ko.observable(true) });

    this.NFe = PropertyEntity({ text: "Núm. NF-e: ", getType: typesKnockout.string, visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });

    this.SituacaoFatura = PropertyEntity({ text: "Situação da Fatura:", options: _situacaoFatura, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.SituacaoFaturamentoCTe = PropertyEntity({ text: "Sit. do Faturamento do CT-e:", options: _situacaoFaturamentoCTe, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.TipoDocumentoCreditoDebito = PropertyEntity({ val: ko.observable(EnumTipoDocumentoCreditoDebito.Todos), options: EnumTipoDocumentoCreditoDebito.obterOpcoesPesquisa(), def: EnumTipoDocumentoCreditoDebito.Todos, text: "Tipo do Documento:", visible: ko.observable(false) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Pedido = PropertyEntity({ type: types.map, text: "Pedido:", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "Veículo:", issue: 143, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ text: "Centro Resultado:", type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Filial = PropertyEntity({ text: "Filial:", issue: 70, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.FilialVenda = PropertyEntity({ text: "Filial Venda:", issue: 71, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });

    this.Transportador = PropertyEntity({ text: "Transportador:", issue: 69, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Terceiro = PropertyEntity({ text: "Prop. Veículo:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Serie = PropertyEntity({ text: "Série:", getType: typesKnockout.int, val: ko.observable(""), visible: ko.observable(true), configInt: _serieConfig, maxlength: 4, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "CodigoCargaEmbarcador" } });
    this.PreCarga = PropertyEntity({ type: types.map, text: "Pré Carga:", visible: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), issue: 16, visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), issue: 16, visible: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Origem:", idBtnSearch: guid(), issue: 12, visible: ko.observable(true) });
    this.EstadoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Estado de Destino:", idBtnSearch: guid(), issue: 12, visible: ko.observable(true) });

    this.TipoCTe = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoCTe.ObterOpcoes(), text: "Tipo do CT-e:", visible: ko.observable(true) });
    this.TipoServico = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoServicoCTe.obterOpcoes(), text: "Tipo de Serviço:", visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: _situacaoCTe, text: "Situação do CT-e:", issue: 120, visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.TipoTomador = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: _tipoTomador, text: "Tipo do Tomador:", visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });

    this.TipoPropriedadeVeiculo = PropertyEntity({ val: ko.observable(""), options: _tipoPropriedadeVeiculo, def: "", text: "Tipo de Propriedade:", visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.Pago = PropertyEntity({ val: ko.observable(""), options: _situacaoPago, def: "", text: "Pago:", visible: ko.observable(false), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.ExibirNotasFiscais = PropertyEntity({ text: "Exibir Notas Fiscais dos CT-es?", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.AnuladoGerencialmente = PropertyEntity({ text: "CTes anulados gerencialmente sem Anulação Fiscal", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.CTeVinculadoACarga = PropertyEntity({ val: ko.observable(true), options: _tipoCTeVinculadoCarga, def: true, text: "Vínculo à Carga:", visible: ko.observable(false) });
    this.CargaEmissaoFinalizada = PropertyEntity({ val: ko.observable(0), options: _tipoCargaEmissaoFinalizada, def: 0, text: "Emissão dos Documentos da Carga:", visible: ko.observable(false) });

    this.ModeloDocumento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Mod. Documento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CFOP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ContratoFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Contrato de Frete:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.CTe = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "CTe:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Vendedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Vendedor:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroDeCustoViagemCodigo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Custo Viagem: ", idBtnSearch: guid(), visible: ko.observable(true) });

    this.FuncionarioResponsavel = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Funcionário Responsável Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.GruposPessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas do Tomador:", issue: 58, idBtnSearch: guid(), visible: ko.observable(false) });
    this.GruposPessoasDiferente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas do Tomador Diferente de:", issue: 58, idBtnSearch: guid(), visible: ko.observable(true) });
    this.GruposPessoasRemetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas do Remetente:", issue: 58, idBtnSearch: guid(), visible: ko.observable(false) });

    this.CST = PropertyEntity({ text: "CST:", options: EnumICMSCTe.ObterOpcoes(), val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, visible: ko.observable(true) });
    this.SegmentoVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Segmento do Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoProposta = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Tipo Proposta:", options: EnumTipoPropostaMultimodal.obterOpcoesSemNumero(), visible: ko.observable(true) });
    this.ChaveCTe = PropertyEntity({ text: "Chave do CT-e:", maxlength: 44 });
    this.NumeroDocumentoRecebedor = PropertyEntity({ type: types.string, text: "Nº Doc. Recebedor Ocorrência:", visible: ko.observable(true) });
    this.NumeroPedidoCliente = PropertyEntity({ text: "Nº do Pedido Cliente:", val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.TipoDeCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga:", issue: 53, idBtnSearch: guid() });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarroceria = PropertyEntity({ val: ko.observable(EnumTipoCarroceria.Todos), options: EnumTipoCarroceria.obterOpcoesPesquisa(), def: EnumTipoCarroceria.Todos, text: "Tipo de Carroceria: ", required: true, issue: 154, enable: ko.observable(true) });
    this.TipoProprietarioVeiculo = PropertyEntity({ text: "Tipo de Transportador:", options: EnumTipoProprietarioVeiculo.obterOpcoesPesquisa(), val: ko.observable(EnumTipoProprietarioVeiculo.Todos), def: EnumTipoProprietarioVeiculo.Todos });
    this.TipoModal = PropertyEntity({ val: ko.observable(EnumTipoModal.Todos), options: EnumTipoModal.obterOpcoesPesquisa(), def: EnumTipoModal.Todos, text: "Tipo Modal", visible: ko.observable(true), enable: ko.observable(true) });
    this.PermiteGerarFaturamento = PropertyEntity({ text: "Permite gerar faturamento:", options: _opcoesPermiteGerarFaturamento, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.NumeroContratoFreteTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Número Contrado de Frete Terceiro:", idBtnSearch: guid(), issue: 56 });
    this.TipoOSConvertido = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Tipo OS Convertido: ", options: EnumTipoOSConvertido.obterOpcoes(), visible: ko.observable(true) });
    this.TipoOS = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Tipo OS: ", options: EnumTipoOS.obterOpcoes(), visible: ko.observable(true) });
    this.ProvedorOS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Provedor OS:", idBtnSearch: guid() });
    this.TipoEmissao = PropertyEntity({ text: "Tipo Emissão:", options: _opcoesTipoEmissao, val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });

    //Emissão Multimodal
    this.NumeroBooking = PropertyEntity({ text: "Número Booking:" });
    this.NumeroOS = PropertyEntity({ text: "Número OS:" });
    this.NumeroControle = PropertyEntity({ text: "Número Controle:" });
    this.SituacaoCarga = PropertyEntity({ val: ko.observable(EnumSituacoesCarga.Todas), def: EnumSituacoesCarga.Todas, text: "Situação Carga:", options: EnumSituacoesCarga.obterOpcoesPesquisaTMS(), visible: ko.observable(true) });
    this.SituacaoCargaMercante = PropertyEntity({ text: "Situação Carga: ", getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumSituacaoCargaMercante.obterOpcoes(), def: [], visible: ko.observable(false) });

    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Origem:", idBtnSearch: guid() });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Porto Destino:", idBtnSearch: guid() });
    this.Viagem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Viagem:", idBtnSearch: guid() });
    this.Container = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Container:", idBtnSearch: guid() });
    this.TipoServicoMultiModal = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoServicoMultimodal.obterOpcoesSemNumero(), text: "Tipo de Serviço Multimodal:" });
    this.VeioPorImportacao = PropertyEntity({ text: "Veio por importação:", options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos });
    this.SomenteCTeSubstituido = PropertyEntity({ text: "Somente CT-e Substituído", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ApenasCTeEnviadoMercante = PropertyEntity({ text: "Apenas CT-es enviados para o mercante", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.CNPJDivergenteCTeMDFe = PropertyEntity({ text: "Apenas CNPJ divergente Transportador CTe/MDF-e", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.NumeroCRT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Número CRT:", idBtnSearch: guid() });
    this.MicDTA = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Número Mic/DTA:", idBtnSearch: guid() });

};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarFiltrosObrigatorios())
                _gridCTes.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCTes.Visible.visibleFade()) {
                _pesquisaCTes.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCTes.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******

function ConfigurarCamposPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        ConfigurarCamposTMS();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        ConfigurarCamposEmbarcador();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        ConfigurarCamposMultiCTe();
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Terceiros) {
        ConfigurarCamposTerceiros();
    }
}

function ConfigurarCamposTMS() {
    _pesquisaCTes.CargaEmissaoFinalizada.visible(true);
    _pesquisaCTes.CTeVinculadoACarga.visible(true);
    _pesquisaCTes.TipoPropriedadeVeiculo.cssClass("col col-lg-4 col-md-4 col-sm-12 col-xs-12");
    _pesquisaCTes.Situacao.cssClass("col col-lg-2 col-md-2 col-sm-12 col-xs-12");
    _pesquisaCTes.TipoServico.cssClass("col col-lg-2 col-md-2 col-sm-12 col-xs-12");
    _pesquisaCTes.ContratoFrete.visible(false);
    _pesquisaCTes.GruposPessoas.visible(true);
    _pesquisaCTes.GruposPessoasRemetente.visible(true);
}

function ConfigurarCamposTerceiros() {
    _pesquisaCTes.Transportador.visible(false);
    _pesquisaCTes.ContratoFrete.visible(false);
    _pesquisaCTes.Carga.visible(true);
    _pesquisaCTes.TipoPropriedadeVeiculo.visible(false);
    _pesquisaCTes.TipoServico.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
    _pesquisaCTes.Situacao.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
    _pesquisaCTes.CTeVinculadoACarga.visible(true);
}

function ConfigurarCamposEmbarcador() {
    _pesquisaCTes.Filial.visible(true);
    _pesquisaCTes.Pago.visible(true);
    _pesquisaCTes.TipoPropriedadeVeiculo.visible(false);
    _pesquisaCTes.TipoDocumentoCreditoDebito.visible(true);
    _pesquisaCTes.CTeVinculadoACarga.visible(true);

    _pesquisaCTes.Serie.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
    _pesquisaCTes.NFe.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
}

function ConfigurarCamposMultiCTe() {
    _pesquisaCTes.Transportador.visible(false);
    _pesquisaCTes.ContratoFrete.visible(false);
    _pesquisaCTes.Carga.visible(true);
    _pesquisaCTes.Pago.visible(true);
    _pesquisaCTes.Situacao.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
    _pesquisaCTes.Pago.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");
    _pesquisaCTes.TipoServico.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");
    _pesquisaCTes.TipoPropriedadeVeiculo.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
    _pesquisaCTes.NFe.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
    _pesquisaCTes.Serie.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
    _pesquisaCTes.CTeVinculadoACarga.visible(true);
}

function loadRelatorioCTes() {
    _pesquisaCTes = new PesquisaCTes();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridCTes = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/CTes/Pesquisa", _pesquisaCTes);

    _gridCTes.SetPermitirEdicaoColunas(true);
    _gridCTes.SetPermitirRedimencionarColunas(true);
    _gridCTes.SetQuantidadeLinhasPorPagina(10);

    _relatorioCTes = new RelatorioGlobal("Relatorios/CTes/BuscarDadosRelatorio", _gridCTes, function () {
        _relatorioCTes.loadRelatorio(function () {
            KoBindings(_pesquisaCTes, "knockoutPesquisaCTes", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCTes", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCTe", false);

            new BuscarClientes(_pesquisaCTes.Remetente);
            new BuscarTiposOperacao(_pesquisaCTes.TipoOperacao);
            new BuscarTipoOcorrencia(_pesquisaCTes.TipoOcorrencia);
            new BuscarConhecimentoNotaReferencia(_pesquisaCTes.CTe);
            new BuscarClientes(_pesquisaCTes.Destinatario);
            new BuscarTransportadores(_pesquisaCTes.Transportador, null, null, null, null, null, null, true);
            new BuscarLocalidades(_pesquisaCTes.Origem);
            new BuscarLocalidades(_pesquisaCTes.Destino);
            new BuscarCargas(_pesquisaCTes.Carga);
            new BuscarEstados(_pesquisaCTes.EstadoOrigem);
            new BuscarEstados(_pesquisaCTes.EstadoDestino);
            new BuscarVeiculos(_pesquisaCTes.Veiculo);
            new BuscarFilial(_pesquisaCTes.Filial);
            new BuscarModeloDocumentoFiscal(_pesquisaCTes.ModeloDocumento, null, null, null, null, true);
            new BuscarClientes(_pesquisaCTes.Terceiro);
            new BuscarCFOPs(_pesquisaCTes.CFOP, EnumTipoCFOP.Saida);
            new BuscarContratoFreteTransportador(_pesquisaCTes.ContratoFrete);
            new BuscarSegmentoVeiculo(_pesquisaCTes.SegmentoVeiculo);
            new BuscarPedidoViagemNavio(_pesquisaCTes.Viagem);
            new BuscarPorto(_pesquisaCTes.PortoOrigem);
            new BuscarPorto(_pesquisaCTes.PortoDestino);
            new BuscarContainers(_pesquisaCTes.Container);
            new BuscarClientes(_pesquisaCTes.Tomador);
            new BuscarClientes(_pesquisaCTes.ProvedorOS);
            new BuscarMotoristas(_pesquisaCTes.Motorista, null, null, null, null);
            new BuscarGruposPessoas(_pesquisaCTes.GruposPessoas);
            new BuscarGruposPessoas(_pesquisaCTes.GruposPessoasDiferente);
            new BuscarGruposPessoas(_pesquisaCTes.GruposPessoasRemetente);
            new BuscarCentroResultado(_pesquisaCTes.CentroResultado);
            new BuscarTiposdeCarga(_pesquisaCTes.TipoDeCarga);
            new BuscarModelosVeiculo(_pesquisaCTes.ModeloVeiculo);
            new BuscarContratoFrete(_pesquisaCTes.NumeroContratoFreteTerceiro);
            new BuscarFuncionario(_pesquisaCTes.FuncionarioResponsavel);
            new BuscarFuncionario(_pesquisaCTes.Vendedor, null, null, null, null, 1, null, null, null, null);
            new BuscarCentroCustoViagem(_pesquisaCTes.CentroDeCustoViagemCodigo);

            //new BuscarPermitirGerarFaturamento(_pesquisaCTes.PermiteGerarFaturamento);

            if (_CONFIGURACAO_TMS.AtivarNovosFiltrosConsultaCarga) {
                _pesquisaCTes.SituacaoCarga.visible(false);
                _pesquisaCTes.SituacaoCargaMercante.visible(true);
            }

            if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal)
                $("#liFiltrosEmissaoMultimodal").hide();

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCTes);

    ConfigurarCamposPorTipoServico();
}

function GerarRelatorioPDFClick(e, sender) {
    if (ValidarFiltrosObrigatorios())
        _relatorioCTes.gerarRelatorio("Relatorios/CTes/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    if (ValidarFiltrosObrigatorios())
        _relatorioCTes.gerarRelatorio("Relatorios/CTes/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function ValidarFiltrosObrigatorios() {
    var tudoCerto = true;
    var valido = ValidarCamposObrigatorios(_pesquisaCTes);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Filtros Obrigatórios", "Informe os filtros obrigatórios!");
        tudoCerto = false;
    }

    return tudoCerto;
}