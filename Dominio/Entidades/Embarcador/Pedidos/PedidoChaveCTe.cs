namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PEDIDO_CHAVE_CTE", EntityName = "PedidoChaveCTe", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoChaveCTe", NameType = typeof(PedidoChaveCTe))]
    public class PedidoChaveCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCTe", Column = "PCC_CHAVE_CTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveCTe { get; set; }

        public virtual string Descricao { get { return $"{Codigo} - {ChaveCTe}"; } }
    }
}
