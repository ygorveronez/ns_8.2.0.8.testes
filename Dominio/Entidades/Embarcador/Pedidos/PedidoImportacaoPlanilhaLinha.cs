namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_IMPORTACAO_PLANILHA_LINHA", EntityName = "PedidoImportacaoPlanilhaLinha", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoImportacaoPlanilhaLinha", NameType = typeof(PedidoImportacaoPlanilhaLinha))]
    public class PedidoImportacaoPlanilhaLinha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoImportacaoPlanilha", Column = "PIP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoImportacaoPlanilha PedidoImportacaoPlanilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PIL_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }
    }
}
