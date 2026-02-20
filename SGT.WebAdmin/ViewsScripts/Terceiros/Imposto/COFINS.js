//*******MAPEAMENTO KNOUCKOUT*******

var _COFINS;

var COFINS = function () {
    this.AliquotaCOFINS = PropertyEntity({ maxlength: 6, getType: typesKnockout.decimal, text: "*Alíquota: ", val: ko.observable("0,0000"), def: "0,0000", required: true, configDecimal: { precision: 4, allowZero: true } });
};

//*******EVENTOS*******

function LoadCOFINS() {

    _COFINS = new COFINS();
    KoBindings(_COFINS, "tabCOFINS");

    _imposto.AliquotaCOFINS = _COFINS.AliquotaCOFINS;

}