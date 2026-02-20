using System;

namespace Dominio.ObjetosDeValor.Embarcador.Fechamento
{
    public class SumFranquia
    {
        public int TotalKMFranquia { get; set; }

        public int TotalKMRealizado { get; set; }

        public int TotalKMExcedido { get; set; }

        public decimal ValorKMFranquia { get; set; }

        public decimal ValorKMExcedido { get; set; }

        public decimal ValorTotalKMFranquia
        {
            get
            {
                return Math.Round(TotalKMFranquia * ValorKMFranquia, 2, MidpointRounding.AwayFromZero);
            }
        }

        public decimal ValorTotalKMExcedido
        {
            get
            {
                return TotalKMExcedido * ValorKMExcedido;
            }
        }

        public decimal Total
        {
            get
            {
                return ValorTotalKMFranquia + ValorTotalKMExcedido;
            }
        }
    }
}
