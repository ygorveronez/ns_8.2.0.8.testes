namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormaDeducaoValePedagio
    {
        NaoAplicado = 0,
        ReduzirValorFrete = 1,
        AcrescentarValorFrete = 2
    }

    public static class FormaDeducaoValePedagioHelper
    {
        public static string ObterDescricao(this FormaDeducaoValePedagio tipo)
        {
            switch (tipo)
            {
                case FormaDeducaoValePedagio.NaoAplicado: return "Não Aplicado";
                case FormaDeducaoValePedagio.ReduzirValorFrete: return "Reduzir do valor do frete";
                case FormaDeducaoValePedagio.AcrescentarValorFrete: return "Acrescentando o valor ao frete";
                default: return string.Empty;
            }
        }
    }
}
