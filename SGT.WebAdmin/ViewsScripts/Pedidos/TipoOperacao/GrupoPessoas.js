/// <reference path="TipoOperacao.js" />

var _gridGrupoTomadoresBloqueados;

function loadGrupoTomadoresBloqueados() {
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: function (data) { excluirGrupoTomadoresBloqueados(_gridGrupoTomadoresBloqueados, data) }, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir], tamanho: 5 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" },
    ];
    _gridGrupoTomadoresBloqueados = new BasicDataTable(_tipoOperacao.GrupoTomadoresBloqueados.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridGrupoTomadoresBloqueados.CarregarGrid([]);
    _tipoOperacao.GrupoTomadoresBloqueados.BasicDataTable = _gridGrupoTomadoresBloqueados;

    new BuscarGruposPessoas(_tipoOperacao.GrupoPessoasTomadoresBloqueados, null, null, _gridGrupoTomadoresBloqueados);
}

function obterGrupoTomadoresBloqueadosSalvar() {
    var listaGrupoTomadoresBloqueados = _tipoOperacao.GrupoTomadoresBloqueados.BasicDataTable.BuscarRegistros();
    var listaGrupoTomadoresBloqueadosRetornar = new Array();

    for (var i = 0; i < listaGrupoTomadoresBloqueados.length; i++) {
        listaGrupoTomadoresBloqueadosRetornar.push({
            Codigo: listaGrupoTomadoresBloqueados[i].Codigo
        });
    }

    return JSON.stringify(listaGrupoTomadoresBloqueadosRetornar);
}

function excluirGrupoTomadoresBloqueados(knout, data) {
    var dados = knout.BuscarRegistros();

    for (var i = 0; i < dados.length; i++) {
        if (data.Codigo == dados[i].Codigo) {
            dados.splice(i, 1);
            break;
        }
    }

    knout.CarregarGrid(dados);
}

function preencherListaGrupoTomadoresBloqueados(data) {
    _gridGrupoTomadoresBloqueados.CarregarGrid(data.GrupoTomadoresBloqueados);
}

function limparListaGrupoTomadoresBloqueados() {
    _gridGrupoTomadoresBloqueados.CarregarGrid([]);
}