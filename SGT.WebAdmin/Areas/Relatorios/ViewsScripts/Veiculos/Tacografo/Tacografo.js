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
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioTacografo, _gridTacografo, _pesquisaTacografo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _Situacao = [
    { value: 1, text: "Entregue" },
    { value: 2, text: "EntreRecebidogue" },
    { value: 3, text: "Perdido" },
    { value: 4, text: "Extraviado" }
];

var _ExcessoVelocidade = [
    { text: "Sim", value: 1 },
    { text: "Não", value: 2 },
    { text: "Todos", value: 0 }
];

var PesquisaTacografo = function () {
    this.Veiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });

    this.DataInicialRepasse = PropertyEntity({ text: "Data Repasse Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalRepasse = PropertyEntity({ text: "Data Repasse Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicialRetorno = PropertyEntity({ text: "Data Retorno Inicial: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFinalRetorno = PropertyEntity({ text: "Data Retorno Final: ", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });

    this.Situacao = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: _Situacao, text: "Situação:", visible: ko.observable(true) });
    this.ExcessoVelocidade = PropertyEntity({ text: "Excesso de Velocidade?:", options: _ExcessoVelocidade, val: ko.observable("0"), def: "0", visible: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridTacografo.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioTacografo() {

    _pesquisaTacografo = new PesquisaTacografo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridTacografo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Tacografo/Pesquisa", _pesquisaTacografo, null, null, 10);
    _gridTacografo.SetPermitirEdicaoColunas(true);

    _relatorioTacografo = new RelatorioGlobal("Relatorios/Tacografo/BuscarDadosRelatorio", _gridTacografo, function () {
        _relatorioTacografo.loadRelatorio(function () {
            KoBindings(_pesquisaTacografo, "knockoutPesquisaTacografo");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaTacografo");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaTacografo");

            new BuscarVeiculos(_pesquisaTacografo.Veiculo);
            new BuscarFuncionario(_pesquisaTacografo.Motorista);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaTacografo);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioTacografo.gerarRelatorio("Relatorios/Tacografo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioTacografo.gerarRelatorio("Relatorios/Tacografo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}