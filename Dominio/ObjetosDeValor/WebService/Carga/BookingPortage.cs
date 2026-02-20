using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class BookingPortage
    {
        [DataMember(Order = 1, IsRequired = true)]
        public BookingPortageFreight Freight { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public BookingPortageRoutingInformation RoutingInformation { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public BookingPortageCargoDetails CargoDetails { get; set; }
    }
}
