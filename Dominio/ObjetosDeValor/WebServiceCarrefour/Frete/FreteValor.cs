using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Frete
{
    public sealed class FreteValor
    {
        public decimal FreteProprio { get; set; }

        public List<ComponenteAdicional> ComponentesAdicionais { get; set; }

        public ICMS.ICMS ICMS { get; set; }

        public ISS.ISS ISS { get; set; }

        public decimal ValorTotalAReceber { get; set; }

        public decimal ValorPrestacaoServico { get; set; }

        [Obsolete]
        public decimal FreteFilialEmissora { get; set; }
    }
}
