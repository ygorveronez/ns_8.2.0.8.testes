/// <reference path="../../../ViewsScripts/Consultas/GrupoPessoa.js" />
/// <reference path="ContatoGrupoPessoaDado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridContatoGrupoPessoa;
var _contatoGrupoPessoa;
var _pesquisaContatoGrupoPessoa;

var PesquisaContatoGrupoPessoa = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContatoGrupoPessoa.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ContatoGrupoPessoa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Grupo de Pessoa:", idBtnSearch: guid(), required: ko.observable(true) });

    this.Contatos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDContatoGrupoPessoa = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadContatoGrupoPessoa() {
    _contatoGrupoPessoa = new ContatoGrupoPessoa();
    KoBindings(_contatoGrupoPessoa, "knockoutCadastroContatoGrupoPessoa");

    HeaderAuditoria("ContatoGrupoPessoa", _contatoGrupoPessoa);

    _crudContatoGrupoPessoa = new CRUDContatoGrupoPessoa();
    KoBindings(_crudContatoGrupoPessoa, "knockoutCRUDContatoGrupoPessoa");

    _pesquisaContatoGrupoPessoa = new PesquisaContatoGrupoPessoa();
    KoBindings(_pesquisaContatoGrupoPessoa, "knockoutPesquisaContatoGrupoPessoa", false, _pesquisaContatoGrupoPessoa.Pesquisar.id);

    new BuscarGruposPessoas(_contatoGrupoPessoa.GrupoPessoa);
    new BuscarGruposPessoas(_pesquisaContatoGrupoPessoa.GrupoPessoa);

    buscarContatoGrupoPessoa();

    loadContatoGrupoPessoaDado();
}

function adicionarClick(e, sender) {
    Salvar(_contatoGrupoPessoa, "ContatoGrupoPessoa/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridContatoGrupoPessoa.CarregarGrid();
                limparCamposContatoGrupoPessoa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_contatoGrupoPessoa, "ContatoGrupoPessoa/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridContatoGrupoPessoa.CarregarGrid();
                limparCamposContatoGrupoPessoa();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Contato do Grupo de Pessoa " + _contatoGrupoPessoa.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_contatoGrupoPessoa, "ContatoGrupoPessoa/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridContatoGrupoPessoa.CarregarGrid();
                limparCamposContatoGrupoPessoa();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposContatoGrupoPessoa();
}

//*******MÉTODOS*******


function buscarContatoGrupoPessoa() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContatoGrupoPessoa, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridContatoGrupoPessoa = new GridView(_pesquisaContatoGrupoPessoa.Pesquisar.idGrid, "ContatoGrupoPessoa/Pesquisa", _pesquisaContatoGrupoPessoa, menuOpcoes, null);
    _gridContatoGrupoPessoa.CarregarGrid();
}

function editarContatoGrupoPessoa(contatoGrupoPessoaGrid) {
    limparCamposContatoGrupoPessoa();
    _contatoGrupoPessoa.Codigo.val(contatoGrupoPessoaGrid.Codigo);
    BuscarPorCodigo(_contatoGrupoPessoa, "ContatoGrupoPessoa/BuscarPorCodigo", function (arg) {
        _pesquisaContatoGrupoPessoa.ExibirFiltros.visibleFade(false);
        _crudContatoGrupoPessoa.Atualizar.visible(true);
        _crudContatoGrupoPessoa.Cancelar.visible(true);
        _crudContatoGrupoPessoa.Excluir.visible(true);
        _crudContatoGrupoPessoa.Adicionar.visible(false);
        RecarregarGridContato();
    }, null);
}

function limparCamposContatoGrupoPessoa() {
    _crudContatoGrupoPessoa.Atualizar.visible(false);
    _crudContatoGrupoPessoa.Cancelar.visible(false);
    _crudContatoGrupoPessoa.Excluir.visible(false);
    _crudContatoGrupoPessoa.Adicionar.visible(true);
    LimparCampos(_contatoGrupoPessoa);
    LimparContatoGrupoPessoaDados();
}