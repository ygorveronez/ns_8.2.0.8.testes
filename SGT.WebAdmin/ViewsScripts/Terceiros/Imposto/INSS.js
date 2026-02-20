//*******MAPEAMENTO KNOUCKOUT*******

var _gridINSS, _INSS, _dadosGeraisINSS;

var DadosGeraisINSS = function () {
    this.PercentualBCINSS = PropertyEntity({ maxlength: 5, getType: typesKnockout.decimal, text: "*% Base Cálculo: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
    this.ValorTetoRetencaoINSS = PropertyEntity({ maxlength: 18, getType: typesKnockout.decimal, text: "*Valor Teto Retenção: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
    this.AliquotaINSSPatronal = PropertyEntity({ maxlength: 5, getType: typesKnockout.decimal, text: "*% INSS Patronal: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
};

var INSS = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorInicial = PropertyEntity({ maxlength: 18, getType: typesKnockout.decimal, text: "*Valor Inicial: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
    this.ValorFinal = PropertyEntity({ maxlength: 18, getType: typesKnockout.decimal, text: "*Valor Final: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });
    this.PercentualAplicar = PropertyEntity({ maxlength: 5, getType: typesKnockout.decimal, text: "*Alíquota: ", val: ko.observable("0,00"), def: "0,00", required: true, configDecimal: { precision: 2, allowZero: true } });

    this.Atualizar = PropertyEntity({ eventClick: AtualizarINSSClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirINSSClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarINSSClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarINSSClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadINSS() {

    _INSS = new INSS();
    KoBindings(_INSS, "knockoutINSS");

    _dadosGeraisINSS = new DadosGeraisINSS();
    KoBindings(_dadosGeraisINSS, "knockoutDadosGeraisINSS");

    _imposto.PercentualBCINSS = _dadosGeraisINSS.PercentualBCINSS;
    _imposto.AliquotaINSSPatronal = _dadosGeraisINSS.AliquotaINSSPatronal;
    _imposto.ValorTetoRetencaoINSS = _dadosGeraisINSS.ValorTetoRetencaoINSS;

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Editar", id: guid(), metodo: EditarINSSClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "ValorInicial", title: "Valor Inicial", width: "26%" },
        { data: "ValorFinal", title: "Valor Final", width: "26%" },
        { data: "Aliquota", title: "Alíquota", width: "28%" }
    ];

    _gridINSS = new BasicDataTable(_INSS.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridINSS();
}

function RecarregarGridINSS() {

    var data = new Array();

    $.each(_imposto.ListaINSS.list, function (i, inss) {
        var INSSGrid = new Object();

        INSSGrid.Codigo = inss.Codigo.val;
        INSSGrid.ValorInicial = inss.ValorInicial.val;
        INSSGrid.ValorFinal = inss.ValorFinal.val;
        INSSGrid.Aliquota = inss.PercentualAplicar.val;

        data.push(INSSGrid);
    });

    _gridINSS.CarregarGrid(data);
}


function AtualizarINSSClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_INSS);

    if (valido) {
        for (var i = 0; i < _imposto.ListaINSS.list.length; i++) {
            if (_INSS.Codigo.val() == _imposto.ListaINSS.list[i].Codigo.val) {
                _imposto.ListaINSS.list[i].ValorInicial.val = _INSS.ValorInicial.val();
                _imposto.ListaINSS.list[i].ValorFinal.val = _INSS.ValorFinal.val();
                _imposto.ListaINSS.list[i].PercentualAplicar.val = _INSS.PercentualAplicar.val();
                break;
            }
        }

        RecarregarGridINSS();
        LimparCamposINSS();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function ExcluirINSSClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esta faixa de INSS?", function () {
        for (var i = 0; i < _imposto.ListaINSS.list.length; i++) {
            if (_INSS.Codigo.val() == _imposto.ListaINSS.list[i].Codigo.val) {
                _imposto.ListaINSS.list.splice(i, 1);
                break;
            }
        }
        LimparCamposINSS();
        RecarregarGridINSS();
    });
}

function CancelarINSSClick(e, sender) {
    LimparCamposINSS();
}

function EditarINSSClick(data) {
    for (var i = 0; i < _imposto.ListaINSS.list.length; i++) {
        if (data.Codigo == _imposto.ListaINSS.list[i].Codigo.val) {
            _INSS.Codigo.val(_imposto.ListaINSS.list[i].Codigo.val);
            _INSS.ValorInicial.val(_imposto.ListaINSS.list[i].ValorInicial.val);
            _INSS.ValorFinal.val(_imposto.ListaINSS.list[i].ValorFinal.val);
            _INSS.PercentualAplicar.val(_imposto.ListaINSS.list[i].PercentualAplicar.val);
            break;
        }
    }

    _INSS.Atualizar.visible(true);
    _INSS.Excluir.visible(true);
    _INSS.Cancelar.visible(true);
    _INSS.Adicionar.visible(false);
}

function AdicionarINSSClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_INSS);

    if (valido) {

        _INSS.Codigo.val(guid());

        _imposto.ListaINSS.list.push(SalvarListEntity(_INSS));

        RecarregarGridINSS();

        LimparCamposINSS();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposINSS() {
    LimparCampos(_INSS);
    _INSS.Atualizar.visible(false);
    _INSS.Excluir.visible(false);
    _INSS.Cancelar.visible(false);
    _INSS.Adicionar.visible(true);
}