using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Mars
{
    public class Documento
    {
        [JsonProperty("number")]
        public string NumeroCTe { get; set; }

        [JsonProperty("series")]
        public string Serie { get; set; }

        [JsonProperty("protocoloCTe")]
        public string ProtocoloCTe { get; set; }

        [JsonProperty("issuer")]
        public Participante Emitente { get; set; }

        [JsonProperty("cteType")]
        public string TipoCTe { get; set; }

        [JsonProperty("modal")]
        public string Modal { get; set; }

        [JsonProperty("model")]
        public string Modelo { get; set; }

        [JsonProperty("issueDateTime")]
        public string DataHoraEmissao { get; set; }

        [JsonProperty("accessKey")]
        public string ChaveAcesso { get; set; }

        [JsonProperty("authorizationProtocol")]
        public string ProtocoloAutorizacao { get; set; }

        [JsonProperty("cfop")]
        public string Cfop { get; set; }

        [JsonProperty("serviceStart")]
        public string InicioViagem { get; set; }

        [JsonProperty("serviceEnd")]
        public string FimViagem { get; set; }

        [JsonProperty("sender")]
        public Participante Remetente { get; set; }

        [JsonProperty("recipient")]
        public Participante Destinatario { get; set; }

        [JsonProperty("predominantProduct")]
        public string ProdutoPredominante { get; set; }

        [JsonProperty("totalCargoValue")]
        public string ValorTotalCarga { get; set; }

        [JsonProperty("grossWeight")]
        public string PesoBruto { get; set; }

        [JsonProperty("totalServiceValue")]
        public string ValorTotalServico { get; set; }

        [JsonProperty("tolls")]
        public List<Pedagio> Pedagio { get; set; }

        [JsonProperty("serviceValueComponents")]
        public ComponentesValorServico ComponentesValorServico { get; set; }

        [JsonProperty("taxInformation")]
        public InformacoesFiscais InformacoesFiscais { get; set; }

        [JsonProperty("originDocuments")]
        public List<DocumentoOrigem> DocumentosOrigem { get; set; }
    }
}
