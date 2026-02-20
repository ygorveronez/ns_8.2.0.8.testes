namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class TipoOperacaoRetornoControleEntrega
    {
        public int CodigoCarga { get; set; }
        public bool PermiteChat { get; set; }
        public string Descricao { get; set; }
        public bool PermiteAdicionarColeta { get; set; }
        public bool EnviarBoletimDeViagemAoFinalizarViagem { get; set; }
        public bool PermiteAdicionarPedidoReentregaAposInicioViagem { get; set; }
    }
}
