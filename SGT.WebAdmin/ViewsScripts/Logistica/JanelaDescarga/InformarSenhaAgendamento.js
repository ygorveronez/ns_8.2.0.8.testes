var _informarSenhaAgendamento;

var InformarSenhaAgendamento = function () {
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.Senha = PropertyEntity({ text: "Senha: ", getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });

    this.Senha.val.subscribe(function (valor) {
        valor = valor.replace(' ', '');
        _informarSenhaAgendamento.Senha.val(valor);
    });

    this.Salvar = PropertyEntity({ eventClick: salvarSenhaClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

function loadInformarSenhaAgendamento() {
    _informarSenhaAgendamento = new InformarSenhaAgendamento();
    KoBindings(_informarSenhaAgendamento, "knockoutInformarSenhaAgendamento");
}

function exibirModalInformarSenhaAgendamento(codigo) {
    _informarSenhaAgendamento.CodigoCarga.val(codigo);
    Global.abrirModal('divModalInformarSenhaAgendamento');

    $("#divModalInformarSenhaAgendamento").on('hidden.bs.modal', function () {
        LimparCampos(_informarSenhaAgendamento);
    });
}

function salvarSenhaClick() {
    executarReST("AgendamentoColeta/SalvarSenha", { Codigo: _informarSenhaAgendamento.CodigoCarga.val(), Senha: _informarSenhaAgendamento.Senha.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "A Senha do Agendamento foi alterada!");
                Global.fecharModal('divModalInformarSenhaAgendamento');
                _tabelaDescarregamento.Load();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}