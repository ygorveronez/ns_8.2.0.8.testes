/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Justificativa.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridFolhaInformacao;
var _folhaInformacao;
var _pesquisaFolhaInformacao;

var PesquisaFolhaInformacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFolhaInformacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var FolhaInformacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.CodigoIntegracao = PropertyEntity({ text: "*Código Integração: ", required: ko.observable(true), maxlength: 100 });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
}

var CRUDFolhaInformacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadFolhaInformacao() {
    _folhaInformacao = new FolhaInformacao();
    KoBindings(_folhaInformacao, "knockoutCadastroFolhaInformacao");

    HeaderAuditoria("FolhaInformacao", _folhaInformacao);

    _crudFolhaInformacao = new CRUDFolhaInformacao();
    KoBindings(_crudFolhaInformacao, "knockoutCRUDFolhaInformacao");

    _pesquisaFolhaInformacao = new PesquisaFolhaInformacao();
    KoBindings(_pesquisaFolhaInformacao, "knockoutPesquisaFolhaInformacao", false, _pesquisaFolhaInformacao.Pesquisar.id);

    new BuscarJustificativas(_folhaInformacao.Justificativa);

    buscarFolhaInformacao();
}

function adicionarClick(e, sender) {
    Salvar(_folhaInformacao, "FolhaInformacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridFolhaInformacao.CarregarGrid();
                limparCamposFolhaInformacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_folhaInformacao, "FolhaInformacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridFolhaInformacao.CarregarGrid();
                limparCamposFolhaInformacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a informação da folha?", function () {
        ExcluirPorCodigo(_folhaInformacao, "FolhaInformacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridFolhaInformacao.CarregarGrid();
                limparCamposFolhaInformacao();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposFolhaInformacao();
}

//*******MÉTODOS*******


function buscarFolhaInformacao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarFolhaInformacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFolhaInformacao = new GridView(_pesquisaFolhaInformacao.Pesquisar.idGrid, "FolhaInformacao/Pesquisa", _pesquisaFolhaInformacao, menuOpcoes, null);
    _gridFolhaInformacao.CarregarGrid();
}

function editarFolhaInformacao(folhaInformacaoGrid) {
    limparCamposFolhaInformacao();
    _folhaInformacao.Codigo.val(folhaInformacaoGrid.Codigo);
    BuscarPorCodigo(_folhaInformacao, "FolhaInformacao/BuscarPorCodigo", function (arg) {
        _pesquisaFolhaInformacao.ExibirFiltros.visibleFade(false);
        _crudFolhaInformacao.Atualizar.visible(true);
        _crudFolhaInformacao.Cancelar.visible(true);
        _crudFolhaInformacao.Excluir.visible(true);
        _crudFolhaInformacao.Adicionar.visible(false);
    }, null);
}

function limparCamposFolhaInformacao() {
    _crudFolhaInformacao.Atualizar.visible(false);
    _crudFolhaInformacao.Cancelar.visible(false);
    _crudFolhaInformacao.Excluir.visible(false);
    _crudFolhaInformacao.Adicionar.visible(true);
    LimparCampos(_folhaInformacao);
}