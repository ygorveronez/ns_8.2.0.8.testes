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
/// <reference path="VistoricaChecklistModeloVeicular.js" />
/// <reference path="../../Consultas/CheckListTipo.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridVistoriaCheckList;
var _VistoriaCheckList;
var _pesquisaVistoriaCheckList;

var PesquisaVistoriaCheckList = function () {
    this.Checklist = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CheckList:", idBtnSearch: guid() });


    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridVistoriaCheckList.CarregarGrid();
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

var VistoriaCheckList = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PeriocidadeVencimento = PropertyEntity({ text: "Periodicidade Vencimento:", maxlength: 50 });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.Checklist = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CheckList:", idBtnSearch: guid(), required: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ val: ko.observable("")})

};

var CRUDVistoriaCheckList = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadVistoriaCheckList() {
    
    _VistoriaCheckList = new VistoriaCheckList();
    KoBindings(_VistoriaCheckList, "knockoutCadastroVistoriaCheckList");

    HeaderAuditoria("VistoriaCheckList", _VistoriaCheckList);

    _crudVistoriaCheckList = new CRUDVistoriaCheckList();
    KoBindings(_crudVistoriaCheckList, "knockoutCRUDVistoriaCheckList");

    _pesquisaVistoriaCheckList = new PesquisaVistoriaCheckList();
    KoBindings(_pesquisaVistoriaCheckList, "knockoutPesquisaVistoriaCheckList", false, _pesquisaVistoriaCheckList.Pesquisar.id);
    loadVistoriaCheckListModeloVeicular();
    buscarVistoriaCheckList();
    new BuscarCheckListTipo(_VistoriaCheckList.Checklist);
    new BuscarModelosVeicularesCarga(_pesquisaVistoriaCheckList.ModeloVeicular)
}

function adicionarClick(e, sender) {
    preencherListasSelecaoVistoriaCheckListModeloVeiculars();
    Salvar(_VistoriaCheckList, "VistoriaCheckList/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridVistoriaCheckList.CarregarGrid();
                limparCamposVistoriaCheckList();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasSelecaoVistoriaCheckListModeloVeiculars();
    Salvar(_VistoriaCheckList, "VistoriaCheckList/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridVistoriaCheckList.CarregarGrid();
                limparCamposVistoriaCheckList();
                RecarregarGridVistoriaChecklistModeloVeicular();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a CheckList " + _VistoriaCheckList.Checklist.val() + "?", function () {
        ExcluirPorCodigo(_VistoriaCheckList, "VistoriaCheckList/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridVistoriaCheckList.CarregarGrid();
                    limparCamposVistoriaCheckList();
                    RecarregarGridVistoriaChecklistModeloVeicular();
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
    limparCamposVistoriaCheckList();
    RecarregarGridVistoriaChecklistModeloVeicular();
}

//*******MÉTODOS*******

function buscarVistoriaCheckList() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarVistoriaCheckList, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridVistoriaCheckList = new GridView(_pesquisaVistoriaCheckList.Pesquisar.idGrid, "VistoriaCheckList/Pesquisa", _pesquisaVistoriaCheckList, menuOpcoes, null);
    _gridVistoriaCheckList.CarregarGrid();
}

function editarVistoriaCheckList(VistoriaCheckListGrid) {
    
    limparCamposVistoriaCheckList();
    _VistoriaCheckList.Codigo.val(VistoriaCheckListGrid.Codigo);
    BuscarPorCodigo(_VistoriaCheckList, "VistoriaCheckList/BuscarPorCodigo", function (arg) {
        _pesquisaVistoriaCheckList.ExibirFiltros.visibleFade(false);
        _crudVistoriaCheckList.Atualizar.visible(true);
        _crudVistoriaCheckList.Cancelar.visible(true);
        _crudVistoriaCheckList.Excluir.visible(true);
        _crudVistoriaCheckList.Adicionar.visible(false);
        RecarregarGridVistoriaChecklistModeloVeicular();

    }, null);
}

function limparCamposVistoriaCheckList() {
    _crudVistoriaCheckList.Atualizar.visible(false);
    _crudVistoriaCheckList.Cancelar.visible(false);
    _crudVistoriaCheckList.Excluir.visible(false);
    _crudVistoriaCheckList.Adicionar.visible(true);
    LimparCampos(_VistoriaCheckList);
}


