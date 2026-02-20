/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridValorDescarga, _pesquisaValorDescarga, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioValorDescarga;

var PesquisaValorDescarga = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade:",issue: 16, idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:",issue: 58, idBtnSearch: guid(), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:",issue: 52, idBtnSearch: guid(),  visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:",issue: 899, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridValorDescarga.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaValorDescarga.Visible.visibleFade() == true) {
                _pesquisaValorDescarga.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaValorDescarga.Visible.visibleFade(true);
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

function LoadValorDescarga() {
    _pesquisaValorDescarga = new PesquisaValorDescarga();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridValorDescarga = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/ValorDescarga/Pesquisa", _pesquisaValorDescarga);

    _gridValorDescarga.SetPermitirEdicaoColunas(true);
    _gridValorDescarga.SetQuantidadeLinhasPorPagina(20);

    _relatorioValorDescarga = new RelatorioGlobal("Relatorios/ValorDescarga/BuscarDadosRelatorio", _gridValorDescarga, function () {
        _relatorioValorDescarga.loadRelatorio(function () {
            KoBindings(_pesquisaValorDescarga, "knockoutPesquisaValorDescarga", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaValorDescarga", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaValorDescarga", false);

            new BuscarClientes(_pesquisaValorDescarga.Pessoa);
            new BuscarGruposPessoas(_pesquisaValorDescarga.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
            new BuscarLocalidadesBrasil(_pesquisaValorDescarga.Localidade);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaValorDescarga);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioValorDescarga.gerarRelatorio("Relatorios/ValorDescarga/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioValorDescarga.gerarRelatorio("Relatorios/ValorDescarga/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
