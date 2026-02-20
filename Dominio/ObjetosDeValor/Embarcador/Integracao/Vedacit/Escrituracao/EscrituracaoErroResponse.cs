using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.Escrituracao;

public class EscrituracaoErroResponse
{
    [JsonProperty("error")]
    public EscrituracaoErro Erro { get; set; }
}

public class EscrituracaoErro
{
    [JsonProperty("code")]
    public int Status { get; set; }

    [JsonProperty("reason")]
    public string Motivo { get; set; }

    [JsonProperty("message")]
    public string Mensagem { get; set; }
}
