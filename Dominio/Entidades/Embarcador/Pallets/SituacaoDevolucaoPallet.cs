namespace Dominio.Entidades.Embarcador.Pallets
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_SITUACAO_DEVOLUCAO", EntityName = "SituacaoDevolucaoPallet", Name = "Dominio.Entidades.Embarcador.Pallets.SituacaoDevolucaoPallet", NameType = typeof(SituacaoDevolucaoPallet))]
    public class SituacaoDevolucaoPallet : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PSD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PSD_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "PSD_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcresceSaldo", Column = "PSD_ACRESCE_SALDO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AcresceSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPalletAvariado", Column = "PSD_SITUACAO_PALLET_AVARIADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SituacaoPalletAvariado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPalletDescartado", Column = "PSD_SITUACAO_PALLET_DESCARTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SituacaoPalletDescartado{ get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PSD_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
