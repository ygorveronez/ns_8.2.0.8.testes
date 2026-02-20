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
/// <reference path="../../Enumeradores/EnumTipoRotaSemParar.js" />
/// <reference path="Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _filialSemParar;

var FilialSemParar = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DiasPrazo = PropertyEntity({ text: Localization.Resources.Filiais.Filial.DiasPrazo.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0), def: 0, maxlength: 2 });
    this.TipoRota = PropertyEntity({ text: Localization.Resources.Filiais.Filial.TipoRota.getRequiredFieldDescription(), val: ko.observable(EnumTipoRotaSemParar.RotaFixa), options: EnumTipoRotaSemParar.obterOpcoes(), def: EnumTipoRotaSemParar.RotaFixa, enable: ko.observable(true), required: ko.observable(true) });

    this.Usuario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Usuario.getRequiredFieldDescription(), val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: ko.observable(false) });
    this.Senha = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Senha.getRequiredFieldDescription(), val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: ko.observable(false) });
    this.CNPJ = PropertyEntity({ text: Localization.Resources.Filiais.Filial.CNPJ.getRequiredFieldDescription(), enable: ko.observable(true), required: ko.observable(false) });
    this.NomeRpt = PropertyEntity({ text: Localization.Resources.Filiais.Filial.NomeArquivoRelatorio.getFieldDescription(), val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao1 = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao + " 1:", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao2 = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao + " 2:", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao3 = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao + " 3:", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao4 = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao + " 4:", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao5 = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao + " 5:", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.Observacao6 = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao + " 6:", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true) });
    this.UtilizarModeoVeicularCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, enable: ko.observable(true), text: Localization.Resources.Filiais.Filial.UtilizarModeloVeicularCarga, def: false });

    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Filiais.Filial.FornecedorValePedagio.getFieldDescription(), idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadFilialSemParar() {
    _filialSemParar = new FilialSemParar();
    KoBindings(_filialSemParar, "knockoutFilialSemParar");

    new BuscarClientes(_filialSemParar.FornecedorValePedagio);
}

//*******MÉTODOS*******

function limparCamposFilialSemParar() {
    LimparCampos(_filialSemParar);
}

function validaCamposObrigatoriosFilialSemParar() {
    _filialSemParar.Usuario.required(false);
    _filialSemParar.Senha.required(false);
    _filialSemParar.CNPJ.required(false);

    if (_filialSemParar.Usuario.val() != "" || _filialSemParar.Senha.val() != "" || _filialSemParar.CNPJ.val() != "") {
        _filialSemParar.Usuario.required(true);
        _filialSemParar.Senha.required(true);
        _filialSemParar.CNPJ.required(true);
    }

    return ValidarCamposObrigatorios(_filialSemParar);
}