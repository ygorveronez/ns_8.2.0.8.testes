namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO_TIPO_DE_CARGA", EntityName = "PeriodoDescarregamentoTipoDeCarga", Name = "Dominio.Entidades.Embarcador.Logistica", NameType = typeof(PeriodoDescarregamentoTipoDeCarga))]
    public class PeriodoDescarregamentoTipoDeCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PeriodoDescarregamento", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento PeriodoDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }
        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}