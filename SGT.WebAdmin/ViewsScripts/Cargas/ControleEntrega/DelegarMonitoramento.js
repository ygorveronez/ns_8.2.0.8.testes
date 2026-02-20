var _modalDelegarMonitoramento;
var _cargasSelecionadas;

var ModalDelegarMonitoramento = function () {
    this.Analista = PropertyEntity({ type: types.entity, required: true, enable: ko.observable(true), codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Analista.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Confirmar = PropertyEntity({ type: types.event, eventClick: ConfirmarDelegarMonitoramentoClick, text: Localization.Resources.Cargas.ControleEntrega.ConfirmarAnalista, visible: ko.observable(true) });
}

async function exibirModalDelegarMonitoramento(podeDelegarMonitoramentoParaUsuarios, cargasSelecionadas) {
    _cargasSelecionadas = cargasSelecionadas;
    if (podeDelegarMonitoramentoParaUsuarios) {
        Global.abrirModal("divModalDelegarMonitoramento");

        _modalDelegarMonitoramento = new ModalDelegarMonitoramento();
        KoBindings(_modalDelegarMonitoramento, "knoutDelegarMonitoramento");

    } else {
        Global.abrirModal("divModalDelegarMonitoramentoAnalista");

        _modalDelegarMonitoramento = new ModalDelegarMonitoramento();
        KoBindings(_modalDelegarMonitoramento, "knoutDelegarMonitoramentoAnalista");
    }

    if (!podeDelegarMonitoramentoParaUsuarios) {
        let usuarioLogado = await buscarDadosUsuarioLogado();
        _modalDelegarMonitoramento.Analista.codEntity(usuarioLogado.Codigo);
        _modalDelegarMonitoramento.Analista.val(usuarioLogado.Nome);
        _modalDelegarMonitoramento.Analista.enable(false);
    }

    new BuscarOperador(_modalDelegarMonitoramento.Analista);
}

async function ConfirmarDelegarMonitoramentoClick() {
    await delegarMonitoramentoParaUsuario(_cargasSelecionadas, _modalDelegarMonitoramento.Analista.codEntity());
    Global.fecharModal("divModalDelegarMonitoramento");
    Global.fecharModal("divModalDelegarMonitoramentoAnalista");
}

async function delegarMonitoramentoParaUsuario(cargasSelecionadas, codigoUsuario) {
    let codigosCargas = cargasSelecionadas.map((c) => c.Carga.val());
    let body = {
        CodigoAnalista: codigoUsuario,
        CodigosCargas: JSON.stringify(codigosCargas)
    }

    await executarReST("ControleEntrega/DefinirAnalistaResponsavelMonitoramentoCarga", body, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.AnalistaAtualizado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

        ExibirCheckBoxCargaEntrega(false);
        obterControleEntregas(1, false, true);
    });
}

async function buscarDadosUsuarioLogado() {
    return new Promise(resolve => {
        executarReST("Usuario/DadosUsuarioLogado", {}, function (retorno) {
            resolve(retorno.Data);
        });
    })
}