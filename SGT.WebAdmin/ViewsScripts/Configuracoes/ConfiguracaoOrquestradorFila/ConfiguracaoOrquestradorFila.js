/// <reference path="../../../wwwroot/js/Global/Auditoria.js" />
/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumIdentificadorControlePosicaoThread.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />

// #region Objetos Globais do Arquivo

var _configuracaoOrquestradorFila;
var _crudConfiguracaoOrquestradorFila;
var _gridConfiguracaoOrquestradorFila;
var _pesquisaConfiguracaoOrquestradorFila;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ConfiguracaoOrquestradorFila = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Identificador = PropertyEntity({ text: "*Identificador:", val: ko.observable(""), enable: ko.observable(true), options: EnumIdentificadorControlePosicaoThread.obterOpcoes(), def: "", required: true });
    this.QuantidadeRegistrosConsulta = PropertyEntity({ text: "*Quantidade Consultar:", getType: typesKnockout.int, visible: ko.observable(true), val: ko.observable(""), def: "", maxlength: 3, required: true });
    this.QuantidadeRegistrosRetorno = PropertyEntity({ text: "*Quantidade Retornar:", getType: typesKnockout.int, visible: ko.observable(true), val: ko.observable(""), def: "", maxlength: 3, required: true });
    this.LimiteTentativas = PropertyEntity({ text: "Limite de Tentativas:", getType: typesKnockout.int, visible: ko.observable(true), val: ko.observable(""), def: "", maxlength: 3 });
    this.TratarRegistrosComFalha = PropertyEntity({ text: "Tratar Registros com Falha?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
}

var CrudConfiguracaoOrquestradorFila = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaConfiguracaoOrquestradorFila = function () {
    this.Identificador = PropertyEntity({ text: "Identificador:", val: ko.observable(EnumIdentificadorControlePosicaoThread.Todos), options: EnumIdentificadorControlePosicaoThread.obterOpcoesPesquisa(), def: EnumIdentificadorControlePosicaoThread.Todos });
    this.TratarRegistrosComFalha = PropertyEntity({ text: "Tratar Falha:", val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridConfiguracaoOrquestradorFila, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridConfiguracaoOrquestradorFila() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridConfiguracaoOrquestradorFila = new GridView(_pesquisaConfiguracaoOrquestradorFila.Pesquisar.idGrid, "ConfiguracaoOrquestradorFila/Pesquisa", _pesquisaConfiguracaoOrquestradorFila, menuOpcoes);
    _gridConfiguracaoOrquestradorFila.CarregarGrid();
}

function loadConfiguracaoOrquestradorFila() {
    _configuracaoOrquestradorFila = new ConfiguracaoOrquestradorFila();
    KoBindings(_configuracaoOrquestradorFila, "knockoutConfiguracaoOrquestradorFila");

    HeaderAuditoria("ConfiguracaoOrquestradorFila", _configuracaoOrquestradorFila);

    _crudConfiguracaoOrquestradorFila = new CrudConfiguracaoOrquestradorFila();
    KoBindings(_crudConfiguracaoOrquestradorFila, "knockoutCRUDConfiguracaoOrquestradorFila");

    _pesquisaConfiguracaoOrquestradorFila = new PesquisaConfiguracaoOrquestradorFila();
    KoBindings(_pesquisaConfiguracaoOrquestradorFila, "knockoutPesquisaConfiguracaoOrquestradorFila", false, _pesquisaConfiguracaoOrquestradorFila.Pesquisar.id);

    loadGridConfiguracaoOrquestradorFila();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarClick(e, sender) {
    Salvar(_configuracaoOrquestradorFila, "ConfiguracaoOrquestradorFila/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridConfiguracaoOrquestradorFila();
                limparCamposConfiguracaoOrquestradorFila();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_configuracaoOrquestradorFila, "ConfiguracaoOrquestradorFila/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridConfiguracaoOrquestradorFila();
                limparCamposConfiguracaoOrquestradorFila();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposConfiguracaoOrquestradorFila();
}

function editarClick(registroSelecionado) {
    limparCamposConfiguracaoOrquestradorFila();

    _configuracaoOrquestradorFila.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_configuracaoOrquestradorFila, "ConfiguracaoOrquestradorFila/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaConfiguracaoOrquestradorFila.ExibirFiltros.visibleFade(false);
                controlarComponentesHabilitados();
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
        ExcluirPorCodigo(_configuracaoOrquestradorFila, "ConfiguracaoOrquestradorFila/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridConfiguracaoOrquestradorFila();
                    limparCamposConfiguracaoOrquestradorFila();
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

// #endregion Funções Associadas a Eventos

// #region Funções

function controlarBotoesHabilitados(registroEmEdicao) {
    _crudConfiguracaoOrquestradorFila.Atualizar.visible(registroEmEdicao);
    _crudConfiguracaoOrquestradorFila.Excluir.visible(registroEmEdicao);
    _crudConfiguracaoOrquestradorFila.Cancelar.visible(registroEmEdicao);
    _crudConfiguracaoOrquestradorFila.Adicionar.visible(!registroEmEdicao);
}

function controlarCamposHabilitados(registroEmEdicao) {
    _configuracaoOrquestradorFila.Identificador.enable(!registroEmEdicao);
}

function controlarComponentesHabilitados(registroEmEdicao) {
    var registroEmEdicao = _configuracaoOrquestradorFila.Codigo.val() > 0;

    controlarBotoesHabilitados(registroEmEdicao);
    controlarCamposHabilitados(registroEmEdicao);
}

function limparCamposConfiguracaoOrquestradorFila() {
    LimparCampos(_configuracaoOrquestradorFila);
    controlarComponentesHabilitados();
}

function recarregarGridConfiguracaoOrquestradorFila() {
    _gridConfiguracaoOrquestradorFila.CarregarGrid();
}

// #endregion Funções
