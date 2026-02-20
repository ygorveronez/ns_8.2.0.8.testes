namespace Dominio.Entidades.Embarcador.Fechamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_FRETE_VALORES_OUTROS_RECURSOS", EntityName = "FechamentoFreteValoresOutrosRecursos", Name = "Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos", NameType = typeof(FechamentoFreteValoresOutrosRecursos))]
    public class FechamentoFreteValoresOutrosRecursos : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FVO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoFrete Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "FVO_QUANTIDADE", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportadorValoresOutrosRecursos", Column = "CFR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frete.ContratoFreteTransportadorValoresOutrosRecursos ValoresOutrosRecursos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }
    }
}
