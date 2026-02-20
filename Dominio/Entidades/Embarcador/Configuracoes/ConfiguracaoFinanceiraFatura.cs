namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FINANCEIRA_FATURA", EntityName = "ConfiguracaoFinanceiraFatura", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura", NameType = typeof(ConfiguracaoFinanceiraFatura))]
    public class ConfiguracaoFinanceiraFatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFF_GERAR_MOVIMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarMovimentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_USO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoUso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFF_GERACAO_MOVIMENTO_FINANCEIRO_POR_MODELO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeracaoMovimentoFinanceiroPorModeloDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFF_GERACAO_MOVIMENTO_FINANCEIRO_POR_MODELO_DOCUMENTO_REVERSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeracaoMovimentoFinanceiroPorModeloDocumentoReversao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Financeira Fatura";
            }
        }
    }
}
