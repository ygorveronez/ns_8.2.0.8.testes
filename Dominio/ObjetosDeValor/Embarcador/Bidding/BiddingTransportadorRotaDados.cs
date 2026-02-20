using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class BiddingTransportadorRotaDados
    {
        public int Codigo { get; set; }
        public string Transportador { get; set; }
        public int TransportadorCodigo { get; set; }
        public int RotaCodigo { get; set; }
        public string RotaDescricao { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public StatusBiddingRota Status { get; set; }
        public int Ranking { get; set; }
        public int Rodada { get; set; }
        public decimal Target { get; set; }
        public DateTime? DataRetorno { get; set; }
    }
}
