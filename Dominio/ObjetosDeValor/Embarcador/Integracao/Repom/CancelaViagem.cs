using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class CancelaViagem
    {
        [JsonProperty(PropertyName = "viagemCodigo", Required = Required.Always)]
        public string ViagemCodigo { get; set; }

        [JsonProperty(PropertyName = "usuario", Required = Required.AllowNull)]
        public string Usuario { get; set; }
    }
}
