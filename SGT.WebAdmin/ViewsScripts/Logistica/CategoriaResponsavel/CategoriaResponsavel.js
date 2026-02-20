/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDCategoriaResponsavel;
var _CategoriaResponsavel;
var _pesquisaCategoriaResponsavel;
var _gridCategoriaResponsavel;
var _mapaCategoriaResponsavel;


/*
 * Declaração das Classesm
 */

var CRUDCategoriaResponsavel = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var CategoriaResponsavel = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });



}

var PesquisaCategoriaResponsavel = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });


    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridCategoriaResponsavel, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridCategoriaResponsavel() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "CategoriaResponsavel/ExportarPesquisa", titulo: "Motivo Seleção de Motorista Fora da Ordem" };

    _gridCategoriaResponsavel = new GridViewExportacao(_pesquisaCategoriaResponsavel.Pesquisar.idGrid, "CategoriaResponsavel/Pesquisa", _pesquisaCategoriaResponsavel, menuOpcoes, configuracoesExportacao);
    _gridCategoriaResponsavel.CarregarGrid();
}

function loadCategoriaResponsavel() {
    _CategoriaResponsavel = new CategoriaResponsavel();
    KoBindings(_CategoriaResponsavel, "knockoutCategoriaResponsavel");

    HeaderAuditoria("CategoriaResponsavel", _CategoriaResponsavel);

    _CRUDCategoriaResponsavel = new CRUDCategoriaResponsavel();
    KoBindings(_CRUDCategoriaResponsavel, "knockoutCRUDCategoriaResponsavel");

    _pesquisaCategoriaResponsavel = new PesquisaCategoriaResponsavel();
    KoBindings(_pesquisaCategoriaResponsavel, "knockoutPesquisaCategoriaResponsavel", false, _pesquisaCategoriaResponsavel.Pesquisar.id);

    loadGridCategoriaResponsavel();
}


/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_CategoriaResponsavel, "CategoriaResponsavel/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridCategoriaResponsavel();
                limparCamposCategoriaResponsavel();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_CategoriaResponsavel, "CategoriaResponsavel/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridCategoriaResponsavel();
                limparCamposCategoriaResponsavel();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposCategoriaResponsavel();
}

function editarClick(registroSelecionado) {
    limparCamposCategoriaResponsavel();

    _CategoriaResponsavel.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_CategoriaResponsavel, "CategoriaResponsavel/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaCategoriaResponsavel.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
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
        ExcluirPorCodigo(_CategoriaResponsavel, "CategoriaResponsavel/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridCategoriaResponsavel();
                    limparCamposCategoriaResponsavel();
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
    _CRUDCategoriaResponsavel.Atualizar.visible(isEdicao);
    _CRUDCategoriaResponsavel.Excluir.visible(isEdicao);
    _CRUDCategoriaResponsavel.Cancelar.visible(isEdicao);
    _CRUDCategoriaResponsavel.Adicionar.visible(!isEdicao);
}

function limparCamposCategoriaResponsavel() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_CategoriaResponsavel);
}

function recarregarGridCategoriaResponsavel() {
    _gridCategoriaResponsavel.CarregarGrid();
}