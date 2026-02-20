using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Autotrac
{
    public class ExpandedAlert
    {
        public long ID { get; set; }
        public int AccountNumber { get; set; }
        public int VehicleCode { get; set; }
        public string VehicleAddress { get; set; }
        public int Priority { get; set; }
        public int GRMN { get; set; }
        public int Ignition { get; set; }
        public int BinaryDatatype { get; set; }
        public string MessageTime { get; set; }
        public DateTime MessageTimeDT { get { return DateTime.Parse(MessageTime); } }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PositionTime { get; set; }
        public DateTime PositionTimeDT { get { return DateTime.Parse(PositionTime); } }
        public string Landmark { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Temperature1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Temperature2 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? RPM { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? Speed { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? Hourmeter { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? Hodometer { get; set; }
    }

}
