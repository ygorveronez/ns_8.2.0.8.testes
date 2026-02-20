namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_XML_NOTA_FISCAL", EntityName = "GestaoDevolucaoNotaFiscalOrigem", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem", NameType = typeof(GestaoDevolucaoNotaFiscalOrigem))]

    public class GestaoDevolucaoNotaFiscalOrigem : EntidadeBase
    {
        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "GNF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }
        #endregion

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucao", Column = "GDV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDevolucao GestaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControleFinalizacaoDevolucao", Column = "GNF_CONTROLE_FINALIZACAO_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControleFinalizacaoDevolucao { get; set; }
    }
}
