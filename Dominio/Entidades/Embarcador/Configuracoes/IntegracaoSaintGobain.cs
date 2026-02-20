namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_SAINTGOBAIN", EntityName = "IntegracaoSaintGobain", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain", NameType = typeof(IntegracaoSaintGobain))]
    public class IntegracaoSaintGobain : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "APIKey", Column = "CIS_APIKey", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string APIKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientID", Column = "CIS_CLIENTID", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIS_CLIENT_SECRET", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlConsultaPedido", Column = "CIS_URL_CONSULTA_PEDIDO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlConsultaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlValidaToken", Column = "CIS_URL_TOKEN", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlValidaToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlConsultaUsuario", Column = "CIS_URL_CONSULTA_USUARIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlConsultaUsuario { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlIntegracaoCargaSnowFlake", Column = "CIS_URL_INTEGRACAO_CARGA_SNOW_FLAKE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlIntegracaoCargaSnowFlake { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlIntegracaoAgendamentoSnowFlake", Column = "CIS_URL_INTEGRACAO_AGENDAMENTO_SNOW_FLAKE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlIntegracaoAgendamentoSnowFlake { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlIntegracaoFreteSnowFlake", Column = "CIS_URL_INTEGRACAO_FRETE_SNOW_FLAKE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UrlIntegracaoFreteSnowFlake { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuariosSnowFlake", Column = "CIS_USUARIO_SNOW_FLAKE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string UsuariosSnowFlake { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaSnowFlake", Column = "CIS_SENHA_SNOW_FLAKE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string SenhaSnowFlake { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApiKeySnowFlake", Column = "CIS_API_KEY_SNOW_FLAKE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ApiKeySnowFlake { get; set; }     
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarEndPointPIPO", Column = "CIS_UTILIZAR_ENDPOINT_PIPO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarEndPointPIPO { get; set; }
    }
}
