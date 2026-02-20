namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PadraoTempoDiasMinutos
    {
        Minutos = 1,
        Dias = 2
    }
    public static class PadraoTempoDiasMinutosHelper
    {
        public static PadraoTempoDiasMinutos ConverterImportacaoEnumerador(string situacao)
        {
            switch (situacao)
            {
                case "1": return PadraoTempoDiasMinutos.Minutos;
                case "2": return PadraoTempoDiasMinutos.Dias;
                default: return PadraoTempoDiasMinutos.Minutos;
            }
        }
    }
}
