using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class OrdemDeCompra
    {
        [DataMember(Name = "iDoc")]
        public int iDoc { get; set; }

        [DataMember(Name = "productOrders")]
        public List<OrdemProduto> productOrders { get; set; }
    }
}
