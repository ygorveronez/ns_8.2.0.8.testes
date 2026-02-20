var _configuracaoTrizy;

var ConfiguracaoTrizy= function () {
    this.EnviarComprovantesDaCarga = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.EnviarTodosComprovantesDaCarga, val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.VersaoIntegracaoTrizy = PropertyEntity({ text: Localization.Resources.Pedidos.TipoOperacao.VersaoIntegracao, val: ko.observable(EnumVersaoIntegracaoTrizy.Versao1), options: EnumVersaoIntegracaoTrizy.obterOpcoes(), def: EnumVersaoIntegracaoTrizy.Versao1, visible: ko.observable(true) });
};

function LoadConfiguracaoTrizy() {
    _configuracaoTrizy = new ConfiguracaoTrizy();
    KoBindings(_configuracaoTrizy, "tabTrizy");

    _tipoOperacao.EnviarComprovantesDaCarga = _configuracaoTrizy.EnviarComprovantesDaCarga;
    _tipoOperacao.VersaoIntegracaoTrizy = _configuracaoTrizy.VersaoIntegracaoTrizy;
    CarregarCamposIntegracaoTrizy();

    _configuracaoTrizy.VersaoIntegracaoTrizy.val.subscribe(function (novoValor) {
        CarregarCamposIntegracaoTrizy();
    });
}