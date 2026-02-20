var _configuracaoRodocred = null;

var ConfiguracaoRodocred = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.URL = PropertyEntity({ text: "*URL", maxlength: 250, required: ko.observable(false) });
    this.Login = PropertyEntity({ text: "*Login Integração:", maxlength: 250, required: ko.observable(false) });
    this.ChaveAutenticacao = PropertyEntity({ text: "*Chave de Autenticação:", maxlength: 250, required: ko.observable(false) });
    this.IDCliente = PropertyEntity({ text: "*ID Cliente:", maxlength: 250, required: ko.observable(false) });
};

function LoadConfiguracaoRodocred() {
    _configuracaoRodocred = new ConfiguracaoRodocred();
    KoBindings(_configuracaoRodocred, "tabRodocred");
}

function LimparCamposConfiguracaoRodocred() {
    LimparCampos(_configuracaoRodocred);
}

function ExibirCamposObrigatorios() {
    exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Informe os campos obrigatórios!");
}

