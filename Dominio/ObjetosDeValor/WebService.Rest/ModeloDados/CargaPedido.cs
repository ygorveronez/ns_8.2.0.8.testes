namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class CargaPedido
    {
        public int Protocolo { get; set; }

        public Cliente Expedidor { get; set; }

        public Cliente Recebedor { get; set; }
    }
}
