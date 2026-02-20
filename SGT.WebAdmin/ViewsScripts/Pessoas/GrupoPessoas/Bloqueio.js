//*******MAPEAMENTO KNOUCKOUT*******

var _bloqueioGrupoPessoas;

var BloqueioGrupoPessoas = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.Motivo.getRequiredFieldDescription(), issue: 0, required: true, maxLength: 400, enable: ko.observable(true) });

    this.Bloquear = PropertyEntity({ eventClick: BloquearGrupoPessoasClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Bloquear, visible: ko.observable(true), icon: 'fa fa-ban', enable: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaBloqueioGrupoPessoasClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Fechar, visible: ko.observable(true), icon: 'fa fa-window-close', enable: ko.observable(true) });
};

//*******EVENTOS*******

function LoadBloqueioGrupoPessoas() {

    _bloqueioGrupoPessoas = new BloqueioGrupoPessoas();
    KoBindings(_bloqueioGrupoPessoas, "knockoutBloqueioGrupoPessoas");

}

function AbrirTelaBloqueioGrupoPessoasClick() {
    LimparCamposBloqueioGrupoPessoas();
    _bloqueioGrupoPessoas.Codigo.val(_grupoPessoas.Codigo.val());
    Global.abrirModal("knockoutBloqueioGrupoPessoas");
}

function FecharTelaBloqueioGrupoPessoasClick() {
    LimparCamposBloqueioGrupoPessoas();
    Global.fecharModal("knockoutBloqueioGrupoPessoas");
}

function BloquearGrupoPessoasClick() {
    Salvar(_bloqueioGrupoPessoas, "GrupoPessoas/Bloquear", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.GrupoPessoas.Sucesso, Localization.Resources.Pessoas.GrupoPessoas.BloqueadoComSucesso);

                $("#divMotivoBloqueioGrupoPessoas").removeClass("d-none");
                $("#txtMotivoBloqueio").text(_bloqueioGrupoPessoas.Motivo.val());
                _grupoPessoas.Bloqueado.val(true);
                _grupoPessoas.MotivoBloqueio.val(_bloqueioGrupoPessoas.Motivo.val());
                _grupoPessoasCRUD.Bloquear.visible(false);
                _grupoPessoasCRUD.Desbloquear.visible(true);

                FecharTelaBloqueioGrupoPessoasClick();
                Global.ResetarAbas();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, arg.Msg);
        }
    });
}

function DesbloquearGrupoPessoasClick() {
    exibirConfirmacao(Localization.Resources.Pessoas.GrupoPessoas.Atencao, Localization.Resources.Pessoas.GrupoPessoas.DesejaDesbloquearGrupoPessoas, function () {
        executarReST("GrupoPessoas/Desbloquear", { Codigo: _grupoPessoas.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Pessoas.GrupoPessoas.Sucesso, Localization.Resources.Pessoas.GrupoPessoas.DesbloqueadoComSucesso);

                    $("#divMotivoBloqueioGrupoPessoas").addClass("d-none");
                    _grupoPessoas.Bloqueado.val(false);
                    _grupoPessoas.MotivoBloqueio.val("");
                    _grupoPessoasCRUD.Bloquear.visible(true);
                    _grupoPessoasCRUD.Desbloquear.visible(false);

                    FecharTelaBloqueioGrupoPessoasClick();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.GrupoPessoas.Falha, arg.Msg);
            }
        });
    });
}

function LimparCamposBloqueioGrupoPessoas() {
    LimparCampos(_bloqueioGrupoPessoas);
}