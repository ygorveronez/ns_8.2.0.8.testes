
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFluxoAvaria
    {
        Todas = 0,
        Dados = 1,
        Produtos = 2,
        AgAprovacao = 3,
        Termo = 4,
        AgLote = 5,
        LoteGerado = 6,
        AgIntegracao = 7,
        Destinacao = 8,
        Finalizado = 9,
        RejeitadaAutorizacao = 10,
        RejeitadaLote = 11,
        RejeitadaIntegracao = 12,
        SemRegraAprovacao = 13,
        SemRegraLote = 14,
        Cancelado = 15
    }

    public static class SituacaoFluxoAvariaHelper
    {
        public static string ObterDescricao(this SituacaoFluxoAvaria situacaoFluxoAvaria)
        {
            switch (situacaoFluxoAvaria)
            {
                case SituacaoFluxoAvaria.Dados: return "Dados";
                case SituacaoFluxoAvaria.Produtos: return "Produtos";
                case SituacaoFluxoAvaria.AgAprovacao: return "Aguardando Aprovação";
                case SituacaoFluxoAvaria.Termo: return "Termo";
                case SituacaoFluxoAvaria.AgLote: return "Agurardando Lote";
                case SituacaoFluxoAvaria.LoteGerado: return "Lote Gerado";
                case SituacaoFluxoAvaria.AgIntegracao: return "AguardandoIntegracao";
                case SituacaoFluxoAvaria.Destinacao: return "Destinação";
                case SituacaoFluxoAvaria.Finalizado: return "Finalizado";
                case SituacaoFluxoAvaria.RejeitadaAutorizacao: return "Autorização Rejeitada";
                case SituacaoFluxoAvaria.RejeitadaLote: return "Lote Rejeitado";
                case SituacaoFluxoAvaria.RejeitadaIntegracao: return "Integração Rejeitada";
                case SituacaoFluxoAvaria.SemRegraAprovacao: return "Sem Regra de Aprovação";
                case SituacaoFluxoAvaria.SemRegraLote: return "Sem Regra de Lote";
                case SituacaoFluxoAvaria.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}
