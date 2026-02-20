namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ENTREGA_CTE", EntityName = "CTeEntrega", Name = "Dominio.Entidades.CTeEntrega", NameType = typeof(CTeEntrega))]
    public class CTeEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Entrega", Column = "ENT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entrega Entrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Finalizado", Column = "CEN_FINALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Finalizado { get; set; }
    }
}
