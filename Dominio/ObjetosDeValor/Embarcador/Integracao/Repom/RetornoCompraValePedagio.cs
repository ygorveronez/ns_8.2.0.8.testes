using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class RetornoCompraValePedagio
    {
        [JsonProperty(PropertyName = "result", Required = Required.AllowNull)]
        public RetornoIntegracaoResult Result { get; set; }

        [JsonProperty(PropertyName = "numeroEixosCavalo", Required = Required.AllowNull)]
        public int NumeroEixosCavalo { get; set; }

        [JsonProperty(PropertyName = "clienteCNPJ", Required = Required.DisallowNull)]
        public string ClienteCNPJ { get; set; }

        [JsonProperty(PropertyName = "filialCNPJ", Required = Required.DisallowNull)]
        public string FilialCNPJ { get; set; }

        [JsonProperty(PropertyName = "transportadorCPFCNPJ", Required = Required.DisallowNull)]
        public string TransportadorCPFCNPJ { get; set; }

        [JsonProperty(PropertyName = "motoristaCPF", Required = Required.DisallowNull)]
        public string MotoristaCPF { get; set; }

        [JsonProperty(PropertyName = "dataEmissao", Required = Required.AllowNull)]
        public DateTime DataEmissao { get; set; }

        [JsonProperty(PropertyName = "razaoSocialCliente", Required = Required.DisallowNull)]
        public string RazaoSocialCliente { get; set; }

        [JsonProperty(PropertyName = "statusCarga", Required = Required.DisallowNull)]
        public string StatusCarga { get; set; }

        [JsonProperty(PropertyName = "viagemAtiva", Required = Required.AllowNull)]
        public bool ViagemAtiva { get; set; }

        [JsonProperty(PropertyName = "codigoViagem", Required = Required.AllowNull)]
        public int CodigoViagem { get; set; }

        [JsonProperty(PropertyName = "veiculo", Required = Required.DisallowNull)]
        public string Veiculo { get; set; }

        [JsonProperty(PropertyName = "tipoEmissao", Required = Required.DisallowNull)]
        public string TipoEmissao { get; set; }

        [JsonProperty(PropertyName = "valor", Required = Required.AllowNull)]
        public decimal Valor { get; set; }

        [JsonProperty(PropertyName = "cartao", Required = Required.DisallowNull)]
        public string Cartao { get; set; }

        [JsonProperty(PropertyName = "tag", Required = Required.DisallowNull)]
        public string Tag { get; set; }

        [JsonProperty(PropertyName = "pracas", Required = Required.DisallowNull)]
        public List<RetornoCompraValePedagioPraca> Pracas { get; set; }

        [JsonProperty(PropertyName = "idVpoAntt", Required = Required.DisallowNull)]
        public List<string> idVpoAntt { get; set; }
    }
}
