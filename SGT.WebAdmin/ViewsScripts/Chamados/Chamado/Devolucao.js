function EtapaCargaChamadoClick() {

    if (_chamado.CargaDevolucao.val() > 0) {
        var data = { Carga: _chamado.CargaDevolucao.val() };
        executarReST("Carga/BuscarCargaPorCodigo", data, function (r) {
            if (r.Success) {
                if (r.Data) {
                    $("#fdsCargaChamado").html('<button type="button" class="close" data-dismiss="modal" aria-hidden="true" style="position: absolute; z-index: 9999; right: 18px;">×</button>');
                    var knoutCarga = GerarTagHTMLDaCarga("fdsCargaChamado", r.Data, false);
                    $("#" + knoutCarga.DivCarga.id).attr("class", "p-2");
                    _cargaAtual = knoutCarga;
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    } else {
        $("#fdsCargaChamado").html('<p>Não foi gerada a carga de devolução</p>');
    }
}

function LoadCargasChamado() {
    buscarDetalhesOperador(function () {
        carregarConteudosHTML(function () {

        });
    });
}

function Etapa3ComoDevolucao() {
    //_analise.ImprimirDevolucao.visible(true);

    _etapa.Etapa3.text("Carga Devolução");
    _etapa.Etapa3.tooltip("Quando necessário, é criado uma carga de devolução.");
    _etapa.Etapa3.tooltipTitle("Carga Devolução");

    _analise.Finalizar.text("Gerar Carga de Devolução");
    _analise.Finalizar.visible(true);
    _analise.Fechar.text("Fechar (Não autorizar a devolução)");
    _analise.Fechar.visible(true);

    $("#knockoutCargaDevolucao").show();
}

function Etapa2ComoDevolucao() {
    $("#knockoutAnaliseDevolucao").show();
}