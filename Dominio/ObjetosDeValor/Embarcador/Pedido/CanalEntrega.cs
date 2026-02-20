namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class CanalEntrega
    {
        public int Codigo { get; set; }
        public bool LiberarPedidoSemNFeAutomaticamente { get; set; }
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        public Embarcador.Filial.Filial Filial { get; set; }

    }
}
