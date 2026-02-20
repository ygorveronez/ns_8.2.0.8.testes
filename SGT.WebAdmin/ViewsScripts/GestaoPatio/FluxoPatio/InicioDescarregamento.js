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

var _inicioDescarregamentoFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var InicioDescarregamentoFluxoPatio = function () {
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

    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaInicioDescarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaInicioDescarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ObservacoesDaEtapa, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoInicioDescarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: rejeitarEtapaInicioDescarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaInicioDescarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadInicioDescarregamentoFluxoPatio() {
    _inicioDescarregamentoFluxoPatio = new InicioDescarregamentoFluxoPatio();
    KoBindings(_inicioDescarregamentoFluxoPatio, "knockoutInicioDescarregamentoFluxoPatio");

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _inicioDescarregamentoFluxoPatio.Transportador.visible(false);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function avancarEtapaInicioDescarregamentoClick() {
    var dados = {
        Codigo: _inicioDescarregamentoFluxoPatio.Codigo.val(),
        Pesagem: _inicioDescarregamentoFluxoPatio.Pesagem.val()
    };

    executarReST("InicioDescarregamento/AvancarEtapa", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.DescarregamentoIniciadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioDescarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function observacoesEtapaInicioDescarregamentoClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.InicioDescarregamento);
}

function reabrirFluxoInicioDescarregamentoClick() {
    executarReST("InicioDescarregamento/ReabrirFluxo", { Codigo: _inicioDescarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReabertoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioDescarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function rejeitarEtapaInicioDescarregamentoClick() {
    executarReST("InicioDescarregamento/RejeitarEtapa", { Codigo: _inicioDescarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.InicioDoCarregamentoRejeitadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioDescarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function voltarEtapaInicioDescarregamentoClick() {
    executarReST("InicioDescarregamento/VoltarEtapa", { Codigo: _inicioDescarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioDescarregamento();
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

function exibirDetalhesInicioDescarregamento(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    executarReST("InicioDescarregamento/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _inicioDescarregamentoFluxoPatio.AvancarEtapa.visible(false);
                _inicioDescarregamentoFluxoPatio.ReabrirFluxo.visible(false);
                _inicioDescarregamentoFluxoPatio.RejeitarEtapa.visible(false);
                _inicioDescarregamentoFluxoPatio.VoltarEtapa.visible(false);

                PreencherObjetoKnout(_inicioDescarregamentoFluxoPatio, retorno);

                _inicioDescarregamentoFluxoPatio.Pesagem.visible(retorno.Data.PermiteInformarPesagem);
                _inicioDescarregamentoFluxoPatio.AvancarEtapa.visible(retorno.Data.PermitirEditarEtapa);

                if (_configuracaoGestaoPatio.InicioDescarregamentoPermiteVoltar) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _inicioDescarregamentoFluxoPatio.VoltarEtapa.visible(!primeiraEtapa && retorno.Data.PermitirEditarEtapa)

                    if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() == EnumEtapaFluxoGestaoPatio.InicioDescarregamento) {
                        _inicioDescarregamentoFluxoPatio.RejeitarEtapa.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                        _inicioDescarregamentoFluxoPatio.ReabrirFluxo.visible(_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado);
                    }
                }

                exibirModalDetalhesInicioDescarregamento();
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

function exibirModalDetalhesInicioDescarregamento() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_inicioDescarregamentoFluxoPatio);
        
    Global.abrirModal('divModalInicioDescarregamentoFluxoPatio');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesInicioDescarregamento();
    });

    $("#divModalInicioDescarregamentoFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_inicioDescarregamentoFluxoPatio);
    });
}

function fecharModalDetalhesInicioDescarregamento() {    
    Global.fecharModal('divModalInicioDescarregamentoFluxoPatio');    
}

// #endregion Funções Privadas
