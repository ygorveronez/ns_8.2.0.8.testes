namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_COMPRA_REQUISICAO", EntityName = "OrdemCompraRequisicao", Name = "Dominio.Entidades.Embarcador.Compras.OrdemCompraRequisicao", NameType = typeof(OrdemCompraRequisicao))]
    public class OrdemCompraRequisicao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemCompra", Column = "ORC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.OrdemCompra OrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RequisicaoMercadoria", Column = "RME_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria Requisicao { get; set; }

        
        public virtual string Descricao
        {
            get
            {
                return this.Requisicao?.Descricao ?? string.Empty;
            }
        }
    }
}
