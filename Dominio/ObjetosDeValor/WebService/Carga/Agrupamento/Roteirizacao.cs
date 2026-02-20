namespace Dominio.ObjetosDeValor.WebService.Carga.Agrupamento
{
    public class Roteirizacao
    {
        public Embarcador.Pessoas.Pessoa Destinatario { get; set; }
        public string DataPrevisaoChegada { get; set; }
        public string DataInicioDescarregamento { get; set; }
        public string IdEntrega { get; set; }
        public int Sequencia { get; set; }
        public int TempoDescarregamentoMinutos { get; set; }
        public string JanelaDeRecebimento { get; set; }
    }
}
