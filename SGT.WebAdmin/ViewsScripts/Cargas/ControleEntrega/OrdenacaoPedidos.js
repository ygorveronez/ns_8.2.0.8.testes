/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _gridOrdenacaoPedidos;
var _ordenacaoPedidos;

var OrdenacaoPedido = function () {
    this.Pedidos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
}

function loadOrdenacaoPedidos() {
    _ordenacaoPedidos = new OrdenacaoPedido();
    KoBindings(_ordenacaoPedidos, "knockoutOrdenacaoPedidos");

    var ordenacao = { column: 2, dir: orderDir.asc };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DT_RowId", visible: false },
        { data: "Ordem", title: Localization.Resources.Cargas.ControleEntrega.Ordem, width: "25%", className: "text-align-right", orderable: false },
        { data: "Cliente", title: Localization.Resources.Cargas.ControleEntrega.Cliente, width: "25%", className: "text-align-left", orderable: false },
        { data: "Cidade", title: Localization.Resources.Cargas.ControleEntrega.Cidade, width: "25%", className: "text-align-left", orderable: false },
        { data: "Estado", title: Localization.Resources.Cargas.ControleEntrega.Estado, width: "25%", className: "text-align-left", orderable: false }
    ];

    _gridOrdenacaoPedidos = new BasicDataTable(_ordenacaoPedidos.Pedidos.idGrid, header, null, ordenacao, null, 10, null, null, null, ajustarOrdemEntregas);

    renderizarGridOrdenacaoPedidos();
}

function renderizarGridOrdenacaoPedidos() {
    _gridOrdenacaoPedidos.CarregarGrid(_ordenacaoPedidos.Pedidos.val());
}

function buscarEntregasDaCarga() {
    executarReST("CargaEntrega/BuscarPorCarga", { Carga: _pedidoEntrega.Carga.val() }, function (retorno) {
        _ordenacaoPedidos.Pedidos.val(retorno.Data);
        renderizarGridOrdenacaoPedidos();

        Global.abrirModal("divModalOrdenacaoPedidos");
        $("#divModalOrdenacaoPedidos").on('hidden.bs.modal', function () { LimparCampos(_ordenacaoPedidos); });
    });
}

function ajustarOrdemEntregas(retornoOrdenacao, reverterOrdenacao) {
    var listaRegistros = _gridOrdenacaoPedidos.BuscarRegistros();
    _ordenacaoPedidos.Pedidos.val([]);
    for (var i = 0; i < retornoOrdenacao.listaRegistrosReordenada.length; i++) {
        var registroReordenado = retornoOrdenacao.listaRegistrosReordenada[i];
        for (var j = 0; j < listaRegistros.length; j++) {
            var registro = listaRegistros[j];
            if (registro.DT_RowId == registroReordenado.idLinha) {
                registro.Ordem = registroReordenado.posicao;
                _ordenacaoPedidos.Pedidos.val().push(registro);
                break;
            }
        }
    }

    renderizarGridOrdenacaoPedidos();

    var data = {
        Codigo: retornoOrdenacao.itemReordenado.idLinha,
        NovaOrdem: retornoOrdenacao.itemReordenado.posicaoAtual
    };

    executarReST("CargaEntrega/ReordenarEntregas", data, function (retorno) {
        if (retorno.Success) {

        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Cargas.ControleEntrega.OcorreuUmErroAoReordenarAsEntregas);
            reverterOrdenacao();
        }
    }, null, true);

}