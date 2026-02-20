namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMotivoRejeicaoAlteracaoPedido
    {
        Todos = 0,
        Embarcador = 1,
        Transportador = 2
    }

    public static class TipoMotivoRejeicaoAlteracaoPedidoHelper
    {
        public static string ObterDescricao(this TipoMotivoRejeicaoAlteracaoPedido tipo)
        {
            switch (tipo)
            {
                case TipoMotivoRejeicaoAlteracaoPedido.Embarcador: return "Embarcador";
                case TipoMotivoRejeicaoAlteracaoPedido.Todos: return "Todos";
                case TipoMotivoRejeicaoAlteracaoPedido.Transportador: return "Transportador";
                default: return string.Empty;
            }
        }
    }
}
