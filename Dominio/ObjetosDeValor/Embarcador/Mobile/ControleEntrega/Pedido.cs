using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class Pedido
    {
        public int Codigo { get; set; }

        public string NumeroPedido { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> Produtos { get; set; }
    }
}
