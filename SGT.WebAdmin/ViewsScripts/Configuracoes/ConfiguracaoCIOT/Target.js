var _configuracaoTarget = null;

var ConfiguracaoTarget = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.URLWebService = PropertyEntity({ text: "URL do Web Service:", maxlength: 150 });
    this.Usuario = PropertyEntity({ text: "Usuário:", maxlength: 50 });
    this.Senha = PropertyEntity({ text: "Senha:", maxlength: 50 });
    this.Token = PropertyEntity({ text: "Token:", maxlength: 50 });
    this.AssociarCartaoMotoristaTransportador = PropertyEntity({ text: "Associar cartão ao Motorista/Transportador", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ConsultarCartaoMotorista = PropertyEntity({ text: "Consultar cartão do Motorista ao integrar CIOT", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.UtilizarCiotTarget = PropertyEntity({ text: "Imprimir CIOT Target", val: ko.observable(false), def: false, enable: ko.observable(true) });
};

function LoadConfiguracaoTarget() {
    _configuracaoTarget = new ConfiguracaoTarget();
    KoBindings(_configuracaoTarget, "tabTarget");
}

function LimparCamposConfiguracaoTarget() {
    LimparCampos(_configuracaoTarget);
}