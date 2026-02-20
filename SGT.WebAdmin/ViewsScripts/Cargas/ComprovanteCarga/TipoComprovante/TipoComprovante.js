/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Consultas/Irregularidade.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTiposComprovantes;
var _tiposComprovantes;
var _pesquisaTiposComprovantes;
var _gridTiposComprovantes;

/*
 * Declaração das Classes
 */

var CRUDTiposComprovantes = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var TiposComprovantes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 250 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, required: true, val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500 });
}

var PesquisaTiposComprovantes = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridTiposComprovantes, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTiposComprovantes() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    //var configuracoesExportacao = { url: "MotivoIrregularidade/ExportarPesquisa", titulo: "Motivo de Rejeição de Alteração de Pedido" };

    _gridTiposComprovantes = new GridViewExportacao(_pesquisaTiposComprovantes.Pesquisar.idGrid, "TipoComprovante/Pesquisa", _pesquisaTiposComprovantes, menuOpcoes);
    _gridTiposComprovantes.CarregarGrid();
}

function loadTiposComprovantes() {
    _tiposComprovantes = new TiposComprovantes();
    KoBindings(_tiposComprovantes, "knockoutTiposComprovantes");

    HeaderAuditoria("TiposComprovantes", _tiposComprovantes);

    _CRUDTiposComprovantes = new CRUDTiposComprovantes();
    KoBindings(_CRUDTiposComprovantes, "knockoutCRUDTiposComprovantes");

    _pesquisaTiposComprovantes = new PesquisaTiposComprovantes();
    KoBindings(_pesquisaTiposComprovantes, "knockoutPesquisaTiposComprovantes", false, _pesquisaTiposComprovantes.Pesquisar.id);

    //BuscarIrregularidades(_tiposComprovantes.Irregularidade);

    loadGridTiposComprovantes();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_tiposComprovantes, "TipoComprovante/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridTiposComprovantes();
                limparCamposTiposComprovantes();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tiposComprovantes, "TipoComprovante/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridTiposComprovantes();
                limparCamposTiposComprovantes();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposTiposComprovantes();
}

function editarClick(registroSelecionado) {
    limparCamposTiposComprovantes();

    _tiposComprovantes.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tiposComprovantes, "TipoComprovante/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTiposComprovantes.ExibirFiltros.visibleFade(false);

                controlarBotoesHabilitados();
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
        ExcluirPorCodigo(_tiposComprovantes, "TipoComprovante/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridTiposComprovantes();
                    limparCamposTiposComprovantes();
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
 * Declaração das Funções Privadas
 */

function controlarBotoesHabilitados() {
    var isEdicao = _tiposComprovantes.Codigo.val() > 0;

    _CRUDTiposComprovantes.Atualizar.visible(isEdicao);
    _CRUDTiposComprovantes.Excluir.visible(isEdicao);
    _CRUDTiposComprovantes.Cancelar.visible(isEdicao);
    _CRUDTiposComprovantes.Adicionar.visible(!isEdicao);
}

function limparCamposTiposComprovantes() {
    LimparCampos(_tiposComprovantes);
    controlarBotoesHabilitados();
}

function recarregarGridTiposComprovantes() {
    _gridTiposComprovantes.CarregarGrid();
}