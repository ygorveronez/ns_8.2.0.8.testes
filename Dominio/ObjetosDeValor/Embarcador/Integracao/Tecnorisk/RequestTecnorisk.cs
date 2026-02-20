using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk
{
    public class RequestTecnorisk
    {
        [JsonProperty("data_hora_atual")]
        public string DataHoraAtual { get; set; }

        [JsonProperty("pgr_id")]
        public int PGRId { get; set; }

        [JsonProperty("unidade_transporte_id")]
        public int UnidadeTransporteId { get; set; }

        [JsonProperty("veiculos")]
        public List<Veiculos> Veiculos { get; set; }

        [JsonProperty("motorista_cpf")]
        public string CPF { get; set; }

        [JsonProperty("motorista_telefone")]
        public string Telefone { get; set; }

        [JsonProperty("monitoramento_prioridade")]
        public int MonitoramentoPrioridade { get; set; }

        [JsonProperty("carga_mercadoria")]
        public int CargaMercadoria { get; set; }

        [JsonProperty("cargas")]
        public List<Cargas> Cargas { get; set; }

        [JsonProperty("viagem_origem_data_saida")]
        public string DataSaidaVeiculo { get; set; }

       [JsonProperty("viagem_origem_hora_saida")]
        public string HoraSaidaVeiculo { get; set; }

        [JsonProperty("viagem_origem_estado")]
        public string EstadoOrigemViagem { get; set; }

        [JsonProperty("viagem_origem_cidade")]
        public string CidadeOrigemViagem { get; set; }

        [JsonProperty("viagem_destino_estado")]
        public string EstadoDestinoViagem { get; set; }

        [JsonProperty("viagem_destino_cidade")]
        public string CidadeDestinoViagem { get; set; }

        [JsonProperty("viagem_destino_data_chegada")]
        public string DataChegadaDestino { get; set; }

        [JsonProperty("viagem_destino_hora_chegada")]
        public string HoraChegadaDestino { get; set; }

         [JsonProperty("viagem_rota")]
        public string IDRotaViagem { get; set; }
    }
}
