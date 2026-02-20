using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTATO_GRUPO_PESSOA_DADO", EntityName = "ContatoGrupoPessoaDado", Name = "Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoaDado", NameType = typeof(ContatoGrupoPessoaDado))]
    public class ContatoGrupoPessoaDado : EntidadeBase, IEquatable<ContatoGrupoPessoaDado>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "CGD_NOME", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "CGD_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "CGD_TELEFONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Celular", Column = "CGD_CELULAR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Celular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContatoGrupoPessoa", Column = "CGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContatoGrupoPessoa ContatoGrupoPessoa { get; set; }

        public virtual bool Equals(ContatoGrupoPessoaDado other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
