using System;

namespace Dominio.Relatorios.Embarcador.DataSource.PedidosVendas
{
    public class RelatorioPedidoVendaParcela
    {
        public int Sequencia { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
    }
}