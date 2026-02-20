namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CargaPedidoHistoricoSituacaoIntegracao
    {
        Aguardando = 1,
        Integrado = 2
    }


    public static class CargaPedidoHistoricoSituacaoIntegracaoHelper
    {
        public static string ObterDescricao(this CargaPedidoHistoricoSituacaoIntegracao o)
        {
            switch (o)
            {
                case CargaPedidoHistoricoSituacaoIntegracao.Aguardando: return "Aguardando";
                case CargaPedidoHistoricoSituacaoIntegracao.Integrado: return "Integrado";
                default: return string.Empty;
            }
        }
    }
}
