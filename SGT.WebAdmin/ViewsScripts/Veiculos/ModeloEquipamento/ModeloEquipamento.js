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

var _gridModeloEquipamento;
var _modeloEquipamento;
var _pesquisaModeloEquipamento;

var PesquisaModeloEquipamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridModeloEquipamento.CarregarGrid();
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

var ModeloEquipamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, maxlength: 150 });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 300 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.MarcaEquipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Marca:", idBtnSearch: guid(), issue: 142 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadModeloEquipamento() {

    _pesquisaModeloEquipamento = new PesquisaModeloEquipamento();
    KoBindings(_pesquisaModeloEquipamento, "knockoutPesquisaModeloEquipamento");

    _modeloEquipamento = new ModeloEquipamento();
    KoBindings(_modeloEquipamento, "knockoutCadastroModeloEquipamento");

    HeaderAuditoria("ModeloEquipamento", _modeloEquipamento);

    new BuscarMarcaEquipamentos(_modeloEquipamento.MarcaEquipamento);

    buscarMarcasEquipamento();   
}

function adicionarClick(e, sender) {
    Salvar(_modeloEquipamento, "ModeloEquipamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridModeloEquipamento.CarregarGrid();
                limparCamposModeloEquipamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    Salvar(_modeloEquipamento, "ModeloEquipamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridModeloEquipamento.CarregarGrid();
                limparCamposModeloEquipamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a marca de equipamento " + _modeloEquipamento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_modeloEquipamento, "ModeloEquipamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridModeloEquipamento.CarregarGrid();
                    limparCamposModeloEquipamento();
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
    limparCamposModeloEquipamento();
}

//*******MÉTODOS*******


function editarModeloEquipamento(modeloEquipamentoGrid) {
    limparCamposModeloEquipamento();
    _modeloEquipamento.Codigo.val(modeloEquipamentoGrid.Codigo);
    BuscarPorCodigo(_modeloEquipamento, "ModeloEquipamento/BuscarPorCodigo", function (arg) {
        _pesquisaModeloEquipamento.ExibirFiltros.visibleFade(false);
        _modeloEquipamento.Atualizar.visible(true);
        _modeloEquipamento.Cancelar.visible(true);
        _modeloEquipamento.Excluir.visible(true);
        _modeloEquipamento.Adicionar.visible(false);
    }, null);
}


function buscarMarcasEquipamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarModeloEquipamento, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridModeloEquipamento = new GridView(_pesquisaModeloEquipamento.Pesquisar.idGrid, "ModeloEquipamento/Pesquisa", _pesquisaModeloEquipamento, menuOpcoes, null);
    _gridModeloEquipamento.CarregarGrid();
}


function limparCamposModeloEquipamento() {
    _modeloEquipamento.Atualizar.visible(false);
    _modeloEquipamento.Cancelar.visible(false);
    _modeloEquipamento.Excluir.visible(false);
    _modeloEquipamento.Adicionar.visible(true);
    LimparCampos(_modeloEquipamento);
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}