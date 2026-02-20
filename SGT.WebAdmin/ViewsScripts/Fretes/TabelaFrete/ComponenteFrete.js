/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Enumeradores/EnumTipoCalculoPesoTabelaFreteComponenteFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoCalculoVolumeTabelaFreteComponenteFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoCalculoCubagemTabelaFreteComponenteFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoComponenteTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoPercentualComponenteTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoCalculoComponenteTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoFinalidadeJustificativa.js" />
/// <reference path="../../Enumeradores/EnumTipoJustificativa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridComponenteFrete;
var _componenteFrete;
var _gridTempoComponenteFrete;

var ComponenteFreteFaixaTempo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HoraInicialTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraInicial.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(false) });
    this.HoraFinalTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraFinal.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(false) });
    this.ValorTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Valor.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.decimal, visible: ko.observable(false) });

    this.PeriodoInicialTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.EoPeriodoInicial, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.HoraInicialCobrancaMinimaTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraInicialCobrancaMinima.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(false) });
    this.HoraFinalCobrancaMinimaTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraFinalCobrancaMinima.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(false) });
};

var ComponenteFrete = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Componente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFrete.Componente.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, issue: 85 });
    this.TipoComponenteFrete = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.IncluirBaseCalculoICMS = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.IncluirNaBaseDeCalculoICMS, issue: 721, val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.IgnorarComponenteQuandoVeiculoPossuiTagValePedagio = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.IgnorarComponenteQuandoVeiculoPossuirTagValePedagio, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.Percentual = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Percentual.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 7, issue: 1336, visible: ko.observable(false), configDecimal: { precision: 3, allowZero: false, allowNegative: false } });
    this.TipoPercentual = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Incide.getRequiredFieldDescription(), options: EnumTipoPercentualComponenteTabelaFrete.ObterOpcoes(), val: ko.observable(EnumTipoPercentualComponenteTabelaFrete.SobreNotaFiscal), def: EnumTipoPercentualComponenteTabelaFrete.SobreNotaFiscal, issue: 1334, visible: ko.observable(false) });
    this.IncluirBaseCalculo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.IncluirOValorNaBaseDeCalculo, issue: 1610, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.Peso = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PesoKG.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 18, configDecimal: { precision: 6, allowZero: false, allowNegative: false }, issue: 0, visible: ko.observable(false) });
    this.ValorExcedentePorKG = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValorExcedentePorKG.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 6, allowZero: false, allowNegative: false }, issue: 0, visible: ko.observable(false) });
    this.TipoCalculoPeso = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TipoDeCalculoPorPeso.getFieldDescription(), options: EnumTipoCalculoPesoTabelaFreteComponenteFrete.obterOpcoes(), val: ko.observable(EnumTipoCalculoPesoTabelaFreteComponenteFrete.PorFracao), def: EnumTipoCalculoPesoTabelaFreteComponenteFrete.PorFracao, issue: 0, visible: ko.observable(false) });
    this.ValorFormula = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.InformarOValorDaFormula.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 18, configDecimal: { precision: 6, allowZero: false, allowNegative: false }, issue: 1336, visible: ko.observable(false) });

    this.Volume = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Volume.getRequiredFieldDescription(), getType: typesKnockout.int, maxlength: 10, issue: 0, visible: ko.observable(false) });
    this.ValorExcedentePorVolume = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValorExcedentePorVolume.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 6, allowZero: false, allowNegative: false }, issue: 0, visible: ko.observable(false) });
    this.TipoCalculoVolume = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TipoCalculoVolume.getRequiredFieldDescription(), options: EnumTipoCalculoVolumeTabelaFreteComponenteFrete.obterOpcoes(), val: ko.observable(EnumTipoCalculoVolumeTabelaFreteComponenteFrete.PorFracao), def: EnumTipoCalculoVolumeTabelaFreteComponenteFrete.PorFracao, issue: 0, visible: ko.observable(false) });


    this.ValorInformadoNaTabela = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.OValorPercentualDestaFormulaSeraInformadoNaTabelaFrete, issue: 1332, val: ko.observable(true), def: true, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.UtilizarCalculoDesseComponenteNaOcorrencia = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarCalculoDesseComponenteNaOcorrencia, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.ValorUnicoParaCarga = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.OValorDesteComponenteSeraUmaTaxaParaTodaCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.TipoDocumentoQuantidadeDocumentos = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TipoDeDocumento.getFieldDescription(), options: EnumTipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.ObterOpcoes(), val: ko.observable(EnumTipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorNotaFiscal), def: EnumTipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorNotaFiscal, issue: 0, visible: ko.observable(false) });
    this.ModeloDocumentoFiscalRestringirQuantidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFrete.SomenteParaModeloDoDocumento.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(false) });

    this.ValorMinimo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValorMinimo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 10, issue: 1337, visible: ko.observable(true) });
    this.ValorMaximo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValorMaximo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 1337, visible: ko.observable(true) });

    this.EntregaMinima = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.QuantidadeEntregasMinima.getFieldDescription(), getType: typesKnockout.int, maxlength: 10, visible: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFrete.JustificativaAcrescimoContratoTerceiros.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.Tipo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Tipo.getRequiredFieldDescription(), options: EnumTipoComponenteTabelaFrete.ObterOpcoes(), val: ko.observable(EnumTipoComponenteTabelaFrete.ValorFixo), def: EnumTipoComponenteTabelaFrete.ValorFixo, issue: 1331, visible: ko.observable(true) });
    this.TipoCalculo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TipoDeCalculo.getFieldDescription(), options: EnumTipoCalculoComponenteTabelaFrete.ObterOpcoes(), val: ko.observable(EnumTipoCalculoComponenteTabelaFrete.Nenhum), def: EnumTipoCalculoComponenteTabelaFrete.Nenhum, issue: 0, visible: ko.observable(true) });
    this.ComponenteComparado = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.EsseComponenteDeveSerComparadoAosDemais, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.UtilizarFormulaRateioCarga = PropertyEntity({ text: "Utilizar Formula Rateio Carga", val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.Cubagem = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.CubagemM3.getRequiredFieldDescription(), getType: typesKnockout.decimal, maxlength: 10, issue: 0, visible: ko.observable(false) });
    this.TipoCalculoCubagem = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TipoDeCalculoPorCubagem.getFieldDescription(), options: EnumTipoCalculoCubagemTabelaFreteComponenteFrete.obterOpcoes(), val: ko.observable(EnumTipoCalculoCubagemTabelaFreteComponenteFrete.PorUnidadeIncompleta), def: EnumTipoCalculoCubagemTabelaFreteComponenteFrete.PorUnidadeIncompleta, issue: 0, visible: ko.observable(false) });

    this.ValidarValorMercadoria = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValidarOValorDaMercadoria, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.ValorMercadoriaMinimo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValorDaMercadoriaMinimo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });
    this.ValorMercadoriaMaximo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValorDaMercadoriaMaximo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });

    this.ValidarPesoMercadoria = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValidarPesoDaMercadoria, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.PesoMercadoriaMinimo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PesoDaMercadoriaMinimo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });
    this.PesoMercadoriaMaximo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PesoDaMercadoriaMaximo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });

    this.ValidarDimensoesMercadoria = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ValidarAsInformacoesDaMercadoria, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.AlturaMercadoriaMinima = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.AlturaMinima.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });
    this.AlturaMercadoriaMaxima = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.AlturaMaxima.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });
    this.LarguraMercadoriaMinima = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.LarguraMinima.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });
    this.LarguraMercadoriaMaxima = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.LarguraMaxima.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });
    this.ComprimentoMercadoriaMinimo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ComprimentoMinimo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });
    this.ComprimentoMercadoriaMaximo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.ComprimentoMaximo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });
    this.VolumeMercadoriaMinimo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TotalMinimo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });
    this.VolumeMercadoriaMaximo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TotalMaximo.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, issue: 0, visible: ko.observable(false) });

    this.UtilizarDiasEspecificos = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarSomenteEmDiasEspecificos, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.SegundaFeira = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.SegundaFeira, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TercaFeira = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.TercaFeira, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.QuartaFeira = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.QuartaFeira, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.QuintaFeira = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.QuintaFeira, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.SextaFeira = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.SextaFeira, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.Sabado = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Sabado, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.Domingo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Domingo, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.Feriados = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Feriados, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.UtilizarPeriodoColeta = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarSomenteEmHorarioDeColetaEspecifico, issue: 0, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.HoraColetaInicial = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HorarioDeColetaInicial.getRequiredFieldDescription(), getType: typesKnockout.time, issue: 0, visible: ko.observable(false) });
    this.HoraColetaFinal = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HorarioDeColetaFinal.getRequiredFieldDescription(), getType: typesKnockout.time, issue: 0, visible: ko.observable(false) });

    this.SomenteComDataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.SomenteComDataPrevisaoEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.EscoltaArmada = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Fretes.TabelaFrete.UtilizarApenasQuandoPossuirEscoltaArmada, visible: ko.observable(true) });
    this.Reentrega = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Fretes.TabelaFrete.UtilizarApenasQuandoPossuirReentrega, visible: ko.observable(true) });
    this.Rastreado = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Fretes.TabelaFrete.UtilizarApenasQuandoForRastreado, visible: ko.observable(true) });
    this.GerenciamentoRisco = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Fretes.TabelaFrete.UtilizarApenasQuandoPossuirGerenciamentoDeRisco, visible: ko.observable(true) });
    this.DespachoTransitoAduaneiro = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Fretes.TabelaFrete.UtilizarApenasQuandoPossuirDespachoDeTransitoAduaneiro, visible: ko.observable(true) });
    this.RestricaoTrafego = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Fretes.TabelaFrete.UtilizarApenasQuandoPossuirRestricaoDeTrafego, visible: ko.observable(true) });
    this.MultiplicarPorAjudante = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Fretes.TabelaFrete.OValorCalculadoDeveSerMultiplicadoPelaQuantidadeDeAjudantes, visible: ko.observable(true) });
    this.MultiplicarPorDeslocamento = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.OValorCalculadoDeveSerMultiplicadoPeloDeslocamento, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.MultiplicarPorDiaria = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.OValorCalculadoDeveSerMultiplicadoPelaQuantidadeDeDiarias, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.MultiplicarPorEntrega = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.OValorDeveSerMultiplicadoPelaQuantidadeDeEntregas, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TipoViagem = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarApenasParaViagem.getFieldDescription(), options: EnumTipoViagemComponenteTabelaFrete.obterOpcoesPesquisa(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });

    this.MultiplicarPorHoraTempo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: Localization.Resources.Fretes.TabelaFrete.MultiplicarOValorPorHora, visible: ko.observable(false) });

    this.PossuiHorasMinimasCobrancaTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.PossuiHorasMinimasParaCobranca, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.HorasMinimasCobrancaTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HorasMinimasParaCobranca.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(true) });

    this.UtilizarArredondamentoHorasTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.UtilizarArredondamentoDeMinutos, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.MinutosArredondamentoHorasTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.MinutosUtilizadosParaArredondamento.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 2, visible: ko.observable(true) });

    this.GridTempo = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.CodigoTempo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HoraInicialTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraInicial.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(false) });
    this.HoraFinalTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraFinal.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(false) });
    this.ValorTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.Valor.getRequiredFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.decimal, visible: ko.observable(false) });

    this.PeriodoInicialTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.EoPeriodoInicial, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.HoraInicialCobrancaMinimaTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraInicialCobrancaMinima.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(false) });
    this.HoraFinalCobrancaMinimaTempo = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.HoraFinalCobrancaMinima.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time, visible: ko.observable(false) });

    this.AdicionarTempo = PropertyEntity({ eventClick: AdicionarTempoComponenteFreteClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarComponenteFreteClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarComponenteFreteClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirComponenteFreteClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: limparCamposComponenteFrete, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Cancelar, visible: ko.observable(false) });

    this.Tempos = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });

    this.TipoCalculoPeso.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoCalculoPesoTabelaFreteComponenteFrete.PorValorFixoComExcedente)
            _componenteFrete.ValorExcedentePorKG.visible(true);
        else
            _componenteFrete.ValorExcedentePorKG.visible(false);
    });


    this.TipoCalculoVolume.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoCalculoVolumeTabelaFreteComponenteFrete.PorValorFixoComExcedente)
            _componenteFrete.ValorExcedentePorVolume.visible(true);
        else
            _componenteFrete.ValorExcedentePorVolume.visible(false);
    });



    this.Tipo.val.subscribe(function (novoValor) {
        if (novoValor === EnumTipoComponenteTabelaFrete.ValorCalculado) {
            $("#divTabComponentesCalculados").show();
        } else {
            _componenteFrete.TipoCalculo.val(EnumTipoCalculoComponenteTabelaFrete.Nenhum);
            _componenteFrete.ValorInformadoNaTabela.val(true);
            _componenteFrete.ValidarValorMercadoria.val(false);
            _componenteFrete.ValidarPesoMercadoria.val(false);
            _componenteFrete.ValidarDimensoesMercadoria.val(false);
            _componenteFrete.UtilizarDiasEspecificos.val(false);
            _componenteFrete.UtilizarPeriodoColeta.val(false);
            _componenteFrete.SomenteComDataPrevisaoEntrega.val(false);
            _componenteFrete.EscoltaArmada.val(false);
            _componenteFrete.Reentrega.val(false);
            _componenteFrete.DespachoTransitoAduaneiro.val(false);
            _componenteFrete.RestricaoTrafego.val(false);
            _componenteFrete.Rastreado.val(false);
            _componenteFrete.GerenciamentoRisco.val(false);
            _componenteFrete.MultiplicarPorHoraTempo.val(false);
            _componenteFrete.PossuiHorasMinimasCobrancaTempo.val(false);
            _componenteFrete.UtilizarArredondamentoHorasTempo.val(false);
            _componenteFrete.MultiplicarPorAjudante.val(false);

            $("#divTabComponentesCalculados").hide();
        }
    });

    this.TipoCalculo.val.subscribe(function () {
        ControlarCamposCalculoComponenteFrete();
    });

    this.ValorInformadoNaTabela.val.subscribe(function () {
        ControlarCamposCalculoComponenteFrete();
    });

    this.ValorUnicoParaCarga.val.subscribe(function () {
        ControlarCamposCalculoComponenteFrete();
    });

    this.PossuiHorasMinimasCobrancaTempo.val.subscribe(function () {
        ControlarCamposCalculoComponenteFrete();
    });

    this.TipoDocumentoQuantidadeDocumentos.val.subscribe(function (novoValor) {
        if (_componenteFrete.TipoCalculo.val() == EnumTipoCalculoComponenteTabelaFrete.QuantidadeDocumentos && novoValor == EnumTipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido)
            _componenteFrete.ModeloDocumentoFiscalRestringirQuantidade.visible(true);
        else {
            _componenteFrete.ModeloDocumentoFiscalRestringirQuantidade.visible(false);
            _componenteFrete.ModeloDocumentoFiscalRestringirQuantidade.val("");
            _componenteFrete.ModeloDocumentoFiscalRestringirQuantidade.codEntity(0);
        }
    });

    this.TipoPercentual.val.subscribe(function (novoValor) {
        if (_componenteFrete.TipoCalculo.val() == EnumTipoCalculoComponenteTabelaFrete.Percentual && novoValor == EnumTipoPercentualComponenteTabelaFrete.SobreValorFrete || novoValor == EnumTipoPercentualComponenteTabelaFrete.SobreValorFreteEComponentes)
            _componenteFrete.IncluirBaseCalculo.visible(true);
        else {
            _componenteFrete.IncluirBaseCalculo.visible(false);
            _componenteFrete.IncluirBaseCalculo.val(false);
        }
    });
}

function ControlarCamposCalculoComponenteFrete() {
    _componenteFrete.ValorFormula.visible(false);
    _componenteFrete.Percentual.visible(false);
    _componenteFrete.UtilizarFormulaRateioCarga.visible(false);
    _componenteFrete.TipoPercentual.visible(false);
    _componenteFrete.IncluirBaseCalculo.visible(false);
    _componenteFrete.Peso.visible(false);
    _componenteFrete.TipoCalculoPeso.visible(false);
    _componenteFrete.TipoCalculoCubagem.visible(false);
    _componenteFrete.Cubagem.visible(false);
    _componenteFrete.ValorExcedentePorKG.visible(false);
    _componenteFrete.TipoDocumentoQuantidadeDocumentos.visible(false);
    _componenteFrete.ModeloDocumentoFiscalRestringirQuantidade.visible(false);
    _componenteFrete.UtilizarArredondamentoHorasTempo.visible(false);
    _componenteFrete.MultiplicarPorHoraTempo.visible(false);
    _componenteFrete.PossuiHorasMinimasCobrancaTempo.visible(false);
    _componenteFrete.HoraInicialTempo.visible(false);
    _componenteFrete.HoraFinalTempo.visible(false);
    _componenteFrete.PeriodoInicialTempo.visible(false);
    _componenteFrete.HoraInicialCobrancaMinimaTempo.visible(false);
    _componenteFrete.HoraFinalCobrancaMinimaTempo.visible(false);
    _componenteFrete.AdicionarTempo.visible(false);
    _componenteFrete.GridTempo.visible(false);
    _componenteFrete.ValorTempo.visible(false);
    _componenteFrete.Volume.visible(false);
    _componenteFrete.ValorExcedentePorVolume.visible(false);
    _componenteFrete.TipoCalculoVolume.visible(false);

    if (_componenteFrete.ValorUnicoParaCarga.val()) {
        _componenteFrete.ValorInformadoNaTabela.val(false);
        _componenteFrete.ValorInformadoNaTabela.enable(false);
    } else {
        _componenteFrete.ValorInformadoNaTabela.enable(true);
    }

    var valorInformadoNaTabela = _componenteFrete.ValorInformadoNaTabela.val();

    if (_componenteFrete.TipoCalculo.val() != EnumTipoCalculoComponenteTabelaFrete.Percentual) {
        _componenteFrete.UtilizarFormulaRateioCarga.val(false);
    }

    switch (_componenteFrete.TipoCalculo.val()) {
        case EnumTipoCalculoComponenteTabelaFrete.Percentual:
            _componenteFrete.TipoPercentual.visible(true);
            _componenteFrete.UtilizarFormulaRateioCarga.visible(true);

            if (!valorInformadoNaTabela)
                _componenteFrete.Percentual.visible(true);

            if (_componenteFrete.TipoPercentual.val() == EnumTipoPercentualComponenteTabelaFrete.SobreValorFrete || _componenteFrete.TipoPercentual.val() == EnumTipoPercentualComponenteTabelaFrete.SobreValorFreteEComponentes)
                _componenteFrete.IncluirBaseCalculo.visible(true);
            else
                _componenteFrete.IncluirBaseCalculo.visible(false);

            break;
        case EnumTipoCalculoComponenteTabelaFrete.Peso:
            _componenteFrete.Peso.visible(true);
            _componenteFrete.TipoCalculoPeso.visible(true);

            if (!valorInformadoNaTabela)
                _componenteFrete.ValorFormula.visible(true);

            break;
        case EnumTipoCalculoComponenteTabelaFrete.Volume:
            _componenteFrete.Volume.visible(true);
            _componenteFrete.TipoCalculoVolume.visible(true);

            if (!valorInformadoNaTabela)
                _componenteFrete.ValorFormula.visible(true);

            break;
        case EnumTipoCalculoComponenteTabelaFrete.ValorFixo:
            _componenteFrete.ValorFormula.visible(true);

            break;
        case EnumTipoCalculoComponenteTabelaFrete.QuantidadeDocumentos:
            _componenteFrete.TipoDocumentoQuantidadeDocumentos.visible(true);

            if (!valorInformadoNaTabela)
                _componenteFrete.ValorFormula.visible(true);

            if (_componenteFrete.TipoDocumentoQuantidadeDocumentos.val() == EnumTipoDocumentoQuantidadeDocumentosTabelaFreteComponenteFrete.PorDocumentoEmitido)
                _componenteFrete.ModeloDocumentoFiscalRestringirQuantidade.visible(true);
            else
                _componenteFrete.ModeloDocumentoFiscalRestringirQuantidade.visible(false);

            break;
        case EnumTipoCalculoComponenteTabelaFrete.Tempo:
            _componenteFrete.UtilizarArredondamentoHorasTempo.visible(true);
            _componenteFrete.MultiplicarPorHoraTempo.visible(true);
            _componenteFrete.PossuiHorasMinimasCobrancaTempo.visible(true);
            _componenteFrete.ValorTempo.visible(true);

            _componenteFrete.HoraInicialTempo.visible(true);
            _componenteFrete.HoraFinalTempo.visible(true);
            _componenteFrete.PeriodoInicialTempo.visible(true);
            _componenteFrete.AdicionarTempo.visible(true);
            _componenteFrete.GridTempo.visible(true);

            if (_componenteFrete.PossuiHorasMinimasCobrancaTempo.val()) {
                _componenteFrete.HoraInicialCobrancaMinimaTempo.visible(true);
                _componenteFrete.HoraFinalCobrancaMinimaTempo.visible(true);
            }

            break;
        case EnumTipoCalculoComponenteTabelaFrete.Eixo:
            if (!valorInformadoNaTabela)
                _componenteFrete.ValorFormula.visible(true);

            break;
        case EnumTipoCalculoComponenteTabelaFrete.Cubagem:
            _componenteFrete.TipoCalculoCubagem.visible(true);
            _componenteFrete.Cubagem.visible(true);

            if (!valorInformadoNaTabela)
                _componenteFrete.ValorFormula.visible(true);

            break;
    }
}

//*******EVENTOS*******

function loadComponenteFrete() {

    _componenteFrete = new ComponenteFrete();
    KoBindings(_componenteFrete, "knockoutComponente");

    new BuscarComponentesDeFrete(_componenteFrete.Componente, RetornoSelecaoComponente);
    new BuscarModeloDocumentoFiscal(_componenteFrete.ModeloDocumentoFiscalRestringirQuantidade, null, null, false, true, true);
    new BuscarJustificativas(_componenteFrete.Justificativa, null, EnumTipoJustificativa.Acrescimo, [EnumTipoFinalidadeJustificativa.ContratoFrete]);

    _componenteFrete.Componente.codEntity.subscribe(ComponenteChange);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFrete.Editar, id: guid(), metodo: editarComponenteFreteClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoComponente", visible: false },
        { data: "DescricaoComponente", title: Localization.Resources.Fretes.TabelaFrete.Componente, width: "40%" },
        { data: "Tipo", title: Localization.Resources.Fretes.TabelaFrete.Tipo, width: "20%" },
        { data: "IncluirBaseCalculoICMS", title: Localization.Resources.Fretes.TabelaFrete.IncluirNaBaseDeCalculoICMS, width: "20%" }
    ];

    _gridComponenteFrete = new BasicDataTable(_componenteFrete.Grid.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    var menuOpcoesGridTempo = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirTempoComponenteFreteClick }] };

    var headerGridTempo = [
        { data: "Codigo", visible: false },
        { data: "Periodo", title: Localization.Resources.Fretes.TabelaFrete.Periodo, width: "30%" },
        { data: "PeriodoCobrancaMinima", title: Localization.Resources.Fretes.TabelaFrete.PeriodoCobrancaMinima, width: "30%" },
        { data: "Valor", title: Localization.Resources.Fretes.TabelaFrete.Valor, width: "20%" }
    ];

    _gridTempoComponenteFrete = new BasicDataTable(_componenteFrete.GridTempo.id, headerGridTempo, menuOpcoesGridTempo, { column: 1, dir: orderDir.asc });

    recarregarGridComponenteFrete();
    RecarregarGridTempoComponenteFrete();

}

function RecarregarGridTempoComponenteFrete() {

    var data = new Array();

    $.each(_componenteFrete.Tempos.list, function (i, tempo) {
        var tempoGrid = new Object();

        tempoGrid.Codigo = tempo.Codigo.val;
        tempoGrid.Periodo = Localization.Resources.Fretes.TabelaFrete.PeriodoDasAsFormatado.format(tempo.HoraInicialTempo.val, tempo.HoraFinalTempo.val);
        tempoGrid.Valor = tempo.ValorTempo.val;

        if (_componenteFrete.PossuiHorasMinimasCobrancaTempo.val() === true && tempo.HoraInicialCobrancaMinimaTempo.val != "" && tempo.HoraFinalCobrancaMinimaTempo.val != "")
            tempoGrid.PeriodoCobrancaMinima = Localization.Resources.Fretes.TabelaFrete.PeriodoDasAsFormatado.format(tempo.HoraInicialCobrancaMinimaTempo.val, tempo.HoraFinalCobrancaMinimaTempo.val);
        else
            tempoGrid.PeriodoCobrancaMinima = "";

        data.push(tempoGrid);
    });

    _gridTempoComponenteFrete.CarregarGrid(data);
}

function ExcluirTempoComponenteFreteClick(data) {
    for (var i = 0; i < _componenteFrete.Tempos.list.length; i++) {
        if (data.Codigo == _componenteFrete.Tempos.list[i].Codigo.val) {
            _componenteFrete.Tempos.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridTempoComponenteFrete();
}

function AdicionarTempoComponenteFreteClick(e, sender) {

    var horaInicio = moment(_componenteFrete.HoraInicialTempo.val(), "HH:mm");
    var horaTermino = moment(_componenteFrete.HoraFinalTempo.val(), "HH:mm");

    for (var i = 0; i < _componenteFrete.Tempos.list.length; i++) {
        var horaInicioGrid = moment(_componenteFrete.Tempos.list[i].HoraInicialTempo.val, "HH:mm");
        var horaTerminoGrid = moment(_componenteFrete.Tempos.list[i].HoraFinalTempo.val, "HH:mm");

        if (horaInicio.isBetween(horaInicioGrid, horaTerminoGrid) ||
            horaTermino.isBetween(horaInicioGrid, horaTerminoGrid) ||
            horaInicio.diff(horaInicioGrid) == 0 ||
            horaInicio.diff(horaTerminoGrid) == 0 ||
            horaTermino.diff(horaTerminoGrid) == 0 ||
            horaTermino.diff(horaInicioGrid) == 0) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.PeriodoJaExiste, Localization.Resources.Fretes.TabelaFrete.PeriodoEntrouEmConflitoComUmPeriodoDasAs.format(_componenteFrete.Tempos.list[i].HoraInicialTempo, _componenteFrete.Tempos.list[i].HoraFinalTempo));
            return;
        }
    }

    var faixaTempo = new ComponenteFreteFaixaTempo();
    faixaTempo.Codigo.val(guid());
    faixaTempo.HoraInicialTempo.val(_componenteFrete.HoraInicialTempo.val());
    faixaTempo.HoraFinalTempo.val(_componenteFrete.HoraFinalTempo.val());
    faixaTempo.HoraInicialCobrancaMinimaTempo.val(_componenteFrete.HoraInicialCobrancaMinimaTempo.val());
    faixaTempo.HoraFinalCobrancaMinimaTempo.val(_componenteFrete.HoraFinalCobrancaMinimaTempo.val());
    faixaTempo.PeriodoInicialTempo.val(_componenteFrete.PeriodoInicialTempo.val());
    faixaTempo.ValorTempo.val(_componenteFrete.ValorTempo.val());

    _componenteFrete.Tempos.list.push(SalvarListEntity(faixaTempo));

    RecarregarGridTempoComponenteFrete();

    LimparCamposTempoComponenteFrete();
}

function LimparCamposTempoComponenteFrete() {
    _componenteFrete.HoraFinalCobrancaMinimaTempo.val("");
    _componenteFrete.HoraInicialCobrancaMinimaTempo.val("");
    _componenteFrete.HoraInicialTempo.val("");
    _componenteFrete.HoraFinalTempo.val("");
    _componenteFrete.ValorTempo.val("");
    _componenteFrete.PeriodoInicialTempo.val(false);
}

function recarregarGridComponenteFrete() {

    var data = new Array();

    $.each(_tabelaFrete.ComponentesFrete.list, function (i, componenteFrete) {
        var componenteFreteGrid = new Object();

        componenteFreteGrid.Codigo = componenteFrete.Codigo.val;
        componenteFreteGrid.CodigoComponente = componenteFrete.Componente.codEntity;
        componenteFreteGrid.DescricaoComponente = componenteFrete.Componente.val;
        componenteFreteGrid.IncluirBaseCalculoICMS = componenteFrete.IncluirBaseCalculoICMS.val ? Localization.Resources.Fretes.TabelaFrete.Sim : Localization.Resources.Fretes.TabelaFrete.Nao;
        componenteFreteGrid.Tipo = componenteFrete.Tipo.val == EnumTipoComponenteTabelaFrete.ValorFixo ? Localization.Resources.Fretes.TabelaFrete.ValorFixo : Localization.Resources.Fretes.TabelaFrete.ValorCalculado;

        data.push(componenteFreteGrid);
    });

    _gridComponenteFrete.CarregarGrid(data);
}

function editarComponenteFreteClick(data) {
    limparCamposComponenteFrete();

    for (var i = 0; i < _tabelaFrete.ComponentesFrete.list.length; i++) {
        if (_tabelaFrete.ComponentesFrete.list[i].Codigo.val == data.Codigo) {
            var componente = _tabelaFrete.ComponentesFrete.list[i];
            var isComponentePedagio = componente.TipoComponenteFrete.val == EnumTipoComponenteFrete.PEDAGIO;

            _componenteFrete.Codigo.val(componente.Codigo.val);
            _componenteFrete.Componente.val(componente.Componente.val);
            _componenteFrete.Componente.codEntity(componente.Componente.codEntity);
            _componenteFrete.Tipo.val(componente.Tipo.val);
            _componenteFrete.TipoCalculo.val(componente.TipoCalculo.val);
            _componenteFrete.UtilizarFormulaRateioCarga.val(componente.UtilizarFormulaRateioCarga.val);
            _componenteFrete.ComponenteComparado.val(componente.ComponenteComparado.val);
            _componenteFrete.IncluirBaseCalculoICMS.val(componente.IncluirBaseCalculoICMS.val);
            _componenteFrete.IgnorarComponenteQuandoVeiculoPossuiTagValePedagio.val(componente.IgnorarComponenteQuandoVeiculoPossuiTagValePedagio.val);
            _componenteFrete.IgnorarComponenteQuandoVeiculoPossuiTagValePedagio.visible(isComponentePedagio);
            _componenteFrete.TipoComponenteFrete.val(componente.TipoComponenteFrete.val);
            //_componenteFrete.UtilizarPercentual.val(componente.UtilizarPercentual.val);
            _componenteFrete.Percentual.val(componente.Percentual.val);
            _componenteFrete.TipoPercentual.val(componente.TipoPercentual.val);
            _componenteFrete.IncluirBaseCalculo.val(componente.IncluirBaseCalculo.val);
            //_componenteFrete.UtilizarPeso.val(componente.UtilizarPeso.val);
            _componenteFrete.TipoCalculoPeso.val(componente.TipoCalculoPeso.val);
            _componenteFrete.Peso.val(componente.Peso.val);
            _componenteFrete.TipoCalculoCubagem.val(componente.TipoCalculoCubagem.val);
            _componenteFrete.Cubagem.val(componente.Cubagem.val);
            _componenteFrete.ValorExcedentePorKG.val(componente.ValorExcedentePorKG.val);
            _componenteFrete.ValorInformadoNaTabela.val(componente.ValorInformadoNaTabela.val);
            _componenteFrete.UtilizarCalculoDesseComponenteNaOcorrencia.val(componente.UtilizarCalculoDesseComponenteNaOcorrencia.val);
            _componenteFrete.ValorUnicoParaCarga.val(componente.ValorUnicoParaCarga.val);
            _componenteFrete.ValorFormula.val(componente.ValorFormula.val);
            _componenteFrete.ValorMinimo.val(componente.ValorMinimo.val);
            _componenteFrete.EntregaMinima.val(componente.EntregaMinima.val);
            _componenteFrete.ValorMaximo.val(componente.ValorMaximo.val);
            _componenteFrete.ValidarValorMercadoria.val(componente.ValidarValorMercadoria.val);
            _componenteFrete.ValorMercadoriaMinimo.val(componente.ValorMercadoriaMinimo.val);
            _componenteFrete.ValorMercadoriaMaximo.val(componente.ValorMercadoriaMaximo.val);
            _componenteFrete.ValidarPesoMercadoria.val(componente.ValidarPesoMercadoria.val);
            _componenteFrete.PesoMercadoriaMinimo.val(componente.PesoMercadoriaMinimo.val);
            _componenteFrete.PesoMercadoriaMaximo.val(componente.PesoMercadoriaMaximo.val);
            _componenteFrete.ValidarDimensoesMercadoria.val(componente.ValidarDimensoesMercadoria.val);
            _componenteFrete.AlturaMercadoriaMinima.val(componente.AlturaMercadoriaMinima.val);
            _componenteFrete.AlturaMercadoriaMaxima.val(componente.AlturaMercadoriaMaxima.val);
            _componenteFrete.LarguraMercadoriaMinima.val(componente.LarguraMercadoriaMinima.val);
            _componenteFrete.LarguraMercadoriaMaxima.val(componente.LarguraMercadoriaMaxima.val);
            _componenteFrete.ComprimentoMercadoriaMinimo.val(componente.ComprimentoMercadoriaMinimo.val);
            _componenteFrete.ComprimentoMercadoriaMaximo.val(componente.ComprimentoMercadoriaMaximo.val);
            _componenteFrete.VolumeMercadoriaMinimo.val(componente.VolumeMercadoriaMinimo.val);
            _componenteFrete.VolumeMercadoriaMaximo.val(componente.VolumeMercadoriaMaximo.val);
            _componenteFrete.UtilizarDiasEspecificos.val(componente.UtilizarDiasEspecificos.val);
            _componenteFrete.SegundaFeira.val(componente.SegundaFeira.val);
            _componenteFrete.TercaFeira.val(componente.TercaFeira.val);
            _componenteFrete.QuartaFeira.val(componente.QuartaFeira.val);
            _componenteFrete.QuintaFeira.val(componente.QuintaFeira.val);
            _componenteFrete.SextaFeira.val(componente.SextaFeira.val);
            _componenteFrete.Sabado.val(componente.Sabado.val);
            _componenteFrete.Domingo.val(componente.Domingo.val);
            _componenteFrete.Feriados.val(componente.Feriados.val);
            _componenteFrete.UtilizarPeriodoColeta.val(componente.UtilizarPeriodoColeta.val);
            _componenteFrete.HoraColetaInicial.val(componente.HoraColetaInicial.val);
            _componenteFrete.HoraColetaFinal.val(componente.HoraColetaFinal.val);
            _componenteFrete.SomenteComDataPrevisaoEntrega.val(componente.SomenteComDataPrevisaoEntrega.val);
            _componenteFrete.EscoltaArmada.val(componente.EscoltaArmada.val);
            _componenteFrete.Reentrega.val(componente.Reentrega.val);
            _componenteFrete.Rastreado.val(componente.Rastreado.val);
            _componenteFrete.DespachoTransitoAduaneiro.val(componente.DespachoTransitoAduaneiro.val);
            _componenteFrete.RestricaoTrafego.val(componente.RestricaoTrafego.val);
            _componenteFrete.GerenciamentoRisco.val(componente.GerenciamentoRisco.val);
            // _componenteFrete.UtilizarQuantidadeDocumentos.val(componente.UtilizarQuantidadeDocumentos.val);
            _componenteFrete.TipoDocumentoQuantidadeDocumentos.val(componente.TipoDocumentoQuantidadeDocumentos.val);
            _componenteFrete.ModeloDocumentoFiscalRestringirQuantidade.val(componente.ModeloDocumentoFiscalRestringirQuantidade.val);
            _componenteFrete.ModeloDocumentoFiscalRestringirQuantidade.codEntity(componente.ModeloDocumentoFiscalRestringirQuantidade.codEntity);
            _componenteFrete.MultiplicarPorHoraTempo.val(componente.MultiplicarPorHoraTempo.val);
            _componenteFrete.MultiplicarPorAjudante.val(componente.MultiplicarPorAjudante.val);
            _componenteFrete.MultiplicarPorDeslocamento.val(componente.MultiplicarPorDeslocamento.val);
            _componenteFrete.MultiplicarPorDiaria.val(componente.MultiplicarPorDiaria.val);
            _componenteFrete.MultiplicarPorEntrega.val(componente.MultiplicarPorEntrega.val);
            _componenteFrete.PossuiHorasMinimasCobrancaTempo.val(componente.PossuiHorasMinimasCobrancaTempo.val);
            _componenteFrete.HorasMinimasCobrancaTempo.val(componente.HorasMinimasCobrancaTempo.val);
            _componenteFrete.UtilizarArredondamentoHorasTempo.val(componente.UtilizarArredondamentoHorasTempo.val);
            _componenteFrete.MinutosArredondamentoHorasTempo.val(componente.MinutosArredondamentoHorasTempo.val);
            _componenteFrete.Tempos.list = componente.Tempos.list.slice(0);
            _componenteFrete.TipoViagem.val(componente.TipoViagem == null || componente.TipoViagem.val == null ? "" : componente.TipoViagem.val.toString());
            _componenteFrete.Justificativa.val(componente.Justificativa.val);
            _componenteFrete.Justificativa.codEntity(componente.Justificativa.codEntity);

            _componenteFrete.Volume.val(componente.Volume.val);
            _componenteFrete.ValorExcedentePorVolume.val(componente.ValorExcedentePorVolume.val);
            _componenteFrete.TipoCalculoVolume.val(componente.TipoCalculoVolume.val);

            RecarregarGridTempoComponenteFrete();

            _componenteFrete.Adicionar.visible(false);
            _componenteFrete.Atualizar.visible(true);
            _componenteFrete.Excluir.visible(true);
            _componenteFrete.Cancelar.visible(true);

            break;
        }
    }
}

function adicionarComponenteFreteClick(e, sender) {

    var valido = ValidarCamposObrigatorios(_componenteFrete);

    if (valido) {

        if (_componenteFrete.Tipo.val() == EnumTipoComponenteTabelaFrete.ValorFixo) {
            var existe = false;

            $.each(_tabelaFrete.ComponentesFrete.list, function (i, componenteFrete) {
                if (componenteFrete.Componente.codEntity == _componenteFrete.Componente.codEntity()) {
                    existe = true;
                    return;
                }
            });

            if (existe) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.ComponenteExistente, Localization.Resources.Fretes.TabelaFrete.OComponenteXEstaCadastrado.format(_componenteFrete.Componente.val()));
                return;
            }
        }

        _componenteFrete.Codigo.val(guid());

        _tabelaFrete.ComponentesFrete.list.push(SalvarListEntity(_componenteFrete));

        recarregarGridComponenteFrete();

        $("#" + _componenteFrete.Componente.id).focus();

        limparCamposComponenteFrete();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function atualizarComponenteFreteClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_componenteFrete);

    if (valido) {

        if (_componenteFrete.Tipo.val() == EnumTipoComponenteTabelaFrete.ValorFixo) {
            var existe = false;

            $.each(_tabelaFrete.ComponentesFrete.list, function (i, componenteFrete) {
                if (componenteFrete.Componente.codEntity == _componenteFrete.Componente.codEntity() && componenteFrete.Codigo.val != _componenteFrete.Codigo.val()) {
                    existe = true;
                    return;
                }
            });

            if (existe) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.ComponenteExiste, Localization.Resources.Fretes.TabelaFrete.OComponenteXEstaCadastrado.format(_componenteFrete.Componente.val()));
                return;
            }
        }

        for (var i = 0; i < _tabelaFrete.ComponentesFrete.list.length; i++) {
            if (_tabelaFrete.ComponentesFrete.list[i].Codigo.val == _componenteFrete.Codigo.val()) {
                _tabelaFrete.ComponentesFrete.list[i] = SalvarListEntity(_componenteFrete);
                break;
            }
        }

        recarregarGridComponenteFrete();

        $("#" + _componenteFrete.Componente.id).focus();

        limparCamposComponenteFrete();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function excluirComponenteFreteClick(e, sender) {
    for (var i = 0; i < _tabelaFrete.ComponentesFrete.list.length; i++) {
        if (_tabelaFrete.ComponentesFrete.list[i].Codigo.val == _componenteFrete.Codigo.val()) {
            _tabelaFrete.ComponentesFrete.list.splice(i, 1);
            break;
        }
    }

    recarregarGridComponenteFrete();

    $("#" + _componenteFrete.Componente.id).focus();

    limparCamposComponenteFrete();
}

function limparCamposComponenteFrete() {
    LimparCampos(_componenteFrete);

    _componenteFrete.Tempos.list = new Array();

    _componenteFrete.Adicionar.visible(true);
    _componenteFrete.Atualizar.visible(false);
    _componenteFrete.Excluir.visible(false);
    _componenteFrete.Cancelar.visible(false);

    RecarregarGridTempoComponenteFrete();
}


/***** MÉTODOS *****/

function RetornoSelecaoComponente(data) {
    _componenteFrete.Componente.val(data.Descricao);
    _componenteFrete.Componente.codEntity(data.Codigo);
    _componenteFrete.TipoComponenteFrete.val(data.TipoComponenteFrete);

    var isComponentePedagio = data.TipoComponenteFrete == EnumTipoComponenteFrete.PEDAGIO;
    _componenteFrete.IgnorarComponenteQuandoVeiculoPossuiTagValePedagio.visible(isComponentePedagio);
}

function ComponenteChange(codigoComponente) {
    if (!codigoComponente) {
        _componenteFrete.IgnorarComponenteQuandoVeiculoPossuiTagValePedagio.visible(false);
        _componenteFrete.TipoComponenteFrete.val(0);
    }
}