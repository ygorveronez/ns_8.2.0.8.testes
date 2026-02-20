/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="Etapas.js" />
/// <reference path="EtapasDestino.js" />

// #region Objetos Globais do Arquivo

var _crudSequenciaGestaoPatio;
var _gridSequenciaGestaoPatio;
var _pesquisaSequenciaGestaoPatio;
var _sequenciaGestaoPatio;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaSequenciaGestaoPatio = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Filial.getFieldDescription(), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoOperacao.getFieldDescription(), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            recarregarGridSequenciaGestaoPatio();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid()
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var SequenciaGestaoPatio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Gerais.Geral.Filial.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Gerais.Geral.TipoOperacao.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });
    this.SequenciaGestaoPatio = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: null, val: GetSetGestaoPatio });
    this.SequenciaGestaoPatioDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: null, val: GetSetGestaoPatioDestino });
    this.OrdemGestaoPatio = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: null, val: GetSetOrdemGestaoPatio });
    this.OrdemGestaoPatioDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: null, val: GetSetOrdemGestaoPatioDestino });
};

var CrudSequenciaGestaoPatio = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadGridSequenciaGestaoPatio() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridSequenciaGestaoPatio = new GridView(_pesquisaSequenciaGestaoPatio.Pesquisar.idGrid, "SequenciaGestaoPatio/Pesquisa", _pesquisaSequenciaGestaoPatio, menuOpcoes);
    _gridSequenciaGestaoPatio.CarregarGrid();
}

function loadSequenciaGestaoPatio() {
    _pesquisaSequenciaGestaoPatio = new PesquisaSequenciaGestaoPatio();
    KoBindings(_pesquisaSequenciaGestaoPatio, "knockoutPesquisaSequenciaGestaoPatio", false, _pesquisaSequenciaGestaoPatio.Pesquisar.id);

    _sequenciaGestaoPatio = new SequenciaGestaoPatio();
    KoBindings(_sequenciaGestaoPatio, "knockoutSequenciaGestaoPatio");

    _crudSequenciaGestaoPatio = new CrudSequenciaGestaoPatio();
    KoBindings(_crudSequenciaGestaoPatio, "knockoutCrudSequenciaGestaoPatio");

    HeaderAuditoria("SequenciaGestaoPatio", _sequenciaGestaoPatio);

    new BuscarFilial(_pesquisaSequenciaGestaoPatio.Filial);
    new BuscarTiposOperacao(_pesquisaSequenciaGestaoPatio.TipoOperacao);
    new BuscarFilial(_sequenciaGestaoPatio.Filial);
    new BuscarTiposOperacao(_sequenciaGestaoPatio.TipoOperacao);

    loadGestaoPatio();
    loadGestaoPatioDestino();
    loadGridSequenciaGestaoPatio();

    ObterIntegracoesHabilitadas();
}

function ObterIntegracoesHabilitadas() {
    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            if (retorno.Data.TiposExistentes != null && retorno.Data.TiposExistentes.length > 0) {

                if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Deca; })) {
                    _gestaoPatio.BalancaGuaritaSaida.visible(true);
                    _gestaoPatio.BalancaGuaritaEntrada.visible(true);
                    _gestaoPatioDestino.BalancaGuaritaSaida.visible(true);
                    _gestaoPatioDestino.BalancaGuaritaEntrada.visible(true);
                }

                if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Qbit || o == EnumTipoIntegracao.BalancaKIKI; })) {
                    _gestaoPatio.GuaritaEntradaTipoIntegracaoBalanca.visible(true);
                    _gestaoPatio.GuaritaSaidaTipoIntegracaoBalanca.visible(true);
                    _gestaoPatioDestino.GuaritaEntradaTipoIntegracaoBalanca.visible(true);
                    _gestaoPatioDestino.GuaritaSaidaTipoIntegracaoBalanca.visible(true);
                }
            }
        }
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick() {
    var sequenciaGestaoPatioSalvar = obterSequenciaGestaoPatioSalvar();

    if (!sequenciaGestaoPatioSalvar)
        return;

    executarReST("Filial/AdicionarSequenciaGestaoPatio", sequenciaGestaoPatioSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                recarregarGridSequenciaGestaoPatio();
                limparCamposSequenciaGestaoPatio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function atualizarClick() {
    var sequenciaGestaoPatioSalvar = obterSequenciaGestaoPatioSalvar();

    if (!sequenciaGestaoPatioSalvar)
        return;

    executarReST("Filial/AtualizarSequenciaGestaoPatio", sequenciaGestaoPatioSalvar, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                recarregarGridSequenciaGestaoPatio();
                limparCamposSequenciaGestaoPatio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function cancelarClick() {
    limparCamposSequenciaGestaoPatio();
}

function editarClick(registroSelecionado) {
    limparCamposSequenciaGestaoPatio();

    executarReST("Filial/BuscarSequenciaGestaoPatio", { Filial: registroSelecionado.CodigoFilial, TipoOperacao: registroSelecionado.CodigoTipoOperacao, }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaSequenciaGestaoPatio.ExibirFiltros.visibleFade(false);
                _sequenciaGestaoPatio.TipoOperacao.enable(false);

                PreencherObjetoKnout(_sequenciaGestaoPatio, retorno);
                verificaVisibilidadeIntegracaoP44(retorno.Data.Filial.GerarIntegracaoP44);
                controlarBotoesHabilitados(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function excluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaExcluirRegistro, function () {
        executarReST("Filial/ExcluirSequenciaGestaoPatio", { Filial: _sequenciaGestaoPatio.Filial.codEntity(), TipoOperacao: _sequenciaGestaoPatio.TipoOperacao.codEntity() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    recarregarGridSequenciaGestaoPatio();
                    limparCamposSequenciaGestaoPatio();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function isCadastroEmEdicao() {
    return _crudSequenciaGestaoPatio.Atualizar.visible();
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesHabilitados(isEdicao) {
    _crudSequenciaGestaoPatio.Adicionar.visible(!isEdicao);
    _crudSequenciaGestaoPatio.Atualizar.visible(isEdicao);
    _crudSequenciaGestaoPatio.Excluir.visible(isEdicao);
}

function limparCamposSequenciaGestaoPatio() {
    LimparCampos(_sequenciaGestaoPatio);
    limparCamposGestaoPatio();
    limparCamposGestaoPatioDestino();
    controlarBotoesHabilitados(false);
    _sequenciaGestaoPatio.TipoOperacao.enable(true);

    Global.ResetarAbas();
}

function obterSequenciaGestaoPatioSalvar() {
    if (!ValidarCamposObrigatorios(_sequenciaGestaoPatio)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return undefined;
    }

    return RetornarObjetoPesquisa(_sequenciaGestaoPatio);
}

function recarregarGridSequenciaGestaoPatio() {
    _gridSequenciaGestaoPatio.CarregarGrid();
}
// #endregion Funções Privadas
