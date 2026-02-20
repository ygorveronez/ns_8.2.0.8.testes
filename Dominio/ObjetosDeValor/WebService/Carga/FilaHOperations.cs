using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class FilaHOperations
    {
        public int type { get; set; }
        public List<DocumentKey> documentsKey { get; set; }
        public List<DocumentNumber> documentsNumber { get; set; }
    }

    public class DocumentKey
    {
        public string documentKey { get; set; }
    }

    public class DocumentNumber
    {
        public string documentNumber { get; set; }
    }
}
