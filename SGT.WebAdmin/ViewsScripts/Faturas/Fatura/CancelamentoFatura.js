var _cancelamentoFatura, _modalCancelamentoFatura;

var CancelamentoFatura = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Motivo = PropertyEntity({ text: "*Motivo:", required: true, issue: 0, maxlength: 400 });
    this.DataCancelamento = PropertyEntity({ text: "*Data de Cancelamento:", getType: typesKnockout.dateTime, visible: ko.observable(_CONFIGURACAO_TMS.InformarDataCancelamentoCancelamentoFatura), required: _CONFIGURACAO_TMS.InformarDataCancelamentoCancelamentoFatura, issue: 0, maxlength: 400 });
    this.DuplicarFatura = PropertyEntity({ text: "Duplicar a fatura", getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(false) });

    this.Cancelar = PropertyEntity({ eventClick: CancelarFaturaClick, type: types.event, text: "Cancelar a Fatura", icon: "fa fa-ban", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaCancelamentoFatura, type: types.event, text: "Fechar", icon: "fa fa-window-close", visible: ko.observable(true) });
};

////*******EVENTOS*******

function LoadCancelamentoFatura() {

    _cancelamentoFatura = new CancelamentoFatura();
    KoBindings(_cancelamentoFatura, "knockoutCancelamentoFatura");

    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Fatura_PermiteDuplicar, _PermissoesPersonalizadas))
        _cancelamentoFatura.DuplicarFatura.visible(true);

    _modalCancelamentoFatura = new bootstrap.Modal(document.getElementById("knockoutCancelamentoFatura"), { backdrop: 'static', keyboard: true });
}

function CancelarFaturaClick() {

    if (!ValidarCamposObrigatorios(_cancelamentoFatura)) {
        exibirMensagem(tipoMensagem.aviso, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    exibirConfirmacao("Atenção!", "Deseja realmente cancelar esta fatura?", function () {
        _cancelamentoFatura.Codigo.val(_fatura.Codigo.val());

        executarReST("FaturaFechamento/ValidarCancelamentoFatura", { Codigo: _fatura.Codigo.val(), DataCancelamento: _cancelamentoFatura.DataCancelamento.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    if (!r.Data.Valido) {
                        if (r.Data.PermiteCancelarFatura) {
                            exibirConfirmacao("Confirmação", r.Data.Mensagem, function () {
                                FinalizarCancelamentoFatura();
                            }, null, "Confirmar", "Cancelar");
                        } else {
                            exibirMensagem(tipoMensagem.atencao, "Atenção", r.Data.Mensagem);
                        }
                    } else {
                        FinalizarCancelamentoFatura();
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function FinalizarCancelamentoFatura() {
    Salvar(_cancelamentoFatura, "FaturaFechamento/ReAbrirFatura", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação realizada com sucesso!");

                FecharTelaCancelamentoFatura();
                LimparCamposCancelamentoFatura();

                _fatura.Situacao.val(EnumSituacoesFatura.EmCancelamento);
                CarregarDadosCabecalho(arg.Data);
                PosicionarEtapa(arg.Data);
                VerificarBotoes();

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

////*******METODOS*******

function LimparCamposCancelamentoFatura() {
    LimparCampos(_cancelamentoFatura);
}

function AbrirTelaCancelamentoFatura(dadosGrid) {
    LimparCamposCancelamentoFatura();

    if (_CONFIGURACAO_TMS.InformarDataCancelamentoCancelamentoFatura)
        _cancelamentoFatura.DataCancelamento.val(Global.DataHoraAtual());

    _modalCancelamentoFatura.show();
}

function FecharTelaCancelamentoFatura() {
    _modalCancelamentoFatura.hide();
    LimparCamposCancelamentoFatura();
}