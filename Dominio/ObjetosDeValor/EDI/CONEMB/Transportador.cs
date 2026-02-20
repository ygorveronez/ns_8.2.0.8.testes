using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.CONEMB
{
    public class Transportador
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public string Filler { get; set; }
        public List<CTeEmbarcado> ConhecimentosEmbarcados { get; set; }
    }
}
