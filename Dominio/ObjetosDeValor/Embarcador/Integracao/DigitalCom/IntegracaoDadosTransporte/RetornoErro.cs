using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.IntegracaoDadosTransporte;

public sealed class RetornoErro
{
    [JsonProperty("timestamp")]
    public string Data { get; set; }

    [JsonProperty("message")]
    public string Mensagem { get; set; }

    [JsonProperty("details")]
    public string Detalhes { get; set; }
}
