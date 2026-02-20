/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Globais.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumClassificacaoNFe.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumFatura.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoEntrega.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoLocalPrestacao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCTe.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCargaMercante.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioNotasEmitidas;
var _pesquisaNFes;
var _gridNFes;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;

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

var PesquisaNFes = function () {
    //var date = new Date();
    //var primeiroDia = moment((new Date(date.getFullYear(), date.getMonth(), 1))).format("DD/MM/YYYY");
    //var ultimoDia = moment((new Date(date.getFullYear(), date.getMonth() + 1, 0))).format("DD/MM/YYYY");

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

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

    this.DataInicialPrevisaoEntregaPedido = PropertyEntity({ text: "Data Previsão Entrega Pedido Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true)  });
    this.DataFinalPrevisaoEntregaPedido = PropertyEntity({ text: "Data Previsão Entrega Pedido Final:", val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataInicialPrevisaoEntregaPedido.dateRangeLimit = this.DataFinalPrevisaoEntregaPedido;
    this.DataFinalPrevisaoEntregaPedido.dateRangeInit = this.DataInicialPrevisaoEntregaPedido;

    this.DataInicialInicioViagemPlanejada = PropertyEntity({ text: "Data Início Viagem Planejada Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalInicioViagemPlanejada = PropertyEntity({ text: "Data Início Viagem Planejada Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialInicioViagemPlanejada.dateRangeLimit = this.DataFinalInicioViagemPlanejada;
    this.DataFinalInicioViagemPlanejada.dateRangeInit = this.DataInicialInicioViagemPlanejada;

    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true, thousands: "" }  });
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

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    //Campos COLOG
    this.FiltroDinamico = PropertyEntity({ maxlength: 200, val: ko.observable(""), def: "" });
    this.StatusEntrega = PropertyEntity({ val: ko.observable(EnumSituacaoEntrega.Todos), def: EnumSituacaoEntrega.Todos, getType: typesKnockout.select, options: ko.observable(EnumSituacaoEntrega.obterOpcoes()) });
    this.Canhoto = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(""), def: "" });
    
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            if (ValidarFiltrosObrigatorios())
                _gridNFes.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaNFes.Visible.visibleFade()) {
                _pesquisaNFes.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaNFes.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
    this.GerarPlanilhaCliente = PropertyEntity({ eventClick: GerarPlanilhaClienteClick, type: types.event, text: "Exportar", idGrid: guid() });
};

//*******EVENTOS*******

function LoadNFes() {
    var opcaoDownloadXML = { descricao: "Download XML", id: guid(), metodo: downloadXMLClick, icone: "", visibilidade: _CONFIGURACAO_TMS.PermitirDownloadDANFE };
    var opcaoDownloadPDF = { descricao: "Download PDF", id: guid(), metodo: downloadPDFClick, icone: "", visibilidade: _CONFIGURACAO_TMS.PermitirDownloadDANFE };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 20, opcoes: [opcaoDownloadXML, opcaoDownloadPDF] };

    _pesquisaNFes = new PesquisaNFes();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridNFes = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/NFes/Pesquisa", _pesquisaNFes, menuOpcoes, null, 10);
    _gridNFes.SetPermitirEdicaoColunas(true);
    _gridNFes.SetHabilitarScrollHorizontal(true, 200);

    ConfigurarCamposPorTipoSistema();
    
    _relatorioNotasEmitidas = new RelatorioGlobal("Relatorios/NFes/BuscarDadosRelatorio", _gridNFes, function () {
        _relatorioNotasEmitidas.loadRelatorio(function () {
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaNFes");
            KoBindings(_pesquisaNFes, "knockoutPesquisaNFes", false, _CRUDFiltrosRelatorio.Preview.id);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaNFes", false);

            new BuscarCargas(_pesquisaNFes.Carga);
            new BuscarTransportadores(_pesquisaNFes.Transportador);
            new BuscarClientes(_pesquisaNFes.Remetente);
            new BuscarClientes(_pesquisaNFes.Destinatario);
            new BuscarClientes(_pesquisaNFes.Expedidor);
            new BuscarLocalidades(_pesquisaNFes.Origem);
            new BuscarLocalidades(_pesquisaNFes.Destino);
            new BuscarEstados(_pesquisaNFes.EstadoOrigem);
            new BuscarEstados(_pesquisaNFes.EstadoDestino);
            new BuscarFilial(_pesquisaNFes.Filial);
            new BuscarGruposPessoas(_pesquisaNFes.GrupoPessoas);
            new BuscarRestricaoEntrega(_pesquisaNFes.Restricao);
            new BuscarNotaFiscal(_pesquisaNFes.NotaFiscal);
            new BuscarTiposdeCarga(_pesquisaNFes.TipoCarga);
            new BuscarTiposOperacao(_pesquisaNFes.TipoOperacao);
            new BuscarMotoristas(_pesquisaNFes.Motorista);

            if (_CONFIGURACAO_TMS.AtivarNovosFiltrosConsultaCarga) {
                _pesquisaNFes.Situacoes.visible(false);
                _pesquisaNFes.Situacoes.val(new Array());
                _pesquisaNFes.SituacaoCargaMercante.visible(true);
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaNFes);
   
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioNotasEmitidas.gerarRelatorio("Relatorios/NFes/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioNotasEmitidas.gerarRelatorio("Relatorios/NFes/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

function GerarPlanilhaClienteClick() {
    executarDownload("Relatorios/NFes/GerarPlanilhaCliente", RetornarObjetoPesquisa(_pesquisaNFes));
}

function ConfigurarCamposPorTipoSistema() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaNFes.Filial.visible(false);
        _pesquisaNFes.TipoLocalPrestacao.visible(false);
        _pesquisaNFes.SituacaoFatura.visible(false);
        _pesquisaNFes.Transportador.text("Empresa/Filial:");
    }
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaNFes.Transportador.visible(false);
    }
}

function ValidarFiltrosObrigatorios() {
    var verificarDatas = true;
    var verificarDatasPrevisaoCargaEntrega = false;

    if (_pesquisaNFes.Carga.val() != "") {
        _pesquisaNFes.DataInicialEmissao.required(false);
        _pesquisaNFes.DataInicialEmissao.text("Data Emissão Inicial: ");
        _pesquisaNFes.DataFinalEmissao.required(false);
        _pesquisaNFes.DataFinalEmissao.text("Data Emissão Final: ");

        verificarDatas = false;
    } else if ((_pesquisaNFes.DataPrevisaoCargaEntregaInicial.val() != "" && _pesquisaNFes.DataPrevisaoCargaEntregaFinal.val() != "") && (_pesquisaNFes.DataInicialEmissao.val() == "" || _pesquisaNFes.DataFinalEmissao.val() == "")) {
        _pesquisaNFes.DataInicialEmissao.required(false);
        _pesquisaNFes.DataInicialEmissao.text("Data Emissão Inicial: ");
        _pesquisaNFes.DataFinalEmissao.required(false);
        _pesquisaNFes.DataFinalEmissao.text("Data Emissão Final: ");

        verificarDatasPrevisaoCargaEntrega = true;
    } else {
        _pesquisaNFes.DataInicialEmissao.required(true);
        _pesquisaNFes.DataInicialEmissao.text("*Data Emissão Inicial: ");
        _pesquisaNFes.DataFinalEmissao.required(true);
        _pesquisaNFes.DataFinalEmissao.text("*Data Emissão Final: ");
 
    }

    var tudoCerto = true;

    var valido = ValidarCamposObrigatorios(_pesquisaNFes);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Filtros Obrigatórios", "Informe os filtros obrigatórios!");
        tudoCerto = false;
    }

    if (verificarDatas) {
        var totalDias = 0;

        if (!verificarDatasPrevisaoCargaEntrega)
            totalDias = Global.ObterDiasEntreDatas(_pesquisaNFes.DataInicialEmissao.val(), _pesquisaNFes.DataFinalEmissao.val());
        else
            totalDias = Global.ObterDiasEntreDatas(_pesquisaNFes.DataPrevisaoCargaEntregaInicial.val(), _pesquisaNFes.DataPrevisaoCargaEntregaFinal.val());

        if (totalDias > 120) {
            exibirMensagem(tipoMensagem.atencao, "Datas Inválidas", "A diferença das datas não pode ser maior que 120 dias.");
            tudoCerto = false;
        }
    }

    return tudoCerto;
}

function downloadXMLClick(e) {
    var data = { Codigo: e.Codigo };
    executarDownload("Relatorios/NFes/DownloadXML", data);
}

function downloadPDFClick(e) {
    var data = { Codigo: e.Codigo };
    executarDownload("Relatorios/NFes/DownloadDANFE", data);
}