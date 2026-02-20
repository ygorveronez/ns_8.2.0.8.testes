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


var _gridMotivoAtrasoCarregamento;
var _motivoAtrasoCarregamento;
var _pesquisaMotivoAtrasoCarreamento;
var _crudMotivoAtrasoCarregamento;

var PesquisaMotivoAtrasoCarregamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable("") });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoAtrasoCarregamento.CarregarGrid();
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
//#region Constructores

function MotivoAtrasoCarregamento() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", val: ko.observable(""), enable: ko.observable(true) });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, enable: ko.observable(true), text: "*Situação: " });
}

function CrudMotivoAtrasoCarregamento() {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//#endregion


//#region Funções Principais

function loadMotivoAtrasoCarregamento() {
    _motivoAtrasoCarregamento = new MotivoAtrasoCarregamento();
    KoBindings(_motivoAtrasoCarregamento, "knockoutMotivoAtrasoCarregamento");

    _crudMotivoAtrasoCarregamento = new CrudMotivoAtrasoCarregamento();
    KoBindings(_crudMotivoAtrasoCarregamento, "knockoutCRUDMotivoAtrasoCarregamento");

    _pesquisaMotivoAtrasoCarreamento = new PesquisaMotivoAtrasoCarregamento();
    KoBindings(_pesquisaMotivoAtrasoCarreamento, "knockoutPesquisaMotivoAtrasoCarregamento");

    BuscarMotivoAtrasoCarregamento();
}



function BuscarMotivoAtrasoCarregamento() {
    const editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarMotivoAtrasoCarregamento, tamanho: "15", icone: "" };
    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridMotivoAtrasoCarregamento = new GridView(_pesquisaMotivoAtrasoCarreamento.Pesquisar.idGrid, "MotivoAtrasoCarregamento/Pesquisa", _pesquisaMotivoAtrasoCarreamento, menuOpcoes, null);
    _gridMotivoAtrasoCarregamento.CarregarGrid();
}

function EditarMotivoAtrasoCarregamento(item) {
    LimparCamposEditarMotivoAtrasoCarregamento();
    _motivoAtrasoCarregamento.Codigo.val(item.Codigo);
    BuscarPorCodigo(_motivoAtrasoCarregamento, "MotivoAtrasoCarregamento/BuscarPorCodigo", function (arg) {
        //_pesquisaMotivoParadaCentro.ExibirFiltros.visibleFade(false);
        controlarBotoesGrid(true);

    }, null);
}

function LimparCamposEditarMotivoAtrasoCarregamento() {
    LimparCampos(_motivoAtrasoCarregamento);
}

//#endregion

//#region Funções Crud
function adicionarClick(e, sender) {
    Salvar(_motivoAtrasoCarregamento, "MotivoAtrasoCarregamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridMotivoAtrasoCarregamento.CarregarGrid();
                LimparCamposEditarMotivoAtrasoCarregamento();

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoAtrasoCarregamento, "MotivoAtrasoCarregamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoAtrasoCarregamento.CarregarGrid();
                LimparCamposEditarMotivoAtrasoCarregamento();
                controlarBotoesGrid(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o registro?", function () {
        ExcluirPorCodigo(_motivoAtrasoCarregamento, "MotivoAtrasoCarregamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoAtrasoCarregamento.CarregarGrid();
                    LimparCamposEditarMotivoAtrasoCarregamento();
                    controlarBotoesGrid(false);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    LimparCamposEditarMotivoAtrasoCarregamento();
    controlarBotoesGrid(false);
}


function controlarBotoesGrid(valor) {
    _crudMotivoAtrasoCarregamento.Atualizar.visible(valor);
    _crudMotivoAtrasoCarregamento.Cancelar.visible(valor);
    _crudMotivoAtrasoCarregamento.Excluir.visible(valor);
    _crudMotivoAtrasoCarregamento.Adicionar.visible(!valor);
}
//#endregion