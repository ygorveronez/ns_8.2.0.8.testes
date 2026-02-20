namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_EVENTO_SUPER_APP", DynamicUpdate = true, EntityName = "TipoOperacaoEventoSuperApp", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp", NameType = typeof(TipoOperacaoEventoSuperApp))]
    public class TipoOperacaoEventoSuperApp : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EventoSuperApp", Column = "ESA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp EventoSuperApp { get; set; }

    }
}
