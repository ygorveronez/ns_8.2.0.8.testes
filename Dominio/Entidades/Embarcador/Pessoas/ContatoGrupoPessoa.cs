using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTATO_GRUPO_PESSOA", EntityName = "ContatoGrupoPessoa", Name = "Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa", NameType = typeof(ContatoGrupoPessoa))]
    public class ContatoGrupoPessoa : EntidadeBase, IEquatable<ContatoGrupoPessoa>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CGP_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Contatos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTATO_GRUPO_PESSOA_DADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CGP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContatoGrupoPessoaDado", Column = "CGD_CODIGO")]
        public virtual IList<ContatoGrupoPessoaDado> Contatos { get; set; }

        public virtual bool Equals(ContatoGrupoPessoa other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
