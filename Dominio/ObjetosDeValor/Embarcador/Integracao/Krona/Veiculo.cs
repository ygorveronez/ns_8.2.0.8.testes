using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Krona
{
    public sealed class Veiculo
    {
        [JsonProperty(PropertyName = "ano", Order = 6, Required = Required.Always)]
        public string Ano { get; set; }

        [JsonProperty(PropertyName = "capacidade", Order = 8)]
        public string Capacidade { get; set; }

        [JsonProperty(PropertyName = "comunicacao", Order = 24, Required = Required.Always)]
        public string Comunicacao { get; set; }

        [JsonProperty(PropertyName = "comunicacao_sec", Order = 27)]
        public string ComunicacaoSecundaria { get; set; }

        [JsonProperty(PropertyName = "cor", Order = 5, Required = Required.Always)]
        public string Cor { get; set; }

        [JsonProperty(PropertyName = "end_bairro", Order = 18, Required = Required.Always)]
        public string EnderecoBairro { get; set; }

        [JsonProperty(PropertyName = "end_cep", Order = 21, Required = Required.Always)]
        public string EnderecoCep { get; set; }

        [JsonProperty(PropertyName = "end_cidade", Order = 19, Required = Required.Always)]
        public string EnderecoCidade { get; set; }

        [JsonProperty(PropertyName = "end_complemento", Order = 17)]
        public string EnderecoComplemento { get; set; }

        [JsonProperty(PropertyName = "end_numero", Order = 16, Required = Required.Always)]
        public string EnderecoNumero { get; set; }

        [JsonProperty(PropertyName = "end_rua", Order = 15, Required = Required.Always)]
        public string EnderecoRua { get; set; }

        [JsonProperty(PropertyName = "end_uf", Order = 20, Required = Required.Always)]
        public string EnderecoUf { get; set; }

        [JsonProperty(PropertyName = "fixo", Order = 28)]
        public string Fixo { get; set; }

        [JsonProperty(PropertyName = "numero_frota", Order = 11)]
        public string FrotaNumero { get; set; }

        [JsonProperty(PropertyName = "transp_frota", Order = 12)]
        public string FrotaTransportador { get; set; }

        [JsonProperty(PropertyName = "id_rastreador", Order = 23, Required = Required.Always)]
        public string IdRastreador { get; set; }

        [JsonProperty(PropertyName = "id_rastreador_sec", Order = 26)]
        public string IdRastreadorSecundario { get; set; }

        [JsonProperty(PropertyName = "marca", Order = 3, Required = Required.Always)]
        public string Marca { get; set; }

        [JsonProperty(PropertyName = "modelo", Order = 4, Required = Required.Always)]
        public string Modelo { get; set; }

        [JsonProperty(PropertyName = "numero_antt", Order = 9)]
        public string NumeroAntt { get; set; }

        [JsonProperty(PropertyName = "placa", Order = 1, Required = Required.Always)]
        public string Placa { get; set; }

        [JsonProperty(PropertyName = "proprietario", Order = 13)]
        public string Proprietario { get; set; }

        [JsonProperty(PropertyName = "proprietario_cpfcnpj", Order = 14)]
        public string ProprietarioCpfCnpj { get; set; }

        [JsonProperty(PropertyName = "renavan", Order = 2, Required = Required.Always)]
        public string Renavan { get; set; }

        [JsonProperty(PropertyName = "tecnologia", Order = 22, Required = Required.Always)]
        public string Tecnologia { get; set; }

        [JsonProperty(PropertyName = "tecnologia_sec", Order = 25)]
        public string TecnologiaSecundaria { get; set; }

        [JsonProperty(PropertyName = "tipo", Order = 7, Required = Required.Always)]
        public string Tipo { get; set; }

        [JsonProperty(PropertyName = "validade_antt", Order = 10)]
        [JsonConverter(typeof(Utilidades.Json.DateTimeConverter), new object[] { "yyyy-MM-dd HH:mm:ss", true })]
        public DateTime? ValidadeAntt { get; set; }
    }
}
