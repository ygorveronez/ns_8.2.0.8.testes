namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_TRANSSAT", EntityName = "IntegracaoTransSat", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTransSat", NameType = typeof(IntegracaoTransSat))]

    public class IntegracaoTransSat : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIT_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLWebServiceIntegracaoTransSat", Column = "CIT_URL_WEBSERVICE_INTEGRACAO_TRANSSAT", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLWebServiceIntegracaoTransSat { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenIntegracaoTransSat", Column = "CIT_TOKEN_INTEGRACAO_TRANSSAT", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string TokenIntegracaoTransSat { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailParaReceberRetornoDaGR", Column = "CIT_EMAIL_PARA_RECEBER_RETORNO_DA_GR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string EmailParaReceberRetornoDaGR { get; set; }
    }
}
