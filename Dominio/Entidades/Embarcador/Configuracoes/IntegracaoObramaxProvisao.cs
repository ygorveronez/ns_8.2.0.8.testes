namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_OBRAMAX_PROVISAO", EntityName = "IntegracaoObramaxProvisao", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramaxProvisao", NameType = typeof(IntegracaoObramaxProvisao))]
    public class IntegracaoObramaxProvisao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoObramaxProvisao", Column = "CIO_POSSUI_INTEGRACAO_PROVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoObramaxProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EndpointObramaxProvisao", Column = "CIO_ENDPOINT_PROVISAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EndpointObramaxProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenObramaxProvisao", Column = "CIO_TOKEN_PROVISAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TokenObramaxProvisao { get; set; }
    }
}
