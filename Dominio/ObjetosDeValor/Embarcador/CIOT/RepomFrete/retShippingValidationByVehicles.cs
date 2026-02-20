using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete
{
    public class retShippingValidationByVehicles : retPadrao
    {
        public retShippingValidationByVehiclesResult Result { get; set; }
    }

    public class retShippingValidationByVehiclesResult
    {
        public string Message { get; set; }

        public List<retShippingValidationByVehiclesVehicle> Vehicles { get; set; }
    }

    public class retShippingValidationByVehiclesVehicle
    {
        public string LicensePlate { get; set; }

        public bool Validate { get; set; }

        public string MessageError { get; set; }
    }
}