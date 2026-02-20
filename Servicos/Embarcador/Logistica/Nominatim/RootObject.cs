using Newtonsoft.Json;

namespace Servicos.Embarcador.Logistica.Nominatim
{
    public class RootObject
    {
        public RootObject() { this.address = new Address(); }
        
        public string place_id { get; set; }
        
        public string licence { get; set; }
        
        public string osm_type { get; set; }
        
        public string osm_id { get; set; }
        
        public string[] boundingbox { get; set; }
        
        public string lat { get; set; }
        
        public string lon { get; set; }
        
        public string display_name { get; set; }

        [JsonProperty(PropertyName = "class")]
        public string classe { get; set; }
        
        public string type { get; set; }
        
        public decimal importance { get; set; }
        
        public Address address { get; set; }
    }
}
