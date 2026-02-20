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

var _solicitacaoVeiculoFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var SolicitacaoVeiculoFluxoPatio = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.PreCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ObservacaoFluxoPatio = PropertyEntity({ visible: false });

    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Carga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroPreCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.PreCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CargaData = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Data.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CargaHora = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Hora.getFieldDescription(), val: ko.observable(""), def: "" });

    this.CodigoIntegracaoDestinatario = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Destinatario.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Remetente = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Fornecedor.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDeCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDaOperacao.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Transportador = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Transportador.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Veiculo.getFieldDescription(), val: ko.observable(""), def: "" });

    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaSolicitacaoVeiculoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(false) });
    this.ExibirObservacao = PropertyEntity({ eventClick: function () { exibirObservacaoFluxoPatio(_solicitacaoVeiculoFluxoPatio.ObservacaoFluxoPatio.val()); }, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ExibirObservacao });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaSolicitacaoVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.Observacoes, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoSolicitacaoVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: rejeitarEtapaSolicitacaoVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaSolicitacaoVeiculoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
    this.EnviarSMSMotorista = PropertyEntity({ eventClick: enviarSMSMotoristaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.EnviarSMSMotorista, visible: ko.observable(true) });
    this.NaoPermitirEnviarSMS = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(0), def: 0 });
    this.InformarDadosTransporte = PropertyEntity({ eventClick: informarDadosTransporteClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.InformarDadosTransporte, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadSolicitacaoVeiculoFluxoPatio() {
    _solicitacaoVeiculoFluxoPatio = new SolicitacaoVeiculoFluxoPatio();
    KoBindings(_solicitacaoVeiculoFluxoPatio, "knockoutSolicitacaoVeiculoFluxoPatio");

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _solicitacaoVeiculoFluxoPatio.Transportador.visible(false);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function avancarEtapaSolicitacaoVeiculoClick() {
    executarReST("SolicitacaoVeiculo/AvancarEtapa", { Codigo: _solicitacaoVeiculoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.SolicitacaoDeVeiculoFinalizadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesSolicitacaoVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function enviarSMSMotoristaClick() {
    executarReST("SolicitacaoVeiculo/EnviarSMSMotorista", { Codigo: _solicitacaoVeiculoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success)
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.SMSFoiEnviado);
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function observacoesEtapaSolicitacaoVeiculoClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.SolicitacaoVeiculo);
}

function reabrirFluxoSolicitacaoVeiculoClick() {
    executarReST("SolicitacaoVeiculo/ReabrirFluxo", { Codigo: _solicitacaoVeiculoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReabertoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesSolicitacaoVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function rejeitarEtapaSolicitacaoVeiculoClick() {
    executarReST("SolicitacaoVeiculo/RejeitarEtapa", { Codigo: _solicitacaoVeiculoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.SolicitacaoDeVeiculoRejeitadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesSolicitacaoVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function voltarEtapaSolicitacaoVeiculoClick() {
    executarReST("SolicitacaoVeiculo/VoltarEtapa", { Codigo: _solicitacaoVeiculoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesSolicitacaoVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function informarDadosTransporteClick() {
    buscarDadosTransporte(_solicitacaoVeiculoFluxoPatio.Codigo.val());
    abrirModalDadosTransporte();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirDetalhesSolicitacaoVeiculo(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    executarReST("SolicitacaoVeiculo/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _solicitacaoVeiculoFluxoPatio.AvancarEtapa.visible(false);
                _solicitacaoVeiculoFluxoPatio.ReabrirFluxo.visible(false);
                _solicitacaoVeiculoFluxoPatio.RejeitarEtapa.visible(false);
                _solicitacaoVeiculoFluxoPatio.VoltarEtapa.visible(false);

                PreencherObjetoKnout(_solicitacaoVeiculoFluxoPatio, retorno);
                if (_configuracaoGestaoPatio.SolicitacaoVeiculoPermiteEnvioSMSMotorista)
                    _solicitacaoVeiculoFluxoPatio.EnviarSMSMotorista.visible(true);

                if (_solicitacaoVeiculoFluxoPatio.NaoPermitirEnviarSMS.val())
                    _solicitacaoVeiculoFluxoPatio.EnviarSMSMotorista.visible(false);

                var permiteEditarEtapa = retorno.Data.PermitirEditarEtapa && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_SolicitarVeiculo, _PermissoesPersonalizadasFluxoPatio);

                if (retorno.Data.PermitirInformarDadosTransporte)
                    _solicitacaoVeiculoFluxoPatio.InformarDadosTransporte.visible(true);

                SetarEnableCamposDadosTransporte(permiteEditarEtapa);

                _dadosTransporte.Atualizar.visible(permiteEditarEtapa);

                _solicitacaoVeiculoFluxoPatio.AvancarEtapa.visible(permiteEditarEtapa);

                if (_configuracaoGestaoPatio.SolicitacaoVeiculoPermiteVoltar && permiteEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _solicitacaoVeiculoFluxoPatio.VoltarEtapa.visible(!primeiraEtapa)

                    if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() == EnumEtapaFluxoGestaoPatio.SolicitacaoVeiculo) {
                        _solicitacaoVeiculoFluxoPatio.RejeitarEtapa.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                        _solicitacaoVeiculoFluxoPatio.ReabrirFluxo.visible(_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado);
                    }
                }

                exibirModalDetalhesSolicitacaoVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalDetalhesSolicitacaoVeiculo() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_solicitacaoVeiculoFluxoPatio);

    Global.abrirModal('divModalSolicitacaoVeiculoFluxoPatio');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesSolicitacaoVeiculo();
    });

    $("#divModalSolicitacaoVeiculoFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_solicitacaoVeiculoFluxoPatio);
    });
}

function fecharModalDetalhesSolicitacaoVeiculo() {
    Global.fecharModal('divModalSolicitacaoVeiculoFluxoPatio');
}

// #endregion Funções Privadas
