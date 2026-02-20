using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_RECEBEDOR_AUTORIZADO", EntityName = "PessoaRecebedorAutorizado", Name = "Dominio.Entidades.Embarcador.Pessoas.PessoaRecebedorAutorizado", NameType = typeof(PessoaRecebedorAutorizado))]
    public class PessoaRecebedorAutorizado : EntidadeBase, IEquatable<PessoaRecebedorAutorizado>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRA_NOME", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRA_CPF", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CPF { get; set; }

        // Guarda o nome do arquivo que contém a foto do recebedor
        [NHibernate.Mapping.Attributes.Property(0, Column = "PRA_GUID_FOTO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidFoto { get; set; }

        public virtual bool Equals(PessoaRecebedorAutorizado other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
