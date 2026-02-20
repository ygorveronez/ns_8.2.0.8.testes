namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ONISYS", EntityName = "IntegracaoOnisys", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoOnisys", NameType = typeof(IntegracaoOnisys))]
    public class IntegracaoOnisys : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CIO_URL_AUTENTICACAO", Type = "AnsiString", Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIO_USUARIO", Type = "AnsiString", Length = 150, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIO_SENHA", Type = "AnsiString", Length = 150, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "CIO_URL_INTEGRACAO", Type = "AnsiString", Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIO_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }
    }
}
