using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.IntegracaoDadosTransporte;

public sealed class RetornoIntegracao
{
    [JsonProperty("status")]
    public StatusMeioPagamento Status { get; set; }

    [JsonProperty("placaVeiculo")]
    public string PlacaVeiculo { get; set; }

    [JsonProperty("nroEixo")]
    public string NumeroEixo { get; set; }

    [JsonProperty("tag")]
    public string Tag { get; set; }

    [JsonProperty("meansOfPayment")]
    public string MeioPagamento { get; set; }
}
