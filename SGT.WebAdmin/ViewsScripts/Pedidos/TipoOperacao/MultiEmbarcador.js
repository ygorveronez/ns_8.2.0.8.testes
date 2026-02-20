var _configuracaoIntegracaoMultiEmbarcador;


var ConfiguracaoIntegracaoMultiEmbarcador = function () {
    this.HabilitarIntegracaoMultiEmbarcador = PropertyEntity({ text: "Habilitar integração com o MultiEmbarcador", val: ko.observable(false), def: false, visible: ko.observable(true), getType: typesKnockout.bool });
    this.TokenIntegracaoMultiEmbarcador = PropertyEntity({ text: "*Token: ", required: false });
    this.URLIntegracaoMultiEmbarcador = PropertyEntity({ text: "*URL: ", required: false });
    this.IntegrarCIOTMultiEmbarcador = PropertyEntity({ text: "Integrar os dados do CIOT", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.IntegrarCargasMultiEmbarcador = PropertyEntity({ text: "Importar as cargas", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PermiteUtilizarEmContratoFrete = PropertyEntity({ text: "Importar os fechamentos de contrato de frete", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.NaoImportarCargasComplementaresMultiEmbarcador = PropertyEntity({ text: "Não importar cargas complementares", val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.DataInicialCargasMultiEmbarcador = PropertyEntity({ text: "Data inicial para importar:", val: ko.observable(""), def: "", getType: typesKnockout.date });
    this.NaoGerarCargaMultiEmbarcador = PropertyEntity({ text: "Não gerar uma carga, apenas importar os documentos", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.NaoIntegrarCancelamentoMultiEmbarcadorComDadosInvalidos = PropertyEntity({ text: "Não integrar cancelamentos com dados inválidos (veículo, motorista, etc.)", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.VincularDocumentosAutomaticamenteEmCargaExistenteMultiEmbarcador = PropertyEntity({ text: "Vincular documentos automaticamente em carga existente", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.UtilizarGeracaoDeNFSeAvancada = PropertyEntity({ text: "Utilizar geração de NFS-e avançada (depende da versão do ambiente do embarcador)", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador = PropertyEntity({ text: "Expressão regular para o número do pedido (deve constar na obsevação do CT-e e MDF-e): ", required: false });

    this.HabilitarIntegracaoMultiEmbarcador.val.subscribe(function (novoValor) {
        _configuracaoIntegracaoMultiEmbarcador.TokenIntegracaoMultiEmbarcador.required = novoValor;
        _configuracaoIntegracaoMultiEmbarcador.URLIntegracaoMultiEmbarcador.required = novoValor;
    });

    this.IntegrarCargasMultiEmbarcador.val.subscribe(function (novoValor) {
        _configuracaoIntegracaoMultiEmbarcador.NaoGerarCargaMultiEmbarcador.visible(novoValor);
    });
};

function LoadConfiguracaoIntegracaoMultiEmbarcador() {
    _configuracaoIntegracaoMultiEmbarcador = new ConfiguracaoIntegracaoMultiEmbarcador();
    KoBindings(_configuracaoIntegracaoMultiEmbarcador, "tabIntegracaoMultiEmbarcador");

    _tipoOperacao.HabilitarIntegracaoMultiEmbarcador = _configuracaoIntegracaoMultiEmbarcador.HabilitarIntegracaoMultiEmbarcador;
    _tipoOperacao.TokenIntegracaoMultiEmbarcador = _configuracaoIntegracaoMultiEmbarcador.TokenIntegracaoMultiEmbarcador;
    _tipoOperacao.URLIntegracaoMultiEmbarcador = _configuracaoIntegracaoMultiEmbarcador.URLIntegracaoMultiEmbarcador;
    _tipoOperacao.IntegrarCIOTMultiEmbarcador = _configuracaoIntegracaoMultiEmbarcador.IntegrarCIOTMultiEmbarcador;
    _tipoOperacao.PermiteUtilizarEmContratoFrete = _configuracaoIntegracaoMultiEmbarcador.PermiteUtilizarEmContratoFrete;
    _tipoOperacao.IntegrarCargasMultiEmbarcador = _configuracaoIntegracaoMultiEmbarcador.IntegrarCargasMultiEmbarcador;
    _tipoOperacao.DataInicialCargasMultiEmbarcador = _configuracaoIntegracaoMultiEmbarcador.DataInicialCargasMultiEmbarcador;
    _tipoOperacao.NaoImportarCargasComplementaresMultiEmbarcador = _configuracaoIntegracaoMultiEmbarcador.NaoImportarCargasComplementaresMultiEmbarcador;
    _tipoOperacao.NaoGerarCargaMultiEmbarcador = _configuracaoIntegracaoMultiEmbarcador.NaoGerarCargaMultiEmbarcador;
    _tipoOperacao.NaoIntegrarCancelamentoMultiEmbarcadorComDadosInvalidos = _configuracaoIntegracaoMultiEmbarcador.NaoIntegrarCancelamentoMultiEmbarcadorComDadosInvalidos;
    _tipoOperacao.VincularDocumentosAutomaticamenteEmCargaExistenteMultiEmbarcador = _configuracaoIntegracaoMultiEmbarcador.VincularDocumentosAutomaticamenteEmCargaExistenteMultiEmbarcador;
    _tipoOperacao.UtilizarGeracaoDeNFSeAvancada = _configuracaoIntegracaoMultiEmbarcador.UtilizarGeracaoDeNFSeAvancada;
    _tipoOperacao.ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador = _configuracaoIntegracaoMultiEmbarcador.ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador;
}