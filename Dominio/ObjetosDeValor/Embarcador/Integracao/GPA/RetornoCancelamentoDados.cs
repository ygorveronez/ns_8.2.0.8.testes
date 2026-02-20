using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GPA
{
    public class RetornoCancelamentoDados
    {
        public int numeroUnidade { get; set; }

        public int numeroCarga { get; set; }

        public List<string> ciots { get; set; }

        public List<CTeCancelamento> ctes { get; set; }

        public List<MDFeCancelamento> mdfes { get; set; }
    }
}
