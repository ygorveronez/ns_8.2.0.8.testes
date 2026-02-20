using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Email
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIG_EMAIL_DOC_TRANSPORTE", EntityName = "ConfigEmailDocTransporte", Name = "Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte", NameType = typeof(ConfigEmailDocTransporte))]
    public class ConfigEmailDocTransporte : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "EPC_EMAIL", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "EPC_USUARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "EPC_Senha", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Smtp", Column = "EPC_SMTP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Smtp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PortaSmtp", Column = "EPC_PORTA_SMTP", TypeType = typeof(int), NotNull = false)]
        public virtual int PortaSmtp { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "RequerAutenticacaoSmtp", Column = "EPC_REQUER_AUTENTICACAO_SMPT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RequerAutenticacaoSmtp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pop3", Column = "EPC_POP3", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Pop3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PortaPop3", Column = "EPC_PORTA_POP3", TypeType = typeof(int), NotNull = false)]
        public virtual int PortaPop3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RequerAutenticacaoPop3", Column = "EPC_REQUER_AUTENTICACAO_POP3", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RequerAutenticacaoPop3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailAtivo", Column = "EPC_EMAIL_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailAtivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRodape", Column = "EPC_MENSAGEM_RODAPE", TypeType = typeof(string), Length = 600, NotNull = true)]
        public virtual string MensagemRodape { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisplayEmail", Column = "EPC_DISPLAY_EMAIL", TypeType = typeof(string), Length = 600, NotNull = false)]
        public virtual string DisplayEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pop3Ativo", Column = "EPC_POP_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Pop3Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SmtpAtivo", Column = "EPC_SMTP_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SmtpAtivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LerDocumentos", Column = "EPC_LER_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LerDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarDocumentos", Column = "EPC_ENVIAR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoConexaoEmail", Column = "EPC_TIPO_CONEXAO_EMAIL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail TipoConexaoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientId", Column = "EPC_CLIENT_ID", TypeType = typeof(string), Length = 600, NotNull = false)]
        public virtual string ClientId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClientSecret", Column = "EPC_CLIENT_SECRET", TypeType = typeof(string), Length = 600, NotNull = false)]
        public virtual string ClientSecret { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TenantId", Column = "EPC_TENANT_ID", TypeType = typeof(string), Length = 600, NotNull = false)]
        public virtual string TenantId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RedirectUri", Column = "EPC_REDIRECT_URI", TypeType = typeof(string), Length = 600, NotNull = false)]
        public virtual string RedirectUri { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "ApiEnvioEmail", Column = "EPC_API_ENVIO_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ApiEnvioEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ServidorEmail", Column = "EPC_SERVIDOR_EMAIL", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServidorEmail), NotNull = true)]
        public virtual TipoServidorEmail ServidorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlEnvio", Column = "EPC_URL_ENVIO", TypeType = typeof(string), Length = 600, NotNull = false)]
        public virtual string UrlEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlAutenticacao", Column = "EPC_URL_AUTENTICACAO", TypeType = typeof(string), Length = 600, NotNull = false)]
        public virtual string UrlAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "caminhoTokenResposta", Column = "EPC_CAMINHO_TOKEN_RESPOSTA", TypeType = typeof(string), Length = 25000, NotNull = false)]
        public virtual string caminhoTokenResposta { get; set; }

        public virtual bool Equals(ConfigEmailDocTransporte other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get
            {
                return this.Email;
            }
        }

    }
}
