namespace Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento
{
    public sealed class CapacidadeCarregamentoPeriodo
    {
        public decimal CapacidadeDisponivel { get; set; }

        public Entidades.Embarcador.Logistica.PeriodoCarregamento PeriodoCarregamento { get; set; }
    }
}
