namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPreCargaOfertaTransportador
    {
        Disponivel = 0,
        AguardandoAceite = 1,
        AguardandoConfirmacao = 2,
        Confirmada = 3,
        Rejeitada = 4
    }

    public static class SituacaoPreCargaOfertaTransportadorHelper
    {
        public static string ObterDescricao(this SituacaoPreCargaOfertaTransportador situacao)
        {
            switch (situacao)
            {
                case SituacaoPreCargaOfertaTransportador.AguardandoAceite: return "Aguardando Aceite";
                case SituacaoPreCargaOfertaTransportador.AguardandoConfirmacao: return "Aguardando Confirmação";
                case SituacaoPreCargaOfertaTransportador.Confirmada: return "Confirmada";
                case SituacaoPreCargaOfertaTransportador.Disponivel: return "Disponível";
                case SituacaoPreCargaOfertaTransportador.Rejeitada: return "Rejeitada";
                default: return string.Empty;
            }
        }
    }
}
