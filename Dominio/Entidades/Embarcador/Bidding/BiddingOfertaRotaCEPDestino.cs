namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_OFERTA_ROTA_CEP_DESTINO", EntityName = "BiddingOfertaRotaCEPDestino", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPDestino", NameType = typeof(BiddingOfertaRotaCEPDestino))]
    public class BiddingOfertaRotaCEPDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingOfertaRota", Column = "TBR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BiddingOfertaRota BiddingOfertaRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCD_CEP_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int CEPInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCD_CEP_FINAL", TypeType = typeof(int), NotNull = true)]
        public virtual int CEPFinal { get; set; }

        public virtual string Descricao
        {
            get
            {
                return string.Format(@"{0:00\.000\-000}", CEPInicial) + " à " + string.Format(@"{0:00\.000\-000}", CEPFinal);
            }
        }
    }
}
