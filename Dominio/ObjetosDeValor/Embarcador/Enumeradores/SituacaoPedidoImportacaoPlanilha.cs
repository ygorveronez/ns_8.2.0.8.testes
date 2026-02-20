namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPedidoImportacaoPlanilha
    {
        GerandoPedidos = 0,
        GerandoCargas = 1,
        Falha = 2,
        Finalizado = 3
    }

    public static class SituacaoPedidoImportacaoPlanilhaHelper
    {
        public static string ObterDescricao(this SituacaoPedidoImportacaoPlanilha situacao)
        {
            switch (situacao)
            {
                case SituacaoPedidoImportacaoPlanilha.GerandoPedidos: return "Gerando os pedidos";
                case SituacaoPedidoImportacaoPlanilha.GerandoCargas: return "Gerando as cargas";
                case SituacaoPedidoImportacaoPlanilha.Falha: return "Falha na importação";
                case SituacaoPedidoImportacaoPlanilha.Finalizado: return "Finalizado";
                default: return "";
            }
        }
    }
}
