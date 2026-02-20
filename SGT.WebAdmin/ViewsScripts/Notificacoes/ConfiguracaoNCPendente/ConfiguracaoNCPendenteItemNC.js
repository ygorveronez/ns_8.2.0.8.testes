/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ItemNaoConformidade.js" />


var _gridItensNaoConformidade;

var ItemNC = function () {
    this.ListaItensNC = PropertyEntity({ type: types.map, required: false, text: "Adicionar item de não conformidade", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadGridConfiguracaoNCPendenteItemNC() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerItem, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridItensNaoConformidade = new BasicDataTable(_configuracaoNCPendenteItemNC.ListaItensNC.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarNaoConformidade(_configuracaoNCPendenteItemNC.ListaItensNC, null, _gridItensNaoConformidade);

    _gridItensNaoConformidade.CarregarGrid([]);
}

function obterListaItens() {
    return _gridItensNaoConformidade.BuscarRegistros();
}

function obterListaItensSalvar() {
    var listaItensNC = obterListaItens();
    var listaItensNCRetornar = new Array();

    listaItensNC.forEach(function (item) {
        listaItensNCRetornar.push(Number(item.Codigo))
    });
    
    return JSON.stringify(listaItensNCRetornar);
}

function removerItem(registroSelecionado) {
    var listaItensNC = obterListaItens();

    for (var i = 0; i < listaItensNC.length; i++) {
        if (registroSelecionado.Codigo == listaItensNC[i].Codigo) {
            listaItensNC.splice(i, 1);
            break;
        }
    }

    _gridItensNaoConformidade.CarregarGrid(listaItensNC);
}
