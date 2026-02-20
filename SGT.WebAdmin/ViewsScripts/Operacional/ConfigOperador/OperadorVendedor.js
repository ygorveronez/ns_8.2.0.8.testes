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
/// <reference path="ConfigOperador.js" />
/// <reference path="../../Consultas/Tranportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridVendedor;
var _vendedor;

var Vendedor = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Vendedor = PropertyEntity({ type: types.event, text: Localization.Resources.Operacional.ConfigOperador.Adicionar, idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadVendedor() {
    _vendedor = new Vendedor();
    KoBindings(_vendedor, "knockoutOperadorVendedor");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: function (data) {
                ExcluirVendedorClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "80%" }
    ];

    _gridVendedor = new BasicDataTable(_vendedor.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarFuncionario(_vendedor.Vendedor, null, _gridVendedor);
    _vendedor.Vendedor.basicTable = _gridVendedor;

    RecarregarGridVendedor();
}

function RecarregarGridVendedor() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_operador.Vendedores.val())) {
        $.each(_operador.Vendedores.val(), function (i, vendedor) {
            var vendedorGrid = new Object();

            vendedorGrid.Codigo = vendedor.Codigo;
            vendedorGrid.Descricao = vendedor.Descricao;

            data.push(vendedorGrid);
        });
    }

    _gridVendedor.CarregarGrid(data);
}

function ExcluirVendedorClick(data) {
    var vendedorGrid = _vendedor.Vendedor.basicTable.BuscarRegistros();

    for (var i = 0; i < vendedorGrid.length; i++) {
        if (data.Codigo == vendedorGrid[i].Codigo) {
            vendedorGrid.splice(i, 1);
            break;
        }
    }

    _vendedor.Vendedor.basicTable.CarregarGrid(vendedorGrid);
}

function LimparCamposVendedor() {
    LimparCampos(_vendedor);
    _vendedor.Vendedor.basicTable.CarregarGrid(new Array());
}