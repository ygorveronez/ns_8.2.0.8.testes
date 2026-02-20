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
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _dadosFechamento;

var DadosFechamento = function () {
    this.DataInicio = PropertyEntity({ text: "* Data Inicial:", getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.DataFim = PropertyEntity({ text: "* Data Final:", getType: typesKnockout.date, enable: ko.observable(true), required: true });

    this.DataFim.dateRangeInit = this.DataInicio;
    this.DataInicio.dateRangeLimit = this.DataFim;
}


//*******EVENTOS*******
function LoadDadosFechamento() {
    //-- Knouckout
    _dadosFechamento = new DadosFechamento();
    KoBindings(_dadosFechamento, "knockoutDadosFechamento");
}




//*******MÉTODOS*******
function EditarDadosFechamento(dados) {
    PreencherObjetoKnout(_dadosFechamento, { Data: dados.DadosFechamento });
    _dadosFechamento.DataInicio.enable(false);
    _dadosFechamento.DataFim.enable(false);
}

function LimparDadosFechamento() {
    LimparCampos(_dadosFechamento);
    _dadosFechamento.DataInicio.enable(true);
    _dadosFechamento.DataFim.enable(true);
}