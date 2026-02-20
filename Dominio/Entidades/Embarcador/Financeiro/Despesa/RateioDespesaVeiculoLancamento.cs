namespace Dominio.Entidades.Embarcador.Financeiro.Despesa
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RATEIO_DESPESA_VEICULO_LANCAMENTO", EntityName = "RateioDespesaVeiculoLancamento", Name = "Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento", NameType = typeof(RateioDespesaVeiculoLancamento))]
    public class RateioDespesaVeiculoLancamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "TRL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioDespesaVeiculo", Column = "TRD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RateioDespesaVeiculo RateioDespesa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRL_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        /// <summary>
        /// Valor faturado pelo veículo no período informado no rateio da despesa
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TRL_VALOR_FATURAMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorFaturamento { get; set; }

        /// <summary>
        /// Percentual do faturamento do veículo sobre o faturamento total no período informado no rateio da despesa
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TRL_PERCENTUAL_SOBRE_FATURAMENTO_TOTAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PercentualSobreFaturamentoTotal { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
