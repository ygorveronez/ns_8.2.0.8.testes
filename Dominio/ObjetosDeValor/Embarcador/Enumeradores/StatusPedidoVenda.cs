namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusPedidoVenda
    {
        Aberta = 1,
        Finalizada = 2,
        Faturada = 3,
        Cancelada = 4,
        AbertaFinalizada = 5,
        PendenteOperacional = 6,
        EmAprovacao = 7
    }

    public static class StatusPedidoVendaHelper
    {
        public static string ObterDescricao(this StatusPedidoVenda status)
        {
            switch (status)
            {
                case StatusPedidoVenda.Aberta: return "Aberta";
                case StatusPedidoVenda.Finalizada: return "Finalizada";
                case StatusPedidoVenda.Faturada: return "Faturada";
                case StatusPedidoVenda.Cancelada: return "Cancelada";
                case StatusPedidoVenda.PendenteOperacional: return "Pendente Operacional";
                case StatusPedidoVenda.EmAprovacao: return "Em Aprovação";
                default: return string.Empty;
            }
        }
    }
}
