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
/// <reference path="../../Enumeradores/EnumTipoRotaTarget.js" />
/// <reference path="Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _filialTarget;

var FilialTarget = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiasPrazo = PropertyEntity({ text: Localization.Resources.Filiais.Filial.DiasPrazo.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0), def: 0, maxlength: 2 });
    this.CodigoCentroCusto = PropertyEntity({ text: Localization.Resources.Filiais.Filial.CentroCusto.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0), def: 0, maxlength: 12 });

    this.Usuario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Usuario.getRequiredFieldDescription(), val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: ko.observable(false) });
    this.Senha = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Senha.getRequiredFieldDescription(), val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: ko.observable(false) });
    this.Token = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Token.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: ko.observable(false) });
    this.CadastrarRotaPorIBGE = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: Localization.Resources.Filiais.Filial.CadastrarRotasTargetIBGE, def: false });
    this.CadastrarRotaPorCoordenadas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: Localization.Resources.Filiais.Filial.CadastrarRotasTargetLatitudeLongitude, def: false });
    this.NaoBuscarCartaoMotoristaTarget = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: Localization.Resources.Filiais.Filial.NaoBuscarCartaoMotoristaTarget, def: false });

    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Filiais.Filial.FornecedorValePedagio.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });

    this.CadastrarRotaPorIBGE.val.subscribe(function (novoValor) {
        if (novoValor) {
            _filialTarget.CadastrarRotaPorCoordenadas.enable(false);
            _filialTarget.CadastrarRotaPorCoordenadas.val(false);
        }
        else {
            _filialTarget.CadastrarRotaPorCoordenadas.enable(true);
        }
    });
    this.CadastrarRotaPorCoordenadas.val.subscribe(function (novoValor) {
        if (novoValor) {
            _filialTarget.CadastrarRotaPorIBGE.enable(false);
            _filialTarget.CadastrarRotaPorIBGE.val(false);
        }
        else {
            _filialTarget.CadastrarRotaPorIBGE.enable(true);
        }
    });
}

//*******EVENTOS*******

function loadFilialTarget() {
    _filialTarget = new FilialTarget();
    KoBindings(_filialTarget, "knockoutFilialTarget");

    new BuscarClientes(_filialTarget.FornecedorValePedagio);
}

//*******MÉTODOS*******

function limparCamposFilialTarget() {
    LimparCampos(_filialTarget);
}

function validaCamposObrigatoriosFilialTarget() {
    _filialTarget.Usuario.required(false);
    _filialTarget.Senha.required(false);
    _filialTarget.Token.required(false);

    if (_filialTarget.Usuario.val() != "" || _filialTarget.Senha.val() != "") {
        _filialTarget.Usuario.required(true);
        _filialTarget.Senha.required(true);
    }

    return ValidarCamposObrigatorios(_filialTarget);
}