namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_NEOKOHM", EntityName = "IntegracaoNeokohm", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNeokohm", NameType = typeof(IntegracaoNeokohm))]
    public class IntegracaoNeokohm : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoNeokohm", Column = "CIN_POSSUI_INTEGRACAO_NEOKOHM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoNeokohm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLIntegracaoNeokohm", Column = "CIN_URL_INTEGRACAO_NEOKOHM", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string URLIntegracaoNeokohm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenNeokohm", Column = "CIN_TOKEN_NEOKOHM", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TokenNeokohm { get; set; }
    }
}
