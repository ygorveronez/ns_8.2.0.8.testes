/// <reference path="../../../Consultas/JustificativaCancelamentoFinanceiro.js" />
/// <reference path="CancelamentoFaturaAnexo.js" />
/// <reference path="EtapaFaturamento.js" />

var _cancelamentoFaturaCarga, _modalCancelamentoFaturaCarga;

var CancelamentoFaturaCarga = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.JustificativaCancelamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.JustificativaCancelamento.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.MotivoDoCancelamento.getFieldDescription(), maxlength: 400 });

    this.Anexo = PropertyEntity({ eventClick: gerenciarAnexosCancelamentoFaturaCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.Anexos, visible: ko.observable(true), enable: ko.observable(true) });

    this.Cancelar = PropertyEntity({ eventClick: cancelarFaturaCargaClick, type: types.event, text: Localization.Resources.Cargas.Carga.CancelarFatura, icon: "fa fa-ban", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: fecharModalCancelamentoFaturaCarga, type: types.event, text: Localization.Resources.Cargas.Carga.Fechar, icon: "fa fa-window-close", visible: ko.observable(true) });
};

////*******EVENTOS*******

function loadCancelamentoFaturaCarga() {
    if (_cancelamentoFaturaCarga)
        return;

    _cancelamentoFaturaCarga = new CancelamentoFaturaCarga();
    KoBindings(_cancelamentoFaturaCarga, "knockoutCancelamentoFaturaCarga");

    new BuscarJustificativaCancelamentoFinanceiro(_cancelamentoFaturaCarga.JustificativaCancelamento);

    _modalCancelamentoFaturaCarga = new bootstrap.Modal(document.getElementById("divModalCancelamentoFaturaCarga"), { backdrop: 'static', keyboard: true });

    loadAnexoCancelamentoFaturaCarga();
}

function cancelarFaturaCargaClick() {
    if (!ValidarCamposObrigatorios(_cancelamentoFaturaCarga)) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteCancelarEstaFatura, function () {
        executarReST("CargaFaturamento/ValidarCancelamentoFatura", { Codigo: _cancelamentoFaturaCarga.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    if (!r.Data.Valido) {
                        if (r.Data.PermiteCancelarFatura) {
                            exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, r.Data.Mensagem, function () {
                                FinalizarCancelamentoFaturaCarga();
                            }, null, Localization.Resources.Gerais.Geral.Confirmar, Localization.Resources.Gerais.Geral.Cancelar);
                        } else {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Data.Mensagem);
                        }
                    } else {
                        FinalizarCancelamentoFaturaCarga();
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function FinalizarCancelamentoFaturaCarga() {
    Salvar(_cancelamentoFaturaCarga, "CargaFaturamento/CancelarFatura", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.SolicitacaoRealizadaComSucesso);

                enviarArquivosAnexadosCancelamentoFaturaCarga(_cancelamentoFaturaCarga.Codigo.val());

                fecharModalCancelamentoFaturaCarga();
                limparCamposCancelamentoFaturaCarga();

                _gridDadosFaturamento.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

////*******METODOS*******

function limparCamposCancelamentoFaturaCarga() {
    LimparCampos(_cancelamentoFaturaCarga);

    limparCamposAnexoCancelamentoFaturaCarga();
}

function abrirModalCancelamentoFaturaCarga(faturamento) {
    loadCancelamentoFaturaCarga();

    limparCamposCancelamentoFaturaCarga();

    _cancelamentoFaturaCarga.Codigo.val(faturamento.CodigoFatura);

    _modalCancelamentoFaturaCarga.show();
}

function fecharModalCancelamentoFaturaCarga() {
    _modalCancelamentoFaturaCarga.hide();
    limparCamposCancelamentoFaturaCarga();
}