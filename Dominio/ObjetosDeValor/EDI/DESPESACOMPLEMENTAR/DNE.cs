using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.DESPESACOMPLEMENTAR
{
    public class DNE
    {
        public string registro { get; set; }
        public List<Nota> Notas { get; set; }

        public Despesa Despesa { get; set; }
    }
}
