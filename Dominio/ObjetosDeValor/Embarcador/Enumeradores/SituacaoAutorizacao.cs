namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAutorizacao
    {
        Todos = 0,
        PendenteAutorizacao = 1,
        SomenteAutorizados = 2
    }

    public static class SituacaoAutorizacaoHelper
    {
        public static string ObterDescricao(this SituacaoAutorizacao status)
        {
            switch (status)
            {
                case SituacaoAutorizacao.Todos: return "Todos";
                case SituacaoAutorizacao.PendenteAutorizacao: return "Pendentes de Autorização";
                case SituacaoAutorizacao.SomenteAutorizados: return "Somente Autorizados";
                default: return string.Empty;
            }
        }
    }
}
