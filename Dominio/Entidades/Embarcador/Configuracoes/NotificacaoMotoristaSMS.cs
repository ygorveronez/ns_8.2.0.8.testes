namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTIFICACAO_MOTORISTA_SMS", EntityName = "NotificacaoMotoristaSMS", Name = "Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS", NameType = typeof(NotificacaoMotoristaSMS))]
    public class NotificacaoMotoristaSMS : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NOT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NOT_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NOT_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NOT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NOT_TIPO_ENVIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacaoMotoristaSMS), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacaoMotoristaSMS TipoEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NOT_NOTIFICACAO_SUPER_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificacaoSuperApp { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}
