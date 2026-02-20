namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoDataPedido
    {
       DataPrevisaoSaida = 1,
       DataPrevisaoEntrega = 2
    }

    public static class TipoDataPedidoHelper
    {
        public static string ObterDescricao(this TipoDataPedido t)
        {
            switch (t)
            {
                case TipoDataPedido.DataPrevisaoSaida: return "Data de previsão de saída";
                case TipoDataPedido.DataPrevisaoEntrega: return "Data de previsão de entrega";
                default: return string.Empty;
            }
        }
    }
}
