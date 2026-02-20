using System;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class BookingPortageRoutingInformation
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int RoutingCode { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public int VoyageNo { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string VesselName { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public string PortOfoadingLocation { get; set; }

        [DataMember(Order = 5, IsRequired = true)]
        public DateTime PortOfLoading { get; set; }

        [DataMember(Order = 6, IsRequired = true)]
        public string PortOfDischargLocation { get; set; }

        [DataMember(Order = 7, IsRequired = true)]
        public DateTime PortOfDischarge { get; set; }
    }
}
