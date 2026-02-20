using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Krona
{
    public sealed class Viagem
    {
        [JsonProperty(PropertyName = "doca_origem", Order = 5 )]
        public string DocaOrigem { get; set; }

        [JsonProperty(PropertyName = "fpp", Order = 6)]
        public string Ffp { get; set; }

        [JsonProperty(PropertyName = "mercadoria_id", Order = 7, Required = Required.Always)]
        public string IdentificadorMercadoria { get; set; }

        [JsonProperty(PropertyName = "id_localizador2_1", Order = 22)]
        public string IdLocalizadorDoisPrimeiro { get; set; }

        [JsonProperty(PropertyName = "id_localizador2_2", Order = 24)]
        public string IdLocalizadorDoisSegundo { get; set; }

        [JsonProperty(PropertyName = "id_localizador2_3", Order = 26)]
        public string IdLocalizadorDoisTerceiro { get; set; }

        [JsonProperty(PropertyName = "id_localizador3_1", Order = 28)]
        public string IdLocalizadorTresPrimeiro { get; set; }

        [JsonProperty(PropertyName = "id_localizador3_2", Order = 30)]
        public string IdLocalizadorTresSegundo { get; set; }

        [JsonProperty(PropertyName = "id_localizador3_3", Order = 32)]
        public string IdLocalizadorTresTerceiro { get; set; }

        [JsonProperty(PropertyName = "id_localizador1_1", Order = 16)]
        public string IdLocalizadorUmPrimeiro { get; set; }

        [JsonProperty(PropertyName = "id_localizador1_2", Order = 18)]
        public string IdLocalizadorUmSegundo { get; set; }

        [JsonProperty(PropertyName = "id_localizador1_3", Order = 20)]
        public string IdLocalizadorUmTerceiro { get; set; }

        [JsonProperty(PropertyName = "numero_cliente", Order = 13)]
        public string NumeroCliente { get; set; }

        [JsonProperty(PropertyName = "liberacao", Order = 12, Required = Required.Always)]
        public string Liberacao { get; set; }

        [JsonProperty(PropertyName = "localizador2_1", Order = 21)]
        public string LocalizadorDoisPrimeiro { get; set; }

        [JsonProperty(PropertyName = "localizador2_2", Order = 23)]
        public string LocalizadorDoisSegundo { get; set; }

        [JsonProperty(PropertyName = "localizador2_3", Order = 25)]
        public string LocalizadorDoisTerceiro { get; set; }

        [JsonProperty(PropertyName = "localizador3_1", Order = 27)]
        public string LocalizadorTresPrimeiro { get; set; }

        [JsonProperty(PropertyName = "localizador3_2", Order = 29)]
        public string LocalizadorTresSegundo { get; set; }

        [JsonProperty(PropertyName = "localizador3_3", Order = 31)]
        public string LocalizadorTresTerceiro { get; set; }

        [JsonProperty(PropertyName = "localizador1_1", Order = 15)]
        public string LocalizadorUmPrimeiro { get; set; }

        [JsonProperty(PropertyName = "localizador1_2", Order = 17)]
        public string LocalizadorUmSegundo { get; set; }

        [JsonProperty(PropertyName = "localizador1_3", Order = 19)]
        public string LocalizadorUmTerceiro { get; set; }

        [JsonProperty(PropertyName = "observacao", Order = 14)]
        public string Observacao { get; set; }

        [JsonProperty(PropertyName = "percurso", Order = 3, Required = Required.Always)]
        public string Percurso { get; set; }

        [JsonProperty(PropertyName = "inicio_previsto", Order = 10, Required = Required.Always)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-dd HH:mm:ss" })]
        public DateTime PrevisaoInicio { get; set; }

        [JsonProperty(PropertyName = "fim_previsto", Order = 11, Required = Required.Always)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-dd HH:mm:ss" })]
        public DateTime PrevisaoFim { get; set; }

        [JsonProperty(PropertyName = "rastreada", Order = 2, Required = Required.Always)]
        public string Rastreada { get; set; }

        [JsonProperty(PropertyName = "rota", Order = 9 )]
        public string Rota { get; set; }

        [JsonProperty(PropertyName = "tipo_cliente", Order = 4, Required = Required.Always)]
        public string TipoCliente { get; set; }

        [JsonProperty(PropertyName = "tipo_viagem", Order = 1, Required = Required.Always)]
        public string TipoViagem { get; set; }

        [JsonProperty(PropertyName = "valor", Order = 8, Required = Required.Always)]
        [JsonConverter(typeof(Utilidades.Json.DecimalConverter))]
        public decimal Valor { get; set; }
    }
}
