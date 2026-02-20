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

var _integracaoEFrete;

var IntegracaoEFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.URL = PropertyEntity({ text: "*URL: ", maxlength: 200, enable: ko.observable(true), required: ko.observable(true) });
    this.CodigoIntegrador = PropertyEntity({ text: "*Código Integrador: ", maxlength: 100, enable: ko.observable(true), required: ko.observable(true) });
    this.Usuario = PropertyEntity({ text: "*Usuário: ", maxlength: 100, enable: ko.observable(true), required: ko.observable(true) });
    this.Senha = PropertyEntity({ text: "*Senha: ", maxlength: 100, enable: ko.observable(true), required: ko.observable(true) });
    this.DiasPrazo = PropertyEntity({ text: "Dias Prazo:", getType: typesKnockout.int, val: ko.observable(""), def: "", maxlength: 2 });

    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor Vale Pedágio:", idBtnSearch: guid() });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.NotificarTransportadorPorEmail = PropertyEntity({ text: "Notificar transportador por e-mail", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.EnviarPontosPassagemRotaFrete = PropertyEntity({ text: "Enviar pontos de passagem da rota do frete", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.EnviarPolilinhaRoteirizacaoCarga = PropertyEntity({ text: "Enviar polilinha da roteirização da carga", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.EnviarTipoVeiculoNaIntegracao = PropertyEntity({ text: "Enviar tipo veículo na integração", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadConfiguracaoEFrete() {
    _integracaoEFrete = new IntegracaoEFrete();
    KoBindings(_integracaoEFrete, "knockoutIntegracaoEFrete");

    BuscarClientes(_integracaoEFrete.FornecedorValePedagio);
    BuscarClientes(_integracaoEFrete.Cliente);
}

//*******MÉTODOS*******

function limparCamposEFrete() {
    LimparCampos(_integracaoEFrete);
}