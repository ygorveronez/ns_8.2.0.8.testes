/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="JanelaColetaUF.js" />
/// <reference path="JanelaColetaLocalidade.js" />
/// <reference path="JanelaColetaPeriodo.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridJanelaColetaCliente;
var _janelaColetaCliente;
var _listaClientes = new Array();

var JanelaColetaCliente = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Cliente = PropertyEntity({ type: types.event, text: "Adicionar Cliente", idBtnSearch: guid() });
}


//*******EVENTOS*******

function LoadJanelaColetaCliente() {
    _janelaColetaCliente = new JanelaColetaCliente();
    KoBindings(_janelaColetaCliente, "knockoutJanelaColetaCliente");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirJanelaColetaClienteClick(_janelaColetaCliente.Cliente, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridJanelaColetaCliente = new BasicDataTable(_janelaColetaCliente.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_janelaColetaCliente.Cliente, function (r) {
        if (r != null) {

            _listaClientes.push({ Codigo: r.Codigo, Descricao: r.Descricao });

            _gridJanelaColetaCliente.CarregarGrid(_listaClientes);
        }
    }, null, null, null, null);
    _janelaColetaCliente.Cliente.basicTable = _gridJanelaColetaCliente;

    RecarregarGridJanelaColetaCliente();
}

function RecarregarGridJanelaColetaCliente() {
    _gridJanelaColetaCliente.CarregarGrid(_janelaColeta.Clientes.val());


    if (_janelaColeta.Clientes.val() != "") {

        _listaClientes = new Array();

        for (var i = 0; i < _janelaColeta.Clientes.val().length; i++) {

            var item = _janelaColeta.Clientes.val()[i];
            _listaClientes.push({ Codigo: item.Codigo, Descricao: item.Descricao });
        }
    }

}

function ExcluirJanelaColetaClienteClick(knoutJanelaColetaCliente, data) {
    var JanelaColetaClienteGrid = knoutJanelaColetaCliente.basicTable.BuscarRegistros();

    for (var i = 0; i < JanelaColetaClienteGrid.length; i++) {
        if (data.Codigo == JanelaColetaClienteGrid[i].Codigo) {
            JanelaColetaClienteGrid.splice(i, 1);
            break;
        }
    }

    knoutJanelaColetaCliente.basicTable.CarregarGrid(JanelaColetaClienteGrid);
}

function LimparCamposJanelaColetaCliente() {
    LimparCampos(_janelaColetaCliente);
    _gridJanelaColetaCliente.CarregarGrid(new Array());
    _listaClientes = new Array();
}
