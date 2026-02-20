namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAutorizacaoToken
    {
        Todos = 0,
        Finalizada = 1,
        Rejeidada = 2,
        Aprovada = 3,
        Pendente = 4,

    }

    public static class SituacaoAutorizacaoTokenHelper
    {
        public static string ObterDescricao(this SituacaoAutorizacaoToken situacaoAprovacao)
        {
            switch (situacaoAprovacao)
            {
                case SituacaoAutorizacaoToken.Finalizada: return "Finalizada";
                case SituacaoAutorizacaoToken.Rejeidada: return "Rejeitada";
                case SituacaoAutorizacaoToken.Aprovada: return "Aprovada";
                case SituacaoAutorizacaoToken.Pendente: return "Pendente";

                default: return "Sem Regra de Aprovação";
            }
        }
    }    
}



