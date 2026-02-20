using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOG_ACESSO", EntityName = "LogAcesso", Name = "Dominio.Entidades.LogAcesso", NameType = typeof(LogAcesso))]
    public class LogAcesso : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Id", Type = "System.Int64", Column = "LOG_ID")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Id { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IPAcesso", Column = "LOG_IPACESSO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IPAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Login", Column = "LOG_LOGIN", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Login { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "LOG_SENHA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "LOG_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SessionID", Column = "LOG_SESSIONID", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SessionID { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "LOG_TIPO", TypeType = typeof(Enumeradores.TipoLogAcesso), NotNull = false)]
        public virtual Enumeradores.TipoLogAcesso Tipo { get; set; }
    }
}
