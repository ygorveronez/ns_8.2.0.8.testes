//*******MAPEAMENTO KNOUCKOUT*******

var _bloqueioPessoa;

var BloqueioPessoa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Motivo.getRequiredFieldDescription(), issue: 0, required: true, maxLength: 400, enable: ko.observable(true) });

    this.Bloquear = PropertyEntity({ eventClick: BloquearPessoaClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.Bloquear, visible: ko.observable(true), icon: 'fa fa-ban', enable: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaBloqueioPessoaClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.Fechar, visible: ko.observable(true), icon: 'fa fa-window-close', enable: ko.observable(true) });
};

//*******EVENTOS*******

function LoadBloqueioPessoa() {

    _bloqueioPessoa = new BloqueioPessoa();
    KoBindings(_bloqueioPessoa, "knockoutBloqueioPessoa");

}

function AbrirTelaBloqueioPessoaClick() {
    LimparCamposBloqueioPessoa();
    _bloqueioPessoa.Codigo.val(_pessoa.Codigo.val());
    Global.abrirModal("knockoutBloqueioPessoa");
}

function FecharTelaBloqueioPessoaClick() {
    LimparCamposBloqueioPessoa();
    Global.fecharModal("knockoutBloqueioPessoa");
}

function BloquearPessoaClick() {
    Salvar(_bloqueioPessoa, "Pessoa/Bloquear", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pessoas.Pessoa.BloqueadoComSucesso);

                $("#divMotivoBloqueioPessoa").removeClass("d-none");
                $("#txtMotivoBloqueio").text(_bloqueioPessoa.Motivo.val());
                _pessoa.Bloqueado.val(true);
                _pessoa.MotivoBloqueio.val(_bloqueioPessoa.Motivo.val());
                _pessoaBotoes.Bloquear.visible(false);
                _pessoaBotoes.Desbloquear.visible(true);

                FecharTelaBloqueioPessoaClick();
                Global.ResetarAbas();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function DesbloquearPessoaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.DesejaRealmenteDesbloquearEstaPessoa, function () {
        executarReST("Pessoa/Desbloquear", { Codigo: _pessoa.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pessoas.Pessoa.DesbloqueadoComSucesso);

                    $("#divMotivoBloqueioPessoa").addClass("d-none");
                    _pessoa.Bloqueado.val(false);
                    _pessoa.MotivoBloqueio.val("");
                    _pessoaBotoes.Bloquear.visible(true);
                    _pessoaBotoes.Desbloquear.visible(false);

                    FecharTelaBloqueioPessoaClick();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function LimparCamposBloqueioPessoa() {
    LimparCampos(_bloqueioPessoa);
}