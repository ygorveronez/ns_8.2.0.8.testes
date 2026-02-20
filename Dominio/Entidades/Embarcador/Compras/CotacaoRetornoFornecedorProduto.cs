using System;

namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COTACAO_RETORNO_FORNECEDOR_PRODUTO", EntityName = "CotacaoRetornoFornecedorProduto", Name = "Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto", NameType = typeof(CotacaoRetornoFornecedorProduto))]
    public class CotacaoRetornoFornecedorProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeRetorno", Column = "CRF_QUANTIDADE_RETORNO", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal QuantidadeRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitarioRetorno", Column = "CRF_VALOR_UNITARIO_RETORNO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorUnitarioRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalRetorno", Column = "CRF_VALOR_TOTAL_RETORNO", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal ValorTotalRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_DATA_PREVISAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_GERAR_ORDEM_COMPRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CondicaoPagamento", Column = "CPA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CondicaoPagamento CondicaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoFornecedor", Column = "COF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CotacaoFornecedor CotacaoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoProduto", Column = "COP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CotacaoProduto CotacaoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRF_MARCA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Marca { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }
    }
}
