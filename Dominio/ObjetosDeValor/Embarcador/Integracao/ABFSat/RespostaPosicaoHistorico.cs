using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ABFSat
{
    public class RespostaPosicaoHistorico
    {
        [JsonPropertyName("ListTrailer")]
        public List<object> ListTrailer { get; set; }

        [JsonPropertyName("Plate")]
        public string Plate { get; set; }

        [JsonPropertyName("DriverIdentification")]
        public object DriverIdentification { get; set; }

        [JsonPropertyName("DocumentNumber")]
        public string DocumentNumber { get; set; }

        [JsonPropertyName("IdPosition")]
        public long IdPosition { get; set; }

        [JsonPropertyName("IdTrackedUnitType")]
        public int IdTrackedUnitType { get; set; }

        [JsonPropertyName("TrackedUnitIntegrationCode")]
        public string TrackedUnitIntegrationCode { get; set; }

        [JsonPropertyName("TrackerSlot")]
        public int TrackerSlot { get; set; }

        [JsonPropertyName("IdTrackedUnit")]
        public int IdTrackedUnit { get; set; }

        [JsonPropertyName("TrackedUnit")]
        public string TrackedUnit { get; set; }

        [JsonPropertyName("IdEvent")]
        public int IdEvent { get; set; }

        [JsonPropertyName("IdMainBatteryMeasureUnit")]
        public int IdMainBatteryMeasureUnit { get; set; }

        [JsonPropertyName("IdBackupBatteryMeasureUnit")]
        public int IdBackupBatteryMeasureUnit { get; set; }

        [JsonPropertyName("Ignition")]
        public bool Ignition { get; set; }

        [JsonPropertyName("Available")]
        public object Available { get; set; }

        [JsonPropertyName("ValidGPS")]
        public object ValidGPS { get; set; }

        [JsonPropertyName("EventDate")]
        public string EventDate { get; set; }

        [JsonPropertyName("UpdateDate")]
        public string UpdateDate { get; set; }

        [JsonPropertyName("Driver")]
        public string Driver { get; set; }

        [JsonPropertyName("Latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("Longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("Address")]
        public string Address { get; set; }

        [JsonPropertyName("DistanceFromGeographicArea")]
        public string DistanceFromGeographicArea { get; set; }

        [JsonPropertyName("ListInputSensor")]
        public Dictionary<string, bool> ListInputSensor { get; set; }

        [JsonPropertyName("ListOutputActuator")]
        public Dictionary<string, bool> ListOutputActuator { get; set; }

        [JsonPropertyName("ListTelemetry")]
        public Dictionary<string, double> ListTelemetry { get; set; }

        [JsonPropertyName("PersonIntegrationCodeCenter")]
        public string PersonIntegrationCodeCenter { get; set; }

        [JsonPropertyName("PersonIntegrationCodeClient")]
        public string PersonIntegrationCodeClient { get; set; }
    }
}
