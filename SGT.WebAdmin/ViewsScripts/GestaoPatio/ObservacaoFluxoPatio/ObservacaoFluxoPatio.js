/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _observacaoFluxoPatio;

/*
 * Declaração das Classes
 */

var ObservacaoFluxoPatio = function () {
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 1000, enable: false });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadObservacaoFluxoPatio() {
    $.get("Content/Static/GestaoPatio/ObservacaoFluxoPatio.html?dyn=" + guid(), function (data) {
        $("#divContainerObservacaoFluxoPatio").html(data);

        _observacaoFluxoPatio = new ObservacaoFluxoPatio();
        KoBindings(_observacaoFluxoPatio, "knockoutObservacaoFluxoPatio");
    });
}

/*
 * Declaração das Funções Públicas
 */

function exibirObservacaoFluxoPatio(observacao) {
    _observacaoFluxoPatio.Observacao.val(observacao);

    exibirModalObservacaoFluxoPatio();
}

/*
 * Declaração das Funções Privadas
 */

function exibirModalObservacaoFluxoPatio() {
    Global.abrirModal('divModalObservacaoFluxoPatio');
    $("#divModalObservacaoFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_observacaoFluxoPatio);
    });
}
