/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="Pessoa.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="UsuarioAdicional.js" />

var _usuarioTerceiro;

//*******MAPEAMENTO KNOUCKOUT*******

var UsuarioTerceiro = function () {
    this.Usuario = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.UsuarioParaAcesso.getFieldDescription(), required: false, getType: typesKnockout.string, maxlength: 20, enable: ko.observable(false) });
    this.Senha = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.SenhaParaAcesso.getFieldDescription(), required: false, getType: typesKnockout.string, maxlength: 15, visible: ko.observable(true) });
    this.ConfirmaSenha = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ConfirmaSenhaParaAcesso.getFieldDescription(), required: false, getType: typesKnockout.string, maxlength: 15, visible: ko.observable(true) });

    this.Email = PropertyEntity({ eventClick: enviarUsuarioEmailClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.EnviarUsuarioPorEmail, visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadUsuarioTerceiro() {
    _usuarioTerceiro = new UsuarioTerceiro();
    KoBindings(_usuarioTerceiro, "knockoutCadastroUsuarioTerceiro");

    loadUsuarioAdicional();

    _usuarioTerceiro.Usuario.val(_pessoa.CNPJ.val());

    if (!_CONFIGURACAO_TMS.NaoUtilizarUsuarioTransportadorTerceiro)
        $("#liUsuarioTerceiro").show();
    else
        $("#liUsuarioTerceiro").hide();
}

function enviarUsuarioEmailClick() {
    Salvar(_pessoa, "Pessoa/EnviarEmailUsuarioTerceiro", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pessoas.Pessoa.EmailEnviadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function preencherUsuarioAutomatico() {
    if (_pessoa != null && _pessoa.CNPJ.val() != "" && _usuarioTerceiro != null && _usuarioTerceiro.Usuario.val() == "") {
        var login = _pessoa.CNPJ.val().replace(/[^0-9]+/g, '');
        var senha = login.substring(0, 5);
        _usuarioTerceiro.Usuario.val(login);
        _usuarioTerceiro.Senha.val(senha);
        _usuarioTerceiro.ConfirmaSenha.val(senha);
    }
}

//*******MÉTODOS*******

//function desabilitarOpcoesPercentuais() {
//    _usuarioTerceiro.PercentualAdiantamentoFretesTerceiro.visible(false);
//    _usuarioTerceiro.PercentualCobradoPadrao.visible(false);
//    _usuarioTerceiro.DescontoPadrao.visible(false);
//    _usuarioTerceiro.ObservacaoCTe.visible(false);
//}
