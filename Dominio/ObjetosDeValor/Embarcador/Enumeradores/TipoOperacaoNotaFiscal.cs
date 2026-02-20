namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOperacaoNotaFiscal
    {
        Entrada = 0,
        Saida = 1
    }


    public static class HelperTipoOperacaoNotaFiscal
    {
        public static TipoOperacaoNotaFiscal getTipoOperacaoNotaFiscal(int tipo)
        {
            if (tipo == 0)
                return TipoOperacaoNotaFiscal.Entrada;
            else
                return TipoOperacaoNotaFiscal.Saida;
        }
    }
}
