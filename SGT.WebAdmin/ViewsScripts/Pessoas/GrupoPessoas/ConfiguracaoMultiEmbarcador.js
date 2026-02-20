var _configuracaoMultiEmbarcador;

//*******MAPEAMENTO KNOUCKOUT*******

var ConfiguracaoMultiEmbarcador = function () {
    this.HabilitarIntegracaoVeiculoMultiEmbarcador = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.HabilitarIntegracaoVeiculosEmbarcador, val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.HabilitarIntegracaoOcorrenciasTMSWSMultiEmbarcador = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.HabilitarIntegracaoOcorrenciasEmbarcador, val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.DataInicialIntegracaoOcorrenciaTMSWSMultiEmbarcador = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.DataInicialIntegracao.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.NaoGerarOcorreciaApenasDocumentos = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NaoGerarOcorrenciaApenasImportar, val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true) });
    this.HabilitarIntegracaoOcorrenciasMultiEmbarcador = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.HabilitarIntegracaoOcorrenciasEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.HabilitarIntegracaoDigitalizacaoCanhotoMultiEmbarcador = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.HabilitarIntegracaoCanhotosDigitalizados, val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.HabilitarIntegracaoXmlCteMultiEmbarcador = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.HabilitarIntegracaoXMLdosCTes, val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.URLIntegracaoMultiEmbarcador = PropertyEntity({ text: "URL:", val: ko.observable(""), def: "", getType: typesKnockout.string, maxLength: 250, enable: ko.observable(true) });
    this.TokenIntegracaoMultiEmbarcador = PropertyEntity({ text: "Token:", val: ko.observable(""), def: "", getType: typesKnockout.string, maxLength: 150, enable: ko.observable(true) });

    this.HabilitarIntegracaoVeiculoMultiEmbarcador.val.subscribe(function (novoValor) {
        if (novoValor)
            $("#divModeloVeicularEmbarcador").show();
        else
            $("#divModeloVeicularEmbarcador").hide();
    });
};

//*******EVENTOS*******

function LoadConfiguracaoMultiEmbarcador() {
    _configuracaoMultiEmbarcador = new ConfiguracaoMultiEmbarcador();
    KoBindings(_configuracaoMultiEmbarcador, "knockoutConfiguracaoMultiEmbarcador");

    _grupoPessoas.HabilitarIntegracaoDigitalizacaoCanhotoMultiEmbarcador = _configuracaoMultiEmbarcador.HabilitarIntegracaoDigitalizacaoCanhotoMultiEmbarcador;
    _grupoPessoas.HabilitarIntegracaoOcorrenciasTMSWSMultiEmbarcador = _configuracaoMultiEmbarcador.HabilitarIntegracaoOcorrenciasTMSWSMultiEmbarcador;
    _grupoPessoas.DataInicialIntegracaoOcorrenciaTMSWSMultiEmbarcador = _configuracaoMultiEmbarcador.DataInicialIntegracaoOcorrenciaTMSWSMultiEmbarcador;
    _grupoPessoas.NaoGerarOcorreciaApenasDocumentos = _configuracaoMultiEmbarcador.NaoGerarOcorreciaApenasDocumentos;
    _grupoPessoas.HabilitarIntegracaoVeiculoMultiEmbarcador = _configuracaoMultiEmbarcador.HabilitarIntegracaoVeiculoMultiEmbarcador;
    _grupoPessoas.HabilitarIntegracaoOcorrenciasMultiEmbarcador = _configuracaoMultiEmbarcador.HabilitarIntegracaoOcorrenciasMultiEmbarcador;
    _grupoPessoas.HabilitarIntegracaoXmlCteMultiEmbarcador = _configuracaoMultiEmbarcador.HabilitarIntegracaoXmlCteMultiEmbarcador;
    _grupoPessoas.URLIntegracaoMultiEmbarcador = _configuracaoMultiEmbarcador.URLIntegracaoMultiEmbarcador;
    _grupoPessoas.TokenIntegracaoMultiEmbarcador = _configuracaoMultiEmbarcador.TokenIntegracaoMultiEmbarcador;

}

function limparCamposConfiguracaoMultiEmbarcador() {
    LimparCampos(_configuracaoMultiEmbarcador);
}