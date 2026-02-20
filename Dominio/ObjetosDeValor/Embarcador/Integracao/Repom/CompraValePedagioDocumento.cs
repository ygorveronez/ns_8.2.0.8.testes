using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class CompraValePedagioDocumento
    {
        [JsonProperty(PropertyName = "documentoCodigo", Required = Required.AllowNull)]
        public string DocumentoCodigo { get; set; }

        [JsonProperty(PropertyName = "serie", Required = Required.AllowNull)]
        public string Serie { get; set; }
    }
}
