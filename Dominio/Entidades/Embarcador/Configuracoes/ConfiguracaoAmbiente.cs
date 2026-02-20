
namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_AMBIENTE", EntityName = "ConfiguracaoAmbiente", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoAmbiente", NameType = typeof(ConfiguracaoAmbiente))]
    public class ConfiguracaoAmbiente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_AMBIENTE_PRODUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AmbienteProducao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_AMBIENTE_SEGURO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AmbienteSeguro { get; set; }

        //Novos
        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_IDENTIFICACAO_AMBIENTGE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string IdentificacaoAmbiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_LOCALIDADE_NAO_CADASTRADA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CodigoLocalidadeNaoCadastrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_RECALCULAR_ICMS_NA_EMISSAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? RecalcularICMSNaEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_APLICAR_VALOR_ICMS_NO_COMPLEMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AplicarValorICMSNoComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ADICIONAR_CTES_FILA_CONSULTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AdicionarCTesFilaConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_NAO_CALCULAR_DIFAL_PARA_CST_NAO_TRIBUTAVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoCalcularDIFALParaCSTNaoTributavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_NAO_UTILIZAR_COLETA_NA_BUSCA_ROTA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoUtilizarColetaNaBuscaRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CODIFICACAO_EDI", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CodificacaoEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_LINK_COTACAO_COMPRA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string LinkCotacaoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_LOGO_PERSONALIZADA_FORNECEDOR", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string LogoPersonalizadaFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_LAYOUT_PERSONALIZADO_FORNECEDOR", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string LayoutPersonalizadoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_OCULTAR_CONTEUDO_COLOG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? OcultarConteudoColog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CONSULTAR_PELO_CUSTO_DA_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ConsultarPeloCustoDaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CONCESSIONARIAS_COM_DESCONTOS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ConcessionariasComDescontos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_PERCENTUAL_DESCONTO_CONCESSIONARIAS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string PercentualDescontoConcessionarias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_PLACA_PADRAO_CONSULTA_VALOR_PEDAGIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string PlacaPadraoConsultaValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CALCULAR_HORARIO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CalcularHorarioDoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENVIAR_NOTIFICACAO_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EnviarTodasNotificacoesPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_OCR_API_LINK", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string APIOCRLink { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_OCR_API_KEY", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string APIOCRKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_QUANTIDADE_SELECAO_AGRUPAMENTO_CARGA_AUTOMATICO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string QuantidadeSelecaoAgrupamentoCargaAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_QUANTIDADE_CARGAS_AGRUPAMENTO_CARGA_AUTOMATICO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string QuantidadeCargasAgrupamentoCargaAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_HORA_EXECUCAO_THREAD_DIARIA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string HorarioExecucaoThreadDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CALCULAR_FRETE_FECHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CalcularFreteFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_GERAR_DOCUMENTO_FECHAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarDocumentoFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_NOVO_LAYOUT_PORTAL_FORNECEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NovoLayoutPortalFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_NOVO_LAYOUT_CABOTAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NovoLayoutCabotagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_FORNECEDOR_TMS", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string FornecedorTMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_UTILIZAR_INTEGRACAO_SAINTGOBAIN_NOVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarIntegracaoSaintGobainNova { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_FILTRAR_CARGAS_POR_PROPRIETARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? FiltrarCargasPorProprietario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_HABILITAR_INTEGRACAO_CARGA_FLUVIAL_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CargaControleEntrega_Habilitar_ImportacaoCargaFluvial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_TIPO_ARMAZENAMENTO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string TipoArmazenamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_TIPO_ARMAZENAMENTO_LEITOR_OCR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string TipoArmazenamentoLeitorOCR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENDERECO_FTP", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string EnderecoFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_USUARIO_FTP", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string UsuarioFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_SENHA_FTP", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string SenhaFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_PORTA_FTP", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string PortaFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_PREFIXO_FTP", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string PrefixosFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_EMAILS_FTP", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string EmailsFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_FTP_PASSIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? FTPPassivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_UTILIZA_SFTP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizaSFTP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_GERAR_NOTIFIS_POR_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarNotFisPorNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CODIGO_EMPRESA_MULTISOFTWARE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CodigoEmpresaMultisoftware { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_MINUTOS_PARA_CONSULTA_NATURA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MinutosParaConsultaNatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_FILIAIS_NATURA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string FiliaisNatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_UTILIZAR_METODO_IMPORTACAO_TABELA_FRETE_POR_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarMetodoImportacaoTabelaFretePorServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_UTILIZAR_IMPORTACAO_TABELA_FRETE_GPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarLayoutImportacaoTabelaFreteGPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_EXIBIR_SITUACAO_INTEGRACAO_XML_GPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ExibirSituacaoIntegracaoXMLGPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_WS_CONSULTA_CTE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string WebServiceConsultaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_PROCESSAR_CTE_NO_MULTICTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ProcessarCTeMultiCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_MAO_UTILIZAR_CNPJ_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoUtilizarCNPJTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_BUSCAR_FILIAL_POR_CNPJ_REMETENTE_dESTINATARIO_GERAR_CARGA_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_SEMPRE_USAR_ATIVIDADE_CLIENTE_CADASTRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SempreUsarAtividadeCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ATUALIZAR_FANTASIA_CLIENTE_CADASTRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AtualizarFantasiaClienteIntegracaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CADASTRAR_MOTORISTA_INTEGRACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CadastrarMotoristaIntegracaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CTE_UTILIZA_PROPRIETARIO_CADASTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CTeUtilizaProprietarioCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CTE_CARREGAR_VINCULO_VEICULO_CADASTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CTeCarregarVinculosVeiculosCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CTE_ATUALIZA_TIPO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CTeAtualizaTipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CTE_NAO_ATUALIZAR_CADASTRO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoAtualizarCadastroVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_AGRUPAR_QUANTIDADES_IMPORTACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AgruparQuantidadesImportacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENCERRA_MDFE_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EncerraMDFeAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENVIA_CONTINGENCIA_MDFE_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EnviaContingenciaMDFeAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENVIAR_CERTIFICADO_ORACLE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EnviarCertificadoOracle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENVIAR_CERTIFICADO_KEY_VAULT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EnviarCertificadoKeyVault { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_PERMITIR_INFORMAR_CAPACIDADE_MAXIMA_PARA_UPLOAD_ARQUIVOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarCapacidadeMaximaParaUploadArquivos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CAPACIDADE_MAXIMA_PARA_UPLOAD_ARQUIVOS", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeMaximaParaUploadArquivos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_EMPRESAS_USUARIOS_MULTI_CTE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string EmpresasUsuariosMultiCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_LIMPAR_MOTORISTA_INTEGRACAO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? LimparMotoristaIntegracaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_LOGIN_AD", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? LoginAD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_REGERAR_DACTE_ORACLE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? RegerarDACTEOracle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_REENVIAR_ERRO_INTEGRACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ReenviarErroIntegracaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ATUALIZAR_TIPO_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AtualizarTipoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_VALIDAR_NFE_JA_IMPORTADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ValidarNFeJaImportada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_UTILIZA_OPTANTE_SIMPLES_NACIONAL_DA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizaOptanteSimplesNacionalDaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_REENVIAR_ERRO_INTEGRACAO_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ReenviarErroIntegracaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENCERRA_MDFE_AUTOMATICO_COM_MESMA_DATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EncerraMDFeAutomaticoComMesmaData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENCERRA_MDFE_ANTES_DA_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EncerraMDFeAntesDaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENCERRA_MDFE_AUTOMATICO_OUTROS_SISTEMAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EncerraMDFeAutomaticoOutrosSistemas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENVIAR_EMAIL_MDFE_CLIENTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EnviarEmailMDFeClientes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_PESO_MAXIMO_INTEGRACAO_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? PesoMaximoIntegracaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_UTILIZAR_DOCA_DO_COMPLEMENTO_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizarDocaDoComplementoFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_RETORNAR_MODELO_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? RetornarModeloVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_MDFE_UTILIZA_DADOS_VEICULO_CADASTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? MDFeUtilizaDadosVeiculoCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_MDFE_UTILIZA_VEICULO_REBOQUE_COMO_TRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? MDFeUtilizaVeiculoReboqueComoTracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_GERAR_CTE_DAS_NFSE_AUTORIZADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarCTeDasNFSeAutorizadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_INCLUIR_ISS_NFSE_LOCALIDADE_TOMADOR_DIFERENTE_PRESTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? IncluirISSNFSeLocalidadeTomadorDiferentePrestador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_INTEGRACAO_NFSE_UTILIZA_ALIQUOTA_MULTI_CTE_QUANDO_TRANSPORTADOR_SIMPLES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ATUALIZAR_VALOR_FRETE_ATUALIZAR_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AtualizarValorFrete_AtualizarICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CONSULTAR_DUPLICIDADE_ORACLE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ConsultarDuplicidadeOracle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENVIAR_INTEGRACAO_MAGALOG_NO_RETORNO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EnviarIntegracaoMagalogNoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENVIAR_INTEGRACAO_ERRO_MDFE_MAGALOG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EnviarIntegracaoErroMDFeMagalog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_INTERVALO_DOCUMENTOS_FISCAIS_EMBARCADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int? IntervaloDocumentosFiscaisEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_PREFIXO_MSMQ", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string PrefixoMSMQ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENDERECO_COMPUTADOR_REMOTO_FILA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string EnderecoComputadorRemotoFila { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ENDPOINT_SERVICE_FILA", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string EndpointServiceFila { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_URL_REPORT_API", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlReportAPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_AUTOSCALING", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Autoscaling { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_CHAVE_PARCEIRO_MIGRATE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ChaveParceiroMigrate { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoProtocolo", Column = "COA_TIPO_PROTOCOLO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoProtocolo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProtocolo TipoProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_DESABILITAR_POPUPS_NOTIFICACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? DesabilitarPopUpsDeNotificacao { get; set; }

    }
}
