namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoImportarOcorrencia
    {
        Todos = 0,
        AgIntegracao = 1,
        Falha = 2,
        Finalizada = 3
    }

    public static class SituacaoImportarOcorrenciaHelper
    {
        public static string ObterDescricao(this SituacaoImportarOcorrencia situacao)
        {
            switch (situacao)
            {
                case SituacaoImportarOcorrencia.Todos: return "Todos";
                case SituacaoImportarOcorrencia.AgIntegracao: return "Ag. Integração";
                case SituacaoImportarOcorrencia.Falha: return "Falha na integração";
                case SituacaoImportarOcorrencia.Finalizada: return "Finalizada";
                default: return string.Empty;
            }
        }
    }
}
