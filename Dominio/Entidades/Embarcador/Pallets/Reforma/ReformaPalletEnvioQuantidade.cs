namespace Dominio.Entidades.Embarcador.Pallets.Reforma
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PALLET_REFORMA_ENVIO_QUANTIDADE", EntityName = "ReformaPalletEnvioQuantidade", Name = "Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvioQuantidade", NameType = typeof(ReformaPalletEnvioQuantidade))]
    public class ReformaPalletEnvioQuantidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PRQ_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ReformaPalletEnvio", Column = "PRE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ReformaPalletEnvio Envio{ get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SituacaoDevolucaoPallet", Column = "PSD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SituacaoDevolucaoPallet SituacaoDevolucaoPallet { get; set; }

        public virtual bool Equals(ReformaPalletEnvioQuantidade other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
