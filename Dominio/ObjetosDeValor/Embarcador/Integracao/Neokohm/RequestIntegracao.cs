using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Neokohm
{
    public class RequestIntegracao
    {
        [JsonProperty("data_final")]
        public string DataFinal { get; set; }

        [JsonProperty("data_inicial")]
        public string DataInicial { get; set; }

        [JsonProperty("inicio_carga")]
        public bool InicioCarga { get; set; }

        [JsonProperty("fim_carga")]
        public bool FimCarga { get; set; }

        [JsonProperty("placa")]
        public string Placa { get; set; }

        [JsonProperty("numcarga")]
        public string NumeroCarga { get; set; }

        [JsonProperty("tempminima")]
        public decimal TemperaturaMinimaCarga { get; set; }

        [JsonProperty("tempmaxima")]
        public decimal TemperaturaMaximaCarga { get; set; }

        [JsonProperty("temperatura_cet")]
        public string TemperaturaIdeal { get; set; }

        [JsonProperty("seca")]
        public bool Seca { get; set; }

        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        [JsonProperty("origens")]
        public List<Origens> Origens { get; set; }

        [JsonProperty("destinos")]
        public List<Destinos> Destinos { get; set; }

    }
}
