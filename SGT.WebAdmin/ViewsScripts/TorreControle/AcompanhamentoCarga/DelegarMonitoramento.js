var _modalDelegarMonitoramento;
var _cargasSelecionadas;

var ModalDelegarMonitoramento = function () {
    this.Analista = PropertyEntity({ type: types.entity, required: true, enable: ko.observable(true), codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Analista.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Confirmar = PropertyEntity({ type: types.event, eventClick: ConfirmarDelegarMonitoramentoClick, text: Localization.Resources.Cargas.ControleEntrega.ConfirmarAnalista, visible: ko.observable(true) });
}

function exibirModalDelegarMonitoramento(podeDelegarMonitoramentoParaUsuarios, codigoCarga) {
    _cargaSelecionada = codigoCarga;
    Global.abrirModal("divModalDelegarMonitoramento");

    _modalDelegarMonitoramento = new ModalDelegarMonitoramento();
    KoBindings(_modalDelegarMonitoramento, "knoutDelegarMonitoramento");

    if (!podeDelegarMonitoramentoParaUsuarios) {
        let usuarioLogado = buscarDadosUsuarioLogado();
        _modalDelegarMonitoramento.Analista.codEntity(usuarioLogado.Codigo);
        _modalDelegarMonitoramento.Analista.val(usuarioLogado.Nome);
        _modalDelegarMonitoramento.Analista.enable(false);
    }

    BuscarOperador(_modalDelegarMonitoramento.Analista);
}

function ConfirmarDelegarMonitoramentoClick() {
    delegarMonitoramentoParaUsuario(_cargaSelecionada, _modalDelegarMonitoramento.Analista.codEntity());
    Global.fecharModal("divModalDelegarMonitoramento");
}

function delegarMonitoramentoParaUsuario(cargaSelecionada, codigoUsuario) {
    let body = {
        CodigoAnalista: codigoUsuario,
        CodigoCarga: JSON.stringify(cargaSelecionada)
    }

    executarReST("AcompanhamentoCarga/DefinirAnalistaResponsavelMonitoramentoCarga", body, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.AnalistaAtualizado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function buscarDadosUsuarioLogado() {
    return new Promise(resolve => {
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            resolve(retorno.Data);
        });
    })
}