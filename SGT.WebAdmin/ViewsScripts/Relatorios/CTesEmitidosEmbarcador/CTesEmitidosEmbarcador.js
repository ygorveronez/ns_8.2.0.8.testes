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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorio;

var Relatorio = function () {
    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão Inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data Emissão Final: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataAutorizacaoInicial = PropertyEntity({ text: "Data Autorização Inicial: ", val: ko.observable(""), getType: typesKnockout.date });
    this.DataAutorizacaoFinal = PropertyEntity({ text: "Data Autorização Final: ", val: ko.observable(""), getType: typesKnockout.date });
    this.Transportador = PropertyEntity({ text: "Transportador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Embarcador = PropertyEntity({ text: "Embarcador:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TodosRaizCNPJEmbarcador = PropertyEntity({ text: "Todos da Raiz do CNPJ do Embarcador", getType: typesKnockout.bool, val: ko.observable(false), def: false, required: false });
    this.GerarRelatorio = PropertyEntity({ eventClick: gerarRelatorioClick, type: types.event, text: "Gerar Relatório" });
}


//*******EVENTOS*******

function loadCTesEmitidosEmbarcador() {
    _relatorio = new Relatorio();
    KoBindings(_relatorio, "knockoutRelatorio");

    new BuscarTransportadores(_relatorio.Transportador);
    new BuscarClientes(_relatorio.Embarcador);
}

function gerarRelatorioClick() {
    executarDownload("CTesEmitidosEmbarcador/DownloadRelatorio", {
        DataEmissaoInicial: _relatorio.DataEmissaoInicial.val(),
        DataEmissaoFinal: _relatorio.DataEmissaoFinal.val(),
        DataAutorizacaoInicial: _relatorio.DataAutorizacaoInicial.val(),
        DataAutorizacaoFinal: _relatorio.DataAutorizacaoFinal.val(),
        CodigoTransportador: _relatorio.Transportador.val() == "" ? "0" : _relatorio.Transportador.codEntity(),
        CodigoEmbarcador: _relatorio.Embarcador.val() == "" ? "0" : _relatorio.Embarcador.codEntity(),
        TodosRaizCNPJEmbarcador: _relatorio.TodosRaizCNPJEmbarcador.val()
    });
}