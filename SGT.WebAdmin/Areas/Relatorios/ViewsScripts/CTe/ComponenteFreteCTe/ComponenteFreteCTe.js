
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridComponenteFreteCTe, _pesquisaComponenteFreteCTe, _CRUDRelatorio, _CRUDFiltrosRelatorio, _relatorioComponenteFreteCTe;

var _opcoesSituacaoCTe = [
    { text: "Autorizado", value: "A" },
    { text: "Pendente", value: "P" },
    { text: "Enviado", value: "E" },
    { text: "Rejeitado", value: "R" },
    { text: "Cancelado", value: "C" },
    { text: "Anulado", value: "Z" },
    { text: "Inutilizado", value: "I" },
    { text: "Denegado", value: "D" },
    { text: "Em Digitação", value: "S" },
    { text: "Em Cancelamento", value: "K" },
    { text: "Em Inutilização", value: "L" }
];


var PesquisaComponenteFreteCTe = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.DataInicialEmissao = PropertyEntity({ text: "Data Emissão Inicial: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Emissão Final: ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.DataInicialAutorizacao = PropertyEntity({ text: "Data Autorização Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalAutorizacao = PropertyEntity({ text: "Data Autorização Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialAutorizacao.dateRangeLimit = this.DataFinalAutorizacao;
    this.DataFinalAutorizacao.dateRangeInit = this.DataInicialAutorizacao;

    this.CTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.ModeloDocumento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo de Documento:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.ComponenteFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Componente de Frete:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: _opcoesSituacaoCTe, text: "Situação:", issue: 120, visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridComponenteFreteCTe.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaComponenteFreteCTe.Visible.visibleFade() == true) {
                _pesquisaComponenteFreteCTe.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaComponenteFreteCTe.Visible.visibleFade(true);
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

function LoadComponenteFreteCTe() {
    _pesquisaComponenteFreteCTe = new PesquisaComponenteFreteCTe();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridComponenteFreteCTe = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ComponenteFreteCTe/Pesquisa", _pesquisaComponenteFreteCTe);

    _gridComponenteFreteCTe.SetPermitirEdicaoColunas(true);
    _gridComponenteFreteCTe.SetQuantidadeLinhasPorPagina(10);

    _relatorioComponenteFreteCTe = new RelatorioGlobal("Relatorios/ComponenteFreteCTe/BuscarDadosRelatorio", _gridComponenteFreteCTe, function () {
        _relatorioComponenteFreteCTe.loadRelatorio(function () {
            KoBindings(_pesquisaComponenteFreteCTe, "knockoutPesquisaComponenteFreteCTe", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaComponenteFreteCTe", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaComponenteFreteCTe", false);

            new BuscarComponentesDeFrete(_pesquisaComponenteFreteCTe.ComponenteFrete);
            new BuscarModeloDocumentoFiscal(_pesquisaComponenteFreteCTe.ModeloDocumento, null, null, null, null, true);
            new BuscarTransportadores(_pesquisaComponenteFreteCTe.Empresa);
            new BuscarGruposPessoas(_pesquisaComponenteFreteCTe.GrupoPessoas);
            new BuscarCargas(_pesquisaComponenteFreteCTe.Carga);
            new BuscarCTes(_pesquisaComponenteFreteCTe.CTe);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaComponenteFreteCTe);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioComponenteFreteCTe.gerarRelatorio("Relatorios/ComponenteFreteCTe/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioComponenteFreteCTe.gerarRelatorio("Relatorios/ComponenteFreteCTe/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
