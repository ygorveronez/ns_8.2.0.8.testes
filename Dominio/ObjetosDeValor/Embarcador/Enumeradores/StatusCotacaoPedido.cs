namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusCotacaoPedido
    {
        Todos = 0,
        Fechada = 1,
        PerdaPorPreco = 2,
        PerdaPorDesistenciaDoServico = 3,
        PerdaPorNaoJustificativaPeloCliente = 4,
        PerdaPorPrazoDeResposta = 5,
        PertaPorQualificacaoDocumental = 6,
        PerdaPorQualificacaoTecnica = 7,
        PerdaPorInfraestrutura = 8,
        PerdaPorAnaliseCadastral = 9,
        EmAnalise = 10,
        Sondagem = 11

    }

    public static class StatusCotacaoPedidoHelper
    {
        public static string Descricao(this StatusCotacaoPedido statusCotacaoPedido)
        {
            switch (statusCotacaoPedido)
            {
                case StatusCotacaoPedido.Todos:
                    return "Todos";
                case StatusCotacaoPedido.Fechada:
                    return "Fechada";
                case StatusCotacaoPedido.PerdaPorPreco:
                    return "Perda por preço";
                case StatusCotacaoPedido.PerdaPorDesistenciaDoServico:
                    return "Perda por desistência do serviço";
                case StatusCotacaoPedido.PerdaPorNaoJustificativaPeloCliente:
                    return "Perda por não justificativa pelo cliente";
                case StatusCotacaoPedido.PerdaPorPrazoDeResposta:
                    return "Perda por prazo de resposta";
                case StatusCotacaoPedido.PertaPorQualificacaoDocumental:
                    return "Perta por qualificação documental";
                case StatusCotacaoPedido.PerdaPorQualificacaoTecnica:
                    return "Perda por qualificação técnica";
                case StatusCotacaoPedido.PerdaPorInfraestrutura:
                    return "Perda por infraestrutura";
                case StatusCotacaoPedido.PerdaPorAnaliseCadastral:
                    return "Perda por análise cadastral";
                case StatusCotacaoPedido.EmAnalise:
                    return "Em análise";
                case StatusCotacaoPedido.Sondagem:
                    return "Sondagem (concorrente)";
                default:
                    return string.Empty;
            }
        }
    }
}
