namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public sealed class AtualizarDatasPedido
    {
        public int ProtocoloPedido { get; set; }
        public string DataAgendamento { get; set; }
        public string DataEntrega { get; set; }
        public string DataUltimaLiberacao { get; set; }
        public string SenhaAgendamento { get; set; }
        public string SenhaAgendamentoCliente { get; set; }
        public string ObservacoesPedido { get; set; }
    }
}
