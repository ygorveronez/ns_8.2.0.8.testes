namespace Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao
{


    public class RespostaRoteirizacaoDetalhe
    {
        public decimal Distancia { get; set; }
        public string Polilinha { get; set; }
        public int TempoMinutos { get; set; }

        public int TempoHoras { get; set; }
        public string PontoDaRota { get; set; }
        public string Informacoes { get; set; }
    }


    public class RespostaRoteirizacao
    {
        public RespostaRoteirizacao()
        {
            Ida = new RespostaRoteirizacaoDetalhe();
            Volta = new RespostaRoteirizacaoDetalhe();
        }

        public string Status { get; set; }
        public decimal Distancia { get; set; }
        public string Polilinha { get; set; }
        public int TempoMinutos { get; set; }

        public int TempoHoras { get; set; }
        public string PontoDaRota { get; set; }
        public string Informacoes { get; set; }

        public RespostaRoteirizacaoDetalhe Ida;
        public RespostaRoteirizacaoDetalhe Volta;

        public bool IsSuccess()
        {
            return Status.Equals("OK");
        }
    }
}
