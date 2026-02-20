namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusPedidoEmbarcadorAssai
    {
        Todos = 0,
        Faturado = 1,
        Liberado = 2,
        EmCarregamento = 3,
        Cancelado = 99
    }

    public static class StatusPedidoEmbarcadorAssaiHelper
    {
        public static string ObterDescricao(this StatusPedidoEmbarcadorAssai status)
        {
            switch (status)
            {
                case StatusPedidoEmbarcadorAssai.Faturado: return "Faturado";
                case StatusPedidoEmbarcadorAssai.Liberado: return "Em Aberto";
                case StatusPedidoEmbarcadorAssai.EmCarregamento: return "Em Carregamento";
                case StatusPedidoEmbarcadorAssai.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
