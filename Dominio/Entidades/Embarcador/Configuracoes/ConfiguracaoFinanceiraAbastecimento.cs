namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FINANCEIRA_ABASTECIMENTO", EntityName = "ConfiguracaoFinanceiraAbastecimento", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento", NameType = typeof(ConfiguracaoFinanceiraAbastecimento))]
    public class ConfiguracaoFinanceiraAbastecimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMovimentoAutomaticoNoLancamentoAbastecimento", Column = "CCF_GERAR_MOVIMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarMovimentoAutomaticoNoLancamentoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_LANCAMENTO_ABASTECIMENTO_POSTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoLancamentoAbastecimentoPosto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_LANCAMENTO_ABASTECIMENTO_POSTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoLancamentoAbastecimentoPosto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_LANCAMENTO_ABASTECIMENTO_BOMBA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoLancamentoAbastecimentoBombaPropria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_LANCAMENTO_ABASTECIMENTO_BOMBA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoLancamentoAbastecimentoBombaPropria { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Financeira de Abastecimento";
            }
        }
    }
}
