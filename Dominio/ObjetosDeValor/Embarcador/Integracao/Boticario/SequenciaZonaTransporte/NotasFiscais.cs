using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario.SequenciaZonaTransporte
{
    public class NotasFiscais
    {
        [JsonProperty(PropertyName = "chaveNFe", Order = 1, Required = Required.Default)]
        public string ChaveNFe { get; set; }
    }
}
