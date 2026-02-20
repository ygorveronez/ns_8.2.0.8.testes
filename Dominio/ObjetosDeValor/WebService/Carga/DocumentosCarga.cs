using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class DocumentosCarga
    {
        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> CTes { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.MDFe.MDFe> MDFes { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.NFS.NFS> NFSes { get; set; }
    }
}
