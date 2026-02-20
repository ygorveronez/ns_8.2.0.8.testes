using System;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class BookingContainerDetails
    {
        [DataMember(Order = 1, IsRequired = true)]
        public DateTime EmptyGate { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public string TerminalName { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public DateTime ContainerAtCY { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public string ContainerNumber { get; set; }

        [DataMember(Order = 5, IsRequired = true)]
        public string SealNumber { get; set; }

        [DataMember(Order = 6, IsRequired = true)]
        public DateTime EmptyGateInDestination { get; set; }
    }
}
