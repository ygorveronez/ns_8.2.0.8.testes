/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />

// #region Objetos Globais do Arquivo

var _visualizarDANFE;

// #endregion Objetos Globais do Arquivo

// #region Funções de Inicialização

function VisualizarDANFE() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.SemImagem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PDFViewer = PropertyEntity({ type: types.local, visible: ko.observable(false) });
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function loadVisualizarDANFE() {
    _visualizarDANFE = new VisualizarDANFE();
    KoBindings(_visualizarDANFE, "knoutVisualizarDANFE");
}

function exibirModalVisualizarDANFE(e) {
    if (!_visualizarDANFE)
        loadVisualizarDANFE();
    BuscarImagemPorCodigoDANFE(e);

    Global.abrirModal("divModalVisualizarDANFE");
}

function GerarURLRenderizacaoDANFE(codigo, cargaPedido) {
    return "Cargas/RenderizarDANFE?Codigo=" + codigo + '&CargaPedido=' + cargaPedido;
}

function BuscarImagemPorCodigoDANFE(e) {
    _visualizarDANFE.Codigo.val(e.Codigo);
    _visualizarDANFE.PDFViewer.get$().attr("src", GerarURLRenderizacaoDANFE(e.Codigo, e.CargaPedido));
    _visualizarDANFE.PDFViewer.visible(true);
}

// #endregion Funções Públicas

