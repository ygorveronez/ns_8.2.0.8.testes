/// <reference path="../../Consultas/CheckListTipo.js" />
/// <reference path="ProdutoEmbarcador.js" />

var _checklist;

var Checklist = function () {
    this.CheckList = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Produtos.ProdutoEmbarcador.Checklist.getFieldDescription(), idBtnSearch: guid(), required: false });
}

function loadChecklist() {
    _checklist = new Checklist();
    KoBindings(_checklist, "knockoutChecklist");

    new BuscarCheckListTipo(_checklist.CheckList);
}

function editarChecklist() {
    _checklist.CheckList.codEntity(_produtoEmbarcador.CheckList.codEntity());
    _checklist.CheckList.val(_produtoEmbarcador.CheckList.val());
}

function limparCamposChecklist() {
    LimparCampos(_checklist);
}

function SalvarChecklist() {
    _produtoEmbarcador.CheckList.codEntity(_checklist.CheckList.codEntity());
    _produtoEmbarcador.CheckList.val(_checklist.CheckList.val());
}