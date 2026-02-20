using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GPA
{
    public class RetornoMDFeDados
    {
        public int numeroUnidade { get; set; }

        public int numeroCarga { get; set; }

        public List<string> ciots { get; set; }

        public List<MDFe> mdfes { get; set; }
    }
}
