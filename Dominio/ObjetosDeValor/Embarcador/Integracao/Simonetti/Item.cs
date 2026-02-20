using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Simonetti
{
    public class Item
    {
        [JsonProperty("codigo_ocorrencia")]
        public string CodigoOcorrencia;

        [JsonProperty("descricao_ocorrencia")]
        public string DescricaoOcorrencia;

        [JsonProperty("data_hora_agendamento")]
        public string DataHoraAgendamento;

        [JsonProperty("observacao")]
        public string Observacao;

        [JsonProperty("codigo_produto")]
        public string CodigoProduto;

        [JsonProperty("placa1")]
        public string PlacaVeiculo1;

        [JsonProperty("cpf_motorista1")]
        public string CPFMotorista1;

        [JsonProperty("placa2")]
        public string PlacaVeiculo2;

        [JsonProperty("cpf_motorista2")]
        public string CPFMotorista2;

        [JsonProperty("placa3")]
        public string PlacaVeiculo3;

        [JsonProperty("cpf_motorista3")]
        public string CPFMotorista3;

        [JsonProperty("placa4")]
        public string PlacaVeiculo4;

        [JsonProperty("cpf_motorista4")]
        public string CPFMotorista4;
    }
}
