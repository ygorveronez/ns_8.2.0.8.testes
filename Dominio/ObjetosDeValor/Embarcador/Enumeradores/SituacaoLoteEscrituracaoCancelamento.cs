namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLoteEscrituracaoCancelamento
    {
        EmCriacao = 1,
        AgIntegracao = 2,
        FalhaIntegracao = 3,
        Finalizado = 4,
        Cancelado = 5
    }

    public static class SituacaoLoteEscrituracaoCancelamentoHelper
    {
        public static string ObterDescricao(this SituacaoLoteEscrituracaoCancelamento situacao)
        {
            switch (situacao)
            {
                case SituacaoLoteEscrituracaoCancelamento.EmCriacao: return "Em Criação";
                case SituacaoLoteEscrituracaoCancelamento.AgIntegracao: return "Ag. Integração";
                case SituacaoLoteEscrituracaoCancelamento.FalhaIntegracao: return "Falha na Integração";
                case SituacaoLoteEscrituracaoCancelamento.Finalizado: return "Finalizado";
                case SituacaoLoteEscrituracaoCancelamento.Cancelado: return "Cancelado";
                default: return "";
            }
        }
    }
}
