namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica
{
    public class Manobra
    {
        public string Acao { get; set; }

        public int Codigo { get; set; }

        public bool Higienizado { get; set; }

        public string HigienizadoDescricao { get; set; }

        public string LocalAtual { get; set; }

        public string LocalDestino { get; set; }

        public string PlacasReboques { get; set; }

        public string PlacaTracao { get; set; }

        public string TempoAguardando { get; set; }

        public Embarcador.Enumeradores.SituacaoManobra Situacao { get; set; }
    }
}
