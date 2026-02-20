using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.VGM
{
    public class VGM
    {
        public CabecalhoUNB CabecalhoUNB { get; set; }
        public CabecalhoUNH CabecalhoUNH { get; set; }
        public CabecalhoBGM CabecalhoBGM { get; set; }

        public List<Container> Containeres { get; set; }

        public RodapeUNT RodapeUNT { get; set; }
        public RodapeUNZ RodapeUNZ { get; set; }
    }
}
