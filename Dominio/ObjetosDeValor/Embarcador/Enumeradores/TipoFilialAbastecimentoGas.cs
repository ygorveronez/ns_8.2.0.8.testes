namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFilialAbastecimentoGas
    {
        Supridora = 1,
        Satelite = 2,
    }

    public static class TipoFilialAbastecimentoGasHelper
    {
        public static string ObterDescricao(this TipoFilialAbastecimentoGas situacao)
        {
            switch (situacao)
            {
                case TipoFilialAbastecimentoGas.Supridora: return "Supridora";
                case TipoFilialAbastecimentoGas.Satelite: return "Satélite";
                default: return string.Empty;
            }
        }
    }
}
