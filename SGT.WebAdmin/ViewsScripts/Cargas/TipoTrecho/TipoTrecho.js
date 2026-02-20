/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoAlerta.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/PortfolioModuloControle.js" />
/// <reference path="../../Consultas/Irregularidade.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTipoTrecho;
var _tipoTrecho;
var _tiposOperacaoTipoTrecho;
var _categoriasOrigemTipoTrecho;
var _categoriasDestinoTipoTrecho;
var _categoriasExpedidorTipoTrecho;
var _categoriasRecebedorTipoTrecho;
var _modelosVeicularesTipoTrecho;
var _clientesOrigemTipoTrecho;
var _clientesDestinoTipoTrecho;
var _pesquisaTipoTrecho;
var _gridTipoTrecho;

/*
 * Declaração das Classes
 */

var CRUDTipoTrecho = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo" });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false)});
}

var TipoTrecho = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", val: ko.observable(""), required: true, visible: true, getType: typesKnockout.string });
    this.Situacao = PropertyEntity({ text: "*Situação: ", val: ko.observable(true), required: true, visible: true, options: _status })
}

var PesquisaTipoTrecho = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable(""), def: "", maxlentgh: 100 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", val: ko.observable(0), required: true, visible: true, options: _statusPesquisa })

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridTipoTrecho, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTipoTrecho() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridTipoTrecho = new GridView(_pesquisaTipoTrecho.Pesquisar.idGrid, "TipoTrecho/Pesquisa", _pesquisaTipoTrecho, menuOpcoes);
    _gridTipoTrecho.CarregarGrid();
}

function loadTipoTrecho() {

    _pesquisaTipoTrecho = new PesquisaTipoTrecho();
    KoBindings(_pesquisaTipoTrecho, "knockoutPesquisaTipoTrecho", false, _pesquisaTipoTrecho.Pesquisar.id);

    _tipoTrecho = new TipoTrecho();
    KoBindings(_tipoTrecho, "knockoutTipoTrechoDetalhes");

    HeaderAuditoria("TipoTrecho", _tipoTrecho);

    _CRUDTipoTrecho = new CRUDTipoTrecho();
    KoBindings(_CRUDTipoTrecho, "knockoutCRUDTipoTrecho");
    loadTipoTrechoTiposOperacao();
    loadTipoTrechoCategoriasOrigem();
    loadTipoTrechoCategoriasDestino();
    loadTipoTrechoCategoriasExpedidor();
    loadTipoTrechoCategoriasRecebedor();
    loadTipoTrechoModelosVeiculares();
    loadTipoTrechoClientesOrigem();
    loadTipoTrechoClientesDestino();

    loadGridTipoTrecho();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    if (!validarTipoTrecho())
        return;

    executarReST("TipoTrecho/Adicionar", obterTipoTrechoSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                recarregarGridTipoTrecho();
                limparCamposTipoTrecho();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function atualizarClick() {
    if (!validarTipoTrecho())
        return;

    executarReST("TipoTrecho/Atualizar", obterTipoTrechoSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                recarregarGridTipoTrecho();
                limparCamposTipoTrecho();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function cancelarClick() {
    limparCamposTipoTrecho();
}

function editarClick(registroSelecionado) {
    limparCamposTipoTrecho();

    _tipoTrecho.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tipoTrecho, "TipoTrecho/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTipoTrecho.ExibirFiltros.visibleFade(false);
                _gridTipoTrechoTiposOperacao.CarregarGrid(retorno.Data.ListaTiposOperacao);
                _gridTipoTrechoCategoriasOrigem.CarregarGrid(retorno.Data.ListaCategoriasOrigem);
                _gridTipoTrechoCategoriasDestino.CarregarGrid(retorno.Data.ListaCategoriasDestino);
                _gridTipoTrechoCategoriasExpedidor.CarregarGrid(retorno.Data.ListaCategoriasExpedidor);
                _gridTipoTrechoCategoriasRecebedor.CarregarGrid(retorno.Data.ListaCategoriasRecebedor);
                _gridTipoTrechoModelosVeiculares.CarregarGrid(retorno.Data.ListaModelosVeiculares);
                _gridTipoTrechoClientesOrigem.CarregarGrid(retorno.Data.ListaClientesOrigem);
                _gridTipoTrechoClientesDestino.CarregarGrid(retorno.Data.ListaClientesDestino);

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
        ExcluirPorCodigo(_tipoTrecho, "TipoTrecho/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridTipoTrecho();
                    limparCamposTipoTrecho();
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
    var isEdicao = _tipoTrecho.Codigo.val() > 0;

    _CRUDTipoTrecho.Atualizar.visible(isEdicao);
    _CRUDTipoTrecho.Excluir.visible(isEdicao);
    _CRUDTipoTrecho.Adicionar.visible(!isEdicao);
}

function limparCamposTipoTrecho() {
    LimparCampos(_tipoTrecho);
    LimparCampos(_tiposOperacaoTipoTrecho);
    LimparCampos(_categoriasOrigemTipoTrecho);
    LimparCampos(_categoriasDestinoTipoTrecho);
    LimparCampos(_categoriasExpedidorTipoTrecho);
    LimparCampos(_categoriasRecebedorTipoTrecho);
    LimparCampos(_modelosVeicularesTipoTrecho);
    LimparCampos(_clientesOrigemTipoTrecho);
    LimparCampos(_clientesDestinoTipoTrecho);
    _gridTipoTrechoTiposOperacao.CarregarGrid([]);
    _gridTipoTrechoCategoriasOrigem.CarregarGrid([]);
    _gridTipoTrechoCategoriasDestino.CarregarGrid([]);
    _gridTipoTrechoCategoriasExpedidor.CarregarGrid([]);
    _gridTipoTrechoCategoriasRecebedor.CarregarGrid([]);
    _gridTipoTrechoModelosVeiculares.CarregarGrid([]);
    _gridTipoTrechoClientesOrigem.CarregarGrid([]);
    _gridTipoTrechoClientesDestino.CarregarGrid([]);

    controlarBotoesHabilitados();
}

function obterTipoTrechoSalvar() {
    var TipoTrecho = RetornarObjetoPesquisa(_tipoTrecho);

    TipoTrecho["CodigosTiposOperacao"] = obterCodigosTiposOperacao();
    TipoTrecho["CodigosCategoriasOrigem"] = obterCodigosCategoriasOrigem();
    TipoTrecho["CodigosCategoriasDestino"] = obterCodigosCategoriasDestino();
    TipoTrecho["CodigosCategoriasExpedidor"] = obterCodigosCategoriasExpedidor();
    TipoTrecho["CodigosCategoriasRecebedor"] = obterCodigosCategoriasRecebedor();
    TipoTrecho["CodigosModelosVeiculares"] = obterCodigosModelosVeiculares();
    TipoTrecho["CodigosClientesOrigem"] = obterCodigosClientesOrigem();
    TipoTrecho["CodigosClientesDestino"] = obterCodigosClientesDestino();

    return TipoTrecho;
}

function recarregarGridTipoTrecho() {
    _gridTipoTrecho.CarregarGrid();
}

function validarTipoTrecho() {

    if (!ValidarCamposObrigatorios(_tipoTrecho)) {
        exibirMensagem("atencao", "Campos Obrigatórios", "Por Favor, informe os campos obrigatórios");
        return false;
    }

    return true;
}
