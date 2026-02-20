namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_VLI", EntityName = "IntegracaoVLI", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI", NameType = typeof(IntegracaoVLI))]
    public class IntegracaoVLI : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VLI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracaoVLI", Column = "VLI_POSSUI_INTEGRACAO_VLI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracaoVLI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLI_URL_AUTENTICACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UrlAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLI_SENHA_AUTENTICACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLI_EDI_TOKEN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string EdiToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLI_CLIENT_ID_AUTENTICACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IDAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLI_URL_INTEGRACAO_RASTREAMENTO", TypeType = typeof(string), Length = 850, NotNull = false)]
        public virtual string UrlIntegracaoRastreamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLI_URL_INTEGRACAO_CARREGAMENTO", TypeType = typeof(string), Length = 850, NotNull = false)]
        public virtual string UrlIntegracaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLI_URL_INTEGRACAO_DESCARREGAMENTO", TypeType = typeof(string), Length = 850, NotNull = false)]
        public virtual string UrlIntegracaoDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VLI_URL_INTEGRACAO_DESCARREGAMENTO_PORTOS_VALE", TypeType = typeof(string), Length = 850, NotNull = false)]
        public virtual string UrlIntegracaoDescarregamentoPortosValeVLI { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração Ferrovia VLI";
            }
        }

    }
}
