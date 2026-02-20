using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT
{
    [JsonObject(ItemNullValueHandling=NullValueHandling.Ignore)]
    public class Grupo
    {
        public long id { get; set; }
    }
}
