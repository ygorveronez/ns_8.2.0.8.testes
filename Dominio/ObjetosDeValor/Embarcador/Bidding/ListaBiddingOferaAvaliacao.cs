using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class ListaBiddingOfertaAvaliacao
    {
        public int CodigoTransportador { get; set; }
        public int CodigoRota { get; set; }
        public TipoLanceBidding TipoOferta { get; set; }
        public decimal CustoEstimado { get; set; }
        public bool NaoOfertar { get; set; }
    }
}