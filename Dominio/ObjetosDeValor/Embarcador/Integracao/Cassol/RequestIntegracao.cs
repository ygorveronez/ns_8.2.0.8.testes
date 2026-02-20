using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol
{
    public class RequestIntegracao
    {
        [JsonProperty("id")]
        public int NumeroCarga { get; set; }

        [JsonProperty("id_cargas_agrupadas")]
        public string NumeroCargasAgrupadas { get; set; }

        [JsonProperty("filial_saida")]
        public int CodigoFilial { get; set; }

        [JsonProperty("dthr_terminocarregamento")]
        public DateTime DataTerminoCarregamento { get; set; }

        [JsonProperty("dthr_criacao")]
        public DateTime DataCriacaoCarga { get; set; }

        [JsonProperty("cod_box")]
        public string TipoCarregamento { get; set; }

        [JsonProperty("fl_conferidobox")]
        public string BoxConferido { get; set; }

        [JsonProperty("id_usuario")]
        public int CodigoUsuarioMulti { get; set; }

        [JsonProperty("fl_integradomultiembarcador")]
        public string IntegradoMultiEmbarcador { get; set; }

        [JsonProperty("itens_carga")]
        public List<RequestIntegracaoItensCarga> ItensCarga { get; set; }
    }
}
