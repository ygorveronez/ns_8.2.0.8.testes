namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_NOTIFICACAO_APP", DynamicUpdate = true, EntityName = "TipoOperacaoNotificacaoApp", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp", NameType = typeof(TipoOperacaoNotificacaoApp))]
    public class TipoOperacaoNotificacaoApp : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TON_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotificacaoApp", Column = "NOT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp NotificacaoApp { get; set; }

    }
}
