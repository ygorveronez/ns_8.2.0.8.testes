namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRegistroPermutaPallet
    {
        Documento = 0,
        Comprovante = 1,
    }

    public static class TipoRegistroPermutaPalletHelper
    {
        public static string ObterDescricao(this TipoRegistroPermutaPallet tipo)
        {
            switch (tipo)
            {
                case TipoRegistroPermutaPallet.Documento: return "Documento";
                case TipoRegistroPermutaPallet.Comprovante: return "Comprovante de pagamento";
                default: return string.Empty;
            }
        }
    }

}