//*******MAPEAMENTO KNOUCKOUT*******

var _gridIRRF, _IRRF, _dadosGeraisIRRF;

var DadosGeraisIRRF = function () {
    this.PercentualBCIR = PropertyEntity({ maxlength: 5, getType: typesKnockout.decimal, text: "*% Base Cálculo: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
    this.ValorPorDependenteDescontoIRRF = PropertyEntity({ getType: typesKnockout.decimal, text: "*% Valor por dependente para desconto da base do IRRF: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
    
};

var IRRF = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorInicial = PropertyEntity({ maxlength: 18, getType: typesKnockout.decimal, text: "*Valor Inicial: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
    this.ValorFinal = PropertyEntity({ maxlength: 18, getType: typesKnockout.decimal, text: "*Valor Final: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
    this.PercentualAplicar = PropertyEntity({ maxlength: 5, getType: typesKnockout.decimal, text: "*Alíquota: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
    this.ValorDeduzir = PropertyEntity({ maxlength: 18, getType: typesKnockout.decimal, text: "*Valor Deduzir: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });

    this.Atualizar = PropertyEntity({ eventClick: AtualizarIRRFClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirIRRFClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarIRRFClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarIRRFClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadIRRF() {

    _IRRF = new IRRF();
    KoBindings(_IRRF, "knockoutIRRF");

    _dadosGeraisIRRF = new DadosGeraisIRRF();
    KoBindings(_dadosGeraisIRRF, "knockoutDadosGeraisIRRF");

    _imposto.PercentualBCIR = _dadosGeraisIRRF.PercentualBCIR;
    _imposto.ValorPorDependenteDescontoIRRF = _dadosGeraisIRRF.ValorPorDependenteDescontoIRRF;

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarIRRFClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "ValorInicial", title: "Valor Inicial", width: "26%" },
        { data: "ValorFinal", title: "Data Final", width: "26%" },
        { data: "Aliquota", title: "Alíquota", width: "28%" }
    ];

    _gridIRRF = new BasicDataTable(_IRRF.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridIRRF();
}

function RecarregarGridIRRF() {

    var data = new Array();

    $.each(_imposto.ListaIRRF.list, function (i, irrf) {
        var IRRFGrid = new Object();

        IRRFGrid.Codigo = irrf.Codigo.val;
        IRRFGrid.ValorInicial = irrf.ValorInicial.val;
        IRRFGrid.ValorFinal = irrf.ValorFinal.val;
        IRRFGrid.Aliquota = irrf.PercentualAplicar.val;

        data.push(IRRFGrid);
    });

    _gridIRRF.CarregarGrid(data);
}


function AtualizarIRRFClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_IRRF);

    if (valido) {
        for (var i = 0; i < _imposto.ListaIRRF.list.length; i++) {
            if (_IRRF.Codigo.val() == _imposto.ListaIRRF.list[i].Codigo.val) {
                _imposto.ListaIRRF.list[i].ValorInicial.val = _IRRF.ValorInicial.val();
                _imposto.ListaIRRF.list[i].ValorFinal.val = _IRRF.ValorFinal.val();
                _imposto.ListaIRRF.list[i].PercentualAplicar.val = _IRRF.PercentualAplicar.val();
                _imposto.ListaIRRF.list[i].ValorDeduzir.val = _IRRF.ValorDeduzir.val();
                break;
            }
        }

        RecarregarGridIRRF();
        LimparCamposIRRF();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function ExcluirIRRFClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esta faixa de IRRF?", function () {
        for (var i = 0; i < _imposto.ListaIRRF.list.length; i++) {
            if (_IRRF.Codigo.val() == _imposto.ListaIRRF.list[i].Codigo.val) {
                _imposto.ListaIRRF.list.splice(i, 1);
                break;
            }
        }

        LimparCamposIRRF();
        RecarregarGridIRRF();
    });
}

function CancelarIRRFClick(e, sender) {
    LimparCamposIRRF();
}

function EditarIRRFClick(data) {
    for (var i = 0; i < _imposto.ListaIRRF.list.length; i++) {
        if (data.Codigo == _imposto.ListaIRRF.list[i].Codigo.val) {
            _IRRF.Codigo.val(_imposto.ListaIRRF.list[i].Codigo.val);
            _IRRF.ValorInicial.val(_imposto.ListaIRRF.list[i].ValorInicial.val);
            _IRRF.ValorFinal.val(_imposto.ListaIRRF.list[i].ValorFinal.val);
            _IRRF.PercentualAplicar.val(_imposto.ListaIRRF.list[i].PercentualAplicar.val);
            _IRRF.ValorDeduzir.val(_imposto.ListaIRRF.list[i].ValorDeduzir.val);
            break;
        }
    }

    _IRRF.Atualizar.visible(true);
    _IRRF.Excluir.visible(true);
    _IRRF.Cancelar.visible(true);
    _IRRF.Adicionar.visible(false);
}

function AdicionarIRRFClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_IRRF);

    if (valido) {

        _IRRF.Codigo.val(guid());

        _imposto.ListaIRRF.list.push(SalvarListEntity(_IRRF));

        RecarregarGridIRRF();

        LimparCamposIRRF();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposIRRF() {
    LimparCampos(_IRRF);
    _IRRF.Atualizar.visible(false);
    _IRRF.Excluir.visible(false);
    _IRRF.Cancelar.visible(false);
    _IRRF.Adicionar.visible(true);
}