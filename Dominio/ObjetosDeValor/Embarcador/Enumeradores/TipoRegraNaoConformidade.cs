namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRegraNaoConformidade
    {
        ExisteXmlNota = 1,
        CargaCancelada = 2,
        ValidarRaiz = 3,
        ValidarCNPJ = 4,
        StatusSefaz = 5,
        SituacaoCadastral = 6,
        EstendidoFilial = 7,
        Nacionalizacao = 8,
        TipoTomador = 11,
        Tomador = 12,
        RecebedorNotaFiscal = 15,
        LocalEntrega = 17,
        RecebedorArmazenagem = 18,
        Transportador = 19,
        NumeroPedido = 20,
        QuantidadePedido = 21,
        PesoLiquidoTotal = 22,
        ProdutoDePara = 23,
        ProdutoSituacao = 24,
        ProdutoFilial = 25,
        ProdutoFilialRecebedor = 26,
        ProdutoConversaoUnidade = 27,
        CapacidadeExcedida = 28,
        Produto = 29,
    }

    public static class TipoRegraNaoConformidadeHelper
    {
        public static string ObterDescricao(this TipoRegraNaoConformidade item)
        {
            switch (item)
            {
                case TipoRegraNaoConformidade.ExisteXmlNota: return "Existe XML da nota";
                case TipoRegraNaoConformidade.CargaCancelada: return "Carga cancelada";
                case TipoRegraNaoConformidade.ValidarRaiz: return "Validar raiz do CNPJ";
                case TipoRegraNaoConformidade.ValidarCNPJ: return "Validar CNPJ";
                case TipoRegraNaoConformidade.StatusSefaz: return "Status da nota";
                case TipoRegraNaoConformidade.SituacaoCadastral: return "Situação cadastral";
                case TipoRegraNaoConformidade.EstendidoFilial: return "Estendido Filial";
                case TipoRegraNaoConformidade.Nacionalizacao: return "Nacionalização";
                case TipoRegraNaoConformidade.TipoTomador: return "Tipo do tomador";
                case TipoRegraNaoConformidade.Tomador: return "Tomador";
                case TipoRegraNaoConformidade.RecebedorNotaFiscal: return "Recebedor notas fiscais";
                case TipoRegraNaoConformidade.LocalEntrega: return "Local de entrega";
                case TipoRegraNaoConformidade.RecebedorArmazenagem: return "Recebedor armazenagem";
                case TipoRegraNaoConformidade.Transportador: return "Transportador";
                case TipoRegraNaoConformidade.NumeroPedido: return "Número do pedido";
                case TipoRegraNaoConformidade.QuantidadePedido: return "Quantidade de pedidos";
                case TipoRegraNaoConformidade.PesoLiquidoTotal: return "Peso líquido total";
                case TipoRegraNaoConformidade.ProdutoDePara: return "Produto De/Para";
                case TipoRegraNaoConformidade.ProdutoSituacao: return "Produto situação";
                case TipoRegraNaoConformidade.ProdutoFilial: return "Produto filial";
                case TipoRegraNaoConformidade.ProdutoFilialRecebedor: return "Produto filial recebedor";
                case TipoRegraNaoConformidade.ProdutoConversaoUnidade: return "Produto conversão de unidade";
                case TipoRegraNaoConformidade.CapacidadeExcedida: return "Capacidade excedida";
                case TipoRegraNaoConformidade.Produto: return "Produto";
                default: return string.Empty;
            }
        }
    }
}
