namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MERCADO_LIVRE_HANDLING_UNIT_DETAIL_ARQUIVO", EntityName = "MercadoLivreHandlingUnitDetailArquivo", Name = "Dominio.Entidades.Embarcardor.Integracao.MercadoLivreHandlingUnitDetailArquivo", NameType = typeof(MercadoLivreHandlingUnitDetailArquivo))]
    public class MercadoLivreHandlingUnitDetailArquivo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "MDA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MercadoLivreHandlingUnitDetail", Column = "MUD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail HandlingUnitDetail { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoCTeParaSubContratacao", Column = "PSC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao PedidoCTeParaSubcontratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PedidoXMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDA_KEY", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Key { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDA_NOME_ARQUIVO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDA_TIPO_DOCUMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMercadoLivreHandlingUnit), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMercadoLivreHandlingUnit TipoDocumento { get; set; }

    }
}
