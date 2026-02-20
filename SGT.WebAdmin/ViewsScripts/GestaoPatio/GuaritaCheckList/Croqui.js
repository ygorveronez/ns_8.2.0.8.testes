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

var _croquis;

var Croquis = function () {
    this.Categorias = ko.observableArray([]);
}


//*******EVENTOS*******
function LoadCroquis() {
    _croquis = new Croquis();
    KoBindings(_croquis, "knockoutCroquis");
}

//*******METODOS*******
function EditarCroquis(data) {
}

function LimparCroquis() {
    _croquis.Categorias([]);
}

function GetSetCroquis() {
    if (arguments.length == 0)
        return GetCroquis();
    else
        SetCroquis(arguments[0]);
}


function GetCroquis() {
    return JSON.stringify(_croquis.Categorias());
}
function SetCroquis(data) {
    _croquis.Categorias(data.slice());
}