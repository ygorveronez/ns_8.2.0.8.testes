namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAjusteTabelaFrete
    {
        Todas = 9,
        Pendente = 0,
        Finalizado = 1,
        Cancelado = 2,
        AgAprovacao = 3,
        AgIntegracao = 5,
        SemRegraAprovacao = 4,
        RejeitadaAutorizacao = 6,
        EmProcessamento = 7,
        ProblemaProcessamento = 8,
        EmCriacao = 10,
        ProblemaCriacao = 11,
        EmAjuste = 12,
        ProblemaAjuste = 13
    }

    public static class SituacaoAjusteTabelaFreteHelper
    {
        public static string ObterDescricao(this SituacaoAjusteTabelaFrete situacao)
        {
            switch (situacao)
            {
                case SituacaoAjusteTabelaFrete.AgAprovacao: return "Ag. Aprovação";
                case SituacaoAjusteTabelaFrete.AgIntegracao: return "Ag. Integração";
                case SituacaoAjusteTabelaFrete.Cancelado: return "Cancelado";
                case SituacaoAjusteTabelaFrete.EmAjuste: return "Aplicando Ajustes";
                case SituacaoAjusteTabelaFrete.EmCriacao: return "Em Criação";
                case SituacaoAjusteTabelaFrete.EmProcessamento: return "Em Processamento";
                case SituacaoAjusteTabelaFrete.Finalizado: return "Finalizado";
                case SituacaoAjusteTabelaFrete.Pendente: return "Pendente";
                case SituacaoAjusteTabelaFrete.ProblemaAjuste: return "Problema ao Aplicar Ajustes";
                case SituacaoAjusteTabelaFrete.ProblemaCriacao: return "Problema na Criação";
                case SituacaoAjusteTabelaFrete.ProblemaProcessamento: return "Problema no Processamento";
                case SituacaoAjusteTabelaFrete.RejeitadaAutorizacao: return "Rejeitada Autorização";
                case SituacaoAjusteTabelaFrete.SemRegraAprovacao: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }
    }
}
