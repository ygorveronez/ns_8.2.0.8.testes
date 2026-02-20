namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_LOGGI", EntityName = "IntegracaoLoggi", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi", NameType = typeof(IntegracaoLoggi))]
    public class IntegracaoLoggi : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIL_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "CIL_URL_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlIntegracaoCTe", Column = "CIL_URL_INTEGRACAO_CTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlIntegracaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientID", Column = "CIL_CLIENT_ID", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIL_CLIENT_SECRET", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "CIL_TOKEN", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Token { get; set; } 
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "URLConsultaPacotes", Column = "CIL_CONSULTA_PACOTES", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLConsultaPacotes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Scope", Column = "CIL_SCOPE", TypeType = typeof(string), Length = 500, NotNull = false)]
		public virtual string Scope { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoEventoEntrega", Column = "CIL_URL_INTEGRACAO_EVENTO_ENTREGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoEventoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacaoEventoEntrega", Column = "CIL_URL_AUTENTICACAO_EVENTO_ENTREGA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacaoEventoEntrega { get; set; }

    }
}
