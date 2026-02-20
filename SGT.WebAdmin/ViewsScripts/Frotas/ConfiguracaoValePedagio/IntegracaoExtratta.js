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
/// <reference path="../../Enumeradores/EnumFornecedorPedagioExtratta.js" />
/// <reference path="../../Enumeradores/EnumTipoRotaExtratta.js" />
/// <reference path="ConfiguracaoValePedagio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoExtratta;

var IntegracaoExtratta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.URL = PropertyEntity({ text: "*URL: ", maxlength: 200, required: ko.observable(true) });
    this.Token = PropertyEntity({ text: "*Token: ", maxlength: 100, required: ko.observable(true) });
    this.CNPJAplicacao = PropertyEntity({ text: "*CNPJ Aplicação:", getType: typesKnockout.cnpj, val: ko.observable(""), def: "", required: ko.observable(true) });
    this.FornecedorParceiro = PropertyEntity({ text: "*Fornecedor Parceiro:", val: ko.observable(EnumFornecedorPedagioExtratta.Moedeiro), options: EnumFornecedorPedagioExtratta.obterOpcoes(), def: EnumFornecedorPedagioExtratta.Moedeiro, required: ko.observable(true) });
    this.TipoRota = PropertyEntity({ text: "*Tipo Rota:", val: ko.observable(EnumTipoRotaExtratta.RotaDinamica), options: EnumTipoRotaExtratta.obterOpcoes(), def: EnumTipoRotaExtratta.RotaDinamica, required: ko.observable(true) });

    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor Vale Pedágio:", idBtnSearch: guid() });
};

//*******EVENTOS*******

function loadConfiguracaoExtratta() {
    _integracaoExtratta = new IntegracaoExtratta();
    KoBindings(_integracaoExtratta, "knockoutIntegracaoExtratta");

    new BuscarClientes(_integracaoExtratta.FornecedorValePedagio);
}

//*******MÉTODOS*******

function limparCamposExtratta() {
    LimparCampos(_integracaoExtratta);
}