namespace Dominio.Entidades.Embarcador.GestaoEntregas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ENTREGA_PEDIDO_FOTO", EntityName = "EntregaPedidoFoto", Name = "Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedidoFoto", NameType = typeof(EntregaPedidoFoto))]
    public class EntregaPedidoFoto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EPF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EntregaPedido", Column = "ENP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EntregaPedido EntregaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EPF_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EPF_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        public virtual string Descricao => "Foto - " + (this.EntregaPedido?.Descricao ?? string.Empty);
    }
}
