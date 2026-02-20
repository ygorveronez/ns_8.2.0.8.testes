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



//*******MAPEAMENTO KNOUCKOUT*******
var _resumoCheckList;

var ResumoCheckList = function () {
    this.Resumo = PropertyEntity({ visible: ko.observable(false) });

    this.Viagem = PropertyEntity({ text: "Viagem: ", val: ko.observable(''), def: '' });
}


//*******EVENTOS*******
function loadResumoCheckList() {
    _resumoCheckList = new ResumoCheckList();
    KoBindings(_resumoCheckList, "knockoutResumoCheckList");
}

//*******MÉTODOS*******
function PreencherResumo(arg) {
    _resumoCheckList.Resumo.visible(true);
    PreencherObjetoKnout(_resumoCheckList, { Data: arg.Resumo });
}