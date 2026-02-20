using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.UVTRN
{
    public class MDFe
    {
        public int Numero { get; set; }

        public List<Dominio.ObjetosDeValor.EDI.UVTRN.NotaFiscal> NotasFiscais { get; set; }
    }
}
