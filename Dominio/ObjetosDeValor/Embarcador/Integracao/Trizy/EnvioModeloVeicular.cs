using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class EnvioModeloVeicular
    {
        public TextoInternacionalizado? description { get; set; }
        public TextoInternacionalizado? helperText { get; set; } 
        public bool? singleBody { get; set; }
        public bool? showImage { get; set; }
        public bool? canLoadMoreThanCapacity { get; set; }
        public bool? multipleLines { get; set; }
        public List<EnvioModeloVeicularSlot> slots { get; set; }

    }
    public class EnvioModeloVeicularSlot
    {
        public decimal? totalCapacity { get; set; }
        public TextoInternacionalizado title { get; set; }
        public TextoInternacionalizado? description { get; set; }
        public List<EnvioModeloVeicularSubSlot> subSlot { get; set; }
        public List<LoadDetailIndicator> indicators { get; set; }
    }
    public class EnvioModeloVeicularSubSlot
    {
        public TextoInternacionalizado label { get; set; }
        public decimal totalCapacity { get; set; }
        public TextoInternacionalizado? placeHolder { get; set; }
    }
    
}
