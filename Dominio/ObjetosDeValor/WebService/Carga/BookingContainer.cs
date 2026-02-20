using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class BookingContainer
    {
        [DataMember(Order = 1, IsRequired = true)]
        public BookingContainerDetails ContainerDetails { get; set; }
    }
}
