/// <reference path="Motorista.js" />
/// <reference path="../../Consultas/Cliente.js"/>

//*******MAPEAMENTO KNOUCKOUT*******

var _gridLocaisCarregamentoAutorizados;

//*******EVENTOS*******

function loadLocaisCarregamentoAutorizados() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                excluirLocaisCarregamentoAutorizados(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Cliente", width: "80%", className: "text-align-left" }
    ];

    _gridLocaisCarregamentoAutorizados = new BasicDataTable(_motorista.GridLocaisCarregamentoAutorizados.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    new BuscarClientes(_motorista.LocaisCarregamentoAutorizados, null, null, null, null, _gridLocaisCarregamentoAutorizados);
    _motorista.LocaisCarregamentoAutorizados.basicTable = _gridLocaisCarregamentoAutorizados;

    recarregarGridLocaisCarregamentoAutorizados();
}

function recarregarGridLocaisCarregamentoAutorizados() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_motorista.LocaisCarregamentosAutorizados.val())) {
        $.each(_motorista.LocaisCarregamentosAutorizados.val(), function (i, locaisCarregamentoAutorizados) {
            var obj = new Object();

            obj.Codigo = locaisCarregamentoAutorizados.Codigo;
            obj.Descricao = locaisCarregamentoAutorizados.Descricao;

            data.push(obj);
        });
    }

    _gridLocaisCarregamentoAutorizados.CarregarGrid(data);
    controlarExibicaoLocaisCarregamentoAutorizados();
}

function excluirLocaisCarregamentoAutorizados(data) {
    var locaisCarregamentoAutorizadosGrid = _motorista.LocaisCarregamentoAutorizados.basicTable.BuscarRegistros();

    for (var i = 0; i < locaisCarregamentoAutorizadosGrid.length; i++) {
        if (data.Codigo == locaisCarregamentoAutorizadosGrid[i].Codigo) {
            locaisCarregamentoAutorizadosGrid.splice(i, 1);
            break;
        }
    }

    _motorista.LocaisCarregamentoAutorizados.basicTable.CarregarGrid(locaisCarregamentoAutorizadosGrid);
}

function LimparCamposLocaisCarregamentoAutorizados() {
    _motorista.LocaisCarregamentoAutorizados.basicTable.CarregarGrid(new Array());
}

function controlarExibicaoLocaisCarregamentoAutorizados() {
    if (_motorista.RestringirLocaisCarregamentoAutorizadosMotoristas.val())
        $("#liTabLocaisCarregamentoAutorizados").show();
    else
        $("#liTabLocaisCarregamentoAutorizados").hide();
}