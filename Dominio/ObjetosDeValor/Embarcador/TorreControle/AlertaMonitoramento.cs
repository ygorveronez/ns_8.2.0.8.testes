namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga
{
    public class AlertaMonitoramento
    {
        public int Codigo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta Tipo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus Status { get; set; }
        public string Funcionario { get; set; }
    }
}
