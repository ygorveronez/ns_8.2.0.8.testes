/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />

//********MAPEAMENTO KNOCKOUT********

var _relatorioGrupoPessoas, _gridGrupoPessoas, _pesquisaGrupoPessoas, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _tipoGrupoPessoas = [
    { text: "Ambos", value: EnumTipoGrupoPessoas.Ambos },
    { text: "Clientes", value: EnumTipoGrupoPessoas.Clientes },
    { text: "Fornecedores", value: EnumTipoGrupoPessoas.Fornecedores }
];

var PesquisaGrupoPessoas = function () {
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.TipoGrupo = PropertyEntity({ getType: typesKnockout.selectMultiple, text: "Tipo do grupo de pessoas:", val: ko.observable(new Array()), def: new Array(), options: _tipoGrupoPessoas });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(0), def: 0, options: _statusPesquisa });
    this.Bloqueado = PropertyEntity({ text: "Bloqueado:", val: ko.observable(9), def: 9, options: EnumSimNaoPesquisa.obterOpcoesPesquisa() });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridGrupoPessoas.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    /* BUSCA AVANÇADA
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaGrupoPessoas.Visible.visibleFade() === true) {
                _pesquisaGrupoPessoas.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaGrupoPessoas.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });*/
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel" });
}

//*********EVENTOS**********

function LoadGrupoPessoas() {
    _pesquisaGrupoPessoas = new PesquisaGrupoPessoas();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridGrupoPessoas = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/GrupoPessoas/Pesquisa", _pesquisaGrupoPessoas, null, null, 10);
    _gridGrupoPessoas.SetPermitirEdicaoColunas(true);

    _relatorioGrupoPessoas = new RelatorioGlobal("Relatorios/GrupoPessoas/BuscarDadosRelatorio", _gridGrupoPessoas, function () {
        _relatorioGrupoPessoas.loadRelatorio(function () {
            KoBindings(_pesquisaGrupoPessoas, "knockoutPesquisaGrupoPessoas", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaGrupoPessoas", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaGrupoPessoas", false);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaGrupoPessoas);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioGrupoPessoas.gerarRelatorio("Relatorios/GrupoPessoas/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioGrupoPessoas.gerarRelatorio("Relatorios/GrupoPessoas/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}