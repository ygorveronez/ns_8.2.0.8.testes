namespace Dominio.Entidades.Embarcador.Email
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMAIL_GLOBALIZADO_FORNECEDOR_ANEXO", EntityName = "EmailGlobalizadoFornecedorAnexo", Name = "Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorAnexo", NameType = typeof(EmailGlobalizadoFornecedorAnexo))]
    public class EmailGlobalizadoFornecedorAnexo : Anexo.Anexo<EmailGlobalizadoFornecedor>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmailGlobalizadoFornecedor", Column = "EGF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override EmailGlobalizadoFornecedor EntidadeAnexo { get; set; }

        #endregion
    }
}
