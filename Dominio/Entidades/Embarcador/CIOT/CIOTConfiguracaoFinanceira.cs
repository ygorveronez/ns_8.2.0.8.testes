namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_CONFIGURACAO_FINANCEIRA", EntityName = "CIOTConfiguracaoFinanceira", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira", NameType = typeof(CIOTConfiguracaoFinanceira))]

    public class CIOTConfiguracaoFinanceira : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "CEF_CODIGO_TIPO_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoParaUso { get; set; }
    
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimentoParaReversao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

    }
}