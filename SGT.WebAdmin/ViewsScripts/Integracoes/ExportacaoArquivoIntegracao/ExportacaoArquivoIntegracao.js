/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _exportacaoArquivoIntegracao;

var ExportacaoArquivoIntegracao = function () {
    this.DataEmissao = PropertyEntity({ text: "Data de Emissão:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataAberturaCIOT = PropertyEntity({ text: "Data de abertura do CIOT:", getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.Contrato = PropertyEntity({ eventClick: gerarArquivoContratosClick, type: types.event, text: "Gerar arquivo dos contratos", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadExportacaoArquivoIntegracao() {
    _exportacaoArquivoIntegracao = new ExportacaoArquivoIntegracao();
    KoBindings(_exportacaoArquivoIntegracao, "knockoutExportacaoArquivoIntegracao");
}

function gerarArquivoContratosClick() {
    executarDownload("ExportacaoArquivoIntegracao/GerarArquivoContratos", { DataEmissao: _exportacaoArquivoIntegracao.DataEmissao.val(), DataAberturaCIOT: _exportacaoArquivoIntegracao.DataAberturaCIOT.val() });
}