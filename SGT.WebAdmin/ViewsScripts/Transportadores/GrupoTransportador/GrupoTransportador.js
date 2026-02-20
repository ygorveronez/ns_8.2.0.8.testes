/// <reference path="Empresa.js" />


/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDGrupoTransportador;
var _grupoTransportador;
var _pesquisaGrupoTransportador;
var _gridGrupoTransportador;

/*
 * Declaração das Classes
 */

var PesquisaGrupoTransportador = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable(""), def: "", maxlength: 150 });
    this.Situacao = PropertyEntity({ text: "Situação:", options: _statusPesquisa, val: ko.observable(0), def: 0 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", val: ko.observable(""), def: "", maxlength: 50 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridGrupoTransportador, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var GrupoTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", val: ko.observable(""), maxlength: 150, required: ko.observable(true) });
    this.Ativo = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação", val: ko.observable(""), def: "", maxlength: 1000 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", val: ko.observable(""), def: "", maxlength: 50 });
    this.ParquearDoumentosAutomaticamente = PropertyEntity({ text: "Parquear Documentos Automaticamente para Transportadores deste Grupo: ", val: ko.observable(false), options: _status, def: false, required: ko.observable(false) });

    this.Empresas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Integracoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
}

var CRUDGrupoTransportador = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGrupoTransportador() {
    _pesquisaGrupoTransportador = new PesquisaGrupoTransportador();
    KoBindings(_pesquisaGrupoTransportador, "knockoutPesquisaGrupoTransportador", false, _pesquisaGrupoTransportador.Pesquisar.id);

    _grupoTransportador = new GrupoTransportador();
    KoBindings(_grupoTransportador, "knockoutGrupoTransportador");

    HeaderAuditoria("GrupoTransportador", _grupoTransportador);

    _CRUDGrupoTransportador = new CRUDGrupoTransportador();
    KoBindings(_CRUDGrupoTransportador, "knockoutCRUDGrupoTransportador");

    loadGridGrupoTransportador();
    loadIntegracaoGrupoTransportador();
}

function loadGridGrupoTransportador() {
    var opcaoEditar = {
        descricao: "Editar", id: guid(), evento: "onclick", metodo: function (item) { editarClick(item); }, tamanho: "10", icone: ""
    };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridGrupoTransportador = new GridView(_pesquisaGrupoTransportador.Pesquisar.idGrid, "GrupoTransportador/Pesquisa", _pesquisaGrupoTransportador, menuOpcoes);
    _gridGrupoTransportador.CarregarGrid();

    LoadGrupoTransportadorEmpresa();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    obterGrupoTransportadorSalvar();

    Salvar(_grupoTransportador, "GrupoTransportador/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridGrupoTransportador();
                limparCamposGrupoTransportador();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    obterGrupoTransportadorSalvar();

    Salvar(_grupoTransportador, "GrupoTransportador/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridGrupoTransportador();
                limparCamposGrupoTransportador();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposGrupoTransportador();
}

function editarClick(registroSelecionado) {
    limparCamposGrupoTransportador();

    _grupoTransportador.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_grupoTransportador, "GrupoTransportador/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaGrupoTransportador.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
                RecarregarGridGrupoTransportadorEmpresa();
                _configuracaoIntegracao.ListaSistemaIntegracao.val(retorno.Data.Integracoes)
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
        ExcluirPorCodigo(_grupoTransportador, "GrupoTransportador/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridGrupoTransportador();
                    limparCamposGrupoTransportador();
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

function controlarBotoesHabilitados(isEdicao) {
    _CRUDGrupoTransportador.Atualizar.visible(isEdicao);
    _CRUDGrupoTransportador.Excluir.visible(isEdicao);
    _CRUDGrupoTransportador.Cancelar.visible(isEdicao);
    _CRUDGrupoTransportador.Adicionar.visible(!isEdicao);
}

function limparCamposGrupoTransportador() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_grupoTransportador);
    LimparCamposGrupoTransportadorEmpresa();
    limparCamposIntegracao();

    RecarregarGridGrupoTransportadorEmpresa();
    ResetarTabs();
    exibirFiltros();
}

function recarregarGridGrupoTransportador() {
    _gridGrupoTransportador.CarregarGrid();
}

function obterGrupoTransportadorSalvar() {
    _grupoTransportador.Empresas.val(JSON.stringify(_grupoTransportadorEmpresa.Empresa.basicTable.BuscarRegistros()));
    _grupoTransportador.Integracoes.val(obterConfiguracaoIntegracaoSalvar());
}

function ResetarTabs() {
    $("#tabGrupoTransportador a:first").tab("show");
}

function exibirFiltros() {
    if (!_pesquisaGrupoTransportador.ExibirFiltros.visibleFade())
        _pesquisaGrupoTransportador.ExibirFiltros.visibleFade(true);
}
