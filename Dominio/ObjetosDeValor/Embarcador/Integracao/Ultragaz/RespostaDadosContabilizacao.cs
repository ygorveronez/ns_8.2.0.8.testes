using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ultragaz
{
    public class RespostaDadosContabilizacao
    {
        [JsonProperty(PropertyName = "fault", Required = Required.Default)]
        public RespostaDadosContabilizacaoErro Erro { get; set; }
    }
}
