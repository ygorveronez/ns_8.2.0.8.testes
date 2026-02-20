var _documentoTituloFinanceiro;

var DocumentoTituloFinanceiro = function () {
    this.Dropzone = PropertyEntity({ type: types.local, idTab: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ConhecimentoEletronico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.ConhecimentosFatura = PropertyEntity({ type: types.listEntity, list: new Array(), text: "Conhecimentos da Fatura:", codEntity: ko.observable(0), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(true) });

    this.PesquisarDocumentosTitulo = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentos.CarregarGrid();
        }, type: types.event, text: "Atualizar", idGrid: guid(), visible: ko.observable(false)
    });
}

function LoadDocumentoTituloFinanceiro() {
    _documentoTituloFinanceiro = new DocumentoTituloFinanceiro();
    KoBindings(_documentoTituloFinanceiro, "tabDocumentos");

    _tituloFinanceiro.Dropzone = _documentoTituloFinanceiro.Dropzone;
    _tituloFinanceiro.ConhecimentoEletronico = _documentoTituloFinanceiro.ConhecimentoEletronico;
    _tituloFinanceiro.ConhecimentosFatura = _documentoTituloFinanceiro.ConhecimentosFatura;
    _tituloFinanceiro.PesquisarDocumentosTitulo = _documentoTituloFinanceiro.PesquisarDocumentosTitulo;
}