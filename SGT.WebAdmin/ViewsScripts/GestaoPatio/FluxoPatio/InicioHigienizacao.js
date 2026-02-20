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

var _inicioHigienizacaoFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var InicioHigienizacaoFluxoPatio = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.PreCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

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

    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaInicioHigienizacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaInicioHigienizacaoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ObservacoesDaEtapa, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoInicioHigienizacaoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: rejeitarEtapaInicioHigienizacaoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaInicioHigienizacaoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadInicioHigienizacaoFluxoPatio() {
    _inicioHigienizacaoFluxoPatio = new InicioHigienizacaoFluxoPatio();
    KoBindings(_inicioHigienizacaoFluxoPatio, "knockoutInicioHigienizacaoFluxoPatio");

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _inicioHigienizacaoFluxoPatio.Transportador.visible(false);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function avancarEtapaInicioHigienizacaoClick() {
    executarReST("InicioHigienizacao/AvancarEtapa", { Codigo: _inicioHigienizacaoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.HigienizacaoIniciadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioHigienizacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function observacoesEtapaInicioHigienizacaoClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.InicioHigienizacao);
}

function reabrirFluxoInicioHigienizacaoClick() {
    executarReST("InicioHigienizacao/ReabrirFluxo", { Codigo: _inicioHigienizacaoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReabertoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioHigienizacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function rejeitarEtapaInicioHigienizacaoClick() {
    executarReST("InicioHigienizacao/RejeitarEtapa", { Codigo: _inicioHigienizacaoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.InicioDaHigienizacaoRejeitadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioHigienizacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function voltarEtapaInicioHigienizacaoClick() {
    executarReST("InicioHigienizacao/VoltarEtapa", { Codigo: _inicioHigienizacaoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioHigienizacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirDetalhesInicioHigienizacao(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    executarReST("InicioHigienizacao/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _inicioHigienizacaoFluxoPatio.AvancarEtapa.visible(false);
                _inicioHigienizacaoFluxoPatio.ReabrirFluxo.visible(false);
                _inicioHigienizacaoFluxoPatio.RejeitarEtapa.visible(false);
                _inicioHigienizacaoFluxoPatio.VoltarEtapa.visible(false);

                PreencherObjetoKnout(_inicioHigienizacaoFluxoPatio, retorno);

                var permiteEditarEtapa = retorno.Data.PermitirEditarEtapa && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioHigienizacao, _PermissoesPersonalizadasFluxoPatio);

                _inicioHigienizacaoFluxoPatio.AvancarEtapa.visible(permiteEditarEtapa);

                if (_configuracaoGestaoPatio.InicioHigienizacaoPermiteVoltar && permiteEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _inicioHigienizacaoFluxoPatio.VoltarEtapa.visible(!primeiraEtapa && retorno.Data.PermitirEditarEtapa)

                    if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() == EnumEtapaFluxoGestaoPatio.InicioHigienizacao) {
                        _inicioHigienizacaoFluxoPatio.RejeitarEtapa.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                        _inicioHigienizacaoFluxoPatio.ReabrirFluxo.visible(_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado);
                    }
                }

                exibirModalDetalhesInicioHigienizacao();
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

function exibirModalDetalhesInicioHigienizacao() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_inicioHigienizacaoFluxoPatio);
        
    Global.abrirModal('divModalInicioHigienizacaoFluxoPatio');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesInicioHigienizacao();
    });

    $("#divModalInicioHigienizacaoFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_inicioHigienizacaoFluxoPatio);
    });
}

function fecharModalDetalhesInicioHigienizacao() {
    Global.fecharModal('divModalInicioHigienizacaoFluxoPatio');
}

// #endregion Funções Privadas
