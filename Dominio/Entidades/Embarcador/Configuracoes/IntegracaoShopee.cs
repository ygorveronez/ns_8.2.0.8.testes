namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_SHOPEE", EntityName = "IntegracaoShopee", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoShopee", NameType = typeof(IntegracaoShopee))]
    public class IntegracaoShopee : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoPacote", Column = "CIS_POSSUI_INTEGRACAO_PACOTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoPacote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EndpointPacote", Column = "CIS_ENDPOINT_PACOTES", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EndpointPacote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIS_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIO_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }
    }
}
