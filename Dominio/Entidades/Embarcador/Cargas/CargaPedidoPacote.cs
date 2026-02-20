namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_PACOTE", EntityName = "CargaPedidoPacote", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote", NameType = typeof(CargaPedidoPacote))]
    public class CargaPedidoPacote : EntidadeBase
    {
        public CargaPedidoPacote() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pacote", Column = "PCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Pacote Pacote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoPrincipal", Column = "CPP_DOCUMENTO_PRINCIPAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoPrincipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalDocumentosAdicionais", Column = "CPP_TOTAL_DOCUMENTOS_ADICIONAIS", TypeType = typeof(int), NotNull = false)]
        public virtual int TotalDocumentosAdicionais { get; set; }
    }
}
