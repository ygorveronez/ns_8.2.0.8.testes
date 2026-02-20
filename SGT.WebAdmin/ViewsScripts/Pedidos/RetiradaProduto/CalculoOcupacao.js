var _pesoPedidos = {};

function calcularPesoProdutos() {
    var pedidos = JSON.parse(obterPedidos());
    var pesoTotal = 0;
    for (var i = 0; i < pedidos.length; i++) {
        var produtos = pedidos[i].Produtos;
        for (var j = 0; j < produtos.length; j++) {
            pesoTotal += (produtos[j].PesoUnitario * produtos[j].QuantidadeRetirada);
        }
        _pesoPedidos[pedidos[i].Codigo] = pesoTotal;
    }
    if (pesoTotal <= 0 || pesoTotal == undefined) {
        pesoTotal = 0;
        _pedido.EspacoDisponivel.val(0);
    }

    _pedido.PesoTotal.val(pesoTotal);
    _pedido.PesoTotal.formatado(Globalize.format(pesoTotal, "n2"));
    calculaDisponibilidade();
    calculaOcupacao();
}

function calculaDisponibilidade() {
    _pedido.EspacoDisponivel.val(_pedido.CapacidadeVeiculo.val() - _pedido.PesoTotal.val());
    _pedido.CapacidadeVeiculo.formatado(Globalize.format(_pedido.CapacidadeVeiculo.val(), "n2"));
    _pedido.EspacoDisponivel.formatado(Globalize.format(_pedido.EspacoDisponivel.val(), "n2"));
}

function calculaOcupacao() {
    var valor = _pedido.PesoTotal.val() / _pedido.CapacidadeVeiculo.val() * 100;
    _pedido.Ocupacao.val(valor.toFixed(2));
    _pedido.Ocupacao.width(valor.toFixed(2));
    _pedido.Ocupacao.formatado(valor.toFixed(2))
    if (_pedido.EspacoDisponivel.val() < 0) {
        _pedido.Ocupacao.width(100);
        //_pedido.Ocupacao.color("#f40808");
        $("#progress-bar-1.progress-bar").removeClass("bg-success");
        $("#progress-bar-1.progress-bar").addClass("bg-danger");

        $("#progress-bar-2.progress-bar").removeClass("bg-success");
        $("#progress-bar-2.progress-bar").addClass("bg-danger");
    } else {
        //_pedido.Ocupacao.color("#57889c");
        $("#progress-bar-1.progress-bar").removeClass("bg-danger");
        $("#progress-bar-1.progress-bar").addClass("bg-success");

        $("#progress-bar-2.progress-bar").removeClass("bg-danger");
        $("#progress-bar-2.progress-bar").addClass("bg-success");
    }
}

function roundNumber(value) {
    if (typeof value == "string")
        return Globalize.parseFloat(value);

    if (typeof value == "number")
        Globalize.format(value, "n2");
}