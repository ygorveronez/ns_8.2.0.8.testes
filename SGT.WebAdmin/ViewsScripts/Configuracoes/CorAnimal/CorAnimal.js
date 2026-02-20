//*******MAPEAMENTO KNOUCKOUT*******

var _formulario, _gridFormulario, _pesquisa, _crud;

var Pesquisa = function () {
    this.Descricao = PropertyEntity({ text: "Cor: " });
    this.Ativo = PropertyEntity({ val: ko.observable(_statusPesquisa.Todos), options: _statusPesquisa, def: _statusPesquisa.Todos, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFormulario.CarregarGrid();
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
var CorAnimal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true });
    this.Ativo = PropertyEntity({ text: "Status:", val: ko.observable(true), options: _status, def: true });
    this.Pesquisar = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
};
var CRUDCorAnimal = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadCorAnimal() {
    _formulario = new CorAnimal();
    KoBindings(_formulario, "knockoutCadastro");

    HeaderAuditoria("CorAnimal", _formulario);

    _crud = new CRUDCorAnimal();
    KoBindings(_crud, "knockoutCRUD");

    _pesquisa = new Pesquisa();
    KoBindings(_pesquisa, "knockoutPesquisa", false, _pesquisa.Pesquisar.id);

    buscarCorAnimal();
}

function adicionarClick(e, sender) {
    Salvar(_formulario, "CorAnimal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridFormulario.CarregarGrid();
                limparCamposCorAnimal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_formulario, "CorAnimal/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridFormulario.CarregarGrid();
                limparCamposCorAnimal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function editar(arquivoGrid) {
    limparCamposCorAnimal();
    _formulario.Codigo.val(arquivoGrid.Codigo);

    BuscarPorCodigo(_formulario, "CorAnimal/BuscarPorCodigo", function (arg) {
        _pesquisa.ExibirFiltros.visibleFade(false);
        _crud.Atualizar.visible(true);
        _crud.Cancelar.visible(true);
        _crud.Excluir.visible(true);
        _crud.Adicionar.visible(false);

    }, null);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a licença " + _formulario.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_formulario, "CorAnimal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridFormulario.CarregarGrid();
                    limparCamposCorAnimal();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposCorAnimal();
}

//*******MÉTODOS*******

function buscarCorAnimal() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCorAnimal, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridFormulario = new GridView(_pesquisa.Pesquisar.idGrid, "CorAnimal/Pesquisar", _pesquisa, menuOpcoes, null);
    _gridFormulario.CarregarGrid();
}

function editarCorAnimal(arquivoGrid) {
    limparCamposCorAnimal();
    _formulario.Codigo.val(arquivoGrid.Codigo);

    BuscarPorCodigo(_formulario, "CorAnimal/BuscarPorCodigo", function (arg) {
        _pesquisa.ExibirFiltros.visibleFade(false);
        _crud.Atualizar.visible(true);
        _crud.Cancelar.visible(true);
        _crud.Excluir.visible(true);
        _crud.Adicionar.visible(false);

    }, null);
}

function limparCamposCorAnimal() {
    _crud.Atualizar.visible(false);
    _crud.Cancelar.visible(false);
    _crud.Excluir.visible(false);
    _crud.Adicionar.visible(true);

    LimparCampos(_formulario);
}