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
/// <reference path="../../Enumeradores/EnumStatusColetaContainer.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridJustificativaContainer;
var _JustificativaContainer;
var _pesquisaJustificativaContainer;

var PesquisaJustificativaContainer = function () {

    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.StatusContainer = PropertyEntity({ text: "Status Container: ", val: ko.observable(EnumStatusColetaContainer.Todas), options: EnumStatusColetaContainer.obterOpcoesPesquisa(), def: EnumStatusColetaContainer.Todas, required: ko.observable(true), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridJustificativaContainer.CarregarGrid();
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

var JustificativaContainer = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.StatusContainer = PropertyEntity({ text: "Status Container: ", val: ko.observable(EnumStatusColetaContainer.AguardandoColeta), options: EnumStatusColetaContainer.obterOpcoes(), def: EnumStatusColetaContainer.AguardandoColeta, required: ko.observable(true), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observacao: ", required: ko.observable(false) });
};

var CRUDJustificativaContainer = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadJustificativaContainer() {
    _JustificativaContainer = new JustificativaContainer();
    KoBindings(_JustificativaContainer, "knockoutCadastroJustificativaContainer");

    HeaderAuditoria("JustificativaContainer", _JustificativaContainer);

    _crudJustificativaContainer = new CRUDJustificativaContainer();
    KoBindings(_crudJustificativaContainer, "knockoutCRUDJustificativaContainer");

    _pesquisaJustificativaContainer = new PesquisaJustificativaContainer();
    KoBindings(_pesquisaJustificativaContainer, "knockoutPesquisaJustificativaContainer", false, _pesquisaJustificativaContainer.Pesquisar.id);

    buscarJustificativaContainer();
}

function adicionarClick(e, sender) {
    Salvar(_JustificativaContainer, "JustificativaContainer/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridJustificativaContainer.CarregarGrid();
                limparCamposJustificativaContainer();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_JustificativaContainer, "JustificativaContainer/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridJustificativaContainer.CarregarGrid();
                limparCamposJustificativaContainer();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Marca EPI " + _JustificativaContainer.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_JustificativaContainer, "JustificativaContainer/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridJustificativaContainer.CarregarGrid();
                    limparCamposJustificativaContainer();
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
    limparCamposJustificativaContainer();
}

//*******MÉTODOS*******

function buscarJustificativaContainer() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarJustificativaContainer, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridJustificativaContainer = new GridView(_pesquisaJustificativaContainer.Pesquisar.idGrid, "JustificativaContainer/Pesquisa", _pesquisaJustificativaContainer, menuOpcoes);
    _gridJustificativaContainer.CarregarGrid();
}

function editarJustificativaContainer(JustificativaContainerGrid) {
    limparCamposJustificativaContainer();
    _JustificativaContainer.Codigo.val(JustificativaContainerGrid.Codigo);
    BuscarPorCodigo(_JustificativaContainer, "JustificativaContainer/BuscarPorCodigo", function (arg) {
        _pesquisaJustificativaContainer.ExibirFiltros.visibleFade(false);
        _crudJustificativaContainer.Atualizar.visible(true);
        _crudJustificativaContainer.Cancelar.visible(true);
        _crudJustificativaContainer.Excluir.visible(true);
        _crudJustificativaContainer.Adicionar.visible(false);
    }, null);
}

function limparCamposJustificativaContainer() {
    _crudJustificativaContainer.Atualizar.visible(false);
    _crudJustificativaContainer.Cancelar.visible(false);
    _crudJustificativaContainer.Excluir.visible(false);
    _crudJustificativaContainer.Adicionar.visible(true);
    LimparCampos(_JustificativaContainer);
}