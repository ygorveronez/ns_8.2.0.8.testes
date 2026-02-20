/// <reference path="../../../Consultas/JustificativaCancelamentoFinanceiro.js" />
/// <reference path="CancelamentoTituloAnexo.js" />
/// <reference path="EtapaFaturamento.js" />

var _cancelamentoTituloCarga, _modalCancelamentoTituloCarga;

var CancelamentoTituloCarga = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.JustificativaCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.JustificativaCancelamento.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.MotivoDoCancelamento.getFieldDescription(), maxlength: 400 });

    this.Anexo = PropertyEntity({ eventClick: gerenciarAnexosCancelamentoTituloCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.Anexos, visible: ko.observable(true), enable: ko.observable(true) });

    this.Cancelar = PropertyEntity({ eventClick: cancelarTituloCargaClick, type: types.event, text: "Cancelar Título", icon: "fa fa-ban", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: fecharModalCancelamentoTituloCarga, type: types.event, text: Localization.Resources.Cargas.Carga.Fechar, icon: "fa fa-window-close", visible: ko.observable(true) });
};

////*******EVENTOS*******

function loadCancelamentoTituloCarga() {
    if (_cancelamentoTituloCarga)
        return;

    _cancelamentoTituloCarga = new CancelamentoTituloCarga();
    KoBindings(_cancelamentoTituloCarga, "knockoutCancelamentoTituloCarga");

    new BuscarJustificativaCancelamentoFinanceiro(_cancelamentoTituloCarga.JustificativaCancelamento);

    _modalCancelamentoTituloCarga = new bootstrap.Modal(document.getElementById("divModalCancelamentoTituloCarga"), { backdrop: 'static', keyboard: true });

    loadAnexoCancelamentoTituloCarga();
}

function cancelarTituloCargaClick() {
    if (!ValidarCamposObrigatorios(_cancelamentoTituloCarga)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, "Deseja realmente cancelar este Título?", function () {
    Salvar(_cancelamentoTituloCarga, "CargaFaturamento/CancelarTitulo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.SolicitacaoRealizadaComSucesso);

                enviarArquivosAnexadosCancelamentoTituloCarga(_cancelamentoTituloCarga.Codigo.val());

                fecharModalCancelamentoTituloCarga();
                limparCamposCancelamentoTituloCarga();

                _gridDadosFaturamento.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
});
}

////*******METODOS*******

function limparCamposCancelamentoTituloCarga() {
    LimparCampos(_cancelamentoTituloCarga);

    limparCamposAnexoCancelamentoTituloCarga();
}

function abrirModalCancelamentoTituloCarga(faturamento) {
    loadCancelamentoTituloCarga();

    limparCamposCancelamentoTituloCarga();

    _cancelamentoTituloCarga.Codigo.val(faturamento.CodigoTitulo);

    _modalCancelamentoTituloCarga.show();
}

function fecharModalCancelamentoTituloCarga() {
    _modalCancelamentoTituloCarga.hide();
    limparCamposCancelamentoTituloCarga();
}