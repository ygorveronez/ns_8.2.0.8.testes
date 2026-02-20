/// <reference path="../../Enumeradores/EmumAprovacaoPendente.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTermoQuitacao.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

//#region Variaveis Globais
var _termoQuitacao;
var _pesquitaTermoQuitacao;
var _gridTermoQuitacao;
var _gridFiliais;
var _crudTermoQuitacao;
//#endregion


//#region Constructores

function TermoQuitacaoDocumentoPDF() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.SemImagem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PDFViewer = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.Anexos = PropertyEntity({ eventClick: gerenciarAnexosClick, type: types.event, text: "Upload Termo Assinado", visible: ko.observable(true), enable: ko.observable(true) });
    this.AssinarTermo = PropertyEntity({ text: 'Assinar Termo Digitalmente', eventClick: assinarTermoClick, type: types.event, idGrid: guid(), visible: ko.observable(true) });
}

//#endregion


//#region Funções de Carregamento
function loadTermoQuitacaoDocumentoPDF() {
    _termoQuitacaoPDF = new TermoQuitacaoDocumentoPDF();
    KoBindings(_termoQuitacaoPDF, "tabTermoQuitacao");
}
//#endregion


//#region Funções Auxiliares

function GerarURLRenderizacao(codigo) {
    return "Financeiros/RenderizarPDF?Codigo=" + codigo;
}

function BuscarImagemPorCodigo(codigo) {
    _termoQuitacaoPDF.Codigo.val();
    _termoQuitacaoPDF.PDFViewer.get$().attr("src", GerarURLRenderizacao(codigo));
    _termoQuitacaoPDF.PDFViewer.visible(true);
}


function assinarTermoClick(e) {
    console.log("Não desenvolvido");
    return;
}
