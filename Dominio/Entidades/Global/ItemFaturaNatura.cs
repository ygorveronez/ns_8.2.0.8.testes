namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NATURA_FATURA_ITEM", EntityName = "ItemFaturaNatura", Name = "Dominio.Entidades.ItemFaturaNatura", NameType = typeof(ItemFaturaNatura))]
    public class ItemFaturaNatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturaNatura", Column = "FAN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FaturaNatura Fatura { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscalDocumentoTransporteNatura", Column = "NDT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscalDocumentoTransporteNatura NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMS", Column = "FAI_ALIQUOTA_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "FAI_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "FAI_VALOR_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaICMSST", Column = "FAI_ALIQUOTA_ICMS_ST", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IVAICMSST", Column = "FAI_IVA_ICMS_ST", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal IVAICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMSST", Column = "FAI_BASE_CALCULO_ICMS_ST", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSST", Column = "FAI_VALOR_ICMS_ST", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPIS", Column = "FAI_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoPIS", Column = "FAI_BASE_CALCULO_PIS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPIS", Column = "FAI_VALOR_PIS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCOFINS", Column = "FAI_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoCOFINS", Column = "FAI_BASE_CALCULO_COFINS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCOFINS", Column = "FAI_VALOR_COFINS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaISS", Column = "FAI_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal AliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoISS", Column = "FAI_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "FAI_VALOR_ISS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDoDesconto", Column = "FAI_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorDoDesconto { get; set; }

    }
}
