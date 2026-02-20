namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_GLOBUS", EntityName = "IntegracaoGlobus", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus", NameType = typeof(IntegracaoGlobus))]

    public class IntegracaoGlobus : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_POSSUI_INTEGRACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_INTEGRAR_COM_CONTABILIDADE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IntegrarComContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_INTEGRAR_COM_ESCRITA_FISCAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IntegrarComEscritaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_INTEGRAR_COM_CONTAS_A_PAGAR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IntegrarComContasPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_INTEGRAR_COM_CONTAS_A_RECEBER", TypeType = typeof(bool), NotNull = true)]
        public virtual bool IntegrarComContasReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ShortCodeEscrituracaoISS", Column = "CIG_SHORT_CODE_ESCRITURACAO_ISS", TypeType = typeof(int), NotNull = false)]
        public virtual int ShortCodeEscrituracaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ShortCodeNFSe", Column = "CIG_SHORT_CODE_NFSE", TypeType = typeof(int), NotNull = false)]
        public virtual int ShortCodeNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ShortCodeFinanceiro", Column = "CIG_SHORT_CODE_FINANCEIRO", TypeType = typeof(int), NotNull = false)]
        public virtual int ShortCodeFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ShortCodeXML", Column = "CIG_SHORT_CODE_XML", TypeType = typeof(int), NotNull = false)]
        public virtual int ShortCodeXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ShortCodeParticipante", Column = "CIG_SHORT_CODE_PARTICIPANTE", TypeType = typeof(int), NotNull = false)]
        public virtual int ShortCodeParticipante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_CODIGO_INTEGRAR_COM_CONTABILIDADE", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoIntegrarComContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_CODIGO_INTEGRAR_COM_ESCRITA_FISCAL", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoIntegrarComEscritaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_CODIGO_INTEGRAR_COM_CONTAS_A_PAGAR", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoIntegrarComContasPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_CODIGO_INTEGRAR_COM_CONTAS_A_RECEBER", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoIntegrarComContasReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_SISTEMA_INTEGRAR_COM_CONTABILIDADE", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string SistemaIntegrarComContabilidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_SISTEMA_INTEGRAR_COM_ESCRITA_FISCAL", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string SistemaIntegrarComEscritaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_SISTEMA_INTEGRAR_COM_CONTAS_A_PAGAR", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string SistemaIntegrarComContasPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIG_SISTEMA_INTEGRAR_COM_CONTAS_A_RECEBER", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string SistemaIntegrarComContasReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLWebServiceEscrituracaoISS", Column = "CIG_URL_WEBSERVICE_ESCRITURACAO_ISS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLWebServiceEscrituracaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLWebServiceNFSe", Column = "CIG_URL_WEBSERVICE_NFSE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLWebServiceNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenEscrituracaoISS", Column = "CIG_TOKEN_ESCRITURACAO_ISS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string TokenEscrituracaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenNFSe", Column = "CIG_TOKEN_NFSE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string TokenNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLWebServiceFinanceiro", Column = "CIG_URL_WEBSERVICE_FINANCEIRO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLWebServiceFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenFinanceiro", Column = "CIG_TOKEN_FINANCEIRO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string TokenFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLWebServiceXML", Column = "CIG_URL_WEBSERVICE_XML", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLWebServiceXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenXML", Column = "CIG_TOKEN_XML", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string TokenXML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLWebServiceParticipante", Column = "CIG_URL_WEBSERVICE_PARTICIPANTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLWebServiceParticipante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenParticipante", Column = "CIG_TOKEN_PARTICIPANTE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string TokenParticipante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sistema", Column = "CIG_SISTEMA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Sistema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIG_USUARIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Grupo", Column = "CIG_GRUPO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Grupo { get; set; }
    }
}
