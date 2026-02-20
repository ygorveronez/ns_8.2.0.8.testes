namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoContatoAgendamentoEntregaPedido
    {
        Transportador = 1,
        Cliente = 2
    }
    
    public static class TipoContatoAgendamentoEntregaPedidoHelper
    {
        public static string ObterDescricao(this TipoContatoAgendamentoEntregaPedido tipo)
        {
            switch (tipo)
            {
                case TipoContatoAgendamentoEntregaPedido.Transportador: return "Transportador";
                case TipoContatoAgendamentoEntregaPedido.Cliente: return "Cliente";
                default: return string.Empty;
            }
        }
    }
}
