using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.OCOREN
{
    public class EDIOCOREN
    {
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public DateTime Data { get; set; }
        public string Intercambio { get; set; }
        public string Intercambio50 { get; set; }
        public string Filler { get; set; }
        public List<CabecalhoDocumento> CabecalhosDocumento { get; set; }
        public Total Total { get; set; }
        public string CPFCNPJRemetente { get; set; }
        public string CPFCNPJDestinatario { get; set; }
        public string CodigoRemetente { get; set; }
        public string TipoCarga { get; set; }
    }
}
