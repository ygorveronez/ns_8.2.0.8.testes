var _configuracaoTruckPad = null;

var ConfiguracaoTruckPad = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.URLTruckPadToken = PropertyEntity({ text: "URL Token:", maxlength: 400 });
    this.URLTruckPad = PropertyEntity({ text: "URL:", maxlength: 400 });
    this.UsuarioTruckPad = PropertyEntity({ text: "Usuário:", maxlength: 150 });
    this.SenhaTruckPad = PropertyEntity({ text: "Senha:", maxlength: 150 });
    this.OfficeID = PropertyEntity({ text: "Office ID:", maxlength: 150 });
};

function LoadConfiguracaoTruckPad() {
    _configuracaoTruckPad = new ConfiguracaoTruckPad();
    KoBindings(_configuracaoTruckPad, "tabTruckPad");
}

function LimparCamposConfiguracaoTruckPad() {
    LimparCampos(_configuracaoTruckPad);
}