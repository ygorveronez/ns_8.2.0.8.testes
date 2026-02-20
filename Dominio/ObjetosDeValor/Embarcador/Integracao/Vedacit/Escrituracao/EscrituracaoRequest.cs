using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.Escrituracao;

public class EscrituracaoRequest
{
    [JsonProperty(PropertyName = "codigoAcao")]
    public string CodigoAcao { get; set; }

    [JsonProperty(PropertyName = "inputXML")]
    public string InputXML { get; set; }

    [JsonProperty(PropertyName = "tipoNFe")]
    public string TipoNFe { get; set; }

    [JsonProperty(PropertyName = "protocolo")]
    public string Protocolo { get; set; }
}
