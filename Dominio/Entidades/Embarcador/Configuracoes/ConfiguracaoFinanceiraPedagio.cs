namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FINANCEIRA_PEDAGIO", EntityName = "ConfiguracaoFinanceiraPedagio", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio", NameType = typeof(ConfiguracaoFinanceiraPedagio))]
    public class ConfiguracaoFinanceiraPedagio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMovimentoAutomaticoNoLancamentoPedagio", Column = "CCF_GERAR_MOVIMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarMovimentoAutomaticoNoLancamentoPedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_LANCAMENTO_PEDAGIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoLancamentoPedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_LANCAMENTO_PEDAGIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoLancamentoPedagio { get; set; }
        public virtual string Descricao
        {
            get
            {
                return "Configuração Financeira Pedágios";
            }
        }
    }
}
