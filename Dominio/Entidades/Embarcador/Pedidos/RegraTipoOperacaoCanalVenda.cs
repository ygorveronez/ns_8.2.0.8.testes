namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_TIPO_OPERACAO_CANAL_VENDA", EntityName = "RegraTipoOperacaoCanalVenda", Name = "Dominio.Entidades.Embarcador.Pedidos.RegraTipoOperacaoCanalVenda", NameType = typeof(RegraTipoOperacaoCanalVenda))]
    public class RegraTipoOperacaoCanalVenda : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalVenda", Column = "CNV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Pedidos.CanalVenda CanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraTipoOperacao", Column = "RTO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraTipoOperacao RegraTipoOperacao { get; set; }
    }
}