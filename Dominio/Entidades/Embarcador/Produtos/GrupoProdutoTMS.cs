using System;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PRODUTO_TMS", EntityName = "GrupoProdutoTMS", Name = "Dominio.Entidades.Embarcador.Embarcador.GrupoProdutoTMS", NameType = typeof(GrupoProdutoTMS))]
    public class GrupoProdutoTMS : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GPR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GRP_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "GRP_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProdutoTMS", Column = "GPR_CODIGO_PAI", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS GrupoProdutoTMSPai { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarDepreciacao", Column = "GRP_GERAR_DEPRECIACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GerarDepreciacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDepreciacao", Column = "GRP_PERCENTUAL_DEPRECIACAO", TypeType = typeof(decimal), Scale = 2, Precision = 7, NotNull = false)]
        public virtual decimal PercentualDepreciacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_DEPRECIACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoDepreciacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_DEPRECIACAO_ACUMULADA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoDepreciacaoAcumulada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_BAIXA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoBaixa { get; set; }

        public virtual bool Equals(GrupoProdutoTMS other)
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