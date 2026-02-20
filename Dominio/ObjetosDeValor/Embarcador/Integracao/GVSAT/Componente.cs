using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GVSAT
{
    [JsonObject(ItemNullValueHandling=NullValueHandling.Ignore)]
    public class Componente
    {
        public long id { get; set; }
        public string nome { get; set; }
        public string valor { get; set; }
    }
}
