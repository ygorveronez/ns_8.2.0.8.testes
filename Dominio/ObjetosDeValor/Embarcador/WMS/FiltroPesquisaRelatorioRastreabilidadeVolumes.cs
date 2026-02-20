using System;

namespace Dominio.ObjetosDeValor.Embarcador.WMS
{
    public class FiltroPesquisaRelatorioRastreabilidadeVolumes
    {
        public string NumeroPedido { get; set; }
        public int Carga { get; set; }
        public int ProdutoEmbarcador { get; set; }
        public DateTime DataPedidoInicial { get; set; }
        public DateTime DataPedidoFinal { get; set; }
        public DateTime DataRecebimentoInicial { get; set; }
        public DateTime DataRecebimentoFinal { get; set; }
        public bool TodosCNPJdaRaizEmbarcador { get; set; }
    }
}
