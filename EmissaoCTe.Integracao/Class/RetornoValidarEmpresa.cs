using System.Collections.Generic;

namespace EmissaoCTe.Integracao
{
    public class RetornoValidarEmpresa
    {
        public Dominio.Enumeradores.TipoAmbiente Ambiente { get; set; }

        public List<string> Inconsistencias { get; set; }
    }
}