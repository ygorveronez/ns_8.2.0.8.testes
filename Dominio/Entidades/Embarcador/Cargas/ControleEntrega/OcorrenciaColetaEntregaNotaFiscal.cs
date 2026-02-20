namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_OCORRENCIA_COLETA_ENTREGA_NOTA_FISCAL", EntityName = "OcorrenciaColetaEntregaNotaFiscal", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaNotaFiscal", NameType = typeof(OcorrenciaColetaEntregaNotaFiscal))]
    public class OcorrenciaColetaEntregaNotaFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaColetaEntrega", Column = "OCE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OcorrenciaColetaEntrega OcorrenciaColetaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }
    }
}
