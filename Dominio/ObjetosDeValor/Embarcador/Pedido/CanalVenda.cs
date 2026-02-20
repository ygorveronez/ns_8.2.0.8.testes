namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class CanalVenda
    {
        public string Descricao { get; set; }

        public string CodigoIntegracao { get; set; }

        public Embarcador.Filial.Filial Filial { get; set; }
    }
}