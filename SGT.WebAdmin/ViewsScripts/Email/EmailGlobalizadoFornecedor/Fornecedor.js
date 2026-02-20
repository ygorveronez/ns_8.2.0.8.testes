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
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="EmailGlobalizadoFornecedor.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFornecedorEmailFornecedor;
var _fornecedorEmailFornecedor;

var FornecedorEmailFornecedor = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Fornecedor = PropertyEntity({ type: types.event, text: "Adicionar Fornecedor", idBtnSearch: guid(), enable: ko.observable(true) });
};

//*******EVENTOS*******

function LoadFornecedorEmailFornecedor() {
    _fornecedorEmailFornecedor = new FornecedorEmailFornecedor();
    KoBindings(_fornecedorEmailFornecedor, "knockoutFornecedorEmailFornecedor");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirFornecedorEmailFornecedorClick(data) } }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridFornecedorEmailFornecedor = new BasicDataTable(_fornecedorEmailFornecedor.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_fornecedorEmailFornecedor.Fornecedor, null, null, [EnumModalidadePessoa.Fornecedor], null, _gridFornecedorEmailFornecedor);
    _fornecedorEmailFornecedor.Fornecedor.basicTable = _gridFornecedorEmailFornecedor;
    _gridFornecedorEmailFornecedor.CarregarGrid([]);
}

function ExcluirFornecedorEmailFornecedorClick(data) {
    var fornecedorEmailFornecedorGrid = _fornecedorEmailFornecedor.Fornecedor.basicTable.BuscarRegistros();

    for (var i = 0; i < fornecedorEmailFornecedorGrid.length; i++) {
        if (data.Codigo == fornecedorEmailFornecedorGrid[i].Codigo) {
            fornecedorEmailFornecedorGrid.splice(i, 1);
            break;
        }
    }

    _fornecedorEmailFornecedor.Fornecedor.basicTable.CarregarGrid(fornecedorEmailFornecedorGrid);
}

function ObterFornecedorEmailFornecedorSalvar() {
    var fornecedorEmailFornecedor = _fornecedorEmailFornecedor.Fornecedor.basicTable.BuscarRegistros();
    var fornecedorEmailFornecedorRetornar = new Array();

    for (var i = 0; i < fornecedorEmailFornecedor.length; i++) {
        fornecedorEmailFornecedorRetornar.push({
            Codigo: fornecedorEmailFornecedor[i].Codigo
        });
    }

    return JSON.stringify(fornecedorEmailFornecedorRetornar);
}

function preencherListaFornecedorEmailFornecedor(data) {
    _gridFornecedorEmailFornecedor.CarregarGrid(data.Fornecedores);
}

function LimparCamposFornecedorEmailFornecedor() {
    _gridFornecedorEmailFornecedor.CarregarGrid([]);
}

function possuiFornecedoresInformados() {
    var fornecedorEmailFornecedor = _fornecedorEmailFornecedor.Fornecedor.basicTable.BuscarRegistros();
    return fornecedorEmailFornecedor.length > 0;
}