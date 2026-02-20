using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class RetornoMovimentacaoCartaoConteudoBilhete
    {
        [JsonProperty(PropertyName = "idBilhete", Required = Required.AllowNull)]
        public string IdBilhete { get; set; }

        [JsonProperty(PropertyName = "status", Required = Required.AllowNull)]
        public string Status { get; set; }
    }
}
