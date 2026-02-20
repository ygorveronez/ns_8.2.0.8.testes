namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ModoRequisicaoMercadoria
    {
        Todos = 0,
        Requisicao = 1,
        Compra = 2
    }

    public static class ModoRequisicaoMercadoriaHelper
    {
        public static string ObterDescricao(this ModoRequisicaoMercadoria modo)
        {
            switch (modo)
            {
                case ModoRequisicaoMercadoria.Requisicao: return "Requisição";
                case ModoRequisicaoMercadoria.Compra: return "Compra";
                default: return "";
            }
        }
    }
}
