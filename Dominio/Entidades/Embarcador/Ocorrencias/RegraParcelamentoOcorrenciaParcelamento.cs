namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PARCELAMENTO_OCORRENCIA_PARCELAMENTO", EntityName = "RegraParcelamentoOcorrenciaParcelamento", Name = "Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento", NameType = typeof(RegraParcelamentoOcorrenciaParcelamento))]
    public class RegraParcelamentoOcorrenciaParcelamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraParcelamentoOcorrencia", Column = "RPO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraParcelamentoOcorrencia RegraParcelamentoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroParcelas", Column = "RPP_NUMERO_PARCELAS", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroParcelas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualInicial", Column = "RPP_PERCENTUAL_INICIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualFinal", Column = "RPP_PERCENTUAL_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualJurosParcela", Column = "RPP_PERCENTUAL_JUROS_PARCELA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualJurosParcela { get; set; }

        public virtual string Descricao
        {
            get { return $"De {this.PercentualInicial.ToString("n2")} a {this.PercentualFinal.ToString("n2")} por cento"; }
        }
    }
}
