namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoProvisao
    {
        Nenhum = 0,
        ProvisaoPorNotaFiscal = 1,
        ProvisaoPorCTe = 2,
    }

    public static class TipoProvisaoHelper
    {
        public static string ObterDescricao(this TipoProvisao tipo)
        {
            switch (tipo)
            {
                case TipoProvisao.ProvisaoPorNotaFiscal: return "Provisão por Nota Fiscal";
                case TipoProvisao.ProvisaoPorCTe: return "Provisão por CT-e";
                default: return string.Empty;
            }
        }
    }
}