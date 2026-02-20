using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda
{
    public partial class DemandaTransporte
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("externalId")]
        public string IdExterno { get; set; }

        [JsonProperty("externalNumber")]
        public string NumeroExterno { get; set; }

        [JsonProperty("loadingDate")]
        public DateTimeOffset? DataCarregamento { get; set; }

        [JsonProperty("deliveryDate")]
        public DateTimeOffset? DataEntrega { get; set; }

        [JsonProperty("cargo")]
        public Carga Carga { get; set; }

        [JsonProperty("cargoType")]
        public Tipo TipoCarga { get; set; }

        [JsonProperty("negotiationType")]
        public Tipo TipoNegociacao { get; set; }

        [JsonProperty("offerRecipientType")]
        public Tipo TipoDestinatarioOferta { get; set; }

        [JsonProperty("temperatureMin")]
        public long TemperaturaMinima { get; set; }

        [JsonProperty("temperatureMax")]
        public long TemperaturaMaxima { get; set; }

        [JsonProperty("freightPriceReference")]
        public decimal PrecoFreteReferencia { get; set; }

        [JsonProperty("acceptanceDeadline")]
        public DateTimeOffset? PrazoAceite { get; set; }

        [JsonProperty("confirmationDeadline")]
        public DateTimeOffset? PrazoConfirmacao { get; set; }

        [JsonProperty("shipperVehicleTypeDescription")]
        public string DescricaoTipoVeiculoRemetente { get; set; }

        [JsonProperty("shipperOperationTypeDescription")]
        public string DescricaoTipoOperacaoRemetente { get; set; }

        [JsonProperty("shipperObservation")]
        public string Observacao { get; set; }

        [JsonProperty("shipperDate")]
        public DateTime? DataRemetente { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("accessories")]
        public List<Tipo> Acessorios { get; set; }

        [JsonProperty("measurementUnits")]
        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.UnidadeMedida> UnidadesMedida { get; set; }

        [JsonProperty("routes")]
        public List<Rota> Rotas { get; set; }

        [JsonProperty("offers")]
        public List<Oferta> Ofertas { get; set; }

        [JsonProperty("hazardClasses")]
        public List<ClasseRisco> ClassesRisco { get; set; }
    }

}
