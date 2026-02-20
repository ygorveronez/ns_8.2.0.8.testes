namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_SAP_API4", EntityName = "IntegracaoSapAPI4", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSapAPI4", NameType = typeof(IntegracaoSapAPI4))]
    public class IntegracaoSapAPI4 : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO_API4")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoSAP_API4", Column = "CIS_POSSUI_INTEGRACAO_SAP_API4", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoSAP_API4 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_USUARIO_API4", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsuarioSAP_API4 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_SENHA_API4", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaSAP_API4 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_API4", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string URLSAP_API4 { get; set; }
    }
}
