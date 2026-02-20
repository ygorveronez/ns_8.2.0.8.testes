namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_TIPO_OPERACAO_TIPOS_OPERACAO", EntityName = "RegraTipoOperacaoTiposOperacao", Name = "Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoTiposOperacao", NameType = typeof(RegraTipoOperacaoTiposOperacao))]
    public class RegraTipoOperacaoTiposOperacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraTipoOperacao", Column = "RTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraTipoOperacao RegraTipoOperacao { get; set; }
    }
}   