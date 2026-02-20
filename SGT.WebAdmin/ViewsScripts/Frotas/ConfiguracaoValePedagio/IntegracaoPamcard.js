/// <reference path="../../../wwwroot/js/libs/jquery-2.1.1.js" />
/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/bootstrap/bootstrap.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.blockui.js" />
/// <reference path="../../../wwwroot/js/Global/knoutViewsSlides.js" />
/// <reference path="../../../wwwroot/js/libs/jquery.maskMoney.js" />
/// <reference path="../../../wwwroot/js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Enumeradores/EnumTipoRotaPamcard.js" />
/// <reference path="../../Enumeradores/EnumAcoesPamcard.js" />
/// <reference path="ConfiguracaoValePedagio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _integracaoPamcard;

var IntegracaoPamcard = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.URL = PropertyEntity({ text: "*URL: ", val: ko.observable(""), def: "", maxlength: 200, enable: ko.observable(true), required: ko.observable(true) });
    this.TipoRota = PropertyEntity({ text: "*Tipo Rota:", val: ko.observable(EnumTipoRotaPamcard.RotaFixa), options: EnumTipoRotaPamcard.obterOpcoes(), def: EnumTipoRotaPamcard.RotaFixa, enable: ko.observable(true), required: ko.observable(true) });
    this.AcoesPamcard = PropertyEntity({ text: "Ações:", val: ko.observable(EnumAcoesPamcard.SomenteCompra), options: EnumAcoesPamcard.obterOpcoes(), def: EnumAcoesPamcard.SomenteCompra, enable: ko.observable(true), required: ko.observable(true) });
    this.AdicionarValorConsultadoComoComponentePedagioCarga = PropertyEntity({ text: "Liberar adicionar valor consultado como componente de pedágio na carga", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(false) });
    this.UtilizarCertificadoFilialMatrizCompraValePedagio = PropertyEntity({ text: "Utilizar certificado da filial matriz para compra de vale pedágio", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.EnviarCEPsNaIntegracao = PropertyEntity({ text: "Enviar CEPs na integração", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.SomarEixosSuspensosValePedagio = PropertyEntity({ text: "Somar Eixos Suspensos Vale Pedágio", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.NaoEnviarIdaVoltaValePedagio = PropertyEntity({ text: "Não Enviar Ida Volta Vale Pedágio", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });
    this.ConsiderarRotaFreteDaCargaNoValePedagio = PropertyEntity({ text: "Considerar Rota Frete da Carga no Vale Pedágio (Roteirização)", val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true) });

    this.AcoesPamcard.val.subscribe(function (acao) {
        if (acao === EnumAcoesPamcard.ConsultaCompra || acao === EnumAcoesPamcard.SomenteConsulta)
            _integracaoPamcard.AdicionarValorConsultadoComoComponentePedagioCarga.visible(true);
        else
            _integracaoPamcard.AdicionarValorConsultadoComoComponentePedagioCarga.visible(false);
    });

    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor Vale Pedágio:", idBtnSearch: guid() });
};

//*******EVENTOS*******

function loadConfiguracaoPamcard() {
    _integracaoPamcard = new IntegracaoPamcard();
    KoBindings(_integracaoPamcard, "knockoutIntegracaoPamcard");

    BuscarClientes(_integracaoPamcard.FornecedorValePedagio);
}

//*******MÉTODOS*******

function limparCamposPamcard() {
    LimparCampos(_integracaoPamcard);
}