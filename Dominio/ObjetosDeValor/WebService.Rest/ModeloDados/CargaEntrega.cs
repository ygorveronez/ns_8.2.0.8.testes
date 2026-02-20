using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class CargaEntrega
    {
        public int ProtocoloCarga { get; set; }

        public IEnumerable<Entrega> Entregas { get; set; }
    }
}
