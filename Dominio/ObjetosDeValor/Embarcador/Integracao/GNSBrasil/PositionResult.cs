using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GNSBrasil
{
    public class PositionResult
    {
        public int IdPosition { get; set; }
        public int IdTrackedUnitType { get; set; }
        public string TrackedUnitIntegrationCode { get; set; }
        public int TrackerSlot { get; set; }
        public int IdTrackedUnit { get; set; }
        public string TrackedUnit { get; set; }
        public int IdEvent { get; set; }
        public int IdMainBatteryMeasureUnit { get; set; }
        public int IdBackupBatteryMeasureUnit { get; set; }
        public bool? Ignition { get; set; }
        public bool? Available { get; set; }
        public bool? ValidGPS { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Driver { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; }
        public string DistanceFromGeographicArea { get; set; }
    }
}
