using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class ViagemV3Features
    {
        public bool? canSearchDocument { get; set; }
        public OccurrenceFeature occurrence { get; set; }
        public LoadDetail? loadDetail { get; set; }
        public PartialDeliveryFeature? partialDelivery { get; set; }
        public PartialDeliveryFeature? notDelivered { get; set; }
        public TravelInitializationActionFeature? travelInitializationAction { get; set; }
        public string? documentItemValidator { get; set; }
    }

    public class OccurrenceFeature
    {
        public bool? enabled { get; set; }
        public List<OccurrenceFeatureCategory> categories { get; set; }
    }

    public class OccurrenceFeatureCategory
    {
        public string category { get; set; }
    }

    public class LoadDetail
    {
        public bool enabled { get; set; }
        public LoadDetailReceipt? receipt { get; set; }
        public string layout { get; set; }
    }

    public class LoadDetailReceipt
    {
        public bool enabled { get; set; }
        public TextoInternacionalizado? titleReceipt { get; set; }
        public TextoInternacionalizado? titleReport { get; set; }
        public List<InformacaoAdicional>? additionalInfo { get; set; }
    }

    public class PartialDeliveryFeature
    {
        public bool enabled { get; set; }
        public List<Reason> reasons { get; set; }
    }

    public class Reason
    {
        public string reason { get; set; }
        public List<ChecklistStep>? template { get; set; }
    }

    public class TravelInitializationActionFeature
    {
        public int point { get; set; }
        public string type { get; set; }
    }
}