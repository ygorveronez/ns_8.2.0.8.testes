namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FINANCEIRA_CONTRATO_FRETE_TERCEIROS_TIPO_OPERACAO", EntityName = "ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao", NameType = typeof(ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao))]
    public class ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoFinanceiraContratoFreteTerceiros", Column = "CCF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros ConfiguracaoFinanceiraContratoFreteTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_TITULO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoGeracaoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_TITULO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoGeracaoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_PAGAMENTO_VIA_CIOT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoPagamentoViaCIOT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_PAGAMENTO_VIA_CIOT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoPagamentoViaCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_INSS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorINSS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_INSS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorINSS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_INSS", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_INSS_PATRONAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorINSSPatronal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_INSS_PATRONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorINSSPatronal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_INSS_PATRONAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorINSSPatronal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_IRRF", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorIRRF { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_IRRF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorIRRF { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_IRRF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_SEST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorSEST { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_SEST", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorSEST { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_SEST", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_SENAT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorSENAT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_SENAT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorSENAT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_SENAT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_VALOR_TARIFA_SAQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorTarifaSaque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_TARIFA_SAQUE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorTarifaSaque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_TARIFA_SAQUE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorTarifaSaque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_VALOR_TARIFA_TRANSFERENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorTarifaTransferencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_TARIFA_TRANSFERENCIA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorTarifaTransferencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_TARIFA_TRANSFERENCIA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorTarifaTransferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_VALOR_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorLiquido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_LIQUIDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorLiquido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_LIQUIDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_VALOR_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_ABASTECIMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_ABASTECIMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_VALOR_ADIANTAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_ADIANTAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_ADIANTAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_VALOR_SALDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorSaldo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_SALDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorSaldo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_SALDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTO_DIFERENCIAR_MOVIMENTO_VALOR_TOTAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DiferenciarMovimentoValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_TOTAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_TOTAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorTotal { get; set; }
    }
}
