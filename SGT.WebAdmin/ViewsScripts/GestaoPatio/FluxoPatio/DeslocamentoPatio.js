/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="FluxoPatio.js" />
/// <reference path="ObservacoesEtapas.js" />

// #region Objetos Globais do Arquivo

var _deslocamentoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DeslocamentoPatio = function () {
    this.FluxoGestaoPatio = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Carga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CargaData = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Data.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CargaHora = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Hora.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DescricaoSituacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Transportador = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Transportador.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Veiculo.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Motorista = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Motorista.getFieldDescription(), val: ko.observable(""), def: "" });
    this.MotoristaTelefone = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Telefone.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.DataPrevisaoChegada.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Remetente = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Fornecedor.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDeCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDaOperacao.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CodigoIntegracaoDestinatario = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Destinatario.getFieldDescription(), val: ko.observable(""), def: "" });
    
    this.FechamentoPesagem = PropertyEntity({ eventClick: fechamentoPesagemClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.FechamentoPesagem, visible: ko.observable(false) });
    this.PesagemLoteInterno = PropertyEntity({ eventClick: pesagemLoteInternoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.DigitacaoDeLoteInterno, visible: ko.observable(false) });
    this.InformarDeslocamento = PropertyEntity({ eventClick: informarDeslocamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ConfirmarEtapa, idGrid: guid(), visible: ko.observable(false) });
    this.ImprimirComprovante = PropertyEntity({ eventClick: imprimirComprovanteClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ImprimirComprovante, idGrid: guid(), visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: VoltarEtapaDeslocamentoPatioClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, idGrid: guid(), visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: RejeitarEtapaDeslocamentoPatioClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, idGrid: guid(), visible: ko.observable(false) });
    this.ReabrirFluxo = PropertyEntity({ eventClick: ReabrirFluxoDeslocamentoPatioClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaDeslocamentoPatioClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ObservacoesDaEtapa, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
};

// #endregion Classes

// #region Funções de Inicialização

function LoadDeslocamentoPatio() {
    _deslocamentoPatio = new DeslocamentoPatio();
    KoBindings(_deslocamentoPatio, "knockoutDeslocamentoPatio");

    _deslocamentoPatio.DataCarregamento.val.subscribe(SetaDataHoraCargaDeslocamentoPatio);

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _deslocamentoPatio.Transportador.visible(false);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function observacoesEtapaDeslocamentoPatioClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.DeslocamentoPatio);
}

function imprimirComprovanteClick(e) {
    executarDownload("Guarita/ComprovanteSaida", { FluxoGestaoPatio: e.FluxoGestaoPatio.val() });
}

function VoltarEtapaDeslocamentoPatioClick() {
    exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, Localization.Resources.GestaoPatio.FluxoPatio.VoceTemCertezaQueDesejaRetornarEtapaAnterior, function () {
        executarReST("DeslocamentoPatio/VoltarEtapa", { FluxoGestaoPatio: _deslocamentoPatio.FluxoGestaoPatio.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornada);
                    atualizarFluxoPatio();
                    Global.fecharModal('divModalDeslocamentoPatio');
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function RejeitarEtapaDeslocamentoPatioClick() {
    exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, Localization.Resources.GestaoPatio.FluxoPatio.VoceTemCertezaQueDesejaRejeitarNessaEtapa, function () {
        executarReST("DeslocamentoPatio/RejeitarEtapa", { FluxoGestaoPatio: _deslocamentoPatio.FluxoGestaoPatio.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRejeitada);
                    atualizarFluxoPatio();
                    Global.fecharModal('divModalDeslocamentoPatio');
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function ReabrirFluxoDeslocamentoPatioClick(e) {
    exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, Localization.Resources.GestaoPatio.FluxoPatio.VoceTemCertezaqueDesejaReabrirFluxo, function () {
        executarReST("DeslocamentoPatio/ReabrirFluxo", { FluxoGestaoPatio: _deslocamentoPatio.FluxoGestaoPatio.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReaberto);
                    atualizarFluxoPatio();
                    Global.fecharModal('divModalDeslocamentoPatio');
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function informarDeslocamentoClick() {
    executarReST("DeslocamentoPatio/InformarDeslocamento", { FluxoGestaoPatio: _deslocamentoPatio.FluxoGestaoPatio.val() }, function (r) {
        if (r.Success) {
            if (r.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.ChegadaInformadaComSucesso);
                atualizarFluxoPatio();

                if (_configuracaoGestaoPatio.ExibirComprovanteSaida) {
                    _deslocamentoPatio.InformarDeslocamento.visible(false);
                    _deslocamentoPatio.VoltarEtapa.visible(false);
                    _deslocamentoPatio.ImprimirComprovante.visible(true);
                    _deslocamentoPatio.RejeitarEtapa.visible(false);
                } else {
                    Global.fecharModal('divModalDeslocamentoPatio');
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function fechamentoPesagemClick(e) {
    BuscarFechamentoPesagem(e.FluxoGestaoPatio.val());

    Global.abrirModal('divModalFechamentoPesagem');

    $("#divModalFechamentoPesagem").one('hidden.bs.modal', function () {
        LimparCampos(_fechamentoPesagem);
    });
}

function pesagemLoteInternoClick(e) {
    abrirPesagemLoteInternoClick(e.FluxoGestaoPatio.val());
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function ExibirDeslocamentoPatio(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;
    LimparCampos(_deslocamentoPatio);

    executarReST("DeslocamentoPatio/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                SetaTituloDeslocamentoPatio(opt.text);
                _deslocamentoPatio.InformarDeslocamento.visible(false);
                _deslocamentoPatio.RejeitarEtapa.visible(false);
                _deslocamentoPatio.VoltarEtapa.visible(false);
                _deslocamentoPatio.ReabrirFluxo.visible(false);
                _deslocamentoPatio.ImprimirComprovante.visible(false);

                var fluxoAberto = (knoutFluxo.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                var fluxoRejeitado = (knoutFluxo.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado || knoutFluxo.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Cancelado);

                PreencherObjetoKnout(_deslocamentoPatio, arg);

                if (arg.Data.VeiculoAindaNaoDeslocou && fluxoAberto) {
                    _deslocamentoPatio.InformarDeslocamento.visible(true);

                    if (_configuracaoGestaoPatio.DeslocamentoPatioPermiteVoltar) {
                        _deslocamentoPatio.VoltarEtapa.visible(_fluxoAtual.EtapaAtual.val() > 0);
                        _deslocamentoPatio.RejeitarEtapa.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo);
                    }

                }
                else if (_configuracaoGestaoPatio.ExibirComprovanteSaida && fluxoAberto)
                    _deslocamentoPatio.ImprimirComprovante.visible(true);

                if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarDeslocamentoPatio, _PermissoesPersonalizadasFluxoPatio))
                    _deslocamentoPatio.InformarDeslocamento.visible(false);

                if (fluxoRejeitado && _configuracaoGestaoPatio.DeslocamentoPatioPermiteVoltar && knoutFluxo.SituacaoEtapaFluxoGestaoPatio.val() != EnumSituacaoEtapaFluxoGestaoPatio.Cancelado)
                    _deslocamentoPatio.ReabrirFluxo.visible(true);

                if (edicaoEtapaFluxoPatioBloqueada())
                    ocultarBotoesEtapa(_deslocamentoPatio);
                                
                Global.abrirModal('divModalDeslocamentoPatio');
                $(window).one('keyup', function (e) {
                    if (e.keyCode == 27)
                        Global.fecharModal('divModalDeslocamentoPatio');
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });

    if (knoutFluxo.DeslocamentoPatioPermiteInformacoesPesagem.val()) {
        _deslocamentoPatio.FechamentoPesagem.visible(true);
        _deslocamentoPatio.PesagemLoteInterno.visible(knoutFluxo.DeslocamentoPatioPermiteInformacoesLoteInterno.val());
    }
    else {
        _deslocamentoPatio.FechamentoPesagem.visible(false);
        _deslocamentoPatio.PesagemLoteInterno.visible(false);
    }
}

// #endregion Funções Públicas

// #region Funções Privadas

function SetaTituloDeslocamentoPatio(titulo) {
    $("#deslocamento-patio-titulo").text(titulo);
}

function SetaDataHoraCargaDeslocamentoPatio() {
    var dataHora = _deslocamentoPatio.DataCarregamento.val();

    var data = "";
    var hora = "";

    if (dataHora != null && dataHora != "") {
        var splittedTime = dataHora.split(" ");
        data = splittedTime[0] || "";
        hora = splittedTime[1] || "";
    }

    _deslocamentoPatio.CargaData.val(data);
    _deslocamentoPatio.CargaHora.val(hora);
}

// #endregion Funções Privadas
