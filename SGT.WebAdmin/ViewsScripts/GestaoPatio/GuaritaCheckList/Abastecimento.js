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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../Enumeradores/EnumTipoAbastecimento.js" />
/// <reference path="GuaritaCheckList.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _guaritaCheckListAbastecimento;

var GuaritaCheckListAbastecimento = function () {
    this.GerarAbastecimento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: "Gerar Abastecimento?", enable: ko.observable(true), visible: ko.observable(true) });

    this.TipoAbastecimento = PropertyEntity({ val: ko.observable(EnumTipoAbastecimento.Arla), options: EnumTipoAbastecimento.obterOpcoes(), def: EnumTipoAbastecimento.Arla, text: "*Tipo: ", issue: 250, enable: ko.observable(true) });

    this.Litros = PropertyEntity({ text: "*Litros:", getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, val: ko.observable(""), maxlength: 8, enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });
    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário:", getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false }, val: ko.observable(""), maxlength: 8, enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });
    this.ValorTotal = PropertyEntity({ text: "*Valor Total:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, val: ko.observable(""), maxlength: 10, enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });

    this.Horimetro = PropertyEntity({ text: "Horímetro:", enable: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false } });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Equipamento:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.Posto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Posto:", idBtnSearch: guid(), issue: 171, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), issue: 140, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });

    this.GerarAbastecimento.val.subscribe(function (novoValor) {
        if (!novoValor) {
            limparCamposGuaritaCheckListAbastecimento();
        } else if (novoValor) {
            $("#liAbastecimento").show();
            if (_CONFIGURACAO_TMS.CNPJPostoPadrao > 0 && _CONFIGURACAO_TMS.CodigoCombustivelPadrao > 0) {
                _guaritaCheckListAbastecimento.Posto.codEntity(_CONFIGURACAO_TMS.CNPJPostoPadrao);
                _guaritaCheckListAbastecimento.Posto.val(_CONFIGURACAO_TMS.PostoPadrao);
                _guaritaCheckListAbastecimento.Produto.codEntity(_CONFIGURACAO_TMS.CodigoCombustivelPadrao);
                _guaritaCheckListAbastecimento.Produto.val(_CONFIGURACAO_TMS.CombustivelPadrao);
                _guaritaCheckListAbastecimento.ValorUnitario.val(_CONFIGURACAO_TMS.ValorCombustivelPadrao);
            }
        }
    });

    this.TipoAbastecimento.val.subscribe(function () {
        LimparCampoEntity(_guaritaCheckListAbastecimento.Produto);
    });

    this.Litros.val.subscribe(function () {
        calculaLitrosGuaritaCheckListAbastecimento();
    });

    this.ValorUnitario.val.subscribe(function () {
        calculaLitrosGuaritaCheckListAbastecimento();
    });

    this.ValorTotal.val.subscribe(function () {
        calculaLitrosGuaritaCheckListAbastecimento();
    });
};

//*******EVENTOS*******

function loadGuaritaCheckListAbastecimento() {
    _guaritaCheckListAbastecimento = new GuaritaCheckListAbastecimento();
    KoBindings(_guaritaCheckListAbastecimento, "knockoutGuaritaCheckListAbastecimento");

    new BuscarClientes(_guaritaCheckListAbastecimento.Posto, RetornoPostoGuaritaCheckListAbastecimento, false, [EnumModalidadePessoa.Fornecedor], null, null, null, null, null, null, null, null, _guaritaCheckListAbastecimento.TipoAbastecimento);
    new BuscarProdutoTMS(_guaritaCheckListAbastecimento.Produto, RetornoProdutoTMSGuaritaCheckListAbastecimento, _guaritaCheckListAbastecimento.TipoAbastecimento);
    new BuscarEquipamentos(_guaritaCheckListAbastecimento.Equipamento);

    $("#liAbastecimento").hide();
}

function RetornoPostoGuaritaCheckListAbastecimento(data) {
    _guaritaCheckListAbastecimento.Posto.val(data.Descricao);
    _guaritaCheckListAbastecimento.Posto.codEntity(data.Codigo);
    if (data.CodigoCombustivel > 0) {
        LimparCampoEntity(_guaritaCheckListAbastecimento.Produto);
        _guaritaCheckListAbastecimento.Produto.codEntity(data.CodigoCombustivel);
        _guaritaCheckListAbastecimento.Produto.val(data.DescricaoCombustivel);
        _guaritaCheckListAbastecimento.ValorUnitario.val(data.ValorCombustivel);
    }
}

function RetornoProdutoTMSGuaritaCheckListAbastecimento(data) {
    _guaritaCheckListAbastecimento.Produto.val(data.Descricao);
    _guaritaCheckListAbastecimento.Produto.codEntity(data.Codigo);
    if (_guaritaCheckListAbastecimento.ValorUnitario.val() === "" || Globalize.parseFloat(_guaritaCheckListAbastecimento.ValorUnitario.val()) === 0)
        _guaritaCheckListAbastecimento.ValorUnitario.val(data.UltimoCustoCombustivel);
}

//*******MÉTODOS*******

function calculaLitrosGuaritaCheckListAbastecimento() {
    var litros = 0;
    var valorTotal = 0;
    var valorUnitario = 0;

    if (_guaritaCheckListAbastecimento.Litros.val() !== "")
        litros = Globalize.parseFloat(_guaritaCheckListAbastecimento.Litros.val());

    if (_guaritaCheckListAbastecimento.ValorUnitario.val() !== "")
        valorUnitario = Globalize.parseFloat(_guaritaCheckListAbastecimento.ValorUnitario.val());

    if (_guaritaCheckListAbastecimento.ValorTotal.val() !== "")
        valorTotal = Globalize.parseFloat(_guaritaCheckListAbastecimento.ValorTotal.val());

    if (litros > 0) {
        if (valorUnitario > 0) {
            _guaritaCheckListAbastecimento.ValorTotal.val(Globalize.format(litros * valorUnitario, "n2"));
        } else if (valorTotal > 0) {
            _guaritaCheckListAbastecimento.ValorUnitario.val(Globalize.format(valorTotal / litros, "n4"));
        }
    }
}

function limparCamposGuaritaCheckListAbastecimento() {
    LimparCampos(_guaritaCheckListAbastecimento);
    SetarEnableCamposKnockout(_guaritaCheckListAbastecimento, true);
    $("#liAbastecimento").hide();
}

function validaCamposObrigatoriosGuaritaCheckListAbastecimento() {
    if (_guaritaCheckListAbastecimento.GerarAbastecimento.val())
        return ValidarCamposObrigatorios(_guaritaCheckListAbastecimento);
    else
        return true;
}
