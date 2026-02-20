namespace Dominio.Entidades.Embarcador.Fechamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_FRETE_MOTORISTA_UTILIZADO", EntityName = "FechamentoFreteMotoristaUtilizado", Name = "Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado", NameType = typeof(FechamentoFreteMotoristaUtilizado))]
    public class FechamentoFreteMotoristaUtilizado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FVU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoFrete", Column = "FEF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoFrete Fechamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "FVU_QUANTIDADE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "FVU_VALOR", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "FVU_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotal { get; set; }
    }
}
