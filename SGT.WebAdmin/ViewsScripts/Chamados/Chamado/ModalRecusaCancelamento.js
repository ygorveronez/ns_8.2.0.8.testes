/// <reference path="../../Consultas/MotivoRecusaCancelamento.js" />
/// <reference path="Analise.js" />

var _modalMotivoRecusaCancelamento;
var _chamadoOcorrenciaModalMotivoRecusa;

var ModalMotivoRecusaCancelamento = function () {
    this.MotivoRecusaCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true) });
    this.Recusar = PropertyEntity({ type: types.event, eventClick: confirmacaoRecusarChamadoClick, text: "Confirmar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ type: types.event, eventClick: confirmarCancelarChamadoClick, text: "Confirmar", visible: ko.observable(false) });
    this.Fechar = PropertyEntity({ type: types.event, eventClick: cancelarRecusaClick, text: "Fechar" });
};

function loadModalMotivoRecusaCancelamento(recusa, cancelamento) {
    _modalMotivoRecusaCancelamento = new ModalMotivoRecusaCancelamento();

    KoBindings(_modalMotivoRecusaCancelamento, "knockoutMotivoRecusa", false);
    var titulo = "";
    if (recusa)
        titulo = "Pesquisa Motivos Recusa";

    if (cancelamento)
        titulo = "Pesquisa Motivos Cancelamento";

    _modalMotivoRecusaCancelamento.Cancelar.visible(cancelamento);
    _modalMotivoRecusaCancelamento.Recusar.visible(recusa);

    new BuscarMotivoRecusaCancelamento(_modalMotivoRecusaCancelamento.MotivoRecusaCancelamento, titulo, null, cancelamento, recusa);

    _chamadoOcorrenciaModalMotivoRecusa = new bootstrap.Modal(document.getElementById("knockoutMotivoRecusa"), { backdrop: 'static' });
}

function cancelarRecusaClick() {

    LimparCampos(_modalMotivoRecusaCancelamento);

    _chamadoOcorrenciaModalMotivoRecusa.hide();
}