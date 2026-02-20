using Dominio.Entidades.Embarcador.Produtos;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_LICITACAO_PRODUTO", EntityName = "SolicitacaoLicitacaoProduto", Name = "Dominio.Entidades.Embarcador.Frete.SolicitacaoLicitacaoProduto", NameType = typeof(SolicitacaoLicitacaoProduto))]
    public class SolicitacaoLicitacaoProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SLP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoLicitacao", Column = "SLI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SolicitacaoLicitacao SolicitacaoLicitacao { get; set; }
    }
}
