using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.RetornoOferta
{
    public class DemandaTransporte
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("externalNumber")]
        public string NumeroExterno { get; set; }

        [JsonProperty("externalId")]
        public string IdExterno { get; set; }

        [JsonProperty("temperatureMin")]
        public decimal TemperaturaMinima { get; set; }

        [JsonProperty("temperatureMax")]
        public decimal TemperaturaMaxima { get; set; }

        [JsonProperty("observation")]
        public string Observacao { get; set; }

        [JsonProperty("freightPriceReference")]
        public decimal PrecoFreteReferencia { get; set; }

        [JsonProperty("acceptanceDeadline")]
        public DateTime PrazoAceite { get; set; }

        [JsonProperty("confirmationDeadline")]
        public DateTime PrazoConfirmacao { get; set; }

        [JsonProperty("shipperVehicleTypeDescription")]
        public string DescricaoTipoVeiculoEmbarcador { get; set; }

        [JsonProperty("shipperOperationTypeDescription")]
        public string DescricaoTipoOperacaoEmbarcador { get; set; }

        [JsonProperty("shipperObservation")]
        public string ObservacaoEmbarcador { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("negotiationType")]
        public Tipo TipoNegociacao { get; set; }

        [JsonProperty("offerRecipientType")]
        public Tipo TipoDestinatarioOferta { get; set; }

        [JsonProperty("transportDemandCargos")]
        public List<DemandaTransporteCarga> DemandasTransporteCarga { get; set; }

        [JsonProperty("transportDemandCargoAccessories")]
        public List<AcessorioCargaDemandaTransporte> AcessoriosCargaDemandaTransporte { get; set; }

        [JsonProperty("transportDemandMeasurementUnits")]
        public List<UnidadeMedidaDemandaTransporte> UnidadesMedidaDemandaTransporte { get; set; }
    }
}
