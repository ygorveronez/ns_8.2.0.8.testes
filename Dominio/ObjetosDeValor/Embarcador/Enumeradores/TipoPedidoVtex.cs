namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPedidoVtex
    {
        // 1 - Ship from stores: Pedidos saem de uma filial da Decatlhon diretamente sem ser pela matriz onde está o estoque principal
        ShipFromStores = 1,
        
        // 2 - RNL Normal: Pedido que sai da matriz estoque por carona na transferência e o cliente retira na loja filial da Decathlon
        RNLNormal = 2,

        // 3 - RNL Express: o cliente pega direto na loja filial a Decatlhon
        RNLExpress = 3,

        // 4 - Market Place IN: Produtos que outras marcas poderão usar o canal Decatlhon (IGNORAR ESSES PEDIDOS)
        MarketPlaceIn = 4,

        // 5 - Market Place OUT: Entrega Decatlhon de Produtos Mercado Livre ou outros
        MarketPlaceOut = 5,

        // 6 - E-Commercer Normal: que é o que já desenhamos no projeto
        ECommerceNormal = 6,

        // 7 - Clique Retire
        CliqueRetire = 7
    }

    public static class TipoPedidoVtexHelper
    {
        public static string ObterDescricao(this TipoPedidoVtex tipo)
        {
            switch (tipo)
            {
                case TipoPedidoVtex.ShipFromStores: return "Ship from Stores";
                case TipoPedidoVtex.RNLNormal: return "RNL Normal";
                case TipoPedidoVtex.RNLExpress: return "RNL Express";
                case TipoPedidoVtex.MarketPlaceIn: return "Market Place IN";
                case TipoPedidoVtex.MarketPlaceOut: return "Market Place OUT";
                case TipoPedidoVtex.ECommerceNormal: return "E-Commerce Normal";
                case TipoPedidoVtex.CliqueRetire: return "Clique Retire";
                default: return string.Empty;
            }
        }
    }
}
