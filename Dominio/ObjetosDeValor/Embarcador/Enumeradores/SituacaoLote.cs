namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLote
    {
        Todas = 8,
        EmCriacao = 1,
        AgAprovacaoTransportador = 2,
        ReprovacaoTransportador = 3,
        EmCorrecao = 4,
        AgIntegracao = 5,
        IntegracaoReprovada = 6,
        Finalizada = 7,
        Integrada = 9,
        AgAprovacaoIntegracao = 10,
        FalhaIntegracao = 11,
        EmIntegracao = 12,
        FinalizadaComDestino = 13
    }

    public static class SituacaoLoteHelper
    {
        public static string ObterDescricao(this SituacaoLote situacaoLote)
        {
            switch (situacaoLote)
            {
                case SituacaoLote.AgAprovacaoTransportador: return "Ag. Aprovação Transportador";
                case SituacaoLote.AgIntegracao: return "Ag. Integração";
                case SituacaoLote.EmCorrecao: return "Em Correção";
                case SituacaoLote.Finalizada: return "Finalizada";
                case SituacaoLote.IntegracaoReprovada: return "Integração Reprovada";
                case SituacaoLote.ReprovacaoTransportador: return "Reprovação Transportador";
                case SituacaoLote.EmCriacao: return "Em Criação";
                case SituacaoLote.Integrada: return "Integrada";
                case SituacaoLote.AgAprovacaoIntegracao: return "Ag. Aprovação Integração";
                case SituacaoLote.FalhaIntegracao: return "Falha Integração";
                case SituacaoLote.EmIntegracao: return "Em Integração";
                case SituacaoLote.FinalizadaComDestino: return "Finalizada com Destino";
                default: return string.Empty;
            }
        }
    }
}
