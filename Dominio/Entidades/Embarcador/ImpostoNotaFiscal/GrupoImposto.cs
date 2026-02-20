using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.ImpostoNotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_IMPOSTO", EntityName = "GrupoImposto", Name = "Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto", NameType = typeof(GrupoImposto))]
    public class GrupoImposto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GRI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GRI_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "GRI_NCM", TypeType = typeof(string), Length = 8, NotNull = true)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "GRI_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ItensImposto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_IMPOSTO_ITENS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GRI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoImpostoItens", Column = "GII_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens> ItensImposto { get; set; }

        public virtual bool Equals(GrupoImposto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto Clonar()
        {
            return (Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto)this.MemberwiseClone();
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
