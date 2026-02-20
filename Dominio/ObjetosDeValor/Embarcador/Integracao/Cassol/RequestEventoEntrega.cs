using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Cassol
{
    public class RequestEventoEntrega
    {
        [JsonProperty("pedven_numpedven")]
        public long PedvenNumPedven { get; set; }

        [JsonProperty("id_tms_carga")]
        public long IdTmsCarga { get; set; }

        [JsonProperty("tms_carga_centerpoint")]
        public bool TmsCargaCenterpoint { get; set; }

        [JsonProperty("tms_carga_agrupado")]
        public bool TmsCargaAgrupada { get; set; }

        [JsonProperty("filial_saida")]
        public long FilialSaida { get; set; }

        [JsonProperty("endereco_codcli")]
        public long EnderecoCodCli { get; set; }

        [JsonProperty("endereco_codend")]
        public long EnderecoCodEnd { get; set; }

        [JsonProperty("tp_pendencia")]
        public string TpPendencia { get; set; }

        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        [JsonProperty("dthr_criacao")]
        public string DthrCriacao { get; set; }
    }
}
