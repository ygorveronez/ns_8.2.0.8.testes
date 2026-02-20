using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class InformacaoCarga
    {
        public decimal ValorTotalCarga { get; set; }
        public string ProdutoPredominante { get; set; }
        public decimal ValorCargaAverbacao { get; set; }
        public string Container { get; set; }
        public string NumeroLacreContainer { get; set; }
        public DateTime DataEntregaContainer { get; set; }
        public string OutrasCaracteristicas { get; set; }
        public string CaracteristicaServico { get; set; }
        public string CaracteristicaTransporte { get; set; }
    }
}
