namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPOSTO_IBPT", EntityName = "ImpostoIBPT", Name = "Dominio.Entidades.ImpostoIBPT", NameType = typeof(ImpostoIBPT))]
    public class ImpostoIBPT : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IBP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", Unique = true, NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "IBP_NCM", TypeType = typeof(string), Length = 9, NotNull = true)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "IBP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualMunicipal", Column = "IBP_PERCENTUAL_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 4, NotNull = true)]
        public virtual decimal PercentualMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualEstadual", Column = "IBP_PERCENTUAL_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 4, NotNull = true)]
        public virtual decimal PercentualEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualFederalNacional", Column = "IBP_PERCENTUAL_FEDERAL_NACIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 4, NotNull = true)]
        public virtual decimal PercentualFederalNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualFederalInternacional", Column = "IBP_PERCENTUAL_FEDERAL_INTERNACIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 4, NotNull = true)]
        public virtual decimal PercentualFederalInternacional { get; set; }
    }
}
