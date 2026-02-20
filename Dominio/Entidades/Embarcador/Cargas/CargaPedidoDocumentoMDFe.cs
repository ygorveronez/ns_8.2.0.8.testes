namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_DOCUMENTO_MDFE", EntityName = "CargaPedidoDocumentoMDFe", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe", NameType = typeof(CargaPedidoDocumentoMDFe))]
    public class CargaPedidoDocumentoMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe)this.MemberwiseClone();
        }

        public virtual string Descricao
        {
            get
            {
                return (this.CargaPedido?.Carga?.CodigoCargaEmbarcador ?? string.Empty) + " - " + (this.MDFe?.Descricao ?? string.Empty);
            }
        }
    }
}
