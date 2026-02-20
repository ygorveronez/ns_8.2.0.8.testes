
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAvaria
    {
        Todas = 0,
        Aberta = 1,
        EmCriacao = 2,
        Cancelada = 3,
        AgAprovacao = 4,
        AgLote = 5,
        AgIntegracao = 12,
        LoteGerado = 8,
        Finalizada = 14,
        SemRegraAprovacao = 7,
        SemRegraLote = 13,
        RejeitadaAutorizacao = 9,
        RejeitadaIntegracao = 10,
        RejeitadaLote = 11
    }

    public static class SituacaoAvariaHelper
    {
        public static string ObterDescricao(this SituacaoAvaria situacaoAvaria)
        {
            switch (situacaoAvaria)
            {
                case SituacaoAvaria.Aberta: return "Aberta";
                case SituacaoAvaria.EmCriacao: return "Em Criação";
                case SituacaoAvaria.Cancelada: return "Cancelada";
                case SituacaoAvaria.AgAprovacao: return "Ag. Aprovação";
                case SituacaoAvaria.AgLote: return "Ag. Lote";
                case SituacaoAvaria.AgIntegracao: return "Ag. Integração";
                case SituacaoAvaria.LoteGerado: return "Lote Gerado";
                case SituacaoAvaria.Finalizada: return "Finalizada";
                case SituacaoAvaria.SemRegraAprovacao: return "Sem Regra Aprovação";
                case SituacaoAvaria.SemRegraLote: return "Sem Regra Lote";
                case SituacaoAvaria.RejeitadaAutorizacao: return "Rejeitada Autorização";
                case SituacaoAvaria.RejeitadaIntegracao: return "Rejeitada Integração";
                case SituacaoAvaria.RejeitadaLote: return "Rejeitada Lote";
                default: return string.Empty;
            }
        }
    }
}
