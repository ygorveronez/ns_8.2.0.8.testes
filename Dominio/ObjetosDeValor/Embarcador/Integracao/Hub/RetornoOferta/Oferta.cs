using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class Oferta
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("releaseDateStart")]
        public DateTime? DataInicioLiberacao { get; set; }

        [JsonProperty("releaseDateEnd")]
        public DateTime? DataFimLiberacao { get; set; }

        [JsonProperty("freightPriceNegotiated")]
        public decimal? PrecoFreteNegociado { get; set; }

        [JsonProperty("transportDemand")]
        public DemandaTransporte DemandaTransporte { get; set; }

        [JsonProperty("carrier")]
        public Transportadora Transportadora { get; set; }

        [JsonProperty("offerCrews")]
        public List<EquipeOferta> EquipesOferta { get; set; }

        [JsonProperty("offerRiskValidations")]
        public List<ValidacaoRiscoOferta> ValidacoesRiscoOferta { get; set; }

        [JsonProperty("offerVehicles")]
        public List<VeiculoOferta> VeiculosOferta { get; set; }

    }
}
