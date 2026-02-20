var _gridTransportadorTerceiro = null, _transportadorTerceiro;

var TransportadorTerceiro = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.TransportadorTerceiro = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarTerceiro, idBtnSearch: guid() });
};


function LoadTransportadoresTerceiros() {

    _transportadorTerceiro = new TransportadorTerceiro();
    KoBindings(_transportadorTerceiro, "knockoutTransportadoresTerceiros");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: function (data) {
                ExcluirTransportadorTerceiroClick(_transportadorTerceiro.TransportadorTerceiro, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "80%" }
    ];

    _gridTransportadorTerceiro = new BasicDataTable(_transportadorTerceiro.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_transportadorTerceiro.TransportadorTerceiro, null, false, [EnumModalidadePessoa.TransportadorTerceiro], null, _gridTransportadorTerceiro);

    _transportadorTerceiro.TransportadorTerceiro.basicTable = _gridTransportadorTerceiro;
    _transportadorTerceiro.TransportadorTerceiro.basicTable.CarregarGrid(new Array());
}

function RecarregarGridTransportadorTerceiro() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.TransportadoresTerceiros.val())) {

        $.each(_tabelaFrete.TransportadoresTerceiros.val(), function (i, transportador) {
            var transportadorGrid = new Object();

            transportadorGrid.Codigo = transportador.Codigo;
            transportadorGrid.Descricao = transportador.Descricao;

            data.push(transportadorGrid);
        });
    }

    _gridTransportadorTerceiro.CarregarGrid(data);
}


function ExcluirTransportadorTerceiroClick(knoutTransportadorTerceiro, data) {
    var transportadorTerceiroGrid = knoutTransportadorTerceiro.basicTable.BuscarRegistros();

    for (var i = 0; i < transportadorTerceiroGrid.length; i++) {
        if (data.Codigo == transportadorTerceiroGrid[i].Codigo) {
            transportadorTerceiroGrid.splice(i, 1);
            break;
        }
    }

    knoutTransportadorTerceiro.basicTable.CarregarGrid(transportadorTerceiroGrid);
}

function LimparCamposTransportadorTerceiro() {
    LimparCampos(_transportadorTerceiro);
}