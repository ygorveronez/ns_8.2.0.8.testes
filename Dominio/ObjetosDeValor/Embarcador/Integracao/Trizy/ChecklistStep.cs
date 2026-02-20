using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class ChecklistStep
    {
        public string type { get; set; }
        public bool? required { get; set; }
        public TextoInternacionalizado label { get; set; }
        public TextoInternacionalizado? helperText { get; set; }
        public ChecklistStepAlert? alert { get; set; }
        public TextoInternacionalizado? placeholder { get; set; }
        public int? minimumFractionDigits { get; set; }
        public int? maximumFractionDigits { get; set; }
        public ChecklistStepMinMax? range { get; set; }
        public ChecklistStepMinMax? limit { get; set; }
        public bool? galleryEnabled { get; set; }
        public bool? canPause { get; set; }
        public int? waitInSeconds { get; set; }
        public string? mode { get; set; }
        public ChecklistStepValidator[]? validator {  get; set; }
        public ChecklistStepImageProcessing? imageProcessing { get; set; }
        public ChecklistStepProportion? proportion { get; set; }
        public ChecklistStepMinMax? validationLimit { get; set; }
        public InformacaoExterna externalInfo { get; set; }
        public List<ChecklistGeofence> geofences { get; set; }
        public ChecklistOutsideAreaFeatures? outsideAreaFeatures { get; set; }
        public ChecklistValidatedMetadataSetting? validatedMetadataSetting { get; set; }
        public bool? useCurrentTimeAsDefault { get; set; }
        public string? step { get; set; }
        public List<LoadDetailIndicator>? indicators { get; set; }
        public List<LoadDetailInputs>? inputs { get; set; }
        public string? validation { get; set; }

    }
}