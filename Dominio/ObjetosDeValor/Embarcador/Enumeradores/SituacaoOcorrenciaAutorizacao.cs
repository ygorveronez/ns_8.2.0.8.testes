namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoOcorrenciaAutorizacao
    {
        Pendente = 0,
        Aprovada = 1,
        Rejeitada = 9
    }

    public static class SituacaoOcorrenciaAutorizacaoHelper
    {
        public static string ObterDescricao(this SituacaoOcorrenciaAutorizacao situacao)
        {
            switch (situacao)
            {
                case SituacaoOcorrenciaAutorizacao.Pendente: return "Pendente";
                case SituacaoOcorrenciaAutorizacao.Aprovada: return "Aprovada";
                case SituacaoOcorrenciaAutorizacao.Rejeitada: return "Rejeitada";
                default: return string.Empty;
            }
        }
    }
}
