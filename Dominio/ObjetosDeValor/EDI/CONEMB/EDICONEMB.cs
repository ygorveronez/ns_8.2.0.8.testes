using System;

namespace Dominio.ObjetosDeValor.EDI.CONEMB
{
    public class EDICONEMB
    {
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public DateTime Data { get; set; }
        public string Intercambio { get; set; }
        public string Filler { get; set; }
        public CabecalhoDocumento CabecalhoDocumento { get; set; }
    }
}
