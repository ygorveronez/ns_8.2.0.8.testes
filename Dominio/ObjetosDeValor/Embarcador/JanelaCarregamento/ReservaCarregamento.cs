namespace Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento
{
    public class ReservaCarregamento
    {
        public ReservaCarregamento()
        {
            this.SituacaoReserva = Enumeradores.SituacaoReserva.NaoAtendePrazoEntrega;
        }

        public int ProtocoloReserva { get; set; }
        public string DataHoraConsegueEntregar { get; set; }
        public string DataHoraPrevisaoCarregamento { get; set; }
        public bool PossuiRota { get; set; }
        public string TempoEstimadoDeViagem { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoReserva SituacaoReserva { get; set; }
    }
}
