using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.NovoApp.Cargas
{
    public class ResponseDocumentosTransporteCarga
    {
        public List<DocumentoFiscal> CTes { get; set; }
        public List<DocumentoFiscal> NFSe { get; set; }
        public List<DocumentoFiscal> Outros { get; set; }
        public List<DocumentoFiscal> MDFes { get; set; }
        public List<ValePedagio> ValePedagios { get; set; }
        public List<CIOT> CIOTs { get; set; }

    }
}
