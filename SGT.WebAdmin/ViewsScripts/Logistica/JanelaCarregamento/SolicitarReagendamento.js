var _solicitarReagendamento;

var SolicitarReagendamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    
    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Observacoes.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 300 });
    this.SolicitarReagendamento = PropertyEntity({ eventClick: solicitarReagendamentoClick, type: types.event, text: Localization.Resources.Cargas.Carga.SolicitarReagendamento, idGrid: guid(), visible: ko.observable(true)});
}

function loadSolicitarReagendamento() {
    _solicitarReagendamento = new SolicitarReagendamento();
    KoBindings(_solicitarReagendamento, "knockoutSolicitarReagendamento");
}

function exibirSolicitarReagendamento(carga) {
    _solicitarReagendamento.Codigo.val(carga.Carga.Codigo);

    $("#divModalSolicitarReagendamento")
        .modal('show')
        .on('hidden.bs.modal', function () {
            LimparCampos(_solicitarReagendamento);
        });
}

function solicitarReagendamentoClick() {
    executarReST("AgendamentoEntregaPedido/SolicitarReagendamento", RetornarObjetoPesquisa(_solicitarReagendamento), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.GeralSucesso, Localization.Resources.Cargas.Carga.ReagendamentoSolicitadoComSucesso);
                Global.fecharModal('divModalSolicitarReagendamento');
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}
