namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoTermoQuitacao
    {
        AguardandoAceiteTransportador = 1,
        AceiteTransportadorRejeitado = 2,
        AguardandoAprovacao = 3,
        SemRegraAprovacao = 4,
        AprovacaoRejeitada = 5,
        Finalizado = 6
    }

    public static class SituacaoTermoQuitacaoHelper
    {
        public static string ObterDescricao(this SituacaoTermoQuitacao situacaoTermoQuitacao)
        {
            switch (situacaoTermoQuitacao)
            {
                case SituacaoTermoQuitacao.AceiteTransportadorRejeitado: return "Aceite Rejeitado";
                case SituacaoTermoQuitacao.AguardandoAceiteTransportador: return "Aguardando Aceite";
                case SituacaoTermoQuitacao.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoTermoQuitacao.AprovacaoRejeitada: return "Aprovação Rejeitada";
                case SituacaoTermoQuitacao.Finalizado: return "Finalizado";
                case SituacaoTermoQuitacao.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }
    }
}
