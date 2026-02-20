using System;

namespace Dominio.Entidades.Embarcador.Notificacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAT", EntityName = "Chat", Name = "Dominio.Entidades.Embarcador.Notificacoes.Chat", NameType = typeof(Chat))]
    public class Chat : EntidadeBase, IEquatable<Chat>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvio", Column = "CHT_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CHT_MENSAGEM", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Lida", Column = "CHT_LIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Lida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLida", Column = "CHT_DATA_LIDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviadoAvisoEmail", Column = "CHT_ENVIADO_AVISO_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviadoAvisoEmail { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_ENVIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioEnvio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioRecebedor { get; set; }

        public virtual bool Equals(Chat other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
