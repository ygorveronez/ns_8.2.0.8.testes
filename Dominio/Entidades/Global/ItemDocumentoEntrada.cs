namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_ENTRADA_ITEM", EntityName = "ItemDocumentoEntrada", Name = "Dominio.Entidades.ItemDocumentoEntrada", NameType = typeof(ItemDocumentoEntrada))]
    public class ItemDocumentoEntrada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DEI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntrada", Column = "DOE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntrada DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeMedidaGeral", Column = "UMG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UnidadeMedidaGeral UnidadeMedida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CFOP", Column = "CFO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CFOP CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencial", Column = "DEI_SEQUENCIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int Sequencial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "DEI_QUANTIDADE", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "DEI_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 5, Precision = 18, NotNull = true)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "DEI_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "DEI_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "DEI_CST", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMS", Column = "DEI_ALIQUOTA_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "DEI_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "DEI_VALOR_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIPI", Column = "DEI_CST_IPI", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIPI", Column = "DEI_ALIQUOTA_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIPI", Column = "DEI_BASE_CALCULO_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIPI", Column = "DEI_VALOR_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMSST", Column = "DEI_BASE_CALCULO_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSST", Column = "DEI_VALOR_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrasDespesas", Column = "DEI_OUTRAS_DESPESAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal OutrasDespesas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "DEI_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPIS", Column = "DEI_CST_PIS", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPIS", Column = "DEI_VALOR_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTCOFINS", Column = "DEI_CST_COFINS", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CSTCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCOFINS", Column = "DEI_VALOR_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorCOFINS { get; set; }
    }
}
