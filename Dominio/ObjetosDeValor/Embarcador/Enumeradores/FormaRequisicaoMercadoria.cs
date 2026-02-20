namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormaRequisicaoMercadoria
    {
        GerarPeloEstoque = 0,
        Estoque = 1,
        Compra = 2
    }

    public static class FormaRequisicaoMercadoriaHelper
    {
        public static string ObterDescricao(this FormaRequisicaoMercadoria forma)
        {
            switch (forma)
            {
                case FormaRequisicaoMercadoria.GerarPeloEstoque: return "Gerar pelo estoque";
                case FormaRequisicaoMercadoria.Estoque: return "Estoque";
                case FormaRequisicaoMercadoria.Compra: return "Compra";
                default: return string.Empty;
            }
        }
    }
}
