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
/// <reference path="../../../../../ViewsScripts/Consultas/Bem.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/GrupoProdutoTMS.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Almoxarifado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioBens, _gridBens, _pesquisaBens, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaBens = function () {
    this.DataAquisicaoInicial = PropertyEntity({ text: "Data Aquisição Inicial: ", getType: typesKnockout.date });
    this.DataAquisicaoFinal = PropertyEntity({ text: "Data Aquisição Final: ", getType: typesKnockout.date });
    this.DataAlocadoInicial = PropertyEntity({ text: "Data Alocação Inicial: ", getType: typesKnockout.date });
    this.DataAlocadoFinal = PropertyEntity({ text: "Data Alocação Final: ", getType: typesKnockout.date });

    this.Bem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Patrimônio: ", idBtnSearch: guid(), visible: true });
    this.GrupoProduto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Produto: ", idBtnSearch: guid(), visible: true });
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Almoxarifado: ", idBtnSearch: guid(), visible: true });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado: ", idBtnSearch: guid(), visible: true });
    this.FuncionarioAlocado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Alocação:", idBtnSearch: guid(), visible: true });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MotivoDefeito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo do Defeito:", idBtnSearch: guid() });
    this.DataEntrega = PropertyEntity({ text: "Data Entrega: ", getType: typesKnockout.date });
    this.DataRetorno = PropertyEntity({ text: "Data Retorno: ", getType: typesKnockout.date });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridBens.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioBens() {

    _pesquisaBens = new PesquisaBens();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridBens = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Bem/Pesquisa", _pesquisaBens, null, null, 10);
    _gridBens.SetPermitirEdicaoColunas(true);

    _relatorioBens = new RelatorioGlobal("Relatorios/Bem/BuscarDadosRelatorio", _gridBens, function () {
        _relatorioBens.loadRelatorio(function () {
            KoBindings(_pesquisaBens, "knockoutPesquisaBens");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaBens");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaBens");

            new BuscarBens(_pesquisaBens.Bem);
            new BuscarGruposProdutosTMS(_pesquisaBens.GrupoProduto, null);
            new BuscarAlmoxarifado(_pesquisaBens.Almoxarifado);
            new BuscarCentroResultado(_pesquisaBens.CentroResultado);
            new BuscarFuncionario(_pesquisaBens.FuncionarioAlocado);
            new BuscarMotivoDefeito(_pesquisaBens.MotivoDefeito);
            new BuscarClientes(_pesquisaBens.Pessoa);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaBens);

}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioBens.gerarRelatorio("Relatorios/Bem/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioBens.gerarRelatorio("Relatorios/Bem/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}