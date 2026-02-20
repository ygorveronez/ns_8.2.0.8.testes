namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPalletGestaoDevolucao
    {
        NoPrazo = 1,
        Vencido = 2,
        Agendado = 3,
        Permuta = 4,
    }

    public static class SituacaoPalletGestaoDevolucaoHelper
    {
        public static string ObterDescricao(this SituacaoPalletGestaoDevolucao situacaoPalletGestaoDevolucao)
        {
            switch (situacaoPalletGestaoDevolucao)
            {
                case SituacaoPalletGestaoDevolucao.NoPrazo: return "No Prazo";
                case SituacaoPalletGestaoDevolucao.Vencido: return "Vencido";
                case SituacaoPalletGestaoDevolucao.Agendado: return "Agendado";
                case SituacaoPalletGestaoDevolucao.Permuta: return "Permuta";
                default: return string.Empty;
            }
        }
    }
}
