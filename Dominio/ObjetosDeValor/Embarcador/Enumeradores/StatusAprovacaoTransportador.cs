namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusAprovacaoTransportador
    {
        Rejeitado = 0,
        Aprovado = 1,
        AguardandoAprovacao = 2,

    }

    public static class StatusAprovacaoTransportadorHelper
    {
        public static string ObterDescricao(this StatusAprovacaoTransportador statusAprovacaoTransportador)
        {
            switch (statusAprovacaoTransportador)
            {
                case StatusAprovacaoTransportador.Rejeitado: return "Rejeitado";
                case StatusAprovacaoTransportador.Aprovado: return "Aprovado";
                case StatusAprovacaoTransportador.AguardandoAprovacao: return "Aguardando Aprovação";
                default: return string.Empty;
            }
        }
    }
}