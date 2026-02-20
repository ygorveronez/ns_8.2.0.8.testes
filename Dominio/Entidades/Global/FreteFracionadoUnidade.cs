namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FRETE_FRACIONADO_UNIDADE", EntityName = "FreteFracionadoUnidade", Name = "Dominio.Entidades.FreteFracionadoUnidade", NameType = typeof(FreteFracionadoUnidade))]
    public class FreteFracionadoUnidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FFU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "FFU_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "UnidadeDeMedida", Column = "UNI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual UnidadeDeMedida UnidadeDeMedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoDe", Column = "FFU_PESO_DE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PesoDe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoAte", Column = "FFU_PESO_ATE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal PesoAte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "FFU_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualGris", Column = "FFU_GRIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualGris { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAdValorem", Column = "FFU_ADVALORE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "FFU_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorExcedente", Column = "FFU_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiICMS", Column = "FFU_INCLUI_ICMS", TypeType = typeof(Dominio.Enumeradores.IncluiICMSFrete), Length = 1, NotNull = false)]
        public virtual Dominio.Enumeradores.IncluiICMSFrete IncluiICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "FFU_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCliente", Column = "FFU_TOMADOR", TypeType = typeof(Dominio.Enumeradores.TipoTomador), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador TipoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTAS", Column = "FFU_VALOR_TAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTAS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoGris", Column = "FFU_MINIMO_GRIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoGris { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMinimoAdValorem", Column = "FFU_MINIMO_ADVALORE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMinimoAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPorUnidadeMedida", Column = "FFU_VALOR_POR_UNIDADE_MEDIDA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorPorUnidadeMedida { get; set; }
    }
}
