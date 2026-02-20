using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoTrocaAlvoPosicoes
    {
        public long CodigoPosicao { get; set; }
        public DateTime? DataVeiculoPosicao { get; set; }
        public double? LatitudePosicao { get; set; }
        public double? LongitudePosicao { get; set; }
        public bool? EmAlvoPosicao { get; set; }
        public bool? EmLocalPosicao { get; set; }
        public string CodigosClientesAlvoPosicao { get; set; }
        public string CodigosSubareasAlvoPosicao { get; set; }
        public string CodigosLocaisPosicao { get; set; }
    }
}
