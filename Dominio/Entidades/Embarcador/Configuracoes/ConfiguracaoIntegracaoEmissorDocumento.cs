using Dominio.Entidades.WebService;
using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_EMISSOR_DOCUMENTO", EntityName = "ConfiguracaoIntegracaoEmissorDocumento", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento", NameType = typeof(ConfiguracaoIntegracaoEmissorDocumento))]
    public class ConfiguracaoIntegracaoEmissorDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração Emissor Documento"; }
        }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissorDocumentoCTe", Column = "CIE_TIPO_EMISSOR_DOCUMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento TipoEmissorDocumentoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEmissorDocumentoMDFe", Column = "CIE_TIPO_EMISSOR_DOCUMENTO_MDFE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento TipoEmissorDocumentoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_RESPONSALVEL_TECNICO_CNPJ", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string ResponsavelTecnicoCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_RESPONSALVEL_TECNICO_NOME_CONTATO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string ResponsavelTecnicoNomeContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_RESPONSALVEL_TECNICO_EMAIL", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string ResponsavelTecnicoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_RESPONSALVEL_TECNICO_TELEFONE", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string ResponsavelTecnicoTelefone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_NSTECH_EXTERNAL_ID", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NSTechExternalId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_NSTECH_URL_API", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NSTechUrlAPICte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_NSTECH_URL_API_MDFE", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NSTechUrlAPIMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_NSTECH_URL_API_WEBHOOK", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NSTechUrlAPIWebHook { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_NSTECH_URL_API_CERTIFICADO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NSTechUrlAPICertificado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_NSTECH_URL_API_LOGO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NSTechUrlAPILogo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_NSTECH_TOKEN_API_KEY", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NSTechTokenAPIKey { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, NotNull = false)]
        public virtual Integradora NSTechIntegradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_NSTECH_URL_WEBHOOK", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NSTechUrlWebhook { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_NSTECH_SUBSCRIBE_ID", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NSTechSubscribeId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIE_OBTER_XML_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObterXMLAutomaticamente { get; set; }

        [Obsolete("Exigências do CIOT em vigência")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacaoCIOT", Column = "CIE_DATA_LIBERACAO_CIOT", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLiberacaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacaoImpostos", Column = "CIE_DATA_LIBERACAO_IMPOSTOS", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLiberacaoImpostos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLiberacaoSomarIBSCBSTotalDocumentoFiscal", Column = "CIE_DATA_LIBERACAO_SOMAR_IBSCBS_TOTALDOCUMENTOFISCAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataLiberacaoSomarIBSCBSTotalDocumentoFiscal { get; set; }
    }
}
