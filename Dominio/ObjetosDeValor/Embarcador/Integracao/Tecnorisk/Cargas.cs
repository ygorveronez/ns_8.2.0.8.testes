using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk
{
    public class Cargas
    {
        [JsonProperty("carga_valor")]
        public decimal Valor { get; set; }
    }
}
