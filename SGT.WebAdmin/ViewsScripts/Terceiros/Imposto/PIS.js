//*******MAPEAMENTO KNOUCKOUT*******

var _PIS;

var PIS = function () {
    this.AliquotaPIS = PropertyEntity({ maxlength: 6, getType: typesKnockout.decimal, text: "*Alíquota: ", val: ko.observable("0,0000"), def: "0,0000", required: true, configDecimal: { precision: 4, allowZero: true } });
};

//*******EVENTOS*******

function LoadPIS() {

    _PIS = new PIS();
    KoBindings(_PIS, "tabPIS");

    _imposto.AliquotaPIS = _PIS.AliquotaPIS;

}