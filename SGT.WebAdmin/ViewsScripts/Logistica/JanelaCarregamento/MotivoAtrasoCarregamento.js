/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/MotivoAtrasoCarregamento.js" />


var _motivoAtrasoCarremgamentoAlterar;

var MotivoAtrasoCarregamentoAlterar = function () {
    this.CodigoJanelaCarregamento = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
    this.MotivoAtrasoCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo Atraso", idBtnSearch: guid(), required: false });

    this.Salvar = PropertyEntity({
        eventClick: salvarMotivoAtrasoCarregamento, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, idGrid: guid(), visible: ko.observable(true)
    });
}

function loadMotivoAtrasoCarregamentoAlterar() {
    _motivoAtrasoCarremgamentoAlterar = new MotivoAtrasoCarregamentoAlterar();
    KoBindings(_motivoAtrasoCarremgamentoAlterar, "knockoutMotivoAtrasoCarregamentoAlterar");

    BuscarMotivoAtrasoCarregamento(_motivoAtrasoCarremgamentoAlterar.MotivoAtrasoCarregamento, retornoMotivoAtrasoCarregamento);
}

function exibirModalMotivoAtraso(codigo) {
    _motivoAtrasoCarremgamentoAlterar.CodigoJanelaCarregamento.val(codigo);

    $("#divModalMotivoAtrasoCarregamento")
        .modal('show')
        .on('hidden.bs.modal', function () {
            LimparCampos(_motivoAtrasoCarremgamentoAlterar);
        });
}

function salvarMotivoAtrasoCarregamento() {
    if (!ValidarCamposObrigatorios(_motivoAtrasoCarremgamentoAlterar)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.PreenchaOsCamposObrigatorios);
        return;
    }

    const data = { Codigo: _motivoAtrasoCarremgamentoAlterar.CodigoJanelaCarregamento.val(), MotivoAtraso: _motivoAtrasoCarremgamentoAlterar.MotivoAtrasoCarregamento.codEntity() }

    executarReST("JanelaCarregamento/AlterarMotivoCarregamento", data, function (retorno) {
        if (retorno.Success) {
            Global.fecharModal('divModalMotivoAtrasoCarregamento');
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function retornoMotivoAtrasoCarregamento(motivo) {
    _motivoAtrasoCarremgamentoAlterar.MotivoAtrasoCarregamento.val(motivo.Descricao);
    _motivoAtrasoCarremgamentoAlterar.MotivoAtrasoCarregamento.codEntity(motivo.Codigo);
}