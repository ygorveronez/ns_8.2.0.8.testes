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

var _separacaoMercadoria;
var _gridSeparadores;

// #endregion Objetos Globais do Arquivo

// #region Classes

var SeparacaoMercadoria = function () {
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

    this.NumeroCarregadores = PropertyEntity({ text: Localization.Resources.GestaoPatio.FluxoPatio.NumeroDeCarregadores.getFieldDescription(), getType: typesKnockout.int, enable: ko.observable(true), maxlength: 15, visible: ko.observable(false) });
    this.ResponsavelCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.GestaoPatio.FluxoPatio.ResponsavelPeloCarregamento.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.AdicionarResponsavelSeparacao = PropertyEntity({ type: types.event, idBtnSearch: guid(), text: Localization.Resources.GestaoPatio.FluxoPatio.AdicionarResponsavelSeparacao, eventClick: abrirModalResponsavelSeparacao, visible: ko.observable(true) });
    this.Separadores = PropertyEntity({ type: types.local, idGrid: guid(), val: ko.observableArray([]) });

    this.Separadores.val.subscribe(function () {
        recarregarGridSeparadores();
    });

    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaSeparacaoMercadoriaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(false) });
    this.ExibirObservacao = PropertyEntity({ eventClick: function () { exibirObservacaoFluxoPatio(_separacaoMercadoria.ObservacaoFluxoPatio.val()); }, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ExibirObservacao });
    this.ObservacoesEtapa = PropertyEntity({ eventClick: observacoesEtapaSeparacaoMercadoriaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.Observacoes, visible: _configuracaoGestaoPatio.HabilitarObservacaoEtapa });
    this.ReabrirFluxo = PropertyEntity({ eventClick: reabrirFluxoSeparacaoMercadoriaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.ReabrirFluxo, visible: ko.observable(false) });
    this.RejeitarEtapa = PropertyEntity({ eventClick: rejeitarEtapaSeparacaoMercadoriaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.RejeitarEtapa, visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaSeparacaoMercadoriaClick, type: types.event, text: Localization.Resources.GestaoPatio.FluxoPatio.VoltarEtapa, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadSeparacaoMercadoria() {
    _separacaoMercadoria = new SeparacaoMercadoria();
    KoBindings(_separacaoMercadoria, "knockoutSeparacaoMercadoria");

    new BuscarFuncionario(_separacaoMercadoria.ResponsavelCarregamento);

    if (_configuracaoGestaoPatio.OcultarTransportador)
        _separacaoMercadoria.Transportador.visible(false);

    loadGridSeparacaoMercadoriaSeparadores();
    loadResponsavelSeparacao();
}

function loadGridSeparacaoMercadoriaSeparadores() {
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, evento: "onclick", metodo: removerSeparadorClick, tamanho: "10", icone: "", visibilidade: visibilidadeOpcaoRemover };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoRemover], tamanho: "10" };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoResponsavel", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "60%", className: "text-align-left" },
        { data: "CapacidadeSeparacao", title: Localization.Resources.GestaoPatio.FluxoPatio.CapacidadeDeSeparacao, width: "20%", className: "text-align-right" }
    ];

    _gridSeparadores = new BasicDataTable(_separacaoMercadoria.Separadores.idGrid, header, menuOpcoes, null, null, 5);
    _gridSeparadores.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function avancarEtapaSeparacaoMercadoriaClick() {
    var dados = {
        Codigo: _separacaoMercadoria.Codigo.val(),
        NumeroCarregadores: _separacaoMercadoria.NumeroCarregadores.val(),
        ResponsavelCarregamento: _separacaoMercadoria.ResponsavelCarregamento.codEntity(),
        Separadores: JSON.stringify(_separacaoMercadoria.Separadores.val())
    };
    
    executarReST("SeparacaoMercadoria/AvancarEtapa", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.SeparacaoDeMercadoriaFianlizadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesSeparacaoMercadoria();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function observacoesEtapaSeparacaoMercadoriaClick() {
    buscarObservacoesEtapa(EnumEtapaFluxoGestaoPatio.SeparacaoMercadoria);
}

function reabrirFluxoSeparacaoMercadoriaClick() {
    executarReST("SeparacaoMercadoria/ReabrirFluxo", { Codigo: _separacaoMercadoria.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.FluxoReabertoComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesSeparacaoMercadoria();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function rejeitarEtapaSeparacaoMercadoriaClick() {
    executarReST("SeparacaoMercadoria/RejeitarEtapa", { Codigo: _separacaoMercadoria.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.SeparacaoDeMercadoriaRejeitadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesSeparacaoMercadoria();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function voltarEtapaSeparacaoMercadoriaClick() {
    executarReST("SeparacaoMercadoria/VoltarEtapa", { Codigo: _separacaoMercadoria.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.GestaoPatio.FluxoPatio.EtapaRetornadaComSucesso);
                atualizarFluxoPatio();
                fecharModalDetalhesSeparacaoMercadoria();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function removerSeparadorClick(registroSelecionado) {
    var registros = _gridSeparadores.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == registroSelecionado.Codigo) {
            registros.splice(i, 1);
            break;
        }
    }

    _gridSeparadores.CarregarGrid(registros);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirDetalhesSeparacaoMercadoria(knoutFluxo, opt) {
    _fluxoAtual = knoutFluxo;

    executarReST("SeparacaoMercadoria/BuscarPorCodigo", { FluxoGestaoPatio: knoutFluxo.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _separacaoMercadoria.AvancarEtapa.visible(false);
                _separacaoMercadoria.ReabrirFluxo.visible(false);
                _separacaoMercadoria.RejeitarEtapa.visible(false);
                _separacaoMercadoria.VoltarEtapa.visible(false);

                _separacaoMercadoria.Separadores.val(retorno.Data.ResponsaveisSeparacao);
                _separacaoMercadoria.NumeroCarregadores.visible(retorno.Data.PermiteInformarDadosCarregadores);
                _separacaoMercadoria.ResponsavelCarregamento.visible(retorno.Data.PermiteInformarDadosCarregadores);
                
                PreencherObjetoKnout(_separacaoMercadoria, retorno);
                
                _separacaoMercadoria.AdicionarResponsavelSeparacao.visible(_separacaoMercadoria.Situacao.val() == EnumSituacaoSeparacaoMercadoria.AguardandoSeparacaoMercadoria);

                var permiteEditarEtapa = retorno.Data.PermitirEditarEtapa && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FluxoGestaoPatio_InformarSeparacaoMercadoria, _PermissoesPersonalizadasFluxoPatio);

                _separacaoMercadoria.AvancarEtapa.visible(permiteEditarEtapa);

                if (_configuracaoGestaoPatio.SeparacaoMercadoriaPermiteVoltar && permiteEditarEtapa) {
                    var primeiraEtapa = _fluxoAtual.EtapaAtual.val() == 0;

                    _separacaoMercadoria.VoltarEtapa.visible(!primeiraEtapa)

                    if (knoutFluxo.EtapaFluxoGestaoPatioAtual.val() == EnumEtapaFluxoGestaoPatio.SeparacaoMercadoria) {
                        _separacaoMercadoria.RejeitarEtapa.visible(_configuracaoGestaoPatio.PermitirRejeicaoFluxo && _fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Aguardando);
                        _separacaoMercadoria.ReabrirFluxo.visible(_fluxoAtual.SituacaoEtapaFluxoGestaoPatio.val() == EnumSituacaoEtapaFluxoGestaoPatio.Rejeitado);
                    }
                }

                exibirModalDetalhesSeparacaoMercadoria();
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

function exibirModalDetalhesSeparacaoMercadoria() {
    if (edicaoEtapaFluxoPatioBloqueada())
        ocultarBotoesEtapa(_separacaoMercadoria);

    Global.abrirModal('divModalSeparacaoMercadoria');

    $(window).one('keyup', function (e) {
        if (e.keyCode == 27)
            fecharModalDetalhesSeparacaoMercadoria();
    });

    $("#divModalSeparacaoMercadoria").one('hidden.bs.modal', function () {
        LimparCampos(_separacaoMercadoria);
    });
}

function fecharModalDetalhesSeparacaoMercadoria() {
    Global.fecharModal('divModalSeparacaoMercadoria');
}

// #endregion Funções Privadas
