/// <reference path="Motorista.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTransportadoras;

//*******EVENTOS*******

function loadTransportador() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                excluirTransportadorClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CNPJ", title: Localization.Resources.Transportadores.Motorista.CNPJ, width: "20%", className: "text-align-left" },
        { data: "RazaoSocial", title: Localization.Resources.Transportadores.Motorista.RazaoSocial, width: "70%", className: "text-align-left" }
    ];

    _gridTransportadoras = new BasicDataTable(_motorista.GridTransportadoras.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    new BuscarTransportadores(_motorista.Transportadora, null, null, null, _gridTransportadoras);
    _motorista.Transportadora.basicTable = _gridTransportadoras;

    recarregarGridTransportadoras();
}

function recarregarGridTransportadoras() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_motorista.Transportadoras.val())) {
        $.each(_motorista.Transportadoras.val(), function (i, transportador) {
            var obj = new Object();

            obj.Codigo = transportador.Codigo;
            obj.CNPJ = transportador.CNPJ;
            obj.RazaoSocial = transportador.RazaoSocial;

            data.push(obj);
        });
    }

    _gridTransportadoras.CarregarGrid(data);
}

function excluirTransportadorClick(data) {
    var transportadorGrid = _motorista.Transportadora.basicTable.BuscarRegistros();

    for (var i = 0; i < transportadorGrid.length; i++) {
        if (data.Codigo == transportadorGrid[i].Codigo) {
            transportadorGrid.splice(i, 1);
            break;
        }
    }

    _motorista.Transportadora.basicTable.CarregarGrid(transportadorGrid);
}

function LimparCamposTransportador() {
    _motorista.Transportadora.basicTable.CarregarGrid(new Array());
}