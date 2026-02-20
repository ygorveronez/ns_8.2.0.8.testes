/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />


var _gridFiliais;

var Filial = function () {
    this.ListaFiliais = PropertyEntity({ type: types.map, required: false, text: "Adicionar Filial", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadGridConfiguracaoNCPendenteFilial() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerFilial, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridFiliais = new BasicDataTable(_configuracaoNCPendenteFilial.ListaFiliais.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarFilial(_configuracaoNCPendenteFilial.ListaFiliais, null, _gridFiliais);
    
    _gridFiliais.CarregarGrid([]);
}

function obterListaFiliais() {
    return _gridFiliais.BuscarRegistros();
}

function obterListaFilialSalvar() {
    var listaFiliais = obterListaFiliais();
    var listarFiliaisRetornar = new Array();

    listaFiliais.forEach(function (filial) {
        listarFiliaisRetornar.push(Number(filial.Codigo))
    });
    
    return JSON.stringify(listarFiliaisRetornar);
}

function removerFilial(registroSelecionado) {
    var listaFilial = obterListaFiliais();

    for (var i = 0; i < listaFilial.length; i++) {
        if (registroSelecionado.Codigo == listaFilial[i].Codigo) {
            listaFilial.splice(i, 1);
            break;
        }
    }

    _gridFiliais.CarregarGrid(listaFilial);
}

