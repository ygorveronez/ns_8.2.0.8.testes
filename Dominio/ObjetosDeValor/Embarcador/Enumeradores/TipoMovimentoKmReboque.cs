namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMovimentoKmReboque
    {
        Todos = 0,
        Vinculo = 1,
        Desvinculo = 2        
    }

    public static class TipoMovimentoKmReboqueHelper
    {
        public static string ObterDescricao(this TipoMovimentoKmReboque tipo)
        {
            switch (tipo)
            {
                case TipoMovimentoKmReboque.Todos: return "Todos";
                case TipoMovimentoKmReboque.Vinculo: return "Vínculo";
                case TipoMovimentoKmReboque.Desvinculo: return "Desvínculo";
                default: return string.Empty;
            }
        }
    }
}
