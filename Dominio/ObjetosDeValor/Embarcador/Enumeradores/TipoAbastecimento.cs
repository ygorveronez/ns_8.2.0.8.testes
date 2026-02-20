namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAbastecimento
    {
        Combustivel = 1,
        Arla = 2,
    }

    public static class TipoAbastecimentoHelper
    {
        public static string ObterDescricao(this TipoAbastecimento tipoAbastecimento)
        {
            switch (tipoAbastecimento)
            {
                case TipoAbastecimento.Combustivel: return "Combustível";
                case TipoAbastecimento.Arla: return "ARLA";
                default: return string.Empty;
            }
        }
    }
}
