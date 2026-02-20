//*******MAPEAMENTO KNOUCKOUT*******

var _SENAT;

var SENAT = function () {
    this.AliquotaSENAT = PropertyEntity({ maxlength: 5, getType: typesKnockout.decimal, text: "*Alíquota: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
};

//*******EVENTOS*******

function LoadSENAT() {

    _SENAT = new SENAT();
    KoBindings(_SENAT, "tabSENAT");

    _imposto.AliquotaSENAT = _SENAT.AliquotaSENAT;

}