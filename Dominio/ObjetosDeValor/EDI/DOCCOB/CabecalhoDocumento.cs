using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.DOCCOB
{
    public class CabecalhoDocumento
    {
        public string IdentificacaoDocumento { get; set; }
        public string Intercambio { get; set; }
        public string Filler { get; set; }
        public List<Transportador> Transportadores { get; set; }
        public Total Total { get; set; }
    }
}
