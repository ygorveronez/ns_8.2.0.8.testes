using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.MasterSAF
{
    public class MasterSAF
    {
        public List<DocumentoMasterSAF> Documentos { get; set; }

        public MasterSAF()
        {
            Documentos = new List<DocumentoMasterSAF>();
        }
    }
}
