namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_ESHIP", EntityName = "IntegracaoEship", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship", NameType = typeof(IntegracaoEship))]
    public class IntegracaoEship : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIE_POSSUI_INTEGRACAO", TypeType = typeof(bool), Length = 500, NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLComunicacao", Column = "CIE_URL_COMUNICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLComunicacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ApiToken", Column = "CIE_TOKEN_API", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ApiToken { get; set; }
    }
}