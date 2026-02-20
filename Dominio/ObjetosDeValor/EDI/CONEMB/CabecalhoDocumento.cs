using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.CONEMB
{
    public class CabecalhoDocumento
    {
        public string IdentificacaoDocumento { get; set; }
        public string Filler { get; set; }
        public List<Transportador> Transportadores {get;set;}
        public Total Total { get; set; }
    }
}
