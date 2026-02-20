namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoVigencia
    {
        Todos = 0,
        ApenasVigentes = 1,
        ForaDeVigencia = 2
    }

    public static class SituacaoVigenciaHelper
    {
        public static string ObterDescricao(this SituacaoVigencia situacao)
        {
            switch (situacao)
            {
                case SituacaoVigencia.ApenasVigentes: return "Apenas Vigentes";
                case SituacaoVigencia.ForaDeVigencia: return "Fora de Vigência";
                default: return string.Empty;
            }
        }
    }
}