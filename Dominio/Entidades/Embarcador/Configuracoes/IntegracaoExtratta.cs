namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_EXTRATTA", EntityName = "IntegracaoExtratta", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta", NameType = typeof(IntegracaoExtratta))]

    public class IntegracaoExtratta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIE_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "CIE_TOKEN", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJAplicacao", Column = "CIE_CNPJ_APLICACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CNPJAplicacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJEmpresa", Column = "CIE_CNPJ_EMPRESA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CNPJEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoUsuario", Column = "CIE_DOCUMENTO_USUARIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string DocumentoUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIE_USUARIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegrarAbastecimentoComTicketLog", Column = "CIE_INTEGRAR_ABASTECIMENTO_TICKET_LOG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarAbastecimentoComTicketLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoClienteTicketLog", Column = "CIE_CODIGO_CLIENTE_TICKET_LOG", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoClienteTicketLog { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoTicketLog", Column = "CIE_CODIGO_PRODUTO_TICKET_LOG", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoProdutoTicketLog { get; set; }
    }
}

