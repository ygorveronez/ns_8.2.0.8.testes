namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_CTE_ANTERIORES_LOGGI", EntityName = "IntegracaoCTeAnterioresLoggi", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTeAnterioresLoggi", NameType = typeof(IntegracaoCTeAnterioresLoggi))]
    public class IntegracaoCTeAnterioresLoggi : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIL_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLAutenticacao", Column = "CIL_URL_AUTENTICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientID", Column = "CIL_CLIENT_ID", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "CIL_CLIENT_SECRET", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Scope", Column = "CIL_SCOPE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Scope { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLEnvioDocumentos", Column = "CIL_URL_ENVIO_DOCUMENTOS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEnvioDocumentos { get; set; }

    }
}
