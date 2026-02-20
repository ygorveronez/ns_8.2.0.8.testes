/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAnexo;

/*
 * Declaração das Funções de Inicialização
 */

function loadAnexo() {
    loadGridAnexo();
}

function loadGridAnexo() {
    var linhasPorPaginas = 7;
    var opcaoDownload = { descricao: "Download", id: guid(), evento: "onclick", metodo: downloadAnexoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoDownload] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable("grid_licitacao_anexo", header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function downloadAnexoClick(registroSelecionado) {
    executarDownload("LicitacaoAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

/*
 * Declaração das Funções
 */

function recarregarGridAnexo(anexos) {
    _gridAnexo.CarregarGrid(anexos);
}
