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
/// <reference path="../../../../../ViewsScripts/Consultas/OrdemServico.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CheckListTipo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumEntradaSaida.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioGuaritaCheckList, _gridGuaritaCheckList, _pesquisaGuaritaCheckList, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _pesquisaTipoEntradaSaida = [
    { text: "Todos", value: EnumEntradaSaida.Todos },
    { text: "Entrada", value: EnumEntradaSaida.Entrada },
    { text: "Saída", value: EnumEntradaSaida.Saida }
];

var PesquisaGuaritaCheckList = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Tipo = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Todos), options: _pesquisaTipoEntradaSaida, def: EnumEntradaSaida.Todos, text: "Tipo: " });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.OrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ordem de Serviço:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.TipoCheck = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Check List: ", idBtnSearch: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridGuaritaCheckList.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
}

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
}

//*******EVENTOS*******


function loadRelatorioGuaritaCheckList() {

    _pesquisaGuaritaCheckList = new PesquisaGuaritaCheckList();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridGuaritaCheckList = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/GuaritaCheckList/Pesquisa", _pesquisaGuaritaCheckList, null, null, 10);
    _gridGuaritaCheckList.SetPermitirEdicaoColunas(true);

    _relatorioGuaritaCheckList = new RelatorioGlobal("Relatorios/GuaritaCheckList/BuscarDadosRelatorio", _gridGuaritaCheckList, function () {
        _relatorioGuaritaCheckList.loadRelatorio(function () {
            KoBindings(_pesquisaGuaritaCheckList, "knockoutPesquisaGuaritaCheckList");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaGuaritaCheckList");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaGuaritaCheckList");

            new BuscarCargas(_pesquisaGuaritaCheckList.Carga);
            new BuscarVeiculos(_pesquisaGuaritaCheckList.Veiculo);
            new BuscarOrdemServico(_pesquisaGuaritaCheckList.OrdemServico);
            new BuscarCheckListTipo(_pesquisaGuaritaCheckList.TipoCheck);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiNFe) {
                _pesquisaGuaritaCheckList.Carga.visible(false);
                _pesquisaGuaritaCheckList.OrdemServico.visible(false);
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaGuaritaCheckList);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioGuaritaCheckList.gerarRelatorio("Relatorios/GuaritaCheckList/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioGuaritaCheckList.gerarRelatorio("Relatorios/GuaritaCheckList/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}