namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet
{
    public sealed class TotalizadoresEnvioDevolucaoPallets
    {
        public int Todos { get; set; }
        public int Pendente { get; set; }
        public int Concluido { get; set; }
        public int Cancelado { get; set; }
        public int Reserva { get; set; }
        public int AguardandoAvaliacao { get; set; }
    }
}
