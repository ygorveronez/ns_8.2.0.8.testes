function LoadSignalRCarregamentoAutomatico() {
    SignalRCarregamentoAutomaticoFechamentoEvent = InformarCarregamentoAutomaticoFinalizadoEvent;
    SignalRCarregamentoAutomaticoAtualizacaoEvent = AtualizarQuantidadeProcessadosCarregamentoAutomatico;
    SignalRFiltroPesquisaGestaoPedidoSessaoRoteirizadorEvent = AbrirFiltroPesquisaaGestaoPedidoSessaoRoteirizador;
}

function AtualizarQuantidadeProcessadosCarregamentoAutomatico(dados) {
    if (_percentualCarregamentoAutomatico != null) {
        var codigoSessaoAtual = _pesquisaMontegemCarga.SessaoRoteirizador.codEntity();
        if (dados.SessaoRoterizador == codigoSessaoAtual) {
            var visible = $("#knockoutPercentualCarregamentoAutomatico").is(':visible');
            if (!visible) {
                Global.abrirModal("knockoutPercentualCarregamentoAutomatico");
            }
            SetarPercentualCarregamentoAutomatico((dados.QuantidadeProcessada * 100) / dados.QuantidadeTotal, dados.Descricao);
        }
    }
}

function InformarCarregamentoAutomaticoFinalizadoEvent(dados) {
    if (_percentualCarregamentoAutomatico != null) {
        var codigoSessaoAtual = _pesquisaMontegemCarga.SessaoRoteirizador.codEntity();
        if (dados.SessaoRoterizador == codigoSessaoAtual) {
            SetarCarregamentoAutomaticoFinalizado(dados);
        }
    }
}

function AbrirFiltroPesquisaaGestaoPedidoSessaoRoteirizador(dados) {
    if (_pesquisaMontegemCarga != null) {
        var codigoSessaoAtual = _pesquisaMontegemCarga.SessaoRoteirizador.codEntity();
        if (dados.SessaoRoteirizador == codigoSessaoAtual) {
            /* dynamic dadosRetorno = new
                {
                    Filial = new { filial.Codigo, filial.Descricao },
                    DataInicio = dataInicio,
                    DataFim = dataFinal,
                    Pedidos = (sessao > 0 ? new List<int>() : codigosPedidos),
                    PedidosSemSessao = pedidosSemSessao,
                    CodigosAgrupadores = (sessao > 0 ? new List<string>() : codigosAgrupadores),
                    SessaoRoteirizador = sessao
                };*/
            modalPesquisar(dados);
        }
    }
}