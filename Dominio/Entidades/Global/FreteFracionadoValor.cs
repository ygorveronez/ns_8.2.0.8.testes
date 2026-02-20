namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FRETE_FRACIONADO_VALOR", EntityName = "FreteFracionadoValor", Name = "Dominio.Entidades.FreteFracionadoValor", NameType = typeof(FreteFracionadoValor))]
    public class FreteFracionadoValor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "FFV_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDe", Column = "FFV_VALOR_DE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorDe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAte", Column = "FFV_VALOR_ATE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorAte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "FFV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualGris", Column = "FFV_GRIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualGris { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAdValorem", Column = "FFV_ADVALORE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "FFV_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorExcedente", Column = "FFV_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiICMS", Column = "FFV_INCLUI_ICMS", TypeType = typeof(Dominio.Enumeradores.IncluiICMSFrete), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.IncluiICMSFrete IncluiICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "FFV_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCliente", Column = "FFV_TOMADOR", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador TipoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTAS", Column = "FFV_VALOR_TAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTAS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoGris", Column = "FFV_MINIMO_GRIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoGris { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoAdValorem", Column = "FFV_MINIMO_ADVALORE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "FFV_TIPO_VALOR", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string TipoValor { get; set; }

    }
}
