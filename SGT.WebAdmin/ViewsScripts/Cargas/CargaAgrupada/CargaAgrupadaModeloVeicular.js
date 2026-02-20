/// <reference path="../../Consultas/ModeloVeicularCarga.js" />

var _CRUDEdicaoModeloVeicular;
var _edicaoModeloVeicular;

var CargaAgrupadaModeloVeicular = function () {
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo Veicular:", idBtnSearch: guid(), required: true });
};

var CRUDCargaAgrupadaModeloVeicular = function () {
    this.Atualizar = PropertyEntity({
        eventClick: atualizarModeloVeicularClick, type: types.event, text: "Atualizar", idFade: guid(), visible: ko.observable(true)
    });
};

function loadCargaAgrupadaModeloVeicular() {
    _edicaoModeloVeicular = new CargaAgrupadaModeloVeicular();
    _CRUDEdicaoModeloVeicular = new CRUDCargaAgrupadaModeloVeicular();

    KoBindings(_edicaoModeloVeicular, "knockoutEdicaoModeloVeicular");
    KoBindings(_CRUDEdicaoModeloVeicular, "knockoutCRUDEdicaoModeloVeicular");

    new BuscarModelosVeicularesCarga(_edicaoModeloVeicular.ModeloVeicular);
}

function atualizarModeloVeicularClick() {
    if (!ValidarCamposObrigatorios(_edicaoModeloVeicular)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário preencher os campos obrigatórios.");
        return;
    }

    executarReST("CargaAgrupada/AlterarModeloVeicular", RetornarObjetoPesquisa(_edicaoModeloVeicular), function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualização realizada com sucesso.");
                atualizarModeloVeicularNaGrid(arg.Data.ModeloVeicularOrigem);
                _gridCarga.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function atualizarModeloVeicularNaGrid(modeloVeicularOrigem) {
    var registros = _gridNovaCarga.BuscarRegistros();
    for (var i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == _edicaoModeloVeicular.CodigoCarga.val()) {
            registros[i].ModeloVeicularOrigem = modeloVeicularOrigem;
            break;
        }
    }

    _gridNovaCarga.CarregarGrid(registros);

    Global.fecharModal("divModalEditarModeloVeicular");
}