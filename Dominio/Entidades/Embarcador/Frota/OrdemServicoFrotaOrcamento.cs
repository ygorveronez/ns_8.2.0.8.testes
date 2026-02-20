namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_ORDEM_SERVICO_ORCAMENTO", EntityName = "OrdemServicoFrotaOrcamento", Name = "Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento", NameType = typeof(OrdemServicoFrotaOrcamento))]
    public class OrdemServicoFrotaOrcamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OSO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", Unique = true, NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrota OrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalProdutos", Column = "OSO_VALOR_TOTAL_PRODUTOS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotalProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMaoObra", Column = "OSO_VALOR_TOTAL_MAO_OBRA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotalMaoObra { get; set; }

        /// <summary>
        /// Somatório dos campos ValorTotalProdutos e ValorTotalMaoObra.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalOrcado", Column = "OSO_VALOR_TOTAL_ORCADO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotalOrcado { get; set; }

        /// <summary>
        /// Somatório do custo estimado dos serviços da OS.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalPreAprovado", Column = "OSO_VALOR_TOTAL_PRE_APROVADO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorTotalPreAprovado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Parcelas", Column = "OSO_PARCELAS", TypeType = typeof(int), NotNull = true)]
        public virtual int Parcelas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "OSO_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

    }
}
