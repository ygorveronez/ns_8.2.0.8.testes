using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_CLASSIFICACAO", EntityName = "PessoaClassificacao", Name = "Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao", NameType = typeof(PessoaClassificacao))]
    public class PessoaClassificacao : EntidadeBase, IEquatable<PessoaClassificacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PCL_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCL_CLASSE", TypeType = typeof(PessoaClasse), NotNull = true)]
        public virtual PessoaClasse Classe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cor", Column = "PCL_COR", TypeType = typeof(string), Length = 7, NotNull = false)]
        public virtual string Cor { get; set; }

        public virtual bool Equals(PessoaClassificacao other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
