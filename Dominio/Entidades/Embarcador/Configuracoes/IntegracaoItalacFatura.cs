namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ITALAC_FATURA", EntityName = "IntegracaoItalacFatura", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalacFatura", NameType = typeof(IntegracaoItalacFatura))]
    public class IntegracaoItalacFatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoItalacFatura", Column = "CIF_POSSUI_INTEGRACAO_ITALAC_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoItalacFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLItalacFatura", Column = "CIF_URL_ITALAC_FATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLItalacFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioItalacFatura", Column = "CIF_USUARIO_ITALAC_FATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string UsuarioItalacFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaItalacFatura", Column = "CIF_SENHA_ITALAC_FATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string SenhaItalacFatura { get; set; }
    }
}