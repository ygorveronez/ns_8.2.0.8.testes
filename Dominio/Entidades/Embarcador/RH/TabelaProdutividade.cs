using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.RH
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_PRODUTIVIDADE", EntityName = "TabelaProdutividade", Name = "Dominio.Entidades.Embarcador.RH.TabelaProdutividade", NameType = typeof(TabelaProdutividade))]
    public class TabelaProdutividade : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.RH.TabelaProdutividade>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TAP_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TAP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Valores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_PRODUTIVIDADE_VALORES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TAP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaProdutividade", Column = "TPV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores> Valores { get; set; }

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

        public virtual bool Equals(TabelaProdutividade other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
