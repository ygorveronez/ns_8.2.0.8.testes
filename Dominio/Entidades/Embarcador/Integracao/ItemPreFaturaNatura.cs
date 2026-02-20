namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_NATURA_PRE_FATURA_ITEM", EntityName = "ItemPreFaturaNatura", Name = "Dominio.Entidades.Embarcardor.Integracao.ItemPreFaturaNatura", NameType = typeof(ItemPreFaturaNatura))]
    public class ItemPreFaturaNatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IPI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreFaturaNatura", Column = "IPN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreFaturaNatura PreFatura { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DTNatura", Column = "IDT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DTNatura DocumentoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_ALIQUOTA_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_VALOR_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_ALIQUOTA_ICMS_ST", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_IVA_ICMS_ST", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal IVAICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_BASE_CALCULO_ICMS_ST", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_VALOR_ICMS_ST", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_BASE_CALCULO_PIS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_VALOR_PIS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_BASE_CALCULO_COFINS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_VALOR_COFINS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_VALOR_ISS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPI_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorDoDesconto { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.DocumentoTransporte?.Descricao ?? string.Empty;
            }
        }
    }
}
