//*******MAPEAMENTO KNOUCKOUT*******

var DownloadOCOREN = function () {
    this.LayoutEDI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Layout:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });

    this.Download = PropertyEntity({
        eventClick: function (e) {
            DownloadOCORENClick();
        }, type: types.event, text: "Download", idGrid: guid(), icon: "fa fa-download"
    });
};
var _modalKnockoutDownloadOCOREN;

//*******EVENTOS*******

function LoadDownloadOCOREN() {
    _downloadOCOREN = new DownloadOCOREN();
    KoBindings(_downloadOCOREN, "knockoutDownloadOCOREN");

    new BuscarLayoutsEDI(_downloadOCOREN.LayoutEDI, null, null, null, null, [EnumTipoLayoutEDI.OCOREN]);

    _pesquisaCTe.LayoutOCOREN = _downloadOCOREN.LayoutEDI;
    _modalKnockoutDownloadOCOREN = new bootstrap.Modal(document.getElementById("knockoutDownloadOCOREN"), { backdrop: true, keyboard: true });
}

function AbrirTelaDownloadOCOREN() {
    _modalKnockoutDownloadOCOREN.show();
}

function DownloadOCORENClick() {
    var dados = RetornarObjetoPesquisa(_pesquisaCTe);

    executarDownload("ConsultaCTe/DownloadOCOREN", dados, function () {
        _modalKnockoutDownloadOCOREN.hide();
    });
}