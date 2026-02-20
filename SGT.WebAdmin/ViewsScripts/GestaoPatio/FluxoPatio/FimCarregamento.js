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

var _fimCarregamentoFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var FimCarregamentoFluxoPatio = function () {
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

    this.ImprimirSinteseMateriais = PropertyEntity({ eventClick: imprimirSinteseMateriaisClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.SinteseDeMateriais, visible: ko.observable(false) });
    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaFimCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaFimCarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.Observacoes, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoFimCarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: rejeitarEtapaFimCarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaFimCarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadFimCarregamentoFluxoPatio() {
    _fimCarregamentoFluxoPatio = new FimCarregamentoFluxoPatio();
    KoBindings(_fimCarregamentoFluxoPatio, "knockoutFimCarregamentoFluxoPatio");

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _fimCarregamentoFluxoPatio.Transportador.visible(false);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function avancarEtapaFimCarregamentoClick() {
    var dados = {
        Codigo: _fimCarregamentoFluxoPatio.Codigo.val(),
        Pesagem: _fimCarregamentoFluxoPatio.Pesagem.val()
    };

    executarReST("FimCarregamento/AvancarEtapa", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.CarregamentoFinalizadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesFimCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function imprimirSinteseMateriaisClick() {
    executarDownload("FimCarregamento/ImprimirSinteseMateriais", { Codigo: _fimCarregamentoFluxoPatio.Codigo.val() });
}

function observacoesEtapaFimCarregamentoClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.FimCarregamento);
}

function reabrirFluxoFimCarregamentoClick() {
    executarReST("FimCarregamento/ReabrirFluxo", { Codigo: _fimCarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReabertoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesFimCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function rejeitarEtapaFimCarregamentoClick() {
    executarReST("FimCarregamento/RejeitarEtapa", { Codigo: _fimCarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FimDoCarregamentoRejeitadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesFimCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function voltarEtapaFimCarregamentoClick() {
    executarReST("FimCarregamento/VoltarEtapa", { Codigo: _fimCarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesFimCarregamento();
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

function exibirDetalhesFimCarregamento(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    executarReST("FimCarregamento/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _fimCarregamentoFluxoPatio.AvancarEtapa.visible(false);
                _fimCarregamentoFluxoPatio.ReabrirFluxo.visible(false);
                _fimCarregamentoFluxoPatio.RejeitarEtapa.visible(false);
                _fimCarregamentoFluxoPatio.VoltarEtapa.visible(false);

                PreencherObjetoKnout(_fimCarregamentoFluxoPatio, retorno);

                var permiteEditarEtapa = retorno.Data.PermitirEditarEtapa && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimCarregamento, _PermissoesPersonalizadasFluxoPatio);

                _fimCarregamentoFluxoPatio.Pesagem.visible(retorno.Data.PermiteInformarPesagem);
                _fimCarregamentoFluxoPatio.AvancarEtapa.visible(permiteEditarEtapa);
                _fimCarregamentoFluxoPatio.ImprimirSinteseMateriais.visible(knoutFluxo.ChegadaVeiculoPermiteImprimirRelacaoDeProdutos.val());

                if (_configuracaoGestaoPatio.FimCarregamentoPermiteVoltar && permiteEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _fimCarregamentoFluxoPatio.VoltarEtapa.visible(!primeiraEtapa)

                    if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() == EnumEtapaFluxoGestaoPatio.FimCarregamento) {
                        _fimCarregamentoFluxoPatio.RejeitarEtapa.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                        _fimCarregamentoFluxoPatio.ReabrirFluxo.visible(_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado);
                    }
                }

                exibirModalDetalhesFimCarregamento();
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

function exibirModalDetalhesFimCarregamento() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_fimCarregamentoFluxoPatio);
        
    Global.abrirModal('divModalFimCarregamentoFluxoPatio');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesFimCarregamento();
    });

    $("#divModalFimCarregamentoFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_fimCarregamentoFluxoPatio);
    });
}

function fecharModalDetalhesFimCarregamento() {
    Global.fecharModal('divModalFimCarregamentoFluxoPatio');
}

// #endregion Funções Privadas
