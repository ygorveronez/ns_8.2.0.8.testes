namespace Dominio.ObjetosDeValor.Embarcador.ControleEntrega
{
    public class ListaPedidos
    {
        public string NumeroShipment { get; set; }
        public string Cliente { get; set; }
        public double Peso { get; set; }
        public int QtdCaixas { get; set; }
        public int Parada { get; set; }
    }
}
