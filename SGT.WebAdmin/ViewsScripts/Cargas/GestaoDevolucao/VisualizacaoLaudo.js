//#region Objetos Globais do Arquivo
var _gestaoDevolucaoVisualizarLaudo;
// #endregion Objetos Globais do Arquivo

//#region Classes
var VisualizarLaudo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.PdfLaudoBase64 = PropertyEntity({ val: ko.observable("") });
    this.Download = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Download, eventClick: downloadPDFLaudo, visible: ko.observable(true), enable: ko.observable(true) });
}
//#endregion Classes

// #region Funções de Inicialização
function loadVisualizarLaudo(codigoLaudo) {
    executarReST("GestaoDevolucao/BuscarPdfVisualizarLaudo", { Codigo: codigoLaudo }, function (r) {
        if (r.Success && r.Data) {
            $.get("Content/Static/Carga/GestaoDevolucao/VisualizarLaudo.html?dyn=" + guid(), function (html) {
                $("#gestaoDevolucaoVisualizarLaudo").html(html);

                _gestaoDevolucaoVisualizarLaudo = new VisualizarLaudo();
                KoBindings(_gestaoDevolucaoVisualizarLaudo, "knockoutGestaoDevolucaoVisualizarLaudo");

                PreencherObjetoKnout(_gestaoDevolucaoVisualizarLaudo, r);

                Global.abrirModal("modalVisualizarLaudo");
            });
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function downloadPDFLaudo(laudo) {
    executarDownload("GestaoDevolucao/DownloadLaudo", { Codigo: laudo.Codigo.val() });
}
// #endregion Funções Associadas a Eventos

// #region Funções Públicas
// #endregion Funções Públicas

// #region Funções Privadas
// #endregion Funções Privadas