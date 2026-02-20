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
/// <reference path="ConfiguracaoValePedagio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoAmbipar;

var IntegracaoAmbipar = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.URL = PropertyEntity({ text: "*URL: ", maxlength: 200, required: ko.observable(true) });
    this.Usuario = PropertyEntity({ text: "*Usuário: ", maxlength: 100, enable: ko.observable(true), required: ko.observable(true) });
    this.Senha = PropertyEntity({ text: "*Senha: ", maxlength: 100, enable: ko.observable(true), required: ko.observable(true) });

    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor Vale Pedágio:", idBtnSearch: guid() });
};

//*******EVENTOS*******

function loadConfiguracaoAmbipar() {
    _integracaoAmbipar = new IntegracaoAmbipar();
    KoBindings(_integracaoAmbipar, "knockoutIntegracaoAmbipar");

    new BuscarClientes(_integracaoAmbipar.FornecedorValePedagio);
}

//*******MÉTODOS*******

function limparCamposAmbipar() {
    LimparCampos(_integracaoAmbipar);
}