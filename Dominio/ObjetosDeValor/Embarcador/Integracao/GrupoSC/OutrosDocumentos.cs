using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GrupoSC
{
    public class OutrosDocumentos
    {
        [JsonProperty("dataEmissao")]
        public string DataEmissao { get; set; }

        [JsonProperty("numeroND")]
        public string NumeroND { get; set; }

        [JsonProperty("serieND")]
        public string SerieND { get; set; }

        [JsonProperty("protocoloND")]
        public string ProtocoloND { get; set; }

        [JsonProperty("transportador")]
        public string Transportador { get; set; }

        [JsonProperty("ocorrenciaMulti")]
        public string OcorrenciaMulti { get; set; }

        [JsonProperty("carga")]
        public string Carga { get; set; }

        [JsonProperty("protocolocargaTMS")]
        public string ProtocoloCargaTMS { get; set; }

        [JsonProperty("motivo")]
        public string Motivo { get; set; }

        [JsonProperty("descricaomotivo")]
        public string DescricaoMotivo { get; set; }

        [JsonProperty("problema")]
        public string Problema { get; set; }

        [JsonProperty("cteOrigem")]
        public string CTeOrigem { get; set; }

        [JsonProperty("serieOrigem")]
        public string SerieOrigem { get; set; }

        [JsonProperty("nfeOrigem")]
        public string NFeOrigem { get; set; }

        [JsonProperty("valorDocumento")]
        public string ValorDocumento { get; set; }

        [JsonProperty("qtdParcela")]
        public string QuantidadeParcela { get; set; }

        [JsonProperty("periodo_pagamento")]
        public string PeriodoPagamento { get; set; }
    }
}