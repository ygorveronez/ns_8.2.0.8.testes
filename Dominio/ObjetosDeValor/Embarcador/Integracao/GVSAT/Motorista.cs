using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT
{
    [JsonObject(ItemNullValueHandling=NullValueHandling.Ignore)]
    public class Motorista
    {
        public long id { get; set; }
    }
}
