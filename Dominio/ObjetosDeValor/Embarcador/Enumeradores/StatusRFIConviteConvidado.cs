namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusRFIConviteConvidado
    {
        Aguardando = 0,
        Aceito = 1,
        Rejeitado = 2
    }

    public static class StatusRFIConviteConvidadoHelper
    {
        public static string ObterDescricao(this StatusRFIConviteConvidado status)
        {
            switch (status)
            {
                case StatusRFIConviteConvidado.Aguardando: return "Aguardando resposta";
                case StatusRFIConviteConvidado.Aceito: return "Aceitou participar";
                case StatusRFIConviteConvidado.Rejeitado: return "Recusou participação";
                default: return string.Empty;
            }
        }
    }
}
