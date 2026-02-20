namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_TIPO_OPERACAO_CANAL_ENTREGA", EntityName = "RegraTipoOperacaoCanalEntrega", Name = "Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalEntrega", NameType = typeof(RegraTipoOperacaoCanalEntrega))]
    public class RegraTipoOperacaoCanalEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraTipoOperacao", Column = "RTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraTipoOperacao RegraTipoOperacao { get; set; }
    }
}