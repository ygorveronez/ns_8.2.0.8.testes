using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class BookingPortageCargoDetails
    {
        [DataMember(Order = 1, IsRequired = true)]
        public int TotalNumberOfPackages { get; set; }
    }
}
