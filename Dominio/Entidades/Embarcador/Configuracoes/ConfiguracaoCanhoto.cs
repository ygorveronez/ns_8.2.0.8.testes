using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_CANHOTO", EntityName = "ConfiguracaoCanhoto", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoCanhoto", NameType = typeof(ConfiguracaoCanhoto))]
    public class ConfiguracaoCanhoto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PRAZO_DIAS_APOS_DATA_EMISSAO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoDiasAposDataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_NOTIFICAR_CANHOTOS_PENDENTES_TODOS_OS_DIAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarCanhotosPendentesTodosOsDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_DISPONIBILIZAR_OPCAO_DE_CANHOTO_EXTRAVIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarOpcaoDeCanhotoExtraviado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PRAZO_PARA_REVERTER_DIGITALIZACAO_APOS_APROVACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoParaReverterDigitalizacaoAposAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_NAO_PERMITIR_RECEBER_CANHOTOS_NAO_DIGITALIZADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirReceberCanhotosNaoDigitalizados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_NOTIFICAR_TRANSPORTADOR_CANHOTOS_QUE_ESTAO_COM_DIGITALIZACAO_REJEITADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarTransportadorCanhotosQueEstaoComDigitalizacaoRejeitada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITIR_AUDITAR_CANHOTOS_FINALIZADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAuditarCanhotosFinalizados { get; set; }

        /// <summary>
        /// Tamanho máximo da imagem do canhoto em KB (kilobytes)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_TAMANHO_MAXIMO_DA_IMAGEM_DO_CANHOTO", TypeType = typeof(int), NotNull = false)]
        public virtual int TamanhoMaximoDaImagemDoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_VALIDAR_SITUACAO_DIGITALIZACAO_CANHOTOS_AO_SUMARIZAR_DOCUMENTO_FATURAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarSituacaoDigitalizacaoCanhotosAoSumarizarDocumentoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_MENSAGEM_RODAPE_EMAIL_CANHOTOS_PENDENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MensagemRodapeEmailCanhotosPendentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_LIBERAR_OARA_PAGAMENTOS_APOS_DIGITALIZACAO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberarParaPagamentoAposDigitalizacaCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_GERAR_CANHOTO_PARA_NOTAS_TIPO_PALLET", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCanhotoParaNotasTipoPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITIR_BLOQUEAR_DOCUMENTO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirBloquearDocumentoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITIR_ALTERAR_IMAGEM_CANHOTO_DIGITALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAlterarImagemCanhotoDigitalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_REJETAR_CANHOTOS_NAO_VALIDADOS_PELO_OCR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RejeitarCanhotosNaoValidadosPeloOCR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITIR_MULTIPLA_SELECAO_LANCAMENTO_LOTE_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirMultiplaSelecaoLancamentoLotePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_EXIGIR_DATA_ENTREGA_NOTA_CLIENTE_CANHOTOS_RECEBER_FISICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirDataEntregaNotaClienteCanhotosReceberFisicamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITIR_IMPORTAR_CANHOTO_NF_FATURADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirImportarCanhotoNFFaturada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_EXIBIR_CANHOTOS_SEM_VINCULO_COM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCanhotosSemVinculoComCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITIR_ATUALIZAR_SITUACAO_CANHOTO_POR_IMPORTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAtualizarSituacaoCanhotoPorImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITIR_ATUALIZAR_SITUACAO_CANHOTO_AVULSO_POR_IMPORTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAtualizarSituacaoCanhotoAvulsoPorImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITIR_IMPORTAR_DOCUMENTOS_FILTRO_SEM_CHAVE_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirImportarDocumentosFiltroSemChaveNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_INTEGRAR_CANHOTOS_COM_VALIDADOR_IA_COMPROVEI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarCanhotosComValidadorIAComprovei { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_OBTER_NUMERO_NOTA_FISCAL_POR_OBJETO_OCR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ObterNumeroNotaFiscalPorObjetoOcr { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_NAO_ATUALIZAR_TELA_CANHOTOS_APOS_APROVACAO_REJEICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAtualizarTelaCanhotosAposAprovacaoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_APROVAR_AUTOMATICAMENTE_A_DIGITALIZACAO_DOS_CANHOTOS_CASO_A_VALIDACAO_DA_IA_COMPROVEI_SEJA_COMPLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AprovarAutomaticamenteADigitalizacaoDosCanhotosCasoAValidacaoDaIAComproveiSejaCompleta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITIR_APROVAR_DIGITALIZACAO_DE_CANHOTO_REJEITADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAprovarDigitalizacaoDeCanhotoRejeitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_VALIDAR_SITUACAO_ENTREGA_AO_ENVIAR_IMAGEM_CANHOTO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarSituacaoEntregaAoEnviarImagemCanhotoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITIR_ENVIAR_IMAGEM_PARA_MULTIPLOS_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirEnviarImagemParaMultiplosCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_RETORNAR_METODO_BUSCAR_ENTREGAS_REALIZADAS_PENDENTE_INTEGRACAO_SOMENTE_CANHOTO_DIGITALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarMetodoBuscarEntregasRealizadasPendentesIntegracaoSomenteCanhotoDigitalizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_REENVIAR_UMA_VEZ_INTEGRACAO_CASO_RETORNAR_FALHA_NA_VALIDACAO_DO_NUMERO_DO_CANHOTO_E_OU_FORMATO_DO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReenviarUmaVezIntegracaoCasoRetornarFalhaNaValidacaoDoNumeroDoCanhotoEOuFormatoDoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_FLEXIBILIDADE_PARA_VALIDACAO_NA_IA_COMPROVEI", TypeType = typeof(double), NotNull = false)]
        public virtual double FlexibilidadeParaValidacaoNaIAComprovei { get; set; }

        [Obsolete("O campo não será mais utilizado")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_VALOR_PARA_CONSIDERAR_COMO_VALIDO_INTEGRACAO_IA_COMPROVEI", TypeType = typeof(double), NotNull = false)]
        public virtual double ValorParaConsiderarComoValido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_NAO_INTEGRAR_IA_COMPROVEI_CANHOTOS_DE_NOTAS_DEVOLVIDAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoIntegrarIAComproveiCanhotosDeNotasDevolvidas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_PERMITIR_RETORNAR_STATUS_CANHOTO_NA_API_DIGITALIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRetornarStatusCanhotoNaAPIDigitalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_HABILITAR_FLUXO_ANALISE_CANHOTO_REJEITADO_IA_COMPROVEI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarFluxoAnaliseCanhotoRejeitadoIA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_VALIDACAO_NUMERO", TypeType = typeof(double), NotNull = false)]
        public virtual double? ValidacaoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_VALIDACAO_ASSINATURA", TypeType = typeof(double), NotNull = false)]
        public virtual double? ValidacaoAssinatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_VALIDACAO_ENCONTROU_DATA", TypeType = typeof(double), NotNull = false)]
        public virtual double? ValidacaoEncontrouData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_VALIDACAO_CANHOTO", TypeType = typeof(double), NotNull = false)]
        public virtual double? ValidacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_EFETUAR_INTEGRACAO_APENAS_CANHOTOS_DIGITALIZADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EfetuarIntegracaoApenasCanhotosDigitalizados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_COMPACTAR_TAMANHO_IA_COMPROVEI_CASO_TAMANHO_ULTRAPASSE_UM_MB", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CompactarImagemCanhotoIaComproveiCasoTamanhoUltrapasseUmMB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_RETORNAR_SOMENTE_CANHOTO_COM_NFE_ENTREGUE_EM_BUSCAR_CANHOTOS_NOTAS_FISCAIS_DIGITALIZADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarSomenteCanhotoComNFeEntregueEmBuscarCanhotosNotasFiscaisDigitalizados { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração para canhoto";
            }
        }
    }
}
