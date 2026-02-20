using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.RH
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_PREMIO_PRODUTIVIDADE", EntityName = "TabelaPremioProdutividade", Name = "Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade", NameType = typeof(TabelaPremioProdutividade))]

    public class TabelaPremioProdutividade : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPP_PERCENTUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TPP_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "TPP_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "TPP_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "GrupoPessoas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_PREMIO_PRODUTIVIDADE_GRUPO_PESSOA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TPP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoas", Column = "GRP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> GrupoPessoas { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "% do prêmio: " + this.Percentual.ToString("n2") + " de " + this.DataInicio.ToString("dd/MM/yyyy") + " até " + this.DataFim.ToString("dd/MM/yyyy");
            }
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

        public virtual bool Equals(TabelaPremioProdutividade other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
