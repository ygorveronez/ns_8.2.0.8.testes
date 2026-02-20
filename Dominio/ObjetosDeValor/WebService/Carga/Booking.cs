using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class Booking
    {
        [DataMember(Order = 1, IsRequired = true)]
        public string ProtocolReferenceIntegration { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public string ClientReference { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string Reference { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public int ModeOfTransport { get; set; }

        [DataMember(Order = 5, IsRequired = true)]
        public int TypeOfTransport { get; set; }

        [DataMember(Order = 6, IsRequired = true)]
        public BookingStatus Status { get; set; }

        [DataMember(Order = 7, IsRequired = true)]
        public BookingPortage Portage { get; set; }

        [DataMember(Order = 8, IsRequired = true)]
        public BookingContainer Container { get; set; }
    }
}
