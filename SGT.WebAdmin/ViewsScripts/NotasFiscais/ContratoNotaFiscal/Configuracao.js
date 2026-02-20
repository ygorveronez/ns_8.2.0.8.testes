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
/// <reference path="ContratoNotaFiscal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoContratoNotaFiscal;

var ConfiguracaoContratoNotaFiscal = function () {
    this.RecorrenciaEmDias = PropertyEntity({ text: "Recorrência em Dias:", val: ko.observable(""), getType: typesKnockout.int, def: "" });
    this.NotificarPorEmail = PropertyEntity({ text: "Notificar por e-mail?", val: ko.observable(false), getType: typesKnockout.bool, def: false });
};

//*******EVENTOS*******

function LoadConfiguracaoContratoNotaFiscal() {
    _configuracaoContratoNotaFiscal = new ConfiguracaoContratoNotaFiscal();
    KoBindings(_configuracaoContratoNotaFiscal, "knockoutContratoConfiguracao");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        $("#liTabConfiguracao").show();
    }
}

function LimparCamposConfiguracaoContratoNotaFiscal() {
    LimparCampos(_configuracaoContratoNotaFiscal);
}