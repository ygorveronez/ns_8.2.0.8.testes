namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoOcorrenciaLote
    {
        Todos = 0,
        EmGeracao = 1,
        Finalizado = 2,
        FalhaNaGeracao = 3
    }

    public static class SituacaoOcorrenciaLoteHelper
    {
        public static string ObterDescricao(this SituacaoOcorrenciaLote situacao)
        {
            switch (situacao)
            {
                case SituacaoOcorrenciaLote.EmGeracao: return "Em Geração";
                case SituacaoOcorrenciaLote.Finalizado: return "Finalizado";
                case SituacaoOcorrenciaLote.FalhaNaGeracao: return "Falha na Geração";
                default: return string.Empty;
            }
        }
    }
}
