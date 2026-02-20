namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class EmissaoCTePortoLoteCargaPedido
    {
        public int Codigo { get; set; }
        public string Carga { get; set; }
        public string NumeroBooking { get; set; }
        public string VVD { get; set; }
        public string PortoOrigem { get; set; }
        public string PortoDestino { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string MensagemRetorno { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCarga { get; set; }
    }
}
