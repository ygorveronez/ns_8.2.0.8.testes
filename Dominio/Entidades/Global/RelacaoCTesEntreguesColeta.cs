namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RELACAO_CTES_ENTREGUES_COLETA", EntityName = "RelacaoCTesEntreguesColeta", Name = "Dominio.Entidades.RelacaoCTesEntreguesColeta", NameType = typeof(RelacaoCTesEntreguesColeta))]
    public class RelacaoCTesEntreguesColeta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RelacaoCTesEntregues", Column = "RCE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RelacaoCTesEntregues RelacaoCTesEntregues { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCO_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCO_PESO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCO_VALOR_EVENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCO_VALOR_FRACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFracao { get; set; }
    }
}
