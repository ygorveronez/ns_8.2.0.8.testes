using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.Escrituracao;

public class EscrituracaoResponse
{
    [JsonProperty("fase")]
    public string Fase { get; set; }

    [JsonProperty("status")]
    public bool Status { get; set; }

    [JsonProperty("mensagem")]
    public string Mensagem { get; set; }
}
