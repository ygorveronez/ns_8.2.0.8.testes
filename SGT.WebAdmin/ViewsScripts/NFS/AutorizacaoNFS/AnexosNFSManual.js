//#region Declaração de Variáveis Globais do Arquivo

var _gridAnexos;
var _listaAnexos;

//#endregion

//#region Declarações dos Objetos

var ListaAnexos = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGrid();
    });
}

//#endregion

//#region Inicializadores

function loadAnexos() {
    _listaAnexos = new ListaAnexos();
    KoBindings(_listaAnexos, "knockoutListaAnexos");
    LoadGridAnexo();
}

function LoadGridAnexo() {
    var linhasPorPaginas = 5;

    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexo, icone: "", visibilidade: true };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexos = new BasicDataTable(_listaAnexos.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexos.CarregarGrid([]);
}

//#endregion

//#region Eventos

function downloadAnexo(registroSelecionado) {
    var dados = { Codigo: registroSelecionado.Codigo };
    executarDownload("AnexoLancamentoNFSManual/DownloadAnexo", dados);
}

//#endregion

//#region Funções privadas

function obterListaAnexos() {
    return _listaAnexos.Anexos.val().slice();
}

function recarregarGrid() {
    var anexos = obterListaAnexos();

    _gridAnexos.CarregarGrid(anexos);
}

//#endregion