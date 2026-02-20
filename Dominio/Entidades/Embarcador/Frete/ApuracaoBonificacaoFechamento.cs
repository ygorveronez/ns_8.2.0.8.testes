namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APURACAO_BONIFICACAO_FECHAMENTO", EntityName = "ApuracaoBonificacaoFechamento", Name = "Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento", NameType = typeof(ApuracaoBonificacaoFechamento))]
    public class ApuracaoBonificacaoFechamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ABF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFaturamento", Column = "ABF_VALOR_FATURAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualApuracao", Column = "ABF_PERCENTUAL_APURACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualApuracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOcorrencia", Column = "ABF_VALOR_OCORRENCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia Ocorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CargaMaiorValor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "ApuracaoBonificacao", Column = "APB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao ApuracaoBonificacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "BonificacaoTransportador", Column = "BNT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador RegraApuracao { get; set; }
    }
}