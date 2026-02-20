using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class retShippingValidationByVehiclesTag : retPadrao
    {
        public string Message { get; set; }

        public List<retShippingValidationByVehiclesTagVehicle> Vehicles { get; set; }
    }

    public class retShippingValidationByVehiclesTagVehicle
    {
        public string LicensePlate { get; set; }

        public bool HasTag { get; set; }

        public string StatusTag { get; set; }

        public string MessageError { get; set; }
    }
}