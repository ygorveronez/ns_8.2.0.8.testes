/// <reference path="../../Consultas/Tranportador.js" />

var _gridTransportador;

function loadGridTransportadores() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirTransportadorClick(data);
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridTransportador = new BasicDataTable(_configuracaoDescargaCliente.Transportadores.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTransportadores(_configuracaoDescargaCliente.Transportadores, null, null, null, _gridTransportador);

    recarregarGridTransportador();
}

function recarregarGridTransportador() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_configuracaoDescargaCliente.Transportadores.val())) {

        $.each(_configuracaoDescargaCliente.Transportadores.val(), function (i, Transportador) {
            var TransportadorGrid = new Object();

            TransportadorGrid.Codigo = Transportador.Codigo;
            TransportadorGrid.Descricao = Transportador.Descricao;

            data.push(TransportadorGrid);
        });
    }

    _gridTransportador.CarregarGrid(data);
}


function excluirTransportadorClick(data) {
    var TransportadorGrid = _gridTransportador.BuscarRegistros();

    for (var i = 0; i < TransportadorGrid.length; i++) {
        if (data.Codigo == TransportadorGrid[i].Codigo) {
            TransportadorGrid.splice(i, 1);
            break;
        }
    }

    _gridTransportador.CarregarGrid(TransportadorGrid);
}