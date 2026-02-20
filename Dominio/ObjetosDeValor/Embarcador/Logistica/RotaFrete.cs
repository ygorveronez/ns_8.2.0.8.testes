namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class RotaFrete
    {
        public string Polilinha { get; set; }

        public int TempoViagemMinutos { get; set; }

        public Enumeradores.TipoUltimoPontoRoteirizacao TipoUltimoPontoRoteirizacao { get; set; }
    }
}
