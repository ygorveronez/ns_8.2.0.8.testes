namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COTACAO_REQUISICAO_MERCADORIA", EntityName = "CotacaoCompraRequisicaoMercadoria", Name = "Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria", NameType = typeof(CotacaoCompraRequisicaoMercadoria))]
    public class CotacaoCompraRequisicaoMercadoria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoCompra", Column = "COT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CotacaoCompra Cotacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RequisicaoMercadoria", Column = "RME_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RequisicaoMercadoria RequisicaoMercadoria { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
