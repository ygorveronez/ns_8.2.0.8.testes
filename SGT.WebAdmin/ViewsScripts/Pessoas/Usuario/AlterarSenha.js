/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="PermissaoUsuario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var AlterarSenha = function () {
    this.SenhaAtual = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.SenhaAtual.getRequiredFieldDescription(), required: true });
    this.NovaSenha = PropertyEntity({ text: Localization.Resources.Pessoas.Usuario.NovaSenha.getRequiredFieldDescription(), required: true });
    this.Confirmacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Confirmacao.getRequiredFieldDescription(), required: false, type: types.local });
    
    this.AlterarSenha = PropertyEntity({ eventClick: alterarSenhaClick, type: types.event, text: Localization.Resources.Pessoas.Usuario.AlterarSenha, visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadAlterarSenha() {

    var alterarSenha = new AlterarSenha();
    KoBindings(alterarSenha, "knockoutAlterarSenha");

}

function alterarSenhaClick(e, sender) {
    if (e.NovaSenha.val().length > 5) {
        if (e.NovaSenha.val() == e.Confirmacao.val()) {
            Salvar(e, "Usuario/AlterarSenha", function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pessoas.Usuario.SenhaAlteradoComSucesso);
                        e.Confirmacao.val("");
                        LimparCampos(e);
                    } else {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            }, sender, exibirCamposObrigatorio);
        } else {
            resetarTabs();
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.Usuario.ValidacaoSenha, Localization.Resources.Pessoas.Usuario.ConfirmacaoDeveSerIgualSenha);
        }
    } else {
        resetarTabs();
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.Usuario.TamanhoDeSenha, Localization.Resources.Pessoas.Usuario.SenhaDeveTerNoMinimoSeisCaracteres);
    }
}

//*******MÉTODOS*******

function exibirCamposObrigatorio() {
    resetarTabs();
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.GeralCampoObrigatorio, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}