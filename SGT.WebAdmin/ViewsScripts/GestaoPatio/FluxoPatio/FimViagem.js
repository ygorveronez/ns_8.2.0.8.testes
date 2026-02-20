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

var _fimViagem;

// #endregion Objetos Globais do Arquivo

// #region Classes

var FimViagem = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.DataFimViagem = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.DataFimDaViagem.getFieldDescription(), val: ko.observable(""), def: "" });

    this.DocumentosPesagem = PropertyEntity({ eventClick: documentosPesagemClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.DocumentosPesagem, visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaFimViagemClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ObservacoesDaEtapa, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ImprimirComprovanteCargaInformada = PropertyEntity({ eventClick: function (e) { imprimirComprovanteCargaInformada(e.Codigo.val()); }, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ImprimirComprovanteDaCarga, visible: ko.observable(isPermitirImprimirComprovanteCargaInformada()) });
    this.InformarFimViagem = PropertyEntity({ eventClick: informarFimViagemClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.FianlizarViagem, idGrid: guid(), visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaFimViagemClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaFimViagemClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.AvancarEtapa, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function LoadFimViagem() {
    _fimViagem = new FimViagem();
    KoBindings(_fimViagem, "knockoutFimViagem");
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function documentosPesagemClick(e) {
    _documentosPesagem.CodigoFluxoGestaoPatio.val(e.Codigo.val());
    _documentosPesagem.CodigoCarga.val(e.Carga.val());
    
    buscarDocumentosPesagem(e.Codigo.val());
    buscarDocumentosPesagemDevolucao(e.Codigo.val());
    buscarDocumentosPesagemNotaFiscalComplementar(e.Codigo.val());
    buscarDocumentosPesagemTicketBalanca(e.Codigo.val());
    buscarDocumentosFornecedor(e.Carga.val());
    buscarDocumentosPesagemNFRemessaIndustrializacao(e.Codigo.val());
    
    Global.abrirModal('divModalDocumentosPesagem');

    $("#divModalDocumentosPesagem").one('hidden.bs.modal', function () {
        LimparCampos(_documentosPesagem);
        _documentosPesagem.Anexos.val([]);
        _listaAnexoFornecedorPesagem.Anexos.val([]);
        Global.ResetarMultiplasAbas();
    });
}

function informarFimViagemClick() {
    executarReST("FimViagem/InformarFimViagem", { Codigo: _fimViagem.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.ViagemFianlizadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesFimViagem();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function observacoesEtapaFimViagemClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.FimViagem);
}

function salvarDadosEtapaFimViagemClick() {
    Salvar(_fimViagem, "FimViagem/SalvarInformacaoEtapa", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.DadosSalvosComSucesso);
                fecharModalDetalhesFimViagem();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function voltarEtapaFimViagemClick() {
    executarReST("FimViagem/VoltarEtapaFimViagem", { Codigo: _fimViagem.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesFimViagem();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function avancarEtapaFimViagemClick() {
    executarReST("FimViagem/AvancarEtapaFimViagem", { Codigo: _fimViagem.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.GestaoPatio.FluxoPatio.EtapaAvancadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesFimViagem();
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

function ExibirFimViagem(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    executarReST("FimViagem/BuscarPorCodigo", { Codigo: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _fimViagem.InformarFimViagem.visible(false);

                PreencherObjetoKnout(_fimViagem, retorno);

                _fimViagem.DocumentosPesagem.visible(retorno.Data.ExibirDocumentosPesagem);

                var permiteEditarEtapa = retorno.Data.PermitirEditarEtapa && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarFimViagem, _PermissoesPersonalizadasFluxoPatio);
                
                _fimViagem.InformarFimViagem.visible(retorno.Data.ViagemAberta && permiteEditarEtapa);

                if (_configuracaoGestaoPatio.FimViagemPermiteVoltar && permiteEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _fimViagem.VoltarEtapa.visible(!primeiraEtapa && retorno.Data.ViagemAberta)
                }

                exibirModalDetalhesFimViagem();
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

function exibirModalDetalhesFimViagem() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_fimViagem);

    Global.abrirModal('divModalFimViagem');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesFimViagem();
    });

    $("#divModalFimViagem").one('hidden.bs.modal', function () {
        LimparCampos(_fimViagem);
    });
}

function fecharModalDetalhesFimViagem() {
    Global.fecharModal('divModalFimViagem');
}

// #endregion Funções Privadas
