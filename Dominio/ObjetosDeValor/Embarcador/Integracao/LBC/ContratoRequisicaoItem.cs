using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.LBC
{
    public sealed class ContratoRequisicaoItem
    {
        [JsonProperty("extContractId")]
        public string IDContratoExterno { get; set; }

        [JsonProperty("reqAdhocApproval")]
        public string AprovacaoAdicional { get; set; }

        [JsonProperty("name")]
        public string NomeContrato { get; set; }

        [JsonProperty("direct")]
        public string ProcessoAprovacao { get; set; }

        [JsonProperty("cluster")]
        public string Cluster { get; set; }

        [JsonProperty("country")]
        public string Pais { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("team")]
        public string Equipe { get; set; }

        [JsonProperty("portfolio")]
        public string[] Categoria { get; set; }

        [JsonProperty("subPortfolio")]
        public string[] SubCategoria { get; set; }

        [JsonProperty("mode")]
        public List<string> ModoContrato { get; set; }

        [JsonProperty("org")]
        public string RazaoSocial { get; set; }

        [JsonProperty("isSupplierRScompliant")]
        public string ConformidadeComRSP { get; set; }

        [JsonProperty("defSap")]
        public string CodigoIntegracaoTransportador { get; set; }

        [JsonProperty("lEntity")]
        public string PessoaJuridica { get; set; }

        [JsonProperty("ctrtType")]
        public string TipoContrato { get; set; }

        [JsonProperty("hubType")]
        public string HubNonHub { get; set; }

        [JsonProperty("otmDomain")]
        public string DominioOTM { get; set; }

        [JsonProperty("effectDate")]
        public string DataInicial { get; set; }

        [JsonProperty("expireDate")]
        public string DataFinal { get; set; }

        [JsonProperty("revExpireDate")]
        public string DataFinalRevisao { get; set; }

        [JsonProperty("currency")]
        public string Moeda { get; set; }

        [JsonProperty("value")]
        public string ValorContrato { get; set; }

        [JsonProperty("revisedValue")]
        public string ValorRevisao { get; set; }

        [JsonProperty("strategic")]
        public string Strategic { get; set; }

        [JsonProperty("payTerms")]
        public string TermosPagamento { get; set; }

        [JsonProperty("hasPenalty")]
        public string ClausulaPenal { get; set; }

        [JsonProperty("comments")]
        public string Observacao { get; set; }

        [JsonProperty("clntContact")]
        public string UsuarioContrato { get; set; }

    }
}
