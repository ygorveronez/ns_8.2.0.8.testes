/// <reference path="../../Consultas/Produto.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTipoOleo;
var _tipoOleo;
var _pesquisaTipoOleo;
var _gridTipoOleo;

/*
 * Declaração das Classes
 */

var CRUDTipoOleo = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var TipoOleo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.TipoDeOleo = PropertyEntity({ text: "*Tipo de Óleo:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Material:", idBtnSearch: guid(), required: true });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração:", required: false, getType: typesKnockout.string, val: ko.observable("") });

};

var PesquisaTipoOleo = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.TipoDeOleo = PropertyEntity({ text: "Tipo de Óleo:", required: false, getType: typesKnockout.string, val: ko.observable("") });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Material:", idBtnSearch: guid() });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridTipoOleo, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridTipoOleo() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridTipoOleo = new GridViewExportacao(_pesquisaTipoOleo.Pesquisar.idGrid, "TipoOleo/Pesquisa", _pesquisaTipoOleo, menuOpcoes);
    _gridTipoOleo.CarregarGrid();
}

function LoadTipoOleo() {
    _tipoOleo = new TipoOleo();
    KoBindings(_tipoOleo, "knockoutTipoOleo");

    _CRUDTipoOleo = new CRUDTipoOleo();
    KoBindings(_CRUDTipoOleo, "knockoutCRUDTipoOleo");

    _pesquisaTipoOleo = new PesquisaTipoOleo();
    KoBindings(_pesquisaTipoOleo, "knockoutPesquisaTipoOleo", false, _pesquisaTipoOleo.Pesquisar.id);

    new BuscarProdutoTMS(_pesquisaTipoOleo.Produto);
    new BuscarProdutoTMS(_tipoOleo.Produto);

    LoadGridTipoOleo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_tipoOleo, "TipoOleo/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridTipoOleo();
                LimparCamposTipoOleo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_tipoOleo, "TipoOleo/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridTipoOleo();
                LimparCamposTipoOleo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposTipoOleo();
}

function EditarClick(registroSelecionado) {
    LimparCamposTipoOleo();

    _tipoOleo.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tipoOleo, "TipoOleo/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaTipoOleo.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                ControlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_tipoOleo, "TipoOleo/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso!");

                    RecarregarGridTipoOleo();
                    LimparCamposTipoOleo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDTipoOleo.Atualizar.visible(isEdicao);
    _CRUDTipoOleo.Excluir.visible(isEdicao);
    _CRUDTipoOleo.Cancelar.visible(isEdicao);
    _CRUDTipoOleo.Adicionar.visible(!isEdicao);
}

function LimparCamposTipoOleo() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_tipoOleo);
}

function RecarregarGridTipoOleo() {
    _gridTipoOleo.CarregarGrid();
}