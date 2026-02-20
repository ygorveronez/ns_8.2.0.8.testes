using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding.RFI
{
    public class RFIChecklist
    {
        public int Codigo { get; set; }
        public string Prazo { get; set; }
        public List<RFIQuestionario> Questionarios { get; set; }
    }
}
