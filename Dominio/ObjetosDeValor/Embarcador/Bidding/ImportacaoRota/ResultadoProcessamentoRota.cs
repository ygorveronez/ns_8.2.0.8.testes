using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding.ImportacaoRota
{
    public class ResultadoProcessamentoRota
    {
        public Entidades.Embarcador.Bidding.BiddingOfertaRota BiddingOfertaRota { get; set; }
        public List<Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPOrigem> CEPsOrigem { get; set; } = new();
        public List<Entidades.Embarcador.Bidding.BiddingOfertaRotaCEPDestino> CEPsDestino { get; set; } = new();
        public List<Entidades.Embarcador.Bidding.Baseline> Baselines { get; set; } = new();
    }
}
