namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoRetornoCIOT
    {
        Autorizado = 0,
        EmProcessamento = 1,
        ProblemaIntegracao = 2
    }

    public static class SituacaoRetornoCIOTHelper
    {
        public static string ObterDescricao(this SituacaoRetornoCIOT? situacao)
        {
            if (!situacao.HasValue)
                return string.Empty;

            return situacao.Value.ObterDescricao();
        }

        public static string ObterDescricao(this SituacaoRetornoCIOT situacao)
        {
            switch (situacao)
            {
                case SituacaoRetornoCIOT.Autorizado: return "Autorizado";
                case SituacaoRetornoCIOT.EmProcessamento: return "Em Processamento";
                case SituacaoRetornoCIOT.ProblemaIntegracao: return "Problema Integração";
                default: return string.Empty;
            }
        }
    }
}
