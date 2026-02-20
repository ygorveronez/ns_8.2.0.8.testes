using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FINANCEIRA_CONTRATO_FRETE_TERCEIROS", EntityName = "ConfiguracaoFinanceiraContratoFreteTerceiros", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros", NameType = typeof(ConfiguracaoFinanceiraContratoFreteTerceiros))]
    public class ConfiguracaoFinanceiraContratoFreteTerceiros : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_GERAR_MOVIMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarMovimentoAutomaticoNaGeracaoContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_PAGO_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorPagoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_PAGO_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorPagoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_VALOR_PAGO_TERCEIRO_CIOT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoValorPagoTerceiroCIOT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO_VALOR_PAGO_TERCEIRO_CIOT", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoReversaoValorPagoTerceiroCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_GERAR_MOVIMENTO_AUTOMATICO_POR_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoAutomaticoPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ConfiguracoesTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_FINANCEIRA_CONTRATO_FRETE_TERCEIROS_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao", Column = "CTO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao> ConfiguracoesTipoOperacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Financeira Contrato de Frete Terceiros";
            }
        }
    }
}
