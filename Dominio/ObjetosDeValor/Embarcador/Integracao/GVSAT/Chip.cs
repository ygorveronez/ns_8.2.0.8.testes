using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT
{
    [JsonObject(ItemNullValueHandling=NullValueHandling.Ignore)]
    public class Chip
    {
        public long id { get; set; }
    }
}
