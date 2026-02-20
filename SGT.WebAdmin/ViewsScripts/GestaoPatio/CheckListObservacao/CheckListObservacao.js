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


//*******MAPEAMENTO KNOUCKOUT*******

var _checkListObservacao;
var _pesquisaCheckListObservacao;
var _gridCheckListObservacao;

var _sitaucaoCategoria = [
    { text: "Tração", value: EnumCategoriaOpcaoCheckList.Tracao },
    { text: "Reboque", value: EnumCategoriaOpcaoCheckList.Reboque },
    { text: "Motorista", value: EnumCategoriaOpcaoCheckList.Motorista }
];
var _sitaucaoCategoriaPesquisa = [
    { text: "Todas", value: '' },
    { text: "Tração", value: EnumCategoriaOpcaoCheckList.Tracao },
    { text: "Reboque", value: EnumCategoriaOpcaoCheckList.Reboque },
    { text: "Motorista", value: EnumCategoriaOpcaoCheckList.Motorista }
];

var CheckListObservacao = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 1000 });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.Categoria = PropertyEntity({ text: "Categoria: ",issue: 1164, val: ko.observable(EnumCategoriaOpcaoCheckList.Tracao), options: _sitaucaoCategoria, def: EnumCategoriaOpcaoCheckList.Tracao });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });

    // CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaCheckListObservacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 1000 });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Categoria = PropertyEntity({ text: "Categoria: ",issue: 1164, val: ko.observable(''), options: _sitaucaoCategoriaPesquisa, def: '' });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCheckListObservacao.CarregarGrid();
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


//*******EVENTOS*******
function loadCheckListObservacao() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaCheckListObservacao = new PesquisaCheckListObservacao();
    KoBindings(_pesquisaCheckListObservacao, "knockoutPesquisaCheckListObservacao", false, _pesquisaCheckListObservacao.Pesquisar.id);

    // Instancia objeto principal
    _checkListObservacao = new CheckListObservacao();
    KoBindings(_checkListObservacao, "knockoutCheckListObservacao");

    // Instancia buscas

    // Inicia busca
    buscarCheckListObservacao();
}

function adicionarClick(e, sender) {
    Salvar(_checkListObservacao, "CheckListObservacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridCheckListObservacao.CarregarGrid();
                limparCamposCheckListObservacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_checkListObservacao, "CheckListObservacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridCheckListObservacao.CarregarGrid();
                limparCamposCheckListObservacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_checkListObservacao, "CheckListObservacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridCheckListObservacao.CarregarGrid();
                    limparCamposCheckListObservacao();
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
    limparCamposCheckListObservacao();
}

function editarCheckListObservacaoClick(itemGrid) {
    // Limpa os campos
    limparCamposCheckListObservacao();

    // Seta o codigo do objeto
    _checkListObservacao.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_checkListObservacao, "CheckListObservacao/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaCheckListObservacao.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _checkListObservacao.Atualizar.visible(true);
                _checkListObservacao.Excluir.visible(true);
                _checkListObservacao.Cancelar.visible(true);
                _checkListObservacao.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function buscarCheckListObservacao() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCheckListObservacaoClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridCheckListObservacao = new GridView(_pesquisaCheckListObservacao.Pesquisar.idGrid, "CheckListObservacao/Pesquisa", _pesquisaCheckListObservacao, menuOpcoes, null);
    _gridCheckListObservacao.CarregarGrid();
}

function limparCamposCheckListObservacao() {
    _checkListObservacao.Atualizar.visible(false);
    _checkListObservacao.Cancelar.visible(false);
    _checkListObservacao.Excluir.visible(false);
    _checkListObservacao.Adicionar.visible(true);
    LimparCampos(_checkListObservacao);
}