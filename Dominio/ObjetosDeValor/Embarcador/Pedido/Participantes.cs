namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class Participantes
    {
        public Entidades.Cliente Tomador { get; set; }
        public Entidades.Cliente Recebedor { get; set; }
        public Entidades.Cliente Remetente { get; set; }
        public Entidades.Cliente Destinatario { get; set; }
        public Entidades.Cliente Expedidor { get; set; }
    }
}
