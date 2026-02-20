using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class RetornoMontarCargaCorpo
    {
        [JsonProperty(PropertyName = "identificador_rota", Required = Required.Default)]
        public RetornoMontarCargaIdentificadoRota ProtocoloDaCarga { get; set; }
    }
}
