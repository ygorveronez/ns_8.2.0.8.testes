/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

var _gridPedidosOutrasCargas;
var _pedidosOutrasCargas;

var PedidosOutrasCargas = function () {
    this.Pedidos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
}

function LoadPedidosOutrasCargas() {
    _pedidosOutrasCargas = new PedidosOutrasCargas();
    KoBindings(_pedidosOutrasCargas, "knockoutPedidosOutrasCargas");

    var ordenacao = { column: 2, dir: orderDir.asc };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DT_RowId", visible: false },
        { data: "NumeroPedido", title: "Pedido", width: "10%", className: "text-align-left", orderable: false },
        { data: "Destintario", title: "Cliente", width: "30%", className: "text-align-left", orderable: false },
        { data: "Carga", title: "Carga(s)", width: "40%", className: "text-align-left", orderable: false },
    ];

    _gridPedidosOutrasCargas = new BasicDataTable(_pedidosOutrasCargas.Pedidos.idGrid, header, null, ordenacao, null, 10, null, null, null);

    renderizarGridPedidosOutrasCargas();
}


function renderizarGridPedidosOutrasCargas() {
    _gridPedidosOutrasCargas.CarregarGrid(_pedidosOutrasCargas.Pedidos.val());
}


function BuscarPedidoOutraCarga(dados) {

    executarReST("Pedido/ObterPedidosOutrasCargas", { Carga: dados.CodigoCarga.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                _pedidosOutrasCargas.Pedidos.val(arg.Data);
                renderizarGridPedidosOutrasCargas();

                Global.abrirModal("divModalPedidosOutrasCargas");

                $("#divModalPedidosOutrasCargas").on('hidden.bs.modal', function () { LimparCampos(_pedidosOutrasCargas); });
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

