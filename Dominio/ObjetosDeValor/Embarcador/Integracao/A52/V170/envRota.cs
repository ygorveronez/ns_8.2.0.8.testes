using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170
{
    public class envRota
    {
        public envRotaGeoJson geoJson { get; set; }

        public string nome { get; set; }

        public string identificador { get; set; }

        public bool ativo { get; set; }

        public string polilinha { get; set; }
    }

    public class envRotaGeoJson
    {
        public string type { get; set; }

        public envRotaGeoJsonDetails details { get; set; }

        public envRotaGeoJsonGeometry geometry { get; set; }

        public envRotaGeoJsonProperties properties { get; set; }
    }

    public class envRotaGeoJsonDetails
    {
        public int? totalTime { get; set; }

        public int? totalDistance { get; set; }

        public envRotaGeoJsonDetailsInputWaypoints inputWaypoints { get; set; }
    }

    public class envRotaGeoJsonDetailsInputWaypoints
    {
        public List<decimal> latLng { get; set; }
    }

    public class envRotaGeoJsonGeometry
    {
        public string type { get; set; }

        public List<envRotaGeoJsonGeometryCoordinates> coordinates { get; set; }
    }

    public class envRotaGeoJsonGeometryCoordinates
    {
    }

    public class envRotaGeoJsonProperties
    {
        public string name { get; set; }

        public string time { get; set; }
    }
}