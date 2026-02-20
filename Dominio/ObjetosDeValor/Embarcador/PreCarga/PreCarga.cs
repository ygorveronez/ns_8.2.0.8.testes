using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.PreCarga
{
    public class PreCarga
    {
        public string NumeroPreCarga { get; set; }
        public Filial.Filial Filial { get; set; }
        public int NumeroPallets { get; set; }
        public decimal PesoTotal { get; set; }
        public decimal CubagemTotal { get; set; }
        public List<Embarcador.Pedido.Produto> Produtos { get; set; }
        public Carga.TipoCargaEmbarcador TipoCargaEmbarcador { get; set; }
        public Carga.ModeloVeicular ModeloVeicular { get; set; }
    }
}
