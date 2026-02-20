/// <reference path="PlanejamentoPedidoTMS.js" />

var _gridPlanejamentoPedidoTMSDisponibilidade;
var _pesquisaPlanejamentoPedidoTMSDisponibilidade;

function callbackGridPlanejamentoDisponibilidade() {
    setarAlocacaoDisponibilidade();
}

function loadGridPlanejamentoPedidoTMSDisponibilidade() {
    _gridPlanejamentoPedidoTMSDisponibilidade = new GridView("grid-planejamento-pedido-disponibilidade", "PlanejamentoPedidoTMS/ObterDisponibilidade", _pesquisaPlanejamentoPedidoTMS, null, null, 20);
    _gridPlanejamentoPedidoTMSDisponibilidade.SetPermitirRedimencionarColunas(true);
    _gridPlanejamentoPedidoTMSDisponibilidade.SetPermitirEdicaoColunas(true);
    _gridPlanejamentoPedidoTMSDisponibilidade.SetSalvarPreferenciasGrid(true);

    if (_pesquisaPlanejamentoPedidoTMS.ExibirDisponibilidade.val()) {
        _gridPlanejamentoPedidoTMSDisponibilidade.CarregarGrid(callbackGridPlanejamentoDisponibilidade);
    }
}

function ExibirPlanejamentoPedidoTMSDisponibilidadeClick(filaSelecionada) {
    
    _pesquisaPlanejamentoPedidoTMS.ExibirDisponibilidade.val(!_pesquisaPlanejamentoPedidoTMS.ExibirDisponibilidade.val());

    if (_pesquisaPlanejamentoPedidoTMS.ExibirDisponibilidade.val()) {
        $("#container-disponibilidade-pedido-fixar").show();
        $("#container-pedido").addClass("col-sm-9");

        _pesquisaPlanejamentoPedidoTMS.ExibirDisponibilidade.text("Ocultar Disponibilidade");
        _gridPlanejamentoPedidoTMSDisponibilidade.CarregarGrid(callbackGridPlanejamentoDisponibilidade);
    }
    else {
        $("#container-disponibilidade-pedido-fixar").hide();
        $("#container-pedido").removeClass("col-sm-9");

        _pesquisaPlanejamentoPedidoTMS.ExibirDisponibilidade.text("Exibir Disponibilidade");
    }
}

function setarTotalVeiculosAlocados() {
    executarReST("PlanejamentoPedidoTMS/ObterTotalVeiculosAlocados", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaPlanejamentoPedidoTMS.VeiculosAlocados.text('Alocados: ' + retorno.Data.Quantidade);
            }
        }
    });
}

function setarAlocacaoDisponibilidade() {
    if (_pesquisaPlanejamentoPedidoTMS.ExibirDisponibilidade.val()) {
        _pesquisaPlanejamentoPedidoTMS.TituloDisponibilidade.text(Global.DataAtual());
        _pesquisaPlanejamentoPedidoTMS.VeiculosDisponiveis.text('Disponíveis: ' + _gridPlanejamentoPedidoTMSDisponibilidade.NumeroRegistros());
        setarTotalVeiculosAlocados();
    }
}