/// <reference path="../../Consultas/NotaFiscalSituacao.js" />

var _agendamentoEntregaNotaFiscalSugestao;

var AgendamentoEntregaNotaFiscalSituacao = function () {
    this.CodigoCargaPedido = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.CodigoCargaEntrega = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.NotaFiscalSituacao = PropertyEntity({ codEntity: ko.observable(0), required: true, type: types.entity, text: "*Situação:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Data = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), required: true, text: "*Data:", enable: ko.observable(true) });
    
    this.Salvar = PropertyEntity({ type: types.event, eventClick: salvarNotaFiscalSituacaoClick, text: "Salvar", id: guid() });
}

function loadAgendamentoEntregaNotaFiscalSituacao() {
    _agendamentoEntregaNotaFiscalSugestao = new AgendamentoEntregaNotaFiscalSituacao();
    KoBindings(_agendamentoEntregaNotaFiscalSugestao, "knockoutSituacaoNotaFiscal");
    
    new BuscarNotaFiscalSituacao(_agendamentoEntregaNotaFiscalSugestao.NotaFiscalSituacao);
}

function salvarNotaFiscalSituacaoClick() {
    if (!ValidarCamposObrigatorios(_agendamentoEntregaNotaFiscalSugestao)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar os campos obrigatórios.");
        return;
    }

    executarReST("AgendamentoEntregaPedido/SalvarSituacaoNotaFiscal", RetornarObjetoPesquisa(_agendamentoEntregaNotaFiscalSugestao), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "A situação das notas do pedido foram salvas.");
                _gridAgendamentoEntregaPedido.CarregarGrid();
                Global.fecharModal('divModalNotaFiscalSituacao');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirModalAgendamentoNotaFiscalSituacao(registroSelecionado) {
    _agendamentoEntregaNotaFiscalSugestao.CodigoCargaPedido.val(registroSelecionado.Codigo);
    _agendamentoEntregaNotaFiscalSugestao.CodigoCargaEntrega.val(registroSelecionado.CodigoCargaEntrega);

    $("#divModalNotaFiscalSituacao")
        .modal('show')
        .on("hidden.bs.modal", function () {
            LimparCampos(_agendamentoEntregaNotaFiscalSugestao);
        });
}