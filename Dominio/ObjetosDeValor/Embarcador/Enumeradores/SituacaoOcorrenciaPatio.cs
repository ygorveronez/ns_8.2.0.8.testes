namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoOcorrenciaPatio
    {
        Pendente = 1,
        Aprovada = 2,
        Reprovada = 3
    }

    public static class SituacaoOcorrenciaPatioHelper
    {
        public static string ObterCorLinha(this SituacaoOcorrenciaPatio situacao)
        {
            switch (situacao)
            {
                case SituacaoOcorrenciaPatio.Aprovada: return "#85de7b";
                case SituacaoOcorrenciaPatio.Pendente: return "#c8e8ff";
                case SituacaoOcorrenciaPatio.Reprovada: return "#ff9999";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoOcorrenciaPatio situacao)
        {
            switch (situacao)
            {
                case SituacaoOcorrenciaPatio.Aprovada: return "Aprovada";
                case SituacaoOcorrenciaPatio.Pendente: return "Pendente";
                case SituacaoOcorrenciaPatio.Reprovada: return "Reprovada";
                default: return string.Empty;
            }
        }
    }
}
