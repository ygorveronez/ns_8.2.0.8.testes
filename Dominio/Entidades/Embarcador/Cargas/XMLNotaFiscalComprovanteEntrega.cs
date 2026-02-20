namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_COMPROVANTE_ENTREGA", EntityName = "XMLNotaFiscalComprovanteEntrega", Name = "Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega", NameType = typeof(XMLNotaFiscalComprovanteEntrega))]
    public class XMLNotaFiscalComprovanteEntrega : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedidoXMLNotaFiscalCTe", Column = "CPX_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe Cte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteComprovanteEntrega", Column = "LCE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual LoteComprovanteEntrega LoteComprovanteEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivoImagem", Column = "NCE_NOME_ARQUIVO_IMAGEM", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string NomeArquivoImagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidNomeArquivoImagem", Column = "NCE_GUID_NOME_ARQUIVO_IMAGEM", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string GuidNomeArquivoImagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "NCE_LATITUDE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "NCE_LONGITUDE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Longitude { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DadosRecebedor", Column = "DRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor DadosRecebedor { get; set; }

        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal {
            get => PedidoXMLNotaFiscal?.XMLNotaFiscal;
        }

    }
}