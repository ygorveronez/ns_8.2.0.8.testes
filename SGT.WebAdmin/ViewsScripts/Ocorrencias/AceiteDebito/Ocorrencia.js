/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _ocorrencia;

var Ocorrencia = function () {
    this.Numero = PropertyEntity({ text: "Número:", val: ko.observable("") });
    this.DataOcorrencia = PropertyEntity({ text: "Data Ocorrência:", val: ko.observable("") });
    this.TipoOcorrencia = PropertyEntity({ text: "Tipo da Ocorrência:", val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable("") });
}



//*******EVENTOS*******
function loadOcorrencia() {
    _ocorrencia = new Ocorrencia();
    KoBindings(_ocorrencia, "knockoutOcorrencia");
}




//*******MÉTODOS*******
function CarregarDadosOcorrencia(arg) {
    PreencherObjetoKnout(_ocorrencia, { Data: arg.DetalhesOcorrencia });
}