using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAT_MOBILE_MENSAGEM", EntityName = "ChatMobileMensagem", Name = " Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem", NameType = typeof(ChatMobileMensagem))]
    public class ChatMobileMensagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMM")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "CMM_MENSAGEM", TypeType = typeof(string), Length = 750, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMM_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Remetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMD_DATA_CONFIRMACAO_LEITURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConfirmacaoLeitura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemLida", Column = "CMM_LIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MensagemLida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemFalhaIntegracao", Column = "CMM_FALHA_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MensagemFalhaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }

    }
}
