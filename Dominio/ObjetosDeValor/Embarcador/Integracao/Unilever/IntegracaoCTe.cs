using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class IntegracaoCTe
    {
        [JsonProperty(PropertyName = "DOCUMENT", Required = Required.Always)]
        public IntegracaoCTeDocumento Documento { get; set; }
    }
}
