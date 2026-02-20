namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoStatusStatusViagem
    {
        public int Codigo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus? Status { get; set; }
        public int? CodigoStatusViagem { get; set; }
    }
}
