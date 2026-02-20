using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.DOCCOB
{
    public class Transportador
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public string CNPJSacado { get; set; }
        public string IESacado { get; set; }
        public string Filler { get; set; }
        public List<DocumentoCobranca> DocumentosCobranca { get; set; }
    }
}
