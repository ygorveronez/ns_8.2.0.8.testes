namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoSessaoRoteirizadorPedido
    {
        OK = 0,
        PesoMaiorCapacidadeVeicular = 1,
        AusenciaDeDisponibilidadeDeFrota = 2,
        CentroDescarregamentoSemVeiculoPermitido = 3,
        RemovidoPedidoSessao = 4,
        LinhaSeparacaoProdutoNaoRoteiriza = 5,
        ProdutoCarregadoTotalmente = 6,
        PedidoNaoPermiteQuebraMultiplosCarregamentos = 7,
        ProdutoPedidoPalletFechado = 8,
        ProdutoPedidoPalletAberto = 9,
        LimitePorTipoCargaModeloNaoAtingido = 10,
        AusenciaDeDisponibilidadeDeFrotaParaGrupoProduto = 11,
        NaoAtendeuCapacidadeModeloVeicular = 12,
        ProdutosPedidoNaoAtendemControleEstoqueSolicitado = 13
    }

    public static class SituacaoSessaoRoteirizadorPedidoHelper
    {
        public static string ObterDescricao(this SituacaoSessaoRoteirizadorPedido situacao)
        {
            switch (situacao)
            {
                case SituacaoSessaoRoteirizadorPedido.OK: return "OK";
                case SituacaoSessaoRoteirizadorPedido.PesoMaiorCapacidadeVeicular: return "O pedido possui peso superior a capacidade máxima de carregamento dos veículos.";
                case SituacaoSessaoRoteirizadorPedido.AusenciaDeDisponibilidadeDeFrota: return "Ausência de disponibilidade de frota.";
                case SituacaoSessaoRoteirizadorPedido.CentroDescarregamentoSemVeiculoPermitido: return "Centro descarregamento do cliente não encontrado ou sem cadastro de \"Veículos permitidos\".";
                case SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao: return "Pedido removido da sessão de roteirização";
                case SituacaoSessaoRoteirizadorPedido.LinhaSeparacaoProdutoNaoRoteiriza: return "Linha de separação do pedido produto configurado para não roteirizar.";
                case SituacaoSessaoRoteirizadorPedido.ProdutoCarregadoTotalmente: return "Produto do pedido carregado totalmente.";
                case SituacaoSessaoRoteirizadorPedido.PedidoNaoPermiteQuebraMultiplosCarregamentos: return "Pedido não permite quebra em multiplos carregamentos e excede a capacidade do veículo.";
                case SituacaoSessaoRoteirizadorPedido.ProdutoPedidoPalletFechado: return "Roteirização por Pallet Aberto e o Pedido possui produto de pallet fechado.";
                case SituacaoSessaoRoteirizadorPedido.ProdutoPedidoPalletAberto: return "Roteirização por Pallet Fechado e o Pedido possui produto de pallet aberto.";
                case SituacaoSessaoRoteirizadorPedido.LimitePorTipoCargaModeloNaoAtingido: return "Limite de pedidos por tipo de carga e modelo veicular não atingidos.";
                case SituacaoSessaoRoteirizadorPedido.AusenciaDeDisponibilidadeDeFrotaParaGrupoProduto: return "Ausência de disponibilidade de frota para o grupo de produtos.";
                case SituacaoSessaoRoteirizadorPedido.NaoAtendeuCapacidadeModeloVeicular: return "Não atendeu a capacidade do modelo veicular no carregamento.";
                case SituacaoSessaoRoteirizadorPedido.ProdutosPedidoNaoAtendemControleEstoqueSolicitado: return "Produtos do pedido não atendem controle de estoque solicitado.";
                default: return string.Empty;
            }
        }
    }

}
