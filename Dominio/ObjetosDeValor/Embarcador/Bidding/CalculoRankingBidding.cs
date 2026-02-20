using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class CalculoRankingBidding
    {
        public int CodigoTransportador { get; set; }
        public string Transportador { get; set; }
        public decimal Valor { get; set; }
        public string Baseline { get; set; }
        public decimal BaselineCalculado { get; set; }
    }
}
