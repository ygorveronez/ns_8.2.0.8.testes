using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCargaStatus
{
    public class Status
    {
        /// <summary>
        /// Número identificador da viagem
        /// </summary>
        [JsonProperty("TripIdentifier")]
        public string NumeroCarga { get; set; }

        /// <summary>
        /// Número do protocolo da carga
        /// </summary>
        [JsonProperty("ShipperRequestNumber")]
        public int Protocolo { get; set; }

        /// <summary>
        /// CNPJ Cliente ou Cod Integração
        /// </summary>
        [JsonProperty("StopIdentifier")]
        public string IdentificadorParada { get; set; }

        /// <summary>
        /// Quantidade total carregada 
        /// </summary>
        [JsonProperty("Volume")]
        public decimal Volume { get; set; }

        /// <summary>
        /// Codigo de status de viagem * 
        /// RC - Reserva criada
        /// VC - Viagem criada
        /// VF - Viagem faturada
        /// EN - Entregue
        /// CF - Em cancelamento fiscal
        /// DV - Documento de transporte em devolução
        /// VE - Viagem eliminada
        /// </summary>
        [JsonProperty("SituacaoViagem")]
        public string SituacaoViagem { get; set; }

        /// <summary>
        /// Receber a URL do xml da nota fiscal
        /// </summary>
        [JsonProperty("UrlXml")]
        public List<UrlXml> UrlXml { get; set; }
    }
}
