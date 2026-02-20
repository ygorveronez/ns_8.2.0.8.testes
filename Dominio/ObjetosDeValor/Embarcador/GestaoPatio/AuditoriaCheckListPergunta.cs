using Dominio.Entidades.Embarcador.GestaoPatio;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public class AuditoriaCheckListPergunta
    {
        public CheckListCargaPergunta Pergunta { get; set; }

        public string RespostaAntiga { get; set; }

        public string RespostaNova { get; set; }
        
        public string Observacao { get; set; }
    }
}
