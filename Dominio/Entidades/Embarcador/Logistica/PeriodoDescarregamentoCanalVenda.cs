namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO_CANAL_VENDA", EntityName = "PeriodoDescarregamentoCanalVenda", Name = "Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda", NameType = typeof(PeriodoDescarregamentoCanalVenda))]
    public class PeriodoDescarregamentoCanalVenda : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PeriodoDescarregamento", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PeriodoDescarregamento PeriodoDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalVenda", Column = "CNV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.CanalVenda CanalVenda { get; set; }
    }
}
