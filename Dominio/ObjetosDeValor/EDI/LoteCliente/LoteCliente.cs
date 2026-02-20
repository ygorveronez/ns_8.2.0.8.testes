using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.LoteCliente
{
    public class LoteCliente
    {
        public Cabecalho Cabecalho { get; set; }
        public List<Cliente> Cliente { get; set; }
        public Rodape Rodape { get; set; }
    }
}
