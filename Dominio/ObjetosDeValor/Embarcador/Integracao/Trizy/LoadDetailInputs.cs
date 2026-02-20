using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class LoadDetailInputs
    {
        public LoadDetailInput input { get; set; }
    }

    public class LoadDetailInput
    {
        public string _id { get; set; }
        public string type { get; set; }
        public bool required { get; set; }
        public TextoInternacionalizado label { get; set; }
        public int? minimumFractionDigits { get; set; }
        public int? maximumFractionDigits { get; set; }
        public ChecklistStepMinMax? range { get; set; }
        public List<LabelValue>? options { get; set; }
        public InformacaoExterna externalInfo { get; set; }
    }
}
