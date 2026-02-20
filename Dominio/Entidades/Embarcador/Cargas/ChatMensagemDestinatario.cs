using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAT_MENSAGEM_DESTINATARIO", EntityName = "ChatMensagemDestinatario", Name = "Dominio.Entidades.Embarcador.Cargas.ChatMensagemDestinatario", NameType = typeof(ChatMensagemDestinatario))]
    public class ChatMensagemDestinatario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChatMobileMensagem", Column = "CMM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem ChatMobileMensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRecebida", Column = "CMD_MENSAGEM_RECEBIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MensagemRecebida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMD_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Destinatario { get; set; }
    }
}
