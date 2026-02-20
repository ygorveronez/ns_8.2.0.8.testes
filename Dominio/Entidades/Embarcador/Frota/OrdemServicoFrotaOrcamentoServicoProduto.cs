namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_ORDEM_SERVICO_ORCAMENTO_SERVICO_PRODUTO", EntityName = "OrdemServicoFrotaOrcamentoServicoProduto", Name = "Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto", NameType = typeof(OrdemServicoFrotaOrcamentoServicoProduto))]
    public class OrdemServicoFrotaOrcamentoServicoProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OSP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaOrcamentoServico", Column = "OOS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrotaOrcamentoServico OrcamentoServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "OSP_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "OSP_QUANTIDADE", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Garantia", Column = "OSP_GARANTIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Garantia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Autorizado", Column = "OSP_AUTORIZADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Autorizado { get; set; }

        public virtual decimal ValorTotal
        {
            get { return Quantidade * Valor; }
        }
    }
}
