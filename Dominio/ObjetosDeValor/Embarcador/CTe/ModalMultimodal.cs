using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ModalMultimodal
    {
        public string NumeroCOTM { get; set; }
        public Dominio.Enumeradores.OpcaoSimNao? IndicadorNegociavel { get; set; }
        public List<ModalMultimodalContainer> Containers { get; set; }
    }
}
