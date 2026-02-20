using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Boticario
{
    public sealed class IntegracaoCTe
    {
        [JsonProperty(PropertyName = "chaveEletronicaXml", Order = 1, Required = Required.Always)]
        public string ChaveCTe { get; set; }

        [JsonProperty(PropertyName = "numero", Order = 2, Required = Required.Always)]
        public string Numero { get; set; }

        [JsonProperty(PropertyName = "protocolo", Order = 3, Required = Required.Always)]
        public int Protocolo { get; set; }

        [JsonProperty(PropertyName = "tipoProcesso", Order = 4, Required = Required.Always)]
        public string TipoProcesso { get; set; }

        [JsonProperty(PropertyName = "centroCusto", Order = 5, Required = Required.Always)]
        public CentroCusto CentroCusto { get; set; }

        [JsonProperty(PropertyName = "pep", Order = 6, Required = Required.Always)]
        public PEP PEP { get; set; }

        [JsonProperty(PropertyName = "transporte", Order = 7, Required = Required.Always)]
        public Transporte Transporte { get; set; }

        [JsonProperty(PropertyName = "triangulacoes", Order = 8, Required = Required.Default)]
        public List<Triangulacao> Triangulacoes { get; set; }
    }
}
