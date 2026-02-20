using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class MontagemCargaFilaPedidoProdutosCarregamento
    {
        public MontagemCargaFilaPedidoProdutosCarregamento()
        {
            this.DataPedido = DateTime.Now.Date;
        }

        public int CodigoCanalEntrega { get; set; }
        public int PrioridadeCanalEntrega { get; set; }
        public int CodigoLinhaSeparacao { get; set; }
        public int PrioridadeLinhaSeparacao { get; set; }
        public int CodigoEnderecoProduto { get; set; }
        public string EnderecoProduto { get; set; }
        public int PrioridadeEnderecoProduto { get; set; }
        public int CodigoPedido { get; set; }
        public DateTime DataPedido { get; set; }
        public decimal PesoTotal { get; set; }
        public decimal Pallet { get; set; }
        public decimal Metro { get; set; }
    }
}
