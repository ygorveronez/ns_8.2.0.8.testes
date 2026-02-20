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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracaoRepom.js" />
/// <reference path="../../Enumeradores/EnumTipoRotaFreteRepom.js" />
/// <reference path="ConfiguracaoValePedagio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoRepom;

var IntegracaoRepom = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //SOAP
    this.CodigoCliente = PropertyEntity({ text: "*Cód. Cliente: ", val: ko.observable(""), def: "", maxlength: 500, enable: ko.observable(true), required: ko.observable(true) });
    this.CodigoFilial = PropertyEntity({ text: "*Cód .Filial: ", val: ko.observable(""), def: "", maxlength: 500, enable: ko.observable(true), required: ko.observable(true) });
    this.AssinaturaDigital = PropertyEntity({ text: "*Assinatura Digital: ", val: ko.observable(""), def: "", maxlength: 1000, enable: ko.observable(true), required: ko.observable(true) });

    //REST
    this.URLAutenticacaoRota = PropertyEntity({ text: "*URL Autenticação Rota: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: ko.observable(false) });
    this.URLRota = PropertyEntity({ text: "*URL Rota: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: ko.observable(false) });
    this.ClientID = PropertyEntity({ text: "*Client ID Rota: ", val: ko.observable(""), def: "", maxlength: 100, enable: ko.observable(true), required: ko.observable(false) });
    this.ClientSecret = PropertyEntity({ text: "*Client Secret Rota: ", val: ko.observable(""), def: "", maxlength: 100, enable: ko.observable(true), required: ko.observable(false) });
    this.URLViagem = PropertyEntity({ text: "*URL Viagem: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: ko.observable(false) });
    this.Usuario = PropertyEntity({ text: "*Usuário Viagem: ", val: ko.observable(""), def: "", maxlength: 100, enable: ko.observable(true), required: ko.observable(false) });
    this.Senha = PropertyEntity({ text: "*Senha Viagem: ", val: ko.observable(""), def: "", maxlength: 100, enable: ko.observable(true), required: ko.observable(false) });

    this.TipoIntegracaoRepom = PropertyEntity({ text: "*Tipo de Integração:", val: ko.observable(EnumTipoIntegracaoRepom.SOAP), options: EnumTipoIntegracaoRepom.obterOpcoes(), def: EnumTipoIntegracaoRepom.SOAP, enable: ko.observable(true), required: ko.observable(true) });
    this.TipoRotaFreteRepom = PropertyEntity({ text: "Tipo Rota Frete:", val: ko.observable(EnumTipoRotaFreteRepom.NaoEspecificado), options: EnumTipoRotaFreteRepom.obterOpcoes(), def: EnumTipoRotaFreteRepom.NaoEspecificado, enable: ko.observable(true), required: ko.observable(false) });

    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor Vale Pedágio:", idBtnSearch: guid() });
    this.FilialCompra = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial para Compra:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.ConsiderarEixosSuspensosNaConsultaDoValePedagio = PropertyEntity({ text: "Considerar Eixos Suspensos na Consulta do Vale Pedágio", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.ConsiderarRotaFreteDaCargaNoValePedagio = PropertyEntity({ text: "Considerar Rota Frete da Carga no Vale Pedágio (Roteirização)", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    
    this.TipoIntegracaoRepom.val.subscribe(function (novoValor) {
        limparCamposREST();
        requiredCamposSOAP(true);
        requiredCamposREST(false);
        $("#liConfiguracaoSoapRepom").show();
        $("#liConfiguracaoRestRepom").hide();

        _integracaoRepom.FilialCompra.visible(novoValor === EnumTipoIntegracaoRepom.REsT);
        
        if (novoValor === EnumTipoIntegracaoRepom.REsT) {
            limparCamposSOAP();
            requiredCamposSOAP(false);
            requiredCamposREST(true);
            $("#liConfiguracaoSoapRepom").hide();
            $("#liConfiguracaoRestRepom").show();
        }
    });
};

//*******EVENTOS*******

function loadConfiguracaoRepom() {
    _integracaoRepom = new IntegracaoRepom();
    KoBindings(_integracaoRepom, "knockoutIntegracaoRepom");

    new BuscarClientes(_integracaoRepom.FornecedorValePedagio);
    new BuscarFilial(_integracaoRepom.FilialCompra);

    $("#liConfiguracaoRestRepom").hide();
}

//*******MÉTODOS*******

function requiredCamposSOAP(required) {
    _integracaoRepom.CodigoCliente.required(required);
    _integracaoRepom.CodigoFilial.required(required);
    _integracaoRepom.AssinaturaDigital.required(required);
}

function requiredCamposREST(required) {
    _integracaoRepom.URLAutenticacaoRota.required(required);
    _integracaoRepom.URLRota.required(required);
    _integracaoRepom.ClientID.required(required);
    _integracaoRepom.ClientSecret.required(required);
    _integracaoRepom.URLViagem.required(required);
    _integracaoRepom.Usuario.required(required);
    _integracaoRepom.Senha.required(required);
}

function limparCamposSOAP() {
    LimparCampo(_integracaoRepom.CodigoCliente);
    LimparCampo(_integracaoRepom.CodigoFilial);
    LimparCampo(_integracaoRepom.AssinaturaDigital);
}

function limparCamposREST() {
    LimparCampo(_integracaoRepom.URLAutenticacaoRota);
    LimparCampo(_integracaoRepom.URLRota);
    LimparCampo(_integracaoRepom.ClientID);
    LimparCampo(_integracaoRepom.ClientSecret);
    LimparCampo(_integracaoRepom.URLViagem);
    LimparCampo(_integracaoRepom.Usuario);
    LimparCampo(_integracaoRepom.Senha);
    LimparCampo(_integracaoRepom.FilialCompra);
}