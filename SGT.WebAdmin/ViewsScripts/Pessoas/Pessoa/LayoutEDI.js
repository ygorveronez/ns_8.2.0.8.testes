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
/// <reference path="../../Consultas/LayoutEDI.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

//var _gridLayoutEDI;
//var _layoutEDI;

//var LayoutEDI = function () {
//    this.Grid = PropertyEntity({ type: types.local });
//    this.Layout = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Layout EDI: ",issue: 258, idBtnSearch: guid(), required: true });

//    this.Adicionar = PropertyEntity({ eventClick: adicionarLayoutEDIClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
//}


////*******EVENTOS*******

//function loadLayoutEDI() {

//    _layoutEDI = new LayoutEDI();
//    KoBindings(_layoutEDI, "knockoutCadastroLayoutEDI");

//    new BuscarLayoutsEDI(_layoutEDI.Layout);

//    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirLayoutEDIClick }] };

//    var header = [{ data: "Codigo", visible: false },
//                  { data: "Descricao", title: "Descrição", width: "80%" }];

//    _gridLayoutEDI = new BasicDataTable(_layoutEDI.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

//    recarregarGridLayoutEDI();
//}

//function recarregarGridLayoutEDI() {
    
//    var data = new Array();

//    $.each(_pessoa.LayoutsEDI.list, function (i, layoutEDI) {
//        var layoutEDIGrid = new Object();

//        layoutEDIGrid.Codigo = layoutEDI.Layout.codEntity;
//        layoutEDIGrid.Descricao = layoutEDI.Layout.val;

//        data.push(layoutEDIGrid);
//    });

//    _gridLayoutEDI.CarregarGrid(data);
//}


//function excluirLayoutEDIClick(data) {
//    $.each(_pessoa.LayoutsEDI.list, function (i, layoutEDI) {
//        if (data.Codigo == layoutEDI.Layout.codEntity) {
//            _pessoa.LayoutsEDI.list.splice(i, 1);
//            return false;
//        }
//    });

//    recarregarGridLayoutEDI();
//}

//function adicionarLayoutEDIClick(e, sender) {
//    var valido = ValidarCamposObrigatorios(_layoutEDI);

//    if (valido) {
//        var existe = false;
//        $.each(_pessoa.LayoutsEDI.list, function (i, layoutEDI) {
//            if (layoutEDI.Layout.codEntity == _layoutEDI.Layout.codEntity()) {
//                existe = true;
//                return;
//            }
//        });

//        if (existe) {
//            exibirMensagem(tipoMensagem.aviso, "Layout de EDI já existente", "O layout de EDI " + _layoutEDI.Layout.val() + " já está cadastrado.");
//            return;
//        }

//        _pessoa.LayoutsEDI.list.push(SalvarListEntity(_layoutEDI));

//        recarregarGridLayoutEDI();

//        $("#" + _layoutEDI.Layout.id).focus();

//        limparCamposLayoutEDI();
//    } else {
//        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
//    }
//}

//function limparCamposLayoutEDI() {
//    LimparCampos(_layoutEDI);
//}