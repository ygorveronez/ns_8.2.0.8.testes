/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _disponibilidadeVeiculo;

var DisponibilidadeVeiculo = function () {
    this.CodigoJanelaCarregamento = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
    this.DataPrevisaoChegada = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), def: "", required: true, text: Localization.Resources.Cargas.Carga.DataPrevisaoDeChegada.getRequiredFieldDescription() });

    this.Salvar = PropertyEntity({
        eventClick: salvarDataPrevisaoChegada, type: types.event, text: Localization.Resources.Gerais.Geral.Salvar, idGrid: guid(), visible: ko.observable(true)
    });
}

function loadDisponibilidadeVeiculo() {
    _disponibilidadeVeiculo = new DisponibilidadeVeiculo();
    KoBindings(_disponibilidadeVeiculo, "knockoutDisponibilidadeVeiculo");
}

function exibirModalDisponibilidadeVeiculo(codigo) {
    _disponibilidadeVeiculo.CodigoJanelaCarregamento.val(codigo);

    $("#divModalDisponibilidadeVeiculo")
        .modal('show')
        .on('hidden.bs.modal', function () {
            LimparCampos(_disponibilidadeVeiculo);
        });
}

function salvarDataPrevisaoChegada() {
    if (!ValidarCamposObrigatorios(_disponibilidadeVeiculo)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.PreenchaOsCamposObrigatorios);
        return;
    }

    executarReST("JanelaCarregamento/SalvarDataPrevisaoChegada", { Codigo: _disponibilidadeVeiculo.CodigoJanelaCarregamento.val(), DataPrevisaoChegada: _disponibilidadeVeiculo.DataPrevisaoChegada.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DataDePrevisaoDeChegadaFoiSalva);
                Global.fecharModal('divModalDisponibilidadeVeiculo');
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}