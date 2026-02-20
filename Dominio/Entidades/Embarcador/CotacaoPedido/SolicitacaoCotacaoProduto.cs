using System;

namespace Dominio.Entidades.Embarcador.CotacaoPedido
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_COTACAO_PRODUTO", EntityName = "SolicitacaoCotacaoProduto", Name = "Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacaoProduto", NameType = typeof(SolicitacaoCotacaoProduto))]
    public class SolicitacaoCotacaoProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacaoProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "SCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoCotacao", Column = "SCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CotacaoPedido.SolicitacaoCotacao SolicitacaoCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "SCP_QUANTIDADE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoUnitario", Column = "SCP_PESO_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrecoUnitario", Column = "SCP_PRECO_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PrecoUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlturaCM", Column = "SCP_ALTURA_CM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal AlturaCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LarguraCM", Column = "SCP_LARGURA_CM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal LarguraCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComprimentoCM", Column = "SCP_COMPRIMENTO_CM", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ComprimentoCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubico", Column = "SCP_METRO_CUBICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetroCubico { get; set; }


        public virtual bool Equals(SolicitacaoCotacaoProduto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
