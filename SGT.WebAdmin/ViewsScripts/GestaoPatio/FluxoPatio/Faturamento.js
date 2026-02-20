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

var _faturamentoFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var FaturamentoFluxoPatio = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.PreCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
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

    this.ExibirObservacao = PropertyEntity({ eventClick: function () { exibirObservacaoFluxoPatio(_faturamentoFluxoPatio.ObservacaoFluxoPatio.val()); }, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ExibirObservacoes });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaFaturamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.Observacoes, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ImprimirCapaViagem = PropertyEntity({ eventClick: imprimirCapaViagemClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ImprimirCapaViagem, visible: _configuracaoGestaoPatio.FaturamentoPermiteImprimirCapaViagem });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaFaturamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadFaturamentoFluxoPatio() {
    _faturamentoFluxoPatio = new FaturamentoFluxoPatio();
    KoBindings(_faturamentoFluxoPatio, "knockoutFaturamentoFluxoPatio");

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _faturamentoFluxoPatio.Transportador.visible(false);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos


function voltarEtapaFaturamentoClick() {
    executarReST("Faturamento/VoltarEtapa", { Codigo: _faturamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalFaturamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function observacoesEtapaFaturamentoClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.Faturamento);
}

function imprimirCapaViagemClick(e) {
    executarDownload("Faturamento/ImprimirCapaViagem", { Codigo: _faturamentoFluxoPatio.Codigo.val() });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirDetalhesFaturamento(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    var permiteExibirModalFaturamento = _configuracaoGestaoPatio.FaturamentoPermiteImprimirCapaViagem || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_PermiteVoltarEtapaFaturamento, _PermissoesPersonalizadasFluxoPatio);

    if (!permiteExibirModalFaturamento)
        return ObterDetalhesCargaFluxoClick(knoutFluxo, opt);
    
    executarReST("Faturamento/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _faturamentoFluxoPatio.VoltarEtapa.visible(false);

                PreencherObjetoKnout(_faturamentoFluxoPatio, retorno);

                var permiteEditarEtapa = retorno.Data.PermitirEditarEtapa && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_SolicitarVeiculo, _PermissoesPersonalizadasFluxoPatio);

                if (_configuracaoGestaoPatio.FaturamentoPermiteVoltar && permiteEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _faturamentoFluxoPatio.VoltarEtapa.visible(!primeiraEtapa)
                }

                exibirModalFaturamento();
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

function exibirModalFaturamento() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_faturamentoFluxoPatio);

    Global.abrirModal('divModalFaturamentoFluxoPatio');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalFaturamento();
    });

    $("#divModalFaturamentoFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_faturamentoFluxoPatio);
    });
}

function fecharModalFaturamento() {
    Global.fecharModal('divModalFaturamentoFluxoPatio');
}

// #endregion Funções Privadas
