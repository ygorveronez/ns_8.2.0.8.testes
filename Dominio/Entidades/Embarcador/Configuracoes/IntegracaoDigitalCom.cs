using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_INTEGRACAO_DIGITALCOM", EntityName = "IntegracaoDigitalCom", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom", NameType = typeof(IntegracaoDigitalCom))]
    public class IntegracaoDigitalCom : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CID_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CID_VALIDACAO_TAG_DIGITALCOM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidacaoTAGDigitalCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EndpointDigitalCom", Column = "CID_ENDPOINT_DIGITALCOM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string EndpointDigitalCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioDigitalCom", Column = "CID_USUARIO_DIGITALCOM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsuarioDigitalCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaDigitalCom", Column = "CID_SENHA_DIGITALCOM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SenhaDigitalCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TokenDigitalCom", Column = "CID_TOKEN_DIGITALCOM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TokenDigitalCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJLogin", Column = "CID_CNPJ_LOGIN", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJLogin { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlAutenticacaoDigitalCom", Column = "CID_URL_AUTENTICACAO_DIGITALCOM", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UrlAutenticacaoDigitalCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoObtencaoCNPJTransportadora", Column = "CID_TIPO_OBTENCAO_CNPJ_TRANSPORTADORA", TypeType = typeof(TipoObtencaoCNPJTransportadora), NotNull = false)]
        public virtual TipoObtencaoCNPJTransportadora TipoObtencaoCNPJTransportadora { get; set; }
    }
}
