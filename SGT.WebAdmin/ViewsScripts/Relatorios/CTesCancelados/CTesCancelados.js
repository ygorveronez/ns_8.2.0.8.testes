/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorio;

var Relatorio = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", val: ko.observable(""), getType: typesKnockout.date });
    this.Transportador = PropertyEntity({ text: "Transportador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.GerarRelatorio = PropertyEntity({ eventClick: gerarRelatorioClick, type: types.event, text: "Gerar Relatório" });
}


//*******EVENTOS*******

function loadCTesCancelados() {
    _relatorio = new Relatorio();
    KoBindings(_relatorio, "knockoutRelatorio");

    new BuscarTransportadores(_relatorio.Transportador);
}

function gerarRelatorioClick() {
    executarDownload("CTesCancelados/DownloadRelatorio", {
        DataInicial: _relatorio.DataInicial.val(),
        DataFinal: _relatorio.DataFinal.val(),
        CodigoTransportador: _relatorio.Transportador.val() == "" ? "0" : _relatorio.Transportador.codEntity()
    });
}