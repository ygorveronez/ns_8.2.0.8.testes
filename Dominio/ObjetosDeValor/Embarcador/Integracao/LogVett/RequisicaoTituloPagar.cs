using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Logvett
{
    public class RequisicaoTituloPagar
    {
        [JsonProperty(PropertyName = "CNPJIPALOG")]
        public string CnpjIpaLog { get; set; }

        [JsonProperty(PropertyName = "CNPJAutonomo")]
        public string CnpjAutonomo { get; set; }

        [JsonProperty(PropertyName = "TituloPrefixo")]
        public string PrefixoTitulo { get; set; }

        [JsonProperty(PropertyName = "TituloNumero")]
        public string NumeroTitulo { get; set; }

        [JsonProperty(PropertyName = "TituloTipo")] //PA = Adiantamento, NF = valor total do título
        public string TipoTitulo { get; set; }

        [JsonProperty(PropertyName = "TituloParcela")]
        public string ParcelaTitulo { get; set; }

        [JsonProperty(PropertyName = "TituloEmissao")] //aaaa-mm-dd
        public string DataEmissaoTitulo { get; set; }

        [JsonProperty(PropertyName = "TituloVencimento")] //aaaa-mm-dd
        public string DataVencimentoTitulo { get; set; }

        [JsonProperty(PropertyName = "Historico")]
        public string Historico { get; set; }

        [JsonProperty(PropertyName = "TituloValor")] //2 casas decimais
        public decimal ValorTitulo { get; set; }
    }
}
