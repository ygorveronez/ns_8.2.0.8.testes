namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_CONTABILIZACAO_MOVIMENTO", EntityName = "LoteContabilizacaoMovimento", Name = "Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoMovimento", NameType = typeof(LoteContabilizacaoMovimento))]
    public class LoteContabilizacaoMovimento: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LCM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteContabilizacao", Column = "LCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LoteContabilizacao LoteContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoExportacaoContabil", Column = "DEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoExportacaoContabil DocumentoExportacaoContabil { get; set; }
    }
}
