using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Rateio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RATEIO_PROVISAO_PRODUTO", EntityName = "RateioProvisaoProduto", Name = "Dominio.Entidades.Embarcador.Rateio.RateioProvisaoProduto", NameType = typeof(RateioProvisaoProduto))]
    public class RateioProvisaoProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }       

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Provisao", Column = "PRV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.Provisao Provisao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Produtos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RATEIO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoProduto", Column = "PRP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> Produtos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProduto", Column = "GPR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProduto GrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoTotalProdutos", Column = "RPP_TOTAL_PRODUTOS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoTotalProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeTotalProdutos", Column = "RPP_QUANTIDADE_TOTAL_PRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeTotalProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalRateio", Column = "RPP_VALOR_TOTAL_RATEIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalRateio { get; set; }

    }
}
