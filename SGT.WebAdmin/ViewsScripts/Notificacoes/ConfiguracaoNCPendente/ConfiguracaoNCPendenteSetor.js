/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />


var _gridSetor;

var Setor = function () {
    this.ListaSetor = PropertyEntity({ type: types.map, required: false, text: "Adicionar Setor", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadGridConfiguracaoNCPendenteSetor() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerSetor, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridSetor = new BasicDataTable(_configuracaoNCPendenteSetor.ListaSetor.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarSetorFuncionario(_configuracaoNCPendenteSetor.ListaSetor, null, null, _gridSetor);

    _gridSetor.CarregarGrid([]);
}

function obterListaSetor() {
    return _gridSetor.BuscarRegistros();
}

function obterListaSetorSalvar() {
    var listaSetor = obterListaSetor();
    var listaSetorRetornar = new Array();

    listaSetor.forEach(function (setor) {
        listaSetorRetornar.push(Number(setor.Codigo))
    });
    
    return JSON.stringify(listaSetorRetornar);
    console.log(JSON.stringify(listaSetorRetornar))
}

function removerSetor(registroSelecionado) {
    var listaSetor = obterListaSetor();

    for (var i = 0; i < listaSetor.length; i++) {
        if (registroSelecionado.Codigo == listaSetor[i].Codigo) {
            listaSetor.splice(i, 1);
            break;
        }
    }

    _gridSetor.CarregarGrid(listaSetor);
}
