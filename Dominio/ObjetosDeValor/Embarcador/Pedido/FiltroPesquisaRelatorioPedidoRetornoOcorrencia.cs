using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class FiltroPesquisaRelatorioPedidoRetornoOcorrencia
    {
        public string NumeroPedido { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
    }
}
