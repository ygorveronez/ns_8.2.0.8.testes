namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_GERAL", EntityName = "ConfiguracaoGeral", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral", NameType = typeof(ConfiguracaoGeral))]
    public class ConfiguracaoGeral : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_NAO_CARREGAR_PLANO_ENTRADA_SAIDA_TIPO_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoCarregarPlanoEntradaSaidaTipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_ATIVAR_CONSULTA_SEGREGACAO_POR_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarConsultaSegregacaoPorEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_PERMITIR_VINCULAR_VEICULO_MOTORISTA_VIA_PLANILHA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirVincularVeiculoMotoristaViaPlanilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_PERMITIR_CRIACAO_DIRETA_MALOTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCriacaoDiretaMalotes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_TRANSFORMAR_JANELA_DE_DESCARREGAMENTO_EM_MULTIPLA_SELECAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TransformarJanelaDeDescarregamentoEmMultiplaSelecao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_CONTROLAR_ORGANIZACAO_PRODUTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlarOrganizacaoProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_FILTRAR_PEDIDOS_SEM_FILTRO_POR_FILIAL_NO_PORTAL_DO_FORNECEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FiltrarPedidosSemFiltroPorFilialNoPortalDoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_AGRUPAR_RELATORIO_ORDEM_COLETA_GUARITA_POR_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparRelatorioOrdemColetaGuaritaPorDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_VALIDAR_TRANSPORTADOR_NAO_INFORMADO_NA_IMPORTACAO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarTransportadorNaoInformadoNaImportacaoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_PERMITIR_AGENDAMENTO_PEDIDOS_SEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAgendamentoPedidosSemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_NAO_PERMITIR_DESABILITAR_COMPRA_VALE_PEDAGIO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirDesabilitarCompraValePedagioVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_IMPRIME_ORDEM_SERVICO_CNPJ_MATRIZ", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimeOrdemServiçoCNPJMatriz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_ALTERAR_MODELO_DOCUMENTO_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlterarModeloDocumentoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_NAO_DESCONTAR_VALOR_DESCONTO_ITEM_AOS_ABASTECIMENTOS_GERADOS_DOCUMENTO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoDescontarValorDescontoItemAosAbastecimentosGeradosDocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_ENVIAR_APENAS_EMAIL_DIARIO_TAXAS_DESCARGA_PENDENTE_APROVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarApenasEmailDiarioTaxasDescargaPendenteAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_HABILITAR_ENVIO_POR_SMS_DE_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarEnvioPorSMSDeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_VISUALIZAR_GNRE_SEM_VALIDACAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarGNRESemValidacaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_NAO_PERMITIR_FINALIZAR_VIAGEM_DETALHES_FIM_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirFinalizarViagemDetalhesFimViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_PERMITIR_ADICIONAR_MOTORISTA_CARGA_MDFE_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAdicionarMotoristaCargaMDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_HABILITAR_CADASTRO_ARMAZEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarCadastroArmazem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_NAO_BLOQUEAR_EMISSAO_NFSE_MANUAL_SEM_DANFSE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoBloquearEmissaoNFSeManualSemDANFSE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRomaneio", Column = "CGE_TIPO_ROMANEIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRomaneio), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRomaneio TipoRomaneio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarRegistrosReceberGNREParaCTesComCST90", Column = "CGE_GERAR_REGISTROS_RECEBER_GNRE_PARA_CTES_COM_CST90", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarRegistrosReceberGNREParaCTesComCST90 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VisualizarPermitirAlterarDataEntregaNaConfirmacaoCanhoto", Column = "CGE_VISUALIAZAR_PERMITIR_ALTERAR_DATA_ENTREGA_CONFIRMACAO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarPermitirAlterarDataEntregaNaConfirmacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_UTILIZAR_LOCALIDADE_TOMADOR_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarLocalidadeTomadorNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGE_NAO_GERAR_OCORRENCIA_CTE_IMPORTADOS_EMAIL_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarOcorrenciaCTeImportadosEmailEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProcessarXMLNotasFiscaisAssincrono", Column = "CGE_PROCESSAR_XML_NOTA_FISCAL_ASSINCRONO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProcessarXMLNotasFiscaisAssincrono { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirCancelamentoNFSManualSeHouverIntegracao", Column = "CGE_NAO_PERMITIR_CANCELAMENTO_NFS_MANUAL_SE_HOUVER_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirCancelamentoNFSManualSeHouverIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodo", Column = "CGE_REMOVER_AUTOMATICAMENTE_REQUISICAO_ABASTECIMENTO_ABERTA_POR_PERIODO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodoDias", Column = "CGE_REMOVER_AUTOMATICAMENTE_REQUISICAO_ABASTECIMENTO_ABERTA_POR_PERIODO_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int RemoverAutomaticamenteRequisicaoAbastecimentoAbertaPorPeriodoDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirImpressaoDAMDFEContingencia", Column = "CGE_PERMITIR_IMPRESSAO_DAMDFE_CONTINGENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirImpressaoDAMDFEContingencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitarFuncionalidadesProjetoGollum", Column = "CGE_HABILITAR_FUNCIONALIDADES_PROJETO_GOLLUM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarFuncionalidadesProjetoGollum { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarCTeApenasParaTomador", Column = "CGE_ENVIAR_CTE_APENAS_PARA_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarCTeApenasParaTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteRealizarConsultaVPUtilizandoModeloVeicularCarga", Column = "CGE_PERMITE_REALIZAR_CONSULTA_VP_UTILIZANDO_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteRealizarConsultaVPUtilizandoModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteSelecionarPlacaPorTipoVeiculoTransbordo", Column = "CGE_PERMITE_SELECIONAR_PLACA_POR_TIPO_VEICULO_TRANSBORDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteSelecionarPlacaPorTipoVeiculoTransbordo { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração geral"; }
        }
    }
}
