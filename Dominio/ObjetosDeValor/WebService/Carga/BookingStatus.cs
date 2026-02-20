using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class BookingStatus
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int ShipmentStatus { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public BookingStatusMaritme StatusMaritme { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public BookingStatusAir StatusAir { get; set; }
    }
}
