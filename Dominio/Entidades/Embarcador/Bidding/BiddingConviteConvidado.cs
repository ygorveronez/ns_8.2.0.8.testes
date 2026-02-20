using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Bidding
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BIDDING_CONVITE_CONVIDADO", EntityName = "BiddingConviteConvidado", Name = "Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado", NameType = typeof(BiddingConviteConvidado))]
    public class BiddingConviteConvidado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BiddingConvite", Column = "TBC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BiddingConvite BiddingConvite { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "CON_CODIGO", NotNull = true)]
        public virtual Empresa Convidado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BCC_STATUS", TypeType = typeof(StatusBiddingConviteConvidado), NotNull = true)]
        public virtual StatusBiddingConviteConvidado Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BCC_STATUS_BIDDING", TypeType = typeof(StatusBiddingConvite), NotNull = true)]
        public virtual StatusBiddingConvite StatusBidding { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BCC_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "BCC_EMAIL_AVISO_CONVIDADO_ENVIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailAvisoConvidadoEnviado { get; set; }
    }
}
