namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPreFaturaNatura
    {
        Pendente = 0,
        Falha = 1,
        Gerada = 2,
        Atualizando = 3
    }

    public static class SituacaoPreFaturaNaturaHelper
    {
        public static string ObterDescricao(this SituacaoPreFaturaNatura situacao)
        {
            switch (situacao)
            {
                case SituacaoPreFaturaNatura.Pendente: return "Pendente";
                case SituacaoPreFaturaNatura.Falha: return "Falha";
                case SituacaoPreFaturaNatura.Gerada: return "Gerada";
                case SituacaoPreFaturaNatura.Atualizando: return "Atualizando";
                default: return string.Empty;
            }
        }
    }
}
