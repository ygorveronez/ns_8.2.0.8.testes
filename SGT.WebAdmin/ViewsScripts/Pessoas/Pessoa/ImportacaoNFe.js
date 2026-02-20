/// <reference path="Pessoa.js" />

var _importacaoNFe;

var ImportacaoNFe = function () {
    this.LerVeiculoObservacaoNotaParaAbastecimento = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.LerDadosVeiculosDaObservacaoDaNotaFiscalDeAbastecimentoParaDocumentoDeEntrada, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.LerPlacaObservacaoNotaParaAbastecimentoInicial = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.IdentificacaoDeInicialDaPlaca.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerPlacaObservacaoNotaParaAbastecimentoFinal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.IdentificacaoDeFinalDaPlaca.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerChassiObservacaoNotaParaAbastecimentoInicial = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.IdentificacaoDeInicialDoNumeroDoChassi.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerChassiObservacaoNotaParaAbastecimentoFinal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.IdentificacaoDeFinalDoNumeroDoChassi.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerKMObservacaoNotaParaAbastecimentoInicial = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.IdentificacaoDeInicialDoKM.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerKMObservacaoNotaParaAbastecimentoFinal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.IdentificacaoDeFinalDoKM.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerHorimetroObservacaoNotaParaAbastecimentoInicial = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.IdentificacaoDeInicialDoHorimetro.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerHorimetroObservacaoNotaParaAbastecimentoFinal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.IdentificacaoDeFinalDoHorimetro.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ProcessarAbastecimentoAutomaticamenteAoReceberXmlDaNFE, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false) });

    this.LerVeiculoObservacaoNotaParaAbastecimento.val.subscribe(function (novoValor) {

        _importacaoNFe.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe.visible(true);

    });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadImportacaoNFe() {
    _importacaoNFe = new ImportacaoNFe();
    KoBindings(_importacaoNFe, "knockoutImportacaoNfe");

    _pessoa.LerVeiculoObservacaoNotaParaAbastecimento = _importacaoNFe.LerVeiculoObservacaoNotaParaAbastecimento;
    _pessoa.LerPlacaObservacaoNotaParaAbastecimentoInicial = _importacaoNFe.LerPlacaObservacaoNotaParaAbastecimentoInicial;
    _pessoa.LerPlacaObservacaoNotaParaAbastecimentoFinal = _importacaoNFe.LerPlacaObservacaoNotaParaAbastecimentoFinal;
    _pessoa.LerChassiObservacaoNotaParaAbastecimentoInicial = _importacaoNFe.LerChassiObservacaoNotaParaAbastecimentoInicial;
    _pessoa.LerChassiObservacaoNotaParaAbastecimentoFinal = _importacaoNFe.LerChassiObservacaoNotaParaAbastecimentoFinal;
    _pessoa.LerKMObservacaoNotaParaAbastecimentoInicial = _importacaoNFe.LerKMObservacaoNotaParaAbastecimentoInicial;
    _pessoa.LerKMObservacaoNotaParaAbastecimentoFinal = _importacaoNFe.LerKMObservacaoNotaParaAbastecimentoFinal;
    _pessoa.LerHorimetroObservacaoNotaParaAbastecimentoInicial = _importacaoNFe.LerHorimetroObservacaoNotaParaAbastecimentoInicial;
    _pessoa.LerHorimetroObservacaoNotaParaAbastecimentoFinal = _importacaoNFe.LerHorimetroObservacaoNotaParaAbastecimentoFinal;
    _pessoa.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe = _importacaoNFe.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe;

  
   
}