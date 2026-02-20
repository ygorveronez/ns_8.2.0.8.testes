using System;

namespace Dominio.Entidades.Embarcador.Rateio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RATEIO_CARGA_PEDIDO_CTE_PRODUTO", EntityName = "RateioCargaPedidoProduto", Name = "Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto", NameType = typeof(RateioCargaPedidoProduto))]
    public class RateioCargaPedidoProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RNC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaDocumentoParaEmissaoNFSManual", Column = "NEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual CargaDocumentoParaEmissaoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaNFS", Column = "CNS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaNFS CargaNFS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeComplementoInfo", Column = "CCC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo CargaCTeComplementoInfo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "RNC_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "RNC_QUANTIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitarioRateio", Column = "RNC_VALOR_UNITARIO_RATEIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitarioRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalRateio", Column = "RNC_VALOR_TOTAL_RATEIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitarioProduto", Column = "RNC_VALOR_UNITARIO_PRODUTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitarioProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "RNC_VALOR_ICMS", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSST", Column = "RNC_VALOR_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalFrete", Column = "RNC_VALOR_TOTAL_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "RNC_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteRateio", Column = "RNC_VALOR_FRETE_RATEIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteRateio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorValePedagioRateio", Column = "RNC_VALOR_VALE_PEDAGIO_RATEIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorValePedagioRateio { get; set; }

        public virtual bool Equals(RateioCargaPedidoProduto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
