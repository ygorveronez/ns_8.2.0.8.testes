using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.CargoX
{
    public class RetornoDocumento
    {
        public List<Documento> ctes { get; set; }
        public List<Documento> nfses { get; set; }
        public List<Documento> mdfes { get; set; }

    }
}
