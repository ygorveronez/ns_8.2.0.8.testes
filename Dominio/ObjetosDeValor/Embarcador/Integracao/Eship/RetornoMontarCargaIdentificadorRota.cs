using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class RetornoMontarCargaIdentificadoRota
    {
        [JsonProperty(PropertyName = "dados", Required = Required.Default)]
        public RetornoMontarCargaIdentificadorRotaDados Dados { get; set; }

    }
}
