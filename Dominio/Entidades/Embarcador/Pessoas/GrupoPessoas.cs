using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS", EntityName = "GrupoPessoas", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas", NameType = typeof(GrupoPessoas))]
    public class GrupoPessoas : EntidadeBase, IEquatable<GrupoPessoas>, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GRP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "GRP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "GRP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        /// <summary>
        /// Da aba Configuração Emissão, que é um componente com o cadastro de Tipo de Operação e Pessoas
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoGrupoPessoasEmissao", Column = "CGE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasEmissao ConfiguracaoEmissao { get; set; }

        /// <summary>
        /// Da aba Configuração Fatura, que é um componente com o cadastro de Pessoas
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoGrupoPessoasFatura", Column = "CGF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasFatura ConfiguracaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoCTeDocumentos", Column = "GRP_TIPO_EMISSAO_CTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos TipoEmissaoCTeDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO_EXCLUSIVO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormulaExclusivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoCTeDocumentosExclusivo", Column = "GRP_TIPO_EMISSAO_CTE_CLIENTE_EXCLUSIVO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos? TipoEmissaoCTeDocumentosExclusivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoCTeParticipantes", Column = "GRP_TIPO_EMISSA_CTE_PARTICIPANTES", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes TipoEmissaoCTeParticipantes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoImportacaoNotaFiscal", Column = "AIN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal ArquivoImportacaoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_EMPRESA_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CTeEmitidoNoEmbarcador", Column = "GRP_CTE_EMITIDO_NO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CTeEmitidoNoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirNumeroPedido", Column = "GRP_EXIGIR_NUMERO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirNumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarArquivoVgm", Column = "GRP_NAO_GERAR_ARQUIVO_VGM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarArquivoVgm { get; set; }

        [Obsolete("Migrado para lista DiasSemanaFatura")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaSemana", Column = "GRP_DIA_SEMANA_FATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.DiaSemana? DiaSemana { get; set; }

        [Obsolete("Migrado para lista DiasMesFatura")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaMesFatura", Column = "GRP_DIA_MES_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiaMesFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_FORMA_GERACAO_TITULO_FATURA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaGeracaoTituloFatura), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaGeracaoTituloFatura? FormaGeracaoTituloFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteFinalDeSemana", Column = "GRP_PERMITE_FINAL_SEMANA_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PermiteFinalDeSemana { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeCanhotoFisico", Column = "GRP_EXIGE_CANHOTO_FISICO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ExigeCanhotoFisico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_ARMAZENA_CANHOTO_FISICO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ArmazenaCanhotoFisicoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomenteOcorrenciasFinalizadoras", Column = "GRP_SOMENTE_OCORRENCIA_FINALIZADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SomenteOcorrenciasFinalizadoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturarSomenteOcorrenciasFinalizadoras", Column = "GRP_FATURAR_SOMENTE_OCORRENCIA_FINALIZADORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? FaturarSomenteOcorrenciasFinalizadoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarFaturaAteReceberCanhotos", Column = "GRP_NAO_GERAR_FATURA_ATE_RECEBER_CANHOTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoGerarFaturaAteReceberCanhotos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoImportarDocumentoDestinadoTransporte", Column = "GRP_NAO_IMPORTAR_DOCUMENTO_DESTINADO_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoImportarDocumentoDestinadoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidaPlacaNFe", Column = "GRP_VALIDA_PLACA_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidaPlacaNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidaOrigemNFe", Column = "GRP_VALIDA_ORIGEM_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidaOrigemNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArmazenaProdutosXMLNFE", Column = "GRP_ARMAZENA_PRODUTO_XML_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArmazenaProdutosXMLNFE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidaDestinoNFe", Column = "GRP_VALIDA_DESTINO_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidaDestinoNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidaEmitenteNFe", Column = "GRP_VALIDA_EMITENTE_NFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidaEmitenteNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LerNumeroPedidoDaObservacaoDaNota", Column = "GRP_LER_NUMERO_PEDIDO_DA_OBSERVACAO_DA_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LerNumeroPedidoDaObservacaoDaNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_CONTROLA_PAGAMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlaPagamentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_VINCULAR_NOTA_FISCAL_EMAIL_NA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularNotaFiscalEmailNaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_NUMERO_PEDIDO_OBSERVACAO_CONTRIBUINTE_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LerNumeroPedidoObservacaoContribuinteNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_IDENTIFICADOR_NUMERO_PEDIDO_OBSERVACAO_CONTRIBUINTE_NOTA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IdentificadorNumeroPedidoObservacaoContribuinteNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLeituraNumeroCargaNotaFiscal", Column = "GRP_TIPO_LEITURA_NUMERO_CARGA_NOTA_FISCAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLeituraNumeroCargaNotaFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLeituraNumeroCargaNotaFiscal TipoLeituraNumeroCargaNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteTomadorFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoFatura", Column = "GRP_OBSERVACAO_FATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCadastroContaBancaria", Column = "GPO_UTILIZAR_CADASTRO_CONTA_BANCARIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCadastroContaBancaria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "GRP_BANCO_AGENCIA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "GRP_BANCO_DIGITO_AGENCIA", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "GRP_BANCO_NUMERO_CONTA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaBanco", Column = "GRP_BANCO_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco? TipoContaBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPrazoFaturamento", Column = "GRP_TIPO_PRAZO_FATURAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento? TipoPrazoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasDePrazoFatura", Column = "GRP_DIA_DE_PRAZO_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasDePrazoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgruparMovimentoFinanceiroPorPedido", Column = "GRP_AGRUPAR_MOVIMENTO_FINANCEIRO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparMovimentoFinanceiroPorPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_DESCRICAO_COMPONENTE_FRETE_EMBARCADOR", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string DescricaoComponenteFreteEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarTituloPorDocumentoFiscal", Column = "GRP_GERAR_TITULO_POR_DOCUMENTO_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloPorDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_NAO_VALIDAR_NOTA_FISCAL_EXISTENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarNotaFiscalExistente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_NAO_VALIDAR_NOTAS_FISCAIS_COM_DIFERENTES_PORTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarNotasFiscaisComDiferentesPortos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_VALE_PEDAGIO_OBRIGATORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValePedagioObrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPagamentoRecebimento", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoPagamentoRecebimento FormaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoGrupoPessoas", Column = "GRP_TIPO_GRUPO_PESSOAS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoGrupoPessoas), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoGrupoPessoas TipoGrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissaoIntramunicipal", Column = "GRP_TIPO_EMISSAO_INTRAMUNICIPAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal TipoEmissaoIntramunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_CONTATO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Contato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_TELEFONE_CONTATO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TelefoneContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_ENVIAR_XML_CTE_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarXMLCTePorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_TITULO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_FATURA_AUTOMATICA_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFaturaAutomaticaCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_FATURAMENTO_A_VISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFaturamentoAVista { get; set; }

        /// <summary>
        /// Utilizar outro modelo de documento quanto for emissão municipal (NFS-e ou NFS Manual)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_UTILIZAR_OUTRO_MODELO_DOCUMENTO_EMISSAO_MUNICIPAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarOutroModeloDocumentoEmissaoMunicipal { get; set; }

        /// <summary>
        /// Modelo de documento fiscal para emissões municipais
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO_EMISSAO_MUNICIPAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalEmissaoMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_NAO_EMITIR_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEmitirMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_TORNAR_PEDIDOS_PRIORITARIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TornarPedidosPrioritarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_PROVISIONAR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ProvisionarDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERA_SOMENTE_PROVISAO_CARGA_COMPLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarSomenteUmaProvisaoCadaCargaCompleta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_DISPONIBILIZAR_DOCUMENTOS_PARA_LOTE_ESCRITURACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaLoteEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_DISPONIBILIZAR_DOCUMENTOS_PARA_LOTE_ESCRITURACAO_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaLoteEscrituracaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_DISPONIBILIZAR_DOCUMENTOS_PARA_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_QUITAR_DOCUMENTO_AUTOMATICAMENTE_AO_GERAR_LOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool QuitarDocumentoAutomaticamenteAoGerarLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_ESCRITURAR_SOMENTE_DOCUMENTOS_EMITIDOS_PARA_NFES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EscriturarSomenteDocumentosEmitidosParaNFe { get; set; }

        /// <summary>
        /// Quado setado, deve emitir as cargas de Redespacho ou Redespacho Intermediário sempre como Redespacho.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_EMITIR_SEMPRE_COMO_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirSempreComoRedespacho { get; set; }

        /// <summary>
        /// Caso o valor do frete enviado pelo embarcador seja diferente do valor calculado pela tabela de frete, é necessário autorização para emissão
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_BLOQUEAR_DIFERENCA_FRETE_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearDiferencaValorFreteEmbarcador { get; set; }

        /// <summary>
        /// Só bloqueia se a diferença do valor do frete enviado pelo embarcador com o valor calculado pela tabela de frete seja maior que um percentual
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_PERCENTUAL_BLOQUEAR_DIFERENCA_FRETE_EMBARCADOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualBloquearDiferencaValorFreteEmbarcador { get; set; }

        /// <summary>
        /// Caso exija a emissão automática de um complemento da diferença do valor do frete enviado pelo embarcador com o valor calculado pela tabela de frete
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_EMITIR_COMPLEMENTO_DIFERENCA_FRETE_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmitirComplementoDiferencaFreteEmbarcador { get; set; }

        /// <summary>
        /// Tipo de ocorrência para a emissão automática de um complemento da diferença do valor do frete enviado pelo embarcador com o valor calculado pela tabela de frete
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_COMPLEMENTO_DIFERENCA_FRETE_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaComplementoDiferencaFreteEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_OCORRENCIA_SEM_TABELA_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaSemTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_TIPO_OCORRENCIA_SEM_TABELA_FRETE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaSemTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirRotaParaEmissaoDocumentos", Column = "GRP_EXIGIR_ROTA_PARA_EMISSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirRotaParaEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarPedidoColeta", Column = "GRP_GERAR_PEDIDO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarPedidoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarOcorrenciaControleEntrega", Column = "GRP_GERAR_OCORRENCIA_CONTROLE_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaControleEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirConsultarOcorrenciaControleEntregaWebService", Column = "GRP_PERMITIR_CONSULTAR_OCORRENCIA_CONTROLE_ENTREGA_WEB_SERVICE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirConsultarOcorrenciaControleEntregaWebService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParquearDocumentosAutomaticamenteParaCNPJDesteGrupo", Column = "GRP_PARQUEAR_DOCUMENTOS_AUTOMATICAMENTE_PARA_CNPJS_DESTE_GRUPO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ParquearDocumentosAutomaticamenteParaCNPJDesteGrupo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoEnviarXMLCteSubcontratacaoOuRedespachoPorEmail", Column = "GRP_NAO_ENVIAR_XML_CTE_SUBCONTRATACAO_REDESPACHO_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarXMLCteSubcontratacaoOuRedespachoPorEmail { get; set; }

        /// <summary>
        /// Caso exija a lista de rotas de frete (caso da platlog na tombini) para emissão dos documentos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_EXIGIR_ROTA_CALCULO_FRETE_PARA_EMISSAO_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirRotaCalculoFreteParaEmissaoDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_MDFE_TRANSBORDO_SEM_CONSIDERAR_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMDFeTransbordoSemConsiderarOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_IMPORTAR_REDESPACHO_INTERMEDIARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportarRedespachoIntermediario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EMITENTE_IMPORTACAO_REDESPACHO_INTERMEDIARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente EmitenteImportacaoRedespachoIntermediario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR_IMPORTACAO_REDESPACHO_INTERMEDIARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ExpedidorImportacaoRedespachoIntermediario { get; set; }

        /// <summary>
        /// Descrição do item utilizado para obter o peso do CT-e para subcontratação.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_DESCRICAO_ITEM_PESO_CTE_SUBCONTRATACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DescricaoItemPesoCTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_DESCRICAO_CARAC_TRANSP_CTE", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CaracteristicaTransporteCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR_IMPORTACAO_REDESPACHO_INTERMEDIARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente RecebedorImportacaoRedespachoIntermediario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_RECEBEDOR_COLETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente RecebedorColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacaoColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_REGEX_VALIDACAO_NUMERO_PEDIDO_EMBARCADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RegexValidacaoNumeroPedidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_PRODUTO_PREDOMINANTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ProdutoPredominante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_OBSERVACAO_NFE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoNfe { get; set; }

        /// <summary>
        /// Deve ser utilizada como informação na etapa de frete da carga.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_OBSERVACAO_EMISSAO_CARGA", TypeType = typeof(string), Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoEmissaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_TIPO_ENVIO_EMAIL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioEmailCTe), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioEmailCTe? TipoEnvioEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_VALOR_MAXIMO_EMISSAO_PENDENTE_PAGAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorMaximoEmissaoPendentePagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_VALOR_LIMITE_FATURAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorLimiteFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasEmAbertoAposVencimento", Column = "GRP_DIA_EM_ABERTO_APOS_VENCIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasEmAbertoAposVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_UTILIZA_MULTIEMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizaMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PessoaClassificacao", Column = "PCL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PessoaClassificacao Classificacao { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_CTE_EMITIDO_EMBARCADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrenciaCTeEmitidoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_PLACA_DA_OBSERVACAO_DA_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LerPlacaDaObservacaoDaNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_PLACA_DA_OBSERVACAO_DA_NOTA_INICIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string LerPlacaDaObservacaoDaNotaInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_PLACA_DA_OBSERVACAO_DA_NOTA_FIM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string LerPlacaDaObservacaoDaNotaFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_PLACA_DA_OBSERVACAO_CONTRIBUINTE_DA_NOTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LerPlacaDaObservacaoContribuinteDaNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_PLACA_DA_OBSERVACAO_CONTRIBUINTE_DA_NOTA_IDENTIFICACAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string LerPlacaDaObservacaoContribuinteDaNotaIdentificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_NUMERO_PEDIDO_OBSERVACAO_NOTA_INICIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string LerNumeroPedidoDaObservacaoDaNotaInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_NUMERO_PEDIDO_OBSERVACAO_NOTA_FIM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string LerNumeroPedidoDaObservacaoDaNotaFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_VEICULO_OBSERVACAO_NOTA_PARA_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? LerVeiculoObservacaoNotaParaAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_PLACA_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_INICIAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerPlacaObservacaoNotaParaAbastecimentoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_PLACA_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_FINAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerPlacaObservacaoNotaParaAbastecimentoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_CHASSI_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_INICIAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerChassiObservacaoNotaParaAbastecimentoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_CHASSI_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_FINAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerChassiObservacaoNotaParaAbastecimentoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_KM_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_INICIAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerKMObservacaoNotaParaAbastecimentoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_KM_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_FINAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerKMObservacaoNotaParaAbastecimentoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_HORIMETRO_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_INICIAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerHorimetroObservacaoNotaParaAbastecimentoInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_HORIMETRO_OBSERVACAO_NOTA_PARA_ABASTECIMENTO_FINAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string LerHorimetroObservacaoNotaParaAbastecimentoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_EXPRESSAO_BOOKING", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ExpressaoBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_EXPRESSAO_CONTAINER", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ExpressaoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarFaturaPorCte", Column = "GRP_GERAR_FATURA_POR_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFaturaPorCte { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "GrupoPessoasRaizesCNPJ", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_RAIZ_CNPJ")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoasRaizCNPJ", Column = "GRC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ> GrupoPessoasRaizesCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "LayoutsEDI", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_LAYOUT_EDI")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoasLayoutEDI", Column = "GLY_CODIGO")]
        public virtual IList<Embarcador.Pessoas.GrupoPessoasLayoutEDI> LayoutsEDI { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual IList<Dominio.Entidades.Cliente> Clientes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ApolicesSeguro", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_APOLICE_SEGURO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ApoliceSeguro", Column = "APS_CODIGO")]
        public virtual ICollection<Embarcador.Seguros.ApoliceSeguro> ApolicesSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RaizesCNPJ", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_RAIZ_CNPJ")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoasRaizCNPJ", Column = "GRC_CODIGO")]
        public virtual IList<Embarcador.Pessoas.GrupoPessoasRaizCNPJ> RaizesCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosReboque", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOA_MODELO_REBOQUE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Cargas.ModeloVeicularCarga> ModelosReboque { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "AutorizadosDownloadDFe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_AUTORIZADO_DOWNLOAD_DFE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> AutorizadosDownloadDFe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "GrupoPessoasConfiguracaoComponentesFretes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_CONFIGURACAO_COMPONENTE_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoasConfiguracaoComponentesFrete", Column = "GRC_CODIGO")]
        public virtual IList<Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete> GrupoPessoasConfiguracaoComponentesFretes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Contatos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PESSOA_CONTATO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PessoaContato", Column = "PCO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Contatos.PessoaContato> Contatos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Vendedores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_FUNCIONARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoasFuncionario", Column = "GPF_CODIGO")]
        public virtual IList<GrupoPessoasFuncionario> Vendedores { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "FormulasObservacaoNfe", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_OBSERVACAO_NFE_FORMULA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoasObservacaoNfeFormula", Column = "GOF_CODIGO")]
        public virtual IList<GrupoPessoasObservacaoNfeFormula> FormulasObservacaoNfe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "MensagemAlerta", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_MENSAGEM_ALERTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoaMensagemAlerta", Column = "GPM_CODIGO")]
        public virtual IList<GrupoPessoaMensagemAlerta> MensagemAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCobrancaMultimodal", Column = "GRP_TIPO_COBRANCA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal TipoCobrancaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModalPropostaMultimodal", Column = "GRP_MODAL_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal ModalPropostaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoMultimodal", Column = "GRP_TIPO_SERVICO_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal TipoServicoMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropostaMultimodal", Column = "GRP_TIPO_PROPOSTA_MULTIMODAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal TipoPropostaMultimodal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmissaoDosDestinatario", Column = "GRP_BLOQUEAR_EMISSAO_DESTINATARIOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearEmissaoDosDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEmissaoDeEntidadeSemCadastro", Column = "GRP_BLOQUEAR_EMISSAO_ENTIDADES_SEM_CADASTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? BloquearEmissaoDeEntidadeSemCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirNumeroControleCliente", Column = "GRP_EXIGIR_NUMERO_CONTROLE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirNumeroControleCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReplicarNumeroControleCliente", Column = "GRP_REPLICAR_NUMERO_CONTROLE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReplicarNumeroControleCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirNumeroNumeroReferenciaCliente", Column = "GRP_EXIGIR_NUMERO_REFERENCIA_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirNumeroNumeroReferenciaCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReplicarNumeroReferenciaTodasNotasCarga", Column = "GRP_REPLICAR_NUMERO_REFERENCIA_TODAS_NOTAS_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReplicarNumeroReferenciaTodasNotasCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesBloquearEmissaoDosDestinatario", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_CLIENTE_BLOQUEAR_EMISSAO_DESTINATARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF_DESTINATARIO")]
        public virtual ICollection<Cliente> ClientesBloquearEmissaoDosDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssuntoEmailFatura", Column = "GRP_ASSUNTO_EMAIL_FATURA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string AssuntoEmailFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorpoEmailFatura", Column = "GRP_CORPO_EMAIL_FATURA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CorpoEmailFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_BOLETO_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarBoletoAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_ENVIAR_ARQUIVOS_DESCOMPACTADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarArquivosDescompactados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_NAO_ENVIAR_EMAIL_FATURA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarEmailFaturaAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEnvioFatura", Column = "GRP_TIPO_ENVIO_FATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura? TipoEnvioFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAgrupamentoFatura", Column = "GRP_TIPO_AGRUPAMENTO_FATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura? TipoAgrupamentoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DiasSemanaFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_DIA_SEMANA_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "GRP_DIA_SEMANA_FATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> DiasSemanaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "DiasMesFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_DIA_MES_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "GRP_DIA_MES_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual ICollection<int> DiasMesFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_HABILITAR_INTEGRACAO_VEICULO_MULTIEMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? HabilitarIntegracaoVeiculoMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_HABILITAR_INTEGRACAO_DIGITALIZACAO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? HabilitarIntegracaoDigitalizacaoCanhotoMultiEmbarcador { get; set; }

        /// <summary>
        /// inicialmente vamos usar apenas para registrar as ocorrencias sem valor (ocorrencias do controle de entrega) mas posteriormente podemos utilizar para todas, só que nesse caso tem que ter o retorno da ocorrencia também.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_HABILITAR_INTEGRACAO_OCORRENCIA_MULTI_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? HabilitarIntegracaoOcorrenciasMultiEmbarcador { get; set; }

        //integracao do TMS para com o MultiEmbarcador, (visivel apenas para o TMS)
        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_HABILITAR_INTEGRACAO_OCORRENCIA_TMS_WS_MULTI_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? HabilitarIntegracaoOcorrenciasTMSWSMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_HABILITAR_INTEGRACAO_XML_CTE_MULTI_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? HabilitarIntegracaoXmlCteMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_DATA_INICIAL_INTEGRACAO_OCORRENCIA_TMS_WS_MULTIEMBARCADOR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialIntegracaoOcorrenciaTMSWSMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_NAO_GERAR_OCORRENCIA_APENAS_IMPORTAR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? NaoGerarOcorreciaApenasDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_TOKEN_INTEGRACAO_MULTIEMBARCADOR", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TokenIntegracaoMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOP_URL_INTEGRACAO_MULTIEMBARCADOR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string URLIntegracaoMultiEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_UTILIZA_META_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? UtilizaMetaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_META_EMISSAO_MENSAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? MetaEmissaoMensal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_META_EMISSAO_ANUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? MetaEmissaoAnual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_BLOQUEADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Bloqueado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_MOTIVO_BLOQUEIO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string MotivoBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_DATA_BLOQUEIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBloqueio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_OBSERVACAO_CTE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_OBSERVACAO_CTE_TERCEIRO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoCTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_CIOT_PARA_TODAS_AS_CARGAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCIOTParaTodasAsCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_NUMERO_PEDIDO_OBSERVACAO_MDFE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LerNumeroPedidoObservacaoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_REGEX_NUMERO_PEDIDO_OBSERVACAO_MDFE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RegexNumeroPedidoObservacaoMDFe { get; set; }

        // Regex numero pedido ct-e = \s\d{10,}\s
        // Regex numero pedido mdf-e = \d{10,}\s

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_NUMERO_PEDIDO_OBSERVACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LerNumeroPedidoObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_REGEX_NUMERO_PEDIDO_OBSERVACAO_CTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RegexNumeroPedidoObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_VINCULAR_MDFE_PELO_NUMERO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularMDFePeloNumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_VINCULAR_CTE_PELO_NUMERO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularCTePeloNumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_SETAR_NUMERO_PEDIDO_EMBARCADOR_PELO_NUMERO_PEDIDO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SetarNumeroPedidoEmbarcadorPeloNumeroPedidoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_NOMENCLATURA_ARQUIVOS_DOWNLOAD_CTE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeNomenclaturaArquivosDownloadCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_NUMERO_PEDIDO_OBSERVACAO_CTE_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LerNumeroPedidoObservacaoCTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_VINCULAR_CTE_PELO_NUMERO_PEDIDO_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularCTeSubcontratacaoPeloNumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_REGEX_NUMERO_PEDIDO_OBSERVACAO_CTE_SUBCONTRATACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RegexNumeroPedidoObservacaoCTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_EMAIL_FATURA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string EmailFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_NUMERO_CONHECIMENTO_NO_BOLETO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarNumeroConhecimentoNoBoleto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_NUMERO_FATURA_NO_BOLETO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarNumeroFaturaNoBoleto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_NUMERO_CARGA_OBSERVACAO_CTE_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LerNumeroCargaObservacaoCTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_REGEX_NUMERO_CARGA_OBSERVACAO_CTE_SUBCONTRATACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RegexNumeroCargaObservacaoCTeSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_FATURAMENTO_MULTIPLA_PARCELA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFaturamentoMultiplaParcela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_QUANTIDADE_PARCELAS_FATURAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string QuantidadeParcelasFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_EMAIL_ENVIO_NOVO_VEICULO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailEnvioNovoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_ENVIAR_NOVO_VEICULO_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarNovoVeiculoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_LER_PDF_NOTA_FISCAL_RECEBIDA_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LerPDFNotaFiscalRecebidaPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTitulo", Column = "GRP_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo? FormaTitulo { get; set; }

        #region Perfil Chamado

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_CLIENTE_NAO_NECESSITA_AUTORIZACAO_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ClienteNaoNecessitaAutorizacaoAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERA_NUMERO_OCORRENCIA_AUTORIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeraNumeroOcorrenciaAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaPrazoCobrancaChamado", Column = "GRP_DIA_PRAZO_COBRANCA_CHAMADO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaPrazoCobrancaChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeMaximaDiasDataReciboAbertura", Column = "GRP_QUANTIDADE_MAXIMO_DIAS_DATA_RECIBO_ABERTURA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeMaximaDiasDataReciboAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMaximoDiferencaRecibo", Column = "GRP_VALOR_MAXIMO_DIFERENCA_RECIBO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorMaximoDiferencaRecibo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssuntoEmailChamado", Column = "GRP_ASSUNTO_EMAIL_CHAMADO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string AssuntoEmailChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorpoEmailChamado", Column = "GRP_CORPO_EMAIL_CHAMADO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string CorpoEmailChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemPadraoOrientacaoMotorista", Column = "GRP_MENSAGEM_PADRAO_ORIENTACAO_MOTORISTA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemPadraoOrientacaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaValorDescarga", Column = "GRP_FORMA_VALOR_DESCARGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaValorDescarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaValorDescarga? FormaValorDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaAberturaOcorrencia", Column = "GRP_FORMA_ABERTURA_OCORRENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaAberturaOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaAberturaOcorrencia? FormaAberturaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPrazoCobrancaChamado", Column = "GRP_TIPO_PRAZO_COBRANCA_CHAMADO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoCobrancaChamado), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoCobrancaChamado? TipoPrazoCobrancaChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelToleranciaValorCliente", Column = "GRP_NIVEL_TOLERANCIA_VALOR_CLIENTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NivelToleranciaValor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NivelToleranciaValor NivelToleranciaValorCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelToleranciaValorMotorista", Column = "GRP_NIVEL_TOLERANCIA_VALOR_MOTORISTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NivelToleranciaValor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NivelToleranciaValor NivelToleranciaValorMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TabelaValores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_PERFIL_TABELA_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoasPerfilTabelaValor", Column = "GFT_CODIGO")]
        public virtual IList<GrupoPessoasPerfilTabelaValor> TabelaValores { get; set; }

        #endregion

        #region Envio E-mail em lote

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_EMAIL_ENVIO_DOCUMENTACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string EmailEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_ASSUNTO_EMAIL_DOCUMENTACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string AssuntoDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_CORPO_EMAIL_DOCUMENTACAO", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string CorpoEmailDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAgrupamentoEnvioDocumentacao", Column = "GRP_TIPO_AGRUPAMENTO_ENVIO_DOCUMENTACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao? TipoAgrupamentoEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaEnvioDocumentacao", Column = "GRP_FORMA_ENVIO_DOCUMENTACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao? FormaEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_EMAIL_ENVIO_DOCUMENTACAO_PORTA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string EmailEnvioDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_ASSUNTO_EMAIL_DOCUMENTACAO_PORTA", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string AssuntoDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_CORPO_EMAIL_DOCUMENTACAO_PORTA", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string CorpoEmailDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAgrupamentoEnvioDocumentacaoPorta", Column = "GRP_TIPO_AGRUPAMENTO_ENVIO_DOCUMENTACAO_PORTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoEnvioDocumentacao? TipoAgrupamentoEnvioDocumentacaoPorta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaEnvioDocumentacaoPorta", Column = "GRP_FORMA_ENVIO_DOCUMENTACAO_PORTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaEnvioDocumentacao? FormaEnvioDocumentacaoPorta { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoasAnexo", Column = "ANX_CODIGO")]
        public virtual IList<GrupoPessoasAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "EmailsModeloDocumento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_MODELO_DOCUMENTO_EMAIL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GPD_GRUPO_PESSOA")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoasModeloDocumentoEmail", Column = "GPD_CODIGO")]
        public virtual IList<GrupoPessoasModeloDocumentoEmail> EmailsModeloDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraCotacaoFeeder", Column = "GRP_REGRA_COTACA_FEEDER", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.RegraCotacaoFeeder), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.RegraCotacaoFeeder? RegraCotacaoFeeder { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_VALOR_FRETE_LIQUIDO_DEVE_SER_VALOR_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorFreteLiquidoDeveSerValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_VALOR_FRETE_LIQUIDO_DEVE_SER_VALOR_A_RECEBER_SEM_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorFreteLiquidoDeveSerValorAReceberSemICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_OCORRENCIA_COMPLEMENTO_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaComplementoSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO_COMPLEMENTO_SUBCONTRATACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrenciaComplementoSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoTipoPagamento", Column = "PTP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento PedidoTipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarDuplicataNotaEntrada", Column = "GRP_GERAR_DUPLICATA_NOTA_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarDuplicataNotaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarParametrizacaoDeHorariosNoAgendamento", Column = "GRP_UTILIZAR_PARAMETRIZACAO_DE_HORARIOS_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarParametrizacaoDeHorariosNoAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntervaloDiasDuplicataNotaEntrada", Column = "GRP_INTERVALO_DIAS_DUPLICATA_NOTA_ENTRADA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string IntervaloDiasDuplicataNotaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaPadraoDuplicataNotaEntrada", Column = "GRP_DIA_PADRAO_DUPLICATA_NOTA_ENTRADA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaPadraoDuplicataNotaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParcelasDuplicataNotaEntrada", Column = "GRP_PARCELAS_DUPLICATA_NOTA_ENTRADA", TypeType = typeof(int), NotNull = false)]
        public virtual int ParcelasDuplicataNotaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IgnorarDuplicataRecebidaXMLNotaEntrada", Column = "GRP_IGNORAR_DUPLICATA_RECEBIDA_XML_NOTA_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IgnorarDuplicataRecebidaXMLNotaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirMultiplosVencimentos", Column = "GRP_PERMITIR_MULTIPLOS_VENCIMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirMultiplosVencimentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_NAO_PERMITIR_VINCULAR_CTE_COMPLEMENTAR_EM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirVincularCTeComplementarEmCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.BoletoConfiguracao BoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_ENVIAR_BOLETO_POR_EMAIL_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarBoletoPorEmailAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_ENVIAR_DOCUMENTACAO_FATURAMENTO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDocumentacaoFaturamentoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_COBRANCA_DIARIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_COBRANCA_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_COBRANCA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_COBRANCA_DIARIA_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string CobrancaDiariaObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_COBRANCA_DESCARGA_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string CobrancaDescargaObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_COBRANCA_CARREGAMENTO_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string CobrancaCarregamentoObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoFinanceira", Column = "GRP_SITUACAO_FINANCEIRA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFinanceira), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFinanceira? SituacaoFinanceira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracaoSituacaoFinanceira", Column = "GRP_DATA_ALTERACAO_SITUACAO_FINANCEIRA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracaoSituacaoFinanceira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_CLASSIFICACAO_EMPRESA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ClassificacaoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Vencimentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_FORNECEDOR_VENCIMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoasFornecedorVencimento", Column = "GFV_CODIGO")]
        public virtual IList<GrupoPessoasFornecedorVencimento> Vencimentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTituloFornecedor", Column = "GRP_FORMA_TITULO_FORNECEDOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo? FormaTituloFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "NCMsPallet", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOAS_NCM_PALLET")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoasNCMPallet", Column = "GNP_CODIGO")]
        public virtual IList<GrupoPessoasNCMPallet> NCMsPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_CONTROLA_PALLETS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlaPallets { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_DISPONIBILIZAR_DOCUMENTOS_PARA_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarDocumentosParaNFsManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_ENVIAR_AUTMATICAMENTE_DOCUMENTACAO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarAutomaticamenteDocumentacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_NAO_ADICIONAR_NOTA_NCM_PALLET_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAdicionarNotaNCMPalletCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_ZERAR_PESO_NOTA_NCM_PALLET_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ZerarPesoNotaNCMPalletCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_HABILITAR_PERIODO_VENCIMENTO_ESPECIFICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarPeriodoVencimentoEspecifico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoCarregamentoTicks", Column = "GRP_TEMPO_CARREGAMENTO_TICKS", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoCarregamentoTicks { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDescarregamentoTicks", Column = "GRP_TEMPO_DESCARREGAMENTO_TICKS", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoDescarregamentoTicks { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_GERAR_IMPRESSAO_ORDEM_COLETA_EXCLUSIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarImpressaoOrdemColetaExclusiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImpressaoOrdemColetaExclusiva", Column = "GRP_TIPO_IMPRESSAO_ORDEM_COLETA_EXCLUSIVA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoOrdemColetaExclusiva), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoOrdemColetaExclusiva TipoImpressaoOrdemColetaExclusiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_NAO_ENVAR_PARA_DOCSYS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarParaDocsys { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigatorioInformarMDFeEmitidoPeloEmbarcador", Column = "GRP_OBRIGATORIO_INFORMAR_MDFE_EMITIDO_PELO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarMDFeEmitidoPeloEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailDespachante", Column = "GRP_EMAIL_DESPACHANTE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string EmailDespachante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "GRP_DESPACHANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Despachante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdicionarDespachanteComoConsignatario", Column = "GRP_ADICIONAR_DESPACHANTE_COMO_CONSIGNATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AdicionarDespachanteComoConsignatario { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposComprovante", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_GRUPO_PESSOA_TIPO_COMPROVANTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoComprovante", Column = "CTC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante> TiposComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirComprovantesLiberacaoPagamentoContratoFrete", Column = "GRP_EXIGIR_COMPROVANTE_LIBERACAO_PAGAMENTO_CONTRATO_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirComprovantesLiberacaoPagamentoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_NAO_VALIDAR_POSSUIR_ACORDO_FATURAMENTO_AVANCO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarPossuiAcordoFaturamentoAvancoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_AVISO_VENCIMETO_HABILITAR_CONFIGURACAO_PERSONALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisoVencimetoHabilitarConfiguracaoPersonalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AvisoVencimetoQunatidadeDias", Column = "GRP_AVISO_VENCIMETO_QUNATIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int AvisoVencimetoQunatidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_AVISO_VENCIMETO_ENVIAR_DIARIAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisoVencimetoEnviarDiariamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_COBRANCA_HABILITAR_CONFIGURACAO_PERSONALIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaHabilitarConfiguracaoPersonalizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CobrancaQunatidadeDias", Column = "GRP_COBRANCA_QUNATIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int CobrancaQunatidadeDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_AVISO_VENCIMETO_NAO_ENVIAR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvisoVencimetoNaoEnviarEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_COBRANCA_NAO_ENVIAR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CobrancaNaoEnviarEmail { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ContasBancarias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_PESSOA_CONTAS_BANCARIAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContaBancaria", Column = "COB_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria> ContasBancarias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_NAO_ALTERAR_DOCUMENTO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoAlterarDocumentoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "GRP_PRIORIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PrioridadeGrupoPessoas), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PrioridadeGrupoPessoas Prioridade { get; set; }

        #region Mercado Livre

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_TIPO_INTEGRACAO_MERCADO_LIVRE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre? TipoIntegracaoMercadoLivre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_INTEGRACAO_MERCADO_LIVRE_CONSULTA_ROTAFACILITY_AUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_INTEGRACAO_MERCADO_LIVRE_AVANCAR_ETAPA_NFE_AUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRP_TIPO_ACRESCIMO_DECRESCIMO_DATA_PREVISAO_SAIDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida TipoTempoAcrescimoDecrescimoDataPrevisaoSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks", Column = "GRP_ACRESCENTAR_DATA_PREVISAO_SAIDA_TICKS", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks { get; set; }

        public virtual TimeSpan TempoAcrescimoDecrescimoDataPrevisaoSaida
        {
            get { return TimeSpan.FromTicks(TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks); }
            set { TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks = value.Ticks; }
        }

        #endregion

        #region Propriedades Virtuais

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return Localization.Resources.Gerais.Geral.Ativo;
                else
                    return Localization.Resources.Gerais.Geral.Inativo;
            }
        }

        public virtual bool Equals(GrupoPessoas other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
        public virtual TimeSpan TempoCarregamento
        {
            get { return TimeSpan.FromTicks(TempoCarregamentoTicks); }
            set { TempoCarregamentoTicks = value.Ticks; }
        }

        public virtual TimeSpan TempoDescarregamento
        {
            get { return TimeSpan.FromTicks(TempoDescarregamentoTicks); }
            set { TempoDescarregamentoTicks = value.Ticks; }
        }

        #endregion
    }
}
