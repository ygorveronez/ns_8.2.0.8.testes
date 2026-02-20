namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum NaturezaMovimentacaoEstoquePallet
    {
        Avaria = 1,
        Cliente = 2,
        Filial = 3,
        Reforma = 4,
        Transportador = 5
    }

    public static class NaturezaMovimentacaoEstoquePalletHelper
    {
        public static string ObterDescricao(this NaturezaMovimentacaoEstoquePallet naturezaMovimentacao)
        {
            switch (naturezaMovimentacao)
            {
                case NaturezaMovimentacaoEstoquePallet.Avaria:
                    return "Avaria";

                case NaturezaMovimentacaoEstoquePallet.Cliente:
                    return "Cliente";

                case NaturezaMovimentacaoEstoquePallet.Filial:
                    return "Filial";

                case NaturezaMovimentacaoEstoquePallet.Reforma:
                    return "Reforma";

                default:
                    return "Transportador";
            }
        }
    }
}
