/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _disponibilidadeVeiculo;

var DisponibilidadeVeiculo = function () {
    this.CodigoJanelaDescarregamento = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
    this.DataPrevisaoChegada = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), def: "", required: true, text: "*Data Previsão de Chegada:" });

    this.Salvar = PropertyEntity({
        eventClick: salvarDataPrevisaoChegada, type: types.event, text: "Salvar", idGrid: guid(), visible: ko.observable(true)
    });
};

function loadDisponibilidadeVeiculo() {
    _disponibilidadeVeiculo = new DisponibilidadeVeiculo();
    KoBindings(_disponibilidadeVeiculo, "knockoutDisponibilidadeVeiculo");
}

function exibirModalDisponibilidadeVeiculo(codigo) {
    _disponibilidadeVeiculo.CodigoJanelaDescarregamento.val(codigo);

    $("#divModalDisponibilidadeVeiculo")
        .modal('show')
        .on('hidden.bs.modal', function () {
            LimparCampos(_disponibilidadeVeiculo);
        });
}

function salvarDataPrevisaoChegada() {
    if (!ValidarCamposObrigatorios(_disponibilidadeVeiculo)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    executarReST("JanelaDescarga/SalvarDataPrevisaoChegada", { Codigo: _disponibilidadeVeiculo.CodigoJanelaDescarregamento.val(), DataPrevisaoChegada: _disponibilidadeVeiculo.DataPrevisaoChegada.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "A data de previsão de chegada foi salva!");
                Global.fecharModal('divModalDisponibilidadeVeiculo');
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}