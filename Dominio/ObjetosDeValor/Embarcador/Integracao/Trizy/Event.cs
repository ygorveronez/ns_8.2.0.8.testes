using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Event
    {
        public string flow { get; set; }
        public bool? hasDeliveryReceipt { get; set; }
        public List<EventItem> items { get; set; }
    }

    public class EventItem
    {
        public string type { get; set; }
        public string? title { get; set; }
        public string? status { get; set; }
        public string? expectedAt { get; set; }
        public Tolerance? tolerance { get; set; }
        public Geofence? geofence { get; set; }
        public bool? required { get; set; }
        public string? externalId { get; set; }
        public int? order { get; set; }
        public string? pause { get; set; }
        public string? checklist { get; set; }
        public List<ChecklistStep>? template { get; set; }
    }
    public class Tolerance
    {
        public string before { get; set; }
        public string after { get; set; }
    }
    public class Geofence
    {
        public Radius radius { get; set; }
        public Location? location { get; set; }

    }
    public class Radius
    {
        public int value { get; set; }
        public string unit { get; set; }
    }
}
