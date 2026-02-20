/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Pessoa.js" />

//#region
var _filiaisCliente;
var _gridfiliaisCliente;
//#endregion

//#region Construstores

var FilialCliente = function () {

    this.FilialCliente = PropertyEntity({ type: types.local });
    this.AdicionarPessoa = PropertyEntity({ idBtnSearch: guid(), type: types.event, text: "Filial Cliente", visible: ko.observable(true), enable: ko.observable(true) });
}


function loadFilialCliente() {
    _filiaisCliente = new FilialCliente();
    KoBindings(_filiaisCliente, "knoutFilialCliente");

    loadGridFilialCliente();
    new BuscarClientes(_filiaisCliente.AdicionarPessoa, null, null, null, null, _gridfiliaisCliente);

}

function loadGridFilialCliente() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirRegistroGrid }]
    };

    _gridfiliaisCliente = new BasicDataTable(_filiaisCliente.FilialCliente.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _filiaisCliente.FilialCliente.basicTable = _gridfiliaisCliente;

    recarregarGridFilialCliente();
}
//#endregion

//#region Funções Auxiliares


function limparCamposFilialCliente() {
    LimparCampos(_filiaisCliente);
}

function recarregarGridFilialCliente() {
    const data = new Array();
    let filiaisCliente = _pessoa.FilialCliente.val();

    if (!string.IsNullOrWhiteSpace(filiaisCliente)) {
        $.each(filiaisCliente, function (i, item) {
            var filialGrid = new Object();

            filialGrid.Codigo = item.Codigo;
            filialGrid.Descricao = item.Descricao;

            data.push(filialGrid);
        });
    }

    _gridfiliaisCliente.CarregarGrid(data);
}

function excluirRegistroGrid(data) {
    var listaFilialCliente = _filiaisCliente.FilialCliente.basicTable.BuscarRegistros();

    for (var i = 0; i < listaFilialCliente.length; i++) {
        if (data.Codigo != listaFilialCliente[i].Codigo)
            continue;

        listaFilialCliente.splice(i, 1);
        break;
    }

    _filiaisCliente.FilialCliente.basicTable.CarregarGrid(listaFilialCliente);
}

function ObterRegistrosFilialCliente() {
    return _filiaisCliente.FilialCliente.basicTable.BuscarRegistros();
}
//#endRegion