/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="ConfiguracaoContratoFrete.js" />
/// <reference path="../../Consultas/ConfiguracaoCIOT.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTipoTerceiro;
var _tipoTerceiro;
var _pesquisaTipoTerceiro;
var _gridTipoTerceiro;

/*
 * Declaração das Classes
 */

var CRUDTipoTerceiro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Pessoas.TipoTerceiro.Adicionar.getFieldDescription(), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Pessoas.TipoTerceiro.Atualizar.getFieldDescription(), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Pessoas.TipoTerceiro.Cancelar.getFieldDescription() });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Pessoas.TipoTerceiro.Excluir.getFieldDescription(), visible: ko.observable(false) });
}

var TipoTerceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Pessoas.TipoTerceiro.Descricao.getFieldDescription(), issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pessoas.TipoTerceiro.CodigoDeIntegracao.getFieldDescription(), required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Pessoas.TipoTerceiro.Situacao.getFieldDescription(), val: ko.observable(true), options: _status, def: true });

    this.ConfiguracaoContratoFrete = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });
    this.ConfiguracaoCIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.TipoTerceiro.ConfiguracaoCIOT.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.ReterImpostosContratoFrete = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.TornarCampoINSSeReterImpostoTrazerComoSim ? true : ""), options: Global.ObterOpcoesNaoSelecionadoBooleano(Localization.Resources.Enumeradores.SimNao.Sim, Localization.Resources.Enumeradores.SimNao.Nao), def: ko.observable(_CONFIGURACAO_TMS.TornarCampoINSSeReterImpostoTrazerComoSim ? true : ""), text: Localization.Resources.Pessoas.TipoTerceiro.ReterImpostos.getFieldDescription() });
}

var PesquisaTipoTerceiro = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Pessoas.TipoTerceiro.Descricao.getFieldDescription(), required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: Localization.Resources.Pessoas.TipoTerceiro.Situacao.getFieldDescription(), val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: Localization.Resources.Pessoas.TipoTerceiro.ExibirFiltros.getFieldDescription(), idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridTipoTerceiro, type: types.event, text: Localization.Resources.Pessoas.TipoTerceiro.Pesquisar.getFieldDescription(), idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTipoTerceiro() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "TipoTerceiro/ExportarPesquisa", titulo: Localization.Resources.Pessoas.TipoTerceiro.TiposDeTerceiros.getFieldDescription() };

    _gridTipoTerceiro = new GridViewExportacao(_pesquisaTipoTerceiro.Pesquisar.idGrid, "TipoTerceiro/Pesquisa", _pesquisaTipoTerceiro, menuOpcoes, configuracoesExportacao);
    _gridTipoTerceiro = new GridView(_pesquisaTipoTerceiro.Pesquisar.idGrid, "TipoTerceiro/Pesquisa", _pesquisaTipoTerceiro, menuOpcoes, undefined, null, null, null, null, null, null, null, configuracoesExportacao, undefined, undefined, null);
    _gridTipoTerceiro.CarregarGrid();
}

function loadTipoTerceiro() {
    _tipoTerceiro = new TipoTerceiro();
    KoBindings(_tipoTerceiro, "knockoutTipoTerceiro");

    HeaderAuditoria("TipoTerceiro", _tipoTerceiro);

    _CRUDTipoTerceiro = new CRUDTipoTerceiro();
    KoBindings(_CRUDTipoTerceiro, "knockoutCRUDTipoTerceiro");

    _pesquisaTipoTerceiro = new PesquisaTipoTerceiro();
    KoBindings(_pesquisaTipoTerceiro, "knockoutPesquisaTipoTerceiro", false, _pesquisaTipoTerceiro.Pesquisar.id);

    //$("#liTabConfiguracoesContratoFrete").hide();

    BuscarConfiguracaoCIOT(_tipoTerceiro.ConfiguracaoCIOT,
        Localization.Resources.Pessoas.TipoTerceiro.ConsultaDeOperadorasDeCIOT,
        Localization.Resources.Pessoas.TipoTerceiro.OperadorasDeCIOT,
        RetornoConsultaConfiguracaoCIOT);

    loadGridTipoTerceiro();
    loadConfiguracaoContratoFrete();
}

function RetornoConsultaConfiguracaoCIOT(data) {
    _tipoTerceiro.ConfiguracaoCIOT.val(data.Descricao);
    _tipoTerceiro.ConfiguracaoCIOT.codEntity(data.Codigo);
}



/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    _tipoTerceiro.ConfiguracaoContratoFrete.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoContratoFrete)));

    Salvar(_tipoTerceiro, "TipoTerceiro/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridTipoTerceiro();
                limparCamposTipoTerceiro();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    _tipoTerceiro.ConfiguracaoContratoFrete.val(JSON.stringify(RetornarObjetoPesquisa(_configuracaoContratoFrete)));

    Salvar(_tipoTerceiro, "TipoTerceiro/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridTipoTerceiro();
                limparCamposTipoTerceiro();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposTipoTerceiro();
}

function editarClick(registroSelecionado) {
    limparCamposTipoTerceiro();

    _tipoTerceiro.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tipoTerceiro, "TipoTerceiro/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTipoTerceiro.ExibirFiltros.visibleFade(false);
                controlarBotoesHabilitados();

                if (retorno.Data.ConfiguracaoContratoFrete != null)
                    PreencherObjetoKnout(_configuracaoContratoFrete, { Data: retorno.Data.ConfiguracaoContratoFrete });
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_tipoTerceiro, "TipoTerceiro/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridTipoTerceiro();
                    limparCamposTipoTerceiro();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _tipoTerceiro.Codigo.val() > 0;

    _CRUDTipoTerceiro.Atualizar.visible(isEdicao);
    _CRUDTipoTerceiro.Excluir.visible(isEdicao);
    _CRUDTipoTerceiro.Adicionar.visible(!isEdicao);
}

function limparCamposTipoTerceiro() {
    controlarBotoesHabilitados();
    LimparCampos(_tipoTerceiro);
    LimparCampos(_configuracaoContratoFrete);
    _configuracaoContratoFrete.ConfiguracaoPercentualAdiantamentoContratoFrete.val(EnumTipoTerceiroConfiguracaoContratoFrete.PorPessoa);
    _configuracaoContratoFrete.ConfiguracaoPercentualAbastecimentoContratoFrete.val(EnumTipoTerceiroConfiguracaoContratoFrete.PorPessoa);
    _configuracaoContratoFrete.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete.val(EnumTipoTerceiroConfiguracaoContratoFrete.PorPessoa);
    _configuracaoContratoFrete.ConfiguracaoDiasVencimentoSaldoContratoFrete.val(EnumTipoTerceiroConfiguracaoContratoFrete.PorPessoa);
}

function recarregarGridTipoTerceiro() {
    _gridTipoTerceiro.CarregarGrid();
}