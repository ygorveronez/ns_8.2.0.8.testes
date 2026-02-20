namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLETA_CONTAINER_CARGA_ENTREGA", EntityName = "ColetaContainerCargaEntrega", Name = "Dominio.Entidades.Embarcador.Pedidos.ColetaContainerCargaEntrega", NameType = typeof(ColetaContainerCargaEntrega))]

    public class ColetaContainerCargaEntrega : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ColetaContainer", Column = "CCR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer ColetaContainer { get; set; }
    }
}
