using System;

namespace Dominio.Entidades.Embarcador.Email
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOG_ENVIO_EMAIL", EntityName = "LogEnvioEmail", Name = "Dominio.Entidades.Email.LogEnvioEmail", NameType = typeof(LogEnvioEmail))]
    public class LogEnvioEmail : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Email.LogEnvioEmail>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LEE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "LEE_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailRemetente", Column = "LEE_EMAIL_REMETENTE", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string EmailRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailDestinatario", Column = "LEE_EMAIL_DESTINATARIO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string EmailDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailResposta", Column = "LEE_EMAIL_RESPOSTA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string EmailResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailCopia", Column = "LEE_EMAIL_COPIA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string EmailCopia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailCopiaOculta", Column = "LEE_EMAIL_COPIA_OCULTA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string EmailCopiaOculta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoAnexo", Column = "LEE_DESCRICAO_ANEXO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string DescricaoAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Assunto", Column = "LEE_ASSUNTO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Assunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "LEE_MENSAGEM", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificacaoEmail", Column = "LEE_IDENTIFICACAO_EMAIL", TypeType = typeof(Enumeradores.IdentificacaoEmail), NotNull = false)]
        public virtual Enumeradores.IdentificacaoEmail IdentificacaoEmail { get; set; }

        public virtual bool Equals(LogEnvioEmail other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
