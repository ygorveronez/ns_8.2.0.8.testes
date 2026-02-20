/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />


var _gridAlertaEmailSetor;

var Setor = function () {
    this.ListaSetor = PropertyEntity({ type: types.map, required: false, text: "Adicionar Setor", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadGridAlertaEmailSetor() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAlertaEmailSetor, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridAlertaEmailSetor = new BasicDataTable(_setorAlertaEmail.ListaSetor.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarSetorFuncionario(_setorAlertaEmail.ListaSetor, null, null, _gridAlertaEmailSetor);
    _setorAlertaEmail.ListaSetor.basicTable = _gridAlertaEmailSetor;

    _gridAlertaEmailSetor.CarregarGrid([]);
}

function obterListaSetor() {
    return _gridAlertaEmailSetor.BuscarRegistros();
}

function obterListaSetorSalvar() {
    var listaSetor = obterListaSetor();
    var listaSetorRetornar = new Array();

    listaSetor.forEach(function (setor) {
        listaSetorRetornar.push(Number(setor.Codigo))
    });
    
    return JSON.stringify(listaSetorRetornar);
}

function removerAlertaEmailSetor(registroSelecionado) {
    var listaSetor = obterListaSetor();

    for (var i = 0; i < listaSetor.length; i++) {
        if (registroSelecionado.Codigo == listaSetor[i].Codigo) {
            listaSetor.splice(i, 1);
            break;
        }
    }

    _gridAlertaEmailSetor.CarregarGrid(listaSetor);
}

function recarregarGridAlertaEmail() {
    _gridAlertaEmail.CarregarGrid();
}