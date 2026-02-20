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
/// <reference path="Load.js" />
/// <reference path="Navegacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _rua;
var _gridRua;
var _ruaSelecionado = null;

var Rua = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 200 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });

    this.Deposito = PropertyEntity({ text: ko.observable(""), val: function () { return (GetDeposito() || {}).Codigo }, def: 0, getType: typesKnockout.int });

    this.Ruas = PropertyEntity({ idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRuaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRuaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRuaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRuaClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}


//*******EVENTOS*******
function loadRua() {
    _rua = new Rua();
    KoBindings(_rua, "knockoutRua");

    GridRuas();
}

function adicionarRuaClick(e, sender) {
    Salvar(e, "Rua/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                limparCamposRua();
                _gridRua.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarRuaClick(e, sender) {
    Salvar(e, "Rua/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                limparCamposRua();
                _gridRua.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirRuaClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a rua?", function () {
        ExcluirPorCodigo(e, "Rua/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Msg != undefined && arg.Msg != null && arg.Msg != "") {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    limparCamposRua();
                    _gridRua.CarregarGrid();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarRuaClick(e) {
    limparCamposRua();
}

//*******MÉTODOS*******
function GridRuas() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarRua, tamanho: "15", icone: "" };
    var selecionar = { descricao: "Selecionar", id: guid(), evento: "onclick", metodo: selecionarRua, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 5,
        descricao: "Opções",
        opcoes: [editar, selecionar]
    };

    _gridRua = new GridView(_rua.Ruas.idGrid, "Rua/Pesquisa", _rua, menuOpcoes);
}

function editarRua(objeto) {
    _rua.Codigo.val(objeto.Codigo);
    _rua.Descricao.val(objeto.Descricao);
    _rua.Ativo.val(objeto.Status);

    _rua.Atualizar.visible(true);
    _rua.Excluir.visible(true);
    _rua.Adicionar.visible(false);
}

function limparCamposRua() {
    _rua.Atualizar.visible(false);
    _rua.Excluir.visible(false);
    _rua.Adicionar.visible(true);

    LimparCampos(_rua);
}

function selecionarRua(data) {
    SetRua(data);

    _bloco.Rua.text(data.Descricao);
    _bloco.Rua.val(data.Codigo);

    _posicao.Rua.text(data.Descricao);
    _posicao.Rua.val(data.Codigo);

    _gridBloco.CarregarGrid(function () {
        AvancarEtapa();
    });
}

function GetRua() {
    return _ruaSelecionado;
}

function SetRua(rua) {
    _ruaSelecionado = rua;

    if (rua != null)
        Etapa3Liberada();
    else
        Etapa3Desativada();
}
