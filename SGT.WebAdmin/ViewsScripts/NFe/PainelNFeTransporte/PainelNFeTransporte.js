/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumClassificacaoNFe.js" />
/// <reference path="../../Enumeradores/EnumFatura.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEntrega.js" />
/// <reference path="../../Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Enumeradores/EnumTipoLocalPrestacao.js" />
/// <reference path="../../Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaMercante.js" />
/// <reference path="AdicionarCargaNotaPendenteIntegracaoMercadoLivre.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPainelNFeTransporte;
var _pesquisaPainelNFeTransporte;
var _BaixarNotasAPIMercadoLivre;

var _situacaoFatura = [
    { text: "Todas", value: "" },
    { text: "Em Andamento", value: EnumSituacoesFatura.EmAndamento },
    { text: "Fechada", value: EnumSituacoesFatura.Fechado }
];

var _situacaoCargaEmbarcador = [
    { text: "Com a Logistica", value: EnumSituacoesCarga.NaLogistica },
    { text: "Dados da Carga", value: EnumSituacoesCarga.Nova },
    { text: "NF-e", value: EnumSituacoesCarga.AgNFe },
    { text: "Cálculo de Frete", value: EnumSituacoesCarga.CalculoFrete },
    { text: "Transportador", value: EnumSituacoesCarga.AgTransportador },
    { text: "Emissão dos Documentos", value: EnumSituacoesCarga.PendeciaDocumentos },
    { text: "Integração", value: EnumSituacoesCarga.AgIntegracao },
    { text: "Impressão", value: EnumSituacoesCarga.AgImpressaoDocumentos },
    { text: "Em Transporte", value: EnumSituacoesCarga.EmTransporte },
    { text: "Encerrada", value: EnumSituacoesCarga.Encerrada },
    { text: "Pagamento Liberado", value: EnumSituacoesCarga.LiberadoPagamento },
    { text: "Canceladas", value: EnumSituacoesCarga.Cancelada }
];

var _situacaoCargaTMS = [
    { text: "Em Andamento", value: EnumSituacoesCarga.NaLogistica },
    { text: "Etapa 1 (Carga)", value: EnumSituacoesCarga.Nova },
    { text: "Etapa 2 (NF-e)", value: EnumSituacoesCarga.AgNFe },
    { text: "Etapa 3 (Frete)", value: EnumSituacoesCarga.CalculoFrete },
    { text: "Etapa 4 e 5 (Documentos)", value: EnumSituacoesCarga.PendeciaDocumentos },
    { text: "Etapa 6 (Integração)", value: EnumSituacoesCarga.AgIntegracao },
    { text: "Em Transporte", value: EnumSituacoesCarga.EmTransporte },
    { text: "Finalizada", value: EnumSituacoesCarga.Encerrada },
    { text: "Cancelada", value: EnumSituacoesCarga.Cancelada },
    { text: "Anulada", value: EnumSituacoesCarga.Anulada }
];

var PesquisaPainelNFeTransporte = function () {

    this.DataInicialEmissao = PropertyEntity({ text: ko.observable("*Data Emissão Inicial: "), val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), getType: typesKnockout.date, visible: ko.observable(true), required: ko.observable(true) });
    this.DataFinalEmissao = PropertyEntity({ text: ko.observable("*Data Emissão Final: "), val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), getType: typesKnockout.date, visible: ko.observable(true), required: ko.observable(true) });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.DataInicialEmissaoCTe = PropertyEntity({ text: "Data Emissão Carga Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalEmissaoCTe = PropertyEntity({ text: "Data Emissão Carga Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialEmissaoCTe.dateRangeLimit = this.DataFinalEmissaoCTe;
    this.DataFinalEmissaoCTe.dateRangeInit = this.DataInicialEmissaoCTe;

    this.DataInicialEmissaoCarga = PropertyEntity({ text: "Data da Carga Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalEmissaoCarga = PropertyEntity({ text: "Data da Carga Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialEmissaoCarga.dateRangeLimit = this.DataFinalEmissaoCarga;
    this.DataFinalEmissaoCarga.dateRangeInit = this.DataInicialEmissaoCarga;

    this.DataInicialPrevisaoEntregaPedido = PropertyEntity({ text: "Data Previsão Entrega Pedido Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataFinalPrevisaoEntregaPedido = PropertyEntity({ text: "Data Previsão Entrega Pedido Final:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataInicialPrevisaoEntregaPedido.dateRangeLimit = this.DataFinalPrevisaoEntregaPedido;
    this.DataFinalPrevisaoEntregaPedido.dateRangeInit = this.DataInicialPrevisaoEntregaPedido;

    this.DataInicialInicioViagemPlanejada = PropertyEntity({ text: "Data Início Viagem Planejada Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalInicioViagemPlanejada = PropertyEntity({ text: "Data Início Viagem Planejada Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialInicioViagemPlanejada.dateRangeLimit = this.DataFinalInicioViagemPlanejada;
    this.DataFinalInicioViagemPlanejada.dateRangeInit = this.DataInicialInicioViagemPlanejada;

    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true, thousands: "" } });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true, thousands: "" } });

    this.SituacaoFatura = PropertyEntity({ text: "Situação da Fatura:", options: _situacaoFatura, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.TipoLocalPrestacao = PropertyEntity({ val: ko.observable(EnumTipoLocalPrestacao.todos), options: EnumTipoLocalPrestacao.obterOpcoesPesquisa(), def: EnumTipoLocalPrestacao.todos, text: "Tipo da Prestação: ", visible: ko.observable(true) });
    this.TipoCTe = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumTipoCTe.ObterOpcoes(), text: "Tipo do CT-e:" });

    this.ClassificacaoNFe = PropertyEntity({ val: ko.observable(EnumClassificacaoNFe.Todos), options: EnumClassificacaoNFe.obterOpcoesPesquisa(), def: EnumClassificacaoNFe.Todos, text: "Classificação NF-e: ", visible: ko.observable(true) });
    this.QuantidadeVolumesInicial = PropertyEntity({ text: "Quantidade Volumes Inicial: ", getType: typesKnockout.int, visible: ko.observable(true) });
    this.QuantidadeVolumesFinal = PropertyEntity({ text: "Quantidade Volumes Final: ", getType: typesKnockout.int, visible: ko.observable(true) });

    this.Remetente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Expedidor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid(), issue: 52, visible: ko.observable(true) });
    this.Filial = PropertyEntity({ text: "Filial:", issue: 70, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ text: ko.observable("Transportador:"), issue: 69, type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), val: ko.observable(""), text: "Carga:", idBtnSearch: guid(), issue: 195, visible: ko.observable(true), multiplesEntitiesConfig: { propDescricao: "CodigoCargaEmbarcador" } });
    this.Origem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), issue: 16, visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), issue: 16, visible: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado de Origem:", idBtnSearch: guid(), issue: 12, visible: ko.observable(true) });
    this.EstadoDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Estado de Destino:", idBtnSearch: guid(), issue: 12, visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), issue: 58, visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Carga:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
    this.Restricao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Restrição:", idBtnSearch: guid() });
    this.NotaFiscal = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Nota Fiscal:", idBtnSearch: guid(), visible: false });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });

    this.NotasFiscaisSemCarga = PropertyEntity({ options: Global.ObterOpcoesPesquisaBooleano("Sem Carga", "Com Carga"), val: ko.observable(""), def: "", text: "Notas Fiscais Sem Carga" });
    this.CargaTransbordo = PropertyEntity({ options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), val: ko.observable(""), def: "", text: "Carga de Transbordo" });
    this.NumeroPedidoCliente = PropertyEntity({ text: "Nº do Pedido Cliente:", val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });

    var situacaoCargaPesquisa = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador ? _situacaoCargaEmbarcador : EnumSituacoesCarga.obterOpcoesTMS();
    this.Situacoes = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Situação da Carga: ", options: ko.observable(situacaoCargaPesquisa), getType: typesKnockout.selectMultiple, visible: ko.observable(true) });
    this.SituacaoCargaMercante = PropertyEntity({ text: "Situações da Carga: ", getType: typesKnockout.selectMultiple, val: ko.observable([]), options: EnumSituacaoCargaMercante.obterOpcoes(), def: [], visible: ko.observable(false) });

    this.SituacoesEntrega = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, text: "Situação da Entrega: ", options: ko.observable(EnumSituacaoEntrega.obterOpcoes()) });
    this.PossuiExpedidor = PropertyEntity({ text: "Possui Expedidor:", options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos });
    this.PossuiRecebedor = PropertyEntity({ text: "Possui Recebedor:", options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos });

    this.DataPrevisaoCargaEntregaInicial = PropertyEntity({ text: "Data previsão entrega inicial:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataPrevisaoCargaEntregaFinal = PropertyEntity({ text: "Data previsão entrega final:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataPrevisaoCargaEntregaInicial.dateRangeLimit = this.DataPrevisaoCargaEntregaFinal;
    this.DataPrevisaoCargaEntregaFinal.dateRangeInit = this.DataPrevisaoCargaEntregaInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPainelNFeTransporte.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
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
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.DownloadLoteXML = PropertyEntity({
        eventClick: function (e) {
            downloadLoteXMLNFeClick();
        }, type: types.event, text: "Baixar Lote de XML", idFade: guid(), visible: ko.observable(true)
    });

    this.DownloadLoteDANFE = PropertyEntity({
        eventClick: function (e) {
            downloadLoteXMLDanfeClick();
        }, type: types.event, text: "Baixar Lote de DANFE", idFade: guid(), visible: ko.observable(true)
    });

    this.DownloadNotasAPIMerdoLivre = PropertyEntity({
        eventClick: function (e) {
            downloadLoteNotasAPIMercadoLivreClick();
        }, type: types.event, text: "Baixar Notas Integração Mercado Livre", idFade: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadPainelNFeTransporte() {

    _pesquisaPainelNFeTransporte = new PesquisaPainelNFeTransporte();

    KoBindings(_pesquisaPainelNFeTransporte, "knockoutPesquisaPainelNFeTransporte", false, _pesquisaPainelNFeTransporte.Pesquisar.id);

    new BuscarCargas(_pesquisaPainelNFeTransporte.Carga);
    new BuscarTransportadores(_pesquisaPainelNFeTransporte.Transportador);
    new BuscarClientes(_pesquisaPainelNFeTransporte.Remetente);
    new BuscarClientes(_pesquisaPainelNFeTransporte.Destinatario);
    new BuscarClientes(_pesquisaPainelNFeTransporte.Expedidor);
    new BuscarLocalidades(_pesquisaPainelNFeTransporte.Origem);
    new BuscarLocalidades(_pesquisaPainelNFeTransporte.Destino);
    new BuscarEstados(_pesquisaPainelNFeTransporte.EstadoOrigem);
    new BuscarEstados(_pesquisaPainelNFeTransporte.EstadoDestino);
    new BuscarFilial(_pesquisaPainelNFeTransporte.Filial);
    new BuscarGruposPessoas(_pesquisaPainelNFeTransporte.GrupoPessoas);
    new BuscarRestricaoEntrega(_pesquisaPainelNFeTransporte.Restricao);
    new BuscarNotaFiscal(_pesquisaPainelNFeTransporte.NotaFiscal);
    new BuscarTiposdeCarga(_pesquisaPainelNFeTransporte.TipoCarga);
    new BuscarTiposOperacao(_pesquisaPainelNFeTransporte.TipoOperacao);
    new BuscarMotoristas(_pesquisaPainelNFeTransporte.Motorista);

    HeaderAuditoria("PainelNFeTransporte");

    buscarNFesParaTransporte();

    loadAdicionarCargaNotaPendenteIntegracaoMercadoLivre();
}

function buscarNFesParaTransporte() {
    var downloadXMLNFe = { descricao: "Download XML NF-e", id: guid(), evento: "onclick", metodo: downloadXMLNFeClick, tamanho: "20", icone: "", visibilidade: true };
    var downloadXMLDanfeNFe = { descricao: "Download DANFE NF-e", id: guid(), evento: "onclick", metodo: downloadXMLDanfeNFeClick, tamanho: "20", icone: "", visibilidade: true };
    
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [downloadXMLNFe, downloadXMLDanfeNFe],
        tamanho: 7
    };

    var configExportacao = {
        url: "PainelNFeTransporte/ExportarPesquisa",
        titulo: "NF-es para Transporte"
    };

    _gridPainelNFeTransporte = new GridViewExportacao("grid-painel-nfe-transporte", "PainelNFeTransporte/Pesquisa", _pesquisaPainelNFeTransporte, menuOpcoes, configExportacao, { column: 5, dir: orderDir.desc }, 10, null);
    _gridPainelNFeTransporte.SetPermitirEdicaoColunas(true);
    _gridPainelNFeTransporte.SetSalvarPreferenciasGrid(true);
    _gridPainelNFeTransporte.CarregarGrid();
}

function downloadXMLNFeClick(e) {
    var data = { Codigo: e.Codigo };
    executarDownload("Relatorios/NFes/DownloadXML", data);
}

function downloadXMLDanfeNFeClick(e) {
    var data = { Codigo: e.Codigo };
    executarDownload("Relatorios/NFes/DownloadDANFE", data);
}

function downloadLoteXMLNFeClick() {

    let data = RetornarObjetoPesquisa(_pesquisaPainelNFeTransporte);
    executarDownload("PainelNFeTransporte/DownloadLoteXML", data);
}

function downloadLoteXMLDanfeClick() {
    let data = RetornarObjetoPesquisa(_pesquisaPainelNFeTransporte);
    executarDownload("PainelNFeTransporte/DownloadLoteXMLDanfe", data);
}

function downloadLoteNotasAPIMercadoLivreClick() {
    abrirModalSelecaoCargaNotasPendetesIntegracaoMercadoLivre();
}
