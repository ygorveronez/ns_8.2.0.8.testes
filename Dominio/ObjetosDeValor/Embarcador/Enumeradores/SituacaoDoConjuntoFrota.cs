namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoDoConjuntoFrota
    {
        Todos = 0,
        SemTracao = 1,
        SemReboque = 2,
        ConjuntoCompleto = 3
    }

    public static class SituacaoDoConjuntoFrotaHelper
    {
        public static string ObterDescricao(this SituacaoDoConjuntoFrota situacaoFrota)
        {
            switch (situacaoFrota)
            {
                case SituacaoDoConjuntoFrota.SemTracao: return "Sem Tração";
                case SituacaoDoConjuntoFrota.SemReboque: return "Sem Reboque";
                case SituacaoDoConjuntoFrota.ConjuntoCompleto: return "Conjunto Completo";
                default: return string.Empty;
            }
        }
    }
}
