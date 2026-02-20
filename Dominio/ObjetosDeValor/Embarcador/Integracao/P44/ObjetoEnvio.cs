using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.P44
{
    public class ObjetoEnvio
    {
        public string id { get; set; }
        public TypeValue carrierIdentifier { get; set; }
        public List<TypeValue> equipmentIdentifiers { get; set; }
        public List<TypeValue> shipmentIdentifiers { get; set; }
        public List<NameValuePredefined> attributes { get; set; }
        public List<ShipmentStop> shipmentStops { get; set; }
    }
}
