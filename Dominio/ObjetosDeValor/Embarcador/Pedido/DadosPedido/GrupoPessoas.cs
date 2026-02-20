namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class GrupoPessoas
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public bool TornarPedidosPrioritarios { get; set; }

        public int? DiasDePrazoFatura { get; set; }

        public TipoOperacao TipoOperacaoColeta { get; set; }

        public TipoPagamentoRecebimento FormaPagamento { get; set; }
    }
}
