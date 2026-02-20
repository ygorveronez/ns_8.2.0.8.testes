using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_CTE_PARA_SUB_CONTRATACAO", EntityName = "PedidoCTeParaSubContratacao", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao", NameType = typeof(PedidoCTeParaSubContratacao))]
    public class PedidoCTeParaSubContratacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PSC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CTe.CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "PSC_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        /// <summary>
        /// Armazena o valor dos compoentes exceto impostos e frete liquido.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalComponentes", Column = "PSC_TOTAL_COMPONENTES", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalComponentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "PSC_VALOR_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCofins", Column = "PSC_VALOR_COFINS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPis", Column = "PSC_VALOR_PIS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSIncluso", Column = "PSC_VALOR_ICMS_INCLUSO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorICMSIncluso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquota", Column = "PSC_PERCENTUAL_ALICOTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualAliquota { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCofins", Column = "PSC_ALICOTA_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPis", Column = "PSC_ALICOTA_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPis { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaInternaDifal", Column = "PSC_PERCENTUAL_ALICOTA_INTERNA_DIFAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualAliquotaInternaDifal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSC_CST", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.CFOP CFOP { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "PSC_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoBC", Column = "PSC_PERCENTUAL_REDUCAO_BC", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirICMSBaseCalculo", Column = "PSC_INCLUIR_ICMS_BASE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirICMSBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualIncluirBaseCalculo", Column = "PSC_PERCENTUAL_INCLUIR_BASE_CALCULO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualIncluirBaseCalculo { get; set; }



        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "PSC_VALOR_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoISS", Column = "PSC_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAliquotaISS", Column = "PSC_PERCENTUAL_ALICOTA_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualRetencaoISS", Column = "PSC_PERCENTUAL_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirISSBaseCalculo", Column = "PSC_INCLUIR_ISS_BASE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirISSBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRetencaoISS", Column = "PSC_VALOR_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReterIR", Column = "PSC_RETER_IR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReterIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIR", Column = "PSC_ALIQUOTA_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIR", Column = "PSC_BASE_CALCULO_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIR", Column = "PSC_VALOR_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIR { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiCTe", Column = "PSC_POSSUI_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiNFS", Column = "PSC_POSSUI_NFS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNFS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiNFSManual", Column = "PSC_POSSUI_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CteSubContratacaoFilialEmissora", Column = "PCS_CTE_SUB_CONTRATACAO_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CteSubContratacaoFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteTabelaFrete", Column = "PSC_VALOR_FRETE_TABELA_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorFreteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualPagamentoAgregado", Column = "PSC_PERCENTUAL_PAGAMENTO_AGREGADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal PercentualPagamentoAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoRegraICMSCTe", Column = "PSC_OBSERVACAO_REGRA_ICMS_CTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoRegraICMSCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSC_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSC_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal? ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSC_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSC_TOTAL_MOEDA_COMPONENTES", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMoedaComponentes { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_ESCRITURACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_ICMS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoICMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_PIS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoPIS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_COFINS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSC_VALOR_MAXIMO_CENTRO_CONTABILIZACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMaximoCentroContabilizacao { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItemServico", Column = "PSC_ITEM_SERVICO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string ItemServico { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocsParaEmissaoNFSManual", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PSC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaDocumentoParaEmissaoNFSManual", Column = "NEM_CODIGO")]
        public virtual ICollection<Cargas.CargaDocumentoParaEmissaoNFSManual> DocsParaEmissaoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraICMS", Column = "RIC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.ICMS.RegraICMS RegraICMS { get; set; }

        #region Imposto IBS/CBS

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NBS", Column = "PSC_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIndicadorOperacao", Column = "PSC_CODIGO_INDICADOR_OPERACAO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string CodigoIndicadorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "PSC_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "PSC_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "PSC_BASE_CALCULO_IBSCBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "PSC_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "PSC_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "PSC_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "PSC_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "PSC_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "PSC_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "PSC_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "PSC_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "PSC_VALOR_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        #endregion Imposto CBS/IBS

        #region Propriedades com Regras

        public virtual void SetarRegraICMS(int codigoRegraICMS)
        {
            if (codigoRegraICMS > 0)
                RegraICMS = new Dominio.Entidades.Embarcador.ICMS.RegraICMS() { Codigo = codigoRegraICMS };
            else
                RegraICMS = null;
        }

        public virtual void SetarRegraOutraAliquota(int codigoOutraAliquota)
        {
            OutrasAliquotas = codigoOutraAliquota > 0 ? new Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas() { Codigo = codigoOutraAliquota } : null;
        }

        public virtual bool Equals(PedidoCTeParaSubContratacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao Clonar()
        {
            return (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao)this.MemberwiseClone();
        }

        #endregion
    }
}
