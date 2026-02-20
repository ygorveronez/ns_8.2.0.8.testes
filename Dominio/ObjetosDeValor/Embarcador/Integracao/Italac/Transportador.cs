using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Italac
{
    public class Transportador
    {
        [JsonProperty(PropertyName = "cpfCnpj")]
        public string CPFCNPJ { get; set; }
    }
}
