namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaFluxoCompra
    {
        Todos = 0,
        Requisicao = 1,
        AprovacaoRequisicao = 2,
        Cotacao = 3,
        RetornoCotacao = 4,
        OrdemCompra = 5,
        AprovacaoOrdemCompra = 6,
        RecebimentoProduto = 7
    }

    public static class EtapaFluxoCompraHelper
    {
        public static string ObterDescricao(this EtapaFluxoCompra situacao)
        {
            switch (situacao)
            {
                case EtapaFluxoCompra.Requisicao: return "Requisição";
                case EtapaFluxoCompra.AprovacaoRequisicao: return "Aprovação da Requisição";
                case EtapaFluxoCompra.Cotacao: return "Cotação";
                case EtapaFluxoCompra.RetornoCotacao: return "Retorno da Cotação";
                case EtapaFluxoCompra.OrdemCompra: return "Ordem de Compra";
                case EtapaFluxoCompra.AprovacaoOrdemCompra: return "Aprovação Ordem de Compra";
                case EtapaFluxoCompra.RecebimentoProduto: return "Recebimento do Produto";
                default: return string.Empty;
            }
        }
    }
}
