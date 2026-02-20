using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class RetornoMontarCargaErros
    {
        [JsonProperty(PropertyName = "erro", Required = Required.Default)]
        public RetornoMontarCargaErrosErro Erro { get; set; }
    }
}
