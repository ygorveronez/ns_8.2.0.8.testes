using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_OPENTECH", EntityName = "ProdutoOpentech", Name = "Dominio.Entidades.Embarcador.Embarcador.ProdutoOpentech", NameType = typeof(ProdutoOpentech))]
    public class ProdutoOpentech : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "POO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguro", Column = "APS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro ApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoOpentech", Column = "POO_CODIGO_PRODUTO_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoProdutoOpentech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "POO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDe", Column = "INF_VALOR_DE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorDe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAte", Column = "INF_VALOR_ATE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorAte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmbarcador", Column = "POO_CODIGO_EMBARCADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Estados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTO_OPENTECH_ESTADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "POO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Dominio.Entidades.Estado> Estados { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "ListaEstados", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + produtoOpentech.UF_SIGLA
                                                                                        FROM T_PRODUTO_OPENTECH_ESTADO produtoOpentech
                                                                                        WHERE produtoOpentech.POO_CODIGO = POO_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string ListaEstados { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "Localidades", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRODUTO_OPENTECH_LOCALIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "POO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Localidade> Localidades { get; set; }

        public virtual bool Equals(ProdutoOpentech other)
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


        public virtual string Descricao
        {
            get
            {
                return this.TipoOperacao?.Descricao ?? string.Empty;
            }
        }
    }
}
