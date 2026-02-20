namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoControleSaldoPedido
    {
        Peso = 0,
        Pallet = 1,
        CarregamentoUnico = 9
    }

    public static class TipoControleSaldoPedidoHelper
    {
        public static string ObterDescricao(this TipoControleSaldoPedido tipoControle)
        {
            switch (tipoControle)
            {
                case TipoControleSaldoPedido.Peso: return "Peso";
                case TipoControleSaldoPedido.Pallet: return "Pallet";
                case TipoControleSaldoPedido.CarregamentoUnico: return "Carregamento Único";
                default: return string.Empty;
            }
        }
    }
}