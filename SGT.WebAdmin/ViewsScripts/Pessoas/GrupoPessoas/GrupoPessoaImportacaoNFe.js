/// <reference path="GrupoPessoas.js" />

var _grupoPessoaImportacaoNFe;

var GrupoPessoaImportacaoNFe = function () {
    this.VincularNotaFiscalEmailNaCarga = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.VincularNotaLidaNaCarga, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });

    this.LerPlacaDaObservacaoDaNota = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LerPlacaObservacaoNotaParaCTe, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.LerPlacaDaObservacaoDaNotaInicio = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoInicio.getFieldDescription(), maxlength: 50, enable: ko.observable(true) });
    this.LerPlacaDaObservacaoDaNotaFim = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoFim.getFieldDescription(), maxlength: 50, enable: ko.observable(true) });

    this.LerPlacaDaObservacaoContribuinteDaNota = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LerPlacaObservacaoContribuinte, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.LerPlacaDaObservacaoContribuinteDaNotaIdentificacao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoXCampo.getFieldDescription(), maxlength: 20, enable: ko.observable(true) });

    this.LerVeiculoObservacaoNotaParaAbastecimento = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LerDadosVeiculoObservacaoNotaFiscal, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.LerPlacaObservacaoNotaParaAbastecimentoInicial = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoInicialPlaca.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerPlacaObservacaoNotaParaAbastecimentoFinal = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoFinalPlaca.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerChassiObservacaoNotaParaAbastecimentoInicial = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoInicialChassi.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerChassiObservacaoNotaParaAbastecimentoFinal = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoFinalChassi.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerKMObservacaoNotaParaAbastecimentoInicial = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoInicialKm.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerKMObservacaoNotaParaAbastecimentoFinal = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoFinalKm.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerHorimetroObservacaoNotaParaAbastecimentoInicial = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoInicialHorimetro.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });
    this.LerHorimetroObservacaoNotaParaAbastecimentoFinal = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoFinalHorimetro.getFieldDescription(), maxlength: 100, enable: ko.observable(true) });

    this.LerNumeroPedidoDaObservacaoDaNota = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LerNumeroPedidoObservacao, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.LerNumeroPedidoDaObservacaoDaNotaInicio = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoInicio.getFieldDescription(), maxlength: 50, enable: ko.observable(true) });
    this.LerNumeroPedidoDaObservacaoDaNotaFim = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.IdentificacaoFim.getFieldDescription(), maxlength: 50, enable: ko.observable(true) });

    this.ExpressaoBooking = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ExpressaoBooking.getFieldDescription(), maxlength: 500, enable: ko.observable(true) });
    this.ExpressaoContainer = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ExpressaoContainer.getFieldDescription(), maxlength: 500, enable: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGrupoPessoaImportacaoNFe() {
    _grupoPessoaImportacaoNFe = new GrupoPessoaImportacaoNFe();
    KoBindings(_grupoPessoaImportacaoNFe, "knockoutImportacaoNfe");

    _grupoPessoas.VincularNotaFiscalEmailNaCarga = _grupoPessoaImportacaoNFe.VincularNotaFiscalEmailNaCarga;

    _grupoPessoas.LerPlacaDaObservacaoDaNota = _grupoPessoaImportacaoNFe.LerPlacaDaObservacaoDaNota;
    _grupoPessoas.LerPlacaDaObservacaoDaNotaInicio = _grupoPessoaImportacaoNFe.LerPlacaDaObservacaoDaNotaInicio;
    _grupoPessoas.LerPlacaDaObservacaoDaNotaFim = _grupoPessoaImportacaoNFe.LerPlacaDaObservacaoDaNotaFim;

    _grupoPessoas.LerPlacaDaObservacaoContribuinteDaNota = _grupoPessoaImportacaoNFe.LerPlacaDaObservacaoContribuinteDaNota;
    _grupoPessoas.LerPlacaDaObservacaoContribuinteDaNotaIdentificacao = _grupoPessoaImportacaoNFe.LerPlacaDaObservacaoContribuinteDaNotaIdentificacao;

    _grupoPessoas.LerVeiculoObservacaoNotaParaAbastecimento = _grupoPessoaImportacaoNFe.LerVeiculoObservacaoNotaParaAbastecimento;
    _grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoInicial = _grupoPessoaImportacaoNFe.LerPlacaObservacaoNotaParaAbastecimentoInicial;
    _grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoFinal = _grupoPessoaImportacaoNFe.LerPlacaObservacaoNotaParaAbastecimentoFinal;
    _grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoInicial = _grupoPessoaImportacaoNFe.LerChassiObservacaoNotaParaAbastecimentoInicial;
    _grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoFinal = _grupoPessoaImportacaoNFe.LerChassiObservacaoNotaParaAbastecimentoFinal;
    _grupoPessoas.LerKMObservacaoNotaParaAbastecimentoInicial = _grupoPessoaImportacaoNFe.LerKMObservacaoNotaParaAbastecimentoInicial;
    _grupoPessoas.LerKMObservacaoNotaParaAbastecimentoFinal = _grupoPessoaImportacaoNFe.LerKMObservacaoNotaParaAbastecimentoFinal;
    _grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoInicial = _grupoPessoaImportacaoNFe.LerHorimetroObservacaoNotaParaAbastecimentoInicial;
    _grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoFinal = _grupoPessoaImportacaoNFe.LerHorimetroObservacaoNotaParaAbastecimentoFinal;
    _grupoPessoas.ExpressaoBooking = _grupoPessoaImportacaoNFe.ExpressaoBooking;
    _grupoPessoas.ExpressaoContainer = _grupoPessoaImportacaoNFe.ExpressaoContainer;

    _grupoPessoas.LerNumeroPedidoDaObservacaoDaNota = _grupoPessoaImportacaoNFe.LerNumeroPedidoDaObservacaoDaNota;
    _grupoPessoas.LerNumeroPedidoDaObservacaoDaNotaInicio = _grupoPessoaImportacaoNFe.LerNumeroPedidoDaObservacaoDaNotaInicio;
    _grupoPessoas.LerNumeroPedidoDaObservacaoDaNotaFim = _grupoPessoaImportacaoNFe.LerNumeroPedidoDaObservacaoDaNotaFim;
}