namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCarregamentoPedido
    {
        NaoDefinido = 0,
        Normal = 1,
        TrocaNota = 2
    }

    public static class TipoCarregamentoPedidoHelper
    {
        public static string ObterDescricao(this TipoCarregamentoPedido tipo)
        {
            switch (tipo)
            {
                case TipoCarregamentoPedido.Normal: return "Normal";
                case TipoCarregamentoPedido.TrocaNota: return "Troca de Nota";
                default: return "Não Definido";
            }
        }
    }
}
