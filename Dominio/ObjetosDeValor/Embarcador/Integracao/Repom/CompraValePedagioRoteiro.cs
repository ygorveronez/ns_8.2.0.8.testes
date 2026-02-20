using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class CompraValePedagioRoteiro
    {
        [JsonProperty(PropertyName = "roteiroCodigo", Required = Required.AllowNull)]
        public int RoteiroCodigo { get; set; }

        [JsonProperty(PropertyName = "percursoCodigo", Required = Required.AllowNull)]
        public int PercursoCodigo { get; set; }
    }
}
