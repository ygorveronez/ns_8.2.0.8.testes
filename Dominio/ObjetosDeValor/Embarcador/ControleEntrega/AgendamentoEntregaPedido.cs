namespace Dominio.ObjetosDeValor.Embarcador.ControleEntrega
{
    public class AgendamentoEntregaPedido
    {
        public string order_container { get; set; }
        public string address_code { get; set; }
        public string address_type{ get; set; }
        public string suggested_scheduling_date { get; set; }
    }
}
