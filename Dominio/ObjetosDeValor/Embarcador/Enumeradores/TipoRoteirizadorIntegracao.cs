namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRoteirizadorIntegracao
    {
        EnviarPedido = 0,
        CancelarPedido = 1,
        AtualizarPedido = 2,
    }

    public static class TipoRoteirizadorIntegracaoHelper
    {
        public static string ObterDescricao(this TipoRoteirizadorIntegracao tipo)
        {
            switch (tipo)
            {
                case TipoRoteirizadorIntegracao.EnviarPedido: return "Enviar Pedido";
                case TipoRoteirizadorIntegracao.CancelarPedido: return "Cancelar Pedido";
                case TipoRoteirizadorIntegracao.AtualizarPedido: return "Atualizar Pedido";
                default: return string.Empty;
            }
        }
    }
}
