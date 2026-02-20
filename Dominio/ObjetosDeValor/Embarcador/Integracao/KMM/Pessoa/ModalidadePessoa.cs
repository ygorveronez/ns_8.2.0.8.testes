using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class ModalidadePessoa
    {
        [JsonProperty(PropertyName = "operation", Required = Required.Always)]
        public string Operation { get; set; }

        [JsonProperty(PropertyName = "modalidade", Required = Required.Always)]
        public int NumModalidade { get; set; }

        [JsonProperty(PropertyName = "ativo", Required = Required.Always)]
        public string Ativo { get; set; }
    }
}
