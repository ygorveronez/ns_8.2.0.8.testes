namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusBiddingConviteConvidado
    {
        Aguardando = 0,
        Aceito = 1,
        Rejeitado = 2
    }

    public static class StatusBiddingConviteConvidadoHelper
    {
        public static string ObterDescricao(this StatusBiddingConviteConvidado status)
        {
            switch (status)
            {
                case StatusBiddingConviteConvidado.Aguardando: return "Aguardando resposta";
                case StatusBiddingConviteConvidado.Aceito: return "Aceitou participar";
                case StatusBiddingConviteConvidado.Rejeitado: return "Recusou participação";
                default: return string.Empty;
            }
        }
    }
}
