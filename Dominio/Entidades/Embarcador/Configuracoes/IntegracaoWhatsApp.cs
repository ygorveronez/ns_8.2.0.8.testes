namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_WHATSAPP", EntityName = "IntegracaoWhatsApp", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWhatsApp", NameType = typeof(IntegracaoWhatsApp))]
    public class IntegracaoWhatsApp : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIW_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIW_POSSUI_INTEGRACAO", TypeType = typeof(bool), Length = 500, NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "CIW_TOKEN", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdContaWhatsAppBusiness", Column = "CIW_ID_CONTA_WHATSAPP_BUSINESS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string IdContaWhatsAppBusiness { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdNumeroTelefone", Column = "CIW_ID_NUMERO_TELEFONE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string IdNumeroTelefone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdAplicativo", Column = "CIW_ID_APLICATIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string IdAplicativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeTemplate", Column = "CIW_NOME_TEMPLATE", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string NomeTemplate { get; set; }
    }
}
