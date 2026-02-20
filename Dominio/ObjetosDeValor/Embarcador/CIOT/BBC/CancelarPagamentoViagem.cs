namespace Dominio.ObjetosDeValor.Embarcador.CIOT.BBC
{
    public class CancelarPagamentoViagem
    {
        public int pagamentoExternoId { get; set; }
        public int viagemExternoId { get; set; }
        public string Evento { get; set; }
        public string dataRequisicao { get; set; }
        public decimal valor { get; set; }
        public string motivo { get; set; }
    }
}
