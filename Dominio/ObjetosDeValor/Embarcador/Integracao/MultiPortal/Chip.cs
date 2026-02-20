using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.MultiPortal
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Chip
    {
        public long id { get; set; }
    }

}

