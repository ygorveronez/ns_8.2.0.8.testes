using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga.Resumo
{
    public class Pedido
    {
        public string NumeroPedidoEmbarcador { get; set; }
        public int Protocolo { get; set; }
        public List<Documento> CTe { get; set; }
        public List<Documento> NFSe { get; set; }
    }
}
