//*******MAPEAMENTO KNOUCKOUT*******

var _observacaoMDFeManual;

var ObservacaoMDFeManual = function () {
    this.ObservacaoFisco = PropertyEntity({ text: "Observação ao Fisco:", maxlength: 2000, enable: ko.observable(true), visible: ko.observable(true), required: false });
    this.ObservacaoContribuinte = PropertyEntity({ text: "Observação do Contribuinte:", maxlength: 5000, enable: ko.observable(true), visible: ko.observable(true), required: false });
};

function LoadObservacao() {

    _observacaoMDFeManual = new ObservacaoMDFeManual();
    KoBindings(_observacaoMDFeManual, "tabObservacao");
}

function AtualizarObservacaoMDFeManual(retorno) {
    _observacaoMDFeManual.ObservacaoFisco.val(retorno.ObservacaoFisco);
    _observacaoMDFeManual.ObservacaoContribuinte.val(retorno.ObservacaoContribuinte);
}