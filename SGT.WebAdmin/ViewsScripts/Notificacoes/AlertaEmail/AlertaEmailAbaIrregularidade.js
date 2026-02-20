/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Irregularidade.js" />

var _gridAlertaEmailIrregularidade;

var Irregularidade = function () {
    this.ListaIrregularidade = PropertyEntity({ type: types.map, required: false, text: "Adicionar Irregularidade", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}


function loadGridAlertaEmailIrregularidade() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerAlertaEmailIrregularidade, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridAlertaEmailIrregularidade = new BasicDataTable(_irregularidadeAlertaEmail.ListaIrregularidade.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarIrregularidades(_irregularidadeAlertaEmail.ListaIrregularidade, null, _gridAlertaEmailIrregularidade);
    _irregularidadeAlertaEmail.ListaIrregularidade.basicTable = _gridAlertaEmailIrregularidade;

    _gridAlertaEmailIrregularidade.CarregarGrid([]);
}


function obterListaIrregularidade() {
    return _gridAlertaEmailIrregularidade.BuscarRegistros();
}

function obterListaIrregularidadeSalvar() {
    var listaIrregularidade = obterListaIrregularidade();
    var listaIrregularidadeRetornar = new Array();

    listaIrregularidade.forEach(function (setor) {
        listaIrregularidadeRetornar.push(Number(setor.Codigo))
    });

    return JSON.stringify(listaIrregularidadeRetornar);
}

function removerAlertaEmailIrregularidade(registroSelecionado) {
    var listaIrregularidade = obterListaIrregularidade();

    for (var i = 0; i < listaIrregularidade.length; i++) {
        if (registroSelecionado.Codigo == listaIrregularidade[i].Codigo) {
            listaIrregularidade.splice(i, 1);
            break;
        }
    }

    _gridAlertaEmailIrregularidade.CarregarGrid(listaIrregularidade);
}

function recarregarGridAlertaEmail() {
    _gridAlertaEmail.CarregarGrid();
}