namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoImportacaoPedido
    {
        Todas = 0,
        Pendente = 1,
        Processando = 2,
        Sucesso = 3,
        Erro = 4,
        Cancelado = 5
    }

    public static class SituacaoImportacaoPedidoHelper
    {
        public static string ObterDescricao(this SituacaoImportacaoPedido situacao)
        {
            switch (situacao)
            {
                case SituacaoImportacaoPedido.Todas:
                    return "Todas";
                case SituacaoImportacaoPedido.Pendente:
                    return "Pendente";
                case SituacaoImportacaoPedido.Processando:
                    return "Processando";
                case SituacaoImportacaoPedido.Sucesso:
                    return "Sucesso";
                case SituacaoImportacaoPedido.Erro:
                    return "Erro";
                case SituacaoImportacaoPedido.Cancelado:
                    return "Cancelado";
                default:
                    return string.Empty;
            }
        }
    }
}
