/// <reference path="../../Enumeradores/EnumStatusCTe.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCTe.js" />
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
/// <reference path="CIOT.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _encerramento;

var Encerramento = function () {
    
}

//*******EVENTOS*******

function LoadEncerramento() {
    _encerramento = new Encerramento();
    KoBindings(_encerramento, "knockoutEncerramento");
}

//*******MÉTODOS*******

function LimparCamposEncerramento() {
    LimparCampos(_encerramento);
}