/// <reference path="PlanejamentoPedido.js" />
/// <reference path="PlanejamentoPedidoDisponibilidadeMotorista.js" />

var efetuouNovaPesquisa = true;
var novaPesquisaVeiculo = false;

var PlanejamentoPedidoListaDisponibilidade = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Data = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.date });

    //Veículo
    this.TituloDisponibilidade = PropertyEntity({ text: ko.observable("Disponibilidade") });
    this.VeiculosDisponiveis = PropertyEntity({ text: ko.observable("") });
    this.VeiculosAlocados = PropertyEntity({ text: ko.observable("") });
    this.FiltroPlanejamentoDisponibilidade = PropertyEntity({ val: ko.observable(""), def: "" });
    this.GridPlanejamentoPedidoDisponibilidade;

    //Motorista
    this.TituloDisponibilidadeMotorista = PropertyEntity({ text: ko.observable("Disponibilidade Motorista") });
    this.MotoristasDisponiveis = PropertyEntity({ text: ko.observable("") });
    this.MotoristasAlocados = PropertyEntity({ text: ko.observable("") });
    this.FiltroPlanejamentoDisponibilidadeMotorista = PropertyEntity({ val: ko.observable(""), def: "" });
    this.GridPlanejamentoPedidoDisponibilidadeMotorista;
};

function obterListaDisponibilidade(callback) {
    if (!efetuouNovaPesquisa) {
        if (callback instanceof Function)
            callback();

        return;
    }

    executarReST("PlanejamentoPedido/ObterListaDisponibilidade", { DataInicio: _pesquisaPlanejamentoPedido.DataInicio.val(), DataFim: _pesquisaPlanejamentoPedido.DataFim.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaPlanejamentoPedido.ListaDisponibilidade.removeAll();

                for (var i = 0; i < retorno.Data.length; i++) {
                    var data = retorno.Data[i];

                    preencherListaDisponibilidade(data);
                }

                efetuouNovaPesquisa = false;
                novaPesquisaVeiculo = true;
                novaPesquisaMotorista = true;

                if (callback instanceof Function)
                    callback();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function preencherListaDisponibilidade(data) {
    var knoutListaDisponibilidade = new PlanejamentoPedidoListaDisponibilidade();
    knoutListaDisponibilidade.Codigo.val(data.Codigo);
    knoutListaDisponibilidade.Data.val(data.Data);
    knoutListaDisponibilidade.TituloDisponibilidade.text(data.Data);
    knoutListaDisponibilidade.TituloDisponibilidadeMotorista.text(data.Data);

    _pesquisaPlanejamentoPedido.ListaDisponibilidade.push(knoutListaDisponibilidade);

    knoutListaDisponibilidade.GridPlanejamentoPedidoDisponibilidade = criarGridPlanejamentoPedidoDisponibilidade(knoutListaDisponibilidade);
    knoutListaDisponibilidade.GridPlanejamentoPedidoDisponibilidadeMotorista = criarGridPlanejamentoPedidoDisponibilidadeMotorista(knoutListaDisponibilidade);
}

function fecharDisponibilidades() {
    if (_pesquisaPlanejamentoPedido.ExibirDisponibilidade.val()) {
        _pesquisaPlanejamentoPedido.ExibirDisponibilidade.val(false);
        exibirPlanejamentoPedidoDisponibilidade();
    }

    if (_pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.val()) {
        _pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.val(false);
        exibirPlanejamentoPedidoDisponibilidadeMotorista();
    }

    efetuouNovaPesquisa = true;
}

/*
 * Declaração das Funções
 */

function ExibirPlanejamentoPedidoDisponibilidadeClick() {
    _pesquisaPlanejamentoPedido.ExibirDisponibilidade.val(!_pesquisaPlanejamentoPedido.ExibirDisponibilidade.val());

    if (!_pesquisaPlanejamentoPedido.ExibirDisponibilidade.val()) {
        exibirPlanejamentoPedidoDisponibilidade();
        return;
    }

    obterListaDisponibilidade(exibirPlanejamentoPedidoDisponibilidade);
}

function forcarRecarregarGridPlanejamentoPedidoDisponibilidade() {
    novaPesquisaVeiculo = true;
    recarregarGridPlanejamentoPedidoDisponibilidade();
}

function recarregarGridPlanejamentoPedidoDisponibilidade() {
    if (!_pesquisaPlanejamentoPedido.ExibirDisponibilidade.val() || !novaPesquisaVeiculo)
        return;

    $.each(_pesquisaPlanejamentoPedido.ListaDisponibilidade.slice(), function (i, knoutListaDisponibilidade) {
        knoutListaDisponibilidade.GridPlanejamentoPedidoDisponibilidade.CarregarGrid(function () {
            callbackGridPlanejamentoDisponibilidade(knoutListaDisponibilidade);
        });
    });

    novaPesquisaVeiculo = false;
}

function criarGridPlanejamentoPedidoDisponibilidade(knout) {
    var draggableRows = false;
    var limiteRegistros = 50;
    var totalRegistrosPorPagina = 50;

    var editarColunaPlanejamentoPedidoDisponibilidade = {
        permite: true,
        callback: function (disponibilidadeSelecionada, linhaSelecionada, cabecalho, callbackTabPress) {
            editarColunaPlanejamentoPedidoDisponibilidadeRetorno(disponibilidadeSelecionada, linhaSelecionada, cabecalho, callbackTabPress, knout)
        },
        atualizarRow: false
    };

    var _gridPlanejamentoPedidoDisponibilidade = new GridView("grid-planejamento-pedido-disponibilidade-" + knout.Codigo.val(), "PlanejamentoPedido/ObterDisponibilidade", knout, null, null, totalRegistrosPorPagina, null, false, draggableRows, null,
        limiteRegistros, editarColunaPlanejamentoPedidoDisponibilidade);

    var _timeoutPlanejamentoPedido;
    $('#localizarPlanejamentoPedido-' + knout.Codigo.val()).on('keyup', function (e) {

        if (_timeoutPlanejamentoPedido) {
            clearTimeout(_timeoutPlanejamentoPedido);
            _timeoutPlanejamentoPedido = null;
        }

        _timeoutPlanejamentoPedido = setTimeout(function () {
            knout.FiltroPlanejamentoDisponibilidade.val(e.target.value);

            _gridPlanejamentoPedidoDisponibilidade.CarregarGrid();

        }, 800);

    });

    return _gridPlanejamentoPedidoDisponibilidade;
}

function callbackGridPlanejamentoDisponibilidade(knout) {
    if (_pesquisaPlanejamentoPedido.ExibirDisponibilidade.val()) {
        knout.VeiculosDisponiveis.text('Disponíveis: ' + knout.GridPlanejamentoPedidoDisponibilidade.NumeroRegistros());

        executarReST("PlanejamentoPedido/ObterTotalVeiculosAlocados", { Data: knout.Data.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    knout.VeiculosAlocados.text('Alocados: ' + retorno.Data.Quantidade);
                }
            }
        });
    }
}

function exibirPlanejamentoPedidoDisponibilidade() {
    if (_pesquisaPlanejamentoPedido.ExibirDisponibilidade.val()) {
        $(".container-disponibilidade-pedido-fixar").show();

        if (_pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.val()) {
            $("#container-pedido").addClass("col-sm-8");
            $("#container-pedido").removeClass("col-sm-10");
            $("#container-disponibilidade").removeClass("col-sm-2");
            $("#container-disponibilidade").addClass("col-sm-4");
            $(".container-disponibilidade-pedido-fixar").addClass("col-sm-6");

            $(".container-disponibilidade-motorista-pedido-fixar").removeClass("col-sm-12");
            $(".container-disponibilidade-motorista-pedido-fixar").addClass("col-sm-6");
        }
        else {
            $("#container-disponibilidade").show();
            $("#container-pedido").addClass("col-sm-10");
            $("#container-pedido").removeClass("col-sm-8");
            $("#container-disponibilidade").addClass("col-sm-2");
            $(".container-disponibilidade-pedido-fixar").addClass("col-sm-12");
        }

        _pesquisaPlanejamentoPedido.ExibirDisponibilidade.text("Ocultar Disponibilidade");
        recarregarGridPlanejamentoPedidoDisponibilidade();
    }
    else {
        $(".container-disponibilidade-pedido-fixar").hide();

        if (_pesquisaPlanejamentoPedido.ExibirDisponibilidadeMotorista.val()) {
            $("#container-pedido").addClass("col-sm-10");
            $("#container-pedido").removeClass("col-sm-8");
            $("#container-disponibilidade").removeClass("col-sm-4");
            $("#container-disponibilidade").addClass("col-sm-2");
            $(".container-disponibilidade-pedido-fixar").removeClass("col-sm-6");

            $(".container-disponibilidade-motorista-pedido-fixar").removeClass("col-sm-6");
            $(".container-disponibilidade-motorista-pedido-fixar").addClass("col-sm-12");
        }
        else {
            $("#container-disponibilidade").hide();
            $("#container-pedido").removeClass("col-sm-10");
            $("#container-disponibilidade").removeClass("col-sm-2");
            $(".container-disponibilidade-pedido-fixar").removeClass("col-sm-12");
        }

        _pesquisaPlanejamentoPedido.ExibirDisponibilidade.text("Exibir Disponibilidade");
    }
}

function editarColunaPlanejamentoPedidoDisponibilidadeRetorno(disponibilidadeSelecionada, linhaSelecionada, cabecalho, callbackTabPress, knout) {
    var dadosAtualizar = {
        Codigo: disponibilidadeSelecionada.Codigo,
        Observacao: disponibilidadeSelecionada.Observacao
    };

    executarReST("PlanejamentoPedido/AlterarDadosDisponibilidade", dadosAtualizar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                knout.GridPlanejamentoPedidoDisponibilidade.AtualizarDataRow(linhaSelecionada, retorno.Data, callbackTabPress);
            }
            else {
                knout.GridPlanejamentoPedidoDisponibilidade.DesfazerAlteracaoDataRow(linhaSelecionada);
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
            }
        }
        else {
            knout.GridPlanejamentoPedidoDisponibilidade.DesfazerAlteracaoDataRow(linhaSelecionada);
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
        }
    });
}