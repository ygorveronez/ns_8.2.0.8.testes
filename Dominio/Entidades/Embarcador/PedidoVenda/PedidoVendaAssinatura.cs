namespace Dominio.Entidades.Embarcador.PedidoVenda
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_VENDA_ASSINATURA", EntityName = "PedidoVendaAssinatura", Name = "Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaAssinatura", NameType = typeof(PedidoVendaAssinatura))]
    public class PedidoVendaAssinatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PVA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PVA_DESCRICAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PVA_NOME_ARQUIVO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PVA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoVenda", Column = "PEV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda PedidoVenda { get; set; }

    }
}
