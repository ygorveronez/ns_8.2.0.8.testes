/// <reference path="../../Enumeradores/EnumDiaSemana.js" />
/// <reference path="../../Enumeradores/EnumTipoEnvioFatura.js" />
/// <reference path="../../Enumeradores/EnumTipoAgrupamentoFatura.js" />
/// <reference path="../../Enumeradores/EnumTipoAgrupamentoEnvioDocumentacao.js" />
/// <reference path="../../Enumeradores/EnumTipoPrazoFaturamento.js" />
/// <reference path="../../Enumeradores/EnumFormaGeracaoTituloFatura.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" /
/// <reference path="../../Enumeradores/EnumTipoConta.js" />
/// <reference path="../../Consultas/BoletoConfiguracao.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracaoCanhoto.js" />

var _diaMes = [
    //{ text: "Sem Configuração", value: 0 },
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 },
    { text: "5", value: 5 },
    { text: "6", value: 6 },
    { text: "7", value: 7 },
    { text: "8", value: 8 },
    { text: "9", value: 9 },
    { text: "10", value: 10 },
    { text: "11", value: 11 },
    { text: "12", value: 12 },
    { text: "13", value: 13 },
    { text: "14", value: 14 },
    { text: "15", value: 15 },
    { text: "16", value: 16 },
    { text: "17", value: 17 },
    { text: "18", value: 18 },
    { text: "19", value: 19 },
    { text: "20", value: 20 },
    { text: "21", value: 21 },
    { text: "22", value: 22 },
    { text: "23", value: 23 },
    { text: "24", value: 24 },
    { text: "25", value: 25 },
    { text: "26", value: 26 },
    { text: "27", value: 27 },
    { text: "28", value: 28 },
    { text: "29", value: 29 },
    { text: "30", value: 30 },
    { text: "31", value: 31 }
];

var contasBancarias;
var ConfiguracaoFaturaModel = function (instancia) {
    var multiTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS;
    var multiEmbarcador = _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador;
    var tipoPrazoFaturamentoPadrao = multiEmbarcador ? EnumTipoPrazoFaturamento.Todos : EnumTipoPrazoFaturamento.DataFatura;

    //Obsoletos
    this.DiaSemana = PropertyEntity({ val: ko.observable(EnumDiaSemana.Todos), options: EnumDiaSemana.obterOpcoesSemConfiguracao(), def: EnumDiaSemana.Todos, text: Localization.Resources.Configuracao.Fatura.DiasDaSemana, required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.DiaMes = PropertyEntity({ val: ko.observable(0), options: _diaMes, def: 0, text: Localization.Resources.Configuracao.Fatura.DiasDoMes, required: false, enable: ko.observable(true), visible: ko.observable(false) });

    this.PermiteFinalSemana = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.PermiteFaturarNoFinalDeSemana, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.ExigeCanhotoFisico = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: multiTMS ? Localization.Resources.Configuracao.Fatura.ExigeEnvioDoCanhoto : Localization.Resources.Configuracao.Fatura.ExigeEnvioDoCanhotoFisicoParaLiberarPagamento, def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ArmazenaCanhotoFisicoCTe = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.ArmazenaCanhotoFisicoDoCTeEmitido, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.NaoGerarFaturaAteReceberCanhotos = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.NaoGerarFaturaAteReceberTodosOsCanhotos, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.SomenteOcorrenciasFinalizadoras = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.ValidarParaGerarPagamentoAoAgregadoSomenteComOcorrenciasFinalizadoras, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.FaturarSomenteOcorrenciasFinalizadoras = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.GerarFaturamentoSomenteComDocumentosQuePossuemOcorrenciasFinalizadoras, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });

    this.TipoPrazoFaturamento = PropertyEntity({ val: ko.observable(tipoPrazoFaturamentoPadrao), options: EnumTipoPrazoFaturamento.obterOpcoes(), def: EnumTipoPrazoFaturamento.DataFatura, text: Localization.Resources.Configuracao.Fatura.TipoPrazoFaturamento.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.FormaGeracaoTituloFatura = PropertyEntity({ val: ko.observable(EnumFormaGeracaoTituloFatura.Padrao), options: EnumFormaGeracaoTituloFatura.obterOpcoes(), def: EnumFormaGeracaoTituloFatura.Padrao, text: Localization.Resources.Configuracao.Fatura.FormaDaGeracaoDosTitulosNaFatura.getFieldDescription(), required: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.DiasDePrazoFatura = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.DiasDePrazoDoFaturamento.getFieldDescription()), required: false, visible: ko.observable(multiTMS || multiEmbarcador), maxlength: 4, getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(true) });

    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.Fatura.Banco.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.Agencia = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.Agencia.getFieldDescription()), required: false, visible: ko.observable(multiTMS), maxlength: 10, enable: ko.observable(true) });
    this.Digito = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.Digito.getFieldDescription()), required: false, visible: ko.observable(multiTMS), maxlength: 1, enable: ko.observable(true) });
    this.NumeroConta = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.NumeroConta.getFieldDescription()), required: false, visible: ko.observable(multiTMS), maxlength: 10, enable: ko.observable(true) });
    this.TipoConta = PropertyEntity({ val: ko.observable(EnumTipoConta.Corrente), options: EnumTipoConta.obterOpcoesBordero(), def: EnumTipoConta.Corrente, text: Localization.Resources.Configuracao.Fatura.Tipo.getFieldDescription(), required: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });

    this.TomadorFatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.Fatura.TomadorDaFatura.getFieldDescription(), idBtnSearch: guid(), required: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.FormaPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.Fatura.FormaDePagamento.getFieldDescription(), idBtnSearch: guid(), issue: 464, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.BoletoConfiguracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.Fatura.ConfiguracoesBoletoParaGeracaoAutomatica.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(multiTMS), enable: ko.observable(true) });

    this.AvisoVencimetoHabilitarConfiguracaoPersonalizada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.AvisoVencimetoHabilitarConfiguracaoPersonalizada.getFieldDescription(), def: false, visible: ko.observable(true) });
    this.AvisoVencimetoNaoEnviarEmail = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.NaoEnviarEmailCobranca.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.AvisoVencimetoQunatidadeDias = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.AvisoVencimetoQunatidadeDias.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true) });
    this.AvisoVencimetoEnviarDiariamente = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.AvisoVencimetoEnviarDiariamente.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CobrancaHabilitarConfiguracaoPersonalizada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.CobrancaHabilitarConfiguracaoPersonalizada.getFieldDescription(), def: false, visible: ko.observable(true) });
    this.CobrancaNaoEnviarEmail = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.NaoEnviarEmailCobranca.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.CobrancaQunatidadeDias = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.CobrancaQunatidadeDias.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true) });

    this.ObservacaoFatura = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.ObservacaoParaFatura.getFieldDescription()), required: false, visible: ko.observable(multiTMS), maxlength: 500, enable: ko.observable(true) });
    this.AssuntoEmailFatura = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.AssuntoEmailFatura.getFieldDescription(), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(true) });
    this.CorpoEmailFatura = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.CorpoEmailFatura.getFieldDescription(), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(true) });
    this.EmailFatura = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.EmailFatura.getFieldDescription(), getType: typesKnockout.multiplesEmails, maxlength: 5000, enable: ko.observable(true), visible: ko.observable(true) });

    this.GerarTituloPorDocumentoFiscal = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.GerarOsTitulosPorDocumentoFiscal, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.GerarFaturamentoAVista = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.GerarFaturamentoVistaPorCTe, def: false, visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal), enable: ko.observable(true) });
    this.GerarTituloAutomaticamente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.GerarTituloAutomaticamente, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.GerarFaturaAutomaticaCte = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.GerarFaturaAutomaticaCte, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.GerarBoletoAutomaticamente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.GerarBoletoAutomaticamente, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.EnviarBoletoPorEmailAutomaticamente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.EnviarBoletoPorEmailAutomaticamente, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.EnviarDocumentacaoFaturamentoCTe = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.EnviarDocumentacaoNoFaturamentoDoCTe, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.EnviarArquivosDescompactados = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.EnviarArquivosDescompactados, def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.NaoEnviarEmailFaturaAutomaticamente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.NaoEnviarPorEmailOsDadosDaFaturaDeFormaAutomatica, def: false, visible: ko.observable(false), enable: ko.observable(true) });
    this.GerarFaturaPorCte = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.GerarFaturasPorCTe, def: false, visible: ko.observable(multiEmbarcador), enable: ko.observable(true) });

    this.GerarFaturamentoMultiplaParcela = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.GerarMultiplasParcelasParaFaturamento, def: false, visible: ko.observable(multiTMS), enable: ko.observable(true) });
    this.QuantidadeParcelasFaturamento = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.IntervaloDeDiasExParaIntervalosDiferentes.getFieldDescription()), required: false, visible: ko.observable(false), maxlength: 500, enable: ko.observable(true) });
    this.GerarFaturamentoMultiplaParcela.val.subscribe(function (novoValor) {
        instancia.Configuracao.QuantidadeParcelasFaturamento.visible(novoValor);
        //instancia.Configuracao.HabilitarPeriodoVencimentoEspecifico.visible(novoValor);
    });
    this.HabilitarPeriodoVencimentoEspecifico = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.HabilitarPeriodoDeVencimentoEspecifico, def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.HabilitarPeriodoVencimentoEspecifico.val.subscribe(function (novoValor) {
        if (!novoValor)
            instancia.ResetarAbas();
    });
    this.FaturaVencimentos = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), enable: ko.observable(true), visible: ko.observable(true) });

    this.GerarTituloAutomaticamenteComAdiantamentoSaldo = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.GerarTituloAutomaticamenteComAdiantamentoSaldo, val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true) });
    this.GerarTituloAutomaticamenteComAdiantamentoSaldo.val.subscribe(function (novoValor) {
        if (!novoValor)
            instancia.ResetarAbas();
    });
    this.UtilizarCadastroContaBancaria = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.UtilizarCadastroContaBancaria), val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true) });
    this.EfetuarImpressaoDaTaxaDeMoedaEstrangeira = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.EfetuarImpressaoDaTaxaDeMoedaEstrangeira), val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true) });

    this.ContaBancaria = PropertyEntity({ type: types.event, text: ko.observable(Localization.Resources.Configuracao.Fatura.AdicionarContaBancaria), idBtnSearch: guid(), enable: ko.observable(true) });
    this.GridContaBancaria = PropertyEntity({ type: types.local });
    this.ContasBancarias = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.NaoValidarPossuiAcordoFaturamentoAvancoCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: ko.observable(Localization.Resources.Configuracao.Fatura.NaoValidarPossuiAcordoFaturamentoAvancoCarga), def: false, visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP), enable: ko.observable(true) });
    this.PercentualAdiantamentoTituloAutomatico = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.PercentualAdiantamentoTituloAutomatico.getFieldDescription()), maxlength: 6, getType: typesKnockout.decimal, val: ko.observable(""), enable: ko.observable(true) });
    this.PrazoAdiantamentoEmDiasTituloAutomatico = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.PrazoAdiantamentoEmDiasTituloAutomatico.getFieldDescription()), maxlength: 4, getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(true) });
    this.PercentualSaldoTituloAutomatico = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.PercentualSaldoTituloAutomatico.getFieldDescription()), maxlength: 6, getType: typesKnockout.decimal, val: ko.observable(""), enable: ko.observable(true) });
    this.PrazoSaldoEmDiasTituloAutomatico = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.PrazoSaldoEmDiasTituloAutomatico.getFieldDescription()), maxlength: 4, getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(true) });

    this.TagNumeroFaturaAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#NumeroFatura"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroDeFatura, enable: ko.observable(true) });
    this.TagObservacaoFaturaAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#ObservacaoFatura"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.ObservacaoFatura, enable: ko.observable(true) });
    this.TagDataFaturaAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#DataFatura"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.DataFatura, enable: ko.observable(true) });
    this.TagCNPJTomadorAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#CNPJTomador"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.CNPJDoTomador, enable: ko.observable(true) });
    this.TagNomeTomadorAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#NomeTomador"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NomeTomador, enable: ko.observable(true) });
    this.TagCNPJEmpresaAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#CNPJEmpresa"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.CNPJEmpresa, enable: ko.observable(true) });
    this.TagNomeEmpresaAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#NomeEmpresa"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NomeEmpresa, enable: ko.observable(true) });
    this.TagViagem = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#Viagem"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Viagem, enable: ko.observable(true) });
    this.TagPortoOrigemEmail = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#PortoOrigem"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.PortoOrigem, enable: ko.observable(true) });
    this.TagPortoDestinoEmail = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#PortoDestino"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.PortoDestino, enable: ko.observable(true) });
    this.TagNumeroCTe = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#NumeroCTe"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroCTe, enable: ko.observable(true) });
    this.TagDatasVencimentosBoletosAssuntoEmail = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoEmailFatura.id, "#DatasVencimentosBoletos"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.VencimentosBoletos, enable: ko.observable(true) });

    this.TagNumeroFatura = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailFatura.id, "#NumeroFatura"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroDeFatura, enable: ko.observable(true) });
    this.TagObservacaoFatura = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailFatura.id, "#ObservacaoFatura"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.ObservacaoFatura, enable: ko.observable(true) });
    this.TagDataFatura = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailFatura.id, "#DataFatura"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.DataFatura, enable: ko.observable(true) });
    this.TagCNPJTomador = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailFatura.id, "#CNPJTomador"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.CNPJDoTomador, enable: ko.observable(true) });
    this.TagNomeTomador = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailFatura.id, "#NomeTomador"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NomeTomador, enable: ko.observable(true) });
    this.TagCNPJEmpresa = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailFatura.id, "#CNPJEmpresa"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.CNPJEmpresa, enable: ko.observable(true) });
    this.TagNomeEmpresa = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailFatura.id, "#NomeEmpresa"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NomeEmpresa, enable: ko.observable(true) });
    this.TagTabela = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailFatura.id, "#Tabela"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Tabela, enable: ko.observable(true) });
    this.TagQuebraLinha = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailFatura.id, "#QuebraLinha"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.QuebraDeLinha, enable: ko.observable(true) });
    this.TagDatasVencimentosBoletosCorpoEmail = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailFatura.id, "#DatasVencimentosBoletos"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.VencimentosBoletos, enable: ko.observable(true) });
    this.TagValorFatura = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailFatura.id, "#ValorFatura"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.ValoraFatura, enable: ko.observable(true) });

    this.DiasSemanaFatura = PropertyEntity({ val: ko.observable([]), options: EnumDiaSemana.obterOpcoes(), def: [], getType: typesKnockout.selectMultiple, text: Localization.Resources.Configuracao.Fatura.DiaDaSemana.getFieldDescription(), required: false, visible: ko.observable(multiTMS), enable: ko.observable(true), title: Localization.Resources.Configuracao.Fatura.SemConfiguracao });
    this.DiasMesFatura = PropertyEntity({ val: ko.observable([]), options: _diaMes, def: [], getType: typesKnockout.selectMultiple, text: Localization.Resources.Configuracao.Fatura.DiaDoMes.getFieldDescription(), required: false, visible: ko.observable(multiTMS || multiEmbarcador), enable: ko.observable(true), title: Localization.Resources.Configuracao.Fatura.SemConfiguracao });
    this.TipoEnvioFatura = PropertyEntity({ val: ko.observable(EnumTipoEnvioFatura.Todos), options: EnumTipoEnvioFatura.obterOpcoes(), def: EnumTipoEnvioFatura.Todos, text: Localization.Resources.Configuracao.Fatura.TipoEnvioDaFatura.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.TipoAgrupamentoFatura = PropertyEntity({ val: ko.observable(EnumTipoAgrupamentoFatura.Todos), options: EnumTipoAgrupamentoFatura.obterOpcoes(), def: EnumTipoAgrupamentoFatura.Todos, text: Localization.Resources.Configuracao.Fatura.TipoAgrupamentoFatura.getFieldDescription(), required: false, visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal), enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ val: ko.observable(EnumFormaTitulo.Outros), options: EnumFormaTitulo.obterOpcoes(), def: EnumFormaTitulo.Outros, text: Localization.Resources.Configuracao.Fatura.FormaDoTitulo.getFieldDescription(), required: false, visible: ko.observable(true), enable: ko.observable(true) });

    this.InformarEmailEnvioDocumentacao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });
    this.EmailEnvioDocumentacao = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.EmailParaEnvioDaDocumentacao.getFieldDescription(), getType: typesKnockout.multiplesEmails, maxlength: 1000, visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal), enable: ko.observable(false) });
    this.TipoAgrupamentoEnvioDocumentacao = PropertyEntity({ val: ko.observable(EnumTipoAgrupamentoEnvioDocumentacao.Nenhum), options: EnumTipoAgrupamentoEnvioDocumentacao.obterOpcoes(), def: EnumTipoAgrupamentoEnvioDocumentacao.Nenhum, text: Localization.Resources.Configuracao.Fatura.TipoDeAgrupamentoParaEnvioDaDocumentacao.getFieldDescription(), required: false, visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal), enable: ko.observable(true) });
    this.FormaEnvioDocumentacao = PropertyEntity({ val: ko.observable(EnumFormaEnvioDocumentacao.PDFCTeNotas), options: EnumFormaEnvioDocumentacao.obterOpcoes(), def: EnumFormaEnvioDocumentacao.PDFCTeNotas, text: Localization.Resources.Configuracao.Fatura.FormaDeEnvio.getFieldDescription(), required: false, visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal), enable: ko.observable(true) });
    this.AssuntoDocumentacao = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.AssuntoEmailDocumentacao.getFieldDescription(), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.CorpoEmailDocumentacao = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.CorpoEmailDocumentacao.getFieldDescription(), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });

    this.InformarEmailEnvioDocumentacao.val.subscribe(function (novoValor) {
        instancia.Configuracao.EmailEnvioDocumentacao.enable(novoValor);
    });

    this.TagViagemDocumentacao = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacao.id, "#Viagem"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Viagem, enable: ko.observable(true) });
    this.TagRemetente = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacao.id, "#Remetente"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Remetente, enable: ko.observable(true) });
    this.TagDestinatario = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacao.id, "#Destinatario"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Destinatario, enable: ko.observable(true) });
    this.TagBooking = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacao.id, "#Booking"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Booking, enable: ko.observable(true) });
    this.TagPortoOrigem = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacao.id, "#PortoOrigem"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.PortoOrigem, enable: ko.observable(true) });
    this.TagPortoDestino = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacao.id, "#PortoDestino"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.PortoDestino, enable: ko.observable(true) });
    this.TagTomador = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacao.id, "#Tomador"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Tomador, enable: ko.observable(true) });
    this.TagNumeroNFCliente = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacao.id, "#NumeroNFCliente"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroNfCliente, enable: ko.observable(true) });
    this.TagNumeroFiscal = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacao.id, "#NumeroFiscal"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroFiscal, enable: ko.observable(true) });
    this.TagNumeroContainer = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacao.id, "#NumeroContainer"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroContainer, enable: ko.observable(true) });
    this.TagNumeroControleSVM = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacao.id, "#NumeroControleSVM"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroControleSVM, enable: ko.observable(true) });

    this.TagViagemCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#Viagem"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Viagem, enable: ko.observable(true) });
    this.TagPortoOrigemCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#PortoOrigem"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.PortoOrigem, enable: ko.observable(true) });
    this.TagPortoDestinoCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#PortoDestino"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.PortoDestino, enable: ko.observable(true) });
    this.TagBookingCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#Booking"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Booking, enable: ko.observable(true) });
    this.TagNumeroFiscalCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#NumeroFiscal"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroFiscal, enable: ko.observable(true) });
    this.TagNumeroNFClienteCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#NumeroNFCliente"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroNfCliente, enable: ko.observable(true) });
    this.TagNumeroControleClienteCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#NumeroControleCliente"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroControleCliente, enable: ko.observable(true) });
    this.TagRemetenteCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#Remetente"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Remetente, enable: ko.observable(true) });
    this.TagDestinatarioCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#Destinatario"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Destinatario, enable: ko.observable(true) });
    this.TagTomadorCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#Tomador"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Tomador, enable: ko.observable(true) });
    this.TagQuebraLinhaCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#QuebraLinha"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.QuebraDeLinha, enable: ko.observable(true) });
    this.TagNumeroContainerCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#NumeroContainer"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroContainer, enable: ko.observable(true) });
    this.TagNumeroControleSVMCorpo = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacao.id, "#NumeroControleSVM"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroControleSVM, enable: ko.observable(true) });

    this.InformarEmailEnvioDocumentacaoPorta = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(true) });
    this.EmailEnvioDocumentacaoPorta = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.EmailParaEnvioDaDocumentacao.getFieldDescription(), getType: typesKnockout.multiplesEmails, maxlength: 1000, visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal), enable: ko.observable(false) });
    this.TipoAgrupamentoEnvioDocumentacaoPorta = PropertyEntity({ val: ko.observable(EnumTipoAgrupamentoEnvioDocumentacao.Nenhum), options: EnumTipoAgrupamentoEnvioDocumentacao.obterOpcoes(), def: EnumTipoAgrupamentoEnvioDocumentacao.Nenhum, text: Localization.Resources.Configuracao.Fatura.TipoDeAgrupamentoParaEnvioDaDocumentacao.getFieldDescription(), required: false, visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal), enable: ko.observable(true) });
    this.FormaEnvioDocumentacaoPorta = PropertyEntity({ val: ko.observable(EnumFormaEnvioDocumentacao.PDFCTeNotas), options: EnumFormaEnvioDocumentacao.obterOpcoes(), def: EnumFormaEnvioDocumentacao.PDFCTeNotas, text: Localization.Resources.Configuracao.Fatura.FormaDeEnvio.getFieldDescription(), required: false, visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal), enable: ko.observable(true) });
    this.AssuntoDocumentacaoPorta = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.AssuntoEmailDocumentacao.getFieldDescription(), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.CorpoEmailDocumentacaoPorta = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.CorpoEmailDocumentacao.getFieldDescription(), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(multiTMS && _CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });

    this.InformarEmailEnvioDocumentacaoPorta.val.subscribe(function (novoValor) {
        instancia.Configuracao.EmailEnvioDocumentacaoPorta.enable(novoValor);
    });

    this.TagViagemDocumentacaoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacaoPorta.id, "#Viagem"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Viagem, enable: ko.observable(true) });
    this.TagRemetentePorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacaoPorta.id, "#Remetente"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Remetente, enable: ko.observable(true) });
    this.TagDestinatarioPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacaoPorta.id, "#Destinatario"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Destinatario, enable: ko.observable(true) });
    this.TagBookingPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacaoPorta.id, "#Booking"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Booking, enable: ko.observable(true) });
    this.TagPortoOrigemPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacaoPorta.id, "#PortoOrigem"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.PortoOrigem, enable: ko.observable(true) });
    this.TagPortoDestinoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacaoPorta.id, "#PortoDestino"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.PortoDestino, enable: ko.observable(true) });
    this.TagTomadorPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacaoPorta.id, "#Tomador"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Tomador, enable: ko.observable(true) });
    this.TagNumeroNFClientePorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacaoPorta.id, "#NumeroNFCliente"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroNfCliente, enable: ko.observable(true) });
    this.TagNumeroFiscalPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacaoPorta.id, "#NumeroFiscal"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroFiscal, enable: ko.observable(true) });
    this.TagNumeroContainerPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacaoPorta.id, "#NumeroContainer"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroContainer, enable: ko.observable(true) });
    this.TagNumeroControleSVMPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.AssuntoDocumentacaoPorta.id, "#NumeroControleSVM"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroControleSVM, enable: ko.observable(true) });

    this.TagViagemCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#Viagem"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Viagem, enable: ko.observable(true) });
    this.TagPortoOrigemCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#PortoOrigem"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.PortoOrigem, enable: ko.observable(true) });
    this.TagPortoDestinoCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#PortoDestino"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.PortoDestino, enable: ko.observable(true) });
    this.TagBookingCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#Booking"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Booking, enable: ko.observable(true) });
    this.TagNumeroFiscalCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#NumeroFiscal"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroFiscal, enable: ko.observable(true) });
    this.TagNumeroNFClienteCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#NumeroNFCliente"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroNfCliente, enable: ko.observable(true) });
    this.TagNumeroControleClienteCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#NumeroControleCliente"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroControleCliente, enable: ko.observable(true) });
    this.TagRemetenteCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#Remetente"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Remetente, enable: ko.observable(true) });
    this.TagDestinatarioCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#Destinatario"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Destinatario, enable: ko.observable(true) });
    this.TagTomadorCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#Tomador"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.Tomador, enable: ko.observable(true) });
    this.TagQuebraLinhaCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#QuebraLinha"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.QuebraDeLinha, enable: ko.observable(true) });
    this.TagNumeroContainerCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#NumeroContainer"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroContainer, enable: ko.observable(true) });
    this.TagNumeroControleSVMCorpoPorta = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.CorpoEmailDocumentacaoPorta.id, "#NumeroControleSVM"); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.NumeroControleSVM, enable: ko.observable(true) });

    // Envio canhoto
    this.CanhotoHabilitarEnvioCanhoto = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Configuracao.Fatura.CanhotoHabilitarEnvioCanhoto.getFieldDescription(), def: false, visible: ko.observable(true) });
    this.TipoIntegracaoCanhoto = PropertyEntity({ val: ko.observable(EnumTipoIntegracaoCanhoto.Nenhum), options: EnumTipoIntegracaoCanhoto.obterOpcoes(), def: EnumTipoIntegracaoCanhoto.Nenhum, text: Localization.Resources.Configuracao.Fatura.TipoIntegracao.getFieldDescription(), required: false, visible: ko.observable(true), enable: true });
    this.EnderecoFTP = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.Endereco, maxlength: 150, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.Usuario, maxlength: 50, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.Senha = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.Fatura.Senha), maxlength: 50, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.Porta = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.Porta, maxlength: 10, def: "21", val: ko.observable("21"), enable: ko.observable(false), visible: ko.observable(false), required: ko.observable(false) });
    this.Diretorio = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.Diretorio, maxlength: 400, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.Passivo = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.FTPPassivo, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.SSL = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.SSL, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.UtilizarSFTP = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.SFTP, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.CertificadoChavePrivada = PropertyEntity({ text: Localization.Resources.Configuracao.Fatura.CertificadoChavePrivada, type: types.file, codEntity: ko.observable(0), val: ko.observable(""), accept: ".txt,.ppk", required: ko.observable(false), visible: ko.observable(false) });
    this.CertificadoChavePrivadaBase64 = PropertyEntity({ text: ko.observable(""), type: types.string, codEntity: ko.observable(0), val: ko.observable(""), visible: ko.observable(false) });
    this.TestarConexaoFTP = PropertyEntity({ eventClick: function () { instancia.TestarConexaoFTP(); }, type: types.event, text: Localization.Resources.Configuracao.Fatura.TestarConexao, visible: ko.observable(false) });
    this.NomeArquivo = PropertyEntity({ text: ko.observable("Selecione um certificado"), val: ko.observable(""), def: "", getType: typesKnockout.string, visible: ko.observable(false) });
    this.Nomenclatura = PropertyEntity({ text: "Nomenclatura do arquivo: ", required: false, visible: ko.observable(true), val: ko.observable(""), maxlength: 600 });
    this.ExtensaoArquivo = PropertyEntity({ text: "Extensão do arquivo: ", required: false, maxlength: 100, visible: ko.observable(true) });

    this.TagTransportadoraCNPJ = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#CNPJTransportadora#"); }, type: types.event, text: "CNPJ Transportador", visible: ko.observable(true) });
    this.TagAno = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#Ano#"); }, type: types.event, text: "Ano", visible: ko.observable(true) });
    this.TagAnoAbreviado = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#AnoAbreviado#"); }, type: types.event, text: "Ano Abreviado", visible: ko.observable(true) });
    this.TagMes = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#Mes#"); }, type: types.event, text: "Mes", visible: ko.observable(true) });
    this.TagDia = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#Dia#"); }, type: types.event, text: "Dia", visible: ko.observable(true) });
    this.TagHora = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#Hora#"); }, type: types.event, text: "Hora", visible: ko.observable(true) });
    this.TagMin = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#Minutos#"); }, type: types.event, text: "Min", visible: ko.observable(true) });
    this.TagSeg = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#Segundos#"); }, type: types.event, text: "Seg", visible: ko.observable(true) });
    this.TagNumero = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#Numero#"); }, type: types.event, text: "Número", visible: ko.observable(true) });
    this.TagNumeroCarregamento = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#NumeroCarregamento#"); }, type: types.event, text: "Número Carregamento", visible: ko.observable(true) });
    this.TagCNPJCliente = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#CNPJCliente#"); }, type: types.event, text: "CNPJ Cliente", visible: ko.observable(true) });
    this.TagFinalCNPJCliente = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#FinalCNPJCliente#"); }, type: types.event, text: "Final do CNPJ Cliente", visible: ko.observable(true) });
    this.TagIdRegistro = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#IdRegistro#"); }, type: types.event, text: "ID Registro (único)", visible: ko.observable(true) });
    this.TagCodigoEmpresa = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#CodigoEmpresa#"); }, type: types.event, text: "Código da Empresa", visible: ko.observable(true) });
    this.TagCodigoEstabelecimento = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#CodigoEstabelecimento#"); }, type: types.event, text: "Código do Estabelecimento", visible: ko.observable(true) });
    this.TagProtocoloCarga = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#ProtocoloCarga#"); }, type: types.event, text: "Protocolo da Carga", visible: ko.observable(true) });
    this.TagCodOcorrencia = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#CodigoOcorrencia#"); }, type: types.event, text: "Código ocorrencia", visible: ko.observable(true) });
    this.TagMinutoOcorrencia = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#MinutoOcorrencia#"); }, type: types.event, text: "Minuto ocorrencia", visible: ko.observable(true) });
    this.TagHoraOcorrencia = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#HoraOcorrencia#"); }, type: types.event, text: "Hora ocorrencia", visible: ko.observable(true) });
    this.TagDataOcorrencia = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#DataOcorrencia#"); }, type: types.event, text: "Data ocorrencia", visible: ko.observable(true) });
    this.TagNumeroNFE = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#NumeroNFE#"); }, type: types.event, text: "Número NFE", visible: ko.observable(true) });
    this.TagSerieNFE = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#SerieNFE#"); }, type: types.event, text: "Série NFE", visible: ko.observable(true) });
    this.TagEmitenteNFE = PropertyEntity({ eventClick: function (e) { InserirTag(instancia.Configuracao.Nomenclatura.id, "#EmitenteNFE#"); }, type: types.event, text: "Emitente NFE", visible: ko.observable(true) });

    //Subscribe
    this.TipoIntegracaoCanhoto.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoIntegracaoCanhoto.FTP) {
            instancia.Configuracao.EnderecoFTP.required(true);
            instancia.Configuracao.EnderecoFTP.visible(true);
            instancia.Configuracao.Usuario.required(true);
            instancia.Configuracao.Usuario.visible(true);
            instancia.Configuracao.Senha.required(true);
            instancia.Configuracao.Senha.visible(true);
            instancia.Configuracao.Porta.required(true);
            instancia.Configuracao.Porta.visible(true);
            instancia.Configuracao.Diretorio.required(true);
            instancia.Configuracao.Diretorio.visible(true);
            instancia.Configuracao.Passivo.required(true);
            instancia.Configuracao.Passivo.visible(true);
            instancia.Configuracao.SSL.required(true);
            instancia.Configuracao.SSL.visible(true);
            instancia.Configuracao.UtilizarSFTP.required(true);
            instancia.Configuracao.UtilizarSFTP.visible(true);
            instancia.Configuracao.CertificadoChavePrivada.visible(true);
            instancia.Configuracao.TestarConexaoFTP.visible(true);
            instancia.Configuracao.NomeArquivo.visible(true);
        } else {
            instancia.Configuracao.EnderecoFTP.required(false);
            instancia.Configuracao.EnderecoFTP.visible(false);
            instancia.Configuracao.Usuario.required(false);
            instancia.Configuracao.Usuario.visible(false);
            instancia.Configuracao.Senha.required(false);
            instancia.Configuracao.Senha.visible(false);
            instancia.Configuracao.Porta.required(false);
            instancia.Configuracao.Porta.visible(false);
            instancia.Configuracao.Diretorio.required(false);
            instancia.Configuracao.Diretorio.visible(false);
            instancia.Configuracao.Passivo.required(false);
            instancia.Configuracao.Passivo.visible(false);
            instancia.Configuracao.SSL.required(false);
            instancia.Configuracao.SSL.visible(false);
            instancia.Configuracao.UtilizarSFTP.required(false);
            instancia.Configuracao.UtilizarSFTP.visible(false);
            instancia.Configuracao.CertificadoChavePrivada.visible(false);
            instancia.Configuracao.TestarConexaoFTP.visible(false);
            instancia.Configuracao.NomeArquivo.visible(false);
        }

    })

    this.UtilizarSFTP.val.subscribe(function (novoValor) {
        instancia.Configuracao.Senha.required(true);
        instancia.Configuracao.Senha.text("*" + Localization.Resources.Configuracao.Fatura.Senha);
        if (novoValor) {
            instancia.Configuracao.Senha.required(false);
            instancia.Configuracao.Senha.text(Localization.Resources.Configuracao.Fatura.Senha);
        } else {
            instancia.Configuracao.NomeArquivo.val("");
            instancia.Configuracao.NomeArquivo.text("");
            instancia.Configuracao.CertificadoChavePrivada.val("");
            instancia.Configuracao.CertificadoChavePrivadaBase64.val("");

        }

    })

    this.CertificadoChavePrivada.val.subscribe(function (novoValor) {
        var nomeArquivo = null;

        if (novoValor != null) {
            nomeArquivo = novoValor.replace("C:\\fakepath\\", "");

            instancia.Configuracao.NomeArquivo.text(nomeArquivo);
            instancia.Configuracao.NomeArquivo.val(nomeArquivo);
            var arquivo = document.getElementById(instancia.Configuracao.CertificadoChavePrivada.id);

            if (arquivo?.files?.[0]) {
                getBase64FileFatura(instancia.Configuracao.CertificadoChavePrivadaBase64, arquivo.files[0]);
            }
        }
    });

    this.CanhotoHabilitarEnvioCanhoto.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoIntegracaoCanhoto.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.EnderecoFTP.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Usuario.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Senha.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Porta.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Diretorio.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Passivo.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.UtilizarSFTP.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.SSL.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.CertificadoChavePrivada.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.CertificadoChavePrivadaBase64.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Nomenclatura.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ExtensaoArquivo.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.DiaSemana.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.DiaMes.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.PermiteFinalSemana.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.UtilizarCadastroContaBancaria.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.ContasBancarias.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.ContaBancaria.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.ExigeCanhotoFisico.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.NaoGerarFaturaAteReceberCanhotos.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.Banco.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor.trim() == "")
            instancia.Configuracao.Banco.codEntity(0);
    });

    this.Banco.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.Agencia.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.Digito.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.NumeroConta.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TipoConta.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TomadorFatura.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor.trim() == "")
            instancia.Configuracao.TomadorFatura.codEntity(0);
    });

    this.TomadorFatura.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.ObservacaoFatura.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TipoPrazoFaturamento.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.FormaGeracaoTituloFatura.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.DiasDePrazoFatura.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.BoletoConfiguracao.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor.trim() == "")
            instancia.Configuracao.BoletoConfiguracao.codEntity(0);
    });

    this.FormaPagamento.val.subscribe(function (novoValor) {
        if (novoValor == null || novoValor.trim() == "")
            instancia.Configuracao.FormaPagamento.codEntity(0);
    });

    this.FormaPagamento.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.GerarTituloPorDocumentoFiscal.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.GerarTituloAutomaticamente.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.GerarFaturaAutomaticaCte.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.SomenteOcorrenciasFinalizadoras.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.FaturarSomenteOcorrenciasFinalizadoras.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.ArmazenaCanhotoFisicoCTe.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.GerarFaturamentoAVista.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.AssuntoEmailFatura.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.CorpoEmailFatura.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.GerarBoletoAutomaticamente.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.EnviarArquivosDescompactados.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TipoEnvioFatura.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TipoAgrupamentoFatura.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.DiasSemanaFatura.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.DiasMesFatura.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.GerarFaturaPorCte.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.EmailEnvioDocumentacao.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TipoAgrupamentoEnvioDocumentacao.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.AssuntoDocumentacao.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.CorpoEmailDocumentacao.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.EmailFatura.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.NaoEnviarEmailFaturaAutomaticamente.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.FormaEnvioDocumentacao.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.InformarEmailEnvioDocumentacao.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.InformarEmailEnvioDocumentacaoPorta.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.EmailEnvioDocumentacaoPorta.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TipoAgrupamentoEnvioDocumentacaoPorta.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.FormaEnvioDocumentacaoPorta.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.AssuntoDocumentacaoPorta.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.CorpoEmailDocumentacaoPorta.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.GerarFaturamentoMultiplaParcela.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.AvisoVencimetoHabilitarConfiguracaoPersonalizada.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.AvisoVencimetoQunatidadeDias.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.AvisoVencimetoEnviarDiariamente.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.CobrancaHabilitarConfiguracaoPersonalizada.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.AvisoVencimetoNaoEnviarEmail.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.CobrancaNaoEnviarEmail.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.CobrancaQunatidadeDias.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.QuantidadeParcelasFaturamento.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.FormaTitulo.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.EnviarBoletoPorEmailAutomaticamente.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.BoletoConfiguracao.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.EnviarDocumentacaoFaturamentoCTe.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.NaoValidarPossuiAcordoFaturamentoAvancoCarga.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.HabilitarPeriodoVencimentoEspecifico.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.FaturaVencimentos.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.GerarTituloAutomaticamenteComAdiantamentoSaldo.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.PercentualAdiantamentoTituloAutomatico.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.PrazoAdiantamentoEmDiasTituloAutomatico.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.PercentualSaldoTituloAutomatico.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.PrazoSaldoEmDiasTituloAutomatico.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.EfetuarImpressaoDaTaxaDeMoedaEstrangeira.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
};

function ConfiguracaoFatura(idContent, knockoutConfiguracao, loadCallback) {
    var instancia = this;
    var vencimentosConfig;
    this.RetornarValores = function (diaSemana, diaMes, permiteFinalSemana, exigeCanhotoFisico, naoGerarFaturaAteReceberCanhotos, banco, agencia, digito, numeroConta, tipoConta, tomadorFatura,
        observacaoFatura, tipoPrazoFaturamento, diasDePrazoFatura, formaPagamento, gerarTituloPorDocumentoFiscal, gerarTituloAutomaticamente, gerarFaturaAutomaticaCte, somenteOcorrenciasFinalizadoras,
        faturarSomenteOcorrenciasFinalizadoras, armazenaCanhotoFisicoCTe, gerarFaturamentoAVista, assuntoEmailFatura, corpoEmailFatura, gerarBoletoAutomaticamente, enviarArquivosDescompactados,
        tipoEnvioFatura, tipoAgrupamentoFatura, diasSemanaFatura, diasMesFatura, gerarFaturaPorCte, formaGeracaoTituloFatura, emailEnvioDocumentacao, tipoAgrupamentoEnvioDocumentacao,
        assuntoDocumentacao, corpoEmailDocumentacao, emailFatura, naoEnviarEmailFaturaAutomaticamente, formaEnvioDocumentacao, informarEmailEnvioDocumentacao, informarEmailEnvioDocumentacaoPorta,
        emailEnvioDocumentacaoPorta, tipoAgrupamentoEnvioDocumentacaoPorta, formaEnvioDocumentacaoPorta, assuntoDocumentacaoPorta, corpoEmailDocumentacaoPorta, gerarFaturamentoMultiplaParcela, quantidadeParcelasFaturamento, formaTitulo,
        boletoConfiguracao, enviarBoletoPorEmailAutomaticamente, enviarDocumentacaoFaturamentoCTe, naoValidarPossuiAcordoFaturamentoAvancoCarga, habilitarPeriodoVencimentoEspecifico, faturaVencimentos,
        gerarTituloAutomaticamenteComAdiantamentoSaldo, percentualAdiantamentoTituloAutomatico, prazoAdiantamentoEmDiasTituloAutomatico, percentualSaldoTituloAutomatico, prazoSaldoEmDiasTituloAutomatico,
        avisoVencimetoHabilitarConfiguracaoPersonalizada, avisoVencimetoQunatidadeDias, avisoVencimetoEnviarDiariamente, cobrancaHabilitarConfiguracaoPersonalizada, cobrancaQunatidadeDias, avisoVencimetoNaoEnviarEmail, cobrancaNaoEnviarEmail,
        canhotoHabilitarEnvioCanhoto, tipoIntegracaoCanhoto, enderecoFTP, usuario, senha, porta, diretorio, passivo, ssl, utilizarSFTP, certificadoChavePrivada, certificadoChavePrivadaBase64, nomenclatura, extensaoArquivo, utilizarCadastroContaBancaria, contasBancarias,
        efetuarImpressaoDaTaxaDeMoedaEstrangeira) {

        instancia.KnockoutConfiguracao.val(JSON.stringify({
            DiaSemana: diaSemana, DiaMes: diaMes, PermiteFinalSemana: permiteFinalSemana, NaoGerarFaturaAteReceberCanhotos: naoGerarFaturaAteReceberCanhotos,
            ExigeCanhotoFisico: exigeCanhotoFisico, Banco: banco, Agencia: agencia, Digito: digito, NumeroConta: numeroConta, TipoConta: tipoConta, TomadorFatura: tomadorFatura,
            ObservacaoFatura: observacaoFatura, TipoPrazoFaturamento: tipoPrazoFaturamento, DiasDePrazoFatura: diasDePrazoFatura, FormaPagamento: formaPagamento,
            GerarTituloPorDocumentoFiscal: gerarTituloPorDocumentoFiscal, GerarTituloAutomaticamente: gerarTituloAutomaticamente, GerarFaturaAutomaticaCte: gerarFaturaAutomaticaCte, SomenteOcorrenciasFinalizadoras: somenteOcorrenciasFinalizadoras,
            FaturarSomenteOcorrenciasFinalizadoras: faturarSomenteOcorrenciasFinalizadoras, ArmazenaCanhotoFisicoCTe: armazenaCanhotoFisicoCTe, GerarFaturamentoAVista: gerarFaturamentoAVista,
            AssuntoEmailFatura: assuntoEmailFatura, CorpoEmailFatura: corpoEmailFatura, GerarBoletoAutomaticamente: gerarBoletoAutomaticamente, EnviarArquivosDescompactados: enviarArquivosDescompactados,
            TipoEnvioFatura: tipoEnvioFatura, TipoAgrupamentoFatura: tipoAgrupamentoFatura, DiasSemanaFatura: diasSemanaFatura, DiasMesFatura: diasMesFatura, GerarFaturaPorCte: gerarFaturaPorCte,
            FormaGeracaoTituloFatura: formaGeracaoTituloFatura, EmailEnvioDocumentacao: emailEnvioDocumentacao, TipoAgrupamentoEnvioDocumentacao: tipoAgrupamentoEnvioDocumentacao,
            AssuntoDocumentacao: assuntoDocumentacao, CorpoEmailDocumentacao: corpoEmailDocumentacao, EmailFatura: emailFatura, NaoEnviarEmailFaturaAutomaticamente: naoEnviarEmailFaturaAutomaticamente,
            FormaEnvioDocumentacao: formaEnvioDocumentacao, InformarEmailEnvioDocumentacao: informarEmailEnvioDocumentacao, InformarEmailEnvioDocumentacaoPorta: informarEmailEnvioDocumentacaoPorta,
            EmailEnvioDocumentacaoPorta: emailEnvioDocumentacaoPorta, TipoAgrupamentoEnvioDocumentacaoPorta: tipoAgrupamentoEnvioDocumentacaoPorta, FormaEnvioDocumentacaoPorta: formaEnvioDocumentacaoPorta,
            AssuntoDocumentacaoPorta: assuntoDocumentacaoPorta, CorpoEmailDocumentacaoPorta: corpoEmailDocumentacaoPorta, GerarFaturamentoMultiplaParcela: gerarFaturamentoMultiplaParcela, QuantidadeParcelasFaturamento: quantidadeParcelasFaturamento,
            FormaTitulo: formaTitulo, BoletoConfiguracao: boletoConfiguracao, EnviarBoletoPorEmailAutomaticamente: enviarBoletoPorEmailAutomaticamente, EnviarDocumentacaoFaturamentoCTe: enviarDocumentacaoFaturamentoCTe,
            NaoValidarPossuiAcordoFaturamentoAvancoCarga: naoValidarPossuiAcordoFaturamentoAvancoCarga, HabilitarPeriodoVencimentoEspecifico: habilitarPeriodoVencimentoEspecifico, FaturaVencimentos: faturaVencimentos,
            GerarTituloAutomaticamenteComAdiantamentoSaldo: gerarTituloAutomaticamenteComAdiantamentoSaldo, PercentualAdiantamentoTituloAutomatico: percentualAdiantamentoTituloAutomatico, PrazoAdiantamentoEmDiasTituloAutomatico: prazoAdiantamentoEmDiasTituloAutomatico,
            PercentualSaldoTituloAutomatico: percentualSaldoTituloAutomatico, PrazoSaldoEmDiasTituloAutomatico: prazoSaldoEmDiasTituloAutomatico, AvisoVencimetoHabilitarConfiguracaoPersonalizada: avisoVencimetoHabilitarConfiguracaoPersonalizada,
            AvisoVencimetoQunatidadeDias: avisoVencimetoQunatidadeDias, AvisoVencimetoEnviarDiariamente: avisoVencimetoEnviarDiariamente, CobrancaHabilitarConfiguracaoPersonalizada: cobrancaHabilitarConfiguracaoPersonalizada, CobrancaQunatidadeDias: cobrancaQunatidadeDias,
            AvisoVencimetoNaoEnviarEmail: avisoVencimetoNaoEnviarEmail, CobrancaNaoEnviarEmail: cobrancaNaoEnviarEmail, CanhotoHabilitarEnvioCanhoto: canhotoHabilitarEnvioCanhoto, TipoIntegracaoCanhoto: tipoIntegracaoCanhoto, EnderecoFTP: enderecoFTP,
            Usuario: usuario, Senha: senha, Porta: porta, Diretorio: diretorio, Passivo: passivo, SSL: ssl, UtilizarSFTP: utilizarSFTP, CertificadoChavePrivada: certificadoChavePrivada, CertificadoChavePrivadaBase64: certificadoChavePrivadaBase64,
            Nomenclatura: nomenclatura, ExtensaoArquivo: extensaoArquivo, UtilizarCadastroContaBancaria: utilizarCadastroContaBancaria, ContasBancarias: contasBancarias, EfetuarImpressaoDaTaxaDeMoedaEstrangeira: efetuarImpressaoDaTaxaDeMoedaEstrangeira
        }));
    };

    this.SetarValores = function (valores) {
        instancia.Configuracao.DiaSemana.val(valores.DiaSemana);
        instancia.Configuracao.DiaMes.val(valores.DiaMes);
        instancia.Configuracao.PermiteFinalSemana.val(valores.PermiteFinalSemana);
        instancia.Configuracao.ExigeCanhotoFisico.val(valores.ExigeCanhotoFisico);
        instancia.Configuracao.NaoGerarFaturaAteReceberCanhotos.val(valores.NaoGerarFaturaAteReceberCanhotos);
        instancia.Configuracao.Banco.val(valores.Banco.Descricao);
        instancia.Configuracao.Banco.codEntity(valores.Banco.Codigo);
        instancia.Configuracao.Agencia.val(valores.Agencia);
        instancia.Configuracao.Digito.val(valores.Digito);
        instancia.Configuracao.NumeroConta.val(valores.NumeroConta);
        instancia.Configuracao.TipoConta.val(valores.TipoConta);
        instancia.Configuracao.TomadorFatura.val(valores.TomadorFatura.Descricao);
        instancia.Configuracao.TomadorFatura.codEntity(valores.TomadorFatura.Codigo);
        instancia.Configuracao.ObservacaoFatura.val(valores.ObservacaoFatura);
        instancia.Configuracao.TipoPrazoFaturamento.val(valores.TipoPrazoFaturamento);
        instancia.Configuracao.FormaGeracaoTituloFatura.val(valores.FormaGeracaoTituloFatura);
        instancia.Configuracao.DiasDePrazoFatura.val(valores.DiasDePrazoFatura);
        instancia.Configuracao.FormaPagamento.val(valores.FormaPagamento.Descricao);
        instancia.Configuracao.FormaPagamento.codEntity(valores.FormaPagamento.Codigo);
        instancia.Configuracao.GerarTituloPorDocumentoFiscal.val(valores.GerarTituloPorDocumentoFiscal);
        instancia.Configuracao.GerarTituloAutomaticamente.val(valores.GerarTituloAutomaticamente);
        instancia.Configuracao.GerarFaturaAutomaticaCte.val(valores.GerarFaturaAutomaticaCte);
        instancia.Configuracao.GerarFaturamentoAVista.val(valores.GerarFaturamentoAVista);
        instancia.Configuracao.SomenteOcorrenciasFinalizadoras.val(valores.SomenteOcorrenciasFinalizadoras);
        instancia.Configuracao.FaturarSomenteOcorrenciasFinalizadoras.val(valores.FaturarSomenteOcorrenciasFinalizadoras);
        instancia.Configuracao.ArmazenaCanhotoFisicoCTe.val(valores.ArmazenaCanhotoFisicoCTe);
        instancia.Configuracao.EmailFatura.val(valores.EmailFatura);
        instancia.Configuracao.AssuntoEmailFatura.val(valores.AssuntoEmailFatura);
        instancia.Configuracao.CorpoEmailFatura.val(valores.CorpoEmailFatura);
        instancia.Configuracao.GerarBoletoAutomaticamente.val(valores.GerarBoletoAutomaticamente);
        instancia.Configuracao.EnviarArquivosDescompactados.val(valores.EnviarArquivosDescompactados);
        instancia.Configuracao.TipoEnvioFatura.val(valores.TipoEnvioFatura);
        instancia.Configuracao.TipoAgrupamentoFatura.val(valores.TipoAgrupamentoFatura);
        instancia.Configuracao.UtilizarCadastroContaBancaria.val(valores.UtilizarCadastroContaBancaria);
        instancia.Configuracao.ContasBancarias.val(JSON.stringify(valores.ContasBancarias));
        instancia.Configuracao.EfetuarImpressaoDaTaxaDeMoedaEstrangeira.val(valores.EfetuarImpressaoDaTaxaDeMoedaEstrangeira);

        RecarregarGrupoPessoaContaBancaria(valores.ContasBancarias, instancia)

        $("#" + instancia.Configuracao.DiasSemanaFatura.id).selectpicker('val', valores.DiasSemanaFatura);
        $("#" + instancia.Configuracao.DiasMesFatura.id).selectpicker('val', valores.DiasMesFatura);

        instancia.Configuracao.GerarFaturaPorCte.val(valores.GerarFaturaPorCte);
        instancia.Configuracao.NaoEnviarEmailFaturaAutomaticamente.val(valores.NaoEnviarEmailFaturaAutomaticamente);

        instancia.Configuracao.EmailEnvioDocumentacao.val(valores.EmailEnvioDocumentacao);
        instancia.Configuracao.TipoAgrupamentoEnvioDocumentacao.val(valores.TipoAgrupamentoEnvioDocumentacao);
        instancia.Configuracao.AssuntoDocumentacao.val(valores.AssuntoDocumentacao);
        instancia.Configuracao.CorpoEmailDocumentacao.val(valores.CorpoEmailDocumentacao);

        instancia.Configuracao.FormaEnvioDocumentacao.val(valores.FormaEnvioDocumentacao);
        instancia.Configuracao.InformarEmailEnvioDocumentacao.val(valores.InformarEmailEnvioDocumentacao);
        instancia.Configuracao.InformarEmailEnvioDocumentacaoPorta.val(valores.InformarEmailEnvioDocumentacaoPorta);
        instancia.Configuracao.EmailEnvioDocumentacaoPorta.val(valores.EmailEnvioDocumentacaoPorta);
        instancia.Configuracao.TipoAgrupamentoEnvioDocumentacaoPorta.val(valores.TipoAgrupamentoEnvioDocumentacaoPorta);
        instancia.Configuracao.FormaEnvioDocumentacaoPorta.val(valores.FormaEnvioDocumentacaoPorta);
        instancia.Configuracao.AssuntoDocumentacaoPorta.val(valores.AssuntoDocumentacaoPorta);
        instancia.Configuracao.CorpoEmailDocumentacaoPorta.val(valores.CorpoEmailDocumentacaoPorta);
        instancia.Configuracao.GerarFaturamentoMultiplaParcela.val(valores.GerarFaturamentoMultiplaParcela);
        instancia.Configuracao.AvisoVencimetoHabilitarConfiguracaoPersonalizada.val(valores.AvisoVencimetoHabilitarConfiguracaoPersonalizada);
        instancia.Configuracao.AvisoVencimetoQunatidadeDias.val(valores.AvisoVencimetoQunatidadeDias);
        instancia.Configuracao.AvisoVencimetoEnviarDiariamente.val(valores.AvisoVencimetoEnviarDiariamente);
        instancia.Configuracao.CobrancaHabilitarConfiguracaoPersonalizada.val(valores.CobrancaHabilitarConfiguracaoPersonalizada);
        instancia.Configuracao.AvisoVencimetoNaoEnviarEmail.val(valores.AvisoVencimetoNaoEnviarEmail);
        instancia.Configuracao.CobrancaNaoEnviarEmail.val(valores.CobrancaNaoEnviarEmail);
        instancia.Configuracao.CobrancaQunatidadeDias.val(valores.CobrancaQunatidadeDias);
        instancia.Configuracao.QuantidadeParcelasFaturamento.val(valores.QuantidadeParcelasFaturamento);
        instancia.Configuracao.FormaTitulo.val(valores.FormaTitulo);

        instancia.Configuracao.BoletoConfiguracao.val(valores.BoletoConfiguracao.Descricao);
        instancia.Configuracao.BoletoConfiguracao.codEntity(valores.BoletoConfiguracao.Codigo);
        instancia.Configuracao.EnviarBoletoPorEmailAutomaticamente.val(valores.EnviarBoletoPorEmailAutomaticamente);
        instancia.Configuracao.EnviarDocumentacaoFaturamentoCTe.val(valores.EnviarDocumentacaoFaturamentoCTe);
        instancia.Configuracao.NaoValidarPossuiAcordoFaturamentoAvancoCarga.val(valores.NaoValidarPossuiAcordoFaturamentoAvancoCarga);
        instancia.Configuracao.HabilitarPeriodoVencimentoEspecifico.val(valores.HabilitarPeriodoVencimentoEspecifico);
        instancia.Configuracao.FaturaVencimentos.val(valores.FaturaVencimentos);

        instancia.Configuracao.GerarTituloAutomaticamenteComAdiantamentoSaldo.val(valores.GerarTituloAutomaticamenteComAdiantamentoSaldo);
        instancia.Configuracao.PercentualAdiantamentoTituloAutomatico.val(valores.PercentualAdiantamentoTituloAutomatico);
        instancia.Configuracao.PrazoAdiantamentoEmDiasTituloAutomatico.val(valores.PrazoAdiantamentoEmDiasTituloAutomatico);
        instancia.Configuracao.PercentualSaldoTituloAutomatico.val(valores.PercentualSaldoTituloAutomatico);
        instancia.Configuracao.PrazoSaldoEmDiasTituloAutomatico.val(valores.PrazoSaldoEmDiasTituloAutomatico);

        if (valores.EnvioCanhoto && valores.EnvioCanhoto.length > 0) {
            var envioCanhoto = valores.EnvioCanhoto[0];
            instancia.Configuracao.CanhotoHabilitarEnvioCanhoto.val(envioCanhoto.CanhotoHabilitarEnvioCanhoto);
            instancia.Configuracao.TipoIntegracaoCanhoto.val(envioCanhoto.TipoIntegracaoCanhoto);
            instancia.Configuracao.EnderecoFTP.val(envioCanhoto.EnderecoFTP);
            instancia.Configuracao.Usuario.val(envioCanhoto.Usuario);
            instancia.Configuracao.Senha.val(envioCanhoto.Senha);
            instancia.Configuracao.Porta.val(envioCanhoto.Porta);
            instancia.Configuracao.Diretorio.val(envioCanhoto.Diretorio);
            instancia.Configuracao.Passivo.val(envioCanhoto.Passivo);
            instancia.Configuracao.SSL.val(envioCanhoto.SSL);
            instancia.Configuracao.UtilizarSFTP.val(envioCanhoto.UtilizarSFTP);
            instancia.Configuracao.CertificadoChavePrivada.val(envioCanhoto.CertificadoChavePrivada);
            instancia.Configuracao.CertificadoChavePrivadaBase64.val(envioCanhoto.CertificadoChavePrivadaBase64);
            instancia.Configuracao.Nomenclatura.val(envioCanhoto.Nomenclatura);
            instancia.Configuracao.ExtensaoArquivo.val(envioCanhoto.ExtensaoArquivo);
        }

        vencimentosConfig.RecarregarGrid();

        instancia.RetornarValores(
            valores.DiaSemana, valores.DiaMes, valores.PermiteFinalSemana, valores.ExigeCanhotoFisico, valores.NaoGerarFaturaAteReceberCanhotos, valores.Banco.Codigo, valores.Agencia,
            valores.Digito, valores.NumeroConta, valores.TipoConta, valores.TomadorFatura.Codigo, valores.ObservacaoFatura, valores.TipoPrazoFaturamento, valores.DiasDePrazoFatura,
            valores.FormaPagamento.Codigo, valores.GerarTituloPorDocumentoFiscal, valores.GerarTituloAutomaticamente, valores.GerarFaturaAutomaticaCte, valores.SomenteOcorrenciasFinalizadoras, valores.FaturarSomenteOcorrenciasFinalizadoras,
            valores.ArmazenaCanhotoFisicoCTe, valores.GerarFaturamentoAVista, valores.AssuntoEmailFatura, valores.CorpoEmailFatura, valores.GerarBoletoAutomaticamente, valores.EnviarArquivosDescompactados,
            valores.TipoEnvioFatura, valores.TipoAgrupamentoFatura, valores.DiasSemanaFatura, valores.DiasMesFatura, valores.GerarFaturaPorCte, valores.FormaGeracaoTituloFatura,
            valores.EmailEnvioDocumentacao, valores.TipoAgrupamentoEnvioDocumentacao, valores.AssuntoDocumentacao, valores.CorpoEmailDocumentacao, valores.EmailFatura, valores.NaoEnviarEmailFaturaAutomaticamente,
            valores.FormaEnvioDocumentacao, valores.InformarEmailEnvioDocumentacao, valores.InformarEmailEnvioDocumentacaoPorta, valores.EmailEnvioDocumentacaoPorta, valores.TipoAgrupamentoEnvioDocumentacaoPorta,
            valores.FormaEnvioDocumentacaoPorta, valores.AssuntoDocumentacaoPorta, valores.CorpoEmailDocumentacaoPorta, valores.GerarFaturamentoMultiplaParcela, valores.QuantidadeParcelasFaturamento, valores.FormaTitulo,
            valores.BoletoConfiguracao.Codigo, valores.EnviarBoletoPorEmailAutomaticamente, valores.EnviarDocumentacaoFaturamentoCTe, valores.NaoValidarPossuiAcordoFaturamentoAvancoCarga, valores.HabilitarPeriodoVencimentoEspecifico, valores.FaturaVencimentos,
            valores.GerarTituloAutomaticamenteComAdiantamentoSaldo, valores.PercentualAdiantamentoTituloAutomatico, valores.PrazoAdiantamentoEmDiasTituloAutomatico, valores.PercentualSaldoTituloAutomatico,
            valores.PrazoSaldoEmDiasTituloAutomatico, valores.AvisoVencimetoHabilitarConfiguracaoPersonalizada, valores.AvisoVencimetoQunatidadeDias, valores.AvisoVencimetoEnviarDiariamente, valores.CobrancaHabilitarConfiguracaoPersonalizada, valores.CobrancaQunatidadeDias,
            valores.AvisoVencimetoNaoEnviarEmail, valores.CobrancaNaoEnviarEmail, valores.CanhotoHabilitarEnvioCanhoto, valores.TipoIntegracaoCanhoto, valores.EnderecoFTP, valores.Usuario, valores.Senha,
            valores.Porta, valores.Diretorio, valores.Passivo, valores.SSL, valores.UtilizarSFTP, valores.CertificadoChavePrivada, valores.CertificadoChavePrivadaBase64, valores.Nomenclatura, valores.ExtensaoArquivo, valores.UtilizarCadastroContaBancaria, valores.ContasBancarias,
            valores.EfetuarImpressaoDaTaxaDeMoedaEstrangeira
        );
    };

    this.Limpar = function () {
        instancia.LimparCampos();

        instancia.KnockoutConfiguracao.val(JSON.stringify(new Array()));

        instancia.LimparCampos();
        LimparCamposGrupoPessoaContaBancaria(instancia);
    };

    this.LimparCampos = function () {
        LimparCampos(instancia.Configuracao);
        vencimentosConfig.LimparListaFaturaVencimentos();
    };

    this.Load = function () {

        var p = new promise.Promise();

        LoadLocalizationResources("Configuracao.Fatura").then(function () {
            instancia.KnockoutConfiguracao = knockoutConfiguracao;
            instancia.Configuracao = new ConfiguracaoFaturaModel(instancia);
            instancia.RegistrosSelecionados = new Array();


            $.get("Content/Static/Configuracao/Fatura.html?dyn=" + guid(), function (data) {

                $("#" + idContent).html(data.replace(/#FaturaVencimentos/g, instancia.Configuracao.FaturaVencimentos.id));

                KoBindings(instancia.Configuracao, idContent);

                new BuscarBanco(instancia.Configuracao.Banco);
                new BuscarClientes(instancia.Configuracao.TomadorFatura);
                new BuscarTipoPagamentoRecebimento(instancia.Configuracao.FormaPagamento);
                new BuscarBoletoConfiguracao(instancia.Configuracao.BoletoConfiguracao);

                LoadGrupoPessoaContaBancaria(instancia.Configuracao, instancia);

                vencimentosConfig = new ConfiguracaoFaturaVencimento(instancia.Configuracao.FaturaVencimentos.id, true, instancia.Configuracao.FaturaVencimentos, function () {

                    instancia.RetornarValoresGerais(instancia);

                    p.done();
                });
                LocalizeCurrentPage();
            });
            LocalizeCurrentPage();
        });
        return p;
    };

    this.ObterTiposIntegracao = function () {
        var p = new promise.Promise();

        p.done();

        return p;
    };

    this.ResetarAbas = function () {
        $("#liObservacaoFatura a").click();
    };

    instancia.ObterTiposIntegracao().then(function () {
        return instancia.Load().then(function () {
            if (loadCallback != null)
                loadCallback();
        });
    });

    this.TestarConexaoFTP = function () {
        executarReST("FTP/TestarConexao", {
            Host: instancia.Configuracao.EnderecoFTP.val(),
            Porta: instancia.Configuracao.Porta.val(),
            Diretorio: instancia.Configuracao.Diretorio.val(),
            Usuario: instancia.Configuracao.Usuario.val(),
            Senha: instancia.Configuracao.Senha.val(),
            Passivo: instancia.Configuracao.Passivo.val(),
            UtilizarSFTP: instancia.Configuracao.UtilizarSFTP.val(),
            SSL: instancia.Configuracao.SSL.val(),
            CertificadoChavePrivadaBase64: instancia.Configuracao.CertificadoChavePrivadaBase64.val()
        }, function (r) {
            if (r.Success) {
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_sucesso").removeClass("hidden");
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_erro").addClass("hidden");
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, r.Msg);
            } else {
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_sucesso").addClass("hidden");
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_erro").removeClass("hidden");
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    };

}

function getBase64FileFatura(element, file) {
    let reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        let encoded = reader.result.toString().replace(/^data:(.*,)?/, '');
        if ((encoded.length % 4) > 0) {
            encoded += '='.repeat(4 - (encoded.length % 4));
        }
        element.val(encoded);
    }
}
function LoadGrupoPessoaContaBancaria(configuracao, instancia) {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirGrupoPessoaContaBancariaClick(configuracao, data, instancia)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridPessoaContaBancaria = new BasicDataTable(configuracao.GridContaBancaria.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarContasBancarias(configuracao.ContaBancaria, function (valor) {
        var data = new Array();

        var PessoaContaBancaria = configuracao.ContaBancaria.basicTable.BuscarRegistros();

        $.each(PessoaContaBancaria, function (i, conta) {
            var PessoaContaBancariaGrid = new Object();

            PessoaContaBancariaGrid.Codigo = conta.Codigo;
            PessoaContaBancariaGrid.Descricao = conta.Descricao;

            data.push(PessoaContaBancariaGrid);
        });

        if (valor != null) {
            $.each(valor, function (i, conta) {
                var PessoaContaBancariaGrid = new Object();

                PessoaContaBancariaGrid.Codigo = conta.Codigo;
                PessoaContaBancariaGrid.Descricao = conta.Descricao;

                data.push(PessoaContaBancariaGrid);
            });
        }
        configuracao.ContaBancaria.basicTable.CarregarGrid(data);

        instancia.RetornarValoresGerais(instancia);


    }, _gridPessoaContaBancaria);

    configuracao.ContaBancaria.basicTable = _gridPessoaContaBancaria;
    configuracao.ContaBancaria.basicTable.CarregarGrid(_gridPessoaContaBancaria);

}
function RecarregarGrupoPessoaContaBancaria(valor, instancia) {
    var data = new Array();

    var PessoaContaBancaria = instancia.Configuracao.ContaBancaria.basicTable.BuscarRegistros();

    $.each(PessoaContaBancaria, function (i, conta) {
        var PessoaContaBancariaGrid = new Object();

        PessoaContaBancariaGrid.Codigo = conta.Codigo;
        PessoaContaBancariaGrid.Descricao = conta.Descricao;
        if (PessoaContaBancariaGrid.Codigo != null && PessoaContaBancariaGrid.Codigo != undefined) {
            data.push(PessoaContaBancariaGrid);
        }
    });

    if (valor != null) {
        $.each(valor, function (i, conta) {
            var PessoaContaBancariaGrid = new Object();

            PessoaContaBancariaGrid.Codigo = conta.Codigo.toString();
            PessoaContaBancariaGrid.Descricao = conta.Descricao;

            if (PessoaContaBancariaGrid.Codigo != null && PessoaContaBancariaGrid.Codigo != undefined) {
                data.push(PessoaContaBancariaGrid);
            }
        });
    }
    instancia.Configuracao.ContaBancaria.basicTable.CarregarGrid(data);

    instancia.Configuracao.ContasBancarias.val(JSON.stringify(instancia.Configuracao.ContaBancaria.basicTable.BuscarRegistros()));

    instancia.RetornarValoresGerais(instancia);
}
function ExcluirGrupoPessoaContaBancariaClick(configuracao, data, instancia) {
    var PessoaContaBancariaGrid = configuracao.ContaBancaria.basicTable.BuscarRegistros();

    for (var i = 0; i < PessoaContaBancariaGrid.length; i++) {
        if (data.Codigo == PessoaContaBancariaGrid[i].Codigo) {
            PessoaContaBancariaGrid.splice(i, 1);
            break;
        }
    }

    configuracao.ContaBancaria.basicTable.CarregarGrid(PessoaContaBancariaGrid);

    instancia.RetornarValoresGerais(instancia);
}

function LimparCamposGrupoPessoaContaBancaria(instancia) {
    if (instancia.Configuracao.ContaBancaria.basicTable != null)
        instancia.Configuracao.ContaBancaria.basicTable.CarregarGrid(new Array());

    instancia.RetornarValoresGerais(instancia);
}
ConfiguracaoFatura.prototype.RetornarValoresGerais = function (instancia) {
    instancia.RetornarValores(instancia.Configuracao.DiaSemana.val(), instancia.Configuracao.DiaMes.val(), instancia.Configuracao.PermiteFinalSemana.val(),
        instancia.Configuracao.ExigeCanhotoFisico.val(), instancia.Configuracao.NaoGerarFaturaAteReceberCanhotos.val(), instancia.Configuracao.Banco.codEntity(), instancia.Configuracao.Agencia.val(),
        instancia.Configuracao.Digito.val(), instancia.Configuracao.NumeroConta.val(), instancia.Configuracao.TipoConta.val(), instancia.Configuracao.TomadorFatura.codEntity(),
        instancia.Configuracao.ObservacaoFatura.val(), instancia.Configuracao.TipoPrazoFaturamento.val(), instancia.Configuracao.DiasDePrazoFatura.val(),
        instancia.Configuracao.FormaPagamento.codEntity(), instancia.Configuracao.GerarTituloPorDocumentoFiscal.val(), instancia.Configuracao.GerarTituloAutomaticamente.val(), instancia.Configuracao.GerarFaturaAutomaticaCte.val(),
        instancia.Configuracao.SomenteOcorrenciasFinalizadoras.val(), instancia.Configuracao.FaturarSomenteOcorrenciasFinalizadoras.val(), instancia.Configuracao.ArmazenaCanhotoFisicoCTe.val(),
        instancia.Configuracao.GerarFaturamentoAVista.val(), instancia.Configuracao.AssuntoEmailFatura.val(), instancia.Configuracao.CorpoEmailFatura.val(),
        instancia.Configuracao.GerarBoletoAutomaticamente.val(), instancia.Configuracao.EnviarArquivosDescompactados.val(), instancia.Configuracao.TipoEnvioFatura.val(),
        instancia.Configuracao.TipoAgrupamentoFatura.val(), instancia.Configuracao.DiasSemanaFatura.val(), instancia.Configuracao.DiasMesFatura.val(), instancia.Configuracao.GerarFaturaPorCte.val(),
        instancia.Configuracao.FormaGeracaoTituloFatura.val(), instancia.Configuracao.EmailEnvioDocumentacao.val(), instancia.Configuracao.TipoAgrupamentoEnvioDocumentacao.val(),
        instancia.Configuracao.AssuntoDocumentacao.val(), instancia.Configuracao.CorpoEmailDocumentacao.val(), instancia.Configuracao.EmailFatura.val(), instancia.Configuracao.NaoEnviarEmailFaturaAutomaticamente.val(),
        instancia.Configuracao.FormaEnvioDocumentacao.val(), instancia.Configuracao.InformarEmailEnvioDocumentacao.val(), instancia.Configuracao.InformarEmailEnvioDocumentacaoPorta.val(), instancia.Configuracao.EmailEnvioDocumentacaoPorta.val(),
        instancia.Configuracao.TipoAgrupamentoEnvioDocumentacaoPorta.val(), instancia.Configuracao.FormaEnvioDocumentacaoPorta.val(), instancia.Configuracao.AssuntoDocumentacaoPorta.val(), instancia.Configuracao.CorpoEmailDocumentacaoPorta.val(),
        instancia.Configuracao.GerarFaturamentoMultiplaParcela.val(), instancia.Configuracao.QuantidadeParcelasFaturamento.val(), instancia.Configuracao.FormaTitulo.val(), instancia.Configuracao.BoletoConfiguracao.codEntity(),
        instancia.Configuracao.EnviarBoletoPorEmailAutomaticamente.val(), instancia.Configuracao.EnviarDocumentacaoFaturamentoCTe.val(), instancia.Configuracao.NaoValidarPossuiAcordoFaturamentoAvancoCarga.val(), instancia.Configuracao.HabilitarPeriodoVencimentoEspecifico.val(),
        instancia.Configuracao.FaturaVencimentos.val(), instancia.Configuracao.GerarTituloAutomaticamenteComAdiantamentoSaldo.val(), instancia.Configuracao.PercentualAdiantamentoTituloAutomatico.val(),
        instancia.Configuracao.PrazoAdiantamentoEmDiasTituloAutomatico.val(), instancia.Configuracao.PercentualSaldoTituloAutomatico.val(), instancia.Configuracao.PrazoSaldoEmDiasTituloAutomatico.val(),
        instancia.Configuracao.AvisoVencimetoHabilitarConfiguracaoPersonalizada.val(), instancia.Configuracao.AvisoVencimetoQunatidadeDias.val(), instancia.Configuracao.AvisoVencimetoEnviarDiariamente.val(), instancia.Configuracao.CobrancaHabilitarConfiguracaoPersonalizada.val(), instancia.Configuracao.CobrancaQunatidadeDias.val(),
        instancia.Configuracao.AvisoVencimetoNaoEnviarEmail.val(), instancia.Configuracao.CobrancaNaoEnviarEmail.val(), instancia.Configuracao.CanhotoHabilitarEnvioCanhoto.val(), instancia.Configuracao.TipoIntegracaoCanhoto.val(),
        instancia.Configuracao.EnderecoFTP.val(), instancia.Configuracao.Usuario.val(), instancia.Configuracao.Senha.val(), instancia.Configuracao.Porta.val(), instancia.Configuracao.Diretorio.val(), instancia.Configuracao.Passivo.val(),
        instancia.Configuracao.SSL.val(), instancia.Configuracao.UtilizarSFTP.val(), instancia.Configuracao.CertificadoChavePrivada.val(), instancia.Configuracao.CertificadoChavePrivadaBase64.val(), instancia.Configuracao.Nomenclatura.val(), instancia.Configuracao.ExtensaoArquivo.val(), instancia.Configuracao.UtilizarCadastroContaBancaria.val(), instancia.Configuracao.ContaBancaria.basicTable != null ? instancia.Configuracao.ContaBancaria.basicTable.BuscarRegistros() : null,
        instancia.Configuracao.EfetuarImpressaoDaTaxaDeMoedaEstrangeira.val()
    );
};

