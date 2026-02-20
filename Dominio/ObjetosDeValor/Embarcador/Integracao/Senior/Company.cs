using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior
{
    public class Company
    {
        [JsonProperty(PropertyName = "code", Required = Required.Default)]
        public int Codigo { get; set; }

        [JsonProperty(PropertyName = "collaborator", Required = Required.Default)]
        public Colaborador Colaborador { get; set; }

        [JsonProperty(PropertyName = "gateCode", Required = Required.Default)]
        public int CodigoPortao { get; set; }
    }
}
