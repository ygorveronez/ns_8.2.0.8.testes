namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_NOTA_FISCAL_ITEM_DEVOLUCAO", EntityName = "CargaEntregaNotaFiscalItemDevolucao", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao", NameType = typeof(CargaEntregaNotaFiscalItemDevolucao))]
    public class CargaEntregaNotaFiscalItemDevolucao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntregaNotaFiscal", Column = "CEF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal CargaEntregaNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDevolucao", Column = "CFD_QUANTIDADE_DEVOLUCAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDevolucao", Column = "CFD_VALOR_DEVOLUCAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NFDevolucao", Column = "CFD_NF_DEVOLUCAO", TypeType = typeof(int), NotNull = false)]
        public virtual int NFDevolucao { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}