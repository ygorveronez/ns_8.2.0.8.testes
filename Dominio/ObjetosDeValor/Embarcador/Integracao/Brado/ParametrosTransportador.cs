using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Brado
{
    public class ParametrosTransportador
    {
        [JsonProperty("pedido_id")]
        public string CodigoPedido { get; set; }

        [JsonProperty("transportador_cnpj")]
        public string CNPJTransportador { get; set; }

        [JsonProperty("quantidade")]
        public int Quantidade { get; set; }

        [JsonProperty("placa_cavalo")]
        public string PlacaCavalo { get; set; }

        [JsonProperty("placa_carreta1")]
        public string PlacaCarreta1 { get; set; }

        [JsonProperty("placa_carreta2")]
        public string PlacaCarreta2 { get; set; }

        [JsonProperty("motorista_nome")]
        public string NomeMotorista { get; set; }

        [JsonProperty("motorista_cpf")]
        public string MotoristaCPF { get; set; }

        [JsonProperty("motorista_cnh")]
        public string MotoristaCNH { get; set; }
    }
}
