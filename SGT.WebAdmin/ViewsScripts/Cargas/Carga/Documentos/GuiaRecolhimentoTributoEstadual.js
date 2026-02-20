//#region Variaveis Globais
var _gridCargaIntegracaoDespesa;
var _historicoGuiaRecolhimento;
var _gridDetalhesGuiaRecolhimento;
//#endregion

//#region Contructores
function loadGridGuiasRecolhimentoTributarioEstatual() {
    const reenviar = { descricao: Localization.Resources.Gerais.Geral.Reenviar, id: guid(), metodo: reenviarGuiRecolhimento, icone: "", visibilidade: true };
    const download = { descricao: Localization.Resources.Gerais.Geral.Download, id: guid(), metodo: () => { }, icone: "", visibilidade: true };
    const cancelar = { descricao: Localization.Resources.Gerais.Geral.Cancelar, id: guid(), metodo: cancelarGuiRecolhimento, icone: "", visibilidade: true };
    const auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: () => { }, icone: "", visibilidade: true };

    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [reenviar, download, cancelar, auditar] };

    _gridCargaGuiasRecolhimento = new GridView(_cargaCTe.PesquisarGuiaRecolhimento.idGrid, "GuiaNacionalRecolhimentoTributoEstual/Consultar", _cargaCTe, menuOpcoes);
    _gridCargaGuiasRecolhimento.CarregarGrid(callbackGridGNRE);
}
var HistoricoGuiaRecolhimento = function () {
    this.GuiaRecolhimento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DetalhesGuiasRecolhimentos = PropertyEntity({ eventClick: function (e) { _gridDetalhesGuiaRecolhimento.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
};

function loadHistoricoGuiaRecolhimento() {
    _historicoGuiaRecolhimento = new HistoricoGuiaRecolhimento();

    KoBindings(_historicoGuiaRecolhimento, "knockoutGuiasRecolhimentos");
    _gridDetalhesGuiaRecolhimento = new GridView(_historicoGuiaRecolhimento.DetalhesGuiasRecolhimentos.idGrid, "GuiaNacionalRecolhimentoTributoEstual/BuscarHistorico", _historicoGuiaRecolhimento);
}
//#endregion

//#region Requisições

function reenviarTodasGuiasRejeitados(e) {

    var data = {
        Carga: _cargaAtual.Codigo.val()
    }
    exibirConfirmacao("Deseja realmente realizar esta ação?", "Ao aceitar serão enviados todos os registro rejeitados", function () {
        executarReST("GuiaNacionalRecolhimentoTributoEstual/ReenviarTodos", data, function (arg) {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            _gridCargaGuiasRecolhimento.CarregarGrid();
            return exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
        });
    });
}

function reenviarGuiRecolhimento(e) {
    executarReST("GuiaNacionalRecolhimentoTributoEstual/Reenviar", { Codigo: e.Codigo}, function (arg) {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        _gridCargaGuiasRecolhimento.CarregarGrid();
        return exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
    });
}

function cancelarGuiRecolhimento(e) {
    executarReST("GuiaNacionalRecolhimentoTributoEstual/Cancelar", { Codigo: e.Codigo }, function (arg) {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        _gridCargaGuiasRecolhimento.CarregarGrid();
        return exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg);
    });
}
//#endregion

//#region Metodos Auxiliares
function historicoGuiaRecolhimento(e) {
    loadHistoricoGuiaRecolhimento();
    _historicoGuiaRecolhimento.GuiaRecolhimento.val(parseInt(e.Codigo));
    _gridDetalhesGuiaRecolhimento.CarregarGrid();
    Global.abrirModal("divModalGuiaRecolhimento");
}
function callbackGridGNRE() {
    if (_gridCargaGuiasRecolhimento.NumeroRegistros() > 0)
        $("#tabGuiaRecolhimento_" + _cargaAtual.DadosCTes.id + "_li").show();
}


//#endregion