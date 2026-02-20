namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoValorContratoFreteADA
    {
        Fixo = 1,
        Calculado = 2
    }
    public static class TipoValorContratoFreteADAHelper
    {
        public static string ObterDescricao(this TipoValorContratoFreteADA tipoValor)
        {
            switch (tipoValor)
            {
                case TipoValorContratoFreteADA.Fixo:
                    return "Fixo";
                case TipoValorContratoFreteADA.Calculado:
                    return "Calculado";
                default:
                    return string.Empty;
            }
        }
    }
}
