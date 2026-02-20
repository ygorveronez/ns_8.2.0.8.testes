/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMarcaEquipamento;
var _marcaEquipamento;
var _pesquisaMarcaEquipamento;

var PesquisaMarcaEquipamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMarcaEquipamento.CarregarGrid();
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

var MarcaEquipamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 150 });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 300 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadMarcaEquipamento() {

    _pesquisaMarcaEquipamento = new PesquisaMarcaEquipamento();
    KoBindings(_pesquisaMarcaEquipamento, "knockoutPesquisaMarcaEquipamento");

    _marcaEquipamento = new MarcaEquipamento();
    KoBindings(_marcaEquipamento, "knockoutCadastroMarcaEquipamento");

    HeaderAuditoria("MarcaEquipamento", _marcaEquipamento);

    buscarMarcasEquipamento();   
}

function adicionarClick(e, sender) {
    Salvar(_marcaEquipamento, "MarcaEquipamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMarcaEquipamento.CarregarGrid();
                limparCamposMarcaEquipamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    Salvar(_marcaEquipamento, "MarcaEquipamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMarcaEquipamento.CarregarGrid();
                limparCamposMarcaEquipamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a marca de equipamento " + _marcaEquipamento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_marcaEquipamento, "MarcaEquipamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMarcaEquipamento.CarregarGrid();
                    limparCamposMarcaEquipamento();
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
    limparCamposMarcaEquipamento();
}

//*******MÉTODOS*******


function editarMarcaEquipamento(marcaEquipamentoGrid) {
    limparCamposMarcaEquipamento();
    _marcaEquipamento.Codigo.val(marcaEquipamentoGrid.Codigo);
    BuscarPorCodigo(_marcaEquipamento, "MarcaEquipamento/BuscarPorCodigo", function (arg) {
        _pesquisaMarcaEquipamento.ExibirFiltros.visibleFade(false);
        _marcaEquipamento.Atualizar.visible(true);
        _marcaEquipamento.Cancelar.visible(true);
        _marcaEquipamento.Excluir.visible(true);
        _marcaEquipamento.Adicionar.visible(false);
    }, null);
}


function buscarMarcasEquipamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMarcaEquipamento, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridMarcaEquipamento = new GridView(_pesquisaMarcaEquipamento.Pesquisar.idGrid, "MarcaEquipamento/Pesquisa", _pesquisaMarcaEquipamento, menuOpcoes, null);
    _gridMarcaEquipamento.CarregarGrid();
}


function limparCamposMarcaEquipamento() {
    _marcaEquipamento.Atualizar.visible(false);
    _marcaEquipamento.Cancelar.visible(false);
    _marcaEquipamento.Excluir.visible(false);
    _marcaEquipamento.Adicionar.visible(true);
    LimparCampos(_marcaEquipamento);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}