namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_MOVIMENTO_TIPO_DESPESA", EntityName = "TipoMovimentoTipoDespesa", Name = "Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa", NameType = typeof(TipoMovimentoTipoDespesa))]
    public class TipoMovimentoTipoDespesa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDespesaFinanceira", Column = "TID_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Despesa.TipoDespesaFinanceira TipoDespesaFinanceira { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimento { get; set; }

        public virtual string Descricao
        {
            get { return TipoDespesaFinanceira?.Descricao ?? string.Empty; }
        }
    }
}
