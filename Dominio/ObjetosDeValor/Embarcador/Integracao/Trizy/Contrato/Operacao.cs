using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Operacao
    {
        [JsonProperty(PropertyName = "descricao", Required = Required.Default)]
        public string Descricao { get; set; }

        [JsonProperty(PropertyName = "tipo", Required = Required.Default)]
        public string Tipo { get; set; }
    }
}
