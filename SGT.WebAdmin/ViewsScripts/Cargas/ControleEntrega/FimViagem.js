/*FimViagem.js*/
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/Global/PermissoesPersonalizadas.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//********MAPEAMENTO*******

var _fimViagem;
var _gridResumoRoteiro;
var _justificativaEncerramentoviagem;

var FimViagem = function () {
    //this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.DataFimViagem = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataFimDaViagem.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataInicioViagem = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioViagem.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataFimViagemInformada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataFimDaViagem.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(false) });
    this.DataInicioViagemInformada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataInicioViagem.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(false) });
    this.Auditar = PropertyEntity({ eventClick: auditarFimViagemCargaClick, type: types.event, visible: _CONFIGURACAO_TMS.PermiteAuditar });
    this.ReabrirCarga = PropertyEntity({ eventClick: reabrirCargaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.ReabrirCarga, idGrid: guid(), visible: ko.observable(false) });
    this.InformarFimViagem = PropertyEntity({ eventClick: informarFimViagemClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.FinalizarViagem, idGrid: guid(), visible: ko.observable(false) });
    this.InformarFimViagemAlterar = PropertyEntity({ eventClick: informarFimViagemAlterarClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AlterarDataDeFim, idGrid: guid(), visible: ko.observable(false) });
    this.GerarOcorrenciaEstadia = PropertyEntity({ eventClick: GerarOcorrenciaEstadiaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.GerarOcorrenciaEstadia, idGrid: guid(), visible: ko.observable(false) });

    this.MotivoEncerramentoManual = PropertyEntity({ text: "Motivo do Encerramento Manual:", val: ko.observable("") });
    this.ObservacaoEncerramentoManual = PropertyEntity({ text: "Observação Encerramento Manual:", val: ko.observable("") });

    this.ExigirJustificativaParaEncerramentoManualViagem = PropertyEntity({ val: ko.observable(false) });

    this.ResumoRoteiro = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(false) });
}

var JustificativaEncerramentoviagem = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Justificativa = PropertyEntity({ text: "Justificativa:", type: types.entity, idBtnSearch: guid(), codEntity: ko.observable(0), val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação", maxlength: 3000 });

    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Salvar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

//********EVENTO*******
function loadFimViagem(carregarSomenteFimViagem) {
    _fimViagem = new FimViagem();
    KoBindings(_fimViagem, "knockoutFimViagem");

    if (!carregarSomenteFimViagem) {
        _justificativaEncerramentoviagem = new JustificativaEncerramentoviagem();
        KoBindings(_justificativaEncerramentoviagem, "knockoutJustificativaEncerramentoViagem");

        new BuscarJustificativaEncerramentoManualViagem(_justificativaEncerramentoviagem.Justificativa);
    }

    _gridResumoRoteiro = new GridView(_fimViagem.ResumoRoteiro.idGrid, "ControleEntregaFimViagem/ConsultarResumoRoteiro", _fimViagem);
}

function exibirFimViagem(fluxoEntrega, etapa) {
    _etapaAtualFluxo = fluxoEntrega;
    LimparCampos(_fimViagem);

    var permiteAlterarDatasInicioFimViagem = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimViagem, _PermissoesPersonalizadasControleEntrega);
    var permiteGerarOcorrenciaEstadia = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteGerarOcorrenciaEstadia, _PermissoesPersonalizadasControleEntrega);

    executarReST("ControleEntregaFimViagem/BuscarPorCarga", { Carga: etapa.Carga }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _fimViagem.InformarFimViagem.visible(false);
                _fimViagem.InformarFimViagemAlterar.visible(false);
                _fimViagem.ReabrirCarga.visible(false);

                PreencherObjetoKnout(_fimViagem, arg);

                if ((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && arg.Data.PermiteTransportadorConfirmarRejeitarEntrega) || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente, _PermissoesPersonalizadasControleEntrega)) {
                    if (arg.Data.ViagemAberta)
                        _fimViagem.InformarFimViagem.visible(true);
                    else if (!arg.Data.ViagemAberta && permiteAlterarDatasInicioFimViagem)
                        _fimViagem.InformarFimViagemAlterar.visible(true);

                    if (arg.Data.DataFimViagem)
                        _fimViagem.DataFimViagemInformada.val(arg.Data.DataFimViagem);

                    if (arg.Data.DataInicioViagem)
                        _fimViagem.DataInicioViagemInformada.val(arg.Data.DataInicioViagem);

                    if (arg.Data.DataFimViagemSugerida)
                        _fimViagem.DataFimViagemInformada.val(arg.Data.DataFimViagemSugerida);
                }

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
                    _fimViagem.InformarFimViagem.visible(!_CONFIGURACAO_TMS.NaoPermitirFinalizarViagemDetalhesFimViagem);

                if (!arg.Data.ViagemAberta && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_PermiteReabrirCargaFinalizada, _PermissoesPersonalizadasControleEntrega) && (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)) {
                    _fimViagem.ReabrirCarga.visible(true)
                }

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                    _fimViagem.ResumoRoteiro.visible(true);
                    _gridResumoRoteiro.CarregarGrid();
                }

                if (permiteGerarOcorrenciaEstadia && arg.Data.HabilitarCobrancaEstadiaAutomaticaPeloTracking)
                    _fimViagem.GerarOcorrenciaEstadia.visible(true);
                else
                    _fimViagem.GerarOcorrenciaEstadia.visible(false);

                    loadModalDataEntregasFimViagem(arg.Data.Carga);

                exibeModalEtapa('#divModalFimViagem');

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

//********MÉTODOS*******

function auditarFimViagemCargaClick(e) {
    var data = { Codigo: e.Carga.val() };
    var closureAuditoria = OpcaoAuditoria("Carga", null, e);

    closureAuditoria(data);
}

function informarFimViagemClick() {
    if (_fimViagem.ExigirJustificativaParaEncerramentoManualViagem.val()) {
        Global.abrirModal("modalJustificativaEncerramentoViagem");
    } else {
        confirmarFimViagem();
    }
}

function confirmarFimViagem() {
    if (!ValidarCamposObrigatorios(_fimViagem)) {
        exibirMensagemCamposObrigatorio();
        return;
    }

    if (!validarCamposObrigatoriosDatasEntregas()) {
        return;
    }

    let entregas = obterEntregasComDatas();
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.VoceTemCertezaQueDesejaFinalizarViagem, function () {
        executarReST("ControleEntregaFimViagem/InformarFimViagem", {
            Carga: _fimViagem.Carga.val(),
            DataFimViagemInformada: _fimViagem.DataFimViagemInformada.val(),
            DataInicioViagemInformada: _fimViagem.DataInicioViagemInformada.val(),
            Entregas: entregas
        }, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.ViagemFinalizadaComSucesso);

                    Global.fecharModal("divModalFimViagem");

                    atualizarControleEntrega();

                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function reabrirCargaClick() {
    exibirConfirmacao("Confirmação", "Você tem certeza que deseja reabrir a carga?", function () {
        executarReST("ControleEntregaFimViagem/ReabrirCarga", {
            Carga: _fimViagem.Carga.val()
        }, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Carga reaberta com sucesso!");

                    Global.fecharModal("divModalFimViagem");

                    atualizarControleEntrega();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function GerarOcorrenciaEstadiaClick() {
    exibirConfirmacao("Confirmação", "Você tem certeza que deseja gerar ocorrência(s) de estadia para esta carga? (caso já tenha gerado irá duplicar a(s) ocorrência(s))", function () {
        executarReST("ControleEntregaFimViagem/GerarOcorrenciaEstadia", {
            Carga: _fimViagem.Carga.val()
        }, function (r) {
            if (r.Success) {
                if (r.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.OcorrenciaEstadiaGeradaComSucesso);

                    Global.fecharModal("divModalFimViagem");

                    atualizarControleEntrega();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    });
}

function informarFimViagemAlterarClick() {
    if (!ValidarCamposObrigatorios(_fimViagem)) {
        exibirMensagemCamposObrigatorio();
        return;
    }

    executarReST("ControleEntregaFimViagem/InformarFimViagemAlterar", {
        Carga: _fimViagem.Carga.val(),
        DataFimViagemInformada: _fimViagem.DataFimViagemInformada.val()
    }, function (r) {
        if (r.Success) {
            if (r.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.DataDeFimDeViagemAlteradaComSucesso);

                Global.fecharModal("divModalFimViagem");

                atualizarControleEntrega();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function salvarClick() {
    _justificativaEncerramentoviagem.Codigo.val(_fimViagem.Carga.val());

    exibirConfirmacao("Confirmação", "Você tem certeza que deseja informar uma justificativa?", function () {
        executarReST("ControleEntregaFimViagem/AdicionarJustificativa", {
            Codigo: _fimViagem.Carga.val(),
            CodigoEncerramento: _justificativaEncerramentoviagem.Justificativa.codEntity(),
            Observacao: _justificativaEncerramentoviagem.Observacao.val(),
        }, function (arg) {
            if (arg.Success) {

                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Justificativa adicionada com sucesso.");

                    LimparCampos(_justificativaEncerramentoviagem);
                    Global.fecharModal("modalJustificativaEncerramentoViagem");
                    confirmarFimViagem();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function cancelarClick() {
    Global.fecharModal("modalJustificativaEncerramentoViagem");
}