namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFluxoCompra
    {
        Todos = 0,
        Aberto = 1,
        Finalizado = 2,
        Cancelado = 3,
        Rejeitado = 4
    }

    public static class SituacaoFluxoCompraHelper
    {
        public static string ObterDescricao(this SituacaoFluxoCompra situacao)
        {
            switch (situacao)
            {
                case SituacaoFluxoCompra.Aberto: return "Aberto";
                case SituacaoFluxoCompra.Finalizado: return "Finalizado";
                case SituacaoFluxoCompra.Cancelado: return "Cancelado";
                case SituacaoFluxoCompra.Rejeitado: return "Rejeitado";
                default: return string.Empty;
            }
        }
    }
}
