namespace Dominio.Entidades.Embarcador.Fechamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_FRETE_ACRESCIMO_DESCONTO", EntityName = "FechamentoFreteAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto", NameType = typeof(FechamentoFreteAcrescimoDesconto))]
    public class FechamentoFreteAcrescimoDesconto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoFrete Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoJustificativaAcrescimoDesconto", Column = "FAD_CODIGO_JUSTIFICATIVA_ACRESCIMO_DESCONTO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoJustificativaAcrescimoDesconto Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "FAD_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }
    }
}
