//*******MAPEAMENTO KNOUCKOUT*******

var _gridPlanoServico, _planoServico, _pesquisaPlanoServico, _crudPlanoServico;

var PesquisaPlanoServico = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPlanoServico.CarregarGrid();
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

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var PlanoServico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
};

var CRUDPlanoServico = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadPlanoServico() {
    _planoServico = new PlanoServico();
    KoBindings(_planoServico, "knockoutCadastro");

    HeaderAuditoria("PlanoServico", _planoServico);

    _crudPlanoServico = new CRUDPlanoServico();
    KoBindings(_crudPlanoServico, "knockoutCRUD");

    _pesquisaPlanoServico = new PesquisaPlanoServico();
    KoBindings(_pesquisaPlanoServico, "knockoutPesquisaPlanoServico", false, _pesquisaPlanoServico.Pesquisar.id);

    buscarPlanoServico();
}

function adicionarClick(e, sender) {
    Salvar(_planoServico, "PlanoServico/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridPlanoServico.CarregarGrid();
                limparCamposPlanoServico();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_planoServico, "PlanoServico/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPlanoServico.CarregarGrid();
                limparCamposPlanoServico();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a licença " + _planoServico.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_planoServico, "PlanoServico/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridPlanoServico.CarregarGrid();
                    limparCamposPlanoServico();
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
    limparCamposPlanoServico();
}

//*******MÉTODOS*******

function buscarPlanoServico() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPlanoServico, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPlanoServico = new GridView(_pesquisaPlanoServico.Pesquisar.idGrid, "PlanoServico/Pesquisar", _pesquisaPlanoServico, menuOpcoes, null);
    _gridPlanoServico.CarregarGrid();
}

function editarPlanoServico(arquivoGrid) {
    limparCamposPlanoServico();
    _planoServico.Codigo.val(arquivoGrid.Codigo);

    BuscarPorCodigo(_planoServico, "PlanoServico/BuscarPorCodigo", function (arg) {
        _pesquisaPlanoServico.ExibirFiltros.visibleFade(false);
        _crudPlanoServico.Atualizar.visible(true);
        _crudPlanoServico.Cancelar.visible(true);
        _crudPlanoServico.Excluir.visible(true);
        _crudPlanoServico.Adicionar.visible(false);

    }, null);
}

function limparCamposPlanoServico() {
    _crudPlanoServico.Atualizar.visible(false);
    _crudPlanoServico.Cancelar.visible(false);
    _crudPlanoServico.Excluir.visible(false);
    _crudPlanoServico.Adicionar.visible(true);

    LimparCampos(_planoServico);
}