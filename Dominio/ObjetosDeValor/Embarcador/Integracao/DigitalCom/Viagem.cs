using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom
{
    public class Viagem
    {
        [JsonProperty("numeroViagem")]
        public string NumeroViagem { get; set; }

        [JsonProperty("meioPagamento")]
        public string MeioPagamento { get; set; }

        [JsonProperty("idRecibo")]
        public string IdRecibo { get; set; }

        [JsonProperty("dataCompra")]
        public string DataCompra { get; set; }

        [JsonProperty("dataViagem")]
        public string DataViagem { get; set; }

        [JsonProperty("inicioVigencia")]
        public string InicioVigencia { get; set; }

        [JsonProperty("dataExpedicao")]
        public string DataExpedicao { get; set; }

        [JsonProperty("fimVigencia")]
        public string FimVigencia { get; set; }

        [JsonProperty("statusCompra")]
        public Status StatusCompra { get; set; }

        [JsonProperty("statusRecibo")]
        public Status StatusRecibo { get; set; }

        [JsonProperty("nomeEmissor")]
        public string NomeEmissor { get; set; }

        [JsonProperty("cnpjEmissor")]
        public string CNPJEmissor { get; set; }

        [JsonProperty("totalViagem")]
        public decimal? TotalViagem { get; set; }

        [JsonProperty("nroEixo")]
        public int NumeroEixos { get; set; }

        [JsonProperty("nomeRota")]
        public string NomeRota { get; set; }

        [JsonProperty("nomeTransportador")]
        public string NomeTransportador { get; set; }

        [JsonProperty("cnpjTransportador")]
        public string CNPJTransportador { get; set; }

        [JsonProperty("placaCavaloTransportador")]
        public string PlacaCavaloTransportador { get; set; }

        [JsonProperty("categoriaTransportador")]
        public string CategoriaTransportador { get; set; }

        [JsonProperty("tagTransportador")]
        public string TAGTransportador { get; set; }

        [JsonProperty("distancia")]
        public decimal Distancia { get; set; }

        [JsonProperty("recibo")]
        public string Recibo { get; set; }

        [JsonProperty("eixoSuspenso")]
        public bool EixoSuspenso { get; set; }

        [JsonProperty("idVpoAntt")]
        public string IdVpoAntt { get; set; }
    }
}
