/// <reference path="../../Consultas/Tranportador.js" />

//Declaração objetos globais
var _gridConvidado, _convidado;
var _convidados = new Array();

var Convidado = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Convidado = PropertyEntity({ type: types.event, text: "Buscar Convidado", idBtnSearch: guid(), visible: ko.observable(true) });
}

function LoadConvidadosRFI() {
    _convidado = new Convidado();
    KoBindings(_convidado, "knockoutConvidado");

    const menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirConvidadoClick(_convidado.Convidado, data)
            }
        }]
    };

    const header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridConvidado = new BasicDataTable(_convidado.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

    BuscarTransportadores(_convidado.Convidado, function (r) {
        if (r != null) {
            for (let i = 0; i < r.length; i++)
                _convidados.push({ Codigo: r[i].Codigo, Descricao: r[i].Descricao });

            _gridConvidado.CarregarGrid(_convidados);

        }
    }, menuOpcoes, true, _gridConvidado);

    _convidado.Convidado.basicTable = _gridConvidado;

    RecarregarGridConvidado();
}

//Funções click

function ExcluirConvidadoClick(knoutConvidado, data) {
    const convidados = knoutConvidado.basicTable.BuscarRegistros();

    for (let i = 0; i < convidados.length; i++) {
        if (data.Codigo == convidados[i].Codigo) {
            convidados.splice(i, 1);
            break;
        }
    }

    knoutConvidado.basicTable.CarregarGrid(convidados);
}

//Funções privadas
function salvarConvidados(codigo) {
    const convidados = obterConvidados();
}

function obterConvidados() {
    return _gridConvidado.BuscarRegistros();
}

function obterArrayConvidados(convidados) {
    const dadosConvidados = new Array();

    if (convidados.length > 0) {
        convidados.forEach(function (convidado) {
            dadosConvidados.push({ convidadoCodigo: convidado.Codigo });
        });
    }

    return dadosConvidados;
}

//Funções globai
function limparGridConvidados() {
    _convidados = [];
    _convidado.Convidado.basicTable.CarregarGrid([]);
}

function RecarregarGridConvidado() {
    _gridConvidado.CarregarGrid(_convidados);
}