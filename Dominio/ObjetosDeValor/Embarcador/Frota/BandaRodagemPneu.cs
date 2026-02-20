namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class BandaRodagemPneu
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoBandaRodagemPneu Tipo { get; set; }
        public MarcaPneu Marca { get; set; }
    }
}
