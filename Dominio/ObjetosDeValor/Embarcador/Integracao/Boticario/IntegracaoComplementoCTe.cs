using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario
{
    public sealed class IntegracaoComplementoCTe
    {
        [JsonProperty(PropertyName = "chaveEletronicaXml", Order = 1, Required = Required.Always)]
        public string ChaveCTe { get; set; }

        [JsonProperty(PropertyName = "chaveEletronicaXmlReferencia", Order = 2, Required = Required.Always)]
        public string ChaveCTeComplementado { get; set; }

        [JsonProperty(PropertyName = "numero", Order = 2, Required = Required.Always)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "protocolo", Order = 3, Required = Required.Always)]
        public int Protocolo { get; set; }

        [JsonProperty(PropertyName = "tipoProcesso", Order = 4, Required = Required.Always)]
        public string TipoProcesso { get; set; }

        [JsonProperty(PropertyName = "transporte", Order = 7, Required = Required.Always)]
        public Transporte Transporte { get; set; }
    }
}
