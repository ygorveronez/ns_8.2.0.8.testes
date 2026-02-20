namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_MARS", EntityName = "IntegracaoMars", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMars", NameType = typeof(IntegracaoMars))]
    public class IntegracaoMars : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIM_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRACAO_CARGA_CTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_INTEGRACAO_CANHOTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientID", Column = "CIM_CLIENT_ID", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClientID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIM_CLIENT_SECRET", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoCancelamentosCargas", Column = "CIM_URL_INTEGRACAO_CANCELAMENTOS_CARGAS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string URLIntegracaoCancelamentosCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientIDCancelamentosCargas", Column = "CIM_CLIENT_ID_CANCELAMENTOS_CARGAS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClientIDCancelamentosCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecretCancelamentosCargas", Column = "CIM_CLIENT_SECRET_CANCELAMENTOS_CARGAS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ClientSecretCancelamentosCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_URL_AUTENTICACAO_CANCELAMENTOS_CARGAS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacaoCancelamentosCargas { get; set; }
    }
}
