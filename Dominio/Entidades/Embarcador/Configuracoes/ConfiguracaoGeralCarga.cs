using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_GERAL_CARGA", EntityName = "ConfiguracaoGeralCarga", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoGeralCarga", NameType = typeof(ConfiguracaoGeralCarga))]
    public class ConfiguracaoGeralCarga : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_ALTERAR_INFORMACOES_AGRUPAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarInformacoesAgrupamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_OBRIGATORIO_OPERADOR_RESPONSAVEL_CANCELAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioOperadorResponsavelCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_CALCULAR_PAUTA_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularPautaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_PERMITIR_REMOVER_ULTIMO_PEDIDO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirRemoverUltimoPedidoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_REMOVER_CARGAS_AGRUPAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRemoverCargasAgrupamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_VALIDAR_LICENCA_VEICULO_PARA_CARGA_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarLicencaVeiculoParaCargaRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_UTILIZAR_PESO_PRODUTO_PARA_CALCULAR_PESO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPesoProdutoParaCalcularPesoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PADRAO_GERACAO_NUMERO_CARGA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string PadraoGeracaoNumeroCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_ALERTAR_TRANSPORTADOR_CANCELAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertarTransportadorCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_TROCAR_FILIAL_QUANDO_EXPEDIDOR_FOR_UMA_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TrocarFilialQuandoExpedidorForUmaFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_HABILITAR_RELATORIO_DE_EMBARQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarRelatorioDeEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_EXIBIR_MENSAGEM_ALERTA_PREVISAO_ENTREGA_NA_MESMA_DATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirMensagemAlertaPrevisaoEntregaNaMesmaData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_MANTER_TRANSPORTADOR_UNICO_EM_CARGAS_AGRUPADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ManterTransportadorUnicoEmCargasAgrupadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimitePesoDocumentoCarga", Column = "CAR_LIMITE_PESO_DOCUMENTO_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal LimitePesoDocumentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteValorDocumentoCarga", Column = "CAR_LIMITE_VALOR_DOCUMENTO_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal LimiteValorDocumentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteTaraPedidosCarga", Column = "CAR_LIMITE_TARA_PEDIDOS_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal LimiteTaraPedidosCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_UTILIZAR_PROGRAMACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarProgramacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PREFIXO_PARA_CARGAS_GERADAS_VIA_CARREGAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string PrefixoParaCargasGeradasViaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_AGRUPAMENTO_CARGAS_ORDENAVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAgrupamentoDeCargasOrdenavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_GERAR_REGISTRO_DE_DESEMBARQUE_NO_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirGerarRegistroDeDesembarqueNoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_ATUALIZAR_VINCULO_VEICULO_MOTORISTA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarVinculoVeiculoMotoristaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_PERMITIR_GERAR_REDESPACHO_DE_CARGAS_DE_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirGerarRedespachoDeCargasDeRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_AVERBAR_MDFE_SOMENTE_EM_CARGAS_COM_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarMDFeSomenteEmCargasComCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_EXIGIR_CONFIGURACAO_TERCEIRO_PARA_GERAR_CIOT_PARA_TODOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirConfiguracaoTerceiroParaGerarCIOTParaTodos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_CANCELAR_DOCUMENTOS_CARGA_PELO_CANCELAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCancelarDocumentosCargaPeloCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_HABILITAR_ENVIO_DOCUMENTACAO_CARGA_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarEnvioDocumentacaoCargaPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_SELECIONAR_SOMENTE_OPERACOES_DE_REDESPACHO_NA_TELA_DE_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SelecionarSomenteOperacoesDeRedespachoNaTelaDeRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NOTIFICAR_NOVA_CARGA_APOS_CONFIRMACAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarNovaCargaAposConfirmacaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_INFORMAR_CENTRO_RESULTADO_ETAPA_UM_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarCentroResultadoNaEtapaUmDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_UTILIZA_REGRAS_DE_APROVACA_OPARA_CANCELAMENTO_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaRegrasDeAprovacaoParaCancelamentoDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_CANCELAMENTO_DA_CARGA_SOMENTE_COM_DOCUMENTOS_EMITIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCancelamentoDaCargaSomenteComDocumentosEmitidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_CONSIDERAR_APENAS_UMA_VEZ_KM_PARA_PEDIDOS_COM_MESMO_DESTINO_ORIGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarApenasUmaVezKMParaPedidosComMesmoDestinoOrigemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_OBRIGAR_JUSTIFICATIVA_CANCELAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarJustificativaCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracaoCancelamentoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_ALTERAR_EMPRESA_NO_CTE_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarEmpresaNoCTeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_FINALIZAR_CARGA_AUTOMATICAMENTE_APOS_ENCERRAMENTO_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarCargaAutomaticamenteAposEncerramentoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_ATUALIZAR_DATA_EMISSAO_PARA_DATA_ATUAL_QUANDO_REEMITIR_CTE_REJEITADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarDataEmissaoParaDataAtualQuandoReemitirCTeRejeitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoUtilizarCodigoCargaOrigemNaObservacaoCTe", Column = "CCG_NAO_UTILIZAR_CODIGO_CARGA_ORIGEM_OBSERVACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoUtilizarCodigoCargaOrigemNaObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_RETORNAR_PEDIDOS_INSERIDOS_MANUALMENTE_AO_GERAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarPedidosInseridosManualmenteAoGerarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_AO_CANCELAR_CARGA_MANTER_PEDIDOS_EM_ABERTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AoCancelarCargaManterPedidosEmAberto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_REALIZAR_INTEGRACAO_DADOS_CANCELAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoDadosCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_UTILIZAR_CONFIGURACAO_TIPO_OPERACAO_GERACAO_CARGA_POR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarConfiguracaoTipoOperacaoGeracaoCargaPorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PADRAO_VISUALIZACAO_OPERADOR_LOGISTICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PadraoVisualizacaoOperadorLogistico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_VINCULAR_AUTOMATICAMENTE_DOCUMENTOS_EMITIDOS_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoVincularAutomaticamenteDocumentosEmitidosEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_AVANCAR_CARGAS_EMITIDAS_EMBARCADOR_POR_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAvancarCargasEmitidasEmbarcadorPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_ATUALIZAR_DADOS_DOS_PEDIDOS_COM_DADOS_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizarDadosDosPedidosComDadosDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_VISUALIZAR_LEGENDA_CARGA_ACORDO_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarLegendaCargaAcordoTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_CANCELAR_CIOT_AUTOMATICAMENTE_FLUXO_CANCELAMENTO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CancelarCIOTAutomaticamenteFluxoCancelamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_ENVIAR_EMAIL_PREVIA_CUSTO_PARA_TRANSPORTADORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailPreviaCustoParaTransportadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITE_INFORMAR_REMENTENTE_LANCAMENTO_NOTA_MANUAL_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarRemetenteLancamentoNotaManualCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_AJUSTAR_VALOR_FRETE_APOS_APROVACAO_PRE_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AjustarValorFreteAposAprovacaoPreCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_GERAR_PDF_DOCUMENTOS_COM_NOTAS_FISCAIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarPDFDocumentosComNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITE_SELECIONAR_MULTIPLAS_CARGAS_PARA_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSelecionarMultiplasCargasParaRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_GERAR_REDESPACHO_DE_CARGAS_AGRUPADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarRedespachoDeCargasAgrupadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_ENCERRAR_MDFE_DE_FORMA_AUTOMATICA_AO_CONFIRMAR_DADOS_DE_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEncerrarMDFeDeFormaAutomaticaAoConfirmarDadosDeTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_EXCLUIR_AGENDAMENTO_CARGA_JANELA_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteExcluirAgendamentoDaCargaJanelaDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_ASSUMIR_SEMPRE_TIPO_OPERACAO_DO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AssumirSempreTipoOperacaoDoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_UTILIZA_CONTROLE_DE_ENTREGA_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaControleDeEntregaManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_SELECIONAR_MULTIPLAS_CARGAS_PARA_AGRUPAR_NO_TRANSBORDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_PERMITIR_ENCERRAR_CIOT_ENCERRAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirEncerrarCIOTEncerrarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_DESABILITAR_UTILIZACAO_CREDITO_OPERADORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesabilitarUtilizacaoCreditoOperadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_CONVERTER_XML_NOTA_FISCAL_PARA_BYTE_ARRAY_AO_IMPORTAR_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConverterXMLNotaFiscalParaByteArrayAoImportarNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_USAR_PRIORIDADE_DA_CARGA_PARA_IMPRESSAO_DE_OBSERVACAO_NO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarPrioridadeDaCargaParaImpressaoDeObservacaoNoCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_REMOVER_VINCULO_NOTA_PEDIDO_ABERTO_AO_CANCELAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemoverVinculoNotaPedidoAbertoAoCancelarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_GERAR_OUTROS_DOCUMENTOS_IMPORTACAO_CTE_COMPLEMENTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOutrosDocumentosNaImportacaoDeCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarNumerodeCargaAlfanumerico", Column = "CCG_GERAR_NUMERO_DE_CARGA_ALFANUMERICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarNumerodeCargaAlfanumerico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_TAMANHO_NUMERO_DE_CARGA_ALFANUMERICO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TamanhoNumerodeCargaAlfanumerico), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TamanhoNumerodeCargaAlfanumerico TamanhoNumerodeCargaAlfanumerico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarValorLimiteApoliceComValorNFe", Column = "CCG_VALIDAR_VALOR_LIMITE_APOLICE_COM_VALOR_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarValorLimiteApoliceComValorNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_PERMITIR_ALTERAR_DADOS_CARGA_QUANDO_TIVER_INTEGRACAO_INTEGRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAlterarDadosCargaQuandoTiverIntegracaoIntegrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_RETORNAR_DADOS_COMPLEMENTARES_DOS_PARTICIPANTES_PEDIDO_DETALHES_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarDadosComplementaresDosParticipantesDoPedidoDetalhePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_ATRIBUIR_VALOR_MERCADORIA_CTE_NOTAS_FISCAIS_DOCUMENTOS_EMITIDOS_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtribuirValorMercadoriaCTeNotasFiscaisDocumentosEmitidosEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_VALIDAR_CONTRATANTE_ORIGEM_VP_INTEGRACAO_PAMCARD", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarContratanteOrigemVPIntegracaoPamcard { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_ENCAIXAR_PEDIDOS_COM_REENTREGA_SOLICITADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEncaixarPedidosComReentregaSolicitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_PERMITIR_AVANCAR_ETAPA_UM_CARGA_COM_TRANSPORTADOR_SEM_APOLICE_VIGENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAvançarEtapaUmCargaComTransportadorSemApoliceVigente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_GERAR_CARGA_COM_FLUXO_FILIAL_EMISSORA_COM_EXPEDIDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaComFluxoFilialEmissoraComExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_CONSIDERAR_DATA_EMISSAO_CTE_CALCULO_EMBARQUE_PREVISAO_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarDataEmissaoCTECalculoEmbarquePrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_CONSIDERAR_CONFIGURACAO_NO_TIPO_DE_OPERACAO_PARA_PARTICIPANTES_DOS_DOCUMENTOS_AO_GERAR_CARGA_ESPELHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarConfiguracaoNoTipoDeOperacaoParaParticipantesDosDocumentosAoGerarCargaEspelho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PROCESSAR_DADOS_TRANSPORTE_AO_FECHAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProcessarDadosTransporteAoFecharCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_UTILIZAR_EMPRESA_FILIAL_EMISSORA_NO_ARQUIVO_EDI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarEmpresaFilialEmissoraNoArquivoEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_RACALCULAR_FRETE_AO_DUPLICAR_CARGA_CANCELAMENTO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? RecalcularFreteAoDuplicarCargaCancelamentoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITE_INFORMAR_FRETE_OPERADOR_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PermiteInformarFreteOperadorFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_CONSIDERAR_RECEBEDOR_AO_CALCULAR_NUMERO_ENTREGAS_EMISSAO_POR_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoConsiderarRecebedorAoCalcularNumeroEntregasEmissaoPorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITE_HABILITAR_CONTINGENCIA_EPEC_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PermiteHabilitarContingenciaEPECAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITE_RECEBER_NOTA_FISCAL_VIA_INTEGRACAO_NAS_ETAPAS_FRETE_E_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteReceberNotaFiscalViaIntegracaoNasEtapasFreteETransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoMinutosParaEnvioProgramadoIntegracao", Column = "CCG_TEMPO_MINUTOS_PARA_ENVIO_PROGRAMADO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? TempoMinutosParaEnvioProgramadoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_SETAR_CARGA_COMO_BLOQUEADA_ENQUANTO_NAO_RECEBER_DESBLOQUEIO_VIA_INTEGRACAO_OU_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SetarCargaComoBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_DESVINCULAR_GERAR_COPIA_CTE_REJEITADO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PermitirDesvincularGerarCopiaCTeRejeitadoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_SALVAR_APENAS_TRANSPORTADOR_ETAPA_UM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSalvarApenasTransportadorEtapaUmCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_EXIGIR_CONFIRMACAO_ETAPA_FRETE_NO_FLUXO_NOTA_APOS_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirConfirmacaoEtapaFreteNoFluxoNotaAposFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITE_REVERTER_ANULACAO_GERENCIAL_TELA_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirReverterAnulacaoGerencialTelaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_ATIVAR_CANCELAMENTO_DE_FATURA_E_TITULO_VINCULADO_AO_FLUXO_DE_CANCELAMENTO_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarCancelamentoDeFaturaETituloAoFluxoDeCancelamentoNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_PERMITIR_ATRIBUIR_VEICULO_CARGA_SE_EXISTIR_MONITORAMENTO_ATIVO_PARA_PLACA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAtribuirVeiculoCargaSeExistirMonitoramentoAtivoParaPlaca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_CONSIDERAR_FILIAL_DA_TRANSPORTADORA_PARA_COMPRA_DO_VALE_PEDAGIO_QUANDO_FOR_E_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarFilialDaTransportadoraParaCompraDoValePedagioQuandoForEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_DISTANCIA_ROTEIRIZACAO_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarDistanciaRoteirizacaoNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_INFORMAR_DOCA_ETAPA_UM_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDocaNaEtapaUmDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_CANCELAR_VALE_PEDAGIO_QUANDO_GERAR_CARGA_TRANSBORDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CancelarValePedagioQuandoGerarCargaTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_FILTRAR_CARGAS_NA_EMISSAO_MANUAL_CTE_SEM_TER_CTES_IMPORTADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirFiltrarCargasNaEmissaoManualCteSemTerCtesImportados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_REMOVER_MULTIPLOS_PEDIDOS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRemoverMultiplosPedidosCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_SOLICITAR_JUSTIFICATIVA_AO_REMOVER_PEDIDO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitarJustificativaAoRemoverPedidoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_VALIDAR_MODELO_VEICULAR_VEICULO_CARGA_ETAPA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarModeloVeicularVeiculoCargaEtapaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_OBRIGATORIEDADE_CIOT_EMISSAO_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatoriedadeCIOTEmissaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirInformarRecebedorAoCriarUmRedespachoManual", Column = "CCG_PERMITIR_INFORMAR_RECEBEDOR_AO_CRIAR_UM_REDESPACHO_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarRecebedorAoCriarUmRedespachoManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_UTILIZAR_DATA_CARREGAMENTO_AO_CRIAR_CARGA_VIA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataCarregamentoAoCriarCargaViaIntegracao { get; set; }

        [Obsolete("Não utilizar essa configuração, configuração temporária.")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_FORCAR_ROTEIRIZACAO_FECHAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ForcarRoteirizacaoFecharCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_INICIAR_CONFIRMACAO_DOCS_FISCAIS_CARGA_POR_THREAD", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciarConfirmacaoDocumentosFiscaisCargaPorThread { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITIR_INFORMAR_VALOR_FRETE_OPERADOR_MESMO_COM_FRETE_CONFIRMADO_PELO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarValorFreteOperadorMesmoComFreteConfirmadoPeloTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_APLICAR_ICMS_METODO_ATUALIZAR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAplicarICMSMetodoAtualizarFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PARAR_CARGA_QUANDO_NAO_INFORMADO_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PararCargaQuandoNaoInformadoCIOT { get; set; }

        #endregion

        public virtual string Descricao
        {
            get { return "Configuração gerais da carga"; }
        }

    }
}