/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />


var _gridTipoOperacao;

var TipoOperacao = function () {
    this.ListaTipoOperacao = PropertyEntity({ type: types.map, required: false, text: "Adicionar Tipo de Operação", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });
}

function loadGridConfiguracaoNCPendenteTipoOperacao() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: "Remover", id: guid(), metodo: removerTipoOperacao, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%", className: "text-align-left" }
    ];

    _gridTipoOperacao = new BasicDataTable(_configuracaoNCPendenteTipoOperacao.ListaTipoOperacao.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarTiposOperacao(_configuracaoNCPendenteTipoOperacao.ListaTipoOperacao, null, null, null, _gridTipoOperacao);

    _gridTipoOperacao.CarregarGrid([]);
}

function obterListaTipoOperacao() {
    return _gridTipoOperacao.BuscarRegistros();
}

function obterListaTipoOperacaoSalvar() {
    var listaTipoOperacao = obterListaTipoOperacao();
    var listaTipoOperacaoRetornar = new Array();

    listaTipoOperacao.forEach(function (tipoOperacao) {
        listaTipoOperacaoRetornar.push(Number(tipoOperacao.Codigo))
    });
    
    return JSON.stringify(listaTipoOperacaoRetornar);
}

function removerTipoOperacao(registroSelecionado) {
    var listaTipoOperacao = obterListaTipoOperacao();

    for (var i = 0; i < listaTipoOperacao.length; i++) {
        if (registroSelecionado.Codigo == listaTipoOperacao[i].Codigo) {
            listaTipoOperacao.splice(i, 1);
            break;
        }
    }

    _gridTipoOperacao.CarregarGrid(listaTipoOperacao);
}