namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class RotaInformacaoPessoa
    {
        public decimal distancia { get; set; }
        public decimal tempo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa { get; set; }
        public Coordenadas coordenadas { get; set; }
        public bool coleta { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem TipoPonto { get; set; }
        public bool Finalizada { get; set; }
        public string DataAgendamento { get; set; }
        public int CodigoOutroEndereco { get { return this.coordenadas?.CodigoOutroEndereco ?? 0; } }
    }
}
