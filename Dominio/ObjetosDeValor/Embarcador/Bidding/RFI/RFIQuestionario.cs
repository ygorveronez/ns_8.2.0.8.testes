using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding.RFI
{
    public class RFIQuestionario
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string Requisito { get; set; }
        public string TipoOpcaoCheckListRFI { get; set; }
        public List<RFIChecklistQuestionarioAlternativa> GridMultiplaEscolha { get; set; }
        public List<RFIChecklistAnexo> ChecklistAnexo { get; set; }
    }
}
