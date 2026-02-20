namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga
{
    public class AlertaCarga
    {
        public int Codigo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga Tipo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus Status { get; set; }
    }
}
