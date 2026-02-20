using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class PedidoVenda
    {
        public int CodigoIntregacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Cliente { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> Produtos { get; set; }
        public decimal Valor { get; set; }
        public string FormaPagamento { get; set; }
        public string Observacao { get; set; }
        public DateTime DataPedido { get; set; }
    }
}
