using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Contatos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_CONTATO", EntityName = "PessoaContato", Name = "Dominio.Entidades.Embarcador.Contatos.PessoaContato", NameType = typeof(PessoaContato))]
    public class PessoaContato : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCO_CONTATO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Contato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCO_EMAIL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCO_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCO_TELEFONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposContato", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PESSOA_CONTATO_TIPO_CONTATO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoContato", Column = "TCO_CODIGO")]
        public virtual ICollection<TipoContato> TiposContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF", Column = "PCO_CPF", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cargo", Column = "PCO_CARGO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Cargo { get; set; }

        public virtual string DescricaoTipoContato
        {
            get
            {
                if (TiposContato != null)
                    return string.Join(", ", TiposContato.Select(o => o.Descricao));
                else
                    return string.Empty;
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual string Descricao
        {
            get
            {
                return Contato;
            }
        }
    }
}
