namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemSituacaoDigitalizacaoCanhoto
    {
        OCR = 1,
        Manual = 2,
        IA = 3
    }

    public static class OrigemSituacaoDigitalizacaoCanhotoHelper
    {
        public static string ObterDescricao(this OrigemSituacaoDigitalizacaoCanhoto origemSituacaoDigitalizacaoCanhoto)
        {
            switch (origemSituacaoDigitalizacaoCanhoto)
            {
                case OrigemSituacaoDigitalizacaoCanhoto.OCR: return "OCR";
                case OrigemSituacaoDigitalizacaoCanhoto.Manual: return "Manual";
                case OrigemSituacaoDigitalizacaoCanhoto.IA: return "IA";
                default: return "";
            }
        }
    }
}