using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Motorista
    {
        public long id { get; set; }
    }
}
