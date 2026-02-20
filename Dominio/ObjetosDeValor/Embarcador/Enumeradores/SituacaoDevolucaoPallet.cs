namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDevolucaoPallet
    {
        AgEntrega = 0,
        Entregue = 1,
        Cancelado = 2,
        Liquidado = 3
    }

    public static class SituacaoDevolucaoPalletHelper
    {
        public static string ObterDescricao(this SituacaoDevolucaoPallet SituacaoDevolucaoPallet)
        {
            switch (SituacaoDevolucaoPallet)
            {
                case SituacaoDevolucaoPallet.AgEntrega: return "Aguardando Entrega";
                case SituacaoDevolucaoPallet.Entregue: return "Entregue";
                case SituacaoDevolucaoPallet.Cancelado: return "Cancelado";
                case SituacaoDevolucaoPallet.Liquidado: return "Liquidado";
                default: return "Nenhuma";
            }
        }
    }
}
