/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaControleExpedicao.js" />
/// <reference path="FluxoPatio.js" />

// #region Objetos Globais do Arquivo

var _expedicaoFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ExpedicaoFluxoPatio = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoFluxoGestaoPatio = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoPreCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.SituacaoCargaControleExpedicao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Auditar = PropertyEntity({ visible: ko.observable(false), eventClick: auditarExpedicaoClick });

    this.NumeroCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Carga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroPreCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.PreCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DescricaoSituacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Transportador = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Transportador.getFieldDescription(), val: ko.observable(""), def: "" });

    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.DataParaCarregamento.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataPrevisao = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.DataPrevisaoCarregamento.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Motorista = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Motorista.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Veiculo.getFieldDescription(), val: ko.observable(""), def: "" });

    this.ImprimirComprovanteCargaInformada = PropertyEntity({ eventClick: function (e) { imprimirComprovanteCargaInformada(e.CodigoCarga.val()); }, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ImprimirComprovanteDeCarga, visible: ko.observable(isPermitirImprimirComprovanteCargaInformada()) });
    this.FechamentoPesagem = PropertyEntity({ eventClick: fechamentoPesagemExpedicaoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.FechamentoPesagem, visible: ko.observable(false) });
    this.InformarInicioCarregamento = PropertyEntity({ eventClick: informarInicioCarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.InformarInicioDoCarregamento, idGrid: guid(), visible: ko.observable(false) });
    this.InformarTerminoCarregamento = PropertyEntity({ eventClick: informarTerminoCarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.InformarTerminoDoCarregamento, idGrid: guid(), visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaExpedicaoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.Observacoes, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaExpedicaoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaExpedicaoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.AvancarEtapa, visible: ko.observable(false) });
    this.DownloadRomaneios = PropertyEntity({ text: "Download Romaneio", visible: ko.observable(true) });

    this.DownloadRomaneioTotalizador = PropertyEntity({
        eventClick: downloadRomaneioTotalizadorPDF, type: types.event, text: "Romaneio Totalizador", idGrid: guid(), visible: ko.observable(true)
    });

    this.DownloadRomaneioDetalhado = PropertyEntity({
        eventClick: downloadRomaneioDetalhadoPDF, type: types.event, text: "Romaneio Detalhado", idGrid: guid(), visible: ko.observable(true)
    });
    this.DownloadRomaneioDetalhadoResumido = PropertyEntity({
        eventClick: downloadRomaneioDetalhadoResumidoPDF, type: types.event, text: "Romaneio Detalhado Resumido", idGrid: guid(), visible: ko.observable(true)
    });
};

// #endregion Classes

// #region Funções de Inicialização

function LoadExpedicaoFluxoPatio() {
    _expedicaoFluxoPatio = new ExpedicaoFluxoPatio();
    KoBindings(_expedicaoFluxoPatio, "knockoutExpedicaoFluxoPatio");

    adicionaSpanDataExpedicao();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function auditarExpedicaoClick() {
    var _fn = OpcaoAuditoria("CargaControleExpedicao", "Codigo", _expedicaoFluxoPatio);

    _fn({ Codigo: _expedicaoFluxoPatio.Codigo.val() });
}

function informarInicioCarregamentoClick(e) {
    executarReST("CargaControleExpedicao/InformarInicioCarregamento", { Codigo: e.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.ChegadaInformadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesExpedicao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function fechamentoPesagemExpedicaoClick(e) {
    BuscarFechamentoPesagem(e.CodigoFluxoGestaoPatio.val());

    Global.abrirModal('divModalFechamentoPesagem');

    $("#divModalFechamentoPesagem").one('hidden.bs.modal', function () {
        LimparCampos(_fechamentoPesagem);
    });
}

function informarTerminoCarregamentoClick(e) {
    executarReST("CargaControleExpedicao/InformarTerminoCarregamento", { Codigo: e.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.CarregamentoLiberadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesExpedicao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function observacoesEtapaExpedicaoClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.Expedicao);
}

function voltarEtapaExpedicaoClick(e) {
    executarReST("CargaControleExpedicao/VoltarEtapa", { Codigo: e.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesExpedicao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function avancarEtapaExpedicaoClick(e) {
    executarReST("CargaControleExpedicao/AvancarEtapa", { Codigo: e.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaAvancadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesExpedicao();
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

function ExibirDetalhesExpedicaoFluxoPatio(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    var dados = { FluxoGestaoPatio: knoutFluxo.Codigo.val() }

    executarReST("CargaControleExpedicao/BuscarPorCodigo", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _expedicaoFluxoPatio.InformarInicioCarregamento.visible(false);
                _expedicaoFluxoPatio.InformarTerminoCarregamento.visible(false);
                _expedicaoFluxoPatio.VoltarEtapa.visible(false);

                PreencherObjetoKnout(_expedicaoFluxoPatio, retorno);

                _expedicaoFluxoPatio.DataPrevisao.val(retorno.Data.DataCarregamentoPrevista);

                if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarExpedicao, _PermissoesPersonalizadasFluxoPatio)) {
                    if (_expedicaoFluxoPatio.SituacaoCargaControleExpedicao.val() == EnumSituacaoCargaControleExpedicao.AgInicioCarregamento)
                        _expedicaoFluxoPatio.InformarInicioCarregamento.visible(true);

                    if (_expedicaoFluxoPatio.SituacaoCargaControleExpedicao.val() == EnumSituacaoCargaControleExpedicao.AguardandoLiberacao)
                        _expedicaoFluxoPatio.InformarTerminoCarregamento.visible(true);

                    if (_expedicaoFluxoPatio.InformarInicioCarregamento.visible() || _expedicaoFluxoPatio.InformarTerminoCarregamento.visible())
                        _expedicaoFluxoPatio.VoltarEtapa.visible(true);
                }

                exibirModalDetalhesExpedicao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });

    if (knoutFluxo.GuaritaEntradaPermiteInformacoesPesagem.val()) {
        _expedicaoFluxoPatio.AvancarEtapa.visible(true);
        _expedicaoFluxoPatio.FechamentoPesagem.visible(true);
    }
    else {
        _expedicaoFluxoPatio.AvancarEtapa.visible(false);
        _expedicaoFluxoPatio.FechamentoPesagem.visible(false);
    }
}

// #endregion Funções Públicas

// #region Funções Privadas

function adicionaSpanDataExpedicao() {
    var $title = $("#divModalExpedicaoFluxoPatio").find(".modal-header h4");

    _$spanTituloExpedicao = $("<span></span>");

    $title.append(_$spanTituloExpedicao);
}

function exibirModalDetalhesExpedicao() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_expedicaoFluxoPatio);

    Global.abrirModal('divModalExpedicaoFluxoPatio');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesExpedicao();
    });

    $("#divModalExpedicaoFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_expedicaoFluxoPatio);
    });
}

function fecharModalDetalhesExpedicao() {
    Global.fecharModal('divModalExpedicaoFluxoPatio');
}

// #endregion Funções Privadas
