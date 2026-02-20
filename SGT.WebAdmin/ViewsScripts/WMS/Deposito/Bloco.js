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


//*******MAPEAMENTO KNOUCKOUT*******
var _bloco;
var _gridBloco;
var _blocoSelecionado;

var Bloco = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 200 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });

    this.Deposito = PropertyEntity({ text: ko.observable(""), val: function () { return (GetDeposito() || {}).Codigo }, def: 0, getType: typesKnockout.int });
    this.Rua = PropertyEntity({ text: ko.observable(""), val: function () { return (GetRua() || {}).Codigo }, def: 0, getType: typesKnockout.int });

    this.Blocos = PropertyEntity({ idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarBlocoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarBlocoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirBlocoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarBlocoClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}


//*******EVENTOS*******
function loadBloco() {
    _bloco = new Bloco();
    KoBindings(_bloco, "knockoutBloco");

    GridBlocos();
}

function adicionarBlocoClick(e, sender) {
    Salvar(e, "Bloco/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                limparCamposBloco();
                _gridBloco.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarBlocoClick(e, sender) {
    Salvar(e, "Bloco/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                limparCamposBloco();
                _gridBloco.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirBlocoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o bloco?", function () {
        ExcluirPorCodigo(e, "Bloco/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Msg != undefined && arg.Msg != null && arg.Msg != "") {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                } else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    limparCamposBloco();
                    _gridBloco.CarregarGrid();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarBlocoClick(e) {
    limparCamposBloco();
}

//*******MÉTODOS*******
function GridBlocos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarBloco, tamanho: "15", icone: "" };
    var selecionar = { descricao: "Selecionar", id: guid(), evento: "onclick", metodo: selecionarBloco, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 5,
        descricao: "Opções",
        opcoes: [editar, selecionar]
    };

    _gridBloco = new GridView(_bloco.Blocos.idGrid, "Bloco/Pesquisa", _bloco, menuOpcoes);
}

function editarBloco(objeto) {
    _bloco.Codigo.val(objeto.Codigo);
    _bloco.Descricao.val(objeto.Descricao);
    _bloco.Ativo.val(objeto.Status);

    _bloco.Atualizar.visible(true);
    _bloco.Excluir.visible(true);
    _bloco.Adicionar.visible(false);
}

function limparCamposBloco() {
    _bloco.Atualizar.visible(false);
    _bloco.Excluir.visible(false);
    _bloco.Adicionar.visible(true);

    LimparCampos(_bloco);
}

function selecionarBloco(data) {
    SetBloco(data);

    _posicao.Bloco.text(data.Descricao);
    _posicao.Bloco.val(data.Codigo);

    _gridPosicao.CarregarGrid(function () {
        AvancarEtapa();
    });
}

function GetBloco() {
    return _blocoSelecionado;
}

function SetBloco(bloco) {
    _blocoSelecionado = bloco;

    if (bloco != null)
        Etapa4Liberada();
    else
        Etapa4Desativada();
}