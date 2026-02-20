using System;

namespace Dominio.Entidades.Embarcador.ConfiguracaoContabil
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_CONTABIL", EntityName = "DocumentoContabil", Name = "Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil", NameType = typeof(DocumentoContabil))]
    public  class DocumentoContabil : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DCB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoCTeParaSubContratacao", Column = "PSC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao PedidoCTeParaSubContratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Stage", Column = "STA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Stage Stage { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImpostoValorAgregado", Column = "IVA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Contabeis.ImpostoValorAgregado ImpostoValorAgregado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "CON_MODELODOC", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "DCB_NUMERO_DOCUMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieDocumento", Column = "DCB_SERIE_DOCUMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int SerieDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPV_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DCB_DATA_EMISSAO_CTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DCB_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DCB_DATA_REGISTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataRegistro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete FechamentoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBruto", Column = "DCB_PESO_BRUTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoBruto { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagamento", Column = "PAG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.Pagamento Pagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pagamento", Column = "PAG_CODIGO_LIBERADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.Pagamento PagamentoLiberado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoFaturamento", Column = "DFA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento DocumentoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CancelamentoPagamento", Column = "CPG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento CancelamentoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Provisao", Column = "PRV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.Provisao Provisao { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CancelamentoProvisao", Column = "CPV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao CancelamentoProvisao { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoProvisao", Column = "DPV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao DocumentoProvisao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "DCB_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoContabil), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoContabil Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaContabil", Column = "CCT_TIPO_CONTA_CONTABIL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContabilizacao", Column = "CCT_TIPO_CONTABILIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorContabilizacao", Column = "DCB_VALOR_CONTABILIZACAO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCofins", Column = "DCB_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPis", Column = "DCB_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIss", Column = "DCB_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIss { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaDocumentoParaEmissaoNFSManual", Column = "NEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual CargaDocumentoParaEmissaoNFSManual { get; set; }

        /// <summary>
        /// Toda movimentação contabil deve possui uma provisão de referencia, usada apenas para extração de dados em nível de relatórios.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoProvisao", Column = "DPV_CODIGO_REFERENCIA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao DocumentoProvisaoReferencia { get; set; }

        #region Imposto IBS/CBS

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "DCB_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "DCB_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "DCB_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        #endregion Imposto CBS/IBS

        #region Propriedades com Regras

        public virtual Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil Clonar()
        {
            return (Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil)this.MemberwiseClone();
        }

        public virtual string Descricao
        {
            get
            {
                return this.NumeroDocumento.ToString();
            }
        }

        public virtual bool Equals(DocumentoContabil other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion Propriedades com Regras
    }
}
