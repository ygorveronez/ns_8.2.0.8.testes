var _pacotesAvulsos;
var _gridPacotesAvulsos;

var PacotesAvulsos = function () {
    this.GridPacotesAvulsos = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
    this.NumeroPacote = PropertyEntity({ text: "Número Pacote:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _gridPacotesAvulsos.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPacotesAvulsosClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPacotesAvulsosClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
};


function loadPacotesAvulsos() {
    _pacotesAvulsos = new PacotesAvulsos();
    KoBindings(_pacotesAvulsos, "knocoutPacotesAvulsos", null, _pacotesAvulsos.Pesquisar);

    BuscarPacotesAvulsos();
}

function BuscarPacotesAvulsos() {
    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: null,
        callbackNaoSelecionado: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: false
    };

    _gridPacotesAvulsos = new GridView(_pacotesAvulsos.GridPacotesAvulsos.idGrid, "CargaNotasFiscais/BuscarPacotesAvulsos", _pacotesAvulsos, null, null, null, null, null, null, multiplaescolha);
}

function cancelarPacotesAvulsosClick() {
    Global.fecharModal("divModalPacotesAvulsos");
}

function adicionarPacotesAvulsosClick(e) {
    var data = {
        Codigos: JSON.stringify(_gridPacotesAvulsos.ObterMultiplosSelecionados()),
        Carga: _cargaAtual.Codigo.val()
    };

    exibirConfirmacao("Confirmação", "Deseja realmente vincular o(s) pacote(s) à carga?", function () {
        executarReST("CargaNotasFiscais/VincularPacoteAvulso", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    Global.fecharModal("divModalPacotesAvulsos");
                    exibirMensagem(tipoMensagem.ok, arg.Msg);
                    _gridPacotes.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}