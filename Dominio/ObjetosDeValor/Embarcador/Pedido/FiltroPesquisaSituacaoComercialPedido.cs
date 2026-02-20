namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaSituacaoComercialPedido
    {
        public string CodigoIntegracao { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.SimNao BloqueiaPedido { get; set; }
    }
}
