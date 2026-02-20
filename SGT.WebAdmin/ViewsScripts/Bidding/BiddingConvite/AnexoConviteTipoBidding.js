/// <reference path="BiddingConvite.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridAnexoTipoBidding, _listaAnexoTipoBidding;

/*
 * Declaração das Classes
 */
var ListaAnexoTipoBidding = function () {
    this.Anexos = PropertyEntity({ text: "Anexos", type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.Anexos.val.subscribe(function () {
        recarregarGridAnexoTipoBidding();
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAnexoConviteTipoBidding() {
    _listaAnexoTipoBidding = new ListaAnexoTipoBidding();
    KoBindings(_listaAnexoTipoBidding, "knockoutListaAnexosConviteTipoBidding");

    loadGridAnexoTipoBidding();
}

function loadGridAnexoTipoBidding() {
    const linhasPorPaginas = 2;
    const opcaoDownload = { descricao: "Download", id: guid(), metodo: downloadAnexoTipoBiddingClick, icone: "", visibilidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoDownload] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "35%", className: "text-align-left" },
        { data: "NomeArquivo", title: "Nome", width: "30%", className: "text-align-left" }
    ];

    _gridAnexoTipoBidding = new BasicDataTable(_listaAnexoTipoBidding.Anexos.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAnexoTipoBidding.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function downloadAnexoTipoBiddingClick(registroSelecionado) {
    const dados = { Codigo: registroSelecionado.Codigo };

    executarDownload("TipoBiddingAnexo/DownloadAnexo", dados);
}

/*
 * Declaração das Funções Públicas
 */

function limparAnexoTipoBidding() {
    _listaAnexoTipoBidding.Anexos.val(new Array());
}

/*
 * Declaração das Funções Privadas
 */

function isOpcaoDownloadAnexoTipoBiddingVisivel(registroSelecionado) {
    return !isNaN(registroSelecionado.Codigo);
}

function obterAnexosConviteTipoBidding() {
    return _listaAnexoTipoBidding.Anexos.val().slice();
}

function obterFormDataAnexosTipoBidding(anexos) {
    if (anexos.length > 0) {
        const formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.Descricao);
        });

        return formData;
    }

    return undefined;
}

function recarregarGridAnexoTipoBidding() {
    const anexos = obterAnexosConviteTipoBidding();

    _gridAnexoTipoBidding.CarregarGrid(anexos);
}