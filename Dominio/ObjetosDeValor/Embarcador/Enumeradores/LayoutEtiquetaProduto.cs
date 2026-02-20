namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum LayoutEtiquetaProduto
    {
        QrCode = 1,
        CodigoBarras = 2, 
    }

    public static class LayoutEtiquetaProdutoHelper
    {
        public static string ObterDescricao(this LayoutEtiquetaProduto tipo)
        {
            switch (tipo)
            {
                case LayoutEtiquetaProduto.QrCode: return "Qr Code";
                case LayoutEtiquetaProduto.CodigoBarras: return "Código de Barras";                
                default: return string.Empty;
            }
        }
    }
}
