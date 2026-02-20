using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_OCORRENCIA", EntityName = "ConfiguracaoOcorrencia", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia", NameType = typeof(ConfiguracaoOcorrencia))]
    public class ConfiguracaoOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_VISUALIZAR_ULTIMO_USUARIO_DELEGADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarUltimoUsuarioDelegadoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_PERMITE_INFORMAR_CENTRO_RESULTADO_APROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarCentroResultadoAprovacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_PERMITE_GERAR_OCORRENCIA_CARGA_ANULADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteGerarOcorrenciaCargaAnulada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_EXIBIR_DESTINATARIO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDestinatarioOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_ENVIAR_XMLDANFE_CLIENTE_OCORRENCIA_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarXMLDANFEClienteOcorrenciaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_PERMITE_DOWNLOAD_COMPACTADO_ARQUIVO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteDownloadCompactadoArquivoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_NAO_IMPRIMIR_TIPO_OCORRENCIA_NA_OBSERVACAO_CTE_COMPLEMENTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoImprimirTipoOcorrenciaNaObservacaoCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_EXIBIR_CAMPO_INFORMATIVO_PAGADOR_AUTORIZACAO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirCampoInformativoPagadorAutorizacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_SALVAR_DOCUMENTOS_DO_CTE_ANTERIOR_AO_IMPORTAR_CTE_COMPLEMENTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SalvarDocumentosDoCteAnteriorAoImportarCTeComplementar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_INDUZIR_TRANSPORTADOR_SELECIONAR_APENAS_COMPLEMENTO_SOLITACAO_COMPLEMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_UTILIZAR_BONIFICACAO_PARA_TRANSPORTADORES_VIA_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarBonificacaoParaTransportadoresViaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_PERMITE_INFORMAR_MAIS_DE_UMA_OCORRENCIA_POR_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteInformarMaisDeUmaOcorrenciaPorNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_GERAR_OBSERVACAO_SUBSTITUTO_SOMENTE_NUMERO_CTE_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarObservacaoSubstitutoSomenteNumeroCTeAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_PERMITIR_DEFINIR_CST_NO_TIPO_DE_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirDefinirCSTnoTipoDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_PERMITIR_ADICIONAR_MAIS_DE_UMA_OCORRENCIA_PARA_MESMO_EVENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAdicionarMaisOcorrenciaMesmoEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_HABILITAR_IMPORTACAO_OCORRENCIA_VIA_NOTFIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarImportacaoOcorrenciaViaNOTFIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_UTILIZAR_NUMERO_TENTATIVAS_TEMPO_INTERVALO_INTEGRACAO_OCORRENCIA_PERSONALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarNumeroTentativasTempoIntervaloIntegracaoOcorrenciaPersonalizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_NUMERO_TENTATIVAS_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativasIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_INTERVALO_MINUTOS_ENTRE_INTEGRACOES", TypeType = typeof(int), NotNull = false)]
        public virtual int IntervaloMinutosEntreIntegracoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_IGNORAR_SITUACAO_DAS_NOTAS_AO_GERAR_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarSituacaoDasNotasAoGerarOcorrencia { get; set; }

        [Obsolete("Criada sem necessidade, problema mudou de etapa na integração")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_GERAR_INTEGRACOES_CTE_SUBCONTRATACAO_NAS_OCORRENCIAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarIntegracoesCteSubcontratacaoNasOcorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_PERMITE_ALTERAR_OCORRENCIA_APOS_REPROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAlterarOcorrenciaAposReprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_PERMITIR_INCLUIR_OCORRENCIA_POR_SELECAO_NOTAS_FISCAIS_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirIncluirOcorrenciaPorSelecaoNotasFiscaisCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_NAO_GERAR_ATENDIMENTO_DUPLICADO_PARA_MESMA_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarAtendimentoDuplicadoParaMesmaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_TRAZER_CENTRO_RESULTADO_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TrazerCentroResultadoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_EXIBIR_TODOS_CTES_DA_CARGA_NA_AUTORIZACAO_DE_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTodosCTesDaCargaNaAutorizacaoDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_UTLIZA_USUARIO_PADRAO_PARA_GERACAO_OCORRENCIA_POR_EDI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaUsuarioPadraoParaGeracaoOcorrenciaPorEDI { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_USUARIO_PADRAO_PARA_GERACAO_OCORRENCIA_POR_EDI", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioPadraoParaGeracaoOcorrenciaPorEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_PERMITIR_VINCULO_AUTOMATICO_ENTRE_OCORRENCIA_E_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirVinculoAutomaticoEntreOcorreciaEAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COO_PERMITE_REABRIR_OCORRENCIA_EM_CASO_DE_REJEICAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirReabrirOcorrenciaEmCasoDeRejeicao { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para ocorrências"; }
        }
    }
}
