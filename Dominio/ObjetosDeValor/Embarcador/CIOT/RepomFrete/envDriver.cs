using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class envDriver
    {
        public string Country { get; set; }
        public string NationalId { get; set; }
        public string DriverLicenseNumber { get; set; }
        public Address Address { get; set; }
        public List<Phones> Phones { get; set; }
        public DriverPersonalInformation DriverPersonalInformation { get; set; }
    }

    public class DriverPersonalInformation
    {
        public string BirthDate { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
    }
}