using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Obramax
{
    public class Transportadora
    {
        [JsonProperty("CodigoIntegracao")]
        public string CodigoIntegracao { get; set; }

        [JsonProperty("RazaoSocial")]
        public string RazaoSocial { get; set; }
        
    }
}
