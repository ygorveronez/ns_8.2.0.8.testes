/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="OrdenacaoPedidos.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _pedidoEntrega;
var _buscaPedidosReentrega;
var _buscaPedidosEntrega;


/*
 * Declaração das Classes
 */
var PedidoEntrega = function () {
    this.Pedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idBtnSearch: guid() });
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, codEntity: ko.observable(0), getType: typesKnockout.int });
}


/*
 * Declaração das Funções de Inicialização
 */
function loadAdicionarPedidoEntrega() {
    _pedidoEntrega = new PedidoEntrega();

    _buscaPedidosReentrega = new BuscarPedidosReentregaControleEntrega(_pedidoEntrega.Pedido, PedidoReentregaSelecionado);
    _buscaPedidosEntrega = new BuscarPedidoEntregaControleEntrega(_pedidoEntrega.Pedido, PedidoEntregaSelecionado, _pedidoEntrega.Carga);

    loadOrdenacaoPedidos();
}


/*
 * Declaração das Funções Associadas a Eventos
 */
function adicionarPedidoEntregaClick() {
    var carga = _etapaAtualFluxo.Carga.val();
    _pedidoEntrega.Carga.val(carga);

    exibirModalAdicionarPedidoEntrega();
}

function adicionarPedidoReentregaClick() {
    var carga = _etapaAtualFluxo.Carga.val();
    _pedidoEntrega.Carga.val(carga);

    exibirModalAdicionarPedidoReentrega();
}

function exibirModalAdicionarPedidoEntrega() {
    _buscaPedidosEntrega.AbrirBusca();
}

function exibirModalAdicionarPedidoReentrega() {
    _buscaPedidosReentrega.AbrirBusca();
}

/*
 * Declaração das Funções Privadas
 */
function PedidoEntregaSelecionado(pedido) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.VoceRealmenteDesejaAdicionarPedido.format(pedido.NumeroPedidoEmbarcador), function () {
        _buscaPedidosEntrega.FecharBusca();

        var data = {
            Codigo: pedido.Codigo,
            Carga: _pedidoEntrega.Carga.val()
        };

        adicionarPedidoEntrega(data);
    })
}

function PedidoReentregaSelecionado(pedido) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.VoceRealmenteDesejaAdicionarPedido.format(pedido.NumeroPedidoEmbarcador), function () {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Deseja adicionar mais um pedido?", function () {

            var data = {
                Codigo: pedido.Codigo,
                Carga: _pedidoEntrega.Carga.val(),
                NumeroPedidoEmbarcador: pedido.NumeroPedidoEmbarcador
            };

            adicionarPedidoEntrega(data);
        },
            function () {
                _buscaPedidosReentrega.FecharBusca();

                var data = {
                    Codigo: pedido.Codigo,
                    Carga: _pedidoEntrega.Carga.val(),
                    NumeroPedidoEmbarcador: pedido.NumeroPedidoEmbarcador
                };

                adicionarPedidoEntrega(data, buscarEntregasDaCarga);
            })
    })
}

function adicionarPedidoEntrega(data, callback) {
    executarReST("ControleEntrega/AdicionarEntrega", data, function (arg) {
        if (arg.Success) {
            if (arg.Msg) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.ControleEntrega.PedidoAdicionado, Localization.Resources.Cargas.ControleEntrega.PedidoFoiAdicionadoComSucesso.format(data.NumeroPedidoEmbarcador));
                atualizarControleEntrega();

                if (callback != undefined)
                    callback();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}