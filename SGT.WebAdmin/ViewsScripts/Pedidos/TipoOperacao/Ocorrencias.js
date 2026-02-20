var _gridOcorrencias;

function buscarOcorrencias() {
    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), evento: "onclick", metodo: function (data) { excluirOcorrencia(_gridOcorrencias, data) }, tamanho: "20", icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [excluir], tamanho: 5 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%" },
    ];
    _gridOcorrencias = new BasicDataTable(_tipoOperacao.Ocorrencias.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridOcorrencias.CarregarGrid([]);
    _tipoOperacao.Ocorrencias.BasicDataTable = _gridOcorrencias;
}

function obterOcorrenciaSalvar() {
    var listaOcorrencia = _tipoOperacao.Ocorrencias.BasicDataTable.BuscarRegistros();
    var listaOcorrenciaRetornar = new Array();

    for (var i = 0; i < listaOcorrencia.length; i++) {
        listaOcorrenciaRetornar.push({
            Codigo: listaOcorrencia[i].Codigo
        });
    }

    return JSON.stringify(listaOcorrencia);
}

function excluirOcorrencia(knout, data) {
    var dados = knout.BuscarRegistros();

    for (var i = 0; i < dados.length; i++) {
        if (data.Codigo == dados[i].Codigo) {
            dados.splice(i, 1);
            break;
        }
    }

    knout.CarregarGrid(dados);
}

function preencherListaOcorrencias( data) {
    _gridOcorrencias.CarregarGrid(data.TiposOcorrencia);
}