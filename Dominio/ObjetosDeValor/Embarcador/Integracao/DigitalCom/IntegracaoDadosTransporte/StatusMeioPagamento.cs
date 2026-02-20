using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.IntegracaoDadosTransporte;

public sealed class StatusMeioPagamento
{
    [JsonProperty("id")]
    public int Codigo { get; set; }

    [JsonProperty("messageKey")]
    public string Mensagem { get; set; }

    [JsonProperty("tipoMensagem")]
    public string TipoMensagem { get; set; }

    [JsonProperty("descricao")]
    public string Descricao { get; set; }
}

