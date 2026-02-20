using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class LoadDetailIndicator
    {
        public TextoInternacionalizado? title { get; set; }
        public List<LoadDetailIndicatorData> data { get; set; }
    }
    public class LoadDetailIndicatorData
    {
        public TextoInternacionalizado label { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }
}
