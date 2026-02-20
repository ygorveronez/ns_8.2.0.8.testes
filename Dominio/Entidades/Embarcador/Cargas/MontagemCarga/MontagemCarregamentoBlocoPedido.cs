namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONTAGEM_CARREGAMENTO_BLOCO_PEDIDO", EntityName = "MontagemCarregamentoBlocoPedido", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido", NameType = typeof(MontagemCarregamentoBlocoPedido))]
    public class MontagemCarregamentoBlocoPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MBP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MontagemCarregamentoBloco", Column = "MCB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBloco Bloco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }

        //public virtual decimal Peso { get; set; }
        //public virtual decimal MetroCubico { get; set; }
        //public virtual decimal QuantidadePallet { get; set; }
        //public virtual decimal Volumes { get; set; }
    }
}
