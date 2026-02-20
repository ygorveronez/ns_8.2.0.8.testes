namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOrdemEmbarqueHistoricoIntegracao
    {
        AdicaoPedido = 1,
        RemocaoPedido = 2,
        TrocaPedido = 3,
        AlteracaoOrdemEmbarque = 4
    }

    public static class TipoOrdemEmbarqueHistoricoIntegracaoHelper
    {
        public static string ObterDescricao(this TipoOrdemEmbarqueHistoricoIntegracao tipo)
        {
            switch (tipo)
            {
                case TipoOrdemEmbarqueHistoricoIntegracao.AdicaoPedido: return "Adição de Pedido";
                case TipoOrdemEmbarqueHistoricoIntegracao.AlteracaoOrdemEmbarque: return "Alteração de Ordem de Embarque";
                case TipoOrdemEmbarqueHistoricoIntegracao.RemocaoPedido: return "Remoção de Pedido";
                case TipoOrdemEmbarqueHistoricoIntegracao.TrocaPedido: return "Troca de Pedido";
                default: return string.Empty;
            }
        }
    }
}
