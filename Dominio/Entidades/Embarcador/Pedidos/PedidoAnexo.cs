namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_ANEXO", EntityName = "PedidoAnexo", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo", NameType = typeof(PedidoAnexo))]
    public class PedidoAnexo : Anexo.Anexo<Pedido>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Pedido EntidadeAnexo { get; set; }

        #endregion

        public virtual PedidoAnexo Clonar()
        {
            return (PedidoAnexo)this.MemberwiseClone();
        }
    }
}
