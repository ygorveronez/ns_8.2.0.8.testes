using System;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class BookingPortageFreight
    {
        [DataMember(Order = 1, IsRequired = true)]
        public DateTime BookingDate { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public int BookingNumber { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string Despachante { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public string PortOfLoading { get; set; }

        [DataMember(Order = 5, IsRequired = true)]
        public DateTime PortOfLoadingETS { get; set; }

        [DataMember(Order = 6, IsRequired = true)]
        public DateTime PortOfDischargeETA { get; set; }

        [DataMember(Order = 7, IsRequired = true)]
        public DateTime RevisionOfDeparture { get; set; }

        [DataMember(Order = 8, IsRequired = true)]
        public DateTime Departure { get; set; }

        [DataMember(Order = 9, IsRequired = true)]
        public DateTime RevisionArrival { get; set; }

        [DataMember(Order = 10, IsRequired = true)]
        public DateTime FinalArrival { get; set; }

        [DataMember(Order = 11, IsRequired = true)]
        public DateTime GateOutDestination { get; set; }

        [DataMember(Order = 12, IsRequired = true)]
        public string ShippingLineSCACCode { get; set; }

        [DataMember(Order = 13, IsRequired = true)]
        public string OceanBLNo { get; set; }

        [DataMember(Order = 14, IsRequired = true)]
        public string VoyageNo { get; set; }

        [DataMember(Order = 15, IsRequired = true)]
        public string VesselName { get; set; }

        [DataMember(Order = 16, IsRequired = true)]
        public int ShipmentMovement { get; set; }

        [DataMember(Order = 17, IsRequired = true)]
        public DateTime CargoDeadline { get; set; }

        [DataMember(Order = 18, IsRequired = true)]
        public DateTime DraftDeadline { get; set; }
    }
}
