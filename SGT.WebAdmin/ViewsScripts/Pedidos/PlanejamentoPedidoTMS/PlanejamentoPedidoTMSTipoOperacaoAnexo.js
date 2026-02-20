var _anexo;

var PlanejamentoPedidoTMSTipoOperacaoAnexo = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.Anexos.val.subscribe(RenderizarGridAnexos);
}

function loadPlanejamentoPedidoTMSTipoOperacaoAnexo() {
    _anexo = new PlanejamentoPedidoTMSTipoOperacaoAnexo();
    KoBindings(_anexo, "knoutAnexo");
    loadGridAnexo();
}

function loadGridAnexo() {
    var linhasPorPaginas = 4;
    var opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 20, opcoes: [opcaoDownload] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "40%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "25%", className: "text-align-left" }
    ];

    _gridAnexo = new BasicDataTable(_anexo.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexo.CarregarGrid([]);
}

function downloadAnexoClick(registroSelecionado) {
    executarDownload("TipoOperacaoAnexo/DownloadAnexo", { Codigo: registroSelecionado.Codigo });
}

function GetAnexos() {
    return _anexo.Anexos.val().slice();
}

function RenderizarGridAnexos() {
    var anexos = GetAnexos();
    _gridAnexo.CarregarGrid(anexos);
}

function PreencherGridAnexos(data) {
    _anexo.Anexos.val(data.AnexosTipoOperacao);
}