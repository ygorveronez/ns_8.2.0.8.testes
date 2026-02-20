namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_FS", EntityName = "IntegracaoFS", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFS", NameType = typeof(IntegracaoFS))]
    public class IntegracaoFS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IFS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "IFS_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "IFS_URL_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "IFS_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientID", Column = "IFS_CLIENT_ID", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "IFS_CLIENT_SECRET", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientSecret { get; set; }
    }
}
