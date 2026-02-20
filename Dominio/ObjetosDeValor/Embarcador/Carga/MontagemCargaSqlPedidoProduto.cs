using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class MontagemCargaSqlPedidoProduto
    {

        public MontagemCargaSqlPedidoProduto()
        {
            this.NumeroRegistros = 0;
            this.ListaObjetosPedidoProduto = new List<MontagemCargaPedidoProdutoObjeto>();
        }

        public int NumeroRegistros { get; set; }

        public List<MontagemCargaPedidoProdutoObjeto> ListaObjetosPedidoProduto { get; set; }

    }

    public class MontagemCargaPedidoProdutoObjeto
    {
        public int codigoCargaPedido { get; set; }
        public int codigoPedido { get; set; }
        public int codigoProduto { get; set; }
        public decimal PesoUnitario { get; set; }
        public decimal Quantidade { get; set; }
        public int QuantidadeCaixa { get; set; }
        public int QuantidadeCaixasVazias { get; set; }
        public decimal QuantidadePlanejada { get; set; }
        public decimal PesoTotalEmbalagem { get; set; }
        public decimal ValorProduto { get; set; }
    }

}
