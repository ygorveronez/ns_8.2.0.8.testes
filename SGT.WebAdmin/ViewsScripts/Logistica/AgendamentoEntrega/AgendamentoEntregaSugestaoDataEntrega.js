var _agendamentoEntregaSugestaoDataEntrega;

var AgendamentoEntregaSugestaoDataEntrega = function () {
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.DataSugestaoEntrega = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), required: true, text: "*Data Sugerida para Entrega:", enable: ko.observable(true) });
    
    this.Salvar = PropertyEntity({ type: types.event, eventClick: salvarDataSugeridaEntregaClick, text: "Salvar", id: guid() });
}

function loadAgendamentoEntregaSugestaoDataEntrega() {
    _agendamentoEntregaSugestaoDataEntrega = new AgendamentoEntregaSugestaoDataEntrega();
    KoBindings(_agendamentoEntregaSugestaoDataEntrega, "knockoutDataSugestaoEntrega");
}

function salvarDataSugeridaEntregaClick() {
    if (!ValidarCamposObrigatorios(_agendamentoEntregaSugestaoDataEntrega)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar a data.");
        return;
    }

    executarReST("AgendamentoEntregaPedido/SalvarSugestaoDataEntrega", RetornarObjetoPesquisa(_agendamentoEntregaSugestaoDataEntrega), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "A data sugerida para entrega foi informada.");
                _gridAgendamentoEntregaPedido.CarregarGrid();
                Global.fecharModal('divModalDataSugestaoEntrega');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirModalAgendamentoSugestaoDataEntrega(registroSelecionado) {
    _agendamentoEntregaSugestaoDataEntrega.CodigoCarga.val(registroSelecionado.CodigoCarga);

    $("#divModalDataSugestaoEntrega")
        .modal('show')
        .on("hidden.bs.modal", function () {
            LimparCampos(_agendamentoEntregaSugestaoDataEntrega);
        });
}