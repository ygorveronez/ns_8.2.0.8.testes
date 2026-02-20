using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ENVIO_EMAIL_LOG", EntityName = "CargaEnvioEmailLog", Name = "Dominio.Entidades.Embarcador.Cargas.CargaEnvioEmailLog", NameType = typeof(CargaEnvioEmailLog))]
    public class CargaEnvioEmailLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CEL_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Emails", Column = "CEL_EMAILS", Type = "StringClob", NotNull = true)]
        public virtual string Emails { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CEL_OBSERVACAO", Type = "StringClob", NotNull = false)]
        public virtual string Observacao { get; set; }
    }
}
