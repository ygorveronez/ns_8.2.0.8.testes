//*******MAPEAMENTO KNOUCKOUT*******

var _SEST;

var SEST = function () {
    this.AliquotaSEST = PropertyEntity({ maxlength: 5, getType: typesKnockout.decimal, text: "*Alíquota: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
};

//*******EVENTOS*******

function LoadSEST() {

    _SEST = new SEST();
    KoBindings(_SEST, "tabSEST");

    _imposto.AliquotaSEST = _SEST.AliquotaSEST;

}