namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class DadosPortalRetiraPedidoProduto
    {
        public int Codigo { get; set; }

        //#35952
        public decimal Quantidade { get; set; }
    }
}
