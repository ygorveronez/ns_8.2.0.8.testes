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


var _gridTipoPercurso;
var _tipoPercurso;
var _pesquisaTipoPercurso;
var crudTipoPercurso;

var PesquisaTipoPercurso = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable("") });
    this.CodigoIntegracao = PropertyEntity({ text: "Codigo de Integração: ", val: ko.observable(""), enable: ko.observable(true) });
    this.Vazio = PropertyEntity({ text: "Vazio: ", val: ko.observable(EnumTipoPercurso.Todos), options: EnumTipoPercurso.obterOpcoesPesquisa(), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoPercurso.CarregarGrid();
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

function TipoPercurso() {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable(""), enable: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: "Codigo de Integração: ", val: ko.observable(""), enable: ko.observable(true) });
    this.TipoPercurso = PropertyEntity({ text: "Tipo Percurso: ", val: ko.observable(""), enable: ko.observable(true) });
    this.Vazio = PropertyEntity({ text: "Vazio: ", val: ko.observable(EnumTipoPercurso.Ida), options: EnumTipoPercurso.obterOpcoes(), enable: ko.observable(true) });

}

function CrudTipoPercurso() {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//#endregion


//#region Funções Principais

function loadTipoPercurso() {

    _tipoPercurso = new TipoPercurso();
    KoBindings(_tipoPercurso, "knockoutTipoPercurso");

    crudTipoPercurso = new CrudTipoPercurso();
    KoBindings(crudTipoPercurso, "knockoutCRUDTipoPercurso");

    _pesquisaTipoPercurso = new PesquisaTipoPercurso();
    KoBindings(_pesquisaTipoPercurso, "knockoutPesquisaTipoPercurso");

    BuscarTipoPercurso();
}



function BuscarTipoPercurso() {
    const editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarTipoPercurso, tamanho: "15", icone: "" };
    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoPercurso = new GridView(_pesquisaTipoPercurso.Pesquisar.idGrid, "TipoPercurso/Pesquisa", _pesquisaTipoPercurso, menuOpcoes,);
    _gridTipoPercurso.CarregarGrid();
}

function EditarTipoPercurso(item) {
    LimparCamposEditarTipoPercurso();
    _tipoPercurso.Codigo.val(item.Codigo);
    BuscarPorCodigo(_tipoPercurso, "TipoPercurso/BuscarPorCodigo", function (arg) {
        _pesquisaTipoPercurso.ExibirFiltros.visibleFade(false);
        controlarBotoesGrid(true);

    }, null);
}

function LimparCamposEditarTipoPercurso() {
    LimparCampos(_tipoPercurso);
}

//#endregion

//#region Funções Crud
function adicionarClick(e, sender) {
    Salvar(_tipoPercurso, "TipoPercurso/Adicionar", function (arg) {
        if (arg.Success) {
            _pesquisaTipoPercurso.ExibirFiltros.visibleFade(false);
            _gridTipoPercurso.CarregarGrid();
            LimparCamposEditarTipoPercurso();
            exibirMensagem(tipoMensagem.ok, "Aviso", arg.Msg);

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tipoPercurso, "TipoPercurso/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoPercurso.CarregarGrid();
                LimparCamposEditarTipoPercurso();
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
        ExcluirPorCodigo(_tipoPercurso, "TipoPercurso/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTipoPercurso.CarregarGrid();
                    LimparCamposEditarTipoPercurso();
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
    LimparCamposEditarTipoPercurso();
    controlarBotoesGrid(false);
}


function controlarBotoesGrid(valor) {
    crudTipoPercurso.Atualizar.visible(valor);
    crudTipoPercurso.Cancelar.visible(valor);
    crudTipoPercurso.Excluir.visible(valor);
    crudTipoPercurso.Adicionar.visible(!valor);
}
//#endregion