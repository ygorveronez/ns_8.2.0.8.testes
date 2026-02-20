var _configuracaoAmbipar = null;

var ConfiguracaoAmbipar = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.URL = PropertyEntity({ text: "*URL", maxlength: 250, required: ko.observable(false) });
    this.Usuario = PropertyEntity({ text: "*Usuário:", maxlength: 250, required: ko.observable(false) });
    this.Senha = PropertyEntity({ text: "*Senha:", maxlength: 250, required: ko.observable(false) });
};

function LoadConfiguracaoAmbipar() {
    _configuracaoAmbipar = new ConfiguracaoAmbipar();
    KoBindings(_configuracaoAmbipar, "tabAmbipar");
}

function LimparCamposConfiguracaoAmbipar() {
    LimparCampos(_configuracaoAmbipar);
}

function ExibirCamposObrigatorios() {
    exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Informe os campos obrigatórios!");
}

