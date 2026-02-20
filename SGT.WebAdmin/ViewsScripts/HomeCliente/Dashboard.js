var Dashboard = function () {
    var _chart;
    var _dashboard;

    var dadosGrafico = [
        {
            Situacao: EnumSituacaoAcompanhamentoPedido.AgColeta,
            Cor: "#1cc8ee"
        },
        {
            Situacao: EnumSituacaoAcompanhamentoPedido.EmTransporte,
            Cor: "#f9ae0b"
        },
        {
            Situacao: EnumSituacaoAcompanhamentoPedido.SaiuParaEntrega,
            Cor: "#ed2e7e"
        },
        {
            Situacao: EnumSituacaoAcompanhamentoPedido.Entregue,
            Cor: "#475999"
        },
        {
            Situacao: EnumSituacaoAcompanhamentoPedido.EntregaParcial,
            Cor: "#f9763d"
        },
        {
            Situacao: EnumSituacaoAcompanhamentoPedido.EntregaRejeitada,
            Cor: "#ffe9e0"
        },
        {
            Situacao: EnumSituacaoAcompanhamentoPedido.ProblemaNoTransporte,
            Cor: "#747aca"
        }
    ];
    var configGrafico = {
        type: 'doughnut',
        data: {
            labels: [],
            datasets: [{
                data: [],
                situacao: [],
                backgroundColor: [],
                hoverBackgroundColor: []
            }]
        },
        options: {
            elements: {
                center: {
                    text: '',
                    color: '#475999',
                    fontStyle: 'Roboto',
                    sidePadding: 20,
                    minFontSize: 25,
                    lineHeight: 25
                }
            }
        }
    };

    /**
     * Definições Knockout
     */
    var Dashboard = function () {
        this.Chart = PropertyEntity({});
        this.GraficoVazio = PropertyEntity({ val: ko.observable(false) });
    };

    /**
     * Eventos Knockout
     */


    /**
     * Métodos Público
     */
    this.Load = function (id) {
        _dashboard = new Dashboard();
        KoBindings(_dashboard, id);

        if (!_CONFIGURACAO_TMS.SomenteVisualizacaoBI)
            carregarGrafico();
    }

    /**
     * Métodos Privados
     */
    var navegarParaPedidosComSituacao = function (situacao) {
        _FiltroPedidoGlobal.situacao = situacao;
        window.location.hash = 'Pedidos/Pedido';
    }

    var obeterCorPorSituacao = function (situacao) {
        var cor = dadosGrafico.filter(function (d) {
            return d.Situacao == situacao;
        });

        if (!cor) return '';

        return cor[0].Cor;
    }

    var preencherGrafico = function (dados) {
        var totalPedidos = 0;

        for (var i in dados) {
            var dado = dados[i];
            var cor = obeterCorPorSituacao(dado.Situacao)
            var label = EnumSituacaoAcompanhamentoPedido.obterDescricaoPortalCliente(dado.Situacao);

            totalPedidos += dado.Quantidade;

            configGrafico.data.labels.push(label);
            configGrafico.data.datasets[0].backgroundColor.push(cor);
            configGrafico.data.datasets[0].situacao.push(dado.Situacao);
            configGrafico.data.datasets[0].data.push(dado.Quantidade);
        }

        if (1 === totalPedidos)
            configGrafico.options.elements.center.text = "Uma entrega";
        else
            configGrafico.options.elements.center.text = totalPedidos + " entregas";

        if (0 === totalPedidos) {
            _dashboard.GraficoVazio.val(true);
            return;
        }

        var $elemento = _dashboard.Chart.get$();
        var canvasContexto = $elemento[0].getContext("2d");
        _chart = new Chart(canvasContexto, configGrafico);

        $elemento.click(function (e) {
            var activePoints = _chart.getElementsAtEvent(e);

            if (0 === activePoints.length)
                return;

            var selectedIndex = activePoints[0]._index;
            if (!_CONFIGURACAO_TMS.SomenteVisualizacaoBI)
                navegarParaPedidosComSituacao(configGrafico.data.datasets[0].situacao[selectedIndex]);
        });
    }

    var carregarGrafico = function () {
        var dados = {
            Filtros: JSON.stringify(dadosGrafico.map(function (f) { return f.Situacao }))
        };

        executarReST("PedidoCliente/BuscarDadosFiltroPedidos", dados, function (arg) {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

            if (!arg.Data)
                return exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);

            var dados = arg.Data.Filtros;

            preencherGrafico(dados);
        });
    }
}