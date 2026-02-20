namespace Dominio.Entidades.Embarcador.Email
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMAIL_GLOBALIZADO_FORNECEDOR_CLIENTE", EntityName = "EmailGlobalizadoFornecedorCliente", Name = "Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorCliente", NameType = typeof(EmailGlobalizadoFornecedorCliente))]
    public class EmailGlobalizadoFornecedorCliente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ECT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmailGlobalizadoFornecedor", Column = "EGF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmailGlobalizadoFornecedor EmailGlobalizadoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }
    }
}
