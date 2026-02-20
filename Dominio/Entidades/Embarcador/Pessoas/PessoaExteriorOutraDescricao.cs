namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_EXTERIOR_OUTRA_DESCRICAO", EntityName = "PessoaExteriorOutraDescricao", Name = "Dominio.Entidades.Embarcador.Pessoas.PessoaExteriorOutraDescricao", NameType = typeof(PessoaExteriorOutraDescricao))]
    public class PessoaExteriorOutraDescricao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "POD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "POD_RAZAO_SOCIAL", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string RazaoSocial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "POD_ENDERECO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Endereco { get; set; }

        public virtual string Descricao
        {
            get
            {
                return RazaoSocial + " - " + Endereco;
            }
        }
    }
}
