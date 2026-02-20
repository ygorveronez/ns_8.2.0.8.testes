using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class BiddingTransportadorRotaOfertaDados
    {
        public int CodigoTransportadorRota { get; set; }
        public decimal CustoEstimado { get; set; }
        public decimal FreteTonelada { get; set; }
        public decimal AdicionalPorEntrega { get; set; }
        public decimal Ajudante { get; set; }
        public decimal PedagioParaEixo { get; set; }
        public int VeiculosVerdes { get; set; }
        public bool? NaoOfertar { get; set; }
        public TipoLanceBidding TipoLance { get; set; }
    }
}
