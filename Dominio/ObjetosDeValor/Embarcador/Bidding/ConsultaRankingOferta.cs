using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class ConsultaRankingOferta
    {
        public int Codigo { get; set; }
        public string RotaDescricao { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string PrimeiroTransportadorValor { get; set; }
        public string PrimeiroTransportadorDescricao { get; set; }
        public string PrimeiroBaseline { get; set; }
        public string SegundoTransportadorValor { get; set; }
        public string SegundoTransportadorDescricao { get; set; }
        public string SegundoBaseline { get; set; }
        public string TerceiroTransportadorValor { get; set; }
        public string TerceiroTransportadorDescricao { get; set; }
        public string TerceiroBaseline { get; set; }
        public string ValorSimulado { get; set; }
        public string TransportadorDescricaoSimulado { get; set; }
        public string BaselineSimulado { get; set; }
        public int CodigoTransportadorSimulado { get; set; }
        public string TransportadoresCalculados { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Bidding.CalculoRankingBidding> TransportadoresCalculadosObjeto
        {
            get
            {
                return JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Bidding.CalculoRankingBidding>>(TransportadoresCalculados ?? string.Empty);
            }
        }
    }
}
