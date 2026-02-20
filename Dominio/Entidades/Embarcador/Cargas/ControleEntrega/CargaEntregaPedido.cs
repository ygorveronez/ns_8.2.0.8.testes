namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_PEDIDO", EntityName = "CargaEntregaPedido", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido", NameType = typeof(CargaEntregaPedido))]
    public class CargaEntregaPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }
        public virtual string Descricao => Codigo.ToString();

        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido)this.MemberwiseClone();
        }
    }
}