namespace Dominio.Entidades.Embarcador.Pallets
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_DEVOLUCAO_SITUACAO", EntityName = "DevolucaoPalletSituacao", Name = "Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletSituacao", NameType = typeof(DevolucaoPalletSituacao))]
    public class DevolucaoPalletSituacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DevolucaoPallet", Column = "PDE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DevolucaoPallet Devolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SituacaoDevolucaoPallet", Column = "PSD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SituacaoDevolucaoPallet Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PDS_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "PSD_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "PSD_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcresceSaldo", Column = "PSD_ACRESCE_SALDO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AcresceSaldo { get; set; }
    }
}
