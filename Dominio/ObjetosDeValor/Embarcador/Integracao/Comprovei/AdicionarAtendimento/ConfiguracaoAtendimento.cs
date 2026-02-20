using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento
{
    public class ConfiguracaoAtendimento
    {
        [JsonProperty("nfe_key")]
        public string ChaveNFe { get; set; }

        [JsonProperty("success")]
        public bool Sucesso { get; set; }

        [JsonProperty("description")]
        public string Descricao { get; set; }
    }
}
