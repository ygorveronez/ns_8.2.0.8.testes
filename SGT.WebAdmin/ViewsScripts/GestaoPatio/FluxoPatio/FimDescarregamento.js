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

var _fimDescarregamentoFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var FimDescarregamentoFluxoPatio = function () {
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

    this.Pesagem = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: Localization.Resources.GestaoPatio.FluxoPatio.Pesagem.getFieldDescription(), required: false, visible: ko.observable(false) });

    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaFimDescarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaFimDescarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ObservacoesDaEtapa, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoFimDescarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: rejeitarEtapaFimDescarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaFimDescarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadFimDescarregamentoFluxoPatio() {
    _fimDescarregamentoFluxoPatio = new FimDescarregamentoFluxoPatio();
    KoBindings(_fimDescarregamentoFluxoPatio, "knockoutFimDescarregamentoFluxoPatio");

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _fimDescarregamentoFluxoPatio.Transportador.visible(false);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function avancarEtapaFimDescarregamentoClick() {
    var dados = {
        Codigo: _fimDescarregamentoFluxoPatio.Codigo.val(),
        Pesagem: _fimDescarregamentoFluxoPatio.Pesagem.val()
    };

    executarReST("FimDescarregamento/AvancarEtapa", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.DescarregamentoFinalizadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesFimDescarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function observacoesEtapaFimDescarregamentoClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.FimDescarregamento);
}

function reabrirFluxoFimDescarregamentoClick() {
    executarReST("FimDescarregamento/ReabrirFluxo", { Codigo: _fimDescarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReabertoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesFimDescarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function rejeitarEtapaFimDescarregamentoClick() {
    executarReST("FimDescarregamento/RejeitarEtapa", { Codigo: _fimDescarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FimDoDescarregamentoRejeitadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesFimDescarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function voltarEtapaFimDescarregamentoClick() {
    executarReST("FimDescarregamento/VoltarEtapa", { Codigo: _fimDescarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesFimDescarregamento();
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

function exibirDetalhesFimDescarregamento(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    executarReST("FimDescarregamento/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _fimDescarregamentoFluxoPatio.AvancarEtapa.visible(false);
                _fimDescarregamentoFluxoPatio.ReabrirFluxo.visible(false);
                _fimDescarregamentoFluxoPatio.RejeitarEtapa.visible(false);
                _fimDescarregamentoFluxoPatio.VoltarEtapa.visible(false);

                PreencherObjetoKnout(_fimDescarregamentoFluxoPatio, retorno);

                _fimDescarregamentoFluxoPatio.Pesagem.visible(retorno.Data.PermiteInformarPesagem);
                _fimDescarregamentoFluxoPatio.AvancarEtapa.visible(retorno.Data.PermitirEditarEtapa);

                if (_configuracaoGestaoPatio.FimDescarregamentoPermiteVoltar) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _fimDescarregamentoFluxoPatio.VoltarEtapa.visible(!primeiraEtapa && retorno.Data.PermitirEditarEtapa)

                    if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() == EnumEtapaFluxoGestaoPatio.FimDescarregamento) {
                        _fimDescarregamentoFluxoPatio.RejeitarEtapa.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                        _fimDescarregamentoFluxoPatio.ReabrirFluxo.visible(_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado);
                    }
                }

                exibirModalDetalhesFimDescarregamento();
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

function exibirModalDetalhesFimDescarregamento() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_fimDescarregamentoFluxoPatio);

    Global.abrirModal('divModalFimDescarregamentoFluxoPatio');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesFimDescarregamento();
    });

    $("#divModalFimDescarregamentoFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_fimDescarregamentoFluxoPatio);
    });
}

function fecharModalDetalhesFimDescarregamento() {
    Global.fecharModal('divModalFimDescarregamentoFluxoPatio');
}

// #endregion Funções Privadas
