using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto
{
    public class EntidadeFornecedor
    {
        [JsonProperty("codigoEntidade")]
        public string CodigoEntidade { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("tipoPessoa")]
        public string TipoPessoa { get; set; }

        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }

        [JsonProperty("fone1")]
        public string Fone1 { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("cdFilial")]
        public int CdFilial { get; set; }
    }
}
