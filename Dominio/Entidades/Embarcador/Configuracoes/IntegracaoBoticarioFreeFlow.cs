namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_BOTICARIO_FREE_FLOW", EntityName = "IntegracaoBoticarioFreeFlow", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow", NameType = typeof(IntegracaoBoticarioFreeFlow))]
    public class IntegracaoBoticarioFreeFlow : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIB_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracao", Column = "CIB_URL_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CIB_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIB_CLIENT_SECRET", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientId", Column = "CIB_CLIENT_ID", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ClientId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLConsultaAVIPED", Column = "CIB_URL_CONSULTA_AVIPED", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLConsultaAVIPED { get; set; }
    }
}
