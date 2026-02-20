var _configuracaoEmissaoCTeOpcoesTipoIntegracao;
var _PermissoesPersonalizadas;

var ConfiguracaoEmissaoCTeModel = function (instancia, enable) {

    this.GridApoliceSeguro = PropertyEntity({ type: types.local });
    this.ApoliceSeguro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.AdicionarApoliceDeSeguro, idBtnSearch: guid(), enable: ko.observable(enable), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-12 col-lg-12"), issue: 263 });

    this.ObservacaoEmissaoCarga = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Configuracao.EmissaoCTe.InformativoParaEmissaoDaCarga.getFieldDescription(), enable: ko.observable(enable), visible: ko.observable(true) });
    this.Observacao = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), enable: ko.observable(enable), visible: ko.observable(true), maxlength: 2000 });
    this.ObservacaoTerceiro = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Configuracao.EmissaoCTe.ObservacaoParaTerceiros.getFieldDescription(), enable: ko.observable(enable), visible: ko.observable(true), maxlength: 2000 });
    this.ArquivoImportacaoNotasFiscais = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.ArquivoParaImportacaoDeNotasFiscais.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true), issue: 402 });
    this.FormulaRateioFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.FormulaDoRateioDoFrete.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true), issue: 257 });
    this.TipoRateioDocumentos = PropertyEntity({ val: ko.observable(EnumTipoEmissaoCTeDocumentos.NaoInformado), options: EnumTipoEmissaoCTeDocumentos.obterOpcoes(), text: Localization.Resources.Configuracao.EmissaoCTe.RateioDosDocumentos.getFieldDescription(), def: EnumTipoEmissaoCTeDocumentos.NaoInformado, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true), issue: 400 });
    this.TipoEmissaoCTeParticipantes = PropertyEntity({ val: ko.observable(EnumTipoEmissaoCTeParticipantes.Normal), options: EnumTipoEmissaoCTeParticipantes.obterOpcoes(), text: Localization.Resources.Configuracao.EmissaoCTe.ParticipantesDosDocumentos.getFieldDescription(), def: EnumTipoEmissaoCTeParticipantes.Normal, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true), issue: 401 });
    this.TipoEmissaoIntramunicipal = PropertyEntity({ val: ko.observable(EnumTipoEmissaoIntramunicipal.NaoEspecificado), options: EnumTipoEmissaoIntramunicipal.obterOpcoes(), text: Localization.Resources.Configuracao.EmissaoCTe.TipoDeDocumentoParaFretesMunicipais.getFieldDescription(), def: EnumTipoEmissaoCTeParticipantes.Normal, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true), issue: 606 });
    this.TipoIntegracao = PropertyEntity({ val: ko.observable(_configuracaoEmissaoCTeOpcoesTipoIntegracao[0].value), options: _configuracaoEmissaoCTeOpcoesTipoIntegracao, text: Localization.Resources.Configuracao.EmissaoCTe.Integracao.getFieldDescription(), def: _configuracaoEmissaoCTeOpcoesTipoIntegracao[0].value, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true), issue: 267 });
    this.CobrarOutroDocumento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(enable) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.CobrarValorEmOutroDocumentoFiscal.getFieldDescription(), enable: ko.observable(false), required: false, idBtnSearch: guid(), visible: ko.observable(true), issue: 370 });
    this.TipoReceita = PropertyEntity({ val: ko.observable(EnumTipoReceita.NaoInformada), options: EnumTipoReceita.obterOpcoes(), text: "Revenue Type", def: EnumTipoReceita.NaoInformada, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.DisponibilizarDocumentosParaNFsManual = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.DisponibilizarDocumentosParaNFsManual, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.EmitirEmpresaFixa = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(enable) });
    this.EmpresaEmissora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.EmitirOsDocumentosCTeMDFeComMesmaEmpresaFilial.getFieldDescription(), enable: ko.observable(false), required: false, idBtnSearch: guid(), visible: ko.observable(true), issue: 63 });
    this.ComponentesFrete = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), enable: ko.observable(true), visible: ko.observable(true) });
    this.CTeEmitidoNoEmbarcador = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.OsCTesSaoEmitidosNoEmbarcador, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador), issue: 403 });
    this.AgruparMovimentoFinanceiroPorPedido = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.AgruparMovimentosFinanceirosPorPedido, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 406 });
    this.ExigirNumeroPedido = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.ExigeQueNumeroDoPedidoNoEmbarcadorSejaInformado, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 1065 });
    this.RegexValidacaoNumeroPedidoEmbarcador = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Configuracao.EmissaoCTe.PadraoParaValidacaoDoNumeroDoPedidoExpressaoRegular, enable: ko.observable(enable), visible: ko.observable(false) });
    this.NaoValidarNotaFiscalExistente = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.NaoValidarNotasFiscaisJaUtilizadasEmOutrosCTes, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 1066 });
    this.NaoValidarNotasFiscaisComDiferentesPortos = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.NaoValidarNotasFiscaisJaUtilizadasEmOutrosCTesDeDiferentesPortosDeOrigem, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.ValePedagioObrigatorio = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.ExigeQueValePedagioSejaInformado, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 1067 });
    this.NaoEmitirMDFe = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.NaoEmitirMDFe, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });
    this.ProvisionarDocumentos = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.PermiteProvisionarDocumentos, getType: typesKnockout.bool, enable: ko.observable(enable), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ? ko.observable(true) : ko.observable(false), issue: 0 });
    this.DisponibilizarDocumentosParaLoteEscrituracao = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.DisponibilizarDocumentosEmitidosParaEscrituracaoEmLote, getType: typesKnockout.bool, enable: ko.observable(enable), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ? ko.observable(true) : ko.observable(false), issue: 0 });
    this.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.DisponibilizarDocumentosCanceladosParaEscrituracaoEmLote, getType: typesKnockout.bool, enable: ko.observable(enable), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ? ko.observable(true) : ko.observable(false), issue: 0 });
    this.DisponibilizarDocumentosParaPagamento = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.DisponibilizarDocumentosParaPagamento, getType: typesKnockout.bool, enable: ko.observable(enable), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ? ko.observable(true) : ko.observable(false), issue: 0 });
    this.QuitarDocumentoAutomaticamenteAoGerarLote = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.QuitarDocumentoAutomaticamenteAoGerarLote, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(false), issue: 0 });
    this.EscriturarSomenteDocumentosEmitidosParaNFe = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.EscriturarSomenteDocumentosEmitidosParaNFeNotaFiscalEletronica, getType: typesKnockout.bool, enable: ko.observable(enable), visible: _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador ? ko.observable(true) : ko.observable(false), issue: 0 });
    this.GerarCIOTParaTodasAsCargas = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.GerarCIOTParaTodasAsCargas, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });
    this.ValorFreteLiquidoDeveSerValorAReceber = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.ValorDoFreteLiquidoDeveSerValorReceber, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });
    this.ValorFreteLiquidoDeveSerValorAReceberSemICMS = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.ValorDoFreteLiquidoDeveSerValorReceberMenosICMS, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });

    this.AvancarCargasDocumentosVinculados = PropertyEntity({ type: types.event, text: Localization.Resources.Configuracao.EmissaoCTe.AvancarCargasComDocumentosVinculados, visible: ko.observable(false), enable: ko.observable(false) });

    this.TornarPedidosPrioritarios = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.TornarPedidosPrioritarios, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(false), issue: 0 });

    this.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.ObrigatorioInformarMDFeEmitidoPeloEmbarcador = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.ObrigatorioInformarMDFeEmitidoPeloEmbarcador, getType: typesKnockout.bool, val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.DescricaoItemPesoCTeSubcontratacao = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.DescricaoDoPesoNosCTesParaSubcontratacao, issue: 2401, maxlength: 50, enable: ko.observable(true), visible: ko.observable(true) });
    this.CaracteristicaTransporteCTe = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.CaracteristicaDoTransporteParaCTe.getFieldDescription(), maxlength: 15, enable: ko.observable(true), visible: ko.observable(true) });

    this.GerarMDFeTransbordoSemConsiderarOrigem = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.GerarMDFeDeCargasDeTransbordoPartirDoDestinoDoPedido, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });

    this.ImportarRedespachoIntermediario = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.PermitirImportacaoDeCTesDeRedespachoIntermediarioNaCarga, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });
    this.EmitenteImportacaoRedespachoIntermediario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.FixarEmitenteDoRedespachoIntermediario.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true), issue: 0 });
    this.ExpedidorImportacaoRedespachoIntermediario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.FixarExpedidorDoRedespachoIntermediario.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true), issue: 0 });
    this.RecebedorImportacaoRedespachoIntermediario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.FixarRecebedorDoRedespachoIntermediario.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true), issue: 0 });

    this.BloquearDiferencaValorFreteEmbarcador = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.ExigirAutorizacaoParaCargasComValorDoEmbarcadorDiferenteDaTabelaDeFrete, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });
    this.PercentualBloquearDiferencaValorFreteEmbarcador = PropertyEntity({ val: ko.observable("0,00"), def: "0,00", text: Localization.Resources.Configuracao.EmissaoCTe.PorcentagemDeDiferencaMinima.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 5, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });
    this.EmitirComplementoDiferencaFreteEmbarcador = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.GerarOcorrenciaAutomaticamenteParaCargasComValorDoEmbarcadorDiferenteDaTabelaDeFrete, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });
    this.GerarOcorrenciaSemTabelaFrete = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.GerarOcorrenciaSemTabelaFrete, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true) });
    this.TipoOcorrenciaSemTabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.TipoDeOcorrencia.getRequiredFieldDescription(), enable: ko.observable(enable), required: false, idBtnSearch: guid(), visible: ko.observable(true), issue: 0 });
    this.LiberarDocumentosEmitidosQuandoEntregaForConfirmada = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.LiberarDocumentosEmitidosQuandoEntregaForConfirmada, getType: typesKnockout.bool, visible: ko.observable(true), issue: 0 });
    this.DisponibilizarComposicaoRateioCarga = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.DisponibilizarComposicaoRateioCarga, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TipoOcorrenciaComplementoDiferencaFreteEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.TipoDeOcorrencia.getRequiredFieldDescription(), enable: ko.observable(enable), required: false, idBtnSearch: guid(), visible: ko.observable(true), issue: 0 });
    this.NaoPermitirVincularCTeComplementarEmCarga = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.NaoPermitirVincularCTeComplementarEmCargas, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });

    this.TipoOcorrenciaCTeEmitidoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.TipoDeOcorrenciaAutomatizacaoSeOcorrencias.getRequiredFieldDescription(), enable: ko.observable(enable), required: false, idBtnSearch: guid(), visible: ko.observable(true), issue: 0 });

    this.UtilizarOutroModeloDocumentoEmissaoMunicipal = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(enable) });
    this.ModeloDocumentoFiscalEmissaoMunicipal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.GerarOutroDocumentoMunicipal.getFieldDescription(), enable: ko.observable(false), required: false, idBtnSearch: guid(), visible: ko.observable(false), issue: 1524 });

    this.EnderecoFTP = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.Endereco.getRequiredFieldDescription(), maxlength: 150, enable: ko.observable(true), visible: ko.observable(false) });
    this.Usuario = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.Usuario.getRequiredFieldDescription(), maxlength: 50, enable: ko.observable(true), visible: ko.observable(false) });
    this.Senha = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.Senha.getRequiredFieldDescription(), maxlength: 50, enable: ko.observable(true), visible: ko.observable(false) });
    this.Porta = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.Porta.getRequiredFieldDescription(), maxlength: 10, def: "21", val: ko.observable("21"), enable: ko.observable(true), visible: ko.observable(false) });
    this.Diretorio = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.Diretorio.getFieldDescription(), maxlength: 400, enable: ko.observable(true), visible: ko.observable(false) });
    this.NomenclaturaArquivo = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.NomenclaturaDoArquivo.getFieldDescription(), maxlength: 100, enable: ko.observable(true), visible: ko.observable(false) });
    this.Passivo = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.FTPPassivo.getFieldDescription(), getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.UtilizarSFTP = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.SFTP, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.SSL = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.SSL, getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true), visible: ko.observable(false) });

    this.TestarConexaoFTP = PropertyEntity({ eventClick: function () { instancia.TestarConexaoFTP(); }, type: types.event, text: Localization.Resources.Configuracao.EmissaoCTe.TestarConexao, visible: ko.observable(true) });

    this.TagNumeroCTe = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.NomenclaturaArquivo.id, "#NumeroCTe"); }, type: types.event, text: Localization.Resources.Configuracao.EmissaoCTe.NumeroDoCTe, visible: ko.observable(false), enable: ko.observable(true) });
    this.TagSerieCTe = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.NomenclaturaArquivo.id, "#SerieCTe"); }, type: types.event, text: Localization.Resources.Configuracao.EmissaoCTe.SerieDoCTe, visible: ko.observable(false), enable: ko.observable(true) });
    this.TagChaveCTe = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.NomenclaturaArquivo.id, "#ChaveCTe"); }, type: types.event, text: Localization.Resources.Configuracao.EmissaoCTe.ChaveDoCTe, visible: ko.observable(false), enable: ko.observable(true) });
    this.TagCNPJEmissor = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.NomenclaturaArquivo.id, "#CNPJEmissor"); }, type: types.event, text: Localization.Resources.Configuracao.EmissaoCTe.CNPJDoEmissor, visible: ko.observable(false), enable: ko.observable(true) });
    this.TagCNPJTomador = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.NomenclaturaArquivo.id, "#CNPJTomador"); }, type: types.event, text: Localization.Resources.Configuracao.EmissaoCTe.CNPJDoTomador, visible: ko.observable(false), enable: ko.observable(true) });
    this.TagNumeroBooking = PropertyEntity({ eventClick: function () { InserirTag(instancia.Configuracao.NomenclaturaArquivo.id, "#NumeroBooking"); }, type: types.event, text: Localization.Resources.Configuracao.EmissaoCTe.NumeroBooking, visible: ko.observable(false), enable: ko.observable(true) });

    this.DescricaoComponenteFreteEmbarcador = PropertyEntity({ text: ko.observable(Localization.Resources.Configuracao.EmissaoCTe.DescricaoDoComponenteDeFreteLiquido.getFieldDescription()), maxlength: 30, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoEnvioEmail = PropertyEntity({ val: ko.observable(EnumTipoEnvioEmailCTe.Normal), options: EnumTipoEnvioEmailCTe.ObterOpcoes(), text: Localization.Resources.Configuracao.EmissaoCTe.TipoDeEnvioDoEmailAutomaticoDoCTe.getFieldDescription(), def: EnumTipoEnvioEmailCTe.Normal, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true), issue: 267 });
    this.ValorMaximoEmissaoPendentePagamento = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Configuracao.EmissaoCTe.ValorMaximoPendenteDePagamento.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 18, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });
    this.ValorLimiteFaturamento = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Configuracao.EmissaoCTe.ValorLimiteDeFaturamento.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 18, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });
    this.DiasEmAbertoAposVencimento = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Configuracao.EmissaoCTe.DiasEmAbertoAposVencimento.getFieldDescription(), getType: typesKnockout.int, maxlength: 18, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });

    this.TempoCarregamento = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.TempoCarregamento.getFieldDescription(), getType: typesKnockout.mask, mask: "99:99", visible: ko.observable(true) });
    this.TempoDescarregamento = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.TempoDescarregamento.getFieldDescription(), getType: typesKnockout.mask, mask: "99:99", visible: ko.observable(true) });
    this.AverbarCTeImportadoDoEmbarcador = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.AverbarCTeImportadoDoEmbarcador, getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });
    //Multimodal
    this.GridClientesBloquearEmissaoDosDestinatario = PropertyEntity({ type: types.local, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.ClientesBloquearEmissaoDosDestinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.AdicionarDestinatarioSerBloqueado, idBtnSearch: guid(), enable: ko.observable(enable), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-6 col-lg-6"), issue: 263, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });

    this.TipoPropostaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoPropostaMultimodal.Nenhum), options: EnumTipoPropostaMultimodal.obterOpcoes(), text: Localization.Resources.Configuracao.EmissaoCTe.TipoDaProposta.getFieldDescription(), def: EnumTipoPropostaMultimodal.Nenhum, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.TipoServicoMultimodal = PropertyEntity({ val: ko.observable(EnumTipoServicoMultimodal.Nenhum), options: (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal ? EnumTipoServicoMultimodal.obterOpcoesSVMTerceiro() : EnumTipoServicoMultimodal.obterOpcoes()), text: Localization.Resources.Configuracao.EmissaoCTe.TipoDoServico.getFieldDescription(), def: EnumTipoServicoMultimodal.Nenhum, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true) });
    this.ModalPropostaMultimodal = PropertyEntity({ val: ko.observable(EnumModalPropostaMultimodal.Nenhum), options: EnumModalPropostaMultimodal.obterOpcoes(), text: Localization.Resources.Configuracao.EmissaoCTe.ModalDaProposta.getFieldDescription(), def: EnumModalPropostaMultimodal.Nenhum, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.TipoCobrancaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoCobrancaMultimodal.Nenhum), options: (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal ? EnumTipoCobrancaMultimodal.obterOpcoesMultimodal() : EnumTipoCobrancaMultimodal.obterOpcoes()), text: Localization.Resources.Configuracao.EmissaoCTe.TipoDoModal.getFieldDescription(), def: EnumTipoCobrancaMultimodal.Nenhum, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(true) });

    this.BloquearEmissaoDeEntidadeSemCadastro = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(enable), text: Localization.Resources.Configuracao.EmissaoCTe.BloquearEmissaoSeExistirAlgumaEntidadeSemCadastro, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });
    this.BloquearEmissaoDosDestinatario = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(enable), text: Localization.Resources.Configuracao.EmissaoCTe.BloquearEmissaoCasoExistaAlgumDestinatarioAbaixo, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) });

    this.GerarOcorrenciaComplementoSubcontratacao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(enable), text: Localization.Resources.Configuracao.EmissaoCTe.GerarOcorrenciaAutomaticamenteParaComplementosDeCTesParaSubcontratacao, visible: ko.observable(_CONFIGURACAO_TMS.GerarOcorrenciaComplementoSubcontratacao === true) });
    this.TipoOcorrenciaComplementoSubcontratacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Configuracao.EmissaoCTe.TipoDeOcorrenciaDaSubcontratacao.getRequiredFieldDescription(), enable: ko.observable(enable), required: false, idBtnSearch: guid(), visible: ko.observable(true), issue: 0 });
    this.GerarSomenteUmaProvisaoCadaCargaCompleta = PropertyEntity({ val: ko.observable(false), def: false, text: "Gerar Somente Uma Provisao Cada Carga Completa", getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });

    this.TipoIntegracaoMercadoLivre = PropertyEntity({ val: ko.observable(EnumTipoIntegracaoMercadoLivre.HandlingUnit), options: EnumTipoIntegracaoMercadoLivre.obterOpcoes(), text: Localization.Resources.Configuracao.EmissaoCTe.TipoIntegracaoMercadoLivre.getFieldDescription(), def: EnumTipoIntegracaoMercadoLivre.HandlingUnit, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(false), issue: 0 });
    this.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente.getFieldDescription(), getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(true), issue: 0 });
    this.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Configuracao.EmissaoCTe.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente.getFieldDescription(), getType: typesKnockout.bool, enable: ko.observable(enable), visible: ko.observable(false), issue: 0 });
    this.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida = PropertyEntity({ val: ko.observable(EnumTipoTempoAcrescimoDecrescimoDataPrevisaoSaida.Acrescimo), options: EnumTipoTempoAcrescimoDecrescimoDataPrevisaoSaida.obterOpcoes(), text: Localization.Resources.Configuracao.EmissaoCTe.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida.getFieldDescription(), def: EnumTipoTempoAcrescimoDecrescimoDataPrevisaoSaida.Acrescimo, enable: ko.observable(enable), required: ko.observable(false), visible: ko.observable(false), issue: 0 });
    this.TempoAcrescimoDecrescimoDataPrevisaoSaida = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.TempoAcrescimoDecrescimoDataPrevisaoSaida.getFieldDescription(), getType: typesKnockout.mask, mask: "99:99", visible: ko.observable(false) });

    this.FatorCubagemRateioFormula = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.FatorDeCubagem, val: ko.observable(""), def: "", getType: typesKnockout.decimal, maxlength: 10, visible: ko.observable(false) });
    this.TipoUsoFatorCubagemRateioFormula = PropertyEntity({ text: Localization.Resources.Configuracao.EmissaoCTe.TipoDeUsoDoFatorDeCubagem, val: ko.observable(EnumTipoUsoFatorCubagem.UtilizarApenasQuandoMaiorQueOPesoDaMercadoria), options: EnumTupoUsoFatorCubagem.obterOpcoes(), def: EnumTipoUsoFatorCubagem.UtilizarApenasQuandoMaiorQueOPesoDaMercadoria, issue: 0, visible: ko.observable(false) });

    this.TipoPropostaMultimodal.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoServicoMultimodal.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ModalPropostaMultimodal.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoCobrancaMultimodal.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.BloquearEmissaoDeEntidadeSemCadastro.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.BloquearEmissaoDosDestinatario.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.ValorMaximoEmissaoPendentePagamento.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoEnvioEmail.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ValorLimiteFaturamento.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.DiasEmAbertoAposVencimento.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.FormulaRateioFrete.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            instancia.Configuracao.FormulaRateioFrete.codEntity(0);
    });

    this.FormulaRateioFrete.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoRateioDocumentos.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoEmissaoCTeParticipantes.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TipoEmissaoIntramunicipal.val.subscribe(function (novoValor) {
        instancia.RetornarValoresGerais(instancia);

        if (novoValor == EnumTipoEmissaoIntramunicipal.SempreNFSManual)
            instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.visible(true);
        else
            instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.visible(false);
    });

    this.TipoReceita.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TipoIntegracao.val.subscribe(function (novoValor) {
        instancia.RetornarValoresGerais(instancia);
        instancia.ChangeTipoIntegracao(novoValor);
    });

    this.CTeEmitidoNoEmbarcador.val.subscribe(function (novoValor) {
        instancia.RetornarValoresGerais(instancia);
        instancia.Configuracao.DescricaoComponenteFreteEmbarcador.text((novoValor ? "*" : "") + Localization.Resources.Configuracao.EmissaoCTe.DescricaoDoComponenteDeFreteLiquido.getFieldDescription());
        instancia.Configuracao.DescricaoComponenteFreteEmbarcador.required = false;
    });
    this.ModeloDocumentoFiscal.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); instancia.ModeloDocumentoFiscalChange(instancia); });
    this.DisponibilizarDocumentosParaNFsManual.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.EmpresaEmissora.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ComponentesFrete.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.ArquivoImportacaoNotasFiscais.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            instancia.Configuracao.ArquivoImportacaoNotasFiscais.codEntity(0);
    });

    this.ArquivoImportacaoNotasFiscais.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.EnderecoFTP.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Usuario.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Senha.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Porta.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Diretorio.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Passivo.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.AgruparMovimentoFinanceiroPorPedido.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.AverbarCTeImportadoDoEmbarcador.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.DescricaoComponenteFreteEmbarcador.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ExigirNumeroPedido.val.subscribe(function (novoValor) {
        instancia.RetornarValoresGerais(instancia);

        instancia.Configuracao.RegexValidacaoNumeroPedidoEmbarcador.visible(novoValor);
    });

    this.NaoValidarNotasFiscaisComDiferentesPortos.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.NaoValidarNotaFiscalExistente.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ValePedagioObrigatorio.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.NomenclaturaArquivo.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.UtilizarSFTP.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.SSL.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.UtilizarOutroModeloDocumentoEmissaoMunicipal.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ModeloDocumentoFiscalEmissaoMunicipal.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.NaoEmitirMDFe.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TornarPedidosPrioritarios.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.BloquearDiferencaValorFreteEmbarcador.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.EmitirComplementoDiferencaFreteEmbarcador.val.subscribe(function (novoValor) {
        instancia.RetornarValoresGerais(instancia);
        instancia.Configuracao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.required = novoValor;
    });

    this.GerarOcorrenciaSemTabelaFrete.val.subscribe(function (novoValor) {
        instancia.RetornarValoresGerais(instancia);
        instancia.Configuracao.TipoOcorrenciaSemTabelaFrete.required = novoValor;
    });
    this.GerarSomenteUmaProvisaoCadaCargaCompleta.val.subscribe(function (novoValor) {
        instancia.RetornarValoresGerais(instancia);
    });



    this.PercentualBloquearDiferencaValorFreteEmbarcador.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoOcorrenciaSemTabelaFrete.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ProvisionarDocumentos.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.GerarMDFeTransbordoSemConsiderarOrigem.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            instancia.Configuracao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.codEntity(0);
    });

    this.TipoOcorrenciaSemTabelaFrete.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            instancia.Configuracao.TipoOcorrenciaSemTabelaFrete.codEntity(0);
    });

    this.ImportarRedespachoIntermediario.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.EmitenteImportacaoRedespachoIntermediario.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.EmitenteImportacaoRedespachoIntermediario.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            instancia.Configuracao.EmitenteImportacaoRedespachoIntermediario.codEntity(0);
    });
    this.ExpedidorImportacaoRedespachoIntermediario.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ExpedidorImportacaoRedespachoIntermediario.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            instancia.Configuracao.ExpedidorImportacaoRedespachoIntermediario.codEntity(0);
    });
    this.RecebedorImportacaoRedespachoIntermediario.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.RecebedorImportacaoRedespachoIntermediario.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            instancia.Configuracao.RecebedorImportacaoRedespachoIntermediario.codEntity(0);
    });

    this.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoUsoFatorCubagemRateioFormula.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.FatorCubagemRateioFormula.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.DisponibilizarComposicaoRateioCarga.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.DescricaoItemPesoCTeSubcontratacao.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.CaracteristicaTransporteCTe.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.RegexValidacaoNumeroPedidoEmbarcador.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ObservacaoEmissaoCarga.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.Observacao.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ObservacaoTerceiro.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TipoOcorrenciaCTeEmitidoEmbarcador.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoOcorrenciaCTeEmitidoEmbarcador.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            instancia.Configuracao.TipoOcorrenciaCTeEmitidoEmbarcador.codEntity(0);
    });

    this.DisponibilizarDocumentosParaLoteEscrituracao.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.EscriturarSomenteDocumentosEmitidosParaNFe.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.DisponibilizarDocumentosParaPagamento.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.QuitarDocumentoAutomaticamenteAoGerarLote.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.GerarCIOTParaTodasAsCargas.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ValorFreteLiquidoDeveSerValorAReceber.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.ValorFreteLiquidoDeveSerValorAReceberSemICMS.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.GerarOcorrenciaComplementoSubcontratacao.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoOcorrenciaComplementoSubcontratacao.codEntity.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoOcorrenciaComplementoSubcontratacao.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            instancia.Configuracao.TipoOcorrenciaComplementoSubcontratacao.codEntity(0);
    });
    this.NaoPermitirVincularCTeComplementarEmCarga.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    if (!_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.GrupoPessoas_NaoPermitirAlterarValorLimiteFaturamento, _PermissoesPersonalizadas)))
        this.ValorLimiteFaturamento.enable(false);


    this.TempoCarregamento.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TempoDescarregamento.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.TipoIntegracaoMercadoLivre.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente.val.subscribe(function (novoValor) {
        if (novoValor) {
            instancia.Configuracao.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente.visible(true);
            instancia.Configuracao.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida.visible(true);
            instancia.Configuracao.TempoAcrescimoDecrescimoDataPrevisaoSaida.visible(true);
        }
        else {
            instancia.Configuracao.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente.visible(false);
            instancia.Configuracao.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida.visible(false);
            instancia.Configuracao.TempoAcrescimoDecrescimoDataPrevisaoSaida.visible(false);
        }

        instancia.RetornarValoresGerais(instancia);
    });

    this.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
    this.TempoAcrescimoDecrescimoDataPrevisaoSaida.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });

    this.ObrigatorioInformarMDFeEmitidoPeloEmbarcador.val.subscribe(function () { instancia.RetornarValoresGerais(instancia); });
};

function ConfiguracaoEmissaoCTe(idContent, knockoutConfiguracao, knockoutPessoa, knockoutGrupoPessoas, enable, callback, knoutEmpresa) {

    var instancia = this;
    var componentesConfig;
    instancia.PromiseLoad = new promise.Promise();

    if (enable == null)
        enable = true;

    this.Limpar = function () {
        LimparCampos(instancia.Configuracao);
        instancia.ApolicesSeguro = new Array();
        instancia.GridApoliceSeguro.CarregarGrid(instancia.ApolicesSeguro);
        instancia.ClientesBloqueados = new Array();
        instancia.GridClientesBloquearEmissaoDosDestinatario.CarregarGrid(instancia.ClientesBloqueados);
        instancia.Configuracao.ArquivoImportacaoNotasFiscais.enable(true);
        instancia.Configuracao.ModeloDocumentoFiscal.enable(false);
        instancia.Configuracao.ModeloDocumentoFiscal.required = false;
        instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.enable(false);
        instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.required = false;
        instancia.Configuracao.EmpresaEmissora.enable(false);
        instancia.Configuracao.EmpresaEmissora.required = false;
        instancia.Configuracao.FormulaRateioFrete.enable(true);
        instancia.Configuracao.TipoRateioDocumentos.enable(true);
        instancia.Configuracao.TipoEmissaoCTeParticipantes.enable(true);
        instancia.Configuracao.TipoEmissaoIntramunicipal.enable(true);
        instancia.Configuracao.TipoReceita.enable(true);
        instancia.Configuracao.CobrarOutroDocumento.enable(true);
        instancia.Configuracao.UtilizarOutroModeloDocumentoEmissaoMunicipal.enable(true);
        instancia.Configuracao.EmitirEmpresaFixa.enable(true);
        instancia.Configuracao.ComponentesFrete.enable(true);
        instancia.Configuracao.AvancarCargasDocumentosVinculados.visible(false);
        instancia.Configuracao.AverbarCTeImportadoDoEmbarcador.enable(true);
        instancia.Configuracao.TipoUsoFatorCubagemRateioFormula.visible(false);
        instancia.Configuracao.FatorCubagemRateioFormula.visible(false);
        instancia.RetornarValoresGerais(instancia);

        componentesConfig.LimparListaComponentes();

        $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_erro").addClass("hidden");
        $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_sucesso").addClass("hidden");
    };

    this.VisualizarCamposGrupoPessoa = function () {
        instancia.PromiseLoad.then(function () {
            instancia.Configuracao.TornarPedidosPrioritarios.visible(true);
        });
    };

    this.SetarValores = function (valores) {
        instancia.PromiseLoad.then(function () {
            instancia.ApolicesSeguro = valores.ApolicesSeguro;
            instancia.GridApoliceSeguro.CarregarGrid(instancia.ApolicesSeguro);
            
            instancia.ClientesBloqueados = valores.ClientesBloqueados;
            instancia.GridClientesBloquearEmissaoDosDestinatario.CarregarGrid(instancia.ClientesBloqueados);
            instancia.Configuracao.EnderecoFTP.val(valores.EnderecoFTP);
            instancia.Configuracao.Porta.val(valores.Porta);
            instancia.Configuracao.Diretorio.val(valores.Diretorio);
            instancia.Configuracao.Usuario.val(valores.Usuario);
            instancia.Configuracao.Senha.val(valores.Senha);
            instancia.Configuracao.Passivo.val(valores.Passivo);
            instancia.Configuracao.ArquivoImportacaoNotasFiscais.val(valores.ArquivoImportacaoNotasFiscais.Descricao);
            instancia.Configuracao.ArquivoImportacaoNotasFiscais.codEntity(valores.ArquivoImportacaoNotasFiscais.Codigo);
            instancia.Configuracao.FormulaRateioFrete.val(valores.FormulaRateioFrete.Descricao);
            instancia.Configuracao.FormulaRateioFrete.codEntity(valores.FormulaRateioFrete.Codigo);
            instancia.Configuracao.ModeloDocumentoFiscal.codEntity(valores.ModeloDocumentoFiscal.Codigo);
            instancia.Configuracao.ModeloDocumentoFiscal.val(valores.ModeloDocumentoFiscal.Descricao);
            instancia.Configuracao.DisponibilizarDocumentosParaNFsManual.val(valores.DisponibilizarDocumentosParaNFsManual);
            instancia.Configuracao.EmpresaEmissora.codEntity(valores.EmpresaEmissora.Codigo);
            instancia.Configuracao.EmpresaEmissora.val(valores.EmpresaEmissora.Descricao);
            instancia.Configuracao.TipoRateioDocumentos.val(valores.TipoRateioDocumentos);
            instancia.Configuracao.CobrarOutroDocumento.val(valores.CobrarOutroDocumento);
            instancia.Configuracao.EmitirEmpresaFixa.val(valores.EmitirEmpresaFixa);
            instancia.Configuracao.CTeEmitidoNoEmbarcador.val(valores.CTeEmitidoNoEmbarcador);
            instancia.Configuracao.TipoIntegracao.val(valores.TipoIntegracao);
            instancia.Configuracao.ComponentesFrete.val(valores.ComponentesFrete);
            instancia.Configuracao.TipoEmissaoCTeParticipantes.val(valores.TipoEmissaoCTeParticipantes);
            instancia.Configuracao.TipoEmissaoIntramunicipal.val(valores.TipoEmissaoIntramunicipal);
            instancia.Configuracao.TipoReceita.val(valores.TipoReceita);
            instancia.Configuracao.AgruparMovimentoFinanceiroPorPedido.val(valores.AgruparMovimentoFinanceiroPorPedido);
            instancia.Configuracao.ExigirNumeroPedido.val(valores.ExigirNumeroPedido);
            instancia.Configuracao.RegexValidacaoNumeroPedidoEmbarcador.val(valores.RegexValidacaoNumeroPedidoEmbarcador);
            instancia.Configuracao.NaoValidarNotaFiscalExistente.val(valores.NaoValidarNotaFiscalExistente);
            instancia.Configuracao.NaoValidarNotasFiscaisComDiferentesPortos.val(valores.NaoValidarNotasFiscaisComDiferentesPortos);
            instancia.Configuracao.ValePedagioObrigatorio.val(valores.ValePedagioObrigatorio);
            instancia.Configuracao.DescricaoComponenteFreteEmbarcador.val(valores.DescricaoComponenteFreteEmbarcador);
            instancia.Configuracao.NomenclaturaArquivo.val(valores.NomenclaturaArquivo);
            instancia.Configuracao.UtilizarSFTP.val(valores.UtilizarSFTP);
            instancia.Configuracao.SSL.val(valores.SSL);
            instancia.Configuracao.UtilizarOutroModeloDocumentoEmissaoMunicipal.val(valores.UtilizarOutroModeloDocumentoEmissaoMunicipal);
            instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.val(valores.ModeloDocumentoFiscalEmissaoMunicipal.Descricao);
            instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.codEntity(valores.ModeloDocumentoFiscalEmissaoMunicipal.Codigo);
            instancia.Configuracao.NaoEmitirMDFe.val(valores.NaoEmitirMDFe);
            instancia.Configuracao.TornarPedidosPrioritarios.val(valores.TornarPedidosPrioritarios);
            instancia.Configuracao.BloquearDiferencaValorFreteEmbarcador.val(valores.BloquearDiferencaValorFreteEmbarcador);
            instancia.Configuracao.EmitirComplementoDiferencaFreteEmbarcador.val(valores.EmitirComplementoDiferencaFreteEmbarcador);
            instancia.Configuracao.GerarOcorrenciaSemTabelaFrete.val(valores.GerarOcorrenciaSemTabelaFrete);
            instancia.Configuracao.LiberarDocumentosEmitidosQuandoEntregaForConfirmada.val(valores.LiberarDocumentosEmitidosQuandoEntregaForConfirmada);
            instancia.Configuracao.PercentualBloquearDiferencaValorFreteEmbarcador.val(valores.PercentualBloquearDiferencaValorFreteEmbarcador);
            instancia.Configuracao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.codEntity(valores.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.Codigo);
            instancia.Configuracao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.val(valores.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.Descricao);
            instancia.Configuracao.TipoOcorrenciaSemTabelaFrete.codEntity(valores.TipoOcorrenciaSemTabelaFrete.Codigo);
            instancia.Configuracao.TipoOcorrenciaSemTabelaFrete.val(valores.TipoOcorrenciaSemTabelaFrete.Descricao);
            instancia.Configuracao.TipoOcorrenciaCTeEmitidoEmbarcador.codEntity(valores.TipoOcorrenciaCTeEmitidoEmbarcador.Codigo);
            instancia.Configuracao.TipoOcorrenciaCTeEmitidoEmbarcador.val(valores.TipoOcorrenciaCTeEmitidoEmbarcador.Descricao);
            instancia.Configuracao.ProvisionarDocumentos.val(valores.ProvisionarDocumentos);
            instancia.Configuracao.GerarMDFeTransbordoSemConsiderarOrigem.val(valores.GerarMDFeTransbordoSemConsiderarOrigem);
            instancia.Configuracao.ImportarRedespachoIntermediario.val(valores.ImportarRedespachoIntermediario);
            instancia.Configuracao.EmitenteImportacaoRedespachoIntermediario.codEntity(valores.EmitenteImportacaoRedespachoIntermediario.Codigo);
            instancia.Configuracao.EmitenteImportacaoRedespachoIntermediario.val(valores.EmitenteImportacaoRedespachoIntermediario.Descricao);
            instancia.Configuracao.ExpedidorImportacaoRedespachoIntermediario.codEntity(valores.ExpedidorImportacaoRedespachoIntermediario.Codigo);
            instancia.Configuracao.ExpedidorImportacaoRedespachoIntermediario.val(valores.ExpedidorImportacaoRedespachoIntermediario.Descricao);
            instancia.Configuracao.RecebedorImportacaoRedespachoIntermediario.codEntity(valores.RecebedorImportacaoRedespachoIntermediario.Codigo);
            instancia.Configuracao.RecebedorImportacaoRedespachoIntermediario.val(valores.RecebedorImportacaoRedespachoIntermediario.Descricao);
            instancia.Configuracao.DescricaoItemPesoCTeSubcontratacao.val(valores.DescricaoItemPesoCTeSubcontratacao);
            instancia.Configuracao.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao.val(valores.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao);
            instancia.Configuracao.ObservacaoEmissaoCarga.val(valores.ObservacaoEmissaoCarga);
            instancia.Configuracao.Observacao.val(valores.Observacao);
            instancia.Configuracao.ObservacaoTerceiro.val(valores.ObservacaoTerceiro);
            instancia.Configuracao.CaracteristicaTransporteCTe.val(valores.CaracteristicaTransporteCTe);
            instancia.Configuracao.TipoEnvioEmail.val(valores.TipoEnvioEmail);
            instancia.Configuracao.ValorMaximoEmissaoPendentePagamento.val(valores.ValorMaximoEmissaoPendentePagamento);
            instancia.Configuracao.DisponibilizarDocumentosParaLoteEscrituracao.val(valores.DisponibilizarDocumentosParaLoteEscrituracao);
            instancia.Configuracao.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento.val(valores.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento);
            instancia.Configuracao.EscriturarSomenteDocumentosEmitidosParaNFe.val(valores.EscriturarSomenteDocumentosEmitidosParaNFe);
            instancia.Configuracao.TipoPropostaMultimodal.val(valores.TipoPropostaMultimodal);
            instancia.Configuracao.TipoServicoMultimodal.val(valores.TipoServicoMultimodal);
            instancia.Configuracao.ModalPropostaMultimodal.val(valores.ModalPropostaMultimodal);
            instancia.Configuracao.TipoCobrancaMultimodal.val(valores.TipoCobrancaMultimodal);
            instancia.Configuracao.BloquearEmissaoDeEntidadeSemCadastro.val(valores.BloquearEmissaoDeEntidadeSemCadastro);
            instancia.Configuracao.BloquearEmissaoDosDestinatario.val(valores.BloquearEmissaoDosDestinatario);
            instancia.Configuracao.DisponibilizarDocumentosParaPagamento.val(valores.DisponibilizarDocumentosParaPagamento);
            instancia.Configuracao.QuitarDocumentoAutomaticamenteAoGerarLote.val(valores.QuitarDocumentoAutomaticamenteAoGerarLote);
            instancia.Configuracao.GerarCIOTParaTodasAsCargas.val(valores.GerarCIOTParaTodasAsCargas);
            instancia.Configuracao.ValorFreteLiquidoDeveSerValorAReceber.val(valores.ValorFreteLiquidoDeveSerValorAReceber);
            instancia.Configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS.val(valores.ValorFreteLiquidoDeveSerValorAReceberSemICMS);
            instancia.Configuracao.GerarOcorrenciaComplementoSubcontratacao.val(valores.GerarOcorrenciaComplementoSubcontratacao);
            instancia.Configuracao.TipoOcorrenciaComplementoSubcontratacao.val(valores.TipoOcorrenciaComplementoSubcontratacao.Descricao);
            instancia.Configuracao.TipoOcorrenciaComplementoSubcontratacao.codEntity(valores.TipoOcorrenciaComplementoSubcontratacao.Codigo);
            instancia.Configuracao.ValorLimiteFaturamento.val(valores.ValorLimiteFaturamento);
            instancia.Configuracao.DiasEmAbertoAposVencimento.val(valores.DiasEmAbertoAposVencimento);
            instancia.Configuracao.NaoPermitirVincularCTeComplementarEmCarga.val(valores.NaoPermitirVincularCTeComplementarEmCarga);
            instancia.Configuracao.TempoCarregamento.val(valores.TempoCarregamento);
            instancia.Configuracao.TempoDescarregamento.val(valores.TempoDescarregamento);
            instancia.Configuracao.TipoIntegracaoMercadoLivre.val(valores.TipoIntegracaoMercadoLivre);
            instancia.Configuracao.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente.val(valores.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente);
            instancia.Configuracao.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente.val(valores.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente);
            instancia.Configuracao.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida.val(valores.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida);
            instancia.Configuracao.TempoAcrescimoDecrescimoDataPrevisaoSaida.val(valores.TempoAcrescimoDecrescimoDataPrevisaoSaida);
            instancia.Configuracao.ObrigatorioInformarMDFeEmitidoPeloEmbarcador.val(valores.ObrigatorioInformarMDFeEmitidoPeloEmbarcador);
            instancia.Configuracao.GerarSomenteUmaProvisaoCadaCargaCompleta.val(valores.GerarSomenteUmaProvisaoCadaCargaCompleta);
            instancia.Configuracao.DisponibilizarComposicaoRateioCarga.val(valores.DisponibilizarComposicaoRateioCarga);
            instancia.Configuracao.AverbarCTeImportadoDoEmbarcador.val(valores.AverbarCTeImportadoDoEmbarcador);
            instancia.Configuracao.TipoUsoFatorCubagemRateioFormula.val(valores.TipoUsoFatorCubagemRateioFormula);
            instancia.Configuracao.FatorCubagemRateioFormula.val(valores.FatorCubagemRateioFormula);

            componentesConfig.RecarregarGrid();

            VerificarHabilitaDesabilitaCamposEmissao(instancia,valores);

            instancia.RetornarValores(
                valores.FormulaRateioFrete.Codigo,
                valores.TipoRateioDocumentos,
                valores.ApolicesSeguro,
                valores.TipoIntegracao,
                valores.ModeloDocumentoFiscal.Codigo,
                valores.DisponibilizarDocumentosParaNFsManual,
                valores.ComponentesFrete,
                valores.TipoEmissaoCTeParticipantes,
                instancia.Configuracao.CTeEmitidoNoEmbarcador.val(),
                valores.EmpresaEmissora.Codigo,
                valores.ArquivoImportacaoNotasFiscais.Codigo,
                valores.EnderecoFTP, valores.Usuario,
                valores.Senha,
                valores.Porta,
                valores.Diretorio,
                valores.Passivo,
                valores.AgruparMovimentoFinanceiroPorPedido,
                valores.DescricaoComponenteFreteEmbarcador,
                valores.ExigirNumeroPedido,
                valores.NaoValidarNotaFiscalExistente,
                valores.TipoEmissaoIntramunicipal,                
                valores.ValePedagioObrigatorio,
                valores.NomenclaturaArquivo,
                valores.UtilizarSFTP,
                valores.SSL,
                valores.UtilizarOutroModeloDocumentoEmissaoMunicipal,
                valores.ModeloDocumentoFiscalEmissaoMunicipal.Codigo,
                valores.NaoEmitirMDFe,
                valores.TornarPedidosPrioritarios,
                valores.BloquearDiferencaValorFreteEmbarcador,
                valores.EmitirComplementoDiferencaFreteEmbarcador,
                valores.GerarOcorrenciaSemTabelaFrete,
                valores.PercentualBloquearDiferencaValorFreteEmbarcador,
                valores.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.Codigo,
                valores.TipoOcorrenciaSemTabelaFrete.Codigo,
                valores.ProvisionarDocumentos,
                valores.GerarMDFeTransbordoSemConsiderarOrigem,
                valores.ImportarRedespachoIntermediario,
                valores.EmitenteImportacaoRedespachoIntermediario.Codigo,
                valores.ExpedidorImportacaoRedespachoIntermediario.Codigo,
                valores.RecebedorImportacaoRedespachoIntermediario.Codigo,
                valores.DescricaoItemPesoCTeSubcontratacao,
                valores.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao,
                valores.RegexValidacaoNumeroPedidoEmbarcador,
                valores.ObservacaoEmissaoCarga,
                valores.CaracteristicaTransporteCTe,
                valores.TipoEnvioEmail,
                valores.ValorMaximoEmissaoPendentePagamento,
                valores.TipoOcorrenciaCTeEmitidoEmbarcador.Codigo,
                valores.DisponibilizarDocumentosParaLoteEscrituracao,
                valores.EscriturarSomenteDocumentosEmitidosParaNFe,
                valores.TipoPropostaMultimodal,
                valores.TipoServicoMultimodal,
                valores.ModalPropostaMultimodal,
                valores.TipoCobrancaMultimodal,
                valores.BloquearEmissaoDeEntidadeSemCadastro,
                valores.BloquearEmissaoDosDestinatario,
                valores.ClientesBloqueados,
                valores.DisponibilizarDocumentosParaPagamento,
                valores.QuitarDocumentoAutomaticamenteAoGerarLote,
                valores.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento,
                valores.Observacao,
                valores.ObservacaoTerceiro,
                valores.GerarCIOTParaTodasAsCargas,
                valores.ValorFreteLiquidoDeveSerValorAReceber,
                valores.GerarOcorrenciaComplementoSubcontratacao,
                valores.TipoOcorrenciaComplementoSubcontratacao.Codigo,
                valores.NaoValidarNotasFiscaisComDiferentesPortos,
                valores.ValorLimiteFaturamento,
                valores.DiasEmAbertoAposVencimento,
                valores.NaoPermitirVincularCTeComplementarEmCarga,
                valores.ValorFreteLiquidoDeveSerValorAReceberSemICMS,
                valores.TempoCarregamento,
                valores.TempoDescarregamento,
                valores.TipoIntegracaoMercadoLivre,
                valores.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente,
                valores.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente,
                valores.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida,
                valores.TempoAcrescimoDecrescimoDataPrevisaoSaida,
                valores.ObrigatorioInformarMDFeEmitidoPeloEmbarcador,
                valores.GerarSomenteUmaProvisaoCadaCargaCompleta,
                valores.DisponibilizarComposicaoRateioCarga,
                valores.AverbarCTeImportadoDoEmbarcador,
                valores.TipoUsoFatorCubagemRateioFormula,
                valores.FatorCubagemRateioFormula,
                valores.TipoReceita
            );
        });
    };

    this.RetornarValores = function (formulaRateioFrete, tipoRateioDocumentos, apolicesSeguro, tipoIntegracao, modeloDocumentoFiscal, disponibilizarDocumentosParaNFsManual, componentesFrete, TipoEmissaoCTeParticipantes, CTeEmitidoNoEmbarcador, empresaEmissora, arquivoImportacaoNotasFiscais, enderecoFTP, usuario, senha, porta, diretorio, passivo, agruparMovimentoFinanceiroPorPedido, descricaoComponenteFreteEmbarcador, exigirNumeroPedido, NaoValidarNotaFiscalExistente, tipoEmissaoIntramunicipal, valePedagioObrigatorio, nomenclaturaArquivo, utilizarSFTP, ssl, utilizarOutroModeloDocumentoEmissaoMunicipal, modeloDocumentoFiscalEmissaoMunicipal, naoEmitirMDFe, tornarPedidosPrioritarios, bloquearDiferencaValorFreteEmbarcador, emitirComplementoDiferencaFreteEmbarcador, gerarOcorrenciaSemTabelaFrete, percentualBloquearDiferencaValorFreteEmbarcador, tipoOcorrenciaComplementoDiferencaFreteEmbarcador, tipoOcorrenciaSemTabelaFrete, provisionarDocumentos, gerarMDFeTransbordoSemConsiderarOrigem, importarRedespachoIntermediario, emitenteImportacaoRedespachoIntermediario, expedidorImportacaoRedespachoIntermediario, recebedorImportacaoRedespachoIntermediario, descricaoItemPesoCTeSubcontratacao, utilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao, regexValidacaoNumeroPedidoEmbarcador, observacaoEmissaoCarga, CaracteristicaTransporteCTe, tipoEnvioEmail, valorMaximoEmissaoPendentePagamento, tipoOcorrenciaCTeEmitidoEmbarcador, disponibilizarDocumentosParaLoteEscrituracao, escriturarSomenteDocumentosEmitidosParaNFe, tipoPropostaMultimodal, tipoServicoMultimodal, modalPropostaMultimodal, tipoCobrancaMultimodal, bloquearEmissaoDeEntidadeSemCadastro, bloquearEmissaoDosDestinatario, clientesBloqueados, disponibilizarDocumentosParaPagamento, quitarDocumentoAutomaticamenteAoGerarLote, disponibilizarDocumentosParaLoteEscrituracaoCancelamento, observacao, observacaoTerceiro, gerarCIOTParaTodasAsCargas, valorFreteLiquidoDeveSerValorAReceber, gerarOcorrenciaComplementoSubcontratacao, tipoOcorrenciaComplementoSubcontratacao, naoValidarNotasFiscaisComDiferentesPortos, valorLimiteFaturamento, diasEmAbertoAposVencimento, naoPermitirVincularCTeComplementarEmCarga, valorFreteLiquidoDeveSerValorAReceberSemICMS, tempoCarregamento, tempoDescarregamento, tipoIntegracaoMercadoLivre, integracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente, integracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente, tipoTempoAcrescimoDecrescimoDataPrevisaoSaida, tempoAcrescimoDecrescimoDataPrevisaoSaida, obrigatorioInformarMDFeEmitidoPeloEmbarcador, gerarSomenteUmaProvisaoCadaCargaCompleta, disponibilizarComposicaoRateioCarga, averbarCTeImportadoDoEmbarcador, tipoUsoFatorCubagemRateioFormula, fatorCubagemRateioFormula, tipoReceita) {
        var codigosApolices = new Array();

        for (var i = 0; i < apolicesSeguro.length; i++)
            codigosApolices.push(apolicesSeguro[i].Codigo);

        var codigosClientes = new Array();

        for (var a = 0; i < clientesBloqueados.length; i++)
            codigosClientes.push(clientesBloqueados[a].Codigo);

        instancia.KnockoutConfiguracao.val(JSON.stringify({
            FormulaRateioFrete: formulaRateioFrete,
            TipoRateioDocumentos: tipoRateioDocumentos,
            TipoIntegracao: tipoIntegracao,
            ApolicesSeguro: codigosApolices,
            ModeloDocumentoFiscal: modeloDocumentoFiscal,
            DisponibilizarDocumentosParaNFsManual: disponibilizarDocumentosParaNFsManual,
            ComponentesFrete: componentesFrete,
            TipoEmissaoCTeParticipantes: TipoEmissaoCTeParticipantes,
            CTeEmitidoNoEmbarcador: CTeEmitidoNoEmbarcador,
            EmpresaEmissora: empresaEmissora,
            ArquivoImportacaoNotasFiscais: arquivoImportacaoNotasFiscais,
            EnderecoFTP: enderecoFTP,
            Usuario: usuario,
            Senha: senha,
            Porta: porta,
            Diretorio: diretorio,
            Passivo: passivo,
            AgruparMovimentoFinanceiroPorPedido: agruparMovimentoFinanceiroPorPedido,
            AverbarCTeImportadoDoEmbarcador: averbarCTeImportadoDoEmbarcador,
            DescricaoComponenteFreteEmbarcador: descricaoComponenteFreteEmbarcador,
            ExigirNumeroPedido: exigirNumeroPedido,
            NaoValidarNotaFiscalExistente: NaoValidarNotaFiscalExistente,
            NaoValidarNotasFiscaisComDiferentesPortos: naoValidarNotasFiscaisComDiferentesPortos,
            TipoEmissaoIntramunicipal: tipoEmissaoIntramunicipal,
            ValePedagioObrigatorio: valePedagioObrigatorio,
            NomenclaturaArquivo: nomenclaturaArquivo,
            UtilizarSFTP: utilizarSFTP,
            SSL: ssl,
            UtilizarOutroModeloDocumentoEmissaoMunicipal: utilizarOutroModeloDocumentoEmissaoMunicipal,
            ModeloDocumentoFiscalEmissaoMunicipal: modeloDocumentoFiscalEmissaoMunicipal,
            NaoEmitirMDFe: naoEmitirMDFe,
            TornarPedidosPrioritarios: tornarPedidosPrioritarios,
            BloquearDiferencaValorFreteEmbarcador: bloquearDiferencaValorFreteEmbarcador,
            EmitirComplementoDiferencaFreteEmbarcador: emitirComplementoDiferencaFreteEmbarcador,
            GerarOcorrenciaSemTabelaFrete: gerarOcorrenciaSemTabelaFrete,
            PercentualBloquearDiferencaValorFreteEmbarcador: Globalize.parseFloat(percentualBloquearDiferencaValorFreteEmbarcador),
            TipoOcorrenciaComplementoDiferencaFreteEmbarcador: tipoOcorrenciaComplementoDiferencaFreteEmbarcador,
            TipoOcorrenciaSemTabelaFrete: tipoOcorrenciaSemTabelaFrete,
            ProvisionarDocumentos: provisionarDocumentos,
            GerarMDFeTransbordoSemConsiderarOrigem: gerarMDFeTransbordoSemConsiderarOrigem,
            ImportarRedespachoIntermediario: importarRedespachoIntermediario,
            EmitenteImportacaoRedespachoIntermediario: emitenteImportacaoRedespachoIntermediario,
            ExpedidorImportacaoRedespachoIntermediario: expedidorImportacaoRedespachoIntermediario,
            RecebedorImportacaoRedespachoIntermediario: recebedorImportacaoRedespachoIntermediario,
            DescricaoItemPesoCTeSubcontratacao: descricaoItemPesoCTeSubcontratacao,
            UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao: utilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao,
            RegexValidacaoNumeroPedidoEmbarcador: regexValidacaoNumeroPedidoEmbarcador,
            ObservacaoEmissaoCarga: observacaoEmissaoCarga,
            CaracteristicaTransporteCTe: CaracteristicaTransporteCTe,
            TipoEnvioEmail: tipoEnvioEmail,
            ValorMaximoEmissaoPendentePagamento: valorMaximoEmissaoPendentePagamento,
            TipoOcorrenciaCTeEmitidoEmbarcador: tipoOcorrenciaCTeEmitidoEmbarcador,
            DisponibilizarDocumentosParaLoteEscrituracao: disponibilizarDocumentosParaLoteEscrituracao,
            EscriturarSomenteDocumentosEmitidosParaNFe: escriturarSomenteDocumentosEmitidosParaNFe,
            TipoPropostaMultimodal: tipoPropostaMultimodal,
            TipoServicoMultimodal: tipoServicoMultimodal,
            ModalPropostaMultimodal: modalPropostaMultimodal,
            TipoCobrancaMultimodal: tipoCobrancaMultimodal,
            BloquearEmissaoDeEntidadeSemCadastro: bloquearEmissaoDeEntidadeSemCadastro,
            BloquearEmissaoDosDestinatario: bloquearEmissaoDosDestinatario,
            ClientesBloqueados: codigosClientes,
            DisponibilizarDocumentosParaPagamento: disponibilizarDocumentosParaPagamento,
            QuitarDocumentoAutomaticamenteAoGerarLote: quitarDocumentoAutomaticamenteAoGerarLote,
            DisponibilizarDocumentosParaLoteEscrituracaoCancelamento: disponibilizarDocumentosParaLoteEscrituracaoCancelamento,
            Observacao: observacao,
            ObservacaoTerceiro: observacaoTerceiro,
            GerarCIOTParaTodasAsCargas: gerarCIOTParaTodasAsCargas,
            ValorFreteLiquidoDeveSerValorAReceber: valorFreteLiquidoDeveSerValorAReceber,
            GerarOcorrenciaComplementoSubcontratacao: gerarOcorrenciaComplementoSubcontratacao,
            TipoOcorrenciaComplementoSubcontratacao: tipoOcorrenciaComplementoSubcontratacao,
            ValorLimiteFaturamento: valorLimiteFaturamento,
            DiasEmAbertoAposVencimento: diasEmAbertoAposVencimento,
            NaoPermitirVincularCTeComplementarEmCarga: naoPermitirVincularCTeComplementarEmCarga,
            ValorFreteLiquidoDeveSerValorAReceberSemICMS: valorFreteLiquidoDeveSerValorAReceberSemICMS,
            TempoCarregamento: tempoCarregamento,
            TempoDescarregamento: tempoDescarregamento,
            TipoIntegracaoMercadoLivre: tipoIntegracaoMercadoLivre,
            IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente: integracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente,
            IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente: integracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente,
            TipoTempoAcrescimoDecrescimoDataPrevisaoSaida: tipoTempoAcrescimoDecrescimoDataPrevisaoSaida,
            TempoAcrescimoDecrescimoDataPrevisaoSaida: tempoAcrescimoDecrescimoDataPrevisaoSaida,
            ObrigatorioInformarMDFeEmitidoPeloEmbarcador: obrigatorioInformarMDFeEmitidoPeloEmbarcador,
            GerarSomenteUmaProvisaoCadaCargaCompleta: gerarSomenteUmaProvisaoCadaCargaCompleta,
            DisponibilizarComposicaoRateioCarga: disponibilizarComposicaoRateioCarga,
            FatorCubagemRateioFormula: fatorCubagemRateioFormula,
            TipoUsoFatorCubagemRateioFormula: tipoUsoFatorCubagemRateioFormula,
            TipoReceita: tipoReceita
        }));
    };

    this.CobrarOutroDocumentoClick = function () {
        if (instancia.Configuracao.CobrarOutroDocumento.val()) {
            instancia.Configuracao.ModeloDocumentoFiscal.required = true;
            instancia.Configuracao.ModeloDocumentoFiscal.enable(true);
        } else {
            instancia.Configuracao.ModeloDocumentoFiscal.enable(false);
            instancia.Configuracao.ModeloDocumentoFiscal.required = false;
            instancia.Configuracao.ModeloDocumentoFiscal.val("");
            instancia.Configuracao.ModeloDocumentoFiscal.codEntity(0);
        }
    };

    this.UtilizarOutroModeloDocumentoEmissaoMunicipalClick = function () {
        if (instancia.Configuracao.UtilizarOutroModeloDocumentoEmissaoMunicipal.val()) {
            instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.required = true;
            instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.enable(true);
        } else {
            instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.enable(false);
            instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.required = false;
            instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.val("");
            instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.codEntity(0);
        }
    };

    this.EmitirEmpresaFixaClick = function () {
        if (instancia.Configuracao.EmitirEmpresaFixa.val()) {
            instancia.Configuracao.EmpresaEmissora.required = true;
            instancia.Configuracao.EmpresaEmissora.enable(true);
        } else {
            instancia.Configuracao.EmpresaEmissora.enable(false);
            instancia.Configuracao.EmpresaEmissora.required = false;
            instancia.Configuracao.EmpresaEmissora.val("");
            instancia.Configuracao.EmpresaEmissora.codEntity(0);
        }
    };

    this.CTeEmitidoNoEmbarcadorClick = function () {
        VerificarHabilitaDesabilitaCamposEmissao(instancia);
    };

    this.ObterTiposIntegracao = function () {
        var p = new promise.Promise();

        executarReST("TipoIntegracao/BuscarTodos", {
            Tipos: JSON.stringify([
                EnumTipoIntegracao.Avior,
                EnumTipoIntegracao.Avon,
                EnumTipoIntegracao.NaoInformada,
                EnumTipoIntegracao.NaoPossuiIntegracao,
                EnumTipoIntegracao.Natura,
                EnumTipoIntegracao.FTP,
                EnumTipoIntegracao.GPAEscrituracaoCTe,
                EnumTipoIntegracao.UnileverFourKites,
                EnumTipoIntegracao.MercadoLivre,
                EnumTipoIntegracao.Intercement,
                EnumTipoIntegracao.Michelin,
                EnumTipoIntegracao.Dansales,
                EnumTipoIntegracao.InforDoc,
                EnumTipoIntegracao.Magalu,
                EnumTipoIntegracao.LoggiFaturas,
                EnumTipoIntegracao.Calisto,
            ])
        }, function (r) {
            if (r.Success) {
                _configuracaoEmissaoCTeOpcoesTipoIntegracao = new Array();

                for (var i = 0; i < r.Data.length; i++)
                    _configuracaoEmissaoCTeOpcoesTipoIntegracao.push({ value: r.Data[i].Codigo, text: r.Data[i].Descricao });
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }

            p.done();
        });

        return p;
    };

    this.DesabilitarEmissaoCTe = function () {
        if (instancia.GridApoliceSeguro != null)
            instancia.GridApoliceSeguro.CarregarGrid(instancia.ApolicesSeguro, false);

        if (instancia.GridClientesBloquearEmissaoDosDestinatario != null)
            instancia.GridClientesBloquearEmissaoDosDestinatario.CarregarGrid(instancia.ClientesBloqueados, false);

        if (instancia.Configuracao != null) {
            instancia.Configuracao.FormulaRateioFrete.enable(false);
            instancia.Configuracao.TipoRateioDocumentos.enable(false);
            instancia.Configuracao.ApoliceSeguro.enable(false);
            instancia.Configuracao.ClientesBloquearEmissaoDosDestinatario.enable(false);
            instancia.Configuracao.TipoIntegracao.enable(false);
            instancia.Configuracao.ArquivoImportacaoNotasFiscais.enable(false);
        }
    };

    this.Load = function () {
        LoadLocalizationResources("Configuracao.EmissaoCTe").then(function () {

            instancia.Configuracao = new ConfiguracaoEmissaoCTeModel(instancia, enable);
            instancia.ApolicesSeguro = new Array();
            instancia.ClientesBloqueados = new Array();
            instancia.KnockoutConfiguracao = knockoutConfiguracao;

            $.get("Content/Static/Configuracao/EmissaoCTe.html?dyn=" + guid(), function (data) {

                $("#" + idContent).html(data.replace(/#ComponentesFrete/g, instancia.Configuracao.ComponentesFrete.id));

                var configuracaoObservacaoCTe = new ConfiguracaoObservacaoCTe();

                var parametrosConfiguracaoObservacaoCTe = {
                    Knouckout: instancia.Configuracao,
                    KnoutObservacao: instancia.Configuracao.Observacao,
                    KnoutObservacaoTerceiro: instancia.Configuracao.ObservacaoTerceiro,
                    IdContainerObservacao: "divContainerObservacao",
                    IdContainerObservacaoTerceiro: "divContainerObservacaoTerceiro"
                };

                configuracaoObservacaoCTe.Load(parametrosConfiguracaoObservacaoCTe).then(function () {

                    KoBindings(instancia.Configuracao, idContent);

                    componentesConfig = new ConfiguracaoComponentesDeFrete(instancia.Configuracao.ComponentesFrete.id, enable, instancia.Configuracao.ComponentesFrete, function () {
                        new BuscarRateioFormulas(instancia.Configuracao.FormulaRateioFrete, function (r) {
                            if (r != null) {
                                instancia.Configuracao.FormulaRateioFrete.val(r.Descricao);
                                instancia.Configuracao.FormulaRateioFrete.codEntity(r.Codigo);
                                if (r.ParametroRateioFormula == EnumParametroRateioFormula.PorPesoUtilizandoFatorCubagem) {
                                    instancia.Configuracao.FatorCubagemRateioFormula.visible(true);
                                    instancia.Configuracao.TipoUsoFatorCubagemRateioFormula.visible(true);
                                }
                                else {
                                    instancia.Configuracao.FatorCubagemRateioFormula.visible(false);
                                    instancia.Configuracao.TipoUsoFatorCubagemRateioFormula.visible(false);
                                }
                                instancia.RetornarValoresGerais(instancia);
                            }
                        });
                        var menuOpcoes = {
                            tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
                                descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                                    for (var i = 0; i < instancia.ApolicesSeguro.length; i++) {
                                        if (instancia.ApolicesSeguro[i].Codigo == data.Codigo) {
                                            instancia.ApolicesSeguro.splice(i, 1);
                                            instancia.GridApoliceSeguro.CarregarGrid(instancia.ApolicesSeguro);
                                            instancia.RetornarValoresGerais(instancia);
                                            break;
                                        }
                                    }
                                }
                            }]
                        };

                        if (!enable)
                            menuOpcoes = null;

                        var header = [
                            { data: "Codigo", visible: false },
                            { data: "Seguradora", title: Localization.Resources.Configuracao.EmissaoCTe.Seguradora, width: "20%" },
                            { data: "NumeroApolice", title: Localization.Resources.Configuracao.EmissaoCTe.Apolice, width: "15%" },
                            { data: "NumeroAverbacao", title: Localization.Resources.Configuracao.EmissaoCTe.Averbacao, width: "15%" },
                            { data: "Responsavel", title: Localization.Resources.Configuracao.EmissaoCTe.Responsavel, width: "15%" },
                            { data: "Vigencia", title: Localization.Resources.Configuracao.EmissaoCTe.Vigencia, width: "20%" }
                        ];

                        instancia.GridApoliceSeguro = new BasicDataTable(instancia.Configuracao.GridApoliceSeguro.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

                        new BuscarApolicesSeguro(instancia.Configuracao.ApoliceSeguro, knockoutGrupoPessoas, knockoutPessoa, instancia.GridApoliceSeguro, function (r) {
                            if (r != null) {
                                for (var i = 0; i < r.length; i++)
                                    instancia.ApolicesSeguro.push({
                                        Codigo: r[i].Codigo,
                                        Seguradora: r[i].Seguradora,
                                        NumeroApolice: r[i].NumeroApolice,
                                        NumeroAverbacao: r[i].NumeroAverbacao,
                                        Responsavel: r[i].Responsavel,
                                        Vigencia: r[i].InicioVigencia + Localization.Resources.Configuracao.EmissaoCTe.Ate + r[i].FimVigencia
                                    });

                                instancia.GridApoliceSeguro.CarregarGrid(instancia.ApolicesSeguro);

                                instancia.RetornarValoresGerais(instancia);
                            }
                        }, true, _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS ? knoutEmpresa : null, true);

                        instancia.Configuracao.ApoliceSeguro.basicTable = instancia.GridApoliceSeguro;
                        instancia.GridApoliceSeguro.CarregarGrid(instancia.ApolicesSeguro);

                        //if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
                        //instancia.Configuracao.TipoServicoMultimodal.visible(false);

                        var menuOpcoesBloqueioDestinatario = {
                            tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
                                descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                                    for (var i = 0; i < instancia.ClientesBloqueados.length; i++) {
                                        if (instancia.ClientesBloqueados[i].Codigo == data.Codigo) {
                                            instancia.ClientesBloqueados.splice(i, 1);
                                            instancia.GridClientesBloquearEmissaoDosDestinatario.CarregarGrid(instancia.ClientesBloqueados);
                                            instancia.RetornarValoresGerais(instancia);
                                            break;
                                        }
                                    }
                                }
                            }]
                        };

                        if (!enable)
                            menuOpcoesBloqueioDestinatario = null;

                        var headerBloqueioDestinatario = [
                            { data: "Codigo", visible: false },
                            { data: "CPF_CNPJ", title: Localization.Resources.Configuracao.EmissaoCTe.CNPJCPF, width: "20%" },
                            { data: "Nome", title: Localization.Resources.Configuracao.EmissaoCTe.RazaoSocial, width: "60%" }
                        ];

                        instancia.GridClientesBloquearEmissaoDosDestinatario = new BasicDataTable(instancia.Configuracao.GridClientesBloquearEmissaoDosDestinatario.id, headerBloqueioDestinatario, menuOpcoesBloqueioDestinatario, { column: 1, dir: orderDir.asc }, null, 5);

                        new BuscarClientes(instancia.Configuracao.ClientesBloquearEmissaoDosDestinatario, function (r) {
                            if (r != null) {
                                for (var i = 0; i < r.length; i++)
                                    instancia.ClientesBloqueados.push({
                                        Codigo: r[i].Codigo,
                                        CPF_CNPJ: r[i].CPF_CNPJ,
                                        Nome: r[i].Nome
                                    });

                                instancia.GridClientesBloquearEmissaoDosDestinatario.CarregarGrid(instancia.ClientesBloqueados);

                                instancia.RetornarValoresGerais(instancia);
                            }
                        }, null, null, null, instancia.GridClientesBloquearEmissaoDosDestinatario);

                        instancia.Configuracao.ClientesBloquearEmissaoDosDestinatario.basicTable = instancia.GridClientesBloquearEmissaoDosDestinatario;
                        instancia.GridClientesBloquearEmissaoDosDestinatario.CarregarGrid(instancia.ClientesBloqueados);

                        instancia.RetornarValoresGerais(instancia);

                        $("#" + instancia.Configuracao.UtilizarOutroModeloDocumentoEmissaoMunicipal.id).click(instancia.UtilizarOutroModeloDocumentoEmissaoMunicipalClick);
                        $("#" + instancia.Configuracao.CobrarOutroDocumento.id).click(instancia.CobrarOutroDocumentoClick);
                        $("#" + instancia.Configuracao.EmitirEmpresaFixa.id).click(instancia.EmitirEmpresaFixaClick);
                        $("#" + instancia.Configuracao.CTeEmitidoNoEmbarcador.id).click(instancia.CTeEmitidoNoEmbarcadorClick);

                        new BuscarModeloDocumentoFiscal(instancia.Configuracao.ModeloDocumentoFiscal, null, null, true);
                        new BuscarModeloDocumentoFiscal(instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal, null, null, true);
                        new BuscarTransportadores(instancia.Configuracao.EmpresaEmissora, null, null, true);
                        new BuscarArquivoImportacaoNotasFiscais(instancia.Configuracao.ArquivoImportacaoNotasFiscais);
                        new BuscarTipoOcorrencia(instancia.Configuracao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador);
                        new BuscarTipoOcorrencia(instancia.Configuracao.TipoOcorrenciaSemTabelaFrete);
                        new BuscarTipoOcorrencia(instancia.Configuracao.TipoOcorrenciaCTeEmitidoEmbarcador);
                        new BuscarClientes(instancia.Configuracao.EmitenteImportacaoRedespachoIntermediario);
                        new BuscarClientes(instancia.Configuracao.ExpedidorImportacaoRedespachoIntermediario);
                        new BuscarClientes(instancia.Configuracao.RecebedorImportacaoRedespachoIntermediario);
                        new BuscarTipoOcorrencia(instancia.Configuracao.TipoOcorrenciaComplementoSubcontratacao);

                        LocalizeCurrentPage();

                        instancia.PromiseLoad.done();
                    });
                });
            });
        });

        return instancia.PromiseLoad;
    };

    this.ChangeTipoIntegracao = function (tipo) {
        if (tipo == EnumTipoIntegracao.MercadoLivre) {
            instancia.Configuracao.EnderecoFTP.visible(false);
            instancia.Configuracao.Usuario.visible(false);
            instancia.Configuracao.Senha.visible(false);
            instancia.Configuracao.Porta.visible(false);
            instancia.Configuracao.Diretorio.visible(false);
            instancia.Configuracao.Passivo.visible(false);
            instancia.Configuracao.UtilizarSFTP.visible(false);
            instancia.Configuracao.SSL.visible(false);
            instancia.Configuracao.TestarConexaoFTP.visible(false);
            instancia.Configuracao.NomenclaturaArquivo.visible(false);
            instancia.Configuracao.TagChaveCTe.visible(false);
            instancia.Configuracao.TagNumeroCTe.visible(false);
            instancia.Configuracao.TagSerieCTe.visible(false);
            instancia.Configuracao.TagCNPJEmissor.visible(false);
            instancia.Configuracao.TagCNPJTomador.visible(false);
            instancia.Configuracao.TagNumeroBooking.visible(false);
            instancia.Configuracao.TipoIntegracaoMercadoLivre.visible(false);
            instancia.Configuracao.TipoIntegracaoMercadoLivre.visible(true);
        } else if (tipo == EnumTipoIntegracao.FTP) {
            instancia.Configuracao.EnderecoFTP.visible(true);
            instancia.Configuracao.Usuario.visible(true);
            instancia.Configuracao.Senha.visible(true);
            instancia.Configuracao.Porta.visible(true);
            instancia.Configuracao.Diretorio.visible(true);
            instancia.Configuracao.Passivo.visible(true);
            instancia.Configuracao.UtilizarSFTP.visible(true);
            instancia.Configuracao.SSL.visible(true);
            instancia.Configuracao.TestarConexaoFTP.visible(true);
            instancia.Configuracao.NomenclaturaArquivo.visible(true);
            instancia.Configuracao.TagChaveCTe.visible(true);
            instancia.Configuracao.TagNumeroCTe.visible(true);
            instancia.Configuracao.TagSerieCTe.visible(true);
            instancia.Configuracao.TagCNPJEmissor.visible(true);
            instancia.Configuracao.TagCNPJTomador.visible(true);
            instancia.Configuracao.TagNumeroBooking.visible(true);
            instancia.Configuracao.TipoIntegracaoMercadoLivre.visible(false);
        } else {
            instancia.Configuracao.EnderecoFTP.visible(false);
            instancia.Configuracao.Usuario.visible(false);
            instancia.Configuracao.Senha.visible(false);
            instancia.Configuracao.Porta.visible(false);
            instancia.Configuracao.Diretorio.visible(false);
            instancia.Configuracao.Passivo.visible(false);
            instancia.Configuracao.UtilizarSFTP.visible(false);
            instancia.Configuracao.SSL.visible(false);
            instancia.Configuracao.TestarConexaoFTP.visible(false);
            instancia.Configuracao.NomenclaturaArquivo.visible(false);
            instancia.Configuracao.TagChaveCTe.visible(false);
            instancia.Configuracao.TagNumeroCTe.visible(false);
            instancia.Configuracao.TagSerieCTe.visible(false);
            instancia.Configuracao.TagCNPJEmissor.visible(false);
            instancia.Configuracao.TagCNPJTomador.visible(false);
            instancia.Configuracao.TagNumeroBooking.visible(false);
            instancia.Configuracao.TipoIntegracaoMercadoLivre.visible(false);
        }
    };

    this.ModeloDocumentoFiscalChange = function (instancia) {
        if (instancia.Configuracao.ModeloDocumentoFiscal.codEntity()) {
            instancia.Configuracao.DisponibilizarDocumentosParaNFsManual.visible(true);
        } else {
            instancia.Configuracao.DisponibilizarDocumentosParaNFsManual.visible(false);
            instancia.Configuracao.DisponibilizarDocumentosParaNFsManual.val(false);
        }
    }

    this.TestarConexaoFTP = function () {
        executarReST("FTP/TestarConexao", {
            Host: instancia.Configuracao.EnderecoFTP.val(),
            Porta: instancia.Configuracao.Porta.val(),
            Diretorio: instancia.Configuracao.Diretorio.val(),
            Usuario: instancia.Configuracao.Usuario.val(),
            Senha: instancia.Configuracao.Senha.val(),
            Passivo: instancia.Configuracao.Passivo.val(),
            UtilizarSFTP: instancia.Configuracao.UtilizarSFTP.val(),
            SSL: instancia.Configuracao.SSL.val()
        }, function (r) {
            if (r.Success) {
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_sucesso").removeClass("hidden");
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_erro").addClass("hidden");
            } else {
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_sucesso").addClass("hidden");
                $("#" + instancia.Configuracao.TestarConexaoFTP.id + "_erro").removeClass("hidden");
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    };

    instancia.ObterTiposIntegracao().then(function () {
        return instancia.Load();
    }).then(function () {
        if (callback != null)
            callback();
    });
}

function VerificarHabilitaDesabilitaCamposEmissao(instancia, valores) {
    if (instancia.Configuracao.CTeEmitidoNoEmbarcador.val()) {
        instancia.Configuracao.FormulaRateioFrete.enable(false);
        instancia.Configuracao.TipoRateioDocumentos.enable(false);
        instancia.Configuracao.TipoEmissaoCTeParticipantes.enable(false);
        instancia.Configuracao.TipoEmissaoIntramunicipal.enable(false);
        instancia.Configuracao.TipoReceita.enable(false);
        instancia.Configuracao.CobrarOutroDocumento.enable(false);
        instancia.Configuracao.EmitirEmpresaFixa.enable(false);
        instancia.Configuracao.ModeloDocumentoFiscal.enable(false);
        instancia.Configuracao.EmpresaEmissora.enable(false);
        instancia.Configuracao.ComponentesFrete.enable(false);
        instancia.Configuracao.ArquivoImportacaoNotasFiscais.enable(false);
        instancia.Configuracao.EnderecoFTP.enable(false);
        instancia.Configuracao.Usuario.enable(false);
        instancia.Configuracao.Senha.enable(false);
        instancia.Configuracao.Porta.enable(false);
        instancia.Configuracao.Diretorio.enable(false);
        instancia.Configuracao.Passivo.enable(false);
        instancia.Configuracao.UtilizarSFTP.enable(false);
        instancia.Configuracao.SSL.enable(false);
        instancia.Configuracao.NomenclaturaArquivo.enable(false);
        instancia.Configuracao.TagChaveCTe.enable(false);
        instancia.Configuracao.TagNumeroCTe.enable(false);
        instancia.Configuracao.TagSerieCTe.enable(false);
        instancia.Configuracao.TagCNPJEmissor.enable(false);
        instancia.Configuracao.TagCNPJTomador.enable(false);
        instancia.Configuracao.TagNumeroBooking.enable(false);
        instancia.Configuracao.NaoValidarNotaFiscalExistente.enable(false);
        instancia.Configuracao.NaoValidarNotasFiscaisComDiferentesPortos.enable(false);
        instancia.Configuracao.ValePedagioObrigatorio.enable(false);
        instancia.Configuracao.UtilizarOutroModeloDocumentoEmissaoMunicipal.enable(false);
        instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.enable(false);
        instancia.Configuracao.NaoEmitirMDFe.enable(false);
        instancia.Configuracao.BloquearDiferencaValorFreteEmbarcador.enable(false);
        instancia.Configuracao.EmitirComplementoDiferencaFreteEmbarcador.enable(false);
        instancia.Configuracao.GerarOcorrenciaSemTabelaFrete.enable(false);
        instancia.Configuracao.PercentualBloquearDiferencaValorFreteEmbarcador.enable(false);
        instancia.Configuracao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.enable(false);
        instancia.Configuracao.TipoOcorrenciaSemTabelaFrete.enable(false);
        instancia.Configuracao.ProvisionarDocumentos.enable(false);
        instancia.Configuracao.GerarMDFeTransbordoSemConsiderarOrigem.enable(false);
        instancia.Configuracao.ImportarRedespachoIntermediario.enable(false);
        instancia.Configuracao.EmitenteImportacaoRedespachoIntermediario.enable(false);
        instancia.Configuracao.ExpedidorImportacaoRedespachoIntermediario.enable(false);
        instancia.Configuracao.RecebedorImportacaoRedespachoIntermediario.enable(false);
        instancia.Configuracao.DescricaoItemPesoCTeSubcontratacao.enable(false);
        instancia.Configuracao.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao.enable(false);
        instancia.Configuracao.RegexValidacaoNumeroPedidoEmbarcador.enable(false);
        instancia.Configuracao.ObservacaoEmissaoCarga.enable(false);
        instancia.Configuracao.Observacao.enable(false);
        instancia.Configuracao.ObservacaoTerceiro.enable(false);
        instancia.Configuracao.CaracteristicaTransporteCTe.enable(false);
        instancia.Configuracao.TipoOcorrenciaCTeEmitidoEmbarcador.visible(true);
        instancia.Configuracao.TipoOcorrenciaCTeEmitidoEmbarcador.enable(true);
        instancia.Configuracao.DisponibilizarDocumentosParaLoteEscrituracao.enable(false);
        instancia.Configuracao.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento.enable(false);
        instancia.Configuracao.EscriturarSomenteDocumentosEmitidosParaNFe.enable(false);
        instancia.Configuracao.DisponibilizarDocumentosParaPagamento.enable(false);
        instancia.Configuracao.QuitarDocumentoAutomaticamenteAoGerarLote.enable(false);
        instancia.Configuracao.GerarCIOTParaTodasAsCargas.enable(false);
        instancia.Configuracao.ValorFreteLiquidoDeveSerValorAReceber.enable(true);
        instancia.Configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS.enable(true);
        instancia.Configuracao.GerarOcorrenciaComplementoSubcontratacao.enable(false);
        instancia.Configuracao.TipoOcorrenciaComplementoSubcontratacao.enable(false);
        instancia.Configuracao.NaoPermitirVincularCTeComplementarEmCarga.visible(true);
        instancia.Configuracao.TipoIntegracaoMercadoLivre.enable(false);
        instancia.Configuracao.ObrigatorioInformarMDFeEmitidoPeloEmbarcador.visible(true);
        instancia.Configuracao.GerarSomenteUmaProvisaoCadaCargaCompleta.enable(false);
        instancia.Configuracao.AvancarCargasDocumentosVinculados.visible(true && _CONFIGURACAO_TMS.PermitirAvancarCargasEmitidasEmbarcadorPorTipoOperacao);
        instancia.Configuracao.AvancarCargasDocumentosVinculados.enable(true && _CONFIGURACAO_TMS.PermitirAvancarCargasEmitidasEmbarcadorPorTipoOperacao);
        instancia.Configuracao.FatorCubagemRateioFormula.visible(false);
        instancia.Configuracao.TipoUsoFatorCubagemRateioFormula.visible(false);
    } else {

        if (valores != null) {
            if (valores.FormulaRateioFrete.ParametroRateioFormula == EnumParametroRateioFormula.PorPesoUtilizandoFatorCubagem) {
                instancia.Configuracao.FatorCubagemRateioFormula.visible(true);
                instancia.Configuracao.TipoUsoFatorCubagemRateioFormula.visible(true);
            }
            else {
                instancia.Configuracao.FatorCubagemRateioFormula.visible(false);
                instancia.Configuracao.TipoUsoFatorCubagemRateioFormula.visible(false);
            }
        }


        instancia.Configuracao.FormulaRateioFrete.enable(true);
        instancia.Configuracao.TipoRateioDocumentos.enable(true);
        instancia.Configuracao.TipoEmissaoCTeParticipantes.enable(true);
        instancia.Configuracao.TipoEmissaoIntramunicipal.enable(true);
        instancia.Configuracao.TipoReceita.enable(true);
        instancia.Configuracao.CobrarOutroDocumento.enable(true);
        instancia.Configuracao.EmitirEmpresaFixa.enable(true);
        instancia.Configuracao.ArquivoImportacaoNotasFiscais.enable(true);
        instancia.Configuracao.EnderecoFTP.enable(true);
        instancia.Configuracao.Usuario.enable(true);
        instancia.Configuracao.Senha.enable(true);
        instancia.Configuracao.Porta.enable(true);
        instancia.Configuracao.Diretorio.enable(true);
        instancia.Configuracao.Passivo.enable(true);
        instancia.Configuracao.UtilizarSFTP.enable(true);
        instancia.Configuracao.SSL.enable(true);
        instancia.Configuracao.NomenclaturaArquivo.enable(true);
        instancia.Configuracao.TagChaveCTe.enable(true);
        instancia.Configuracao.TagNumeroCTe.enable(true);
        instancia.Configuracao.TagSerieCTe.enable(true);
        instancia.Configuracao.TagCNPJEmissor.enable(true);
        instancia.Configuracao.TagCNPJTomador.enable(true);
        instancia.Configuracao.TagNumeroBooking.enable(true);
        instancia.Configuracao.NaoValidarNotaFiscalExistente.enable(true);
        instancia.Configuracao.NaoValidarNotasFiscaisComDiferentesPortos.enable(true);
        instancia.Configuracao.ValePedagioObrigatorio.enable(true);
        instancia.Configuracao.UtilizarOutroModeloDocumentoEmissaoMunicipal.enable(true);
        instancia.Configuracao.NaoEmitirMDFe.enable(true);
        instancia.Configuracao.BloquearDiferencaValorFreteEmbarcador.enable(true);
        instancia.Configuracao.EmitirComplementoDiferencaFreteEmbarcador.enable(true);
        instancia.Configuracao.GerarOcorrenciaSemTabelaFrete.enable(true);
        instancia.Configuracao.PercentualBloquearDiferencaValorFreteEmbarcador.enable(true);
        instancia.Configuracao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.enable(true);
        instancia.Configuracao.TipoOcorrenciaSemTabelaFrete.enable(true);
        instancia.Configuracao.ProvisionarDocumentos.enable(true);
        instancia.Configuracao.GerarMDFeTransbordoSemConsiderarOrigem.enable(true);
        instancia.Configuracao.ImportarRedespachoIntermediario.enable(true);
        instancia.Configuracao.EmitenteImportacaoRedespachoIntermediario.enable(true);
        instancia.Configuracao.ExpedidorImportacaoRedespachoIntermediario.enable(true);
        instancia.Configuracao.RecebedorImportacaoRedespachoIntermediario.enable(true);
        instancia.Configuracao.DescricaoItemPesoCTeSubcontratacao.enable(true);
        instancia.Configuracao.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao.enable(true);
        instancia.Configuracao.CaracteristicaTransporteCTe.enable(true);
        instancia.Configuracao.DisponibilizarDocumentosParaLoteEscrituracao.enable(true);
        instancia.Configuracao.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento.enable(true);
        instancia.Configuracao.EscriturarSomenteDocumentosEmitidosParaNFe.enable(true);
        instancia.Configuracao.DisponibilizarDocumentosParaPagamento.enable(true);
        instancia.Configuracao.QuitarDocumentoAutomaticamenteAoGerarLote.enable(true);
        instancia.Configuracao.GerarCIOTParaTodasAsCargas.enable(true);
        instancia.Configuracao.ValorFreteLiquidoDeveSerValorAReceber.enable(false);
        instancia.Configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS.enable(false);
        instancia.Configuracao.ObrigatorioInformarMDFeEmitidoPeloEmbarcador.visible(false);

        if (instancia.Configuracao.CobrarOutroDocumento.val())
            instancia.Configuracao.ModeloDocumentoFiscal.enable(true);

        if (instancia.Configuracao.UtilizarOutroModeloDocumentoEmissaoMunicipal.val())
            instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.enable(true);

        if (instancia.Configuracao.EmitirEmpresaFixa.val())
            instancia.Configuracao.EmpresaEmissora.enable(true);

        instancia.Configuracao.ComponentesFrete.enable(true);
        instancia.Configuracao.RegexValidacaoNumeroPedidoEmbarcador.enable(true);
        instancia.Configuracao.ObservacaoEmissaoCarga.enable(true);
        instancia.Configuracao.Observacao.enable(true);
        instancia.Configuracao.ObservacaoTerceiro.enable(true);

        instancia.Configuracao.TipoOcorrenciaCTeEmitidoEmbarcador.visible(false);
        instancia.Configuracao.TipoOcorrenciaCTeEmitidoEmbarcador.enable(false);
        instancia.Configuracao.GerarOcorrenciaComplementoSubcontratacao.enable(true);
        instancia.Configuracao.TipoOcorrenciaComplementoSubcontratacao.enable(true);
        instancia.Configuracao.NaoPermitirVincularCTeComplementarEmCarga.visible(false);
        instancia.Configuracao.LiberarDocumentosEmitidosQuandoEntregaForConfirmada.visible(false);
        instancia.Configuracao.TipoIntegracaoMercadoLivre.enable(true);
        instancia.Configuracao.GerarSomenteUmaProvisaoCadaCargaCompleta.enable(true);
        instancia.Configuracao.AvancarCargasDocumentosVinculados.visible(false);
        instancia.Configuracao.AvancarCargasDocumentosVinculados.enable(false);
    }
}

ConfiguracaoEmissaoCTe.prototype.RetornarValoresGerais = function (instancia) {
    instancia.RetornarValores(
        instancia.Configuracao.FormulaRateioFrete.codEntity(),
        instancia.Configuracao.TipoRateioDocumentos.val(),
        instancia.ApolicesSeguro,
        instancia.Configuracao.TipoIntegracao.val(),
        instancia.Configuracao.ModeloDocumentoFiscal.codEntity(),
        instancia.Configuracao.DisponibilizarDocumentosParaNFsManual.val(),
        instancia.Configuracao.ComponentesFrete.val(),
        instancia.Configuracao.TipoEmissaoCTeParticipantes.val(),
        instancia.Configuracao.CTeEmitidoNoEmbarcador.val(),
        instancia.Configuracao.EmpresaEmissora.codEntity(),
        instancia.Configuracao.ArquivoImportacaoNotasFiscais.codEntity(),
        instancia.Configuracao.EnderecoFTP.val(),
        instancia.Configuracao.Usuario.val(),
        instancia.Configuracao.Senha.val(),
        instancia.Configuracao.Porta.val(),
        instancia.Configuracao.Diretorio.val(),
        instancia.Configuracao.Passivo.val(),
        instancia.Configuracao.AgruparMovimentoFinanceiroPorPedido.val(),
        instancia.Configuracao.DescricaoComponenteFreteEmbarcador.val(),
        instancia.Configuracao.ExigirNumeroPedido.val(),
        instancia.Configuracao.NaoValidarNotaFiscalExistente.val(),
        instancia.Configuracao.TipoEmissaoIntramunicipal.val(),        
        instancia.Configuracao.ValePedagioObrigatorio.val(),
        instancia.Configuracao.NomenclaturaArquivo.val(),
        instancia.Configuracao.UtilizarSFTP.val(),
        instancia.Configuracao.SSL.val(),
        instancia.Configuracao.UtilizarOutroModeloDocumentoEmissaoMunicipal.val(),
        instancia.Configuracao.ModeloDocumentoFiscalEmissaoMunicipal.codEntity(),
        instancia.Configuracao.NaoEmitirMDFe.val(),
        instancia.Configuracao.TornarPedidosPrioritarios.val(),
        instancia.Configuracao.BloquearDiferencaValorFreteEmbarcador.val(),
        instancia.Configuracao.EmitirComplementoDiferencaFreteEmbarcador.val(),
        instancia.Configuracao.GerarOcorrenciaSemTabelaFrete.val(),
        instancia.Configuracao.PercentualBloquearDiferencaValorFreteEmbarcador.val(),
        instancia.Configuracao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador.codEntity(),
        instancia.Configuracao.TipoOcorrenciaSemTabelaFrete.codEntity(),
        instancia.Configuracao.ProvisionarDocumentos.val(),
        instancia.Configuracao.GerarMDFeTransbordoSemConsiderarOrigem.val(),
        instancia.Configuracao.ImportarRedespachoIntermediario.val(),
        instancia.Configuracao.EmitenteImportacaoRedespachoIntermediario.codEntity(),
        instancia.Configuracao.ExpedidorImportacaoRedespachoIntermediario.codEntity(),
        instancia.Configuracao.RecebedorImportacaoRedespachoIntermediario.codEntity(),
        instancia.Configuracao.DescricaoItemPesoCTeSubcontratacao.val(),
        instancia.Configuracao.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao.val(),
        instancia.Configuracao.RegexValidacaoNumeroPedidoEmbarcador.val(),
        instancia.Configuracao.ObservacaoEmissaoCarga.val(),
        instancia.Configuracao.CaracteristicaTransporteCTe.val(),
        instancia.Configuracao.TipoEnvioEmail.val(),
        instancia.Configuracao.ValorMaximoEmissaoPendentePagamento.val(),
        instancia.Configuracao.TipoOcorrenciaCTeEmitidoEmbarcador.codEntity(),
        instancia.Configuracao.DisponibilizarDocumentosParaLoteEscrituracao.val(),
        instancia.Configuracao.EscriturarSomenteDocumentosEmitidosParaNFe.val(),
        instancia.Configuracao.TipoPropostaMultimodal.val(),
        instancia.Configuracao.TipoServicoMultimodal.val(),
        instancia.Configuracao.ModalPropostaMultimodal.val(),
        instancia.Configuracao.TipoCobrancaMultimodal.val(),
        instancia.Configuracao.BloquearEmissaoDeEntidadeSemCadastro.val(),
        instancia.Configuracao.BloquearEmissaoDosDestinatario.val(),
        instancia.ClientesBloqueados,
        instancia.Configuracao.DisponibilizarDocumentosParaPagamento.val(),
        instancia.Configuracao.QuitarDocumentoAutomaticamenteAoGerarLote.val(),
        instancia.Configuracao.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento.val(),
        instancia.Configuracao.Observacao.val(),
        instancia.Configuracao.ObservacaoTerceiro.val(),
        instancia.Configuracao.GerarCIOTParaTodasAsCargas.val(),
        instancia.Configuracao.ValorFreteLiquidoDeveSerValorAReceber.val(),
        instancia.Configuracao.GerarOcorrenciaComplementoSubcontratacao.val(),
        instancia.Configuracao.TipoOcorrenciaComplementoSubcontratacao.codEntity(),
        instancia.Configuracao.NaoValidarNotasFiscaisComDiferentesPortos.val(),
        instancia.Configuracao.ValorLimiteFaturamento.val(),
        instancia.Configuracao.DiasEmAbertoAposVencimento.val(),
        instancia.Configuracao.NaoPermitirVincularCTeComplementarEmCarga.val(),
        instancia.Configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS.val(),
        instancia.Configuracao.TempoCarregamento.val(),
        instancia.Configuracao.TempoDescarregamento.val(),
        instancia.Configuracao.TipoIntegracaoMercadoLivre.val(),
        instancia.Configuracao.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente.val(),
        instancia.Configuracao.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente.val(),
        instancia.Configuracao.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida.val(),
        instancia.Configuracao.TempoAcrescimoDecrescimoDataPrevisaoSaida.val(),
        instancia.Configuracao.ObrigatorioInformarMDFeEmitidoPeloEmbarcador.val(),
        instancia.Configuracao.GerarSomenteUmaProvisaoCadaCargaCompleta.val(),
        instancia.Configuracao.DisponibilizarComposicaoRateioCarga.val(),
        instancia.Configuracao.AverbarCTeImportadoDoEmbarcador.val(),
        instancia.Configuracao.TipoUsoFatorCubagemRateioFormula.val(),
        instancia.Configuracao.FatorCubagemRateioFormula.val(),
        instancia.Configuracao.TipoReceita.val(),
    );
};