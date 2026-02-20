using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOG_ENVIO_SMS", EntityName = "LogEnvioSMS", Name = "Dominio.Entidades.Embarcador.NotaFiscal.LogEnvioSMS", NameType = typeof(LogEnvioSMS))]
    public class LogEnvioSMS : EntidadeBase, IEquatable<LogEnvioSMS>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "LES_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Celular", Column = "LES_CELULAR", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Celular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Link", Column = "LES_LINK", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Link { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnvio", Column = "LES_STATUS_ENVIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool StatusEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemEnvio", Column = "LES_MENSAGEM_ENVIO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemEnvio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual bool Equals(LogEnvioSMS other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
