using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever
{
    public class Pedido
    {
        public string ProtocoloPedido { get; set; }
        public List<Stage> Stage { get; set; }
        public TipoPercurso TipoPercurso { get; set; }
    }
}
