using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA", DynamicUpdate = true, EntityName = "CargaOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia", NameType = typeof(CargaOcorrencia))]
    public class CargaOcorrencia : EntidadeBase, IEquatable<CargaOcorrencia>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOcorrencia", Column = "COC_NUMERO_CONTRATO", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_NUMERO_OCORRENCIA_CLIENTE", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroOcorrenciaCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_OUTRO_EMITENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Emitente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_CODIGO_TIPO_OCORRENCIA_PARA_INTEGRACAO", TypeType = typeof(string), NotNull = false, Length = 50)]
        public virtual string CodigoTipoOcorrenciaParaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DTNatura", Column = "IDT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.DTNatura DTNatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador ContratoFrete { get; set; }

        [Obsolete("Migrado para uma lista, N:N Chamados.ChamadoOcorrencia")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_DATA_PRAZO_APROVACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrazoAprovacao { get; set; }

        /// <summary>
        /// Indica se irá emitir complemento para filial emissora
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "EmiteComplementoFilialEmissora", Column = "COC_EMITE_COMPLEMENTO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmiteComplementoFilialEmissora { get; set; }

        /// <summary>
        /// Flag indica que todo o controle da emissão dos CT-es está ocorrendo na emissão do Ct-e de subcontratação da filial emissora.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "LiberadaParaEmissaoCTeSubContratacaoFilialEmissora", Column = "COC_LIBERARA_PARA_EMISSAO_CTE_SUBCONTRATACAO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberadaParaEmissaoCTeSubContratacaoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_REENVIOU_INTEGRACAO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReenviouIntegracaoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_REENVIOU_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReenviouIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_POSSUI_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_NFS_MANUAL_PENDENTE_GERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NFSManualPendenteGeracao { get; set; }

        /// <summary>
        /// Flag indica que todo o controle da integração está ocorrendo com a filial emissora.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_INTEGRANDO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrandoFilialEmissora { get; set; }

        /// <summary>
        /// Indica se a OCORRENCIA já foi integrada com o transportador/MultiTMS.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_INTEGROU_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrouTransportador { get; set; }

        [Obsolete("USAR OrigemOcorrenciaPorPeriodo. ESSA FLAG SERA REMOCIDA")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorrenciaPorPeriodo", Column = "COC_OCORRENCIA_POR_PERIODO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaPorPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemOcorrencia", Column = "COC_ORIGEM_OCORRENCIA", TypeType = typeof(OrigemOcorrencia), NotNull = false)]
        public virtual OrigemOcorrencia OrigemOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoInicio", Column = "COC_PERIODO_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PeriodoInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PeriodoFim", Column = "COC_PERIODO_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? PeriodoFim { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Cargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_OCORRENCIA_CARGAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Carga", Column = "CAR_CODIGO")]
        public virtual ICollection<Cargas.Carga> Cargas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoCredito", Column = "SCR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito SolicitacaoCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "COC_OBSERVACAO", Type = "StringClob", NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCancelamento", Column = "COC_OBSERVACAO_CANCELAMENTO", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string ObservacaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicaoCancelamento", Column = "COC_MOTIVO_REJEICAO_CANCELAMENTO", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string MotivoRejeicaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalizacaoEmissaoOcorrencia", Column = "COC_DATA_FINALIZACAO_EMISSAO_OCORRENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacaoEmissaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataOcorrencia", Column = "COC_DATA_OCORRENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_DATA_BASE_APROVACAO_AUTOMATICA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseAprovacaoAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_DATA_EVENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoOcorrencia", Column = "COC_SITUACAO_OCORRENCIA", TypeType = typeof(SituacaoOcorrencia), NotNull = true)]
        public virtual SituacaoOcorrencia SituacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoOcorrenciaNoCancelamento", Column = "COC_SITUACAO_OCORRENCIA_NO_CANCELAMENTO", TypeType = typeof(SituacaoOcorrencia), NotNull = false)]
        public virtual SituacaoOcorrencia SituacaoOcorrenciaNoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_RESPONSAVEL_AUTORIZACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario ResponsavelAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAlteracao", Column = "COC_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataAlteracao { get; set; }

        /// <summary>
        /// É o valor do tipo de ocorrencia onde o componente soma o valor no valor liquido do frete.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOcorrenciaLiquida", Column = "COC_VALOR_OCORRENCIA_LIQUIDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorOcorrenciaLiquida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOcorrencia", Column = "COC_VALOR_OCORRENCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOcorrenciaOriginal", Column = "COC_VALOR_OCORRENCIA_ORIGINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorOcorrenciaOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTe", Column = "COC_OBSERVACAO_CTE", TypeType = typeof(string), NotNull = true, Length = 2000)]
        public virtual string ObservacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO_EMISSAO_MUNICIPAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscalEmissaoMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouTodosDocumentos", Column = "COC_GEROU_TODOS_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouTodosDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSFrete", Column = "COC_INCLUIR_ICMS_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IncluirICMSFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsavel", Column = "COC_RESPONSAVEL", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador? Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pagamento", Column = "COC_PAGAMENTO", TypeType = typeof(AutorizacaoOcorrenciaPagamento), NotNull = false)]
        public virtual AutorizacaoOcorrenciaPagamento? Pagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ContaContabil", Column = "COC_CONTA_CONTABIL", TypeType = typeof(string), NotNull = false, Length = 6)]
        public virtual string ContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOP", Column = "COC_CFOP", TypeType = typeof(string), NotNull = false, Length = 6)]
        public virtual string CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidNomeArquivo", Column = "COC_GUID_NOME_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidNomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "COC_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAcresciomoValor", Column = "COC_PERCENTUAL_ACRESCIMO_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal PercentualAcresciomoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaOcorrenciaVinculada", Column = "COC_OCORRENCIA_VINCULADA", TypeType = typeof(int), NotNull = false)]
        public virtual int CargaOcorrenciaVinculada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AgImportacaoCTe", Column = "COC_AG_IMPORTACAO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgImportacaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerandoIntegracoes", Column = "COC_GERANDO_INTEGRACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerandoIntegracoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_GERADA_POR_GATILHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeradaPorGatilho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_BASE_CALCULO_ICMS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_ALIQUOTA_ICMS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_VALOR_ICMS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTICMS", Column = "COC_CST_ICMS", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "COC_LATITUDE", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "COC_LONGITUDE", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrioridadeAprovacaoAtualEtapaAprovacao", Column = "COC_PRIORIDADE_APROVACAO_ATUAL_ETAPA_APROVACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int PrioridadeAprovacaoAtualEtapaAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrioridadeAprovacaoAtualEtapaEmissao", Column = "COC_PRIORIDADE_APROVACAO_ATUAL_ETAPA_EMISSAO", TypeType = typeof(int), NotNull = true)]
        public virtual int PrioridadeAprovacaoAtualEtapaEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OcorrenciaIntegracao", Column = "OIN_CODIGO")]
        public virtual IList<OcorrenciaIntegracao> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "VeiculosContrato", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_OCORRENCIA_CONTRATO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OcorrenciaContratoVeiculo", Column = "OCV_CODIGO")]
        public virtual IList<OcorrenciaContratoVeiculo> VeiculosContrato { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CargaOcorrenciaAutorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_OCORRENCIA_AUTORIZACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaOcorrenciaAutorizacao", Column = "COA_CODIGO")]
        public virtual ICollection<CargaOcorrenciaAutorizacao> CargaOcorrenciaAutorizacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_INATIVA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Inativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_COMPLEMENTO_VALOR_FRETE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComplementoValorFreteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_ERRO_INTEGRACAO_COM_GPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ErroIntegracaoComGPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_INTEGRADO_COM_GPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoComGPA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_MENSAGEM_PENDENCIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string MensagemPendencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_NOME_RECEBEDOR", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NomeRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_TIPO_DOCUMENTO_RECEBEDOR", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string TipoDocumentoRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_NUMERO_DOCUMENTO_RECEBEDOR", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroDocumentoRecebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_NOTIFICAR_DEBITOS_ATIVOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarDebitosAtivos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_CTE_EMITIDO_NO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? CTeEmitidoNoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "COC_USUARIO_RESPONSAVEL_APROVACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioResponsavelAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quilometragem", Column = "COC_QUILOMETRAGEM", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Quilometragem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoTipoDeOcorrenciaDeCTe", Column = "GTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe GrupoOcorrencia { get; set; }

        /**
         * CAMPO MODIFICADO PARA SER GENÉRICO, NÃO UTILIZA-LO 
         * DROPAR EM 03/10/2018
         */
        [Obsolete]
        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_QUANTIDADE_AJUDANTE", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeAjudante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_QUANTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_MOEDA", TypeType = typeof(MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoAprovador", Column = "COC_OBSERVACAO_APROVADOR", TypeType = typeof(string), NotNull = false, Length = 5000)]
        public virtual string ObservacaoAprovador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAprovacao", Column = "COC_CODIGO_APROVACAO", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string CodigoAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_QUANTIDADE_PARCELAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeParcelas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualJurosParcela", Column = "COC_PERCENTUAL_JUROS_PARCELA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualJurosParcela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_NAO_GERAR_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_MOTIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ULTIMO_USUARIO_DELEGADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UltimoUsuarioDelegado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ULTIMO_USUARIO_DELEGOU", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UltimoUsuarioDelegou { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Lote", Column = "LAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.Lote LoteAvaria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Infracao", Column = "INF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.Infracao Infracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicialEstadia", Column = "COC_DATA_INICIAL_ESTADIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialEstadia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalEstadia", Column = "COC_DATA_FINAL_ESTADIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalEstadia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_OCORRENCIA_DE_ESTADIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaDeEstadia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorasEstadia", Column = "COC_HORAS_ESTADIA", TypeType = typeof(double), NotNull = false)]
        public virtual double? HorasEstadia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorasFreetime", Column = "COC_HORAS_FREETIME", TypeType = typeof(double), NotNull = false)]
        public virtual double? HorasFreetime { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_MODELO_COBRANCA_ESTADIA", TypeType = typeof(ModeloCobrancaEstadiaTracking), NotNull = false)]
        public virtual ModeloCobrancaEstadiaTracking? ModeloCobrancaEstadiaTracking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_TIPO_CARGA_ENTREGA", TypeType = typeof(TipoCargaEntrega), NotNull = false)]
        public virtual TipoCargaEntrega? TipoCargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_VALOR_ORIGINAL_OCORRENCIA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorOriginalOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "COC_PROTOCOLO", TypeType = typeof(int), NotNull = false)]
        public virtual int Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeAjudantes", Column = "COC_QUANTIDADE_AJUDANTES", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeAjudantes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorrenciaRecebidaDeIntegracao", Column = "COC_OCORRENCIA_RECEBIDA_DE_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaRecebidaDeIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OcorrenciaReprovada", Column = "COC_OCORRENCIA_REPROVADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcorrenciaReprovada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoERP", Column = "COC_INTEGRADO_ERP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegradoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarSelecaoPorNotasFiscaisCTe", Column = "COC_UTILIZAR_SELECAO_POR_NOTAS_FISCAIS_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarSelecaoPorNotasFiscaisCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucao", Column = "GDV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao GestaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TiposCausadoresOcorrencia", Column = "TPO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia TiposCausadoresOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOcorrenciaCausas", Column = "TTOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas CausasTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COC_PERIODO_PAGAMENTO", TypeType = typeof(PeriodoPagamento), NotNull = false)]
        public virtual PeriodoPagamento? PeriodoPagamento { get; set; }

        #region Propriedades Virtuais

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumerosCTes", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(cte1.CON_NUM AS NVARCHAR(20))
                                                                                    FROM T_CARGA_CTE_COMPLEMENTO_INFO cteComplementoInfo
                                                                                    inner join T_CARGA_CTE cargaCTe ON cteComplementoInfo.ccc_codigo = cargaCTe.ccc_codigo
	                                                                                inner join T_CTE cte1 ON cte1.CON_CODIGO = cargaCTe.CON_CODIGO
                                                                                    WHERE cteComplementoInfo.COC_CODIGO = COC_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumerosCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumerosCTesOriginarios", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CONVERT(nvarchar(20), cte1.CON_NUM)
		                                                                                        FROM T_CARGA_CTE_COMPLEMENTO_INFO cteComplementoInfo
		                                                                                        inner join T_CARGA_CTE cargaCTe ON cteComplementoInfo.CCT_CODIGO_COMPLEMENTADO = cargaCTe.CCT_CODIGO
		                                                                                        inner join T_CTE cte1 ON cte1.CON_CODIGO = cargaCTe.CON_CODIGO
		                                                                                        WHERE cteComplementoInfo.COC_CODIGO = COC_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumerosCTesOriginarios { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "TomadorCTe", Formula = @"(SELECT top(1) tomadorCTe.PCT_NOME
                                                                                                   FROM T_CARGA_CTE_COMPLEMENTO_INFO cteComplementoInfo
                                                                                                   inner join T_CARGA_CTE cargaCTe ON cteComplementoInfo.ccc_codigo = cargaCTe.ccc_codigo
                                                                                                   inner join T_CTE cte1 ON cte1.CON_CODIGO = cargaCTe.CON_CODIGO
                                                                                                   inner join T_CTE_PARTICIPANTE tomadorCTe on tomadorCTe.PCT_CODIGO = cte1.CON_TOMADOR_PAGADOR_CTE
                                                                                                   WHERE cteComplementoInfo.COC_CODIGO = COC_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL)", TypeType = typeof(string), Lazy = true)]
        public virtual string TomadorCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotasFiscaisCTesOriginarios", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CONVERT(nvarchar(20), docs.NFC_NUMERO)
                                                                                                     FROM T_CARGA_CTE_COMPLEMENTO_INFO cteComplementoInfo
                                                                                                     inner join T_CARGA_CTE cargaCTe ON cteComplementoInfo.CCT_CODIGO_COMPLEMENTADO = cargaCTe.CCT_CODIGO
                                                                                                     inner join T_CTE cte1 ON cte1.CON_CODIGO = cargaCTe.CON_CODIGO
                                                                                                     inner join T_CTE_DOCS docs on docs.CON_CODIGO = cte1.CON_CODIGO
                                                                                                     WHERE cteComplementoInfo.COC_CODIGO = COC_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NotasFiscaisCTesOriginarios { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Operadores", Formula = @"SUBSTRING((SELECT ', ' + funcionario.FUN_NOME 
                                                                                FROM T_CARGA_OCORRENCIA_AUTORIZACAO autoriza
                                                                                INNER JOIN T_FUNCIONARIO funcionario ON autoriza.FUN_CODIGO = funcionario.FUN_CODIGO
                                                                                WHERE autoriza.COA_SITUACAO <> 0 AND autoriza.COC_CODIGO = COC_CODIGO and funcionario.FUN_CODIGO IS NOT NULL GROUP BY funcionario.FUN_NOME FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string Operadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicao", Formula = @"SUBSTRING((SELECT ', ' + autoriza.COA_MOTIVO 
                                                                                FROM T_CARGA_OCORRENCIA_AUTORIZACAO autoriza
                                                                                WHERE autoriza.COA_SITUACAO = 9 AND autoriza.COC_CODIGO = COC_CODIGO GROUP BY autoriza.COA_MOTIVO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string MotivoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaRejeicao", Formula = @"SUBSTRING((SELECT ', ' + justificativa.MRO_DESCRICAO 
                                                                                FROM T_CARGA_OCORRENCIA_AUTORIZACAO autoriza
                                                                                INNER JOIN T_MOTIVO_REJEICAO_OCORRENCIA justificativa ON justificativa.MRO_CODIGO = autoriza.MRO_CODIGO
                                                                                WHERE autoriza.COA_SITUACAO = 9 AND autoriza.COC_CODIGO = COC_CODIGO GROUP BY justificativa.MRO_DESCRICAO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string JustificativaRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAprovacao", Column = "COC_DATA_APROVACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoCTes", Formula = @"SUBSTRING((SELECT DISTINCT '; ' + cte1.CON_OBSGERAIS
                                                                                    FROM T_CARGA_CTE_COMPLEMENTO_INFO cteComplementoInfo
                                                                                    inner join T_CARGA_CTE cargaCTe ON cteComplementoInfo.ccc_codigo = cargaCTe.ccc_codigo
	                                                                                inner join T_CTE cte1 ON cte1.CON_CODIGO = cargaCTe.CON_CODIGO
                                                                                    WHERE cteComplementoInfo.COC_CODIGO = COC_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string ObservacaoCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroDataInicial", Formula = @"(SELECT top 1 ocorrenciaParamentros.COC_DATA_INICIO
                                                                                            FROM T_CARGA_OCORRENCIA_PARAMETROS ocorrenciaParamentros
                                                                                            INNER JOIN T_CARGA_OCORRENCIA cargaOcorrencia ON ocorrenciaParamentros.COC_CODIGO = cargaOcorrencia.COC_CODIGO
                                                                                            INNER JOIN T_PARAMETROS_OCORRENCIA parametros ON parametros.POC_CODIGO = ocorrenciaParamentros.POC_CODIGO
                                                                                            WHERE parametros.POC_TIPO_PARAMETRO = 1 AND cargaOcorrencia.COC_CODIGO = COC_CODIGO order by ocorrenciaParamentros.POC_CODIGO)", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime ParametroDataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroDataFim", Formula = @"(SELECT top 1 ocorrenciaParamentros.COC_DATA_FIM
                                                                                            FROM T_CARGA_OCORRENCIA_PARAMETROS ocorrenciaParamentros
                                                                                            INNER JOIN T_CARGA_OCORRENCIA cargaOcorrencia ON ocorrenciaParamentros.COC_CODIGO = cargaOcorrencia.COC_CODIGO
                                                                                            INNER JOIN T_PARAMETROS_OCORRENCIA parametros ON parametros.POC_CODIGO = ocorrenciaParamentros.POC_CODIGO
                                                                                            WHERE parametros.POC_TIPO_PARAMETRO = 1 AND cargaOcorrencia.COC_CODIGO = COC_CODIGO order by ocorrenciaParamentros.POC_CODIGO)", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime ParametroDataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDeHoras", Formula = @"(SELECT sum(cast(case when ocorrenciaParamentros.COC_TOTAL_HORAS > 0 then ocorrenciaParamentros.COC_TOTAL_HORAS else cast(DATEDIFF(minute,ocorrenciaParamentros.COC_DATA_INICIO, ocorrenciaParamentros.COC_DATA_FIM) as numeric(18,2)) / 60  end as decimal(18,2)))
                                                                                           FROM T_CARGA_OCORRENCIA_PARAMETROS ocorrenciaParamentros
                                                                                           INNER JOIN T_CARGA_OCORRENCIA cargaOcorrencia ON ocorrenciaParamentros.COC_CODIGO = cargaOcorrencia.COC_CODIGO
                                                                                           INNER JOIN T_PARAMETROS_OCORRENCIA parametros ON parametros.POC_CODIGO = ocorrenciaParamentros.POC_CODIGO
                                                                                           WHERE parametros.POC_TIPO_PARAMETRO = 1 AND cargaOcorrencia.COC_CODIGO = COC_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal QuantidadeDeHoras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroData1", Formula = @"(SELECT top 1 ocorrenciaParamentros.COC_DATA
                                                                                         FROM T_CARGA_OCORRENCIA_PARAMETROS ocorrenciaParamentros
                                                                                         INNER JOIN T_CARGA_OCORRENCIA cargaOcorrencia ON ocorrenciaParamentros.COC_CODIGO = cargaOcorrencia.COC_CODIGO
                                                                                         INNER JOIN T_PARAMETROS_OCORRENCIA parametros ON parametros.POC_CODIGO = ocorrenciaParamentros.POC_CODIGO
                                                                                         WHERE parametros.POC_TIPO_PARAMETRO = 5 AND cargaOcorrencia.COC_CODIGO = COC_CODIGO order by ocorrenciaParamentros.POC_CODIGO)", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime ParametroData1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroData2", Formula = @"(SELECT top 1 ocorrenciaParamentros.COC_DATA
                                                                                         FROM T_CARGA_OCORRENCIA_PARAMETROS ocorrenciaParamentros
                                                                                         INNER JOIN T_CARGA_OCORRENCIA cargaOcorrencia ON ocorrenciaParamentros.COC_CODIGO = cargaOcorrencia.COC_CODIGO
                                                                                         INNER JOIN T_PARAMETROS_OCORRENCIA parametros ON parametros.POC_CODIGO = ocorrenciaParamentros.POC_CODIGO
                                                                                         WHERE parametros.POC_TIPO_PARAMETRO = 5 AND cargaOcorrencia.COC_CODIGO = COC_CODIGO order by ocorrenciaParamentros.POC_CODIGO DESC)", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime ParametroData2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroTexto", Formula = @"(SELECT top 1 ocorrenciaParamentros.COC_TEXTO
                                                                                         FROM T_CARGA_OCORRENCIA_PARAMETROS ocorrenciaParamentros
                                                                                         INNER JOIN T_CARGA_OCORRENCIA cargaOcorrencia ON ocorrenciaParamentros.COC_CODIGO = cargaOcorrencia.COC_CODIGO
                                                                                         INNER JOIN T_PARAMETROS_OCORRENCIA parametros ON parametros.POC_CODIGO = ocorrenciaParamentros.POC_CODIGO
                                                                                         WHERE parametros.POC_TIPO_PARAMETRO = 4 AND cargaOcorrencia.COC_CODIGO = COC_CODIGO order by ocorrenciaParamentros.POC_CODIGO)", TypeType = typeof(string), Lazy = true)]
        public virtual string ParametroTexto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JanelaDescarga", Formula = @"(SELECT top 1 descarga.cld_hora_inicio_descarga + ' até ' + descarga.cld_hora_limete_descarga
                                                                                         FROM T_CARGA_PEDIDO cargaPedido
                                                                                         INNER JOIN T_PEDIDO pedido on cargaPedido.ped_codigo = pedido.ped_codigo
                                                                                         INNER JOIN T_CARGA_OCORRENCIA cargaOcorrencia on cargaPedido.car_codigo = cargaOcorrencia.car_codigo
                                                                                         INNER JOIN T_CLIENTE_DESCARGA descarga on pedido.cli_codigo = descarga.cli_cgccpf and pedido.cli_codigo_remetente = descarga.cli_cgccpf_origem
                                                                                         where cargaOcorrencia.COC_CODIGO = COC_CODIGO)", TypeType = typeof(string), Lazy = true)]
        public virtual string JanelaDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DestinatariosCTes", Formula = @"SUBSTRING((SELECT DISTINCT ' / ' + participanteCTe.PCT_NOME
                                                                                    FROM T_CARGA_OCORRENCIA_DOCUMENTO cargaOcorrenciaDocumento
                                                                                    inner join T_CARGA_CTE cargaCTe2 on cargaOcorrenciaDocumento.CCT_CODIGO = cargaCTe2.CCT_CODIGO
                                                                                    inner join T_CTE cte2 on cargaCTe2.CON_CODIGO = cte2.CON_CODIGO
                                                                                    inner join T_CTE_PARTICIPANTE participanteCTe ON cte2.CON_DESTINATARIO_CTE = participanteCTe.PCT_CODIGO
                                                                                    WHERE cargaOcorrenciaDocumento.COC_CODIGO = COC_CODIGO and cargaOcorrenciaDocumento.CCT_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string DestinatariosCTes { get; set; }



        public virtual string Descricao
        {
            get { return this.NumeroOcorrencia.ToString(); }
        }

        public virtual string DescricaoDataOcorrencia
        {
            get
            {
                PeriodoAcordoContratoFreteTransportador periodo = Periodo;

                if (periodo == PeriodoAcordoContratoFreteTransportador.NaoPossui)
                    return DataOcorrencia.ToString("dd/MM/yyyy HH:mm");

                DateTimeFormatInfo formatacaoData = CultureInfo.CreateSpecificCulture("pt-BR").DateTimeFormat;
                string ano = PeriodoInicio.Value.ToString("yyyy");
                string nomeMes = formatacaoData.GetMonthName(PeriodoInicio.Value.Month).ToLower();

                nomeMes = $"{char.ToUpper(nomeMes[0])}{nomeMes.Substring(1)}";

                if (periodo == PeriodoAcordoContratoFreteTransportador.Semanal)
                {
                    if (PeriodoInicio.Value.Day == 8)
                        return $"2ª Sem. de {nomeMes} de {ano}";

                    if (PeriodoInicio.Value.Day == 15)
                        return $"3ª Sem. de {nomeMes} de {ano}";

                    if (PeriodoInicio.Value.Day == 22)
                        return $"4ª Sem. de {nomeMes} de {ano}";

                    return $"1ª Sem. de {nomeMes} de {ano}";
                }

                if (periodo == PeriodoAcordoContratoFreteTransportador.Decendial)
                {
                    if (PeriodoInicio.Value.Day == 11)
                        return $"2ª Dez. de {nomeMes} de {ano}";

                    if (PeriodoInicio.Value.Day == 21)
                        return $"3ª Dez. de {nomeMes} de {ano}";

                    return $"1ª Dez. de {nomeMes} de {ano}";
                }

                if (periodo == PeriodoAcordoContratoFreteTransportador.Quinzenal)
                {
                    if (PeriodoInicio.Value.Day == 16)
                        return $"2ª Quin. de {nomeMes} de {ano}";

                    return $"1ª Quin. de {nomeMes} de {ano}";
                }

                return $"{nomeMes} de {ano}";
            }
        }

        public virtual string DescricaoSituacao
        {
            get { return SituacaoOcorrencia.ObterDescricao(); }
        }

        public virtual EtapaAutorizacaoOcorrencia EtapaAutorizacaoOcorrencia
        {
            get
            {
                if (SituacaoOcorrencia == SituacaoOcorrencia.AgAprovacao)
                    return EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia;

                return EtapaAutorizacaoOcorrencia.EmissaoOcorrencia;
            }
        }

        public virtual bool OrigemOcorrenciaPorPeriodo
        {
            get { return this.OrigemOcorrencia == OrigemOcorrencia.PorContrato || this.OrigemOcorrencia == OrigemOcorrencia.PorPeriodo; }
        }

        public virtual PeriodoAcordoContratoFreteTransportador Periodo
        {
            get
            {
                if (!OrigemOcorrenciaPorPeriodo || !PeriodoInicio.HasValue || !PeriodoFim.HasValue)
                    return PeriodoAcordoContratoFreteTransportador.NaoPossui;

                int diaInicioPeriodo = PeriodoInicio.Value.Day;
                int diaFimPeriodo = PeriodoFim.Value.Day;

                if ((diaInicioPeriodo == 8) || (diaInicioPeriodo == 15) || (diaInicioPeriodo == 22) || (diaInicioPeriodo == 29))
                    return PeriodoAcordoContratoFreteTransportador.Semanal;

                if ((diaInicioPeriodo == 11) || (diaInicioPeriodo == 21))
                    return PeriodoAcordoContratoFreteTransportador.Decendial;

                if (diaInicioPeriodo == 16)
                    return PeriodoAcordoContratoFreteTransportador.Quinzenal;

                if (diaFimPeriodo == 7)
                    return PeriodoAcordoContratoFreteTransportador.Semanal;

                if (diaFimPeriodo == 10)
                    return PeriodoAcordoContratoFreteTransportador.Decendial;

                if (diaFimPeriodo == 15)
                    return PeriodoAcordoContratoFreteTransportador.Quinzenal;

                return PeriodoAcordoContratoFreteTransportador.Mensal;
            }
        }

        public virtual Empresa ObterEmitenteOcorrencia()
        {
            if (this.OrigemOcorrencia == OrigemOcorrencia.PorPeriodo || this.OrigemOcorrencia == OrigemOcorrencia.PorContrato)
                return this.Emitente;

            if (this.OrigemOcorrencia == OrigemOcorrencia.PorCarga)
                return this.Carga?.Empresa;

            return null;
        }

        public virtual bool Equals(CargaOcorrencia other)
        {
            return (this.Codigo == other.Codigo);
        }

        #endregion
    }
}
