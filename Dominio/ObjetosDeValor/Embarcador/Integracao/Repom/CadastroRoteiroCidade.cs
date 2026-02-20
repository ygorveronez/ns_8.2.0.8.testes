using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class CadastroRoteiroCidade
    {
        [JsonProperty(PropertyName = "codigoIBGE", Required = Required.Always)]
        public string CodigoIBGE { get; set; }

        [JsonProperty(PropertyName = "estado", Required = Required.Always)]
        public string Estado { get; set; }
    }
}
