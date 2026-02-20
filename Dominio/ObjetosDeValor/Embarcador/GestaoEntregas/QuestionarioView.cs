using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoEntregas
{
    public class QuestionarioView
    {
        public bool HabilitarAvaliacao { get; set; }

        public bool HabilitarAvaliacaoQuestionario { get; set; }
     
        public bool AvaliacaoRespondia { get; set; }

        public bool HabilitarAnexos { get; set; }

        public string LinkAvaliacaoExterna { get; set; }

        public List<PerguntasView> Perguntas { get; set; }

        public List<MotivosView> MotivosAvaliacao { get; set; }
    }
}

