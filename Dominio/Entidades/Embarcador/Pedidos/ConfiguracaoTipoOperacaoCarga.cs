using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_CARGA", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoCarga", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCarga", NameType = typeof(ConfiguracaoTipoOperacaoCarga))]
    public class ConfiguracaoTipoOperacaoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_NAO_PERMITIR_INFORMAR_MOTORISTA_COM_CNH_VENCIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirInformarMotoristaComCNHVencida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_MULTIPLICAR_QUANTIDADE_PRODUTO_POR_CAIXA_PELA_QUANTIDADE_CAIXA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeInformarIscaNaCargaComValorMaiorQue", Column = "CCG_EXIGE_INFORMAR_ISCA_NA_CARGA_VALOR_MAIOR_QUE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ExigeInformarIscaNaCargaComValorMaiorQue { get; set; }

        /// <summary>
        /// Parâmetro descontinuado. Utilizar o parâmetro LayoutImpressaoOrdemColeta (TipoOperacao.ConfiguracaoJanelaCarregamento.LayoutImpressaoOrdemColeta) no objeto ConfiguracaoTipoOperacaoJanelaCarregamento.cs
        /// </summary>
        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_UTILIZAR_LAYOUT_ORDEM_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarLayoutOrdemCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_ADICIONAR_BL_COMO_OUTRO_DOCUMENTO_AUTOMATICAMENTE_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarBLComoOutroDocumentoAutomaticamenteNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITE_ADICIONAR_ANEXOS_GUARITA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAdicionarAnexosGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLimiteNaCarga", Column = "CCG_VALOR_LIMITE_NA_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorLimiteNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarEmailAoGerarCargaPlanejamentoPedidosDetalhado", Column = "CCG_ENVIAR_EMAIL_AO_GERAR_CARGA_PLANEJAMENTO_PEDIDOS_DETALHADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailAoGerarCargaPlanejamentoPedidosDetalhado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirVisualizarOrdenarAsZonasDeTransporte", Column = "CCG_PERMITIR_VISUALIZAR_ORDENAR_AS_ZONAS_DE_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirVisualizarOrdenarAsZonasDeTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirOperadorInsercaoCargaNoPortalTransportador", Column = "CCG_EXIBIR_OPERADOR_INSERCAO_CARGA_NO_PORTAL_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirOperadorInsercaoCargaNoPortalTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAdicionarObservacaoNaEtapaUmDaCarga", Column = "CCG_PERMITIR_ADICIONAR_OBSERVACAO_NA_ETAPA_UM_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAdicionarObservacaoNaEtapaUmDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirNumeroPedidoNosDetalhesDaCarga", Column = "CCG_EXIBIR_NUMERO_PEDIDO_NOS_DETALHES_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirNumeroPedidoNosDetalhesDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoParaAlertarPorEmailResponsavelDaFilialDaCargaQueAindaNaoTeveOsDadosDeTransporteInformados", Column = "CCG_TEMPO_PARA_ALERTAR_POR_EMAIL_RESPONSAVEL_DA_FILIAL_QUE_CARGA_AINDA_NAO_TEVE_OS_DADOS_DE_TRANSPORTE_INFORMADOS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoParaAlertarPorEmailResponsavelDaFilialDaCargaQueAindaNaoTeveOsDadosDeTransporteInformados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoParaRecebimentoDosPacotes", Column = "CCG_TEMPO_PARA_RECEBIMENTO_DOS_PACOTES", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoParaRecebimentoDosPacotes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirFiltroDePedidosEtapaNotaFiscal", Column = "CCG_EXIBIR_FILTRO_DE_PEDIDO_ETAPA_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirFiltroDePedidosEtapaNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAlterarDataRetornoCDCarga", Column = "CCG_PERMITIR_ALTERAR_DATA_RETORNO_CD_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarDataRetornoCDCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IgnorarRateioConfiguradoPorto", Column = "CCG_IGNORAR_RATEIO_CONFIGURADO_PORTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarRateioConfiguradoPorto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarLoteProdutoVersusLoteNotaFiscal", Column = "CCG_VALIDAR_LOTE_PRODUTO_VERSUS_LOTE_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarLoteProdutoVersusLoteNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExecutarCalculoRelevanciaDeCustoNFePorCFOP", Column = "CCG_EXECUTAR_CALCULO_RELEVANCIA_CUSTO_NFE_POR_CFOP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExecutarCalculoRelevanciaDeCustoNFePorCFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AguardarRecebimentoProdutoParaProvisionar", Column = "CCG_AGUARDAR_RECEBIMENTO_PRODUTO_PARA_PROVISIONAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardarRecebimentoProdutoParaProvisionar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAdicionarNovosPedidosPorNotasAvulsas", Column = "CCG_PERMITIR_ADICIONAR_NOVOS_PEDIDOS_POR_NOTAS_AVULSAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAdicionarNovosPedidosPorNotasAvulsas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoERP", Column = "CCG_INTEGRADO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarRetornoAutomaticoMomento", Column = "CCG_GERAR_RETORNO_AUTOMATICO_CONFIRMACAO_VIAGEM", TypeType = typeof(GerarRetornoAutomaticoMomento), NotNull = false)]
        public virtual GerarRetornoAutomaticoMomento GerarRetornoAutomaticoMomento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlertarAlteracoesPedidoNoFluxoPatio", Column = "CCG_ALERTAR_ALTERACOES_PEDIDO_NO_FLUXO_PATIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertarAlteracoesPedidoNoFluxoPatio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivoModuloNaoConformidades", Column = "CCG_ATIVO_MODULO_NAO_CONFORMIDADES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivoModuloNaoConformidades { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HerdarDadosDeTransporteCargaPrimeiroTrecho", Column = "CCG_HERDAR_DADOS_DE_TRANSPORTE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HerdarDadosDeTransporteCargaPrimeiroTrecho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarKMRecibidoDoEmbarcador", Column = "CCG_CONSIDERAR_KM_RECIBIDO_DO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarKMRecibidoDoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigarInformarValePedagio", Column = "CCG_OBRIGAR_INFORMAR_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarInformarValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirRelacionarOutrasCargas", Column = "CCG_PERMITIR_RELACIONAR_OUTRAS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRelacionarOutrasCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeNotaFiscalTenhaTagRetirada", Column = "CCG_EXIGE_NOTA_FISCAL_TENHA_TAG_RETIRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeNotaFiscalTenhaTagRetirada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VincularPedidoDeAcordoComNumeroOrdem", Column = "CCG_VINCULAR_PEDIDO_DE_ACORDO_COM_NUMERO_ORDEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularPedidoDeAcordoComNumeroOrdem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteCancelamentoUnitarioDocumentoFrete", Column = "CCG_PERMITE_CANCELAMENTO_UNITARIO_DOCUMENTO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteCancelamentoUnitarioDocumentoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarKMDaRotaFrete", Column = "CCG_CONSIDERAR_KM_DA_ROTA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarKMDaRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DeixarPedidosDisponiveisParaMontegemCarga", Column = "CCG_DEIXAR_PEDIDOS_DISPONIVEIS_PARA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeixarPedidosDisponiveisParaMontegemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrecisaEsperarNotasFilhaParaGerarPagamento", Column = "CCG_PRECISA_ESPERAR_NOTAS_FILHA_PARA_GERAR_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PrecisaEsperarNotasFilhaParaGerarPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrecisaEsperarNotaTransferenciaParaGeraPagamento", Column = "CCG_PRECISA_ESPERAR_NOTAS_TRANSFERENCIA_PARA_GERAR_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PrecisaEsperarNotaTransferenciaParaGeraPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarPesoDasNotasRelevantes", Column = "CCG_VALIDAR_PESO_DAS_NOTAS_RELEVANTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarPesoDasNotasRelevantes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCargaEspelhoAoConfirmarEntrega", Column = "CCG_GERAR_CARGA_ESPELHO_AO_CONFIRMAR_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaEspelhoAoConfirmarEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearInclusaoArquivosXMLDeNFeCarga", Column = "CCG_BLOQUEAR_INCLUSAO_ARQUIVOS_XML_DE_NFE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearInclusaoArquivosXMLDeNFeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirIntegrarPacotes", Column = "CCG_PERMITIR_INTEGRAR_PACOTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirIntegrarPacotes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_TIPO_OPERACAO_CARGA_ESPELHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacaoCargaEspelho { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_TIPO_OPERACAO_CARGA_RETORNO_COLETAS_REJEITADAS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacaoCargaRetornoColetasRejeitadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirUsoNotasQueEstaoEmOutraCarga", Column = "CCG_NAO_PERMITIR_USO_NOTAS_QUE_ESTAO_EM_OUTRA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirUsoNotasQueEstaoEmOutraCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformarLacreNosDadosTransporte", Column = "CCG_INFORMAR_LACRE_NOS_DADOS_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarLacreNosDadosTransporte { get; set; }

        [Obsolete("Criado errado")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCargaPagamentoConfirmarControleEntrega", Column = "CCG_GERAR_CARGA_PAGAMENTO_CONFIRMAR_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaPagamentoConfirmarControleEntrega { get; set; }

        [Obsolete("Criado errado")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_TIPO_OPERACAO_CONFIRMACAO_CONTROLE_ENTREGA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacaoConfirmacaoControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCancelamentoCargaDocumento", Column = "CCG_TIPO_CANCELAMENTO_CARGA_DOCUMENTO", TypeType = typeof(TipoCancelamentoCargaDocumento), NotNull = false)]
        public virtual TipoCancelamentoCargaDocumento TipoCancelamentoCargaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes", Column = "CCG_PERCENTUAL_TOLERANCIA_PARA_EMISSAO_ENTRE_CTES_RECEBIDOS_VERSUS_PACOTES", TypeType = typeof(int), NotNull = false)]
        public virtual int PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDiasValidacaoNFeDataCarregamento", Column = "CCG_QUANTIDADE_DIAS_VALIDACAO_NFE_DATA_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasValidacaoNFeDataCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirEditarPesoBrutoDaNotaFiscalEletronica", Column = "CCG_NAO_PERMITIR_EDITAR_PESO_BRUTO_DA_NOTA_FISCAL_ELETRONICA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirEditarPesoBrutoDaNotaFiscalEletronica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDeEnvioPorSMSDeDocumentos", Column = "CCG_TIPO_DE_ENVIO_POR_SMS_DE_DOCUMENTOS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDeEnvioPorSMSDeDocumentos), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDeEnvioPorSMSDeDocumentos TipoDeEnvioPorSMSDeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarDistribuidorPorRegiaoNaRegiaoDestino", Column = "CCG_UTILIZAR_DISTRIBUIDOR_POR_REGIAO_NA_REGIAO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDistribuidorPorRegiaoNaRegiaoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarConsultaContainerEMP", Column = "CCG_HABILITAR_CONSULTA_CONTAINER_EMP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarConsultaContainerEMP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirInformarAjudantesNaCarga", Column = "CCG_PERMITIR_INFORMAR_AJUDANTES_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarAjudantesNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCargaRetornoRejeitarTodasColetas", Column = "CCG_GERAR_CARGA_RETORNO_REJEITAR_TODAS_COLETAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaRetornoRejeitarTodasColetas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidaValorPreCalculoValorFrete", Column = "CCG_VALIDAR_VALOR_PRE_CALCULO_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidaValorPreCalculoValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarValorMinimoCarga", Column = "CCG_VALIDAR_VALOR_MINIMO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarValorMinimoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoCarga", Column = "CCG_VALOR_MINIMO_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorMinimoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete", Column = "CCG_PERMITE_INFORMAR_TRANSPORTADOR_ETAPA_UM_QUANDO_NOTA_FISCAL_NAO_RECEBIDA_ANTES_CALCULO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AvancarCargaAutomaticamenteAoReceberIntegracaoNotasWS", Column = "CCG_AVANCAR_CARGA_AUTOMATICAMENTE_AO_RECEBER_INTEGRACAO_NOTAS_WS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvancarCargaAutomaticamenteAoReceberIntegracaoNotasWS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento", Column = "CCG_MANTER_CARGA_NA_ETAPA_UM_X_HORAS_ANTES_DATA_DE_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento", Column = "CCG_HORAS_PARA_MANTER_CARGA_NA_ETAPA_UM_X_HORAS_ANTES_DATA_DE_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoAvancarEtapaSePlacaEstiverEmMonitoramento", Column = "CCG_NAO_AVANCAR_ETAPA_SE_PLACA_ESTIVER_EM_MONITORAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAvancarEtapaSePlacaEstiverEmMonitoramento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOperacaoPreCarga", Column = "CCG_TIPO_OPERACAO_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoOperacaoPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirSelecionarPreCargaNaCarga", Column = "CCG_PERMITIR_SELECIONAR_PRE_CARGA_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirSelecionarPreCargaNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioInformarAliquotaImpostoSuspensoeValor", Column = "CCG_OBRIGATORIO_INFORMAR_ALIQUOTA_IMPOSTO_SUSPENSO_VALOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarAliquotaImpostoSuspensoeValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionaPrefixoCodigoCarga", Column = "CCG_ADICIONA_PREFIXO_CODIGO_CARGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string AdicionaPrefixoCodigoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncrementaCodigoPorTipoOperacao", Column = "CCG_INCREMENTA_CODIGO_POR_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncrementaCodigoPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProximoCodigoCarga", Column = "CCG_PROXIMO_CODIGO_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int ProximoCodigoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_LIBERAR_CARGA_SEM_NFE_AUTOMATICAMENTE_APOS_LIBERAR_FATURAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReceberAtualizacaoDeTransporteEmQualquerSituacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_UTILIZAR_DIRECIONAMENTO_CUSTO_EXTRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDirecionamentoCustoExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DirecionamentoCustoExtra", Column = "CCG_DIRECIONAMENTO_CUSTO_EXTRA", TypeType = typeof(TipoDirecionamentoCustoExtra), NotNull = false)]
        public virtual TipoDirecionamentoCustoExtra DirecionamentoCustoExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_OBRIATORIO_JUSTIFICAR_CUSTO_EXTRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioJustificarCustoExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_UTILIZA_INTEGRACAO_OK_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaIntegracaoOKColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_BUSCAR_DOCUMENTOS_E_AVERBACAO_PELA_OS_MAE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarDocumentosEAverbacaoPelaOSMae { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcordoFaturamento", Column = "CCG_ACORDO_FATURAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcordoFaturamento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcordoFaturamento AcordoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarCargaSemPlanejamento", Column = "CCG_LIBERAR_CARGA_SEM_PLANEJAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarCargaSemPlanejamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoProvedor", Column = "CCG_DOCUMENTO_PROVEDOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoProvedor), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoProvedor DocumentoProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarRedespachoAutomaticamenteAposEmissaoDocumentos", Column = "CCG_GERAR_REDESPACHO_AUTOMATICAMENTE_APOS_EMISSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarRedespachoAutomaticamenteAposEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformarTransportadorSubcontratadoEtapaUm", Column = "CCG_INFORMAR_TRANSPORTADOR_SUBCONTRATADO_ETAPA_UM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarTransportadorSubcontratadoEtapaUm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCargaEspelhoAutomaticamenteAoFinalizarCarga", Column = "CCG_GERAR_CARGA_ESPELHO_AUTOMATICAMENTE_AO_FINALIZAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaEspelhoAutomaticamenteAoFinalizarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarRotaFreteInformadoPedido", Column = "CCG_UTILIZAR_ROTA_FRETE_INFORMADO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarRotaFreteInformadoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NecessitaInformarPlacaCarregamento", Column = "CCG_NECESSITA_INFORMAR_PLACA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NecessitaInformarPlacaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarVinculoMotoristaGenericoCarga", Column = "CCG_HABILITAR_VINCULO_MOTORISTA_GENERICO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarVinculoMotoristaGenericoCarga { get; set; }

        /// <summary>
        /// Ao ficar vinculada no pedido (T_PEDIDO_NOTAS_FISCAIS), irá adicionar a nota fiscal automaticamente na próxima carga que utilizar aquele pedido
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga", Column = "CCG_DISPONIBILIZAR_NOTA_FISCAL_NO_PEDIDO_AO_FINALIZAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AvancarCargaQuandoPedidoZeroPacotes", Column = "CCG_AVANCAR_CARGA_QUANDO_PEDIDO_ZERO_PACOTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvancarCargaQuandoPedidoZeroPacotes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberarPedidoComRecebedorParaMontagemCarga", Column = "CCG_LIBERAR_PEDIDO_COM_RECEBEDOR_PARA_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarPedidoComRecebedorParaMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BuscarPacoteMesmoCNPJAdicionarCargaAposConsulta", Column = "CCG_BUSCAR_PACOTE_MESMO_CNPJ_ADICIONAR_CARGA_APOS_CONSULTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarPacoteMesmoCNPJAdicionarCargaAposConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMDFeParaRecebedorDaCarga", Column = "CCG_GERAR_MDFE_PARA_RECEBEDOR_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMDFeParaRecebedorDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRotaCarga", Column = "CCG_TIPO_ROTA_CARGA", TypeType = typeof(TipoRotaCarga), NotNull = false)]
        public virtual TipoRotaCarga TipoRotaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_TIPO_OPERACAO_INTERNACIONAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoOperacaoInternacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LayoutEmailTipoPropostaTipoOperacao", Column = "CCG_LAYOUT_EMAIL_TIPO_PROPOSTA_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LayoutEmailTipoPropostaTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirAvancarCargaComTracaoSemReboque", Column = "CCG_NAO_PERMITIR_AVANCAR_CARGA_COM_TRACAO_SEM_REBOQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirAvancarCargaComTracaoSemReboque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornarSituacaoAoRemoverPedidos", Column = "CCG_RETORNAR_SITUACAO_AO_REMOVER_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarSituacaoAoRemoverPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAposRemocaoPedidos", Column = "CCG_SITUACAO_APOS_REMOCAO_PEDIDOS", TypeType = typeof(SituacaoCarga), NotNull = false)]
        public virtual SituacaoCarga SituacaoAposRemocaoPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_ROTEIRIZAR_CARGA_ETAPA_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RoteirizarCargaEtapaNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_REMARK_SPED", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.RemarkSped), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.RemarkSped? RemarkSped { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_GERAR_CARGA_DE_REDESPACHO_VINCULANDO_APENAS_UMA_NOTA_POR_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCargaDeRedespachoVinculandoApenasUmaNotaPorEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirIncluirCTesDeDiferentesUFsEmMDFeUnico", Column = "CCG_PERMITIR_INCLUIR_CTES_DE_DIFERENTES_UFS_EM_MDFE_UNICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirIncluirCTesDeDiferentesUFsEmMDFeUnico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCG_PERMITE_INFORMAR_MOTORISTAS_SOMENTE_ETAPA_UM_QUANDO_NOTAS_FISCAIS_NAO_SAO_RECEBIDAS_ANTES_CALCULAR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarMotoristasSomenteEtapaUmQuandoNotasFiscaisNaoSaoRecebidasAntesCalcularFrete { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações da Carga"; }
        }
    }
}
