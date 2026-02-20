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
/// <reference path="../../Enumeradores/EnumTipoIntegradoraCIOT.js" />


var _configuracaoCIOT;

var _tipoIntegradoraCIOT = EnumTipoIntegradoraCIOT.obterOpcoes([
    EnumTipoIntegradoraCIOT.EFrete,
]);

var ConfiguracaoCIOT = function () {
    this.HabilitarCIOT = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.HabilitarConfiguracaoCIOT, def: false, getType: typesKnockout.bool });
    this.HabilitarCIOT.val.subscribe(HabilitarCIOTChange);

    this.ObrigatoriedadeCIOTEmissaoMDFe = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.ObrigatoriedadeCIOTEmitirMDFe, def: false, getType: typesKnockout.bool });
    this.EncerrarCIOTPorViagem = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Transportadores.Transportador.EncerrarCIOTFimViagem, def: false, getType: typesKnockout.bool });

    this.TipoIntegracaoCIOT = PropertyEntity({ val: ko.observable(EnumTipoIntegradoraCIOT.EFrete), options: _tipoIntegradoraCIOT, def: EnumTipoIntegradoraCIOT.EFrete, text: Localization.Resources.Transportadores.Transportador.TipoIntegradoraCIOT.getFieldDescription(), integradora: ko.observable("efrete"), defIntegradora: 'efrete' });
    this.TipoIntegracaoCIOT.val.subscribe(TipoIntegracaoCIOTChange);

    this.CodigoIntegracaoEfrete = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Transportadores.Transportador.CodigoIntegracaoEfrete.getFieldDescription() });
}

//*******EVENTOS*******

function loadConfiguracoesCIOT() {
    _configuracaoCIOT = new ConfiguracaoCIOT();
    KoBindings(_configuracaoCIOT, "knockoutCadastroConfiguracaoCIOT");
}

function TipoIntegracaoCIOTChange(integradora) {
    switch (integradora) {
        case EnumTipoIntegradoraCIOT.EFrete:
            return "efrete";
        default:
            return "";
    }
}

function HabilitarCIOTChange(habilitado) {
    if (habilitado)
        $("#liTabImpostosCIOT").show();
    else
        $("#liTabImpostosCIOT").hide();
}

function limparCamposConfiguracoesCIOT() {
    LimparCampos(_configuracaoCIOT);
    _configuracaoCIOT.TipoIntegracaoCIOT.integradora(_configuracaoCIOT.TipoIntegracaoCIOT.defIntegradora);
    $("#liTabImpostosCIOT").hide();
}