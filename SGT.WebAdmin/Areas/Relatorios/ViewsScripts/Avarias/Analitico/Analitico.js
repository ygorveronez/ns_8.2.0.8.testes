/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoAvaria.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEtapaLote.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAvaria, _pesquisaAvaria, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioAvaria;

var _situacaoAvaria = [
    { text: "Em Criação", value: EnumSituacaoAvaria.EmCriacao },
    { text: "Ag Aprovação", value: EnumSituacaoAvaria.AgAprovacao },
    { text: "Ag Lote", value: EnumSituacaoAvaria.AgLote },
    { text: "Ag Integração", value: EnumSituacaoAvaria.AgIntegracao },
    { text: "Lote Gerado", value: EnumSituacaoAvaria.LoteGerado },
    { text: "Sem Regra Aprovação", value: EnumSituacaoAvaria.SemRegraAprovacao },
    { text: "Sem Regra Lote", value: EnumSituacaoAvaria.SemRegraLote },
    { text: "Rejeitada Autorização", value: EnumSituacaoAvaria.RejeitadaAutorizacao },
    { text: "Finalizada", value: EnumSituacaoAvaria.Finalizada }
];

var _intNumeroConfig = {
    precision: 0,
    allowZero: false,
    allowNegative: false,
    thousands: ""
};


var PesquisaAvaria = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroAvaria = PropertyEntity({ text: "Número da Avaria:", val: ko.observable(""), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: _intNumeroConfig });
    this.Solicitante = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Solicitante:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.SituacaoAvaria = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable(EnumSituacaoAvaria.Todas), options: _situacaoAvaria, def: ko.observable([]), text: "Situação: " });

    this.Etapa = PropertyEntity({ getType: typesKnockout.select, val: ko.observable(EnumEtapaLote.Todas), def: EnumEtapaLote.Todas, options: EnumEtapaLote.obterOpcoesPesquisa(), text: "Etapa: " });
    
    this.DataSolicitacaoInicial = PropertyEntity({ text: "Data Avaria Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataSolicitacaoFinal = PropertyEntity({ text: "Data Avaria Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataSolicitacaoInicial.dateRangeLimit = this.DataSolicitacaoFinal;
    this.DataSolicitacaoFinal.dateRangeInit = this.DataSolicitacaoInicial;

    this.DataGeracaoLoteInicial = PropertyEntity({ text: "Data Geração Lote Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataGeracaoLoteFinal = PropertyEntity({ text: "Data Geração Lote Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataGeracaoLoteInicial.dateRangeLimit = this.DataGeracaoLoteFinal;
    this.DataGeracaoLoteFinal.dateRangeInit = this.DataGeracaoLoteInicial;

    this.DataIntegracaoLoteInicial = PropertyEntity({ text: "Data Integração Lote Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataIntegracaoLoteFinal = PropertyEntity({ text: "Data Integração Lote Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataIntegracaoLoteInicial.dateRangeLimit = this.DataIntegracaoLoteFinal;
    this.DataIntegracaoLoteFinal.dateRangeInit = this.DataIntegracaoLoteInicial;
    
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridAvaria.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaAvaria.Visible.visibleFade()) {
                _pesquisaAvaria.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaAvaria.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******

function LoadAvaria() {
    _pesquisaAvaria = new PesquisaAvaria();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridAvaria = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Analitico/Pesquisa", _pesquisaAvaria);

    _gridAvaria.SetPermitirEdicaoColunas(true);
    _gridAvaria.SetQuantidadeLinhasPorPagina(10);

    _relatorioAvaria = new RelatorioGlobal("Relatorios/Analitico/BuscarDadosRelatorio", _gridAvaria, function () {
        _relatorioAvaria.loadRelatorio(function () {
            KoBindings(_pesquisaAvaria, "knockoutPesquisaAvaria", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaAvaria", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaAvaria", false);

            //BUSCA DOS CAMPOS DE PESQUISA E OUTROS BINDINGS
            new BuscarFuncionario(_pesquisaAvaria.Solicitante);
            new BuscarTransportadores(_pesquisaAvaria.Transportador);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaAvaria);

    controlarCampos();
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioAvaria.gerarRelatorio("Relatorios/Analitico/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioAvaria.gerarRelatorio("Relatorios/Analitico/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

//*******MÉTODOS*******

function controlarCampos() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Terceiros) {
        _pesquisaAvaria.Solicitante.visible(false);
    } else {
        _pesquisaAvaria.Solicitante.visible(true);
    }
}