namespace Dominio.Entidades.Embarcador.Pallets.Avaria
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_AVARIA_QUANTIDADE", EntityName = "AvariaPalletQuantidade", Name = "Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPalletQuantidade", NameType = typeof(AvariaPalletQuantidade))]
    public class AvariaPalletQuantidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PAQ_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AvariaPallet", Column = "PAV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AvariaPallet AvariaPallet { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SituacaoDevolucaoPallet", Column = "PSD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SituacaoDevolucaoPallet SituacaoDevolucaoPallet { get; set; }

        public virtual bool Equals(AvariaPalletQuantidade other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
