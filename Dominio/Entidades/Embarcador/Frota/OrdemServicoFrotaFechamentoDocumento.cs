namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_ORDEM_SERVICO_FECHAMENTO_DOCUMENTO", EntityName = "OrdemServicoFrotaFechamentoDocumento", Name = "Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento", NameType = typeof(OrdemServicoFrotaFechamentoDocumento))]
    public class OrdemServicoFrotaFechamentoDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OFD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrota OrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.DocumentoEntradaTMS DocumentoEntrada { get; set; }
    }
}
