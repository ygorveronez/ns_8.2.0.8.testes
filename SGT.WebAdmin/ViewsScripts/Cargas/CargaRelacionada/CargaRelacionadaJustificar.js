var _justificativa;

var Justificativa = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Justificativa = PropertyEntity({ text: "*Justificativa Descritiva: ", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500, required: true });

    this.Cancelar = PropertyEntity({ eventClick: limparModalJustificativa, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(true) });
    this.Remover = PropertyEntity({ eventClick: removerJustificativaClick, type: types.event, text: "Remover", idGrid: guid(), visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarJustificativaClick, type: types.event, text: "Salvar", idGrid: guid(), visible: ko.observable(true) });
}

function loadJustificativa() {
    _justificativa = new Justificativa();
    KoBindings(_justificativa, "knockoutJustificar");
}

function adicionarJustificativaClick() {
    if (ValidarCamposObrigatorios(_justificativa)) {
        executarReST("CargaRelacionada/AdicionarJustificativa", { Codigo: _justificativa.Codigo.val(), Justificativa: _justificativa.Justificativa.val() }, function (retorno) {
            if (retorno.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);

                _gridCargaRelacionada.CarregarGrid();
                limparModalJustificativa();
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function removerJustificativaClick() {
    executarReST("CargaRelacionada/RemoverJustificativa", { Codigo: _justificativa.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);

            _gridCargaRelacionada.CarregarGrid();
            limparModalJustificativa();
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function buscarJustificativa(codigo) {
    executarReST("CargaRelacionada/BuscarJustificativaPorCodigo", { Codigo: codigo }, function (retorno) {
        _justificativa.Codigo.val(retorno.Data.Codigo);
        _justificativa.Justificativa.val(retorno.Data.Justificativa);
    });
}

function limparModalJustificativa() {
    Global.fecharModal("divModalJustificar");
    LimparCampos(_justificativa);
}