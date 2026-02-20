using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.ControleEntrega
{
    public class ListaMs
    {
        public string NumeroMs { get; set; }
        public int QtdPallet { get; set; }
        public List<ListaPedidos> ListaShipments { get; set; }
    }
}
