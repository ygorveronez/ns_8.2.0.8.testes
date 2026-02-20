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
/// <reference path="Transportador.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _impostoRendaCIOT;
var _impostoINSSCIOT;
var _impostoSESTCIOT;
var _impostoSENATCIOT;

var _gridImpostoRendaCIOT;
var _gridImpostoINSSCIOT;

var ImpostoRendaCIOT = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.BaseCalculo = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.BaseDeCalculo.getFieldDescription(), def: "", getType: typesKnockout.decimal });

    this.De = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.De.getFieldDescription(), def: "", getType: typesKnockout.decimal });
    this.Ate = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.Ate.getFieldDescription(), def: "", getType: typesKnockout.decimal });
    this.Aplicar = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.Aplicar.getFieldDescription(), def: "", getType: typesKnockout.decimal });
    this.Deduzir = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.Deduzir.getFieldDescription(), def: "", getType: typesKnockout.decimal });

    this.Adicionar = PropertyEntity({ eventClick: adicionarImpostoRendaCIOTClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarImpostoRendaCIOTClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarImpostoRendaCIOTClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirImpostoRendaCIOTClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var ImpostoINSSCIOT = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.BaseCalculo = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.BaseDeCalculo.getFieldDescription(), def: "", getType: typesKnockout.decimal });
    this.TetoRetencao = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.TetoRetencao.getFieldDescription(), def: "", getType: typesKnockout.decimal });

    this.De = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.De.getFieldDescription(), def: "", getType: typesKnockout.decimal });
    this.Ate = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.Ate.getFieldDescription(), def: "", getType: typesKnockout.decimal });
    this.Aplicar = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.Aplicar.getFieldDescription(), def: "", getType: typesKnockout.decimal });

    this.Adicionar = PropertyEntity({ eventClick: adicionarImpostoINSSCIOTClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarImpostoINSSCIOTClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarImpostoINSSCIOTClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirImpostoINSSCIOTClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var ImpostoSESTCIOT = function () {
    this.Aliquota = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.Aliquota.getFieldDescription(), def: "", getType: typesKnockout.decimal });
}

var ImpostoSENATCIOT = function () {
    this.Aliquota = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Transportadores.Transportador.Aliquota.getFieldDescription(), def: "", getType: typesKnockout.decimal });
}


//*******EVENTOS*******

function loadImpostosCIOT() {
    _impostoRendaCIOT = new ImpostoRendaCIOT()
    KoBindings(_impostoRendaCIOT, "knockoutImpostoRendaCIOT");

    _impostoINSSCIOT = new ImpostoINSSCIOT()
    KoBindings(_impostoINSSCIOT, "knockoutImpostoINSSCIOT");

    _impostoSESTCIOT = new ImpostoSESTCIOT()
    KoBindings(_impostoSESTCIOT, "knockoutImpostoSESTCIOT");

    _impostoSENATCIOT = new ImpostoSENATCIOT()
    KoBindings(_impostoSENATCIOT, "knockoutImpostoSENATCIOT");

    CarregarGridImpostoRendaCIOT();
    CarregarGridImpostoINSSCIOT();
}

function CarregarGridImpostoRendaCIOT() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        tamanho: 7,
        opcoes: [
            { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarImpostoRendaCIOTClick }
        ]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "De", title: Localization.Resources.Transportadores.Transportador.De.getFieldDescription(), width: "20%" },
        { data: "Ate", title: Localization.Resources.Transportadores.Transportador.Ate.getFieldDescription(), width: "20%" },
        { data: "Aplicar", title: Localization.Resources.Transportadores.Transportador.Aplicar.getFieldDescription(), width: "20%" },
        { data: "Deduzir", title: Localization.Resources.Transportadores.Transportador.Deduzir.getFieldDescription(), width: "20%" },
    ];

    _gridImpostoRendaCIOT = new BasicDataTable(_impostoRendaCIOT.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    RecarregarGridImpostoRendaCIOT();
}

function CarregarGridImpostoINSSCIOT() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        tamanho: 7,
        opcoes: [
            { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarImpostoINSSCIOTClick }
        ]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "De", title: Localization.Resources.Transportadores.Transportador.De.getFieldDescription(), width: "30%" },
        { data: "Ate", title: Localization.Resources.Transportadores.Transportador.Ate.getFieldDescription(), width: "30%" },
        { data: "Aplicar", title: Localization.Resources.Transportadores.Transportador.Aplicar.getFieldDescription(), width: "30%" }
    ];

    _gridImpostoINSSCIOT = new BasicDataTable(_impostoINSSCIOT.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    RecarregarGridImpostoINSSCIOT();
}

function RecarregarGridImpostoRendaCIOT() {
    var data = [];
    $.each(_transportador.ImpostoRendaCIOT.list, function (i, ir) {
        data.push({
            Codigo: ir.Codigo.val,
            De: ir.De.val,
            Ate: ir.Ate.val,
            Aplicar: ir.Aplicar.val,
            Deduzir: ir.Deduzir.val
        });
    });
    _gridImpostoRendaCIOT.CarregarGrid(data);
}

function RecarregarGridImpostoINSSCIOT() {
    var data = [];
    $.each(_transportador.ImpostoINSSCIOT.list, function (i, inss) {
        data.push({
            Codigo: inss.Codigo.val,
            De: inss.De.val,
            Ate: inss.Ate.val,
            Aplicar: inss.Aplicar.val
        });
    });
    _gridImpostoINSSCIOT.CarregarGrid(data);
}

function editarImpostoRendaCIOTClick(data) {
    _impostoRendaCIOT.Atualizar.visible(true);
    _impostoRendaCIOT.Excluir.visible(true);
    _impostoRendaCIOT.Adicionar.visible(false);

    EditarListEntity(_impostoRendaCIOT, data);
}

function editarImpostoINSSCIOTClick(data) {
    _impostoINSSCIOT.Atualizar.visible(true);
    _impostoINSSCIOT.Excluir.visible(true);
    _impostoINSSCIOT.Adicionar.visible(false);

    EditarListEntity(_impostoINSSCIOT, data);
}

function adicionarImpostoRendaCIOTClick(e, sender) {
    if (ValidarCamposObrigatorios(_impostoRendaCIOT)) {
        _impostoRendaCIOT.Codigo.val(guid());
        _transportador.ImpostoRendaCIOT.list.push(SalvarListEntity(_impostoRendaCIOT));

        RecarregarGridImpostoRendaCIOT();
        LimparCamposImpostoCIOT(_impostoRendaCIOT);
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
    }
}

function adicionarImpostoINSSCIOTClick(e, sender) {
    if (ValidarCamposObrigatorios(_impostoINSSCIOT)) {
        _impostoINSSCIOT.Codigo.val(guid());
        _transportador.ImpostoINSSCIOT.list.push(SalvarListEntity(_impostoINSSCIOT));

        RecarregarGridImpostoINSSCIOT();
        LimparCamposImpostoCIOT(_impostoINSSCIOT);
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
    }
}

function atualizarImpostoRendaCIOTClick(e, sender) {
    if (ValidarCamposObrigatorios(_impostoRendaCIOT)) {
        $.each(_transportador.ImpostoRendaCIOT.list, function (i, ir) {
            if (ir.Codigo.val == _impostoRendaCIOT.Codigo.val()) {
                AtualizarListEntity(_impostoRendaCIOT, ir);
                return false;
            }
        });

        RecarregarGridImpostoRendaCIOT();
        LimparCamposImpostoCIOT(_impostoRendaCIOT);
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
    }
}

function atualizarImpostoINSSCIOTClick(e, sender) {
    if (ValidarCamposObrigatorios(_impostoINSSCIOT)) {
        $.each(_transportador.ImpostoINSSCIOT.list, function (i, inss) {
            if (inss.Codigo.val == _impostoINSSCIOT.Codigo.val()) {
                AtualizarListEntity(_impostoINSSCIOT, inss);
                return false;
            }
        });

        RecarregarGridImpostoINSSCIOT();
        LimparCamposImpostoCIOT(_impostoINSSCIOT);
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
    }
}

function excluirImpostoRendaCIOTClick(e, sender) {
    $.each(_transportador.ImpostoRendaCIOT.list, function (i, ir) {
        if (ir.Codigo.val == _impostoRendaCIOT.Codigo.val()) {
            _transportador.ImpostoRendaCIOT.list.splice(i, 1);
            return false;
        }
    });

    RecarregarGridImpostoRendaCIOT();
    LimparCamposImpostoCIOT(_impostoRendaCIOT);
}

function excluirImpostoINSSCIOTClick(e, sender) {
    $.each(_transportador.ImpostoINSSCIOT.list, function (i, inss) {
        if (inss.Codigo.val == _impostoINSSCIOT.Codigo.val()) {
            _transportador.ImpostoINSSCIOT.list.splice(i, 1);
            return false;
        }
    });

    RecarregarGridImpostoINSSCIOT();
    LimparCamposImpostoCIOT(_impostoINSSCIOT);
}

function cancelarImpostoRendaCIOTClick(e) {
    LimparCamposImpostoCIOT(_impostoRendaCIOT);
}

function cancelarImpostoINSSCIOTClick(e) {
    LimparCamposImpostoCIOT(_impostoINSSCIOT);
}

function LimparCamposImpostoCIOT(ko) {
    ko.Atualizar.visible(false);
    ko.Excluir.visible(false);
    ko.Adicionar.visible(true);

    for (var i in { 'Codigo': '', 'De': '', 'Ate': '', 'Aplicar': '', 'Deduzir': '', })
        i in ko && ko[i].val(ko[i].def);
}