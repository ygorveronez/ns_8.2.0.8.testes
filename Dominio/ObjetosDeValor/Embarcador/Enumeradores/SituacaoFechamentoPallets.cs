namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFechamentoPallets
    {
        Aberto = 1,
        Finalizado = 2,
    }

    public static class SituacaoFechamentoPalletsHelper
    {
        public static string ObterDescricao(this SituacaoFechamentoPallets situacao)
        {
            switch (situacao)
            {
                case SituacaoFechamentoPallets.Aberto:
                    return "Aberto";
                case SituacaoFechamentoPallets.Finalizado:
                    return "Finalizado";
                default:
                    return "";
            }
        }
    }
}