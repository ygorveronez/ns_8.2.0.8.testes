namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RELACAO_CTES_ENTREGUES_CTE", EntityName = "RelacaoCTesEntreguesCTes", Name = "Dominio.Entidades.RelacaoCTesEntreguesCTes", NameType = typeof(RelacaoCTesEntreguesCTes))]
    public class RelacaoCTesEntreguesCTes : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RelacaoCTesEntregues", Column = "RCE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RelacaoCTesEntregues RelacaoCTesEntregues { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCC_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }
    }
}
