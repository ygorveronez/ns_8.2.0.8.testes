using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.MDFe
{
    public class MunicipioDescarregamento
    {
        public int CodigoIBGE { get; set; }

        public List<DocumentoMunicipioDescarregamento> Documentos { get; set; }
    }
}
