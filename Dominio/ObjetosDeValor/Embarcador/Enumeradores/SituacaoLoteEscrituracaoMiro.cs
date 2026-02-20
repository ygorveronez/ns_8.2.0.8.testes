namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLoteEscrituracaoMiro
    {
        Todos = 0,
        EmCriacao = 1,
        AgIntegracao = 2,
        FalhaIntegracao = 3,
        Finalizado = 4,
        Cancelado = 5
    }

    public static class SituacaoLoteEscrituracaoMiroHelper
    {
        public static string ObterDescricao(this SituacaoLoteEscrituracaoMiro situacao)
        {
            switch (situacao)
            {
                case SituacaoLoteEscrituracaoMiro.EmCriacao:
                    return "Em Criação";
                case SituacaoLoteEscrituracaoMiro.AgIntegracao:
                    return "Aguardando Integração";
                case SituacaoLoteEscrituracaoMiro.FalhaIntegracao:
                    return "Falha Integração";
                case SituacaoLoteEscrituracaoMiro.Finalizado:
                    return "Finalizado";    
                case SituacaoLoteEscrituracaoMiro.Cancelado:
                    return "Cancelado";
                default:
                    return "";
            }
        }
    }
}
