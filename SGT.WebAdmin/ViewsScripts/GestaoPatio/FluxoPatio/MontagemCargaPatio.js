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

var _montagemCargaPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var MontagemCargaPatio = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.PreCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.EtapaAntecipada = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: 0 });
    this.ObservacaoFluxoPatio = PropertyEntity({ visible: false });

    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Carga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroPreCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.PreCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CargaData = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Data.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CargaHora = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Hora.getFieldDescription(), val: ko.observable(""), def: "" });

    this.CodigoIntegracaoDestinatario = PropertyEntity({ val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Destinatario.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Remetente = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Fornecedor.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDeCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDeOperacao.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Transportador = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Transportador.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Veiculo.getFieldDescription(), val: ko.observable(""), def: "" });

    this.QuantidadeCaixas = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.QuantidadeDeCaixas.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(""), def: "", enable: ko.observable(true), maxlength: 15, visible: ko.observable(false) });
    this.QuantidadeItens = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.QuantidadeDeItens.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(""), def: "", enable: ko.observable(true), maxlength: 15, visible: ko.observable(false) });
    this.QuantidadePalletsFracionados = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.QuantidadeDePalletsFracionados.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(""), def: "", enable: ko.observable(true), maxlength: 15, visible: ko.observable(false) });
    this.QuantidadePalletsInteiros = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.QuantidadeDePalletsInteiros.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(""), def: "", enable: ko.observable(true), maxlength: 15, visible: ko.observable(false) });

    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaMontagemCargaPatioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(false) });
    this.ExibirObservacao = PropertyEntity({ eventClick: function () { exibirObservacaoFluxoPatio(_montagemCargaPatio.ObservacaoFluxoPatio.val()); }, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ExibirObservacao });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaMontagemCargaPatioClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.Observacoes, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoMontagemCargaPatioClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: rejeitarEtapaMontagemCargaPatioClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaMontagemCargaPatioClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
    this.ImprimirComprovante = PropertyEntity({ eventClick: imprimirComprovanteMontagemCargaPatioClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ImprimirComprovante, idGrid: guid(), visible: ko.observable(true) });
    this.AbrirAtendimento = PropertyEntity({ eventClick: abrirAtendimentoMontagemCargaPatioClick, type: types.event, text: "Abrir Atendimento", idGrid: guid(), visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadMontagemCargaPatio() {
    _montagemCargaPatio = new MontagemCargaPatio();
    KoBindings(_montagemCargaPatio, "knockoutMontagemCargaPatio");

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _montagemCargaPatio.Transportador.visible(false);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function avancarEtapaMontagemCargaPatioClick() {
    var dados = {
        Codigo: _montagemCargaPatio.Codigo.val(),
        QuantidadeCaixas: _montagemCargaPatio.QuantidadeCaixas.val(),
        QuantidadeItens: _montagemCargaPatio.QuantidadeItens.val(),
        QuantidadePalletsFracionados: _montagemCargaPatio.QuantidadePalletsFracionados.val(),
        QuantidadePalletsInteiros: _montagemCargaPatio.QuantidadePalletsInteiros.val()
    };

    if (_configuracaoGestaoPatio.MontagemCargaPermiteAntecipar && _montagemCargaPatio.EtapaAntecipada.val())
        exibirConfirmacao(Localization.Resources.GestaoPatio.FluxoPatio.AnteciparEtapa, Localization.Resources.GestaoPatio.FluxoPatio.AoConfirmarEtapaSeraAntecipadaPermanecendoSequenciaUltimaetapaConfirmada.format(Localization.Resources.GestaoPatio.FluxoPatio.MontagemDeCarga), () => executarAvancarEtapaMontagemCargaPatio(dados));
    else
        executarAvancarEtapaMontagemCargaPatio(dados);
}

function executarAvancarEtapaMontagemCargaPatio(dados) {
    executarReST("MontagemCargaPatio/AvancarEtapa", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.MontagemDeCargaDePatioFinalizadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesMontagemCargaPatio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function observacoesEtapaMontagemCargaPatioClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.MontagemCarga);
}

function reabrirFluxoMontagemCargaPatioClick() {
    executarReST("MontagemCargaPatio/ReabrirFluxo", { Codigo: _montagemCargaPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReabertoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesMontagemCargaPatio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function rejeitarEtapaMontagemCargaPatioClick() {
    executarReST("MontagemCargaPatio/RejeitarEtapa", { Codigo: _montagemCargaPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.MontagemDeCargaDePatioRejeitadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesMontagemCargaPatio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function voltarEtapaMontagemCargaPatioClick() {
    executarReST("MontagemCargaPatio/VoltarEtapa", { Codigo: _montagemCargaPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesMontagemCargaPatio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function imprimirComprovanteMontagemCargaPatioClick() {
    executarDownload("MontagemCargaPatio/ComprovanteMontagemCarga", { Codigo: _montagemCargaPatio.Codigo.val() });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirDetalhesMontagemCarga(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;
    _montagemCargaPatio.EtapaAntecipada.val(!opt.etapaLiberada);

    executarReST("MontagemCargaPatio/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _montagemCargaPatio.AvancarEtapa.visible(false);
                _montagemCargaPatio.ReabrirFluxo.visible(false);
                _montagemCargaPatio.RejeitarEtapa.visible(false);
                _montagemCargaPatio.VoltarEtapa.visible(false);
                _montagemCargaPatio.AbrirAtendimento.visible(retorno.Data.MontagemCargaPermiteGerarAtendimento);

                PreencherObjetoKnout(_montagemCargaPatio, retorno);

                var permiteEditarEtapa = retorno.Data.PermitirEditarEtapa && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarMontagemCarga, _PermissoesPersonalizadasFluxoPatio);

                _montagemCargaPatio.QuantidadeCaixas.visible(retorno.Data.PermiteInformarQuantidadeCaixas);
                _montagemCargaPatio.QuantidadeItens.visible(retorno.Data.PermiteInformarQuantidadeItens);
                _montagemCargaPatio.QuantidadePalletsFracionados.visible(retorno.Data.PermiteInformarQuantidadePallets);
                _montagemCargaPatio.QuantidadePalletsInteiros.visible(retorno.Data.PermiteInformarQuantidadePallets);
                _montagemCargaPatio.AvancarEtapa.visible(permiteEditarEtapa);

                if (_configuracaoGestaoPatio.MontagemCargaPermiteVoltar && permiteEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _montagemCargaPatio.VoltarEtapa.visible(!primeiraEtapa)

                    if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() == EnumEtapaFluxoGestaoPatio.MontagemCarga) {
                        _montagemCargaPatio.RejeitarEtapa.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                        _montagemCargaPatio.ReabrirFluxo.visible(_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado);
                    }
                }

                exibirModalDetalhesMontagemCargaPatio();
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

function exibirModalDetalhesMontagemCargaPatio() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_montagemCargaPatio);

    Global.abrirModal('divModalMontagemCargaPatio');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesMontagemCargaPatio();
    });

    $("#divModalMontagemCargaPatio").one('hidden.bs.modal', function () {
        LimparCampos(_montagemCargaPatio);
    });
}

function fecharModalDetalhesMontagemCargaPatio() {
    Global.fecharModal('divModalMontagemCargaPatio');
}

function abrirAtendimentoMontagemCargaPatioClick() {
    var dadosCarga = { Carga: _montagemCargaPatio.Carga };

    CriarNovoChamado(dadosCarga, "divModalChamadoOcorrencia");
}

// #endregion Funções Privadas
