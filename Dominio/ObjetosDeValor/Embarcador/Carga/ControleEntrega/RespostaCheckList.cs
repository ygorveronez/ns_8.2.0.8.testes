
namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public class RespostaCheckList
    {
        public int Codigo { get; set; }

        public int Tipo { get; set; }

        public bool NaoAplica { get; set; }

        public string Resposta { get; set; }

        public RespostaCheckListAlternativas[] Alternativas { get; set; }
    }
}
