using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.PREFAT
{
    public class CabecalhoIntercambio
    {
        public string IdentificacaoDocumento { get; set; }
        public string IdentificacaoRemetente { get; set; }
        public string IdentificacaoDestinatario { get; set; }
        public string Data { get; set; }
        public string Hora { get; set; }
        public string IdentificacaoIntercambio { get; set; }
        public string Filler { get; set; }
        public List<CabecalhoDocumento> CabecalhosDocumentos { get; set; }
        public List<PreFatura> PreFaturas { get; set; }
        public Total Total { get; set; }
    }
}
