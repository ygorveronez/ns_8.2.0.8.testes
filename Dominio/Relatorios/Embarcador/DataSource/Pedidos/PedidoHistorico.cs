using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class PedidoHistorico
    {
        public long Codigo { get; set; }
        public string Usuario { get; set; }
        public DateTime DataHora { get; set; }
        public string Propriedade { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorAtual { get; set; }
    }
}
