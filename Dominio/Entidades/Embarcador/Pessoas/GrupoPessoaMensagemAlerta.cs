using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_MENSAGEM_ALERTA", EntityName = "GrupoPessoaMensagemAlerta", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta", NameType = typeof(GrupoPessoaMensagemAlerta))]
    public class GrupoPessoaMensagemAlerta : EntidadeBase, IEquatable<GrupoPessoaMensagemAlerta>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GPM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemAlerta", Column = "GPM_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string MensagemAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tag", Column = "GPM_TAG", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Tag { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }

        public virtual bool Equals(GrupoPessoaMensagemAlerta other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
