/// <reference path="PlanejamentoPedido.js" />
/// <reference path="PlanejamentoPedidoDisponibilidade.js" />

var novaPesquisaMotorista = false;

function ExibirPlanejamentoPedidoDisponibilidadeMotoristaClick() {
    _pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.val(!_pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.val());

    if (!_pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.val()) {
        exibirPlanejamentoPedidoDisponibilidadeMotorista();
        return;
    }

    obterListaDisponibilidade(exibirPlanejamentoPedidoDisponibilidadeMotorista);
}

function forcarRecarregarGridPlanejamentoPedidoDisponibilidadeMotorista() {
    novaPesquisaMotorista = true;
    recarregarGridPlanejamentoPedidoDisponibilidadeMotorista();
}

function recarregarGridPlanejamentoPedidoDisponibilidadeMotorista() {
    if (!_pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.val() || !novaPesquisaMotorista)
        return;

    $.each(_pesquisaPlanejamentoPedido.ListaDisponibilidade.slice(), function (i, knoutListaDisponibilidade) {
        knoutListaDisponibilidade.GridPlanejamentoPedidoDisponibilidadeMotorista.CarregarGrid(function () {
            callbackGridPlanejamentoDisponibilidadeMotorista(knoutListaDisponibilidade);
        });
    });

    novaPesquisaMotorista = false;
}

function criarGridPlanejamentoPedidoDisponibilidadeMotorista(knout) {
    var draggableRows = false;
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 50;

    var editarColunaPlanejamentoPedidoDisponibilidadeMotorista = {
        permite: true,
        callback: function (disponibilidadeSelecionada, linhaSelecionada, cabecalho, callbackTabPress) {
            editarColunaPlanejamentoPedidoDisponibilidadeMotoristaRetorno(disponibilidadeSelecionada, linhaSelecionada, cabecalho, callbackTabPress, knout)
        },
        atualizarRow: false
    };

    var _gridPlanejamentoPedidoDisponibilidadeMotorista = new GridView("grid-planejamento-pedido-disponibilidade-motorista-" + knout.Codigo.val(), "PlanejamentoPedido/ObterDisponibilidadeMotorista", knout, null, null, totalRegistrosPorPagina, null, false, draggableRows, null,
        limiteRegistros, editarColunaPlanejamentoPedidoDisponibilidadeMotorista);

    var _timeoutPlanejamentoPedidoDisponibilidadeMotorista;
    $('#localizarMotoristaPlanejamentoPedido-' + knout.Codigo.val()).on('keyup', function (e) {

        if (_timeoutPlanejamentoPedidoDisponibilidadeMotorista) {
            clearTimeout(_timeoutPlanejamentoPedidoDisponibilidadeMotorista);
            _timeoutPlanejamentoPedidoDisponibilidadeMotorista = null;
        }

        _timeoutPlanejamentoPedidoDisponibilidadeMotorista = setTimeout(function () {
            knout.FiltroPlanejamentoDisponibilidadeMotorista.val(e.target.value);

            _gridPlanejamentoPedidoDisponibilidadeMotorista.CarregarGrid();

        }, 800);

    });

    return _gridPlanejamentoPedidoDisponibilidadeMotorista;
}

function callbackGridPlanejamentoDisponibilidadeMotorista(knout) {
    if (_pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.val()) {
        knout.MotoristasDisponiveis.text('Disponíveis: ' + knout.GridPlanejamentoPedidoDisponibilidadeMotorista.NumeroRegistros());

        executarReST("PlanejamentoPedido/ObterTotalMotoristasAlocados", { Data: knout.Data.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    knout.MotoristasAlocados.text('Alocados: ' + retorno.Data.Quantidade);
                }
            }
        });
    }
}

function exibirPlanejamentoPedidoDisponibilidadeMotorista() {
    if (_pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.val()) {
        $(".container-disponibilidade-motorista-pedido-fixar").show();

        if (_pesquisaPlanejamentoPedido.ExibirDisponibilidade.val()) {
            $("#container-pedido").addClass("col-sm-8");
            $("#container-pedido").removeClass("col-sm-10");
            $("#container-disponibilidade").removeClass("col-sm-2");
            $("#container-disponibilidade").addClass("col-sm-4");
            $(".container-disponibilidade-motorista-pedido-fixar").addClass("col-sm-6");

            $(".container-disponibilidade-pedido-fixar").removeClass("col-sm-12");
            $(".container-disponibilidade-pedido-fixar").addClass("col-sm-6");
        }
        else {
            $("#container-disponibilidade").show();
            $("#container-pedido").addClass("col-sm-10");
            $("#container-pedido").removeClass("col-sm-8");
            $("#container-disponibilidade").addClass("col-sm-2");
            $(".container-disponibilidade-motorista-pedido-fixar").addClass("col-sm-12");
        }

        _pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.text("Ocultar Disponibilidade Motorista");
        recarregarGridPlanejamentoPedidoDisponibilidadeMotorista();
    }
    else {
        $(".container-disponibilidade-motorista-pedido-fixar").hide();

        if (_pesquisaPlanejamentoPedido.ExibirDisponibilidade.val()) {
            $("#container-pedido").addClass("col-sm-10");
            $("#container-pedido").removeClass("col-sm-8");
            $("#container-disponibilidade").removeClass("col-sm-4");
            $("#container-disponibilidade").addClass("col-sm-2");
            $(".container-disponibilidade-motorista-pedido-fixar").removeClass("col-sm-6");

            $(".container-disponibilidade-pedido-fixar").removeClass("col-sm-6");
            $(".container-disponibilidade-pedido-fixar").addClass("col-sm-12");
        }
        else {
            $("#container-disponibilidade").hide();
            $("#container-pedido").removeClass("col-sm-10");
            $("#container-disponibilidade").removeClass("col-sm-2");
            $(".container-disponibilidade-motorista-pedido-fixar").removeClass("col-sm-12");
        }

        _pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.text("Exibir Disponibilidade Motorista");
    }
}

function editarColunaPlanejamentoPedidoDisponibilidadeMotoristaRetorno(disponibilidadeSelecionada, linhaSelecionada, cabecalho, callbackTabPress, knout) {
    var dadosAtualizar = {
        Codigo: disponibilidadeSelecionada.Codigo,
        Observacao: disponibilidadeSelecionada.Observacao
    };

    executarReST("PlanejamentoPedido/AlterarDadosDisponibilidadeMotorista", dadosAtualizar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                knout.GridPlanejamentoPedidoDisponibilidadeMotorista.AtualizarDataRow(linhaSelecionada, retorno.Data, callbackTabPress);
            }
            else {
                knout.GridPlanejamentoPedidoDisponibilidadeMotorista.DesfazerAlteracaoDataRow(linhaSelecionada);
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
            }
        }
        else {
            knout.GridPlanejamentoPedidoDisponibilidadeMotorista.DesfazerAlteracaoDataRow(linhaSelecionada);
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}