using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_FINANCEIRO", EntityName = "ConfiguracaoFinanceiro", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoFinanceiro", NameType = typeof(ConfiguracaoFinanceiro))]
    public class ConfiguracaoFinanceiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_VALIDAR_DUPLICIDADE_TITULO_SEM_DATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarDuplicidadeTituloSemData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ATIVAR_CONTROLE_DESPESAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarControleDespesas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_EXIBIR_NUMERO_PEDIDO_EMBARCADOR_GESTAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNumeroPedidoEmbarcadorGestaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_PERMITIR_DEIXAR_DOCUMENTO_EM_TRATATIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirDeixarDocumentoEmTratativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasDeFechamentoParaGeracaoPagamentoEscrituracaoAutomatico", Column = "COF_DIAS_FECHAMENTO_PARA_GERACAO_PAGAMENTO_ESCRITURACAO_AUTOMATICO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string DiasDeFechamentoParaGeracaoPagamentoEscrituracaoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_DESPACHANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Despachante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_QUANTIDADE_DIAS_LIMITE_VENCIMENTO_TITULO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasLimiteVencimentoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_MOVIMENTACAO_FINANCEIRA_PARA_TITULOS_DE_PROVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MovimentacaoFinanceiraParaTitulosDeProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_UTILIZAR_VALOR_DESPROPORCIONAL_RATEIO_DESPESA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarValorDesproporcionalRateioDespesaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_PERMITIR_PROVISIONAMENTO_DE_NOTAS_CTES_NA_TELA_PROVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirProvisionamentoDeNotasCTesNaTelaProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_PERMITIR_MOVIMENTO_PELA_DATA_VENCIMENTO_CONTRATO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoPelaDataVencimentoContratoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_CUSTO_PARA_REALIZAR_RATEIOS_SOBRE_DOCUMENTO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCustoParaRealizarRateiosSobreDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_AUTOMATIZAR_GERACAO_LOTES_PROVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AutomatizarGeracaoLoteProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_LOTES_APOS_EMISSAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarLotesAposEmissaoDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_LOTE_PAGAMENTO_APOS_DIGITALIZACAO_DO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarLotePagamentoAposDigitalizacaoDoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_GERAR_LOTES_PROVISAO_APOS_EMISSAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarLotesProvisaoAposEmissaoDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_GERAR_LOTE_PROVISAO_PARA_CARGA_AGUARDANDO_IMPORTAR_CTE_OU_LANCAR_NFS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarLoteProvisaoParaCargaAguardandoImportarCTeOuLancarNFS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_GERAR_LOTE_PROVISAO_PARA_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarLoteProvisaoParaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_UTILIZAR_DATA_VENCIMENTO_TITULO_MOVIMENTO_CONTRATO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataVencimentoTituloMovimentoContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_INCLUIR_ICMS_BASE_CALCULO_PIS_COFINS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIncluirICMSBaseCalculoPisCofins { get; set; }

        [Obsolete("Não é mais usado pois não pode ser uma configuração geral")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INCLUIR_VALOR_DE_ICMS_NO_VALOR_DA_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirValorDeICMSNoValorDaPrestacao { get; set; }

        [Obsolete("Não é mais usado pois não pode ser uma configuração geral")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_INCLUIR_VALOR_DE_ICMS_NO_VALOR_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirValorDeICMSNoValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEM_NAO_OBRIGAR_TIPO_OPERACAO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoObrigarTipoOperacaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_HABILITAR_OPCAO_GERAR_FATURAS_APENAS_CANHOTOS_APROVADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarOpcaoGerarFaturasApenasCanhotosAprovados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ATIVAR_COLUNA_CST_CONSULTA_DOCUMENTOS_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarColunaCSTConsultaDocumentosFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_ATIVAR_COLUNA_NUMERO_CONTAINER_CONSULTA_DOCUMENTOS_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarColunaNumeroContainerConsultaDocumentosFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_RATEAR_MOVIMENTOS_DESCONTOS_ACRESCIMOS_BAIXA_TITULOS_PAGAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RatearMovimentosDescontosAcrescimosBaixaTitulosPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_PERMITIR_CONFIRMAR_DOCUMENTOS_FATURA_APENAS_COM_CTES_ESCRITURADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirConfirmarDocumentosFaturaApenasComCtesEscriturados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NAO_VALIDAR_CONDICAO_PAGAMENTO_FECHAMENTO_LOTE_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarCondicaoPagamentoFechamentoLotePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_EXIGIR_INFORMAR_FILIAL_EMISSAO_FATURAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarFilialEmissaoFaturas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_GERAR_LOTES_PAGAMENTO_INDIVIDUAIS_POR_DOCUMENTO ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarLotesPagamentoIndividuaisPorDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_SOMAR_VALOR_ISS_NO_TOTAL_RECEBER_GERACAO_LOTE_PROVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomarValorISSNoTotalReceberGeracaoLoteProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_HORAS_LIBERACAO_PAGAMENTO_TRANSPORTADOR_COM_CERTIFICADO", TypeType = typeof(int), NotNull = false)]
        public virtual int HorasLiberacaoDocumentoPagamentoTransportadorComCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_HORAS_LIBERACAO_PAGAMENTO_TRANSPORTADOR_SEM_CERTIFICADO", TypeType = typeof(int), NotNull = false)]
        public virtual int HorasLiberacaoDocumentoPagamentoTransportadorSemCertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_GERAR_AUTOMATICAMENTE_LOTES_CANCELADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarAutomaticamenteLotesCancelados { get; set; }

        [Obsolete("Criada a coluna COF_MINUTOS_AGUARDAR_GERACAO_LOTE_PAGAMENTO para salvar o total de minutos para gerar o lote de pagamento")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "HorasGeracao", Column = "COF_HORAS_GERACAO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HorasGeracao { get; set; }

        [Obsolete("Criada a coluna COF_MINUTOS_AGUARDAR_GERACAO_LOTE_PAGAMENTO_ULTIMO_DIA_MES para salvar o total de minutos para gerar o lote de pagamento no último dia do mês")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "HorasGeracaoUltimoDiaMes", Column = "COF_HORAS_GERACAO_ULTIMO_DIA_MES", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan? HorasGeracaoUltimoDiaMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_QUANTIDADE_DIAS_ABERTO_ESTORNO_PROVISAO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasAbertoEstornoProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_HABILITA_INTERVALO_TEMPO_LIBERA_DOCUMENTO_EMITIDO_ESCRITURACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? HabilitaIntervaloTempoLiberaDocumentoEmitidoEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_GERAR_DOCUMENTO_PROVISAO_AO_RECEBER_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarDoumentoProvisaoAoReceberNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_PERMITIR_PROVISIONAR_SEM_CALCULO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoPermitirProvisionarSemCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_GERAR_ESTORNO_PROVISAO_AUTOMATICO_APOS_ESCRITURACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarEstornoProvisaoAutomaticoAposEscrituracao { get; set; }

        [Obsolete("Criada a coluna COF_GERAR_ESTORNO_PROVISAO_AUTOMATICO_APOS_LIBERACAO_PAGAMENTO para atender novo fluxo de geração de estornos automáticos")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_GERAR_ESTORNO_PROVISAO_AUTOMATICO_APOS_DATA_DIGITALIZACAO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarEstornoProvisaoAutomaticoAposDataDigitalizacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_GERAR_ESTORNO_PROVISAO_AUTOMATICO_APOS_LIBERACAO_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarEstornoProvisaoAutomaticoAposLiberacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_UTILIZAR_ESTORNO_PROVISAO_DE_FORMA_AUTOMATIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarEstornoProvisaoDeFormaAutomatizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_RATEIO_PROVISAO_POR_GRUPO_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? RateioProvisaoPorGrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_GERAR_PAGAMENTO_PARA_MOTORISTA_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarPagamentoParaMotoristaTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_EFETUAR_VINCULO_CENTRO_RESULTADO_CTE_SUBSTITUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EfetuarVinculoCentroResultadoCTeSubstituto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_GERAR_LOTE_PROVISAO_INDIVIDUAL_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarLoteProvisaoIndividualNfe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_UTILIZAR_FECHAMENTO_AUTOMATICO_PROVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarFechamentoAutomaticoProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_EFETUAR_CANCELAMENTO_DE_PAGAMENTO_AO_CANCELAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EfetuarCancelamentoDePagamentoAoCancelarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_VALIDAR_DATA_PREVISAO_PAGAMENTO_E_DATA_PAGAMENTO_NO_CANCELAMENTO_DOS_CTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarDataPrevisaoPagamentoEDataPagamentoNoCancelamentoDosCTes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoCancelamentoPagamento", Column = "COF_MOTIVO_CANCELAMENTO_PAGAMENTO_PADRAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.MotivoCancelamentoPagamento MotivoCancelamentoPagamentoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_MINUTOS_AGUARDAR_GERACAO_LOTE_PAGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int? MinutosAguardarGeracaoLotePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_MINUTOS_AGUARDAR_GERACAO_LOTE_PAGAMENTO_ULTIMO_DIA_MES", TypeType = typeof(int), NotNull = false)]
        public virtual int? MinutosAguardarGeracaoLotePagamentoUltimoDiaMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_DELAY_FATURAMENTO_AUTOMATICO", TypeType = typeof(int), NotNull = false)]
        public virtual int? DelayFaturamentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_QUANTIDADE_DOCUMENTOS_LOTE_PAGAMENTO_GERADO_AUTOMATICO", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadeDocumentosLotePagamentoGeradoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_QUANTIDADE_MINIMA_DOCUMENTOS_LOTE_PAGAMENTO_GERADO_AUTOMATICO", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadeMinimaDocumentosLotePagamentoGeradoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_TRAVAR_FLUXO_COMPRA_CASO_FORNECEDOR_DIVERGENTE_NA_ORDEM_COMPRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TravarFluxoCompraCasoFornecedorDivergenteNaOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_PERMITIR_GERAR_LOTES_PAGAMENTOS_DOCUMENTOS_BLOQUEADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirGerarLotesPagamentosDocumentosBloqueados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_NAO_PERMITIR_REENVIAR_INTEGRACOES_PAGAMENTO_SE_CANCELADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirReenviarIntegracoesPagamentoSeCancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_BLOQUEIO_ENVIO_INTEGRACOES_CARGAS_ANULADAS_E_CANCELADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloqueioEnvioIntegracoesCargasAnuladaseCanceladas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_GERAR_INTEGRACAO_CONTABILIZACAO_CTES_APOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarIntegracaoContabilizacaoCtesApos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_DELAY_INTEGRACAO_CONTABILIZACAO_CTES", TypeType = typeof(int), NotNull = false)]
        public virtual int? DelayIntegracaoContabilizacaoCtes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_AGRUPAR_PROVISOES_POR_NOTA_FISCAL_FECHAMENTO_MENSAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparProvisoesPorNotaFiscalFechamentoMensal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_UTILIZAR_CONFIGURACOES_TRANSPORTADOR_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarConfiguracoesTransportadorParaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_BAIXA_TITULOS_RENEGOCIACAO_GERAR_NOVO_TITULO_POR_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BaixaTitulosRenegociacaoGerarNovoTituloPorDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COF_GERAR_LOTE_PAGAMENTO_SOMENTE_PARA_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarLotePagamentoSomenteParaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarPreenchimentoTomadorFaturaConfiguracao", Column = "COF_UTILIZAR_PREENCHIMENTO_TOMADOR_FATURA_CONFIGURACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPreenchimentoTomadorFaturaConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ManterValorMoedaConfirmarDocumentosFatura", Column = "COF_MANTER_VALOR_MOEDA_CONFIRMAR_DOCUMENTOS_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ManterValorMoedaConfirmarDocumentosFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarEmpresaFilialImpressaoReciboPagamentoMotorista", Column = "COF_UTILIZAR_EMPRESA_FILIAL_IMPRESSAO_RECIBO_PAGAMENTO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarEmpresaFilialImpressaoReciboPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoLiberarPagamentosBloqueadosAoLiberarCanhoto", Column = "COF_NAO_LIBERAR_PAGAMENTOS_BLOQUEADOS_AO_LIBERAR_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoLiberarPagamentosBloqueadosAoLiberarCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DesbloquearPagamentoPorCanhoto", Column = "COF_DESBLOQUEAR_PAGAMENTO_POR_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DesbloquearPagamentoPorCanhoto { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para o financeiro"; }
        }
    }
}
