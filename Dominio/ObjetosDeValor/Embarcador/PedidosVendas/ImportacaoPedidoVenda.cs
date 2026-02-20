using System;

namespace Dominio.ObjetosDeValor.Embarcador.PedidosVendas
{
    public class ImportacaoPedidoVenda
    {
        public string NumeroOrcamento { get; set; }

        public DateTime DataEmissao { get; set; }

        public DateTime DataPrevisao { get; set; }        

        public Dominio.Entidades.Cliente Cliente { get; set; }

        public string Observacao { get; set; }

        public string Referencia { get; set; }

        public string FormaPagamento { get; set; }

        public Dominio.Entidades.Produto Produto { get; set; }

        public string DescricaoItem { get; set; }
        public string NumeroOrdemCompra { get; set; }
        public string NumeroItemOrdemCompra { get; set; }

        public decimal Quantidade { get; set; }

        public decimal ValorUnitario { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
