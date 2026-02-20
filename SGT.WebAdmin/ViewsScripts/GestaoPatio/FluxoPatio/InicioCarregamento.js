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

var _inicioCarregamentoFluxoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var InicioCarregamentoFluxoPatio = function () {
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
    this.DataLacre = PropertyEntity({ text:Localization.Resources.GestaoPatio.FluxoPatio.DataLacre.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroDoca = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.NumeroDoca.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Remetente = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Fornecedor.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDeCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.TipoDaOperacao.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Transportador = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Transportador.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.Veiculo.getFieldDescription(), val: ko.observable(""), def: "" });

    this.Pesagem = PropertyEntity({ getType: typesKnockout.decimal, configDecimal: { allowZero: false }, maxlength: 15, text: Localization.Resources.GestaoPatio.FluxoPatio.Pesagem.getFieldDescription(), required: false, visible: ko.observable(false) });

    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaInicioCarregamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(false) });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaInicioCarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.Observacoes, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoInicioCarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: rejeitarEtapaInicioCarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaInicioCarregamentoClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
    this.ImprimirViaCega = PropertyEntity({ eventClick: downloadViaCegaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ImprimirViaCega, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadInicioCarregamentoFluxoPatio() {
    _inicioCarregamentoFluxoPatio = new InicioCarregamentoFluxoPatio();
    KoBindings(_inicioCarregamentoFluxoPatio, "knockoutInicioCarregamentoFluxoPatio");

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _inicioCarregamentoFluxoPatio.Transportador.visible(false);
}

// #endregion Funções de Inicialização

// #region Funções Públicas

function exibirDetalhesInicioCarregamento(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    executarReST("InicioCarregamento/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _inicioCarregamentoFluxoPatio.AvancarEtapa.visible(false);
                _inicioCarregamentoFluxoPatio.ReabrirFluxo.visible(false);
                _inicioCarregamentoFluxoPatio.RejeitarEtapa.visible(false);
                _inicioCarregamentoFluxoPatio.VoltarEtapa.visible(false);

                PreencherObjetoKnout(_inicioCarregamentoFluxoPatio, retorno);

                var permiteEditarEtapa = VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarInicioCarregamento, _PermissoesPersonalizadasFluxoPatio) && retorno.Data.PermitirEditarEtapa;

                _inicioCarregamentoFluxoPatio.Pesagem.visible(retorno.Data.PermiteInformarPesagem);
                _inicioCarregamentoFluxoPatio.AvancarEtapa.visible(permiteEditarEtapa);

                if (_configuracaoGestaoPatio.InicioCarregamentoPermiteVoltar && permiteEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _inicioCarregamentoFluxoPatio.VoltarEtapa.visible(!primeiraEtapa)

                    if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() == EnumEtapaFluxoGestaoPatio.InicioCarregamento) {
                        _inicioCarregamentoFluxoPatio.RejeitarEtapa.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                        _inicioCarregamentoFluxoPatio.ReabrirFluxo.visible(_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado);
                    }
                }

                exibirModalDetalhesInicioCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Associadas a Eventos

function avancarEtapaInicioCarregamentoClick() {
    var dados = {
        Codigo: _inicioCarregamentoFluxoPatio.Codigo.val(),
        Pesagem: _inicioCarregamentoFluxoPatio.Pesagem.val()
    };

    executarReST("InicioCarregamento/AvancarEtapa", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.CarregamentoIniciadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function observacoesEtapaInicioCarregamentoClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.InicioCarregamento);
}

function reabrirFluxoInicioCarregamentoClick() {
    executarReST("InicioCarregamento/ReabrirFluxo", { Codigo: _inicioCarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReabertoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function rejeitarEtapaInicioCarregamentoClick() {
    executarReST("InicioCarregamento/RejeitarEtapa", { Codigo: _inicioCarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.InicioDoCarregamentoRejeitadoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function voltarEtapaInicioCarregamentoClick() {
    executarReST("InicioCarregamento/VoltarEtapa", { Codigo: _inicioCarregamentoFluxoPatio.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesInicioCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function downloadViaCegaClick(e) {
    executarDownload("FluxoPatio/DownloadInicioCarregamentoViaCega", { Codigo: e.Carga.val() });
}

// #endregion Funções Associadas a Eventos

// #region Funções Privadas

function exibirModalDetalhesInicioCarregamento() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_inicioCarregamentoFluxoPatio);
        
    Global.abrirModal('divModalInicioCarregamentoFluxoPatio');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesInicioCarregamento();
    });

    $("#divModalInicioCarregamentoFluxoPatio").one('hidden.bs.modal', function () {
        LimparCampos(_inicioCarregamentoFluxoPatio);
    });
}

function fecharModalDetalhesInicioCarregamento() {
    Global.fecharModal('divModalInicioCarregamentoFluxoPatio');
}

// #endregion Funções Privadas
