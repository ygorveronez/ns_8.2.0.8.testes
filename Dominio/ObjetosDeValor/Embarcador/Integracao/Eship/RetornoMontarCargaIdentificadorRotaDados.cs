using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class RetornoMontarCargaIdentificadorRotaDados
    {
        [JsonProperty(PropertyName = "idEmbarque", Required = Required.Default)]
        public string IdEmbarque { get; set; }

        [JsonProperty(PropertyName = "idOrdem", Required = Required.Default)]
        public string IdOrdem { get; set; }

    }
}
