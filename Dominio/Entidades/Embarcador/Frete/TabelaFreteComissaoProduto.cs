using System;


namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_COMISSAO_PRODUTO", EntityName = "TabelaFreteComissaoProduto", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto", NameType = typeof(TabelaFreteComissaoProduto))]
    public class TabelaFreteComissaoProduto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador ContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualValorProduto", Column = "TFC_PERCENTUAL_VALOR_PRODUTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualValorProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TFC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        #region Obsoletos

        [Obsolete("Não utilizar, será removido")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [Obsolete("Não utilizar, será removido")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLimitePercentual", Column = "TFC_VALOR_LIMITE_PERCENTUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorLimitePercentual { get; set; }

        [Obsolete("Não utilizar, será removido")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorToneladaEntregue", Column = "TFC_VALOR_TONELADA_ENTREGUE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorToneladaEntregue { get; set; }

        #endregion

        public virtual TabelaFreteComissaoProduto Clonar()
        {
            return (TabelaFreteComissaoProduto)this.MemberwiseClone();
        }

        public virtual bool Equals(TabelaFreteComissaoProduto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

    }
}
