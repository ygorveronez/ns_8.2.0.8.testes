using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FreteValor
    {
        public decimal FreteProprio { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> ComponentesAdicionais { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS ICMS { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.ISS.ISS ISS { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.IBSCBS.IBSCBS IBSCBS { get; set; }
        public decimal ValorTotalAReceber { get; set; }
        public decimal ValorTotalDocumentoFiscal { get; set; }
        public decimal ValorPrestacaoServico { get; set; }
        public decimal ValorAReceberSemImpostoIncluso { get; set; }
        [Obsolete]
        public decimal FreteFilialEmissora { get; set; }
        public decimal ValorCalculadoPelaTabela { get; set; }
    }
}
