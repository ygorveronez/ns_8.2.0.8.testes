using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Senior;

public class RetornoIntegracaoSenior
{
    [JsonProperty("message")]
    public string Mensagem { get; set; } = string.Empty;

    [JsonProperty("sucesso")]
    public bool sucesso { get; set; } = false;

    [JsonProperty("error")]
    public bool? Erro { get; set; } = false;

    [JsonProperty("status")]
    public int? Status { get; set; } = 0;

    [JsonProperty("statusCode")]
    public bool SituacaoValida { get; set; } = false;

    [JsonIgnore]
    public bool PossuiErro => !sucesso;
}
