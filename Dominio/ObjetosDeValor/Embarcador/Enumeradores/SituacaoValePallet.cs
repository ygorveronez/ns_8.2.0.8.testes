namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoValePallet
    {
        AgRecolhimento = 1,
        AgDevolucao = 4,
        Recolhido = 2,
        Cancelado = 3
    }

    public static class SituacaoValePalletHelper
    {
        public static string ObterDescricao(this SituacaoValePallet situacao)
        {
            switch (situacao)
            {
                case SituacaoValePallet.AgRecolhimento:
                    return "Ag. Recolhimento";
                case SituacaoValePallet.AgDevolucao:
                    return "Ag. Devolução";
                case SituacaoValePallet.Recolhido:
                    return "Recolhido";
                case SituacaoValePallet.Cancelado:
                    return "Cancelado";
                default:
                    return "";
            }
        }
    }
}
