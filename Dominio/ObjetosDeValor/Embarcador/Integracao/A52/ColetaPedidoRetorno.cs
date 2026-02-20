using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52
{
    public class ColetaPedidoRetorno
    {
        [JsonProperty(PropertyName = "cd_carga", Required = Required.Default)]
        public string Carga { get; set; }
    }
}