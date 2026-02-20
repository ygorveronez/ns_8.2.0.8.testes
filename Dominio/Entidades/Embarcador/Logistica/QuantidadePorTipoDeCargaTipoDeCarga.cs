namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_QUANTIDADE_POR_TIPO_DE_CARGA_TIPO_DE_CARGA", EntityName = "QuantidadePorTipoDeCargaTipoDeCarga", Name = "Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga", NameType = typeof(QuantidadePorTipoDeCargaTipoDeCarga))]
    public class QuantidadePorTipoDeCargaTipoDeCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "QTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoCarga { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "QuantidadePorTipoDeCargaDescarregamento", Column = "QPT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual QuantidadePorTipoDeCargaDescarregamento QuantidadePorTipoDeCargaDescarregamento { get; set; }
    }
}
